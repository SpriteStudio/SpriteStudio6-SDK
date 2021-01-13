//
//  main.cpp
//  Converter
//
//  Created by Hiroki Azumada on 2014/01/07.
//  Copyright (c) 2014年 Hiroki Azumada. All rights reserved.
//

// TODO
#include "ssloader.h"
#include "ssplayer_animedecode.h"
#include "ssHelper.h"
#include "sshTextureBMP.h"
#include "ssplayer_mesh.h"

#include "sscharconverter.h"

#include <string>
#include <algorithm>
#include <iostream>
#include <fstream>
#include <iostream>

#ifndef _WIN32
#include <sys/stat.h>
#endif

#include "Lump.h"
#include "LumpExporter.h"
#include "FileUtil.h"
#include "SsPlayerConverter.h"


#ifdef _BACKBUFFER_RENDERING__
	#include "BackGroudRender.h"
#endif



static const int DATA_VERSION_1			= 1;
static const int DATA_VERSION_2         = 2;
static const int DATA_VERSION_3         = 3;
static const int DATA_VERSION_4			= 4;
static const int DATA_VERSION_5			= 5;
static const int DATA_VERSION_6			= 6;
static const int DATA_VERSION_7			= 7;
static const int DATA_VERSION_8			= 8;
static const int DATA_VERSION_9			= 9;
static const int DATA_VERSION_10		= 10;
static const int DATA_VERSION_11		= 11;

static const int DATA_ID				= 0x42505353;
static const int CURRENT_DATA_VERSION	= DATA_VERSION_11;


enum {
	PART_FLAG_INVISIBLE			= 1 << 0,
	PART_FLAG_FLIP_H			= 1 << 1,
	PART_FLAG_FLIP_V			= 1 << 2,

	// optional parameter flags
	PART_FLAG_CELL_INDEX		= 1 << 3,
	PART_FLAG_POSITION_X		= 1 << 4,
	PART_FLAG_POSITION_Y		= 1 << 5,
	PART_FLAG_POSITION_Z		= 1 << 6,
	PART_FLAG_PIVOT_X			= 1 << 7,
	PART_FLAG_PIVOT_Y           = 1 << 8,
	PART_FLAG_ROTATIONX			= 1 << 9,
	PART_FLAG_ROTATIONY			= 1 << 10,
	PART_FLAG_ROTATIONZ			= 1 << 11,
	PART_FLAG_SCALE_X			= 1 << 12,
	PART_FLAG_SCALE_Y			= 1 << 13,
	PART_FLAG_LOCALSCALE_X		= 1 << 14,
	PART_FLAG_LOCALSCALE_Y		= 1 << 15,
	PART_FLAG_OPACITY			= 1 << 16,
	PART_FLAG_LOCALOPACITY		= 1 << 17,
	PART_FLAG_PARTS_COLOR		= 1 << 18,
	PART_FLAG_VERTEX_TRANSFORM	= 1 << 19,

	PART_FLAG_SIZE_X			= 1 << 20,
	PART_FLAG_SIZE_Y			= 1 << 21,

	PART_FLAG_U_MOVE			= 1 << 22,
	PART_FLAG_V_MOVE			= 1 << 23,
	PART_FLAG_UV_ROTATION		= 1 << 24,
	PART_FLAG_U_SCALE			= 1 << 25,
	PART_FLAG_V_SCALE			= 1 << 26,
	PART_FLAG_BOUNDINGRADIUS	= 1 << 27,

	PART_FLAG_MASK				= 1 << 28,
	PART_FLAG_PRIORITY			= 1 << 29,

	PART_FLAG_INSTANCE_KEYFRAME	= 1 << 30,
	PART_FLAG_EFFECT_KEYFRAME   = 1 << 31,


	NUM_PART_FLAGS
};

enum {
	PART_FLAG_MESHDATA			= 1 << 0,
	NUM_PART_FLAGS2
};

enum {
	VERTEX_FLAG_LT		= 1 << 0,
	VERTEX_FLAG_RT		= 1 << 1,
	VERTEX_FLAG_LB		= 1 << 2,
	VERTEX_FLAG_RB		= 1 << 3,
	VERTEX_FLAG_ONE		= 1 << 4	// color blend only
};

enum {
	USER_DATA_FLAG_INTEGER	= 1 << 0,
	USER_DATA_FLAG_RECT		= 1 << 1,
	USER_DATA_FLAG_POINT	= 1 << 2,
	USER_DATA_FLAG_STRING	= 1 << 3
};

enum {
	OUTPUT_FORMAT_FLAG_SSBP	= 1 << 0,
	OUTPUT_FORMAT_FLAG_JSON	= 1 << 1,
	OUTPUT_FORMAT_FLAG_CSOURCE	= 1 << 2,
	OUTPUT_FORMAT_FLAG_SSFB	= 1 << 3,
	OUTPUT_FORMAT_FLAG_SSPKG = 1 << 4 ,
};

// MEMO: LumpExporter::StringEncodingと内容は同じだがmain.cppの中で閉じるもの＋厳密には目的が異なるので、別個に定義しておく
enum {
	ARGUMENT_ENCODE_UTF8 = 0,
	ARGUMENT_ENCODE_SJIS,
};

bool convert_error_exit = false;	//データにエラーがありコンバートを中止した

union converter32 {
	int i;
	unsigned int ui;
	float f;
};
converter32 c32;



struct Options
{
	typedef std::vector<std::string> StringList;

	bool							isHelp;
	bool							isVerbose;
	StringList						inList;
	LumpExporter::StringEncoding	encoding;
	std::string						imageBaseDir;
	std::string						outputDir;

	int								outputFormat;
	int								argumentEncode;

	Options()
		: isHelp(false)
		, isVerbose(false)
		, encoding(LumpExporter::UTF8)
	{}
};


typedef std::map<const spritestudio6::SsCell*, int> CellList;

CellList* makeCellList(spritestudio6::SsProject* proj)
{
	// セルデータの出力と、全てセルデータを集約したリストを作る
	CellList* cellList = new std::map<const spritestudio6::SsCell*, int>();
	int cellListIndex = 0;

	for (size_t mapIndex = 0; mapIndex < proj->cellmapList.size(); mapIndex++)
	{
		const spritestudio6::SsCellMap* cellMap = proj->getCellMap((int)mapIndex);
		for (size_t cellIndex = 0; cellIndex < cellMap->cells.size(); cellIndex++)
		{
			const spritestudio6::SsCell* cell = cellMap->cells[cellIndex];
			cellList->insert(CellList::value_type(cell, cellListIndex++));
		}
	}
	
	return cellList;
}


static const spritestudio6::SsKeyframe* findDefaultKeyframe(spritestudio6::SsAnimeDecoder& decoder, int partIndex, spritestudio6::SsAttributeKind::_enum tag)
{
	SPRITESTUDIO6SDK_foreach(std::vector<spritestudio6::SsPartAndAnime>, decoder.getPartAnime(), it)
	{
		spritestudio6::SsPartAnime* partAnime = it->second;
		spritestudio6::SsPart* part = it->first;
		if (part->arrayIndex != partIndex) continue;

		SPRITESTUDIO6SDK_foreach(spritestudio6::SsAttributeList, partAnime->attributes, attrIt)
		{
			spritestudio6::SsAttribute* attr = *attrIt;
			if (attr->tag != tag) continue;
			
			const spritestudio6::SsKeyframe* key = attr->firstKey();
			return key;
		}
		
	}
	return NULL;
}


static spritestudio6::SsAttribute* findAttribute(spritestudio6::SsPartAnime* partAnime, spritestudio6::SsAttributeKind::_enum tag)
{
	SPRITESTUDIO6SDK_foreach(spritestudio6::SsAttributeList, partAnime->attributes, attrIt)
	{
		spritestudio6::SsAttribute* attr = *attrIt;
		if (attr->tag == tag) return attr;
	}
	return NULL;
}


static const spritestudio6::SsKeyframe* findFirstKey(spritestudio6::SsPartAnime* partAnime, spritestudio6::SsAttributeKind::_enum tag)
{
	spritestudio6::SsAttribute* attr = findAttribute(partAnime, tag);
	if (attr)
	{
		return attr->firstKey();
	}
	return NULL;
}


static const spritestudio6::SsPartState* findState(std::list<spritestudio6::SsPartState*>& partList, int partIndex)
{
	SPRITESTUDIO6SDK_foreach(std::list<spritestudio6::SsPartState*>, partList, it)
	{
		const spritestudio6::SsPartState* state = *it;
		if (state->index == partIndex) return state;
	}
	return NULL;
}


//各機種用のコンソール用に文字コードを変換する関数
// kindArgumentEncodeはARGUMENT_ENCODE_～の値で、強制指定する場合だけ有効な値を与えてください。
static std::string convert_console_string(const std::string& srcUTF8, int kindArgumentEncode=-1)
{
	std::string dst;
	if(kindArgumentEncode < 0)
	{
#if _WIN32 || _WIN64
		kindArgumentEncode = ARGUMENT_ENCODE_SJIS;
#else
		kindArgumentEncode = ARGUMENT_ENCODE_UTF8;
#endif
	}

	switch(kindArgumentEncode)
	{
	case ARGUMENT_ENCODE_SJIS:
		dst = spritestudio6::SsCharConverter::utf8_to_sjis(srcUTF8);
		break;

	case ARGUMENT_ENCODE_UTF8:
	default:
		dst = srcUTF8;
		break;
	}

	return dst;
}


struct PartInitialData
{
	int		sortedOrder;
	int		index;
	int		lowflag;
	int		highflag;
	int		cellIndex;
	float	posX;
	float	posY;
	float	posZ;
	float	pivotX;
	float	pivotY;
	float	rotationZ;
	float	rotationX;
	float	rotationY;
	float	scaleX;
	float	scaleY;
	float	localscaleX;
	float	localscaleY;
	int		opacity;
	int		localopacity;
	float	size_X;
	float	size_Y;
	float	uv_move_X;
	float	uv_move_Y;
	float	uv_rotation;
	float	uv_scale_X;
	float	uv_scale_Y;
	float	boundingRadius;
	int		masklimen;
	int		priority;

	//インスタンスアトリビュート
	int		instanceValue_curKeyframe;
	int		instanceValue_startFrame;
	int		instanceValue_endFrame;
	int		instanceValue_loopNum;
	float	instanceValue_speed;
	int		instanceValue_loopflag;
	//エフェクトアトリビュート
	int		effectValue_curKeyframe;
	int		effectValue_startTime;
	float	effectValue_speed;
	int		effectValue_loopflag;
};



//全全角が使われてるかのチェック
bool isZenkaku( const spritestudio6::SsString* str )
{
	bool rc = false;
	int i = 0;
	int size = (int)str->length();
	const char *c = str->c_str();

	while ( true )
	{
		if ( ( c[i] == '\0' ) || ( size == i ) )
		{
			//終了
			break;
		}
		if ( ( c[i] < 0x20 ) || ( c[i] > 0x7e ) )
		{
			//半角以外
			rc = true;
			//終了
			break;
		}
		i++;
	}
	return( rc );
}

static std::vector<int16_t> s_frameIndexVec;

static void parseParts_ssqe(Lump* topLump, spritestudio6::SsProject* proj, const std::string& imageBaseDir)
{
}

