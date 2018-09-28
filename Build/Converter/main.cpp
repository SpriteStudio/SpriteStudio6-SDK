//
//  main.cpp
//  Converter
//
//  Created by Hiroki Azumada on 2014/01/07.
//  Copyright (c) 2014年 Hiroki Azumada. All rights reserved.
//

// TODO
#include "ssloader.h"
#include "babel/babel.h"
#include "ssplayer_animedecode.h"
#include "ssHelper.h"
#include "sshTextureBMP.h"
#include "ssplayer_mesh.h"

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

#include "flatbuffers/flatbuffers.h"
#include "flatbuffers/util.h"
#include "ssfb_generated.h"


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
};

bool convert_error_exit = false;	//データにエラーがありコンバートを中止した

union converter32 {
	int i;
	unsigned int ui;
	float f;
};
converter32 c32;


typedef std::map<const SsCell*, int> CellList;

CellList* makeCellList(SsProject* proj)
{
	// セルデータの出力と、全てセルデータを集約したリストを作る
	CellList* cellList = new std::map<const SsCell*, int>();
	int cellListIndex = 0;

	for (size_t mapIndex = 0; mapIndex < proj->cellmapList.size(); mapIndex++)
	{
		const SsCellMap* cellMap = proj->cellmapList[mapIndex];

		for (size_t cellIndex = 0; cellIndex < cellMap->cells.size(); cellIndex++)
		{
			const SsCell* cell = cellMap->cells[cellIndex];
			cellList->insert(CellList::value_type(cell, cellListIndex++));
		}
	}
	
	return cellList;
}


static const SsKeyframe* findDefaultKeyframe(SsAnimeDecoder& decoder, int partIndex, SsAttributeKind::_enum tag)
{
	foreach(std::vector<SsPartAndAnime>, decoder.getPartAnime(), it)
	{
		SsPartAnime* partAnime = it->second;
		SsPart* part = it->first;
		if (part->arrayIndex != partIndex) continue;
		
		foreach(SsAttributeList, partAnime->attributes, attrIt)
		{
			SsAttribute* attr = *attrIt;
			if (attr->tag != tag) continue;
			
			const SsKeyframe* key = attr->firstKey();
			return key;
		}
		
	}
	return NULL;
}


static SsAttribute* findAttribute(SsPartAnime* partAnime, SsAttributeKind::_enum tag)
{
	foreach(SsAttributeList, partAnime->attributes, attrIt)
	{
		SsAttribute* attr = *attrIt;
		if (attr->tag == tag) return attr;
	}
	return NULL;
}


static const SsKeyframe* findFirstKey(SsPartAnime* partAnime, SsAttributeKind::_enum tag)
{
	SsAttribute* attr = findAttribute(partAnime, tag);
	if (attr)
	{
		return attr->firstKey();
	}
	return NULL;
}