static Lump* parseParts(spritestudio6::SsProject* proj, const std::string& imageBaseDir , const std::string& outPath)
{
//	static SsPartStateLess _ssPartStateLess;
	std::cerr << SPRITESTUDIOSDK_VERSION << "\n";	//バージョン表記は ssloader.h　にあります。
	std::cerr << "Ss6Converter ssbpFormatVersion=" << CURRENT_DATA_VERSION << "\n";
	std::cerr << "convert start!" << "\n";

	CellList* cellList = makeCellList(proj);

	Lump* topLump = Lump::set("ss::ProjectData", true, "ProjectData");

	if (spritestudio6::checkFileVersion(proj->version, SPRITESTUDIO6_SSPJVERSION) == false)
	{
		std::cerr << "エラー：SpriteStudio Ver.5.xのプロジェクトは使用できません。\n";
		std::cerr << "SpriteStudio Ver.6.xで保存する必要があります。\n";
		convert_error_exit = true;	//エラーが発生コンバート失敗
		return 0;
	}

	topLump->add(Lump::s32Data(DATA_ID, "dataId"));

	topLump->add(Lump::s32Data(CURRENT_DATA_VERSION, "version"));

	//4互換設定の出力
	topLump->add(Lump::s32Data(0, "flags"));

	if (imageBaseDir.length() > 0)
	{
		topLump->add(Lump::stringData(imageBaseDir, "imageBaseDir"));
	}
	else
	{
		topLump->add(Lump::stringData("", "imageBaseDir"));
	}

	Lump* cellsData = Lump::set("ss::Cell[]", true, "Cell");
	topLump->add(cellsData);
	Lump* packDataArray = Lump::set("ss::AnimePackData[]", true, "AnimePackData");
	topLump->add(packDataArray);
	Lump* effectfileArray = Lump::set("ss::EffectFile[]", true, "EffectFile");
	topLump->add(effectfileArray);

	topLump->add(Lump::s16Data((int)cellList->size(), "numCells"));
	
	topLump->add(Lump::s16Data((int)proj->animeList.size(), "numAnimePacks"));
	
	topLump->add(Lump::s16Data((int)proj->effectfileList.size(), "numEffectFileList"));

	topLump->add(Lump::s16Data((int)proj->sequenceList.size(), "numSequencePacks"));

	//セルマップ警告
	if (proj->cellmapList.size() == 0)
	{
		std::cerr << "警告：セルマップが存在しません。" << "\n";
		convert_error_exit = true;	//エラーが発生コンバート失敗
		return 0;
	}
	// セルの情報
	for (size_t mapIndex = 0; mapIndex < proj->cellmapList.size(); mapIndex++)
	{
		const spritestudio6::SsCellMap* cellMap = proj->cellmapList[mapIndex].get();

		Lump* cellMapData = Lump::set("ss::CellMap", true, "CellMap");
		cellMapData->add(Lump::stringData(cellMap->name, "name"));
		cellMapData->add(Lump::stringData(cellMap->imagePath, "imagePath"));

		cellMapData->add(Lump::s16Data((int)mapIndex, "index"));
		short wrapMode;
		short filterMode;
		if (cellMap->overrideTexSettings == true )							///< テクスチャ設定をプロジェクトの設定ではなく下記設定を使う
		{
			//個別の設定を使う
			cellMapData->add(Lump::s16Data(cellMap->wrapMode, "wrapmode"));				///< テクスチャのラップモード
			wrapMode = cellMap->wrapMode;
			cellMapData->add(Lump::s16Data(cellMap->filterMode, "filtermode"));			///< テクスチャのフィルタモード
			filterMode = cellMap->filterMode;
		}
		else
		{
			//プロジェクトの設定を使う
			cellMapData->add(Lump::s16Data(proj->settings.wrapMode, "wrapmode"));		///< テクスチャのラップモード
			wrapMode = proj->settings.wrapMode;
			cellMapData->add(Lump::s16Data(proj->settings.filterMode, "filtermode"));		///< テクスチャのフィルタモード
			filterMode = proj->settings.filterMode;
		}
		cellMapData->add(Lump::s16Data(0, "reserved"));	// reserved

		//全角チェック
		if ( isZenkaku( &cellMap->name ) == true )
		{
			std::cerr << "エラー：セルマップに全角が使用されています。半角英数でリネームしてください。: " << cellMap->name << "\n";
			convert_error_exit = true;	//エラーが発生コンバート失敗
		}

		if (cellMap->cells.size() == 0)
		{
			//セルマップにセルが１つも登録されていない場合はエラーにする
			std::cerr << "エラー：セルマップにセルが存在しません。セルを１つ以上作成してください。: " << cellMap->name << "\n";
			convert_error_exit = true;	//エラーが発生コンバート失敗
			return 0;
		}
		for (size_t cellIndex = 0; cellIndex < cellMap->cells.size(); cellIndex++)
		{
			const spritestudio6::SsCell* cell = cellMap->cells[cellIndex];

			Lump* cellData = Lump::set("ss::Cell", false, "Cell");
			cellsData->add(cellData);
			
			cellData->add(Lump::stringData(cell->name, "name"));
			cellData->add(cellMapData);

			cellData->add(Lump::s16Data((int)cellIndex, "indexInCellMap"));
			cellData->add(Lump::s16Data((int)cell->pos.x, "x"));
			cellData->add(Lump::s16Data((int)cell->pos.y, "y"));
			cellData->add(Lump::s16Data((int)cell->size.x, "width"));
			cellData->add(Lump::s16Data((int)cell->size.y, "height"));
			cellData->add(Lump::s16Data(0, "reserved"));	// reserved
			cellData->add(Lump::floatData(cell->pivot.x, "pivot_x"));
			cellData->add(Lump::floatData(cell->pivot.y, "pivot_y"));
			float u1 = cell->pos.x / cellMap->pixelSize.x;
			float v1 = cell->pos.y / cellMap->pixelSize.y;
			float u2 = ( cell->pos.x + cell->size.x ) / cellMap->pixelSize.x;
			float v2 = ( cell->pos.y + cell->size.y ) / cellMap->pixelSize.y;
			cellData->add(Lump::floatData(u1, "u1"));			//テクスチャのサイズを出力
			cellData->add(Lump::floatData(v1, "v1"));
			cellData->add(Lump::floatData(u2, "u2"));			//テクスチャのサイズを出力
			cellData->add(Lump::floatData(v2, "v2"));

			//全角チェック
			if (isZenkaku(&cell->name) == true)
			{
				std::cerr << "エラー：セルに全角が使用されています。半角英数でリネームしてください。: " << cell->name << "\n";
				convert_error_exit = true;	//エラーが発生コンバート失敗
				return 0;
			}
		}
	}

	//アニメーション警告
	if (proj->animeList.size() == 0)
	{
		std::cerr << "警告：アニメーションが存在しません" << "\n";
		convert_error_exit = true;	//エラーが発生コンバート失敗
		return 0;
	}
	// パーツ、アニメ情報
	for (int packIndex = 0; packIndex < (int)proj->animeList.size(); packIndex++)
	{
		const spritestudio6::SsAnimePack* animePack = proj->animeList[packIndex].get();
		const spritestudio6::SsModel& model = animePack->Model;
		
		// AnimePackData
		Lump* animePackData = Lump::set("ss::AnimePackData", false, "AnimePackData");
		packDataArray->add(animePackData);

		Lump* partDataArray = Lump::set("ss::PartData[]", true, "PartData");
		Lump* animeDataArray = Lump::set("ss::AnimationData[]", true, "AnimationData");

		animePackData->add(Lump::stringData(animePack->name, "name"));

		//全角チェック
		if ( isZenkaku( &animePack->name ) == true )
		{
			std::cerr << "エラー：ファイル名に全角が使用されています。半角英数でリネームしてください。: " << animePack->name << "\n";
			convert_error_exit = true;	//エラーが発生コンバート失敗
			return 0;
		}
		animePackData->add(partDataArray);
		animePackData->add(animeDataArray);
		animePackData->add(Lump::s16Data((int)model.partList.size(), "numParts"));
		animePackData->add(Lump::s16Data((int)animePack->animeList.size(), "numAnimations"));

		// パーツ情報（モデル）の出力
		for (int partIndex = 0; partIndex < (int)model.partList.size(); partIndex++)
		{
			const spritestudio6::SsPart* part = model.partList[partIndex];

			// PartData
			Lump* partData = Lump::set("ss::PartData", false, "PartData" );
			partDataArray->add(partData);

			partData->add(Lump::stringData(part->name, "name"));

			//全角チェック
			if ( isZenkaku( &part->name ) == true )
			{
				std::cerr << "エラー：パーツ名に全角が使用されています。半角英数でリネームしてください。: " << part->name << "\n";
				convert_error_exit = true;	//エラーが発生コンバート失敗
				return 0;
			}
			partData->add(Lump::s16Data(part->arrayIndex, "index"));
			partData->add(Lump::s16Data(part->parentIndex, "parentIndex"));

			//5.5対応5.3.5に無いパーツ種別がある場合ワーニングを表示する
			switch (part->type)
			{
			case spritestudio6::SsPartType::null:			// null。領域を持たずSRT情報のみ。ただし円形の当たり判定は設定可能。
			case spritestudio6::SsPartType::normal:		// 通常パーツ。領域を持つ。画像は無くてもいい。
			case spritestudio6::SsPartType::mask:			// 6.0マスクパーツ
			case spritestudio6::SsPartType::constraint:	// 6.0コンストレイントパーツ
			case spritestudio6::SsPartType::bonepoint:		// 6.0ボーンエフェクトパーツ
			case spritestudio6::SsPartType::joint:			// 6.0ジョイントパーツ
			case spritestudio6::SsPartType::armature:		// 6.0ボーンパーツ
			case spritestudio6::SsPartType::mesh:			// 6.0メッシュパーツ
				partData->add(Lump::s16Data(part->type, "type"));
				break;
			case spritestudio6::SsPartType::instance:		// インスタンス。他アニメ、パーツへの参照。シーン編集モードの代替になるもの
				//参照アニメのポインタが無い場合はNULLパーツになる。
				{
					spritestudio6::SsString packname = part->refAnimePack;
					spritestudio6::SsString animename = part->refAnime;
					spritestudio6::SsAnimePack* refpack = proj->findAnimationPack(packname);
					spritestudio6::SsAnimation* refanime = refpack->findAnimation(animename);
					if (refanime == NULL)
					{
						partData->add(Lump::s16Data(spritestudio6::SsPartType::null, "type"));
						std::cerr << "警告：参照のないインスタンスパーツが存在します: " << animePack->name << ".ssae " << part->name << "\n";
					}
					else
					{
						partData->add(Lump::s16Data(part->type, "type"));
					}
				}
				break;
			case spritestudio6::SsPartType::effect:		// 5.5エフェクトパーツ
				//参照エフェクト名が空の場合はNULLパーツになる。
				if (part->refEffectName == "")
				{
					partData->add(Lump::s16Data(spritestudio6::SsPartType::null, "type"));
					//未実装　ワーニングを表示しNULLパーツにする
					std::cerr << "警告：参照のないエフェクトパーツが存在します: " << animePack->name << ".ssae " << part->name << "\n";
				}
				else
				{
					partData->add(Lump::s16Data(part->type, "type"));
				}
				break;
			default:
				//未対応パーツ　ワーニングを表示しNULLパーツにする
				std::cerr << "警告：未対応のパーツ種別が使われています: " << animePack->name << ".ssae " << part->name << "\n";
				partData->add(Lump::s16Data(spritestudio6::SsPartType::null, "type"));
				break;
			}
			partData->add(Lump::s16Data(part->boundsType, "boundsType"));
			partData->add(Lump::s16Data(part->alphaBlendType, "alphaBlendType"));
			partData->add(Lump::s16Data(0, "reserved"));	// reserved

			//インスタンスアニメ名
			if ( part->refAnime == "" )
			{
				const spritestudio6::SsString str = "";
//				partData->add(Lump::s16Data((int)str.length()));				//文字列のサイズ
				partData->add(Lump::stringData(str, "refname"));							//文字列
			}
			else
			{
				const spritestudio6::SsString str = part->refAnimePack + "/" + part->refAnime;
//				partData->add(Lump::s16Data((int)str.length()));				//文字列のサイズ
				partData->add(Lump::stringData(str, "refname"));							//文字列
			}
			//エフェクト名
			if (part->refEffectName == "")
			{
				const spritestudio6::SsString str = "";
				partData->add(Lump::stringData(str, "effectfilename"));							//文字列
			}
			else
			{
				const spritestudio6::SsString str = part->refEffectName;
				partData->add(Lump::stringData(str, "effectfilename"));							//文字列
			}
			//カラーラベル
			const spritestudio6::SsString str = part->colorLabel;
			partData->add(Lump::stringData(str, "colorLabel"));								//文字列

			//マスク対象
			partData->add(Lump::s16Data(part->maskInfluence, "maskInfluence"));

		}

		// アニメ情報の出力
		spritestudio6::SsCellMapList* cellMapList = new spritestudio6::SsCellMapList();	// SsAnimeDecoderのデストラクタで破棄される
		spritestudio6::SsAnimeDecoder decoder;
//		const SsKeyframe* key;

		for (int animeIndex = 0; animeIndex < (int)animePack->animeList.size(); animeIndex++)
		{
			spritestudio6::SsAnimePack* animePack = proj->getAnimePack(packIndex);
			spritestudio6::SsModel* model = &animePack->Model;
			spritestudio6::SsAnimation* anime = animePack->animeList[animeIndex];
			
			cellMapList->set(proj, animePack);
			decoder.setAnimation(model, anime, cellMapList, proj);
			std::list<spritestudio6::SsPartState*>& partList = decoder.getPartSortList();
			
			// AnimationData
			Lump* animeData = Lump::set("ss::AnimationData", false, "AnimationData");
			animeDataArray->add(animeData);

			// パーツごとのアニメーションパラメータ初期値
			// ※とりあえず先頭フレームの値を初期値にしているが、一番使われている値を初期値にすべきかも
			size_t numParts = model->partList.size();
			std::vector<PartInitialData> initialDataList;
			
			decoder.setPlayFrame(0);
			decoder.update();

			Lump* initialDataArray = Lump::set("ss::AnimationInitialData[]", true, "AnimationInitialData");
			int sortedOrder = 0;
			SPRITESTUDIO6SDK_foreach(std::vector<spritestudio6::SsPartAndAnime>, decoder.getPartAnime(), it)
			{
				spritestudio6::SsPart* part = it->first;
//				spritestudio6::SsPartAnime* partAnime = it->second;
				
				const spritestudio6::SsPartState* state = findState(partList, part->arrayIndex);
				
				PartInitialData init;
				init.sortedOrder = sortedOrder++;
				init.index = state->index;

				int flags = 0;
				int flags2 = 0;
				if (state->hide)  flags |= PART_FLAG_INVISIBLE;
				//イメージ反転を適用する
				bool hFlip = state->hFlip ^ state->imageFlipH;
				bool vFlip = state->vFlip ^ state->imageFlipV;
				if (hFlip) flags |= PART_FLAG_FLIP_H;
				if (vFlip) flags |= PART_FLAG_FLIP_V;

				init.lowflag = flags;
				init.highflag = flags2;

				// cellIndex
				int cellIndex = -1;
				if (state->cellValue.cell) cellIndex = (*cellList)[state->cellValue.cell];
				init.cellIndex = cellIndex;
				
				init.posX = state->position.x;
				init.posY = state->position.y;
				init.posZ = state->position.z;
				init.pivotX = state->pivotOffset.x;
				init.pivotY = state->pivotOffset.y;
				init.rotationX = state->rotation.x;
				init.rotationY = state->rotation.y;
				init.rotationZ = state->rotation.z;

				init.scaleX = state->scale.x;
				init.scaleY = state->scale.y;
				init.localscaleX = state->localscale.x;
				init.localscaleY = state->localscale.y;

				init.opacity = (int)(state->alpha * 255);
				init.localopacity = (int)(state->localalpha * 255);

				//サイズはエディターでは初期値が1が設定されているが、
				//本来であればキーがないときはセルのサイズが初期値になる
				init.size_X = state->size.x;
				init.size_Y = state->size.y;
				spritestudio6::SsCell * cell = state->cellValue.cell;
				if ( cell )
				{
					//セルデータがある場合はセルのサイズを初期値にする
					init.size_X = cell->size.x;
					init.size_Y = cell->size.y;
				}
				//初期値をSSのディフォルトにする
/*
				init.uv_move_X = state->uvTranslate.x;
				init.uv_move_Y = state->uvTranslate.y;
				init.uv_rotation = state->uvRotation;
				init.uv_scale_X = state->uvScale.x;
				init.uv_scale_Y = state->uvScale.y;
*/
				init.uv_move_X = 0;
				init.uv_move_Y = 0;
				init.uv_rotation = 0;
				init.uv_scale_X = 1;
				init.uv_scale_Y = 1;
				init.boundingRadius = state->boundingRadius;

				init.masklimen = state->masklimen;
				init.priority = state->prio;

				//インスタンス関連
				init.instanceValue_curKeyframe = state->instanceValue.curKeyframe;
				init.instanceValue_startFrame = state->instanceValue.startFrame;
				init.instanceValue_endFrame = state->instanceValue.endFrame;
				init.instanceValue_loopNum = state->instanceValue.loopNum;
				init.instanceValue_speed = state->instanceValue.speed;
				init.instanceValue_loopflag = state->instanceValue.loopflag;
				//エフェクト関連
				init.effectValue_startTime = state->effectValue.startTime;
				init.effectValue_speed = state->effectValue.speed;
				init.effectValue_loopflag = state->effectValue.loopflag;
				init.effectValue_curKeyframe = state->effectValue.curKeyframe;

				initialDataList.push_back(init);
				
				Lump* initialData = Lump::set("ss::AnimationInitialData", false, "AnimationInitialData");
				initialDataArray->add(initialData);

				initialData->add(Lump::s16Data(init.index, "index"));
				initialData->add(Lump::s16Data(0, "reserved")); //ダミーデータ
				initialData->add(Lump::s32Data(init.lowflag, "lowflag"));
				initialData->add(Lump::s32Data(init.highflag, "highflag"));
				initialData->add(Lump::s16Data(init.priority, "priority"));
				initialData->add(Lump::s16Data(init.cellIndex, "cellIndex"));
				initialData->add(Lump::s16Data(init.opacity, "opacity"));
				initialData->add(Lump::s16Data(init.localopacity, "localopacity"));
				initialData->add(Lump::s16Data(init.masklimen, "masklimen"));
				initialData->add(Lump::s16Data(0, "reserved")); //ダミーデータ
				initialData->add(Lump::floatData(init.posX, "positionX"));
				initialData->add(Lump::floatData(init.posY, "positionY"));
				initialData->add(Lump::floatData(init.posZ, "positionZ"));
				initialData->add(Lump::floatData(init.pivotX, "pivotX"));
				initialData->add(Lump::floatData(init.pivotY, "pivotY"));
				initialData->add(Lump::floatData(init.rotationX, "rotationX"));
				initialData->add(Lump::floatData(init.rotationY, "rotationY"));
				initialData->add(Lump::floatData(init.rotationZ, "rotationZ"));
				initialData->add(Lump::floatData(init.scaleX, "scaleX"));
				initialData->add(Lump::floatData(init.scaleY, "scaleY"));
				initialData->add(Lump::floatData(init.localscaleX, "localscaleX"));
				initialData->add(Lump::floatData(init.localscaleY, "localscaleY"));
				initialData->add(Lump::floatData(init.size_X, "size_X"));
				initialData->add(Lump::floatData(init.size_Y, "size_Y"));
				initialData->add(Lump::floatData(init.uv_move_X, "uv_move_X"));
				initialData->add(Lump::floatData(init.uv_move_Y, "uv_move_Y"));
				initialData->add(Lump::floatData(init.uv_rotation, "uv_rotation"));
				initialData->add(Lump::floatData(init.uv_scale_X, "uv_scale_X"));
				initialData->add(Lump::floatData(init.uv_scale_Y, "uv_scale_Y"));
				initialData->add(Lump::floatData(init.boundingRadius, "boundingRadius"));
				//インスタンス関連
				initialData->add(Lump::s32Data(init.instanceValue_curKeyframe, "instanceValue_curKeyframe"));
				initialData->add(Lump::s32Data(init.instanceValue_startFrame, "instanceValue_startFrame"));
				initialData->add(Lump::s32Data(init.instanceValue_endFrame, "instanceValue_endFrame"));
				initialData->add(Lump::s32Data(init.instanceValue_loopNum, "instanceValue_loopNum"));
				initialData->add(Lump::floatData(init.instanceValue_speed, "instanceValue_speed"));
				initialData->add(Lump::s32Data(init.instanceValue_loopflag, "instanceValue_loopflag"));
				//エフェクト関連
				initialData->add(Lump::s32Data(init.effectValue_curKeyframe, "effectValue_curKeyframe"));
				initialData->add(Lump::s32Data(init.effectValue_startTime, "effectValue_startTime"));
				initialData->add(Lump::floatData(init.effectValue_speed, "effectValue_speed"));
				initialData->add(Lump::s32Data(init.effectValue_loopflag, "effectValue_loopflag"));
			}

			Lump* meshsDataUV = Lump::set("ss::ss_u16*[]", true, "meshsDataUV");
			{
				decoder.setPlayFrame(0);
				decoder.update();

				SPRITESTUDIO6SDK_foreach(std::vector<spritestudio6::SsPartAndAnime>, decoder.getPartAnime(), it)
				{
					spritestudio6::SsPart* part = it->first;
					const spritestudio6::SsPartState* state = findState(partList, part->arrayIndex);

					//サイズ分のUV出力
					Lump* meshData = Lump::set("ss::ss_u16*[]", true, "meshData");
					meshsDataUV->add(meshData);

					//メッシュのサイズを書き出す
					if (part->type == spritestudio6::SsPartType::mesh)
					{
						int meshsize = state->meshPart->ver_size;
						meshData->add(Lump::s32Data((int)state->meshPart->isBind, "isBind"));	//バインドの有無
						meshData->add(Lump::s32Data(meshsize, "meshsize"));	//サイズ
						int i;
						std::vector<float>& uvsRaw = *(state->meshPart->uvs.get());
						for (i = 0; i < meshsize; i++)
						{
							float u = uvsRaw[i * 2 + 0];
							float v = uvsRaw[i * 2 + 1];
							meshData->add(Lump::floatData(u, "u"));
							meshData->add(Lump::floatData(v, "v"));
						}
					}
					else
					{
						meshData->add(Lump::s32Data(0, "isBind"));
					}

				}

			}

			Lump* meshsDataIndices = Lump::set("ss::ss_u16*[]", true, "meshsDataIndices");
			{
				decoder.setPlayFrame(0);
				decoder.update();

				SPRITESTUDIO6SDK_foreach(std::vector<spritestudio6::SsPartAndAnime>, decoder.getPartAnime(), it)
				{
					spritestudio6::SsPart* part = it->first;
					const spritestudio6::SsPartState* state = findState(partList, part->arrayIndex);

					//サイズ分のUV出力
					Lump* meshData = Lump::set("ss::ss_u16*[]", true, "meshData");
					meshsDataIndices->add(meshData);

					//メッシュのサイズを書き出す
					if (part->type == spritestudio6::SsPartType::mesh)
					{
						int tri_size = state->meshPart->tri_size;
						meshData->add(Lump::s32Data(tri_size, "tri_size"));	//サイズ
						int i;
						std::vector<unsigned short>& indicesRaw = *(state->meshPart->indices.get());
						for (i = 0; i < tri_size; i++)
						{
							int po1 = indicesRaw[i * 3 + 0];
							int po2 = indicesRaw[i * 3 + 1];
							int po3 = indicesRaw[i * 3 + 2];
							meshData->add(Lump::s32Data(po1, "po1"));
							meshData->add(Lump::s32Data(po2, "po2"));
							meshData->add(Lump::s32Data(po3, "po3"));
						}
					}
					else
					{
						meshData->add(Lump::s32Data(0, "tri_size"));
					}
				}
			}


			// フレーム毎データ
			Lump* frameDataIndexArray = Lump::set("ss::ss_u16*[]", true, "frameDataIndexArray");

			for (int frame = 0; frame < decoder.getAnimeTotalFrame(); frame++)
			{
				// パラメータを計算し更新する
				decoder.setPlayFrame(static_cast<float>(frame));
				decoder.update();
				
				// パーツの描画順が初期値と変わっていないかチェックする
				bool prioChanged = false;
//				int initialDataListIndex = 0;
//				foreach(std::list<SsPartState*>, partList, it)
//				{
//					const SsPartState* state = *it;
//					if (state->index != sortedInitialDataList.at(initialDataListIndex++).index)
//					{
//						// 描画順が変わっている
//						prioChanged = true;
//						break;
//					}
//				}

				// パーツごとのデータを出力する
				Lump* frameData = Lump::set("ss::ss_u16[]", true, "frameData");
				frameDataIndexArray->add(frameData);

				Lump* frameFlag = Lump::s16Data(0, "frameFlag");
//				frameData->add(frameFlag);

				int outPartsCount = 0;
				SPRITESTUDIO6SDK_foreach(std::list<spritestudio6::SsPartState*>, partList, it)
				{
					const spritestudio6::SsPartState* state = *it;
					//セルに設定された原点補正を取得
					spritestudio6::SsVector2 pivot;
					pivot.x = 0;
					pivot.y = 0;
					//セルの原点情報はセル情報へ含める
/*
					SsCell * cell = state->cellValue.cell;
					float cpx = 0;
					float cpy = 0;

					if (cell)
					{
						// セルに設定された原点オフセットを適用する
						// ※セルの原点は中央が0,0で＋が右上方向になっている
						cpx = cell->pivot.x;
						if (state->hFlip) cpx = -cpx;	// 水平フリップによって原点を入れ替える
						// 上が＋で入っているのでここで反転する。
						cpy = -cell->pivot.y;
						if (state->vFlip) cpy = -cpy;	// 垂直フリップによって原点を入れ替える
					}
					// 次に原点オフセットアニメの値を足す
					pivot.x = cpx + state->pivotOffset.x;
					pivot.y = cpy + -state->pivotOffset.y;
*/
					pivot.x = state->pivotOffset.x;
					pivot.y = state->pivotOffset.y;


					int cellIndex = -1;
					if (state->cellValue.cell) cellIndex = (*cellList)[state->cellValue.cell];
					
					// フラグのみパラメータ
					int s_flags = 0;
					if (state->hide)	s_flags |= PART_FLAG_INVISIBLE;
					//イメージ反転を適用する
					bool hFlip = state->hFlip ^ state->imageFlipH;
					bool vFlip = state->vFlip ^ state->imageFlipV;
					if (hFlip)			s_flags |= PART_FLAG_FLIP_H;
					if (vFlip)			s_flags |= PART_FLAG_FLIP_V;
					
					// 以下、規定値のときは出力を省略する
					int p_flags = 0;
					const PartInitialData& init = initialDataList.at(state->index);
					if (cellIndex != init.cellIndex)						p_flags |= PART_FLAG_CELL_INDEX;
					if (state->position.x != init.posX)						p_flags |= PART_FLAG_POSITION_X;
					if (state->position.y != init.posY)						p_flags |= PART_FLAG_POSITION_Y;
					if (state->position.z != init.posZ)						p_flags |= PART_FLAG_POSITION_Z;
					if (pivot.x != init.pivotX)								p_flags |= PART_FLAG_PIVOT_X;
					if (pivot.y != init.pivotY)								p_flags |= PART_FLAG_PIVOT_Y;
					if (state->rotation.x != init.rotationX)				p_flags |= PART_FLAG_ROTATIONX;
					if (state->rotation.y != init.rotationY)				p_flags |= PART_FLAG_ROTATIONY;
					if (state->rotation.z != init.rotationZ)				p_flags |= PART_FLAG_ROTATIONZ;

					if (state->scale.x != init.scaleX)						p_flags |= PART_FLAG_SCALE_X;
					if (state->scale.y != init.scaleY)						p_flags |= PART_FLAG_SCALE_Y;
					if (state->localscale.x != init.localscaleX)			p_flags |= PART_FLAG_LOCALSCALE_X;
					if (state->localscale.y != init.localscaleY)			p_flags |= PART_FLAG_LOCALSCALE_Y;
					if ((int)( state->alpha * 255 ) != init.opacity)		p_flags |= PART_FLAG_OPACITY;
					if ((int)( state->localalpha * 255 ) != init.localopacity)		p_flags |= PART_FLAG_LOCALOPACITY;
					if (state->size.x != init.size_X)						p_flags |= PART_FLAG_SIZE_X;
					if (state->size.y != init.size_Y)						p_flags |= PART_FLAG_SIZE_Y;
					if (state->uvTranslate.x != init.uv_move_X )			p_flags |= PART_FLAG_U_MOVE;
					if (state->uvTranslate.y != init.uv_move_Y)				p_flags |= PART_FLAG_V_MOVE;
					if (state->uvRotation != init.uv_rotation)				p_flags |= PART_FLAG_UV_ROTATION;
					if (state->uvScale.x != init.uv_scale_X)				p_flags |= PART_FLAG_U_SCALE;
					if (state->uvScale.y != init.uv_scale_Y)				p_flags |= PART_FLAG_V_SCALE;
					if (state->boundingRadius != init.boundingRadius)		p_flags |= PART_FLAG_BOUNDINGRADIUS;
					if (state->masklimen != init.masklimen)					p_flags |= PART_FLAG_MASK;
					if (state->prio != init.priority)						p_flags |= PART_FLAG_PRIORITY;
					//インスタンス情報出力チェック
					if (state->refAnime)
					{
						//インスタンス関連
						if (
							(state->instanceValue.curKeyframe != init.instanceValue_curKeyframe)
							|| (state->instanceValue.startFrame != init.instanceValue_startFrame)
							|| (state->instanceValue.endFrame != init.instanceValue_endFrame)
							|| (state->instanceValue.loopNum != init.instanceValue_loopNum)
							|| (state->instanceValue.speed != init.instanceValue_speed)
							|| (state->instanceValue.loopflag != init.instanceValue_loopflag)
							)
						{
							p_flags |= PART_FLAG_INSTANCE_KEYFRAME;
						}
					}
					//エフェクト情報の出力チェック
					if (state->refEffect)
					{
						//エフェクト関連
						if (
							(state->effectValue.startTime != init.effectValue_startTime)
							|| (state->effectValue.speed != init.effectValue_speed)
							|| (state->effectValue.loopflag != init.effectValue_loopflag)
							|| (state->effectValue.curKeyframe != init.effectValue_curKeyframe)
							)
						{
							p_flags |= PART_FLAG_EFFECT_KEYFRAME;
						}
					}

					// パーツカラー値を格納する必要があるかチェック
					int cb_flags = 0;
					if (state->is_parts_color)
					{
						switch (state->partsColorValue.target)
						{
							case spritestudio6::SsColorBlendTarget::whole:
								if ( 
									  ( state->partsColorValue.color.rgba.a == 0 )
								   && ( state->partsColorValue.color.rgba.r == 0 )
								   && ( state->partsColorValue.color.rgba.g == 0 )
								   && ( state->partsColorValue.color.rgba.b == 0 )
								   )
								{
									//右のキーが単色、左のキーが4頂点などの場合に単色の色出力ができないため
									//フラグがあるのに単色の色が設定されていない場合は4頂点カラーとして出力
									cb_flags = VERTEX_FLAG_LT|VERTEX_FLAG_RT|VERTEX_FLAG_LB|VERTEX_FLAG_RB;
								}
								else
								{
									cb_flags = VERTEX_FLAG_ONE;
								}

								break;
							case spritestudio6::SsColorBlendTarget::vertex:
								cb_flags = VERTEX_FLAG_LT|VERTEX_FLAG_RT|VERTEX_FLAG_LB|VERTEX_FLAG_RB;
								break;
							default:
								// Unsupported
								assert(false);
								break;
						}
						
						if (cb_flags) p_flags |= PART_FLAG_PARTS_COLOR;
					}

					// 頂点変形のオフセット値を格納する必要あるかチェック
					int vt_flags = 0;
					if (state->is_vertex_transform)
					{
						for (int vtxNo = 0; vtxNo < 4; vtxNo++)
						{
							if (proj->settings.vertexAnimeFloat != 0)
							{
								// 小数対応
								if (
								     ( state->vertexValue.offsets[vtxNo].x != 0.0f )
								  || ( state->vertexValue.offsets[vtxNo].y != 0.0f)
								   )
								{
									vt_flags |= 1 << vtxNo;
								}
							}
							else
							{
								// 小数点以下切り捨てで
								if (
								     ( (int)state->vertexValue.offsets[vtxNo].x != 0 )
								  || ( (int)state->vertexValue.offsets[vtxNo].y != 0 )
								   )
								{
									vt_flags |= 1 << vtxNo;
								}
							}
						}

						if (vt_flags) p_flags |= PART_FLAG_VERTEX_TRANSFORM;
					}

					int p_flags2 = 0;
					//メッシュ情報を出力する必要があるかチェックする
					if (state->partType == spritestudio6::SsPartType::mesh)
					{
						p_flags2 |= PART_FLAG_MESHDATA;
					}

					

//					if (!prioChanged && s_flags == init.flags && p_flags == 0)
//					{
//						// このパーツは出力を省略する
//						continue;
//					}
					
					// パーツの座標値、回転、スケールなどを出力する
					std::string tagname = "part_" + std::to_string(outPartsCount) + "_";
					outPartsCount++;
					frameData->add(Lump::s16Data(state->index, tagname + "index"));
//					frameData->add(Lump::s16Data(0));				//32bitアライメント用ダミーデータ
					frameData->add(Lump::s32Data(s_flags | p_flags, tagname + "flag1"));
					c32.ui = s_flags | p_flags;
					//intで出力すると上位ビットが立った場合に丸めが発生するので、floatで出力し、プレイヤーではbitを整数で扱う事になる
					frameData->add(Lump::s32Data(p_flags2, tagname + "flag2"));
					c32.ui = p_flags2;
					//intで出力すると上位ビットが立った場合に丸めが発生するので、floatで出力し、プレイヤーではbitを整数で扱う事になる

					if (p_flags & PART_FLAG_CELL_INDEX)
					{
						frameData->add(Lump::s16Data(cellIndex, tagname + "cellIndex"));
					}
					if (p_flags & PART_FLAG_POSITION_X)
					{
						frameData->add(Lump::floatData(state->position.x, tagname + "position_x"));
					}
					if (p_flags & PART_FLAG_POSITION_Y)
					{
						frameData->add(Lump::floatData(state->position.y, tagname + "position_y"));
					}
					if (p_flags & PART_FLAG_POSITION_Z)
					{
						frameData->add(Lump::floatData(state->position.z, tagname + "position_z"));
					}

					if (p_flags & PART_FLAG_PIVOT_X)
					{
						frameData->add(Lump::floatData(pivot.x, tagname + "pivot_x"));
					}
					if (p_flags & PART_FLAG_PIVOT_Y)
					{
						frameData->add(Lump::floatData(pivot.y, tagname + "pivot_y"));
					}
					if (p_flags & PART_FLAG_ROTATIONX)
					{
						frameData->add(Lump::floatData(state->rotation.x, tagname + "rotation_x"));	// degree
					}
					if (p_flags & PART_FLAG_ROTATIONY)
					{
						frameData->add(Lump::floatData(state->rotation.y, tagname + "rotation_y"));	// degree
					}
					if (p_flags & PART_FLAG_ROTATIONZ)
					{
						frameData->add(Lump::floatData(state->rotation.z, tagname + "rotation_z"));	// degree
					}
					if (p_flags & PART_FLAG_SCALE_X)
					{
						frameData->add(Lump::floatData(state->scale.x, tagname + "scale_x"));
					}
					if (p_flags & PART_FLAG_SCALE_Y)
					{
						frameData->add(Lump::floatData(state->scale.y, tagname + "scale_y"));
					}
					if (p_flags & PART_FLAG_LOCALSCALE_X)
					{
						frameData->add(Lump::floatData(state->localscale.x, tagname + "localscale_x"));
					}
					if (p_flags & PART_FLAG_LOCALSCALE_Y)
					{
						frameData->add(Lump::floatData(state->localscale.y, tagname + "localscale_y"));
					}
					if (p_flags & PART_FLAG_OPACITY)
					{
						frameData->add(Lump::s16Data((int)(state->alpha * 255), tagname + "alpha"));
					}
					if (p_flags & PART_FLAG_LOCALOPACITY)
					{
						frameData->add(Lump::s16Data((int)(state->localalpha * 255), tagname + "localalpha"));
					}

					if (p_flags & PART_FLAG_SIZE_X)
					{
						frameData->add(Lump::floatData(state->size.x, tagname + "size_x"));
					}
					if (p_flags & PART_FLAG_SIZE_Y)
					{
						frameData->add(Lump::floatData(state->size.y, tagname + "size_y"));
					}

					if (p_flags & PART_FLAG_U_MOVE)
					{
						frameData->add(Lump::floatData(state->uvTranslate.x, tagname + "uvTranslate.x"));
					}
					if (p_flags & PART_FLAG_V_MOVE)
					{
						frameData->add(Lump::floatData(state->uvTranslate.y, tagname + "uvTranslate.y"));
					}
					if (p_flags & PART_FLAG_UV_ROTATION)
					{
						frameData->add(Lump::floatData(state->uvRotation, tagname + "uvRotation"));
					}
					if (p_flags & PART_FLAG_U_SCALE)
					{
						frameData->add(Lump::floatData(state->uvScale.x, tagname + "uvScale_x"));
					}
					if (p_flags & PART_FLAG_V_SCALE)
					{
						frameData->add(Lump::floatData(state->uvScale.y, tagname + "uvScale_y"));
					}

					if (p_flags & PART_FLAG_BOUNDINGRADIUS)
					{
						frameData->add(Lump::floatData(state->boundingRadius, tagname + "boundingRadius"));
					}

					if (p_flags & PART_FLAG_MASK)
					{
						frameData->add(Lump::s16Data(state->masklimen, tagname + "masklimen"));
					}
					if (p_flags & PART_FLAG_PRIORITY)
					{
						frameData->add(Lump::s16Data(state->prio, tagname + "prio"));
					}

					//インスタンス情報出力
					if (p_flags & PART_FLAG_INSTANCE_KEYFRAME)
					{
						frameData->add(Lump::s32Data(state->instanceValue.curKeyframe, tagname + "instanceValue_curKeyframe"));
						c32.i = state->instanceValue.curKeyframe;
						frameData->add(Lump::s32Data(state->instanceValue.startFrame, tagname + "instanceValue_startFrame"));
						c32.i = state->instanceValue.startFrame;
						frameData->add(Lump::s32Data(state->instanceValue.endFrame, tagname + "instanceValue_endFrame"));
						c32.i = state->instanceValue.endFrame;
						frameData->add(Lump::s32Data(state->instanceValue.loopNum, tagname + "instanceValue_loopNum"));
						c32.i = state->instanceValue.loopNum;
						frameData->add(Lump::floatData(state->instanceValue.speed, tagname + "instanceValue_speed"));
						frameData->add(Lump::s32Data(state->instanceValue.loopflag, tagname + "instanceValue_loopflag"));
						c32.i = state->instanceValue.loopflag;
					}
					//エフェクト情報出力
					if (p_flags & PART_FLAG_EFFECT_KEYFRAME)
					{
						frameData->add(Lump::s32Data(state->effectValue.curKeyframe, tagname + "effectValue_curKeyframe"));	//キー配置フレーム
						c32.i = state->effectValue.curKeyframe;
						frameData->add(Lump::s32Data(state->effectValue.startTime, tagname + "effectValue_startTime"));	//開始フレーム
						c32.i = state->effectValue.startTime;
						frameData->add(Lump::floatData(state->effectValue.speed, tagname + "effectValue_speed"));		//再生速度
						frameData->add(Lump::s32Data(state->effectValue.loopflag, tagname + "effectValue_loopflag"));		//独立動作
						c32.i = state->effectValue.loopflag;
					}


					// 頂点変形データ
					if (p_flags & PART_FLAG_VERTEX_TRANSFORM)
					{
						// どの頂点のオフセット値が格納されているかのフラグ
						frameData->add(Lump::s16Data(vt_flags));

						// 各頂点のオフセット値
						for (int vtxNo = 0; vtxNo < 4; vtxNo++)
						{
							if (vt_flags & (1 << vtxNo))
							{
								std::string tagname_x = tagname + "vertexValue.offsets_" + std::to_string(vtxNo) + "_x";
								std::string tagname_y = tagname + "vertexValue.offsets_" + std::to_string(vtxNo) + "_x";
								if (proj->settings.vertexAnimeFloat != 0)
								{
									//頂点変形の少数対応
									frameData->add(Lump::floatData(state->vertexValue.offsets[vtxNo].x, tagname_x));
									frameData->add(Lump::floatData(state->vertexValue.offsets[vtxNo].y, tagname_y));
								}
								else
								{
									frameData->add(Lump::floatData((float)((int)state->vertexValue.offsets[vtxNo].x), tagname_x));
									frameData->add(Lump::floatData((float)((int)state->vertexValue.offsets[vtxNo].y), tagname_y));
								}
							}
						}
					}

					// パーツカラーデータ
					if (p_flags & PART_FLAG_PARTS_COLOR)
					{
						// ブレンド方法と、単色もしくはどの頂点に対するカラー値が格納されているかをu16にまとめる
						int typeAndFlags = (int)state->partsColorValue.blendType | (cb_flags << 8);
						frameData->add(Lump::s16Data(typeAndFlags));

						if (cb_flags & VERTEX_FLAG_ONE)
						{
							frameData->add(Lump::floatData(state->partsColorValue.color.rate, tagname + "partsColorValue_color_rate"));
							frameData->add(Lump::colorData(state->partsColorValue.color.rgba.toARGB(), tagname + "partsColorValue_color_rgba"));
						}
						else
						{
							for (int vtxNo = 0; vtxNo < 4; vtxNo++)
							{
								std::string tagname_rate = tagname + "partsColorValue_colors_" + std::to_string(vtxNo) + "_rate";
								std::string tagname_rgba = tagname + "partsColorValue_colors_" + std::to_string(vtxNo) + "_rgba";
								if (cb_flags & (1 << vtxNo))
								{
									frameData->add(Lump::floatData(state->partsColorValue.colors[vtxNo].rate, tagname_rate));
									frameData->add(Lump::colorData(state->partsColorValue.colors[vtxNo].rgba.toARGB(), tagname_rgba));
								}
							}
						}
					}
					//メッシュ情報を出力する必要があるかチェックする
					if (p_flags2 & PART_FLAG_MESHDATA)
					{
						//頂点情報を出力
						int i;
						int size = state->meshPart->ver_size;
						std::vector<float>& draw_verticesRaw = *(state->meshPart->draw_vertices.get());
						for (i = 0; i < size; i++)
						{
							std::string tagname_mesh_x = tagname + "mesh_" + std::to_string(i) + "_x";
							std::string tagname_mesh_y = tagname + "mesh_" + std::to_string(i) + "_y";
							std::string tagname_mesh_z = tagname + "mesh_" + std::to_string(i) + "_z";

							float mesh_x = draw_verticesRaw[i * 3 + 0];
							float mesh_y = draw_verticesRaw[i * 3 + 1];
							float mesh_z = draw_verticesRaw[i * 3 + 2];
							frameData->add(Lump::floatData(mesh_x, tagname_mesh_x));		//x
							frameData->add(Lump::floatData(mesh_y, tagname_mesh_y));		//y
							frameData->add(Lump::floatData(mesh_z, tagname_mesh_z));		//z
						}
					}
				}
				
				// 出力されたパーツ数と、描画順の変更があるかのフラグ
				frameFlag->data.i = outPartsCount | (prioChanged ? 0x8000 : 0);

			}

			// ユーザーデータ
			Lump* userDataIndexArray = Lump::set("ss::ss_u16*[]", true, "userDataIndexArray");
			bool hasUserData = false;

			for (int frame = 0; frame < decoder.getAnimeTotalFrame(); frame++)
			{
				Lump* userData = Lump::set("ss::ss_u16[]", true, "userData");
				int partsCount = 0;

				SPRITESTUDIO6SDK_foreach(std::vector<spritestudio6::SsPartAndAnime>, decoder.getPartAnime(), it)
				{
					spritestudio6::SsPart* part = it->first;
					spritestudio6::SsPartAnime* partAnime = it->second;
					if (!partAnime) continue;

					SPRITESTUDIO6SDK_foreach(spritestudio6::SsAttributeList, partAnime->attributes, attrIt)
					{
						spritestudio6::SsAttribute* attr = *attrIt;
						if (attr->tag != spritestudio6::SsAttributeKind::user) continue;

						// このフレームのデータを含む?
						if (attr->key_dic.find(frame) != attr->key_dic.end())
						{
							hasUserData = true;
							partsCount++;
							
							const spritestudio6::SsKeyframe* keyframe = attr->key_dic.at(frame);
							spritestudio6::SsUserDataAnime udat;
							GetSsUserDataAnime(keyframe, udat);

							int flags = 0;
							if (udat.useInteger) flags |= USER_DATA_FLAG_INTEGER;
							if (udat.useRect)    flags |= USER_DATA_FLAG_RECT;
							if (udat.usePoint)   flags |= USER_DATA_FLAG_POINT;
							if (udat.useString)  flags |= USER_DATA_FLAG_STRING;

							userData->add(Lump::s16Data(flags, "flags"));
							userData->add(Lump::s16Data(part->arrayIndex, "arrayIndex"));

							if (udat.useInteger)
							{
								userData->add(Lump::s32Data(udat.integer, "integer"));
							}
							if (udat.useRect)
							{
								userData->add(Lump::s32Data(udat.rect.x, "rect_x"));
								userData->add(Lump::s32Data(udat.rect.y, "rect_y"));
								userData->add(Lump::s32Data(udat.rect.w, "rect_w"));
								userData->add(Lump::s32Data(udat.rect.h, "rect_h"));
							}
							if (udat.usePoint)
							{
								userData->add(Lump::s32Data((int)udat.point.x, "point_x"));
								userData->add(Lump::s32Data((int)udat.point.y, "point_y"));
							}
							if (udat.useString)
							{
								const spritestudio6::SsString& str = udat.string;
								userData->add(Lump::s16Data((int)str.length(), "str_length"));
								userData->add(Lump::stringData(str, "str"));
							}
						}
					}
				}
				
				if (partsCount)
				{
					userData->addFirst(Lump::s16Data(partsCount));
					userDataIndexArray->add(userData);
					s_frameIndexVec.push_back(frame);
				}
				else
				{
					userDataIndexArray->add(Lump::s32Data(0));
					delete userData;
				}
			}


			// ラベルデータ
			Lump* LabelDataIndexArray = Lump::set("ss::ss_u16*[]", true, "LabelDataIndexArray");
			bool hasLabelData = false;
			int label_idx = 0;
			for (label_idx = 0; label_idx < (int)anime->labels.size(); label_idx++)
			{
				Lump* labelData = Lump::set("ss::ss_u16[]", true, "labelData");

				spritestudio6::SsString str;
				str = anime->labels[label_idx]->name;
				//全角チェック
				if ( isZenkaku( &str ) == true )
				{
					std::cerr << "エラー：ラベルに全角が使用されています。半角英数でリネームしてください。: " << str << "\n";
					convert_error_exit = true;	//エラーが発生コンバート失敗
					return 0;
				}

//				labelData->add(Lump::s16Data((int)str.length()));				//文字列のサイズ
				labelData->add(Lump::stringData(str, "str"));							//文字列
				labelData->add(Lump::s16Data(anime->labels[label_idx]->time, "time"));	//設定されたフレーム
				hasLabelData = true;

				LabelDataIndexArray->add(labelData);
			}

			if ( hasLabelData == false )
			{
				LabelDataIndexArray->add(Lump::s32Data(0));
			}
			
			animeData->add(Lump::stringData(anime->name, "name"));
			animeData->add(initialDataArray);
			animeData->add(frameDataIndexArray);
			animeData->add(hasUserData ? userDataIndexArray : Lump::s32Data(0, "userDataIndexArray"));
			animeData->add(hasLabelData ? LabelDataIndexArray : Lump::s32Data(0, "LabelDataIndexArray"));
			animeData->add(meshsDataUV);
			animeData->add(meshsDataIndices);
			animeData->add(Lump::s16Data(decoder.getAnimeStartFrame(), "startFrames"));
			animeData->add(Lump::s16Data(decoder.getAnimeEndFrame(), "endFrames"));
			animeData->add(Lump::s16Data(decoder.getAnimeTotalFrame(), "totalFrames"));
			animeData->add(Lump::s16Data(anime->settings.fps, "fps"));
			animeData->add(Lump::s16Data(label_idx, "labelNum"));							//ラベルデータ数
			animeData->add(Lump::s16Data((int)anime->settings.canvasSize.x, "canvasSizeW"));		//基準枠W
			animeData->add(Lump::s16Data((int)anime->settings.canvasSize.y, "canvasSizeH"));		//基準枠H
			animeData->add(Lump::s16Data(0, "reserved"));									//ダミーデータ
			animeData->add(Lump::floatData(anime->settings.pivot.x, "canvasPvotX"));			//基準枠位置
			animeData->add(Lump::floatData(anime->settings.pivot.y, "canvasPvotY"));			//基準枠位置


#if _BACKBUFFER_RENDERING__

			//if (options.outputFormat == OUTPUT_FORMAT_FLAG_SSPKG)
			if (isOpenGLContextInitialized())
			{

				ConverterOpenGLClear();
				//対象フレームを検査して良さそうなところを持ってくる
				decoder.setPlayFrame(10);
				decoder.draw();
				ConverterOpenGLDrawEnd();

				//std::filesystem::path opath = outPath;

				std::string outputfile = outPath + "\\" + anime->name + ".png";

				ConverterOpenGLOutputBitMapImage(outputfile);
			}
#endif

		}
	}

	//エフェクトデータ
	for (int effectIndex = 0; effectIndex < (int)proj->effectfileList.size(); effectIndex++)
	{
		Lump* effectFile = Lump::set("ss::EffectFile", false, "EffectFile");
		effectfileArray->add(effectFile);

		const spritestudio6::SsEffectFile* effectfile = proj->getEffectFile(effectIndex);
		effectFile->add(Lump::stringData(effectfile->name, "name"));				//エフェクト名

		const spritestudio6::SsEffectModel *effectmodel = &effectfile->effectData;
		effectFile->add(Lump::s16Data(effectmodel->fps, "fps"));					//FPS

		effectFile->add(Lump::s16Data(effectmodel->isLockRandSeed, "isLockRandSeed"));		//乱数を固定するかどうか

		effectFile->add(Lump::s16Data(effectmodel->lockRandSeed, "LockRandSeed"));			//固定する場合の乱数の種

		effectFile->add(Lump::s16Data(effectmodel->layoutScaleX, "layoutScaleX"));			//レイアウトスケールX

		effectFile->add(Lump::s16Data(effectmodel->layoutScaleY, "layoutScaleY"));			//レイアウトスケールY

																			//エフェクトノードの出力
		effectFile->add(Lump::s16Data((int)effectmodel->nodeList.size(), "numNodeList"));	//エフェクトノード数


		Lump* effectNodeArray = Lump::set("ss::EffectNode[]", true, "EffectNodeArray");
		effectFile->add(effectNodeArray);									//ノード配列

		for (size_t nodeindex = 0; nodeindex < effectmodel->nodeList.size(); nodeindex++)
		{
			//エフェクトノードを追加
			Lump* effectNode = Lump::set("ss::EffectNode", false, "EffectNode");
			effectNodeArray->add(effectNode);

			spritestudio6::SsEffectNode *node = effectmodel->nodeList[nodeindex];
			int	arrayIndex = node->arrayIndex;				//通し番号
			int	parentIndex = node->parentIndex;			//親の番号
			spritestudio6::SsEffectNodeType::_enum	type = node->type;		//ノードの種類
			//			bool visible = = node->visible;					//エディター用
			//MEMO: 旧ソースではbehaviorが実体にしてnode->behaviorの内容をコピーしていたが……スマートポインタ化後は
			//      所有権の問題でエラーがでるので注意（スマートポインタの所有権の関係）
			spritestudio6::SsEffectBehavior& behavior = node->behavior;		//動作パラメータ
			spritestudio6::SsRenderBlendType::_enum blendType = behavior.BlendType;	//描画方法
			//セル番号
			spritestudio6::SsCell*	refCell = behavior.refCell;
			int cellIndex = -1;
			if (refCell)
			{
				cellIndex = (*cellList)[refCell];
			}
			spritestudio6::SsString CellName = behavior.CellName;
			spritestudio6::SsString CellMapName = behavior.CellMapName;
			//ファイルへ書き出し
			effectNode->add(Lump::s16Data(arrayIndex, "arrayIndex"));		//通し番号
			effectNode->add(Lump::s16Data(parentIndex, "parentIndex"));	//親の番号
			effectNode->add(Lump::s16Data(type, "type"));			//ノードの種類
			effectNode->add(Lump::s16Data(cellIndex, "cellIndex"));		//セルの番号
			effectNode->add(Lump::s16Data(blendType, "blendType"));		//描画方法
			effectNode->add(Lump::s16Data((int)(behavior.plist.size()), "numBehavior"));	//コマンドパラメータ数

			Lump* effectBehaviorArray = Lump::set("ss::ss_u16*[]", true, "effectBehaviorArray");
			effectNode->add(effectBehaviorArray);			//コマンドパラメータ配列

			//コマンドパラメータ
			for (size_t plistindex = 0; plistindex < behavior.plist.size(); plistindex++)
			{
				Lump* effectBehavior = Lump::set("ss::ss_u16[]", true, "effectBehavior");
				effectBehaviorArray->add(effectBehavior);

				spritestudio6::SsEffectElementBase *elementbase = behavior.plist[plistindex].get();
				spritestudio6::SsEffectFunctionType::enum_ myType = elementbase->myType;
				effectBehavior->add(Lump::s32Data(myType, "SsEffectFunctionType"));	//コマンドタイプ

				switch (myType)
				{
				case spritestudio6::SsEffectFunctionType::Basic:
				{
					//基本情報
					spritestudio6::ParticleElementBasic *element = (spritestudio6::ParticleElementBasic*)elementbase;

					int							maximumParticle = element->maximumParticle;
					spritestudio6::f32VValue	speed = element->speed;
					spritestudio6::i32VValue 	lifespan = element->lifespan;
					float						angle = element->angle;
					float						angleVariance = element->angleVariance;
					int							interval = element->interval;
					int							lifetime = element->lifetime;
					int							attimeCreate = element->attimeCreate;
					int							priority = element->priority;
					float						speedMinValue = speed.getMinValue();	//初速最小
					float						speedMaxValue = speed.getMaxValue();	//初速最大
					int							lifespanMinValue = lifespan.getMinValue();	//パーティクル生存時間最小
					int							lifespanMaxValue = lifespan.getMaxValue();	//パーティクル生存時間最大

					effectBehavior->add(Lump::s32Data(priority, "priority"));				//表示優先度
					effectBehavior->add(Lump::s32Data(maximumParticle, "maximumParticle"));		//最大パーティクル数
					effectBehavior->add(Lump::s32Data(attimeCreate, "attimeCreate"));			//一度に作成するパーティクル数
					effectBehavior->add(Lump::s32Data(interval, "interval"));				//生成間隔
					effectBehavior->add(Lump::s32Data(lifetime, "lifetime"));				//エミッター生存時間
					effectBehavior->add(Lump::floatData(speedMinValue, "speedMinValue"));		//初速最小
					effectBehavior->add(Lump::floatData(speedMaxValue, "speedMaxValue"));		//初速最大
					effectBehavior->add(Lump::s32Data(lifespanMinValue, "lifespanMinValue"));		//パーティクル生存時間最小
					effectBehavior->add(Lump::s32Data(lifespanMaxValue, "lifespanMaxValue"));		//パーティクル生存時間最大
					effectBehavior->add(Lump::floatData(angle, "angle"));				//射出方向
					effectBehavior->add(Lump::floatData(angleVariance, "angleVariance"));		//射出方向範囲
					break;
				}
				case spritestudio6::SsEffectFunctionType::RndSeedChange:
				{
					//シード上書き
					spritestudio6::ParticleElementRndSeedChange *element = (spritestudio6::ParticleElementRndSeedChange*)elementbase;
					int		Seed = element->Seed;
					effectBehavior->add(Lump::s32Data(Seed, "Seed"));					//上書きする値
					break;
				}
				case spritestudio6::SsEffectFunctionType::Delay:
				{
					//発生：タイミング
					spritestudio6::ParticleElementDelay *element = (spritestudio6::ParticleElementDelay*)elementbase;
					int		DelayTime = element->DelayTime;
					effectBehavior->add(Lump::s32Data(DelayTime, "DelayTime"));				//遅延時間
					break;
				}
				case spritestudio6::SsEffectFunctionType::Gravity:
				{
					//重力を加える
					spritestudio6::ParticleElementGravity *element = (spritestudio6::ParticleElementGravity*)elementbase;
					spritestudio6::SsVector2   Gravity = element->Gravity;
					effectBehavior->add(Lump::floatData(Gravity.x, "Gravity_x"));				//X方向の重力
					effectBehavior->add(Lump::floatData(Gravity.y, "Gravity_y"));				//Y方向の重力
					break;
				}
				case spritestudio6::SsEffectFunctionType::Position:
				{
					//座標：生成時
					spritestudio6::ParticleElementPosition *element = (spritestudio6::ParticleElementPosition*)elementbase;
					spritestudio6::f32VValue OffsetX = element->OffsetX;
					spritestudio6::f32VValue OffsetY = element->OffsetY;
					effectBehavior->add(Lump::floatData(OffsetX.getMinValue(), "OffsetXMinValue"));				//X座標に加算最小
					effectBehavior->add(Lump::floatData(OffsetX.getMaxValue(), "OffsetXMaxValue"));				//X座標に加算最大
					effectBehavior->add(Lump::floatData(OffsetY.getMinValue(), "OffsetYMinValue"));				//X座標に加算最小
					effectBehavior->add(Lump::floatData(OffsetY.getMaxValue(), "OffsetYMaxValue"));				//X座標に加算最大
					break;
				}
				case spritestudio6::SsEffectFunctionType::Rotation:
				{
					//Z回転を追加
					spritestudio6::ParticleElementRotation *element = (spritestudio6::ParticleElementRotation*)elementbase;
					spritestudio6::f32VValue Rotation = element->Rotation;
					spritestudio6::f32VValue RotationAdd = element->RotationAdd;
					effectBehavior->add(Lump::floatData(Rotation.getMinValue(), "RotationMinValue"));			//角度初期値最小
					effectBehavior->add(Lump::floatData(Rotation.getMaxValue(), "RotationMaxValue"));			//角度初期値最大
					effectBehavior->add(Lump::floatData(RotationAdd.getMinValue(), "RotationAddMinValue"));			//角度初期加算値最小
					effectBehavior->add(Lump::floatData(RotationAdd.getMaxValue(), "RotationAddMaxValue"));			//角度初期加算値最大
					break;
				}
				case spritestudio6::SsEffectFunctionType::TransRotation:
				{
					//Z回転速度変更
					spritestudio6::ParticleElementRotationTrans *element = (spritestudio6::ParticleElementRotationTrans*)elementbase;
					float   RotationFactor = element->RotationFactor;
					float	EndLifeTimePer = element->EndLifeTimePer;
					effectBehavior->add(Lump::floatData(RotationFactor, "RotationFactor"));					//角度目標加算値
					effectBehavior->add(Lump::floatData(EndLifeTimePer, "EndLifeTimePer"));					//到達時間
					break;
				}
				case spritestudio6::SsEffectFunctionType::TransSpeed:
				{
					//速度：変化
					spritestudio6::ParticleElementTransSpeed *element = (spritestudio6::ParticleElementTransSpeed*)elementbase;
					spritestudio6::f32VValue Speed = element->Speed;
					effectBehavior->add(Lump::floatData(Speed.getMinValue(), "SpeedMinValue"));				//速度目標値最小
					effectBehavior->add(Lump::floatData(Speed.getMaxValue(), "SpeedMaxValue"));				//速度目標値最大
					break;
				}
				case spritestudio6::SsEffectFunctionType::TangentialAcceleration:
				{
					//接線加速度
					spritestudio6::ParticleElementTangentialAcceleration *element = (spritestudio6::ParticleElementTangentialAcceleration*)elementbase;
					spritestudio6::f32VValue Acceleration = element->Acceleration;

					effectBehavior->add(Lump::floatData(Acceleration.getMinValue(), "AccelerationMinValue"));		//設定加速度最小
					effectBehavior->add(Lump::floatData(Acceleration.getMaxValue(), "AccelerationMaxValue"));		//設定加速度最大
					break;
				}
				case spritestudio6::SsEffectFunctionType::InitColor:
				{
					//カラーRGBA：生成時
					spritestudio6::ParticleElementInitColor *element = (spritestudio6::ParticleElementInitColor*)elementbase;
					spritestudio6::SsU8cVValue Color = element->Color;
					effectBehavior->add(Lump::s32Data(Color.getMinValue().toARGB(), "ColorMinValue"));		//設定カラー最小
					effectBehavior->add(Lump::s32Data(Color.getMaxValue().toARGB(), "ColorMaxValue"));		//設定カラー最大
					break;
				}
				case spritestudio6::SsEffectFunctionType::TransColor:
				{
					//カラーRGB：変化
					spritestudio6::ParticleElementTransColor *element = (spritestudio6::ParticleElementTransColor*)elementbase;
					spritestudio6::SsU8cVValue Color = element->Color;
					effectBehavior->add(Lump::s32Data(Color.getMinValue().toARGB(), "ColorMinValue"));		//設定カラー最小
					effectBehavior->add(Lump::s32Data(Color.getMaxValue().toARGB(), "ColorMaxValue"));		//設定カラー最大
					break;
				}
				case spritestudio6::SsEffectFunctionType::AlphaFade:
				{
					//フェード
					spritestudio6::ParticleElementAlphaFade *element = (spritestudio6::ParticleElementAlphaFade*)elementbase;
					spritestudio6::f32VValue  disprange = element->disprange; // mnagaku 頭小文字
					effectBehavior->add(Lump::floatData(disprange.getMinValue(), "disprangeMinValue"));			//表示区間開始
					effectBehavior->add(Lump::floatData(disprange.getMaxValue(), "disprangeMaxValue"));			//表示区間終了
					break;
				}
				case spritestudio6::SsEffectFunctionType::Size:
				{
					//スケール：生成時
					spritestudio6::ParticleElementSize *element = (spritestudio6::ParticleElementSize*)elementbase;
					spritestudio6::f32VValue SizeX = element->SizeX;
					spritestudio6::f32VValue SizeY = element->SizeY;
					spritestudio6::f32VValue ScaleFactor = element->ScaleFactor;
					effectBehavior->add(Lump::floatData(SizeX.getMinValue(), "SizeXMinValue"));				//幅倍率最小
					effectBehavior->add(Lump::floatData(SizeX.getMaxValue(), "SizeXMaxValue"));				//幅倍率最大
					effectBehavior->add(Lump::floatData(SizeY.getMinValue(), "SizeYMinValue"));				//高さ倍率最小
					effectBehavior->add(Lump::floatData(SizeY.getMaxValue(), "SizeYMaxValue"));				//高さ倍率最大
					effectBehavior->add(Lump::floatData(ScaleFactor.getMinValue(), "ScaleFactorMinValue"));			//倍率最小
					effectBehavior->add(Lump::floatData(ScaleFactor.getMaxValue(), "ScaleFactorMaxValue"));			//倍率最大
					break;
				}
				case spritestudio6::SsEffectFunctionType::TransSize:
				{
					//スケール：変化
					spritestudio6::ParticleElementTransSize *element = (spritestudio6::ParticleElementTransSize*)elementbase;
					spritestudio6::f32VValue SizeX = element->SizeX;
					spritestudio6::f32VValue SizeY = element->SizeY;
					spritestudio6::f32VValue ScaleFactor = element->ScaleFactor;
					effectBehavior->add(Lump::floatData(SizeX.getMinValue(), "SizeXMinValue"));				//幅倍率最小
					effectBehavior->add(Lump::floatData(SizeX.getMaxValue(), "SizeXMaxValue"));				//幅倍率最大
					effectBehavior->add(Lump::floatData(SizeY.getMinValue(), "SizeYMinValue"));				//高さ倍率最小
					effectBehavior->add(Lump::floatData(SizeY.getMaxValue(), "SizeYMaxValue"));				//高さ倍率最大
					effectBehavior->add(Lump::floatData(ScaleFactor.getMinValue(), "ScaleFactorMinValue"));			//倍率最小
					effectBehavior->add(Lump::floatData(ScaleFactor.getMaxValue(), "ScaleFactorMaxValue"));			//倍率最大
					break;
				}
				case spritestudio6::SsEffectFunctionType::PointGravity:
				{
					//重力点の追加
					spritestudio6::ParticlePointGravity *element = (spritestudio6::ParticlePointGravity*)elementbase;
					spritestudio6::SsVector2   Position = element->Position;
					float		Power = element->Power;
					effectBehavior->add(Lump::floatData(Position.x, "Position_x"));						//重力点X
					effectBehavior->add(Lump::floatData(Position.y, "Position_y"));						//重力点Y
					effectBehavior->add(Lump::floatData(Power, "Power"));							//パワー
					break;
				}
				case spritestudio6::SsEffectFunctionType::TurnToDirectionEnabled:
				{
					//進行方向に向ける
					spritestudio6::ParticleTurnToDirectionEnabled *element = (spritestudio6::ParticleTurnToDirectionEnabled*)elementbase;
					//コマンドがあれば有効
					effectBehavior->add(Lump::floatData(element->Rotation, "Rotation"));				//方向オフセット
					break;
				}
				case spritestudio6::SsEffectFunctionType::InfiniteEmitEnabled:
				{
					//無限にする
					spritestudio6::ParticleInfiniteEmitEnabled *element = (spritestudio6::ParticleInfiniteEmitEnabled*)elementbase;
					//コマンドがあれば有効
					effectBehavior->add(Lump::s32Data(1, "flag"));									//ダミーデータ
					break;
				}
				case spritestudio6::SsEffectFunctionType::Base:
				default:
					//未使用のコマンドが含まれている
					std::cerr << "警告：未使用のエフェクトコマンドが含まれています。 \n";
					break;
				}
			}
		}
	}
	
	

	// シーケンス情報
	parseParts_ssqe( topLump, proj, imageBaseDir );

	std::cerr << "convert end" << "\n";

	return topLump;
}



//void convertProject(const std::string& outPath, const std::string& outFName,
//	LumpExporter::StringEncoding encoding, const std::string& sspjPath,
//	const std::string& imageBaseDir, const std::string& creatorComment, const int outputFormat)

void convertProject(const std::string& outPath, const std::string& outFName,
	LumpExporter::StringEncoding encoding, const std::string& sspjPath,
	Options& options, const std::string& creatorComment)

{
	const std::string imageBaseDir = options.imageBaseDir;
	const int outputFormat = options.outputFormat;



	std::cerr << convert_console_string( sspjPath ) << "\n";
	spritestudio6::SsProject* proj = spritestudio6::ssloader_sspj::Load(sspjPath);
	Lump* lump;
	try
	{
		if (proj)
		{
			lump = parseParts(proj, imageBaseDir, outPath);
		}
		else
		{
			convert_error_exit = true;	//エラーが発生
		}
	}
	catch (...)
	{
		convert_error_exit = true;	//エラーが発生
	}

	if ( convert_error_exit == true )
	{
		//データにエラーがありコンバートを中止した
		//ファイルの出力を行なわない
		std::cerr << "データにエラーがありコンバートを中止しました \n";
	}
	else
	{
		std::fstream out;
		static const std::string messageErrorFileOpen = "出力ファイルのオープンに失敗しました: ";

		if (outputFormat == OUTPUT_FORMAT_FLAG_JSON)
		{
//			out.open((outPath + ".json").c_str(), std::ios_base::out);
			std::string outPathJson = outPath + outFName + ".json";

			out.open((spritestudio6::SsCharConverter::convert_path_string( outPathJson )).c_str()
						, std::ios_base::out);
			if(out)
			{
				LumpExporter::saveJson(out, encoding, lump, creatorComment);
			}
			else
			{
				std::cerr << messageErrorFileOpen << convert_console_string(outPathJson) << std::endl;
			}
		}
		else if (outputFormat == OUTPUT_FORMAT_FLAG_CSOURCE)
		{
			// out.open((outPath + ".c").c_str(), std::ios_base::out);
			// LumpExporter::saveCSource(out, encoding, lump, "topLabel", creatorComment);
			std::cerr << "*** OBSOLETE C LANGUAGE SOURCE FORMAT. ***"  << std::endl;
		}
		else if (outputFormat == OUTPUT_FORMAT_FLAG_SSFB)
		{
//			out.open((outPath + ".ssfb").c_str(), std::ios_base::binary | std::ios_base::out);
			std::string outPathSsfb = outPath + outFName + ".ssfb";

			out.open((spritestudio6::SsCharConverter::convert_path_string(outPathSsfb)).c_str()
						, std::ios_base::binary | std::ios_base::out);
			if(out)
			{
				LumpExporter::saveSsfb(out, encoding, lump, creatorComment, s_frameIndexVec);
			}
			else
			{
				std::cerr << messageErrorFileOpen << convert_console_string(outPathSsfb) << std::endl;
			}
		}
		else if (outputFormat == OUTPUT_FORMAT_FLAG_SSPKG)
		{
//			std::string outPathSsfb = outPath + outFName + ".sspkg";
			std::string outPathSsfb = outPath + outFName + ".ssfb";//出力はFB

			out.open((spritestudio6::SsCharConverter::convert_path_string(outPathSsfb)).c_str()
				, std::ios_base::binary | std::ios_base::out);
			if (out)
			{
				LumpExporter::saveSsfb(out, encoding, lump, creatorComment, s_frameIndexVec);
			}
			else
			{
				std::cerr << messageErrorFileOpen << convert_console_string(outPathSsfb) << std::endl;
			}
		}
		else
		{
			std::string outPathSsfb = outPath + outFName + ".ssbp";
			out.open((spritestudio6::SsCharConverter::convert_path_string(outPathSsfb)).c_str()
						, std::ios_base::binary | std::ios_base::out);
			if(out)
			{
				LumpExporter::saveBinary(out, encoding, lump, creatorComment);
			}
			else
			{
				std::cerr << messageErrorFileOpen << convert_console_string(outPathSsfb) << std::endl;
			}
		}

	/////////////
	#if 0
		out.close();
		std::string hOutPath = outPath + ".h";
		out.open(hOutPath.c_str(), std::ios_base::binary | std::ios_base::out);
		LumpExporter::saveCSource(out, encoding, lump, "animeData", creatorComment);
	#endif
	/////////////

	//	delete lump;
		delete proj;
	}
}