static const SsPartState* findState(std::list<SsPartState*>& partList, int partIndex)
{
	foreach (std::list<SsPartState*>, partList, it)
	{
		const SsPartState* state = *it;
		if (state->index == partIndex) return state;
	}
	return NULL;
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
bool isZenkaku( const SsString* str )
{
	bool rc = false;
	int i = 0;
	int size = str->length();
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

static flatbuffers::Offset<ss::ssfb::ProjectData> ssfbProjectData;
static flatbuffers::FlatBufferBuilder ssfbBuilder;

static std::vector<int16_t> s_frameIndexVec;

static Lump* parseParts(SsProject* proj, const std::string& imageBaseDir)
{
//	static SsPartStateLess _ssPartStateLess;
	std::cerr << SPRITESTUDIOSDK_VERSION << "\n";	//バージョン表記は ssloader.h　にあります。
	std::cerr << "Ss6Converter ssbpFormatVersion=" << CURRENT_DATA_VERSION << "\n";
	std::cerr << "convert start!" << "\n";

	CellList* cellList = makeCellList(proj);

	flatbuffers::Offset<flatbuffers::String> ssfbImageBaseDir;
	std::vector<flatbuffers::Offset<ss::ssfb::Cell>> ssfbCells;
	std::vector<flatbuffers::Offset<ss::ssfb::AnimePackData>> ssfbAnimePacks;
	std::vector<flatbuffers::Offset<ss::ssfb::EffectFile>> ssfbEffectFileList;

	Lump* topLump = Lump::set("ss::ProjectData", true, "ProjectData");

	if (checkFileVersion(proj->version, SPRITESTUDIO6_SSPJVERSION) == false)
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
		ssfbImageBaseDir = ssfbBuilder.CreateString(imageBaseDir);
	}
	else
	{
		topLump->add(Lump::stringData("", "imageBaseDir"));
		ssfbImageBaseDir = ssfbBuilder.CreateString("");
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
		const SsCellMap* cellMap = proj->cellmapList[mapIndex];
		flatbuffers::Offset<ss::ssfb::CellMap> ssfbCellMap;

		Lump* cellMapData = Lump::set("ss::CellMap", true, "CellMap");
		cellMapData->add(Lump::stringData(cellMap->name, "name"));
		auto ssfbCellMapName = ssfbBuilder.CreateString(cellMap->name);
		cellMapData->add(Lump::stringData(cellMap->imagePath, "imagePath"));
		auto ssfbCellMapImagePath = ssfbBuilder.CreateString(cellMap->imagePath);

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

		ssfbCellMap = ss::ssfb::CreateCellMap(ssfbBuilder, ssfbCellMapName, ssfbCellMapImagePath,
											  static_cast<int16_t>(mapIndex), wrapMode, filterMode);

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
			const SsCell* cell = cellMap->cells[cellIndex];

			Lump* cellData = Lump::set("ss::Cell", false, "Cell");
			cellsData->add(cellData);
			
			cellData->add(Lump::stringData(cell->name, "name"));
			auto ssfbCellName = ssfbBuilder.CreateString(cell->name);
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

			auto ssfbCell = ss::ssfb::CreateCell(ssfbBuilder, ssfbCellName, ssfbCellMap,
					cellIndex, cell->pos.x, cell->pos.y, cell->size.x, cell->size.y,
					cell->pivot.x, cell->pivot.y, u1, v1, u2, v2);

			ssfbCells.push_back(ssfbCell);

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
		const SsAnimePack* animePack = proj->animeList[packIndex];
		const SsModel& model = animePack->Model;
		
		// AnimePackData
		Lump* animePackData = Lump::set("ss::AnimePackData", false, "AnimePackData");
		packDataArray->add(animePackData);

		Lump* partDataArray = Lump::set("ss::PartData[]", true, "PartData");
		Lump* animeDataArray = Lump::set("ss::AnimationData[]", true, "AnimationData");

		animePackData->add(Lump::stringData(animePack->name, "name"));
		auto ssfbAnimePackDataName = ssfbBuilder.CreateString(animePack->name);

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

		std::vector<flatbuffers::Offset<ss::ssfb::PartData>> ssfbParts;
		std::vector<flatbuffers::Offset<ss::ssfb::AnimationData>> ssfbAnimations;
		std::vector<flatbuffers::Offset<ss::ssfb::meshDataUV>> ssfbMeshsDataUV;
		std::vector<flatbuffers::Offset<ss::ssfb::meshDataIndices>> ssfbMeshsDataIndices;
		std::vector<flatbuffers::Offset<ss::ssfb::frameDataIndex>> ssfbFrameData;
		std::vector<flatbuffers::Offset<ss::ssfb::userDataPerFrame>> ssfbUserData;
		std::vector<flatbuffers::Offset<ss::ssfb::labelDataItem>> ssfbLabelData;

		// パーツ情報（モデル）の出力
		for (int partIndex = 0; partIndex < (int)model.partList.size(); partIndex++)
		{
			const SsPart* part = model.partList[partIndex];

			// PartData
			Lump* partData = Lump::set("ss::PartData", false, "PartData" );
			partDataArray->add(partData);

			partData->add(Lump::stringData(part->name, "name"));
			auto ssfbPartDataName = ssfbBuilder.CreateString(part->name);

			//全角チェック
			if ( isZenkaku( &part->name ) == true )
			{
				std::cerr << "エラー：パーツ名に全角が使用されています。半角英数でリネームしてください。: " << part->name << "\n";
				convert_error_exit = true;	//エラーが発生コンバート失敗
				return 0;
			}
			partData->add(Lump::s16Data(part->arrayIndex, "index"));
			partData->add(Lump::s16Data(part->parentIndex, "parentIndex"));

			ss::ssfb::SsPartType ssfbSsPartType;
			//5.5対応5.3.5に無いパーツ種別がある場合ワーニングを表示する
			switch (part->type)
			{
			case SsPartType::null:			// null。領域を持たずSRT情報のみ。ただし円形の当たり判定は設定可能。
			case SsPartType::normal:		// 通常パーツ。領域を持つ。画像は無くてもいい。
			case SsPartType::mask:			// 6.0マスクパーツ
			case SsPartType::constraint:	// 6.0コンストレイントパーツ
			case SsPartType::bonepoint:		// 6.0ボーンエフェクトパーツ
			case SsPartType::joint:			// 6.0ジョイントパーツ
			case SsPartType::armature:		// 6.0ボーンパーツ
			case SsPartType::mesh:			// 6.0メッシュパーツ
				partData->add(Lump::s16Data(part->type, "type"));
				ssfbSsPartType = (ss::ssfb::SsPartType)part->type;
				break;
			case SsPartType::instance:		// インスタンス。他アニメ、パーツへの参照。シーン編集モードの代替になるもの
				//参照アニメのポインタが無い場合はNULLパーツになる。
				{
					SsString packname = part->refAnimePack;
					SsString animename = part->refAnime;
					SsAnimePack* refpack = proj->findAnimationPack(packname);
					SsAnimation* refanime = refpack->findAnimation(animename);
					if (refanime == NULL)
					{
						partData->add(Lump::s16Data(SsPartType::null, "type"));
						ssfbSsPartType = ss::ssfb::SsPartType::SsPartType_Nulltype;
						std::cerr << "警告：参照のないインスタンスパーツが存在します: " << animePack->name << ".ssae " << part->name << "\n";
					}
					else
					{
						partData->add(Lump::s16Data(part->type, "type"));
						ssfbSsPartType = (ss::ssfb::SsPartType)part->type;
					}
				}
				break;
			case SsPartType::effect:		// 5.5エフェクトパーツ
				//参照エフェクト名が空の場合はNULLパーツになる。
				if (part->refEffectName == "")
				{
					partData->add(Lump::s16Data(SsPartType::null, "type"));
					ssfbSsPartType = ss::ssfb::SsPartType::SsPartType_Nulltype;
					//未実装　ワーニングを表示しNULLパーツにする
					std::cerr << "警告：参照のないエフェクトパーツが存在します: " << animePack->name << ".ssae " << part->name << "\n";
				}
				else
				{
					partData->add(Lump::s16Data(part->type, "type"));
					ssfbSsPartType = (ss::ssfb::SsPartType)part->type;
				}
				break;
			default:
				//未対応パーツ　ワーニングを表示しNULLパーツにする
				std::cerr << "警告：未対応のパーツ種別が使われています: " << animePack->name << ".ssae " << part->name << "\n";
				partData->add(Lump::s16Data(SsPartType::null, "type"));
				ssfbSsPartType = ss::ssfb::SsPartType::SsPartType_Nulltype;
				break;
			}
			partData->add(Lump::s16Data(part->boundsType, "boundsType"));
			partData->add(Lump::s16Data(part->alphaBlendType, "alphaBlendType"));
			partData->add(Lump::s16Data(0, "reserved"));	// reserved

			flatbuffers::Offset<flatbuffers::String> ssfbRefAnime;
			//インスタンスアニメ名
			if ( part->refAnime == "" )
			{
				const SsString str = "";
//				partData->add(Lump::s16Data((int)str.length()));				//文字列のサイズ
				partData->add(Lump::stringData(str, "refname"));							//文字列
				ssfbRefAnime = ssfbBuilder.CreateString(str);
			}
			else
			{
				const SsString str = part->refAnimePack + "/" + part->refAnime;
//				partData->add(Lump::s16Data((int)str.length()));				//文字列のサイズ
				partData->add(Lump::stringData(str, "refname"));							//文字列
				ssfbRefAnime = ssfbBuilder.CreateString(str);
			}
			flatbuffers::Offset<flatbuffers::String> ssfbRefEffectName;
			//エフェクト名
			if (part->refEffectName == "")
			{
				const SsString str = "";
				partData->add(Lump::stringData(str, "effectfilename"));							//文字列
				ssfbRefEffectName = ssfbBuilder.CreateString(str);
			}
			else
			{
				const SsString str = part->refEffectName;
				partData->add(Lump::stringData(str, "effectfilename"));							//文字列
				ssfbRefEffectName = ssfbBuilder.CreateString(str);
			}
			//カラーラベル
			const SsString str = part->colorLabel;
			partData->add(Lump::stringData(str, "colorLabel"));								//文字列
			auto ssfbColorLabel = ssfbBuilder.CreateString(str);

			//マスク対象
			partData->add(Lump::s16Data(part->maskInfluence, "maskInfluence"));

			auto ssfbPartDataItem = ss::ssfb::CreatePartData(ssfbBuilder, ssfbPartDataName,
															 static_cast<int16_t>(part->arrayIndex),
															 static_cast<int16_t>(part->parentIndex),
															 ssfbSsPartType, part->boundsType, part->alphaBlendType, ssfbRefAnime, ssfbRefEffectName, ssfbColorLabel,
															 static_cast<int16_t>(part->maskInfluence));
			ssfbParts.push_back(ssfbPartDataItem);
		}

		// アニメ情報の出力
		SsCellMapList* cellMapList = new SsCellMapList();	// SsAnimeDecoderのデストラクタで破棄される
		SsAnimeDecoder decoder;
//		const SsKeyframe* key;

		for (int animeIndex = 0; animeIndex < (int)animePack->animeList.size(); animeIndex++)
		{
			SsAnimePack* animePack = proj->getAnimePackList()[packIndex];
			SsModel* model = &animePack->Model;
			SsAnimation* anime = animePack->animeList[animeIndex];
			
			cellMapList->set(proj, animePack);
			decoder.setAnimation(model, anime, cellMapList, proj);
			std::list<SsPartState*>& partList = decoder.getPartSortList();
			
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
			std::vector<flatbuffers::Offset<ss::ssfb::AnimationInitialData>> ssfbDefaultData;
			foreach(std::vector<SsPartAndAnime>, decoder.getPartAnime(), it)
			{
				SsPart* part = it->first;
//				SsPartAnime* partAnime = it->second;
				
				const SsPartState* state = findState(partList, part->arrayIndex);
				
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
				SsCell * cell = state->cellValue.cell;
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

				auto item = ss::ssfb::CreateAnimationInitialData(ssfbBuilder, init.index,
						init.lowflag, init.highflag, init.priority, init.cellIndex, init.opacity, init.localopacity, init.masklimen,
						init.posX, init.posY, init.posZ, init.pivotX, init.pivotY, init.rotationX, init.rotationY, init.rotationZ,
						init.scaleX, init.scaleY, init.localscaleX, init.localscaleY, init.size_X, init.size_Y,
						init.uv_move_X, init.uv_move_Y, init.uv_rotation, init.uv_scale_X, init.uv_scale_Y, init.boundingRadius,
						init.instanceValue_curKeyframe, init.instanceValue_startFrame, init.instanceValue_endFrame, init.instanceValue_loopNum,
						init.instanceValue_speed, init.instanceValue_loopflag,
						init.effectValue_curKeyframe, init.effectValue_startTime, init.effectValue_speed, init.effectValue_loopflag);
				ssfbDefaultData.push_back(item);
			}

			Lump* meshsDataUV = Lump::set("ss::ss_u16*[]", true, "meshsDataUV");
			{
				decoder.setPlayFrame(0);
				decoder.update();

				foreach(std::vector<SsPartAndAnime>, decoder.getPartAnime(), it)
				{
					SsPart* part = it->first;
					const SsPartState* state = findState(partList, part->arrayIndex);

					//サイズ分のUV出力
					Lump* meshData = Lump::set("ss::ss_u16*[]", true, "meshData");
					meshsDataUV->add(meshData);
					std::vector<float> ssfbUV;

					//メッシュのサイズを書き出す
					if (part->type == SsPartType::mesh)
					{
						int meshsize = state->meshPart->ver_size;
						meshData->add(Lump::s32Data((int)state->meshPart->isBind, "isBind"));	//バインドの有無
						ssfbUV.push_back((float)(int)state->meshPart->isBind);
						meshData->add(Lump::s32Data(meshsize, "meshsize"));	//サイズ
						ssfbUV.push_back(meshsize);
						int i;
						for (i = 0; i < meshsize; i++)
						{
							float u = state->meshPart->uvs[i * 2 + 0];
							float v = state->meshPart->uvs[i * 2 + 1];
							meshData->add(Lump::floatData(u, "u"));
							ssfbUV.push_back(u);
							meshData->add(Lump::floatData(v, "v"));
							ssfbUV.push_back(v);
						}
					}
					else
					{
						meshData->add(Lump::s32Data(0, "isBind"));
						ssfbUV.push_back(0);
					}

					auto serializeSsfbUV = ssfbBuilder.CreateVector(ssfbUV);
					auto item = ss::ssfb::CreatemeshDataUV(ssfbBuilder, serializeSsfbUV);
					ssfbMeshsDataUV.push_back(item);
				}

			}

			Lump* meshsDataIndices = Lump::set("ss::ss_u16*[]", true, "meshsDataIndices");
			{
				decoder.setPlayFrame(0);
				decoder.update();

				foreach(std::vector<SsPartAndAnime>, decoder.getPartAnime(), it)
				{
					SsPart* part = it->first;
					const SsPartState* state = findState(partList, part->arrayIndex);

					//サイズ分のUV出力
					Lump* meshData = Lump::set("ss::ss_u16*[]", true, "meshData");
					meshsDataIndices->add(meshData);
					std::vector<float> ssfbIndices;

					//メッシュのサイズを書き出す
					if (part->type == SsPartType::mesh)
					{
						int tri_size = state->meshPart->tri_size;
						meshData->add(Lump::s32Data(tri_size, "tri_size"));	//サイズ
						ssfbIndices.push_back(tri_size);
						int i;
						for (i = 0; i < tri_size; i++)
						{
							int po1 = (int)state->meshPart->indices[i * 3 + 0];
							int po2 = (int)state->meshPart->indices[i * 3 + 1];
							int po3 = (int)state->meshPart->indices[i * 3 + 2];
							meshData->add(Lump::s32Data(po1, "po1"));
							ssfbIndices.push_back(po1);
							meshData->add(Lump::s32Data(po2, "po2"));
							ssfbIndices.push_back(po2);
							meshData->add(Lump::s32Data(po3, "po3"));
							ssfbIndices.push_back(po3);
						}
					}
					else
					{
						meshData->add(Lump::s32Data(0, "tri_size"));
						ssfbIndices.push_back(0);
					}

					auto serializeSsfbIndices = ssfbBuilder.CreateVector(ssfbIndices);
					auto item = ss::ssfb::CreatemeshDataIndices(ssfbBuilder, serializeSsfbIndices);
					ssfbMeshsDataIndices.push_back(item);
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
				std::vector<float> ssfbFrameData2;
				
				Lump* frameFlag = Lump::s16Data(0, "frameFlag");
//				frameData->add(frameFlag);

				int outPartsCount = 0;
				foreach(std::list<SsPartState*>, partList, it)
				{
					const SsPartState* state = *it;
					//セルに設定された原点補正を取得
					SsVector2 pivot;
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
							case SsColorBlendTarget::whole:
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
							case SsColorBlendTarget::vertex:
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
					if (state->partType == SsPartType::mesh)
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
					ssfbFrameData2.push_back(state->index);
//					frameData->add(Lump::s16Data(0));				//32bitアライメント用ダミーデータ
					frameData->add(Lump::s32Data(s_flags | p_flags, tagname + "flag1"));
					c32.ui = s_flags | p_flags;
					//intで出力すると上位ビットが立った場合に丸めが発生するので、floatで出力し、プレイヤーではbitを整数で扱う事になる
					ssfbFrameData2.push_back(c32.f);	
					frameData->add(Lump::s32Data(p_flags2, tagname + "flag2"));
					c32.ui = p_flags2;
					//intで出力すると上位ビットが立った場合に丸めが発生するので、floatで出力し、プレイヤーではbitを整数で扱う事になる
					ssfbFrameData2.push_back(c32.f);
					
					if (p_flags & PART_FLAG_CELL_INDEX)
					{
						frameData->add(Lump::s16Data(cellIndex, tagname + "cellIndex"));
						ssfbFrameData2.push_back(cellIndex);
					}
					if (p_flags & PART_FLAG_POSITION_X)
					{
						frameData->add(Lump::floatData(state->position.x, tagname + "position_x"));
						ssfbFrameData2.push_back(state->position.x);
					}
					if (p_flags & PART_FLAG_POSITION_Y)
					{
						frameData->add(Lump::floatData(state->position.y, tagname + "position_y"));
						ssfbFrameData2.push_back(state->position.y);
					}
					if (p_flags & PART_FLAG_POSITION_Z)
					{
						frameData->add(Lump::floatData(state->position.z, tagname + "position_z"));
						ssfbFrameData2.push_back(state->position.z);
					}

					if (p_flags & PART_FLAG_PIVOT_X)
					{
						frameData->add(Lump::floatData(pivot.x, tagname + "pivot_x"));
						ssfbFrameData2.push_back(pivot.x);
					}
					if (p_flags & PART_FLAG_PIVOT_Y)
					{
						frameData->add(Lump::floatData(pivot.y, tagname + "pivot_y"));
						ssfbFrameData2.push_back(pivot.y);
					}
					if (p_flags & PART_FLAG_ROTATIONX)
					{
						frameData->add(Lump::floatData(state->rotation.x, tagname + "rotation_x"));	// degree
						ssfbFrameData2.push_back(state->rotation.x);
					}
					if (p_flags & PART_FLAG_ROTATIONY)
					{
						frameData->add(Lump::floatData(state->rotation.y, tagname + "rotation_y"));	// degree
						ssfbFrameData2.push_back(state->rotation.y);
					}
					if (p_flags & PART_FLAG_ROTATIONZ)
					{
						frameData->add(Lump::floatData(state->rotation.z, tagname + "rotation_z"));	// degree
						ssfbFrameData2.push_back(state->rotation.z);
					}
					if (p_flags & PART_FLAG_SCALE_X)
					{
						frameData->add(Lump::floatData(state->scale.x, tagname + "scale_x"));
						ssfbFrameData2.push_back(state->scale.x);
					}
					if (p_flags & PART_FLAG_SCALE_Y)
					{
						frameData->add(Lump::floatData(state->scale.y, tagname + "scale_y"));
						ssfbFrameData2.push_back(state->scale.y);
					}
					if (p_flags & PART_FLAG_LOCALSCALE_X)
					{
						frameData->add(Lump::floatData(state->localscale.x, tagname + "localscale_x"));
						ssfbFrameData2.push_back(state->localscale.x);
					}
					if (p_flags & PART_FLAG_LOCALSCALE_Y)
					{
						frameData->add(Lump::floatData(state->localscale.y, tagname + "localscale_y"));
						ssfbFrameData2.push_back(state->localscale.y);
					}
					if (p_flags & PART_FLAG_OPACITY)
					{
						frameData->add(Lump::s16Data((int)(state->alpha * 255), tagname + "alpha"));
						ssfbFrameData2.push_back(state->alpha * 255);
					}
					if (p_flags & PART_FLAG_LOCALOPACITY)
					{
						frameData->add(Lump::s16Data((int)(state->localalpha * 255), tagname + "localalpha"));
						ssfbFrameData2.push_back(state->localalpha * 255);
					}

					if (p_flags & PART_FLAG_SIZE_X)
					{
						frameData->add(Lump::floatData(state->size.x, tagname + "size_x"));
						ssfbFrameData2.push_back(state->size.x);
					}
					if (p_flags & PART_FLAG_SIZE_Y)
					{
						frameData->add(Lump::floatData(state->size.y, tagname + "size_y"));
						ssfbFrameData2.push_back(state->size.y);
					}

					if (p_flags & PART_FLAG_U_MOVE)
					{
						frameData->add(Lump::floatData(state->uvTranslate.x, tagname + "uvTranslate.x"));
						ssfbFrameData2.push_back(state->uvTranslate.x);
					}
					if (p_flags & PART_FLAG_V_MOVE)
					{
						frameData->add(Lump::floatData(state->uvTranslate.y, tagname + "uvTranslate.y"));
						ssfbFrameData2.push_back(state->uvTranslate.y);
					}
					if (p_flags & PART_FLAG_UV_ROTATION)
					{
						frameData->add(Lump::floatData(state->uvRotation, tagname + "uvRotation"));
						ssfbFrameData2.push_back(state->uvRotation);
					}
					if (p_flags & PART_FLAG_U_SCALE)
					{
						frameData->add(Lump::floatData(state->uvScale.x, tagname + "uvScale_x"));
						ssfbFrameData2.push_back(state->uvScale.x);
					}
					if (p_flags & PART_FLAG_V_SCALE)
					{
						frameData->add(Lump::floatData(state->uvScale.y, tagname + "uvScale_y"));
						ssfbFrameData2.push_back(state->uvScale.y);
					}

					if (p_flags & PART_FLAG_BOUNDINGRADIUS)
					{
						frameData->add(Lump::floatData(state->boundingRadius, tagname + "boundingRadius"));
						ssfbFrameData2.push_back(state->boundingRadius);
					}

					if (p_flags & PART_FLAG_MASK)
					{
						frameData->add(Lump::s16Data(state->masklimen, tagname + "masklimen"));
						ssfbFrameData2.push_back(state->masklimen);
					}
					if (p_flags & PART_FLAG_PRIORITY)
					{
						frameData->add(Lump::s16Data(state->prio, tagname + "prio"));
						ssfbFrameData2.push_back(state->prio);
					}

					//インスタンス情報出力
					if (p_flags & PART_FLAG_INSTANCE_KEYFRAME)
					{
						frameData->add(Lump::s32Data(state->instanceValue.curKeyframe, tagname + "instanceValue_curKeyframe"));
						c32.i = state->instanceValue.curKeyframe;
						ssfbFrameData2.push_back(c32.f);
						frameData->add(Lump::s32Data(state->instanceValue.startFrame, tagname + "instanceValue_startFrame"));
						c32.i = state->instanceValue.startFrame;
						ssfbFrameData2.push_back(c32.f);
						frameData->add(Lump::s32Data(state->instanceValue.endFrame, tagname + "instanceValue_endFrame"));
						c32.i = state->instanceValue.endFrame;
						ssfbFrameData2.push_back(c32.f);
						frameData->add(Lump::s32Data(state->instanceValue.loopNum, tagname + "instanceValue_loopNum"));
						c32.i = state->instanceValue.loopNum;
						ssfbFrameData2.push_back(c32.f);
						frameData->add(Lump::floatData(state->instanceValue.speed, tagname + "instanceValue_speed"));
						ssfbFrameData2.push_back(state->instanceValue.speed);
						frameData->add(Lump::s32Data(state->instanceValue.loopflag, tagname + "instanceValue_loopflag"));
						c32.i = state->instanceValue.loopflag;
						ssfbFrameData2.push_back(c32.f);
					}
					//エフェクト情報出力
					if (p_flags & PART_FLAG_EFFECT_KEYFRAME)
					{
						frameData->add(Lump::s32Data(state->effectValue.curKeyframe, tagname + "effectValue_curKeyframe"));	//キー配置フレーム
						c32.i = state->effectValue.curKeyframe;
						ssfbFrameData2.push_back(c32.f);
						frameData->add(Lump::s32Data(state->effectValue.startTime, tagname + "effectValue_startTime"));	//開始フレーム
						c32.i = state->effectValue.startTime;
						ssfbFrameData2.push_back(c32.f);
						frameData->add(Lump::floatData(state->effectValue.speed, tagname + "effectValue_speed"));		//再生速度
						ssfbFrameData2.push_back(state->effectValue.speed);
						frameData->add(Lump::s32Data(state->effectValue.loopflag, tagname + "effectValue_loopflag"));		//独立動作
						c32.i = state->effectValue.loopflag;
						ssfbFrameData2.push_back(c32.f);
					}


					// 頂点変形データ
					if (p_flags & PART_FLAG_VERTEX_TRANSFORM)
					{
						// どの頂点のオフセット値が格納されているかのフラグ
						frameData->add(Lump::s16Data(vt_flags));
						ssfbFrameData2.push_back(vt_flags);
						
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
									ssfbFrameData2.push_back(state->vertexValue.offsets[vtxNo].x);
									frameData->add(Lump::floatData(state->vertexValue.offsets[vtxNo].y, tagname_y));
									ssfbFrameData2.push_back(state->vertexValue.offsets[vtxNo].y);
								}
								else
								{
									frameData->add(Lump::floatData((int)state->vertexValue.offsets[vtxNo].x, tagname_x));
									ssfbFrameData2.push_back((int)state->vertexValue.offsets[vtxNo].x);
									frameData->add(Lump::floatData((int)state->vertexValue.offsets[vtxNo].y, tagname_y));
									ssfbFrameData2.push_back((int)state->vertexValue.offsets[vtxNo].y);
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
						ssfbFrameData2.push_back(typeAndFlags);
						
						if (cb_flags & VERTEX_FLAG_ONE)
						{
							frameData->add(Lump::floatData(state->partsColorValue.color.rate, tagname + "partsColorValue_color_rate"));
							ssfbFrameData2.push_back(state->partsColorValue.color.rate);
							frameData->add(Lump::colorData(state->partsColorValue.color.rgba.toARGB(), tagname + "partsColorValue_color_rgba"));
							ssfbFrameData2.push_back((state->partsColorValue.color.rgba.toARGB() & 0xffff0000) >> 16);
							ssfbFrameData2.push_back(state->partsColorValue.color.rgba.toARGB() & 0xffff);
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
									ssfbFrameData2.push_back(state->partsColorValue.colors[vtxNo].rate);
									frameData->add(Lump::colorData(state->partsColorValue.colors[vtxNo].rgba.toARGB(), tagname_rgba));
									ssfbFrameData2.push_back((state->partsColorValue.colors[vtxNo].rgba.toARGB() & 0xffff0000) >> 16);
									ssfbFrameData2.push_back(state->partsColorValue.colors[vtxNo].rgba.toARGB() & 0xffff);
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
						for (i = 0; i < size; i++)
						{
							std::string tagname_mesh_x = tagname + "mesh_" + std::to_string(size) + "_x";
							std::string tagname_mesh_y = tagname + "mesh_" + std::to_string(size) + "_y";
							std::string tagname_mesh_z = tagname + "mesh_" + std::to_string(size) + "_z";

							float mesh_x = state->meshPart->draw_vertices[i * 3 + 0];
							float mesh_y = state->meshPart->draw_vertices[i * 3 + 1];
							float mesh_z = state->meshPart->draw_vertices[i * 3 + 2];
							frameData->add(Lump::floatData(mesh_x, tagname_mesh_x));		//x
							ssfbFrameData2.push_back(mesh_x);
							frameData->add(Lump::floatData(mesh_y, tagname_mesh_y));		//y
							ssfbFrameData2.push_back(mesh_y);
							frameData->add(Lump::floatData(mesh_z, tagname_mesh_z));		//z
							ssfbFrameData2.push_back(mesh_z);
						}
					}
				}
				
				// 出力されたパーツ数と、描画順の変更があるかのフラグ
				frameFlag->data.i = outPartsCount | (prioChanged ? 0x8000 : 0);


				auto serializeSsfbFrameData2 = ssfbBuilder.CreateVector(ssfbFrameData2);
				auto item = ss::ssfb::CreateframeDataIndex(ssfbBuilder, serializeSsfbFrameData2);
				ssfbFrameData.push_back(item);
			}

			// ユーザーデータ
			Lump* userDataIndexArray = Lump::set("ss::ss_u16*[]", true, "userDataIndexArray");
			bool hasUserData = false;

			for (int frame = 0; frame < decoder.getAnimeTotalFrame(); frame++)
			{
				Lump* userData = Lump::set("ss::ss_u16[]", true, "userData");
				int partsCount = 0;

				std::vector<flatbuffers::Offset<ss::ssfb::userDataItem>> ssfbUserDataItemData;

				foreach(std::vector<SsPartAndAnime>, decoder.getPartAnime(), it)
				{
					SsPart* part = it->first;
					SsPartAnime* partAnime = it->second;
					if (!partAnime) continue;

					foreach(SsAttributeList, partAnime->attributes, attrIt)
					{
						SsAttribute* attr = *attrIt;
						if (attr->tag != SsAttributeKind::user) continue;

						std::vector<flatbuffers::Offset<void>> ssfbDataArray;
						std::vector<uint8_t> ssfbDataArrayType;

						// このフレームのデータを含む?
						if (attr->key_dic.find(frame) != attr->key_dic.end())
						{
							hasUserData = true;
							partsCount++;
							
							const SsKeyframe* keyframe = attr->key_dic.at(frame);
							SsUserDataAnime udat;
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

								auto item = ss::ssfb::CreateuserDataInteger(ssfbBuilder, udat.integer);
								ssfbDataArray.push_back(item.Union());
								ssfbDataArrayType.push_back(ss::ssfb::USER_DATA_FLAG_INTEGER);
							}
							if (udat.useRect)
							{
								userData->add(Lump::s32Data(udat.rect.x, "rect_x"));
								userData->add(Lump::s32Data(udat.rect.y, "rect_y"));
								userData->add(Lump::s32Data(udat.rect.w, "rect_w"));
								userData->add(Lump::s32Data(udat.rect.h, "rect_h"));

								auto item = ss::ssfb::CreateuserDataRect(ssfbBuilder, udat.rect.x, udat.rect.y, udat.rect.w, udat.rect.h);
								ssfbDataArray.push_back(item.Union());
								ssfbDataArrayType.push_back(ss::ssfb::USER_DATA_FLAG_RECT);
							}
							if (udat.usePoint)
							{
								userData->add(Lump::s32Data((int)udat.point.x, "point_x"));
								userData->add(Lump::s32Data((int)udat.point.y, "point_y"));

								auto item = ss::ssfb::CreateuserDataPoint(ssfbBuilder,
                                                                          static_cast<int32_t>(udat.point.x),
                                                                          static_cast<int32_t>(udat.point.y));
								ssfbDataArray.push_back(item.Union());
								ssfbDataArrayType.push_back(ss::ssfb::USER_DATA_FLAG_POINT);
							}
							if (udat.useString)
							{
								const SsString& str = udat.string;
								userData->add(Lump::s16Data((int)str.length(), "str_length"));
								userData->add(Lump::stringData(str, "str"));

								auto ssfbStr = ssfbBuilder.CreateString(str);
								auto item = ss::ssfb::CreateuserDataString(ssfbBuilder, str.length(), ssfbStr);
								ssfbDataArray.push_back(item.Union());
								ssfbDataArrayType.push_back(ss::ssfb::USER_DATA_FLAG_STRING);
							}

							auto serializeSsfbDataArrayType = ssfbBuilder.CreateVector(ssfbDataArrayType);
							auto serializeSsfbDataArray = ssfbBuilder.CreateVector(ssfbDataArray);
							auto item = ss::ssfb::CreateuserDataItem(ssfbBuilder, static_cast<int16_t>(flags),
                                                                     static_cast<int16_t>(part->arrayIndex),
                                                                     serializeSsfbDataArrayType,
                                                                     serializeSsfbDataArray);
							ssfbUserDataItemData.push_back(item);
						}
					}
				}
				
				if (partsCount)
				{
					userData->addFirst(Lump::s16Data(partsCount));
					userDataIndexArray->add(userData);
					s_frameIndexVec.push_back(frame);

					auto serializeSsfbUserDataItemData = ssfbBuilder.CreateVector(ssfbUserDataItemData);
					auto item = ss::ssfb::CreateuserDataPerFrame(ssfbBuilder, static_cast<int16_t>(frame), serializeSsfbUserDataItemData);
					ssfbUserData.push_back(item);
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

				SsString str;
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
				auto ssfbLabelDataItemName = ssfbBuilder.CreateString(str);
				labelData->add(Lump::s16Data(anime->labels[label_idx]->time, "time"));	//設定されたフレーム
				hasLabelData = true;

				LabelDataIndexArray->add(labelData);

				auto item = ss::ssfb::CreatelabelDataItem(ssfbBuilder, ssfbLabelDataItemName, anime->labels[label_idx]->time);
				ssfbLabelData.push_back(item);
			}

			if ( hasLabelData == false )
			{
				LabelDataIndexArray->add(Lump::s32Data(0));
			}
			
			animeData->add(Lump::stringData(anime->name, "name"));
			auto ssfbAnimationDataName = ssfbBuilder.CreateString(anime->name);
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
			animeData->add(Lump::s16Data(anime->settings.canvasSize.x, "canvasSizeW"));		//基準枠W
			animeData->add(Lump::s16Data(anime->settings.canvasSize.y, "canvasSizeH"));		//基準枠H
			animeData->add(Lump::s16Data(0, "reserved"));									//ダミーデータ
			animeData->add(Lump::floatData(anime->settings.pivot.x, "canvasPvotX"));			//基準枠位置
			animeData->add(Lump::floatData(anime->settings.pivot.y, "canvasPvotY"));			//基準枠位置

			auto serializeSsfbDefaultData = ssfbBuilder.CreateVector(ssfbDefaultData);
			auto serializeSsfbMeshsDataUV = ssfbBuilder.CreateVector(ssfbMeshsDataUV);
			auto serializeSsfbMeshsDataIndices = ssfbBuilder.CreateVector(ssfbMeshsDataIndices);
			auto serializeSsfbFrameData = ssfbBuilder.CreateVector(ssfbFrameData);
			auto serializeSsfbUserData = ssfbBuilder.CreateVector(ssfbUserData);
			auto serializeSsfbLabelData = ssfbBuilder.CreateVector(ssfbLabelData);

			auto item = ss::ssfb::CreateAnimationData(ssfbBuilder, ssfbAnimationDataName,
													  serializeSsfbDefaultData, serializeSsfbFrameData, serializeSsfbUserData,
													  serializeSsfbLabelData, serializeSsfbMeshsDataUV, serializeSsfbMeshsDataIndices,
													  static_cast<int16_t>(decoder.getAnimeStartFrame()),
													  static_cast<int16_t>(decoder.getAnimeEndFrame()),
													  static_cast<int16_t>(decoder.getAnimeTotalFrame()),
													  static_cast<int16_t>(anime->settings.fps),
													  static_cast<int16_t>(label_idx),
													  static_cast<int16_t>(anime->settings.canvasSize.x),
													  static_cast<int16_t>(anime->settings.canvasSize.y),
													  anime->settings.pivot.x, anime->settings.pivot.y);
			ssfbAnimations.push_back(item);
		}

		auto serializeSsfbParts = ssfbBuilder.CreateVector(ssfbParts);
		auto serializeSsfbAnimations = ssfbBuilder.CreateVector(ssfbAnimations);
		auto ssfbAnimePackData = ss::ssfb::CreateAnimePackData(ssfbBuilder, ssfbAnimePackDataName, serializeSsfbParts, serializeSsfbAnimations);
		ssfbAnimePacks.push_back(ssfbAnimePackData);
	}

	//エフェクトデータ
	for (int effectIndex = 0; effectIndex < (int)proj->effectfileList.size(); effectIndex++)
	{
		Lump* effectFile = Lump::set("ss::EffectFile", false, "EffectFile");
		effectfileArray->add(effectFile);

		const SsEffectFile* effectfile = proj->effectfileList[effectIndex];
		effectFile->add(Lump::stringData(effectfile->name, "name"));				//エフェクト名
		auto ssfbEffectFileName = ssfbBuilder.CreateString(effectfile->name);

		const SsEffectModel *effectmodel = &effectfile->effectData;
		effectFile->add(Lump::s16Data(effectmodel->fps, "fps"));					//FPS

		effectFile->add(Lump::s16Data(effectmodel->isLockRandSeed, "isLockRandSeed"));		//乱数を固定するかどうか

		effectFile->add(Lump::s16Data(effectmodel->lockRandSeed, "LockRandSeed"));			//固定する場合の乱数の種

		effectFile->add(Lump::s16Data(effectmodel->layoutScaleX, "layoutScaleX"));			//レイアウトスケールX

		effectFile->add(Lump::s16Data(effectmodel->layoutScaleY, "layoutScaleY"));			//レイアウトスケールY

																			//エフェクトノードの出力
		effectFile->add(Lump::s16Data((int)effectmodel->nodeList.size(), "numNodeList"));	//エフェクトノード数


		Lump* effectNodeArray = Lump::set("ss::EffectNode[]", true, "EffectNodeArray");
		effectFile->add(effectNodeArray);									//ノード配列

		std::vector<flatbuffers::Offset<ss::ssfb::EffectNode>> ssfbEffectNode;
		for (size_t nodeindex = 0; nodeindex < effectmodel->nodeList.size(); nodeindex++)
		{
			//エフェクトノードを追加
			Lump* effectNode = Lump::set("ss::EffectNode", false, "EffectNode");
			effectNodeArray->add(effectNode);

			SsEffectNode *node = effectmodel->nodeList[nodeindex];
			int	arrayIndex = node->arrayIndex;				//通し番号
			int	parentIndex = node->parentIndex;			//親の番号
			SsEffectNodeType::_enum	type = node->type;		//ノードの種類
			//			bool visible = = node->visible;					//エディター用
			SsEffectBehavior behavior = node->behavior;		//動作パラメータ
			SsRenderBlendType::_enum blendType = behavior.BlendType;	//描画方法
			//セル番号
			SsCell*	refCell = behavior.refCell;
			int cellIndex = -1;
			if (refCell)
			{
				cellIndex = (*cellList)[refCell];
			}
			SsString CellName = behavior.CellName;
			SsString CellMapName = behavior.CellMapName;
			//ファイルへ書き出し
			effectNode->add(Lump::s16Data(arrayIndex, "arrayIndex"));		//通し番号
			effectNode->add(Lump::s16Data(parentIndex, "parentIndex"));	//親の番号
			effectNode->add(Lump::s16Data(type, "type"));			//ノードの種類
			effectNode->add(Lump::s16Data(cellIndex, "cellIndex"));		//セルの番号
			effectNode->add(Lump::s16Data(blendType, "blendType"));		//描画方法
			effectNode->add(Lump::s16Data(behavior.plist.size(), "numBehavior"));	//コマンドパラメータ数

			Lump* effectBehaviorArray = Lump::set("ss::ss_u16*[]", true, "effectBehaviorArray");
			effectNode->add(effectBehaviorArray);			//コマンドパラメータ配列

			std::vector<flatbuffers::Offset<void>> ssfbEffectNodeBehavior;
			std::vector<uint8_t> ssfbEffectNodeBehaviorType;
			//コマンドパラメータ
			for (size_t plistindex = 0; plistindex < behavior.plist.size(); plistindex++)
			{
				Lump* effectBehavior = Lump::set("ss::ss_u16[]", true, "effectBehavior");
				effectBehaviorArray->add(effectBehavior);

				SsEffectElementBase *elementbase = behavior.plist[plistindex];
				SsEffectFunctionType::enum_ myType = elementbase->myType;
				effectBehavior->add(Lump::s32Data(myType, "SsEffectFunctionType"));	//コマンドタイプ

				switch (myType)
				{
				case SsEffectFunctionType::Basic:
				{
					//基本情報
					ParticleElementBasic *element = (ParticleElementBasic*)elementbase;

					int			maximumParticle = element->maximumParticle;
					f32VValue	speed = element->speed;
					i32VValue 	lifespan = element->lifespan;
					float		angle = element->angle;
					float		angleVariance = element->angleVariance;
					int			interval = element->interval;
					int			lifetime = element->lifetime;
					int			attimeCreate = element->attimeCreate;
					int			priority = element->priority;
					float speedMinValue = speed.getMinValue();	//初速最小
					float speedMaxValue = speed.getMaxValue();	//初速最大
					int lifespanMinValue = lifespan.getMinValue();	//パーティクル生存時間最小
					int lifespanMaxValue = lifespan.getMaxValue();	//パーティクル生存時間最大

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

					auto item = ss::ssfb::CreateEffectParticleElementBasic(ssfbBuilder, myType, priority,
							maximumParticle, attimeCreate, interval, lifetime,
							speedMinValue, speedMaxValue, lifespanMinValue, lifespanMaxValue, angle, angleVariance);
					ssfbEffectNodeBehavior.push_back(item.Union());
					ssfbEffectNodeBehaviorType.push_back(myType);
					break;
				}
				case SsEffectFunctionType::RndSeedChange:
				{
					//シード上書き
					ParticleElementRndSeedChange *element = (ParticleElementRndSeedChange*)elementbase;
					int		Seed = element->Seed;
					effectBehavior->add(Lump::s32Data(Seed, "Seed"));					//上書きする値

					auto item = ss::ssfb::CreateEffectParticleElementRndSeedChange(ssfbBuilder, Seed);
					ssfbEffectNodeBehavior.push_back(item.Union());
					ssfbEffectNodeBehaviorType.push_back(myType);
					break;
				}
				case SsEffectFunctionType::Delay:
				{
					//発生：タイミング
					ParticleElementDelay *element = (ParticleElementDelay*)elementbase;
					int		DelayTime = element->DelayTime;
					effectBehavior->add(Lump::s32Data(DelayTime, "DelayTime"));				//遅延時間

					auto item = ss::ssfb::CreateEffectParticleElementDelay(ssfbBuilder, DelayTime);
					ssfbEffectNodeBehavior.push_back(item.Union());
					ssfbEffectNodeBehaviorType.push_back(myType);
					break;
				}
				case SsEffectFunctionType::Gravity:
				{
					//重力を加える
					ParticleElementGravity *element = (ParticleElementGravity*)elementbase;
					SsVector2   Gravity = element->Gravity;
					effectBehavior->add(Lump::floatData(Gravity.x, "Gravity_x"));				//X方向の重力
					effectBehavior->add(Lump::floatData(Gravity.y, "Gravity_y"));				//Y方向の重力

					auto item = ss::ssfb::CreateEffectParticleElementGravity(ssfbBuilder, Gravity.x, Gravity.y);
					ssfbEffectNodeBehavior.push_back(item.Union());
					ssfbEffectNodeBehaviorType.push_back(myType);
					break;
				}
				case SsEffectFunctionType::Position:
				{
					//座標：生成時
					ParticleElementPosition *element = (ParticleElementPosition*)elementbase;
					f32VValue   OffsetX = element->OffsetX;
					f32VValue   OffsetY = element->OffsetY;
					effectBehavior->add(Lump::floatData(OffsetX.getMinValue(), "OffsetXMinValue"));				//X座標に加算最小
					effectBehavior->add(Lump::floatData(OffsetX.getMaxValue(), "OffsetXMaxValue"));				//X座標に加算最大
					effectBehavior->add(Lump::floatData(OffsetY.getMinValue(), "OffsetYMinValue"));				//X座標に加算最小
					effectBehavior->add(Lump::floatData(OffsetY.getMaxValue(), "OffsetYMaxValue"));				//X座標に加算最大

					auto item = ss::ssfb::CreateEffectParticleElementPosition(ssfbBuilder,
							OffsetX.getMinValue(), OffsetX.getMaxValue(), OffsetY.getMinValue(), OffsetY.getMaxValue());
					ssfbEffectNodeBehavior.push_back(item.Union());
					ssfbEffectNodeBehaviorType.push_back(myType);
					break;
				}
				case SsEffectFunctionType::Rotation:
				{
					//Z回転を追加
					ParticleElementRotation *element = (ParticleElementRotation*)elementbase;
					f32VValue   Rotation = element->Rotation;
					f32VValue   RotationAdd = element->RotationAdd;
					effectBehavior->add(Lump::floatData(Rotation.getMinValue(), "RotationMinValue"));			//角度初期値最小
					effectBehavior->add(Lump::floatData(Rotation.getMaxValue(), "RotationMaxValue"));			//角度初期値最大
					effectBehavior->add(Lump::floatData(RotationAdd.getMinValue(), "RotationAddMinValue"));			//角度初期加算値最小
					effectBehavior->add(Lump::floatData(RotationAdd.getMaxValue(), "RotationAddMaxValue"));			//角度初期加算値最大

					auto item = ss::ssfb::CreateEffectParticleElementRotation(ssfbBuilder,
							Rotation.getMinValue(), Rotation.getMaxValue(), RotationAdd.getMinValue(), RotationAdd.getMaxValue());
					ssfbEffectNodeBehavior.push_back(item.Union());
					ssfbEffectNodeBehaviorType.push_back(myType);
					break;
				}
				case SsEffectFunctionType::TransRotation:
				{
					//Z回転速度変更
					ParticleElementRotationTrans *element = (ParticleElementRotationTrans*)elementbase;
					float   RotationFactor = element->RotationFactor;
					float	EndLifeTimePer = element->EndLifeTimePer;
					effectBehavior->add(Lump::floatData(RotationFactor, "RotationFactor"));					//角度目標加算値
					effectBehavior->add(Lump::floatData(EndLifeTimePer, "EndLifeTimePer"));					//到達時間

					auto item = ss::ssfb::CreateEffectParticleElementRotationTrans(ssfbBuilder, RotationFactor, EndLifeTimePer);
					ssfbEffectNodeBehavior.push_back(item.Union());
					ssfbEffectNodeBehaviorType.push_back(myType);
					break;
				}
				case SsEffectFunctionType::TransSpeed:
				{
					//速度：変化
					ParticleElementTransSpeed *element = (ParticleElementTransSpeed*)elementbase;
					f32VValue	Speed = element->Speed;
					effectBehavior->add(Lump::floatData(Speed.getMinValue(), "SpeedMinValue"));				//速度目標値最小
					effectBehavior->add(Lump::floatData(Speed.getMaxValue(), "SpeedMaxValue"));				//速度目標値最大

					auto item = ss::ssfb::CreateEffectParticleElementTransSpeed(ssfbBuilder, Speed.getMinValue(), Speed.getMaxValue());
					ssfbEffectNodeBehavior.push_back(item.Union());
					ssfbEffectNodeBehaviorType.push_back(myType);
					break;
				}
				case SsEffectFunctionType::TangentialAcceleration:
				{
					//接線加速度
					ParticleElementTangentialAcceleration *element = (ParticleElementTangentialAcceleration*)elementbase;
					f32VValue	Acceleration = element->Acceleration;

					effectBehavior->add(Lump::floatData(Acceleration.getMinValue(), "AccelerationMinValue"));		//設定加速度最小
					effectBehavior->add(Lump::floatData(Acceleration.getMaxValue(), "AccelerationMaxValue"));		//設定加速度最大

					auto item = ss::ssfb::CreateEffectParticleElementTangentialAcceleration(ssfbBuilder, Acceleration.getMinValue(), Acceleration.getMaxValue());
					ssfbEffectNodeBehavior.push_back(item.Union());
					ssfbEffectNodeBehaviorType.push_back(myType);
					break;
				}
				case SsEffectFunctionType::InitColor:
				{
					//カラーRGBA：生成時
					ParticleElementInitColor *element = (ParticleElementInitColor*)elementbase;
					SsU8cVValue Color = element->Color;
					effectBehavior->add(Lump::s32Data(Color.getMinValue().toARGB(), "ColorMinValue"));		//設定カラー最小
					effectBehavior->add(Lump::s32Data(Color.getMaxValue().toARGB(), "ColorMaxValue"));		//設定カラー最大

					auto item = ss::ssfb::CreateEffectParticleElementInitColor(ssfbBuilder, Color.getMinValue().toARGB(), Color.getMaxValue().toARGB());
					ssfbEffectNodeBehavior.push_back(item.Union());
					ssfbEffectNodeBehaviorType.push_back(myType);
					break;
				}
				case SsEffectFunctionType::TransColor:
				{
					//カラーRGB：変化
					ParticleElementTransColor *element = (ParticleElementTransColor*)elementbase;
					SsU8cVValue Color = element->Color;
					effectBehavior->add(Lump::s32Data(Color.getMinValue().toARGB(), "ColorMinValue"));		//設定カラー最小
					effectBehavior->add(Lump::s32Data(Color.getMaxValue().toARGB(), "ColorMaxValue"));		//設定カラー最大

					auto item = ss::ssfb::CreateEffectParticleElementTransColor(ssfbBuilder, Color.getMinValue().toARGB(), Color.getMaxValue().toARGB());
					ssfbEffectNodeBehavior.push_back(item.Union());
					ssfbEffectNodeBehaviorType.push_back(myType);
					break;
				}
				case SsEffectFunctionType::AlphaFade:
				{
					//フェード
					ParticleElementAlphaFade *element = (ParticleElementAlphaFade*)elementbase;
					f32VValue  disprange = element->disprange; // mnagaku 頭小文字
					effectBehavior->add(Lump::floatData(disprange.getMinValue(), "disprangeMinValue"));			//表示区間開始
					effectBehavior->add(Lump::floatData(disprange.getMaxValue(), "disprangeMaxValue"));			//表示区間終了

					auto item = ss::ssfb::CreateEffectParticleElementAlphaFade(ssfbBuilder, disprange.getMinValue(), disprange.getMaxValue());
					ssfbEffectNodeBehavior.push_back(item.Union());
					ssfbEffectNodeBehaviorType.push_back(myType);
					break;
				}
				case SsEffectFunctionType::Size:
				{
					//スケール：生成時
					ParticleElementSize *element = (ParticleElementSize*)elementbase;
					f32VValue SizeX = element->SizeX;
					f32VValue SizeY = element->SizeY;
					f32VValue ScaleFactor = element->ScaleFactor;
					effectBehavior->add(Lump::floatData(SizeX.getMinValue(), "SizeXMinValue"));				//幅倍率最小
					effectBehavior->add(Lump::floatData(SizeX.getMaxValue(), "SizeXMaxValue"));				//幅倍率最大
					effectBehavior->add(Lump::floatData(SizeY.getMinValue(), "SizeYMinValue"));				//高さ倍率最小
					effectBehavior->add(Lump::floatData(SizeY.getMaxValue(), "SizeYMaxValue"));				//高さ倍率最大
					effectBehavior->add(Lump::floatData(ScaleFactor.getMinValue(), "ScaleFactorMinValue"));			//倍率最小
					effectBehavior->add(Lump::floatData(ScaleFactor.getMaxValue(), "ScaleFactorMaxValue"));			//倍率最大

					auto item = ss::ssfb::CreateEffectParticleElementSize(ssfbBuilder,
							SizeX.getMinValue(), SizeX.getMaxValue(), SizeY.getMinValue(), SizeY.getMaxValue(),
							ScaleFactor.getMinValue(), ScaleFactor.getMaxValue());
					ssfbEffectNodeBehavior.push_back(item.Union());
					ssfbEffectNodeBehaviorType.push_back(myType);
					break;
				}
				case SsEffectFunctionType::TransSize:
				{
					//スケール：変化
					ParticleElementTransSize *element = (ParticleElementTransSize*)elementbase;
					f32VValue SizeX = element->SizeX;
					f32VValue SizeY = element->SizeY;
					f32VValue ScaleFactor = element->ScaleFactor;
					effectBehavior->add(Lump::floatData(SizeX.getMinValue(), "SizeXMinValue"));				//幅倍率最小
					effectBehavior->add(Lump::floatData(SizeX.getMaxValue(), "SizeXMaxValue"));				//幅倍率最大
					effectBehavior->add(Lump::floatData(SizeY.getMinValue(), "SizeYMinValue"));				//高さ倍率最小
					effectBehavior->add(Lump::floatData(SizeY.getMaxValue(), "SizeYMaxValue"));				//高さ倍率最大
					effectBehavior->add(Lump::floatData(ScaleFactor.getMinValue(), "ScaleFactorMinValue"));			//倍率最小
					effectBehavior->add(Lump::floatData(ScaleFactor.getMaxValue(), "ScaleFactorMaxValue"));			//倍率最大

					auto item = ss::ssfb::CreateEffectParticleElementTransSize(ssfbBuilder, SizeX.getMinValue(), SizeX.getMaxValue(),
							SizeY.getMinValue(), SizeY.getMaxValue(),
							ScaleFactor.getMinValue(), ScaleFactor.getMaxValue());
					ssfbEffectNodeBehavior.push_back(item.Union());
					ssfbEffectNodeBehaviorType.push_back(myType);
					break;
				}
				case SsEffectFunctionType::PointGravity:
				{
					//重力点の追加
					ParticlePointGravity *element = (ParticlePointGravity*)elementbase;
					SsVector2   Position = element->Position;
					float		Power = element->Power;
					effectBehavior->add(Lump::floatData(Position.x, "Position_x"));						//重力点X
					effectBehavior->add(Lump::floatData(Position.y, "Position_y"));						//重力点Y
					effectBehavior->add(Lump::floatData(Power, "Power"));							//パワー

					auto item = ss::ssfb::CreateEffectParticlePointGravity(ssfbBuilder, Position.x, Position.y, Power);
					ssfbEffectNodeBehavior.push_back(item.Union());
					ssfbEffectNodeBehaviorType.push_back(myType);
					break;
				}
				case SsEffectFunctionType::TurnToDirectionEnabled:
				{
					//進行方向に向ける
					ParticleTurnToDirectionEnabled *element = (ParticleTurnToDirectionEnabled*)elementbase;
					//コマンドがあれば有効
					effectBehavior->add(Lump::floatData(element->Rotation, "Rotation"));				//方向オフセット

					auto item = ss::ssfb::CreateEffectParticleTurnToDirectionEnabled(ssfbBuilder, element->Rotation);
					ssfbEffectNodeBehavior.push_back(item.Union());
					ssfbEffectNodeBehaviorType.push_back(myType);
					break;
				}
				case SsEffectFunctionType::InfiniteEmitEnabled:
				{
					//無限にする
					ParticleInfiniteEmitEnabled *element = (ParticleInfiniteEmitEnabled*)elementbase;
					//コマンドがあれば有効
					effectBehavior->add(Lump::s32Data(1, "flag"));									//ダミーデータ

					auto item = ss::ssfb::CreateEffectParticleInfiniteEmitEnabled(ssfbBuilder, 1.0);
					ssfbEffectNodeBehavior.push_back(item.Union());
					ssfbEffectNodeBehaviorType.push_back(myType);
					break;
				}
				case SsEffectFunctionType::Base:
				default:
					//未使用のコマンドが含まれている
					std::cerr << "警告：未使用のエフェクトコマンドが含まれています。 \n";
					break;
				}
			}

			auto serializeSsfbEffectNodeBehaviorType = ssfbBuilder.CreateVector(ssfbEffectNodeBehaviorType);
			auto serializeSsfbEffectNodeBehavior = ssfbBuilder.CreateVector(ssfbEffectNodeBehavior);
			auto ssfbEffectNodeItem = ss::ssfb::CreateEffectNode(ssfbBuilder,
									   static_cast<int16_t>(arrayIndex),
									   static_cast<int16_t>(parentIndex),
									   type,
									   static_cast<int16_t>(cellIndex),
									   blendType,
									   static_cast<int16_t>(ssfbEffectNodeBehavior.size()),
									   serializeSsfbEffectNodeBehaviorType,
									   serializeSsfbEffectNodeBehavior);
			ssfbEffectNode.push_back(ssfbEffectNodeItem);
		}

		auto serializeSsfbEffectNode = ssfbBuilder.CreateVector(ssfbEffectNode);
		auto ssfbEffectFile = ss::ssfb::CreateEffectFile(ssfbBuilder, ssfbEffectFileName,
				effectmodel->fps, effectmodel->isLockRandSeed, effectmodel->lockRandSeed,
				effectmodel->layoutScaleX, effectmodel->layoutScaleY,
				ssfbEffectNode.size(),serializeSsfbEffectNode);
		ssfbEffectFileList.push_back(ssfbEffectFile);
	}

	auto serializeSsfbCells = ssfbBuilder.CreateVector(ssfbCells);
	auto serializeSsfbAnimePackData = ssfbBuilder.CreateVector(ssfbAnimePacks);
	auto serializeSsfbEffectFileList = ssfbBuilder.CreateVector(ssfbEffectFileList);
	ssfbProjectData = ss::ssfb::CreateProjectData(ssfbBuilder, DATA_ID, CURRENT_DATA_VERSION, 0,
			ssfbImageBaseDir,
			serializeSsfbCells,
			serializeSsfbAnimePackData,
			serializeSsfbEffectFileList,
			static_cast<int16_t>(ssfbCells.size()),
			static_cast<int16_t>(ssfbAnimePacks.size()),
			static_cast<int16_t>(ssfbEffectFileList.size()));
	ssfbBuilder.Finish(ssfbProjectData);

	std::cerr << "convert end" << "\n";

	return topLump;
}



void convertProject(const std::string& outPath, LumpExporter::StringEncoding encoding, const std::string& sspjPath,
	const std::string& imageBaseDir, const std::string& creatorComment, const int outputFormat)
{
	SSTextureFactory texFactory(new SSTextureBMP());
	std::cerr << sspjPath << "\n";
	SsProject* proj = ssloader_sspj::Load(sspjPath);
	Lump* lump;
	try
	{
		if (proj)
		{
			lump = parseParts(proj, imageBaseDir);
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

		if (outputFormat == OUTPUT_FORMAT_FLAG_JSON)
		{
			out.open((outPath + ".json").c_str(), std::ios_base::out);
			LumpExporter::saveJson(out, encoding, lump, creatorComment);
		}
		else if (outputFormat == OUTPUT_FORMAT_FLAG_CSOURCE)
		{
			out.open((outPath + ".c").c_str(), std::ios_base::out);
			LumpExporter::saveCSource(out, encoding, lump, "topLabel", creatorComment);
		}
		else if (outputFormat == OUTPUT_FORMAT_FLAG_SSFB)
		{
			flatbuffers::SaveFile(std::string(outPath + ".orig.ssfb").c_str(),
								  reinterpret_cast<const char *>(ssfbBuilder.GetBufferPointer()),
								  ssfbBuilder.GetSize(),
								  true);

			out.open((outPath + ".ssfb").c_str(), std::ios_base::binary | std::ios_base::out);
            LumpExporter::saveSsfb(out, encoding, lump, creatorComment, s_frameIndexVec);
		}
		else
		{
			out.open(outPath.c_str(), std::ios_base::binary | std::ios_base::out);
			LumpExporter::saveBinary(out, encoding, lump, creatorComment);
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
"  -f      set output format.\n"
"\n";

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

	Options()
	: isHelp(false)
	, isVerbose(false)
	, encoding(LumpExporter::UTF8)
	{}
};




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
	}
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
			inList.push_back(name);
		}
	}

	options.inList = inList;
	// success
	return true;
}






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
			std::vector<std::string> fileList = FileUtil::findPath(str);
			if (!fileList.empty())
			{
				std::copy(fileList.begin(), fileList.end(), std::back_inserter(sources));
			}
			else
			{
				std::cerr << "Cannot find input file: " << str << std::endl;
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
		std::string outPath = FileUtil::replaceExtension(sspjPath, ".sspj", ".ssbp");
		
		if ( options.outputDir != "" ) 
		{
			//パスが指定されている場合
			int st = 0;
#ifdef _WIN32
			st = outPath.find_last_of("\\");
#else
            st = outPath.find_last_of("/");
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
			outPath = options.outputDir + ssbpname;
		}

		if (options.isVerbose)
		{
			std::cout << "Convert: " << sspjPath << " -> " << outPath << std::endl;
		}
		
		convertProject(outPath, encoding, sspjPath, options.imageBaseDir, creatorComment, options.outputFormat);
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