#define APP_NAME		"Ss6Converter"
#define APP_VERSION		SPRITESTUDIOSDK_VERSION " (Build: " __DATE__ " " __TIME__ ")"


static const char* HELP =
"\n"
APP_NAME " converter version " APP_VERSION "\n"
"usage: " APP_NAME " Input files(.sspj) ...\n"
"\n"
"option:\n"
"  -h      Display usage.\n"
"  -v      Verbose mode.\n"
"  -o      set output path.\n"
//"  -e arg  Encoding of output file (UTF8/SJIS) default:UTF8\n"
//"  -p arg  Specify image file load base path.\n"
"  -pkg    sspkg output mode\n"
"  -f      set output format.\n"
"  usage exsample : " APP_NAME " -o <outputpath> -f < json , ssfb , c , sspkg> <input file name path>\n"
"\n";





class ArgumentPointer
{
	int _argc;
	const char** _argv;
	int _index;

public:
	ArgumentPointer(int argc, const char* argv[])
	: _argc(argc), _argv(argv), _index(1)
	{}

	bool hasNext() const
	{
		return _index < _argc;
	}
	
	std::string next()
	{
		assert(hasNext());
		return std::string(_argv[_index++]);
	}
};


bool parseOption(Options& options, const std::string& opt, ArgumentPointer& args, std::string& illegalArgument)
{
	if (opt == "-h")
	{
		options.isHelp = true;
	}
	else if (opt == "-v")
	{
		options.isVerbose = true;
	}
/*
	//UTF8固定にする
	else if (opt == "-e")
	{
		if (!args.hasNext()) return false;

		// 大文字に変換してからチェックする
		std::string enc = args.next();
		std::string uppered = enc;
		std::transform(uppered.begin(), uppered.end(), uppered.begin(), toupper);
		
		if      (uppered == "UTF8") options.encoding = LumpExporter::UTF8;
		else if (uppered == "SJIS") options.encoding = LumpExporter::SJIS;
		else
		{
			// not support encoding
			illegalArgument = opt + " " + enc;
			return false;
		}
	}
*/
/*
	else if (opt == "-p")
	{
		if (!args.hasNext()) return false;

		options.imageBaseDir = args.next();
	}
*/

	else if (opt == "-o")
	{
		if (!args.hasNext()) return false;

		options.outputDir = args.next();
	}
	else if (opt == "-f")
	{
		if (!args.hasNext()) return false;

		std::string outputFormat = args.next();
		if (outputFormat == "json") options.outputFormat = OUTPUT_FORMAT_FLAG_JSON;
		else if (outputFormat == "c") options.outputFormat = OUTPUT_FORMAT_FLAG_CSOURCE;
		else if (outputFormat == "ssfb") options.outputFormat = OUTPUT_FORMAT_FLAG_SSFB;
		else if (outputFormat == "sspkg") options.outputFormat = OUTPUT_FORMAT_FLAG_SSPKG;
	}
#ifdef _WIN32
	else if (opt == "-c")
	{
		if (!args.hasNext()) return false;

		std::string argumentEncode = args.next();
		if (argumentEncode == "utf8") options.argumentEncode = ARGUMENT_ENCODE_UTF8;
		else if (argumentEncode == "sjis") options.argumentEncode = ARGUMENT_ENCODE_SJIS;
	}
#else
#endif	/* def _WIN32 */
	else
	{
		// unknown
		illegalArgument = opt;
		return false;
	}





	// success
	return true;
}


bool parseArguments(Options& options, int argc, const char* argv[], std::string& illegalArgument)
{
	//オプションフラグの初期化
	options.outputDir = "";
	options.imageBaseDir = "";
	options.outputFormat = 0;
#ifdef _WIN32
	options.argumentEncode = ARGUMENT_ENCODE_SJIS;
#else
	options.argumentEncode = ARGUMENT_ENCODE_UTF8;
#endif	/* def _WIN32 */

	//引数解析
	Options::StringList inList;
	ArgumentPointer args(argc, argv);
	if (!args.hasNext())
	{
		options.isHelp = true;
		return true;
	}

	while (args.hasNext())
	{
		std::string name = args.next();
		if (name[0] == '-')
		{
			bool success = parseOption(options, name, args, illegalArgument);
			if (!success)
			{
				return false;
			}
		}
		else
		{
//			inList.push_back(name);
			std::string nameUTF8;
			switch(options.argumentEncode)
			{
			case ARGUMENT_ENCODE_SJIS:
#ifdef _WIN32
				nameUTF8 = 	spritestudio6::SsCharConverter::sjis_to_utf8(name);;
				break;
#else
				/* Fall-Through */
#endif	/* def _WIN32 */

			case ARGUMENT_ENCODE_UTF8:
			default:
				nameUTF8 = name;
				break;
			}

			inList.push_back(nameUTF8);
		}
	}

	options.inList = inList;
	// success
	return true;
}




#include <memory>

int convertMain(int argc, const char * argv[])
{
	// 引数をパースする
	Options options;
	std::string illegalArgument;
	bool success = parseArguments(options, argc, argv, illegalArgument);
	if (!success)
	{
		// 引数が不正
		std::cerr << "Invalid arguments: " << illegalArgument << std::endl;
		std::cout << HELP;
		return SSPC_ILLEGAL_ARGUMENT;
	}

	if (options.isHelp)
	{
        // usage表示。正常終了にしています。
		std::cout << HELP;
		return SSPC_SUCCESS;
	}


#ifdef _BACKBUFFER_RENDERING__
	std::unique_ptr<spritestudio6::SSTextureFactory> texFactory;
	//spritestudio6::SSTextureFactory* texFactory = nullptr;

	if (options.outputFormat == OUTPUT_FORMAT_FLAG_SSPKG)
	{
		if (!ConverterOpenGLInit())
		{
			texFactory.reset(
				new spritestudio6::SSTextureFactory(
						new spritestudio6::SSTextureBMP()
				)
			);
		}
	}
#endif

	// *** 入力ファイル名チェック
	std::vector<std::string> sources;
	{
		Options::StringList& in = options.inList;
	
		bool error = false;
		for (Options::StringList::iterator it = in.begin(); it != in.end(); it++)
		{
			const std::string& str = *it;
		
#ifdef _WIN32
			// Win32プラットフォーム用コード。Win32APIを使ってワイルドカード展開する
			// MEMO: FileUtilで使用している文字コードはSJISであることに注意
			std::vector<std::string> fileList = FileUtil::findPath(spritestudio6::SsCharConverter::utf8_to_sjis(str));
			if (!fileList.empty())
			{
				for(std::vector<std::string>::iterator it=fileList.begin(); it != fileList.end(); it++)
				{
					sources.push_back(spritestudio6::SsCharConverter::sjis_to_utf8(*it));
				}
			}
			else
			{
				std::cerr << "Cannot find input file: " << convert_console_string(str) << std::endl;
				error = true;
			}
#else
			// Mac/Unixプラットフォーム用コード
			// 本当にファイルが存在するか確認し、見つからないものがあるときはエラーとする
			struct stat st;
			int result = stat(str.c_str(), &st);
			if (result == 0)
			{
				sources.push_back(str);
			}
			else
			{
				std::cerr << "Cannot find input file: " << str << std::endl;
				error = true;
			}
#endif
		}
		if (error)
        {
            return SSPC_NOT_EXIST_INPUT_FILE;
        }
	}
	
	
	
	std::string creatorComment = "Created by " APP_NAME " " APP_VERSION;
	LumpExporter::StringEncoding encoding = options.encoding;
	
	// コンバート実行
	for (std::vector<std::string>::const_iterator it = sources.begin(); it != sources.end(); it++)
	{
		std::string sspjPath = *it;

		//std::string outPath = FileUtil::replaceExtension(sspjPath, ".sspj", ".ssbp");

		std::string outPath = FileUtil::getFilePath(sspjPath);
		std::string outFName = FileUtil::getFileName(sspjPath);//拡張子なし
		
		//パスが指定されている場合
		if ( options.outputDir != "" )
		{

			outPath = FileUtil::normalizeFilePath(options.outputDir);

#if 0
			int st = 0;
#ifdef _WIN32
			st = (int)(outPath.find_last_of("\\"));
#else
            st = (int)(outPath.find_last_of("/"));
#endif
			std::string ssbpname = outPath.substr(st+1);

#ifdef _WIN32
			if ( options.outputDir.substr(options.outputDir.length() - 1) != "\\" )
#else
			if ( options.outputDir.substr(options.outputDir.length() - 1) != "/" )
#endif
			{
				//最後の１文字が"\\"でない場合付加する
#ifdef _WIN32
				options.outputDir = options.outputDir + "\\";
#else
                options.outputDir = options.outputDir + "/";
#endif
			}

//			outPath = options.outputDir + ssbpname;

			outPath = options.outputDir;
#endif
		}

		if (options.isVerbose)
		{
			std::cout << "Convert: " << sspjPath << " -> " << outPath << std::endl;
		}
		
//		convertProject(outPath , outFName , encoding, sspjPath, options.imageBaseDir, creatorComment, options.outputFormat);
		convertProject(outPath, outFName, encoding, sspjPath, options, creatorComment);
	}

	if ( convert_error_exit == true )
	{
		//データにエラーがありコンバートを中止した
	    return SSPC_SSPJ_PARSE_FAILED;
	}
	else
	{
	    return SSPC_SUCCESS;
	}
}




int main(int argc, const char * argv[])
{

	int resultCode = convertMain(argc, argv);
	return resultCode;
}


