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

#ifndef _WIN32
#include <sys/stat.h>
#endif

#include "Lump.h"
#include "LumpExporter.h"
#include "FileUtil.h"
#include "SsPlayerConverter.h"

#include "picojson.h"


static const int DATA_VERSION_1			= 1;
static const int DATA_VERSION_2         = 2;
static const int DATA_VERSION_3         = 3;
static const int DATA_VERSION_4			= 4;
static const int DATA_VERSION_5			= 5;
static const int DATA_VERSION_6			= 6;
static const int DATA_VERSION_7			= 7;

static const int DATA_ID				= 0x42505353;
static const int CURRENT_DATA_VERSION	= DATA_VERSION_7;


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
	OUTPUT_FORMAT_FLAG_CSOURCE	= 1 << 2
};

bool convert_error_exit = false;	//データにエラーがありコンバートを中止した



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

static picojson::object ssjson;

static Lump* parseParts(SsProject* proj, const std::string& imageBaseDir)
{
//	static SsPartStateLess _ssPartStateLess;
	std::cerr << SPRITESTUDIOSDK_VERSION << "\n";	//バージョン表記は ssloader.h　にあります。
	std::cerr << "Ss6Converter ssbpFormatVersion=" << CURRENT_DATA_VERSION << "\n";
	std::cerr << "convert start!" << "\n";

	CellList* cellList = makeCellList(proj);

	Lump* topLump = Lump::set("ss::ProjectData", true);
	ssjson.clear();

	if (checkFileVersion(proj->version, SPRITESTUDIO6_SSPJVERSION) == false)
	{
		std::cerr << "エラー：SpriteStudio Ver.5のプロジェクトは使用できません。\n";
		std::cerr << "SpriteStudio Ver.6で保存する必要があります。\n";
		convert_error_exit = true;	//エラーが発生コンバート失敗
	}


	topLump->add(Lump::s32Data(DATA_ID));
	ssjson.insert(std::make_pair("dataId", picojson::value((double)DATA_ID)));
	
	topLump->add(Lump::s32Data(CURRENT_DATA_VERSION));
	ssjson.insert(std::make_pair("version", picojson::value((double)CURRENT_DATA_VERSION)));
	
	//4互換設定の出力
	topLump->add(Lump::s32Data(0));
	ssjson.insert(std::make_pair("flags", picojson::value(0.0)));
	
	if (imageBaseDir.length() > 0)
	{
		topLump->add(Lump::stringData(imageBaseDir));
		ssjson.insert(std::make_pair("imageBaseDir", picojson::value(imageBaseDir)));
	}
	else
	{
		topLump->add(Lump::s32Data(0));
		ssjson.insert(std::make_pair("imageBaseDir", picojson::value()));
	}

	Lump* cellsData = Lump::set("ss::Cell[]", true);
	topLump->add(cellsData);
	Lump* packDataArray = Lump::set("ss::AnimePackData[]", true);
	topLump->add(packDataArray);
	Lump* effectfileArray = Lump::set("ss::EffectFile[]", true);
	topLump->add(effectfileArray);

	topLump->add(Lump::s16Data((int)cellList->size()));
	ssjson.insert(std::make_pair("numCells", picojson::value((double)cellList->size())));
	
	topLump->add(Lump::s16Data((int)proj->animeList.size()));
	ssjson.insert(std::make_pair("numAnimePacks", picojson::value((double)proj->animeList.size())));
	
	topLump->add(Lump::s16Data((int)proj->effectfileList.size()));
	ssjson.insert(std::make_pair("numEffectFileList", picojson::value((double)proj->effectfileList.size())));
	
	//セルマップ警告
	if (proj->cellmapList.size() == 0)
	{
		std::cerr << "警告：セルマップが存在しない" << "\n";
		convert_error_exit = true;	//エラーが発生コンバート失敗
	}
	// セルの情報
	picojson::array ssjson_cell;
	for (size_t mapIndex = 0; mapIndex < proj->cellmapList.size(); mapIndex++)
	{
		const SsCellMap* cellMap = proj->cellmapList[mapIndex];
		picojson::object cm;
		
		Lump* cellMapData = Lump::set("ss::CellMap", true);
		cellMapData->add(Lump::stringData(cellMap->name));
		cm.insert(std::make_pair("name", picojson::value(cellMap->name)));
		cellMapData->add(Lump::stringData(cellMap->imagePath));
		cm.insert(std::make_pair("imagePath", picojson::value(cellMap->imagePath)));
		
		cellMapData->add(Lump::s16Data((int)mapIndex));
		cm.insert(std::make_pair("index", picojson::value((double)(int)mapIndex)));
		if (cellMap->overrideTexSettings == true )							///< テクスチャ設定をプロジェクトの設定ではなく下記設定を使う
		{
			//個別の設定を使う
			cellMapData->add(Lump::s16Data(cellMap->wrapMode));				///< テクスチャのラップモード
			cm.insert(std::make_pair("wrapmode", picojson::value((double)cellMap->wrapMode)));
			cellMapData->add(Lump::s16Data(cellMap->filterMode));			///< テクスチャのフィルタモード
			cm.insert(std::make_pair("filtermode", picojson::value((double)cellMap->filterMode)));
		}
		else
		{
			//プロジェクトの設定を使う
			cellMapData->add(Lump::s16Data(proj->settings.wrapMode));		///< テクスチャのラップモード
			cm.insert(std::make_pair("wrapmode", picojson::value((double)proj->settings.wrapMode)));
			cellMapData->add(Lump::s16Data(proj->settings.filterMode));		///< テクスチャのフィルタモード
			cm.insert(std::make_pair("filtermode", picojson::value((double)proj->settings.filterMode)));
		}
		cellMapData->add(Lump::s16Data(0));	// reserved


		//全角チェック
		if ( isZenkaku( &cellMap->name ) == true )
		{
			std::cerr << "エラー：セルマップに全角が使用されている: " << cellMap->name << "\n";
			convert_error_exit = true;	//エラーが発生コンバート失敗
		}

		for (size_t cellIndex = 0; cellIndex < cellMap->cells.size(); cellIndex++)
		{
			const SsCell* cell = cellMap->cells[cellIndex];
			picojson::object c;

			Lump* cellData = Lump::set("ss::Cell");
			cellsData->add(cellData);
			
			cellData->add(Lump::stringData(cell->name));
			c.insert(std::make_pair("name", picojson::value(cell->name)));
			cellData->add(cellMapData);
			c.insert(std::make_pair("cellMap", picojson::value(cm)));
			cellData->add(Lump::s16Data((int)cellIndex));
			c.insert(std::make_pair("indexInCellMap", picojson::value((double)(int)cellIndex)));
			cellData->add(Lump::s16Data((int)cell->pos.x));
			c.insert(std::make_pair("x", picojson::value((double)(int)cell->pos.x)));
			cellData->add(Lump::s16Data((int)cell->pos.y));
			c.insert(std::make_pair("y", picojson::value((double)(int)cell->pos.y)));
			cellData->add(Lump::s16Data((int)cell->size.x));
			c.insert(std::make_pair("width", picojson::value((double)(int)cell->size.x)));
			cellData->add(Lump::s16Data((int)cell->size.y));
			c.insert(std::make_pair("height", picojson::value((double)(int)cell->size.y)));
			cellData->add(Lump::s16Data(0));	// reserved
			cellData->add(Lump::floatData(cell->pivot.x));
			c.insert(std::make_pair("pivot_x", picojson::value(cell->pivot.x)));
			cellData->add(Lump::floatData(-cell->pivot.y));
			c.insert(std::make_pair("pivot_y", picojson::value(cell->pivot.y)));
			float u1 = cell->pos.x / cellMap->pixelSize.x;
			float v1 = cell->pos.y / cellMap->pixelSize.y;
			float u2 = ( cell->pos.x + cell->size.x ) / cellMap->pixelSize.x;
			float v2 = ( cell->pos.y + cell->size.y ) / cellMap->pixelSize.y;
			cellData->add(Lump::floatData(u1));			//テクスチャのサイズを出力
			c.insert(std::make_pair("u1", picojson::value(u1)));
			cellData->add(Lump::floatData(v1));
			c.insert(std::make_pair("v1", picojson::value(v1)));
			cellData->add(Lump::floatData(u2));			//テクスチャのサイズを出力
			c.insert(std::make_pair("u2", picojson::value(u2)));
			cellData->add(Lump::floatData(v2));
			c.insert(std::make_pair("v2", picojson::value(v2)));

			ssjson_cell.push_back(picojson::value(c));

			//全角チェック
			if (isZenkaku(&cell->name) == true)
			{
				std::cerr << "エラー：セルに全角が使用されている: " << cell->name << "\n";
				convert_error_exit = true;	//エラーが発生コンバート失敗
			}
		}
	}
	ssjson.insert(std::make_pair("cells", picojson::value(ssjson_cell)));
	

	//アニメーション警告
	if (proj->animeList.size() == 0)
	{
		std::cerr << "警告：アニメーションが存在しない" << "\n";
		convert_error_exit = true;	//エラーが発生コンバート失敗
	}
	picojson::array ssjson_anime;
	// パーツ、アニメ情報
	for (int packIndex = 0; packIndex < (int)proj->animeList.size(); packIndex++)
	{
		const SsAnimePack* animePack = proj->animeList[packIndex];
		const SsModel& model = animePack->Model;
		
		// AnimePackData
		Lump* animePackData = Lump::set("ss::AnimePackData");
		packDataArray->add(animePackData);
		picojson::object a;
		
		Lump* partDataArray = Lump::set("ss::PartData[]", true);
		Lump* animeDataArray = Lump::set("ss::AnimationData[]", true);

		animePackData->add(Lump::stringData(animePack->name));
		a.insert(std::make_pair("name", picojson::value(animePack->name)));
		
		//全角チェック
		if ( isZenkaku( &animePack->name ) == true )
		{
			std::cerr << "エラー：ファイル名に全角が使用されている: " << animePack->name << "\n";
			convert_error_exit = true;	//エラーが発生コンバート失敗
		}
		animePackData->add(partDataArray);
		animePackData->add(animeDataArray);
		animePackData->add(Lump::s16Data((int)model.partList.size()));
		a.insert(std::make_pair("numParts", picojson::value((double)model.partList.size())));
		animePackData->add(Lump::s16Data((int)animePack->animeList.size()));
		a.insert(std::make_pair("numAnimations", picojson::value((double)animePack->animeList.size())));

		picojson::array ps;
		// パーツ情報（モデル）の出力
		for (int partIndex = 0; partIndex < (int)model.partList.size(); partIndex++)
		{
			const SsPart* part = model.partList[partIndex];

			// PartData
			Lump* partData = Lump::set("ss::PartData");
			partDataArray->add(partData);
			picojson::object p;
			
			partData->add(Lump::stringData(part->name));
			p.insert(std::make_pair("name", picojson::value(part->name)));
			
			//全角チェック
			if ( isZenkaku( &part->name ) == true )
			{
				std::cerr << "エラー：パーツ名に全角が使用されている: " << part->name << "\n";
				convert_error_exit = true;	//エラーが発生コンバート失敗
			}
			partData->add(Lump::s16Data(part->arrayIndex));
			p.insert(std::make_pair("index", picojson::value((double)part->arrayIndex)));
			partData->add(Lump::s16Data(part->parentIndex));
			p.insert(std::make_pair("parentIndex", picojson::value((double)part->parentIndex)));
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
				partData->add(Lump::s16Data(part->type));
				p.insert(std::make_pair("type", picojson::value((double)part->type)));
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
						partData->add(Lump::s16Data(SsPartType::null));
						p.insert(std::make_pair("type", picojson::value()));
						std::cerr << "ワーニング：参照のないインスタンスパーツがある: " << animePack->name << ".ssae " << part->name << "\n";
					}
					else
					{
						partData->add(Lump::s16Data(part->type));
						p.insert(std::make_pair("type", picojson::value((double)part->type)));
					}
				}
				break;
			case SsPartType::effect:		// 5.5エフェクトパーツ
				//参照エフェクト名が空の場合はNULLパーツになる。
				if (part->refEffectName == "")
				{
					partData->add(Lump::s16Data(SsPartType::null));
					p.insert(std::make_pair("type", picojson::value()));
					//未実装　ワーニングを表示しNULLパーツにする
					std::cerr << "ワーニング：参照のないエフェクトパーツがある: " << animePack->name << ".ssae " << part->name << "\n";
				}
				else
				{
					partData->add(Lump::s16Data(part->type));
					p.insert(std::make_pair("type", picojson::value((double)part->type)));
				}
				break;
			default:
				//未対応パーツ　ワーニングを表示しNULLパーツにする
				std::cerr << "ワーニング：未対応のパーツ種別が使われている: " << animePack->name << ".ssae " << part->name << "\n";
				partData->add(Lump::s16Data(SsPartType::null));
				p.insert(std::make_pair("type", picojson::value()));
				break;
			}
			partData->add(Lump::s16Data(part->boundsType));
			p.insert(std::make_pair("boundsType", picojson::value((double)part->boundsType)));
			partData->add(Lump::s16Data(part->alphaBlendType));
			p.insert(std::make_pair("alphaBlendType", picojson::value((double)part->alphaBlendType)));
			partData->add(Lump::s16Data(0));	// reserved
												//インスタンスアニメ名
			if ( part->refAnime == "" )
			{
				const SsString str = "";
//				partData->add(Lump::s16Data((int)str.length()));				//文字列のサイズ
				partData->add(Lump::stringData(str));							//文字列
				p.insert(std::make_pair("refname", picojson::value(str)));
			}
			else
			{
				const SsString str = part->refAnimePack + "/" + part->refAnime;
//				partData->add(Lump::s16Data((int)str.length()));				//文字列のサイズ
				partData->add(Lump::stringData(str));							//文字列
				p.insert(std::make_pair("refname", picojson::value(str)));
			}
			//エフェクト名
			if (part->refEffectName == "")
			{
				const SsString str = "";
				partData->add(Lump::stringData(str));							//文字列
				p.insert(std::make_pair("effectfilename", picojson::value(str)));
			}
			else
			{
				const SsString str = part->refEffectName;
				partData->add(Lump::stringData(str));							//文字列
				p.insert(std::make_pair("effectfilename", picojson::value(str)));
			}
			//カラーラベル
			const SsString str = part->colorLabel;
			partData->add(Lump::stringData(str));								//文字列
			p.insert(std::make_pair("colorLabel", picojson::value(str)));

			//マスク対象
			partData->add(Lump::s16Data(part->maskInfluence));
			p.insert(std::make_pair("colorLabel", picojson::value((double)part->maskInfluence)));

			ps.push_back(picojson::value(p));
		}
		a.insert(std::make_pair("parts", picojson::value(ps)));
		
		// アニメ情報の出力
		SsCellMapList* cellMapList = new SsCellMapList();	// SsAnimeDecoderのデストラクタで破棄される
		SsAnimeDecoder decoder;
//		const SsKeyframe* key;

		picojson::array as;
		for (int animeIndex = 0; animeIndex < (int)animePack->animeList.size(); animeIndex++)
		{
			SsAnimePack* animePack = proj->getAnimePackList()[packIndex];
			SsModel* model = &animePack->Model;
			SsAnimation* anime = animePack->animeList[animeIndex];
			
			cellMapList->set(proj, animePack);
			decoder.setAnimation(model, anime, cellMapList, proj);
			std::list<SsPartState*>& partList = decoder.getPartSortList();
			
			// AnimationData
			Lump* animeData = Lump::set("ss::AnimationData");
			animeDataArray->add(animeData);
			picojson::object a2;

			// パーツごとのアニメーションパラメータ初期値
			// ※とりあえず先頭フレームの値を初期値にしているが、一番使われている値を初期値にすべきかも
			size_t numParts = model->partList.size();
			std::vector<PartInitialData> initialDataList;
			
			decoder.setPlayFrame(0);
			decoder.update();

			Lump* initialDataArray = Lump::set("ss::AnimationInitialData[]", true);
			int sortedOrder = 0;
			picojson::array is;
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
				
				Lump* initialData = Lump::set("ss::AnimationInitialData");
				initialDataArray->add(initialData);

				picojson::object i;
				initialData->add(Lump::s16Data(init.index));
				i.insert(std::make_pair("index", picojson::value((double)init.index)));
				initialData->add(Lump::s16Data(0)); //ダミーデータ
				initialData->add(Lump::s32Data(init.lowflag));
				i.insert(std::make_pair("lowflag", picojson::value((double)init.lowflag)));
				initialData->add(Lump::s32Data(init.highflag));
				i.insert(std::make_pair("highflag", picojson::value((double)init.highflag)));
				initialData->add(Lump::s16Data(init.priority));
				i.insert(std::make_pair("priority", picojson::value((double)init.priority)));
				initialData->add(Lump::s16Data(init.cellIndex));
				i.insert(std::make_pair("cellIndex", picojson::value((double)init.cellIndex)));
				initialData->add(Lump::s16Data(init.opacity));
				i.insert(std::make_pair("opacity", picojson::value((double)init.opacity)));
				initialData->add(Lump::s16Data(init.localopacity));
				i.insert(std::make_pair("localopacity", picojson::value((double)init.localopacity)));
				initialData->add(Lump::s16Data(init.masklimen));
				i.insert(std::make_pair("masklimen", picojson::value((double)init.masklimen)));
				initialData->add(Lump::s16Data(0)); //ダミーデータ
				initialData->add(Lump::floatData(init.posX));
				i.insert(std::make_pair("positionX", picojson::value(init.posX)));
				initialData->add(Lump::floatData(init.posY));
				i.insert(std::make_pair("positionY", picojson::value(init.posY)));
				initialData->add(Lump::floatData(init.posZ));
				i.insert(std::make_pair("positionZ", picojson::value(init.posZ)));
				initialData->add(Lump::floatData(init.pivotX));
				i.insert(std::make_pair("pivotX", picojson::value(init.pivotX)));
				initialData->add(Lump::floatData(init.pivotY));
				i.insert(std::make_pair("pivotY", picojson::value(init.pivotY)));
				initialData->add(Lump::floatData(init.rotationX));
				i.insert(std::make_pair("rotationX", picojson::value(init.rotationX)));
				initialData->add(Lump::floatData(init.rotationY));
				i.insert(std::make_pair("rotationY", picojson::value(init.rotationY)));
				initialData->add(Lump::floatData(init.rotationZ));
				i.insert(std::make_pair("rotationZ", picojson::value(init.rotationZ)));
				initialData->add(Lump::floatData(init.scaleX));
				i.insert(std::make_pair("scaleX", picojson::value(init.scaleX)));
				initialData->add(Lump::floatData(init.scaleY));
				i.insert(std::make_pair("scaleY", picojson::value(init.scaleY)));
				initialData->add(Lump::floatData(init.localscaleX));
				i.insert(std::make_pair("localscaleX", picojson::value(init.localscaleX)));
				initialData->add(Lump::floatData(init.localscaleY));
				i.insert(std::make_pair("localscaleY", picojson::value(init.localscaleY)));
				initialData->add(Lump::floatData(init.size_X));
				i.insert(std::make_pair("size_X", picojson::value(init.size_X)));
				initialData->add(Lump::floatData(init.size_Y));
				i.insert(std::make_pair("size_Y", picojson::value(init.size_Y)));
				initialData->add(Lump::floatData(init.uv_move_X));
				i.insert(std::make_pair("uv_move_X", picojson::value(init.uv_move_X)));
				initialData->add(Lump::floatData(init.uv_move_Y));
				i.insert(std::make_pair("uv_move_Y", picojson::value(init.uv_move_Y)));
				initialData->add(Lump::floatData(init.uv_rotation));
				i.insert(std::make_pair("uv_rotation", picojson::value(init.uv_rotation)));
				initialData->add(Lump::floatData(init.uv_scale_X));
				i.insert(std::make_pair("uv_scale_X", picojson::value(init.uv_scale_X)));
				initialData->add(Lump::floatData(init.uv_scale_Y));
				i.insert(std::make_pair("uv_scale_Y", picojson::value(init.uv_scale_Y)));
				initialData->add(Lump::floatData(init.boundingRadius));
				i.insert(std::make_pair("boundingRadius", picojson::value(init.boundingRadius)));
				//インスタンス関連
				initialData->add(Lump::s32Data(init.instanceValue_curKeyframe));
				i.insert(std::make_pair("instanceValue_curKeyframe", picojson::value((double)init.instanceValue_curKeyframe)));
				initialData->add(Lump::s32Data(init.instanceValue_startFrame));
				i.insert(std::make_pair("instanceValue_startFrame", picojson::value((double)init.instanceValue_startFrame)));
				initialData->add(Lump::s32Data(init.instanceValue_endFrame));
				i.insert(std::make_pair("instanceValue_endFrame", picojson::value((double)init.instanceValue_endFrame)));
				initialData->add(Lump::s32Data(init.instanceValue_loopNum));
				i.insert(std::make_pair("instanceValue_loopNum", picojson::value((double)init.instanceValue_loopNum)));
				initialData->add(Lump::floatData(init.instanceValue_speed));
				i.insert(std::make_pair("instanceValue_speed", picojson::value(init.instanceValue_speed)));
				initialData->add(Lump::s32Data(init.instanceValue_loopflag));
				i.insert(std::make_pair("instanceValue_loopflag", picojson::value((double)init.instanceValue_loopflag)));
				//エフェクト関連
				initialData->add(Lump::s32Data(init.effectValue_curKeyframe));
				i.insert(std::make_pair("effectValue_curKeyframe", picojson::value((double)init.effectValue_curKeyframe)));
				initialData->add(Lump::s32Data(init.effectValue_startTime));
				i.insert(std::make_pair("effectValue_startTime", picojson::value((double)init.effectValue_startTime)));
				initialData->add(Lump::floatData(init.effectValue_speed));
				i.insert(std::make_pair("effectValue_speed", picojson::value(init.effectValue_speed)));
				initialData->add(Lump::s32Data(init.effectValue_loopflag));
				i.insert(std::make_pair("effectValue_loopflag", picojson::value((double)init.effectValue_loopflag)));
				
				is.push_back(picojson::value(i));
			}
			a2.insert(std::make_pair("defaultData", picojson::value(is)));
			
			Lump* meshsDataUV = Lump::set("ss::ss_u16*[]", true);
			{
				decoder.setPlayFrame(0);
				decoder.update();

				picojson::array ms;
				foreach(std::vector<SsPartAndAnime>, decoder.getPartAnime(), it)
				{
					SsPart* part = it->first;
					const SsPartState* state = findState(partList, part->arrayIndex);

					//サイズ分のUV出力
					Lump* meshData = Lump::set("ss::ss_u16*[]", true);
					meshsDataUV->add(meshData);
					picojson::array ms2;
					
					//メッシュのサイズを書き出す
					if (part->type == SsPartType::mesh)
					{
						int meshsize = state->meshPart->ver_size;
						meshData->add(Lump::s32Data((int)state->meshPart->isBind));	//バインドの有無
						ms2.push_back(picojson::value((double)(int)state->meshPart->isBind));
						meshData->add(Lump::s32Data(meshsize));	//サイズ
						ms2.push_back(picojson::value((double)meshsize));
						int i;
						for (i = 0; i < meshsize; i++)
						{
							float u = state->meshPart->uvs[i * 2 + 0];
							float v = state->meshPart->uvs[i * 2 + 1];
							meshData->add(Lump::floatData(u));
							ms2.push_back(picojson::value(u));
							meshData->add(Lump::floatData(v));
							ms2.push_back(picojson::value(v));
						}
					}
					else
					{
						meshData->add(Lump::s32Data(0));
						ms2.push_back(picojson::value(0.0));
					}
					ms.push_back(picojson::value(ms2));
				}
				a2.insert(std::make_pair("meshsDataUV", picojson::value(ms)));
			}

			Lump* meshsDataIndices = Lump::set("ss::ss_u16*[]", true);
			{
				decoder.setPlayFrame(0);
				decoder.update();
				picojson::array ms;
				
				foreach(std::vector<SsPartAndAnime>, decoder.getPartAnime(), it)
				{
					SsPart* part = it->first;
					const SsPartState* state = findState(partList, part->arrayIndex);

					//サイズ分のUV出力
					Lump* meshData = Lump::set("ss::ss_u16*[]", true);
					meshsDataIndices->add(meshData);
					picojson::array ms2;
					
					//メッシュのサイズを書き出す
					if (part->type == SsPartType::mesh)
					{
						int tri_size = state->meshPart->tri_size;
						meshData->add(Lump::s32Data(tri_size));	//サイズ
						ms2.push_back(picojson::value((double)tri_size));
						int i;
						for (i = 0; i < tri_size; i++)
						{
							int po1 = (int)state->meshPart->indices[i * 3 + 0];
							int po2 = (int)state->meshPart->indices[i * 3 + 1];
							int po3 = (int)state->meshPart->indices[i * 3 + 2];
							meshData->add(Lump::s32Data(po1));
							ms2.push_back(picojson::value((double)po1));
							meshData->add(Lump::s32Data(po2));
							ms2.push_back(picojson::value((double)po2));
							meshData->add(Lump::s32Data(po3));
							ms2.push_back(picojson::value((double)po3));
						}
					}
					else
					{
						meshData->add(Lump::s32Data(0));
						ms2.push_back(picojson::value(0.0));
					}
				}
				a2.insert(std::make_pair("meshsDataIndices", picojson::value(ms)));
			}
			


			// フレーム毎データ
			picojson::array fs;
			Lump* frameDataIndexArray = Lump::set("ss::ss_u16*[]", true);
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
				Lump* frameData = Lump::set("ss::ss_u16[]", true);
				frameDataIndexArray->add(frameData);
				picojson::array fs2;
				
				Lump* frameFlag = Lump::s16Data(0);
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
							// 小数点以下切り捨てで
							if ((int)state->vertexValue.offsets[vtxNo].x != 0
							 || (int)state->vertexValue.offsets[vtxNo].y != 0)
							{
								vt_flags |= 1 << vtxNo;
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
					outPartsCount++;
					frameData->add(Lump::s16Data(state->index));
					fs2.push_back(picojson::value((double)state->index));
//					frameData->add(Lump::s16Data(0));				//32bitアライメント用ダミーデータ
					frameData->add(Lump::s32Data(s_flags | p_flags));
					fs2.push_back(picojson::value((double)(s_flags | p_flags)));
					frameData->add(Lump::s32Data(p_flags2));
					fs2.push_back(picojson::value((double)p_flags2));
					
					if (p_flags & PART_FLAG_CELL_INDEX)
					{
						frameData->add(Lump::s16Data(cellIndex));
						fs2.push_back(picojson::value((double)cellIndex));
					}
					if (p_flags & PART_FLAG_POSITION_X)
					{
						frameData->add(Lump::floatData(state->position.x));
						fs2.push_back(picojson::value(state->position.x));
					}
					if (p_flags & PART_FLAG_POSITION_Y)
					{
						frameData->add(Lump::floatData(state->position.y));
						fs2.push_back(picojson::value(state->position.y));
					}
					if (p_flags & PART_FLAG_POSITION_Z)
					{
						frameData->add(Lump::floatData(state->position.z));
						fs2.push_back(picojson::value(state->position.z));
					}

					if (p_flags & PART_FLAG_PIVOT_X)
					{
						frameData->add(Lump::floatData(pivot.x));
						fs2.push_back(picojson::value(pivot.x));
					}
					if (p_flags & PART_FLAG_PIVOT_Y)
					{
						frameData->add(Lump::floatData(pivot.y));
						fs2.push_back(picojson::value(pivot.y));
					}
					if (p_flags & PART_FLAG_ROTATIONX)
					{
						frameData->add(Lump::floatData(state->rotation.x));	// degree
						fs2.push_back(picojson::value(state->rotation.x));
					}
					if (p_flags & PART_FLAG_ROTATIONY)
					{
						frameData->add(Lump::floatData(state->rotation.y));	// degree
						fs2.push_back(picojson::value(state->rotation.y));
					}
					if (p_flags & PART_FLAG_ROTATIONZ)
					{
						frameData->add(Lump::floatData(state->rotation.z));	// degree
						fs2.push_back(picojson::value(state->rotation.z));
					}
					if (p_flags & PART_FLAG_SCALE_X)
					{
						frameData->add(Lump::floatData(state->scale.x));
						fs2.push_back(picojson::value(state->scale.x));
					}
					if (p_flags & PART_FLAG_SCALE_Y)
					{
						frameData->add(Lump::floatData(state->scale.y));
						fs2.push_back(picojson::value(state->scale.y));
					}
					if (p_flags & PART_FLAG_LOCALSCALE_X)
					{
						frameData->add(Lump::floatData(state->localscale.x));
						fs2.push_back(picojson::value(state->localscale.x));
					}
					if (p_flags & PART_FLAG_LOCALSCALE_Y)
					{
						frameData->add(Lump::floatData(state->localscale.y));
						fs2.push_back(picojson::value(state->localscale.y));
					}
					if (p_flags & PART_FLAG_OPACITY)
					{
						frameData->add(Lump::s16Data((int)(state->alpha * 255)));
						fs2.push_back(picojson::value((double)(int)(state->alpha * 255)));
					}
					if (p_flags & PART_FLAG_LOCALOPACITY)
					{
						frameData->add(Lump::s16Data((int)(state->localalpha * 255)));
						fs2.push_back(picojson::value((double)(int)(state->localalpha * 255)));
					}

					if (p_flags & PART_FLAG_SIZE_X)
					{
						frameData->add(Lump::floatData(state->size.x));
						fs2.push_back(picojson::value(state->size.x));
					}
					if (p_flags & PART_FLAG_SIZE_Y)
					{
						frameData->add(Lump::floatData(state->size.y));
						fs2.push_back(picojson::value(state->size.y));
					}

					if (p_flags & PART_FLAG_U_MOVE)
					{
						frameData->add(Lump::floatData(state->uvTranslate.x));
						fs2.push_back(picojson::value(state->uvTranslate.x));
					}
					if (p_flags & PART_FLAG_V_MOVE)
					{
						frameData->add(Lump::floatData(state->uvTranslate.y));
						fs2.push_back(picojson::value(state->uvTranslate.y));
					}
					if (p_flags & PART_FLAG_UV_ROTATION)
					{
						frameData->add(Lump::floatData(state->uvRotation));
						fs2.push_back(picojson::value(state->uvRotation));
					}
					if (p_flags & PART_FLAG_U_SCALE)
					{
						frameData->add(Lump::floatData(state->uvScale.x));
						fs2.push_back(picojson::value(state->uvScale.x));
					}
					if (p_flags & PART_FLAG_V_SCALE)
					{
						frameData->add(Lump::floatData(state->uvScale.y));
						fs2.push_back(picojson::value(state->uvScale.y));
					}

					if (p_flags & PART_FLAG_BOUNDINGRADIUS)
					{
						frameData->add(Lump::floatData(state->boundingRadius));
						fs2.push_back(picojson::value(state->boundingRadius));
					}

					if (p_flags & PART_FLAG_MASK)
					{
						frameData->add(Lump::s16Data(state->masklimen));
						fs2.push_back(picojson::value((double)state->masklimen));
					}
					if (p_flags & PART_FLAG_PRIORITY)
					{
						frameData->add(Lump::s16Data(state->prio));
						fs2.push_back(picojson::value((double)state->prio));
					}

					//インスタンス情報出力
					if (p_flags & PART_FLAG_INSTANCE_KEYFRAME)
					{
						frameData->add(Lump::s32Data(state->instanceValue.curKeyframe));
						fs2.push_back(picojson::value((double)state->instanceValue.curKeyframe));
						frameData->add(Lump::s32Data(state->instanceValue.startFrame));
						fs2.push_back(picojson::value((double)state->instanceValue.startFrame));
						frameData->add(Lump::s32Data(state->instanceValue.endFrame));
						fs2.push_back(picojson::value((double)state->instanceValue.endFrame));
						frameData->add(Lump::s32Data(state->instanceValue.loopNum));
						fs2.push_back(picojson::value((double)state->instanceValue.loopNum));
						frameData->add(Lump::floatData(state->instanceValue.speed));
						fs2.push_back(picojson::value(state->instanceValue.speed));
						frameData->add(Lump::s32Data(state->instanceValue.loopflag));
						fs2.push_back(picojson::value((double)state->instanceValue.loopflag));
					}
					//エフェクト情報出力
					if (p_flags & PART_FLAG_EFFECT_KEYFRAME)
					{
						frameData->add(Lump::s32Data(state->effectValue.curKeyframe));	//キー配置フレーム
						fs2.push_back(picojson::value((double)state->effectValue.curKeyframe));
						frameData->add(Lump::s32Data(state->effectValue.startTime));	//開始フレーム
						fs2.push_back(picojson::value((double)state->effectValue.startTime));
						frameData->add(Lump::floatData(state->effectValue.speed));		//再生速度
						fs2.push_back(picojson::value(state->effectValue.speed));
						frameData->add(Lump::s32Data(state->effectValue.loopflag));		//独立動作
						fs2.push_back(picojson::value((double)state->effectValue.loopflag));
					}


					// 頂点変形データ
					if (p_flags & PART_FLAG_VERTEX_TRANSFORM)
					{
						// どの頂点のオフセット値が格納されているかのフラグ
						frameData->add(Lump::s16Data(vt_flags));
						fs2.push_back(picojson::value((double)vt_flags));
						
						// 各頂点のオフセット値
						for (int vtxNo = 0; vtxNo < 4; vtxNo++)
						{
							if (vt_flags & (1 << vtxNo))
							{
								frameData->add(Lump::s16Data((int)state->vertexValue.offsets[vtxNo].x));
								fs2.push_back(picojson::value((double)(int)state->vertexValue.offsets[vtxNo].x));
								frameData->add(Lump::s16Data((int)state->vertexValue.offsets[vtxNo].y));
								fs2.push_back(picojson::value((double)(int)state->vertexValue.offsets[vtxNo].y));
							}
						}
					}

					// パーツカラーデータ
					if (p_flags & PART_FLAG_PARTS_COLOR)
					{
						// ブレンド方法と、単色もしくはどの頂点に対するカラー値が格納されているかをu16にまとめる
						int typeAndFlags = (int)state->partsColorValue.blendType | (cb_flags << 8);
						frameData->add(Lump::s16Data(typeAndFlags));
						fs2.push_back(picojson::value((double)typeAndFlags));
						
						if (cb_flags & VERTEX_FLAG_ONE)
						{
							frameData->add(Lump::floatData(state->partsColorValue.color.rate));
							fs2.push_back(picojson::value(state->partsColorValue.color.rate));
							frameData->add(Lump::colorData(state->partsColorValue.color.rgba.toARGB()));
							fs2.push_back(picojson::value((double)state->partsColorValue.color.rgba.toARGB()));
						}
						else
						{
							for (int vtxNo = 0; vtxNo < 4; vtxNo++)
							{
								if (cb_flags & (1 << vtxNo))
								{
									frameData->add(Lump::floatData(state->partsColorValue.colors[vtxNo].rate));
									fs2.push_back(picojson::value(state->partsColorValue.colors[vtxNo].rate));
									frameData->add(Lump::colorData(state->partsColorValue.colors[vtxNo].rgba.toARGB()));
									fs2.push_back(picojson::value((double)state->partsColorValue.colors[vtxNo].rgba.toARGB()));
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
							float mesh_x = state->meshPart->draw_vertices[i * 3 + 0];
							float mesh_y = state->meshPart->draw_vertices[i * 3 + 1];
							float mesh_z = state->meshPart->draw_vertices[i * 3 + 2];
							frameData->add(Lump::floatData(mesh_x));		//x
							fs2.push_back(picojson::value(mesh_x));
							frameData->add(Lump::floatData(mesh_y));		//y
							fs2.push_back(picojson::value(mesh_y));
							frameData->add(Lump::floatData(mesh_z));		//z
							fs2.push_back(picojson::value(mesh_z));
						}
					}
				}
				
				// 出力されたパーツ数と、描画順の変更があるかのフラグ
				frameFlag->data.i = outPartsCount | (prioChanged ? 0x8000 : 0);

				fs.push_back(picojson::value(fs2));
			}
			a2.insert(std::make_pair("frameData", picojson::value(fs)));
			
			
			// ユーザーデータ
			Lump* userDataIndexArray = Lump::set("ss::ss_u16*[]", true);
			bool hasUserData = false;

			picojson::array us;
			for (int frame = 0; frame < decoder.getAnimeTotalFrame(); frame++)
			{
				Lump* userData = Lump::set("ss::ss_u16[]", true);
				int partsCount = 0;
				picojson::array us2;
				
				foreach(std::vector<SsPartAndAnime>, decoder.getPartAnime(), it)
				{
					SsPart* part = it->first;
					SsPartAnime* partAnime = it->second;
					if (!partAnime) continue;
					
					foreach(SsAttributeList, partAnime->attributes, attrIt)
					{
						SsAttribute* attr = *attrIt;
						if (attr->tag != SsAttributeKind::user) continue;
						
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

							userData->add(Lump::s16Data(flags));
							us2.push_back(picojson::value((double)flags));
							userData->add(Lump::s16Data(part->arrayIndex));
							us2.push_back(picojson::value((double)part->arrayIndex));
							
							if (udat.useInteger)
							{
								userData->add(Lump::s32Data(udat.integer));
								us2.push_back(picojson::value((double)udat.integer));
							}
							if (udat.useRect)
							{
								userData->add(Lump::s32Data(udat.rect.x));
								us2.push_back(picojson::value((double)udat.rect.x));
								userData->add(Lump::s32Data(udat.rect.y));
								us2.push_back(picojson::value((double)udat.rect.y));
								userData->add(Lump::s32Data(udat.rect.w));
								us2.push_back(picojson::value((double)udat.rect.w));
								userData->add(Lump::s32Data(udat.rect.h));
								us2.push_back(picojson::value((double)udat.rect.h));
							}
							if (udat.usePoint)
							{
								userData->add(Lump::s32Data((int)udat.point.x));
								us2.push_back(picojson::value((double)(int)udat.point.x));
								userData->add(Lump::s32Data((int)udat.point.y));
								us2.push_back(picojson::value((double)(int)udat.point.y));
							}
							if (udat.useString)
							{
								const SsString& str = udat.string;
								userData->add(Lump::s16Data((int)str.length()));
								us2.push_back(picojson::value((double)(int)str.length()));
								userData->add(Lump::stringData(str));
								us2.push_back(picojson::value(str));
							}
						}
					}
				}
				
				if (partsCount)
				{
					userData->addFirst(Lump::s16Data(partsCount));
					us2.insert(us2.begin(), picojson::value((double)partsCount));
					userDataIndexArray->add(userData);
				}
				else
				{
					userDataIndexArray->add(Lump::s32Data(0));
					delete userData;
				}
				us.push_back(picojson::value(us2));
			}
			a2.insert(std::make_pair("userData", picojson::value(us)));
			

			// ラベルデータ
			Lump* LabelDataIndexArray = Lump::set("ss::ss_u16*[]", true);
			bool hasLabelData = false;
			int label_idx = 0;
			picojson::array ls;
			for (label_idx = 0; label_idx < (int)anime->labels.size(); label_idx++)
			{
				Lump* labelData = Lump::set("ss::ss_u16[]", true);
				picojson::array ls2;
				
				SsString str;
				str = anime->labels[label_idx]->name;
				//全角チェック
				if ( isZenkaku( &str ) == true )
				{
					std::cerr << "エラー：ラベルに全角が使用されている: " << str << "\n";
					convert_error_exit = true;	//エラーが発生コンバート失敗
				}

//				labelData->add(Lump::s16Data((int)str.length()));				//文字列のサイズ
				labelData->add(Lump::stringData(str));							//文字列
				ls2.push_back(picojson::value(str));
				labelData->add(Lump::s16Data(anime->labels[label_idx]->time));	//設定されたフレーム
				ls2.push_back(picojson::value((double)anime->labels[label_idx]->time));
				hasLabelData = true;

				LabelDataIndexArray->add(labelData);

				ls.push_back(picojson::value(ls2));
			}
			a2.insert(std::make_pair("labelData", picojson::value(ls)));
			if ( hasLabelData == false )
			{
				LabelDataIndexArray->add(Lump::s32Data(0));
			}
			
			animeData->add(Lump::stringData(anime->name));
			a2.insert(std::make_pair("name", picojson::value(anime->name)));
			animeData->add(initialDataArray);
			animeData->add(frameDataIndexArray);
			animeData->add(hasUserData ? userDataIndexArray : Lump::s32Data(0));
			animeData->add(hasLabelData ? LabelDataIndexArray : Lump::s32Data(0));
			animeData->add(meshsDataUV);
			animeData->add(meshsDataIndices);
			animeData->add(Lump::s16Data(decoder.getAnimeStartFrame()));
			a2.insert(std::make_pair("startFrames", picojson::value((double)decoder.getAnimeStartFrame())));
			animeData->add(Lump::s16Data(decoder.getAnimeEndFrame()));
			a2.insert(std::make_pair("endFrames", picojson::value((double)decoder.getAnimeEndFrame())));
			animeData->add(Lump::s16Data(decoder.getAnimeTotalFrame()));
			a2.insert(std::make_pair("totalFrames", picojson::value((double)decoder.getAnimeTotalFrame())));
			animeData->add(Lump::s16Data(anime->settings.fps));
			a2.insert(std::make_pair("fps", picojson::value((double)anime->settings.fps)));
			animeData->add(Lump::s16Data(label_idx));							//ラベルデータ数
			a2.insert(std::make_pair("labelNum", picojson::value((double)label_idx)));
			animeData->add(Lump::s16Data(anime->settings.canvasSize.x));		//基準枠W
			a2.insert(std::make_pair("canvasSizeW", picojson::value((double)anime->settings.canvasSize.x)));
			animeData->add(Lump::s16Data(anime->settings.canvasSize.y));		//基準枠H
			a2.insert(std::make_pair("canvasSizeH", picojson::value((double)anime->settings.canvasSize.y)));
			animeData->add(Lump::s16Data(0));									//ダミーデータ
			animeData->add(Lump::floatData(anime->settings.pivot.x));			//基準枠位置
			a2.insert(std::make_pair("canvasPvotX", picojson::value(anime->settings.pivot.x)));
			animeData->add(Lump::floatData(anime->settings.pivot.y));			//基準枠位置
			a2.insert(std::make_pair("canvasPvotY", picojson::value(anime->settings.pivot.y)));
			
			as.push_back(picojson::value(a2));
		}
		a.insert(std::make_pair("animations", picojson::value(as)));
		ssjson_anime.push_back(picojson::value(a));
	}
	ssjson.insert(std::make_pair("animePacks", picojson::value(ssjson_anime)));
	
	picojson::array ssjson_effect;
	//エフェクトデータ
	for (int effectIndex = 0; effectIndex < (int)proj->effectfileList.size(); effectIndex++)
	{
		Lump* effectFile = Lump::set("ss::EffectFile");
		effectfileArray->add(effectFile);
		picojson::object e;

		const SsEffectFile* effectfile = proj->effectfileList[effectIndex];
		effectFile->add(Lump::stringData(effectfile->name));				//エフェクト名
		e.insert(std::make_pair("name", picojson::value(effectfile->name)));

		const SsEffectModel *effectmodel = &effectfile->effectData;
		effectFile->add(Lump::s16Data(effectmodel->fps));					//FPS
		e.insert(std::make_pair("fps", picojson::value((double)(effectmodel->fps))));
		
		effectFile->add(Lump::s16Data(effectmodel->isLockRandSeed));		//乱数を固定するかどうか
		e.insert(std::make_pair("isLockRandSeed", picojson::value((double)(effectmodel->isLockRandSeed))));
		
		effectFile->add(Lump::s16Data(effectmodel->lockRandSeed));			//固定する場合の乱数の種
		e.insert(std::make_pair("LockRandSeed", picojson::value((double)(effectmodel->lockRandSeed))));
		
		effectFile->add(Lump::s16Data(effectmodel->layoutScaleX));			//レイアウトスケールX
		e.insert(std::make_pair("layoutScaleX", picojson::value((double)(effectmodel->layoutScaleX))));
		
		effectFile->add(Lump::s16Data(effectmodel->layoutScaleY));			//レイアウトスケールY
		e.insert(std::make_pair("layoutScaleY", picojson::value((double)(effectmodel->layoutScaleY))));
		
																			//エフェクトノードの出力
		effectFile->add(Lump::s16Data((int)effectmodel->nodeList.size()));	//エフェクトノード数
		e.insert(std::make_pair("numNodeList", picojson::value((double)(effectmodel->nodeList.size()))));

		Lump* effectNodeArray = Lump::set("ss::EffectNode[]", true);
		effectFile->add(effectNodeArray);									//ノード配列

		picojson::array ns;
		for (size_t nodeindex = 0; nodeindex < effectmodel->nodeList.size(); nodeindex++)
		{
			//エフェクトノードを追加
			Lump* effectNode = Lump::set("ss::EffectNode");
			effectNodeArray->add(effectNode);
			picojson::object n;

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
			effectNode->add(Lump::s16Data(arrayIndex));		//通し番号
			n.insert(std::make_pair("arrayIndex", picojson::value((double)arrayIndex)));
			effectNode->add(Lump::s16Data(parentIndex));	//親の番号
			n.insert(std::make_pair("parentIndex", picojson::value((double)parentIndex)));
			effectNode->add(Lump::s16Data(type));			//ノードの種類
			n.insert(std::make_pair("type", picojson::value((double)type)));
			effectNode->add(Lump::s16Data(cellIndex));		//セルの番号
			n.insert(std::make_pair("cellIndex", picojson::value((double)cellIndex)));
			effectNode->add(Lump::s16Data(blendType));		//描画方法
			n.insert(std::make_pair("blendType", picojson::value((double)blendType)));
			effectNode->add(Lump::s16Data(behavior.plist.size()));	//コマンドパラメータ数
			n.insert(std::make_pair("numBehavior", picojson::value((double)(behavior.plist.size()))));

			Lump* effectBehaviorArray = Lump::set("ss::ss_u16*[]", true);
			effectNode->add(effectBehaviorArray);			//コマンドパラメータ配列

			picojson::array cs;
			//コマンドパラメータ
			for (size_t plistindex = 0; plistindex < behavior.plist.size(); plistindex++)
			{
				Lump* effectBehavior = Lump::set("ss::ss_u16[]", true);
				effectBehaviorArray->add(effectBehavior);
				picojson::object c;
				
				SsEffectElementBase *elementbase = behavior.plist[plistindex];
				SsEffectFunctionType::enum_ myType = elementbase->myType;
				effectBehavior->add(Lump::s32Data(myType));	//コマンドタイプ
				c.insert(std::make_pair("SsEffectFunctionType", picojson::value((double)myType)));

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

					effectBehavior->add(Lump::s32Data(priority));				//表示優先度
					c.insert(std::make_pair("priority", picojson::value((double)priority)));
					effectBehavior->add(Lump::s32Data(maximumParticle));		//最大パーティクル数
					c.insert(std::make_pair("maximumParticle", picojson::value((double)maximumParticle)));
					effectBehavior->add(Lump::s32Data(attimeCreate));			//一度に作成するパーティクル数
					c.insert(std::make_pair("attimeCreate", picojson::value((double)attimeCreate)));
					effectBehavior->add(Lump::s32Data(interval));				//生成間隔
					c.insert(std::make_pair("interval", picojson::value((double)interval)));
					effectBehavior->add(Lump::s32Data(lifetime));				//エミッター生存時間
					c.insert(std::make_pair("lifetime", picojson::value((double)lifetime)));
					effectBehavior->add(Lump::floatData(speedMinValue));		//初速最小
					c.insert(std::make_pair("speedMinValue", picojson::value(speedMinValue)));
					effectBehavior->add(Lump::floatData(speedMaxValue));		//初速最大
					c.insert(std::make_pair("speedMaxValue", picojson::value(speedMaxValue)));
					effectBehavior->add(Lump::s32Data(lifespanMinValue));		//パーティクル生存時間最小
					c.insert(std::make_pair("lifespanMinValue", picojson::value((double)lifespanMinValue)));
					effectBehavior->add(Lump::s32Data(lifespanMaxValue));		//パーティクル生存時間最大
					c.insert(std::make_pair("lifespanMaxValue", picojson::value((double)lifespanMaxValue)));
					effectBehavior->add(Lump::floatData(angle));				//射出方向
					c.insert(std::make_pair("angle", picojson::value(angle)));
					effectBehavior->add(Lump::floatData(angleVariance));		//射出方向範囲
					c.insert(std::make_pair("angleVariance", picojson::value(angleVariance)));
					break;
				}
				case SsEffectFunctionType::RndSeedChange:
				{
					//シード上書き
					ParticleElementRndSeedChange *element = (ParticleElementRndSeedChange*)elementbase;
					int		Seed = element->Seed;
					effectBehavior->add(Lump::s32Data(Seed));					//上書きする値
					c.insert(std::make_pair("Seed", picojson::value((double)Seed)));
					break;
				}
				case SsEffectFunctionType::Delay:
				{
					//発生：タイミング
					ParticleElementDelay *element = (ParticleElementDelay*)elementbase;
					int		DelayTime = element->DelayTime;
					effectBehavior->add(Lump::s32Data(DelayTime));				//遅延時間
					c.insert(std::make_pair("DelayTime", picojson::value((double)DelayTime)));
					break;
				}
				case SsEffectFunctionType::Gravity:
				{
					//重力を加える
					ParticleElementGravity *element = (ParticleElementGravity*)elementbase;
					SsVector2   Gravity = element->Gravity;
					effectBehavior->add(Lump::floatData(Gravity.x));				//X方向の重力
					c.insert(std::make_pair("Gravity_x", picojson::value(Gravity.x)));
					effectBehavior->add(Lump::floatData(Gravity.y));				//Y方向の重力
					c.insert(std::make_pair("Gravity_y", picojson::value(Gravity.y)));
					break;
				}
				case SsEffectFunctionType::Position:
				{
					//座標：生成時
					ParticleElementPosition *element = (ParticleElementPosition*)elementbase;
					f32VValue   OffsetX = element->OffsetX;
					f32VValue   OffsetY = element->OffsetY;
					effectBehavior->add(Lump::floatData(OffsetX.getMinValue()));				//X座標に加算最小
					c.insert(std::make_pair("OffsetXMinValue", picojson::value(OffsetX.getMinValue())));
					effectBehavior->add(Lump::floatData(OffsetX.getMaxValue()));				//X座標に加算最大
					c.insert(std::make_pair("OffsetXMaxValue", picojson::value(OffsetX.getMaxValue())));
					effectBehavior->add(Lump::floatData(OffsetY.getMinValue()));				//X座標に加算最小
					c.insert(std::make_pair("OffsetYMinValue", picojson::value(OffsetY.getMinValue())));
					effectBehavior->add(Lump::floatData(OffsetY.getMaxValue()));				//X座標に加算最大
					c.insert(std::make_pair("OffsetYMaxValue", picojson::value(OffsetY.getMaxValue())));
					break;
				}
				case SsEffectFunctionType::Rotation:
				{
					//Z回転を追加
					ParticleElementRotation *element = (ParticleElementRotation*)elementbase;
					f32VValue   Rotation = element->Rotation;
					f32VValue   RotationAdd = element->RotationAdd;
					effectBehavior->add(Lump::floatData(Rotation.getMinValue()));			//角度初期値最小
					c.insert(std::make_pair("RotationMinValue", picojson::value(Rotation.getMinValue())));
					effectBehavior->add(Lump::floatData(Rotation.getMaxValue()));			//角度初期値最大
					c.insert(std::make_pair("RotationMaxValue", picojson::value(Rotation.getMaxValue())));
					effectBehavior->add(Lump::floatData(RotationAdd.getMinValue()));			//角度初期加算値最小
					c.insert(std::make_pair("RotationAddMinValue", picojson::value(RotationAdd.getMinValue())));
					effectBehavior->add(Lump::floatData(RotationAdd.getMaxValue()));			//角度初期加算値最大
					c.insert(std::make_pair("RotationAddMaxValue", picojson::value(RotationAdd.getMaxValue())));
					break;
				}
				case SsEffectFunctionType::TransRotation:
				{
					//Z回転速度変更
					ParticleElementRotationTrans *element = (ParticleElementRotationTrans*)elementbase;
					float   RotationFactor = element->RotationFactor;
					float	EndLifeTimePer = element->EndLifeTimePer;
					effectBehavior->add(Lump::floatData(RotationFactor));					//角度目標加算値
					c.insert(std::make_pair("RotationFactor", picojson::value(RotationFactor)));
					effectBehavior->add(Lump::floatData(EndLifeTimePer));					//到達時間
					c.insert(std::make_pair("EndLifeTimePer", picojson::value(EndLifeTimePer)));
					break;
				}
				case SsEffectFunctionType::TransSpeed:
				{
					//速度：変化
					ParticleElementTransSpeed *element = (ParticleElementTransSpeed*)elementbase;
					f32VValue	Speed = element->Speed;
					effectBehavior->add(Lump::floatData(Speed.getMinValue()));				//速度目標値最小
					c.insert(std::make_pair("SpeedMinValue", picojson::value(Speed.getMinValue())));
					effectBehavior->add(Lump::floatData(Speed.getMaxValue()));				//速度目標値最大
					c.insert(std::make_pair("SpeedMaxValue", picojson::value(Speed.getMaxValue())));
					break;
				}
				case SsEffectFunctionType::TangentialAcceleration:
				{
					//接線加速度
					ParticleElementTangentialAcceleration *element = (ParticleElementTangentialAcceleration*)elementbase;
					f32VValue	Acceleration = element->Acceleration;
					effectBehavior->add(Lump::floatData(Acceleration.getMinValue()));		//設定加速度最小
					c.insert(std::make_pair("AccelerationMinValue", picojson::value(Acceleration.getMinValue())));
					effectBehavior->add(Lump::floatData(Acceleration.getMaxValue()));		//設定加速度最大
					c.insert(std::make_pair("AccelerationMaxValue", picojson::value(Acceleration.getMaxValue())));
					break;
				}
				case SsEffectFunctionType::InitColor:
				{
					//カラーRGBA：生成時
					ParticleElementInitColor *element = (ParticleElementInitColor*)elementbase;
					SsU8cVValue Color = element->Color;
					effectBehavior->add(Lump::s32Data(Color.getMinValue().toARGB()));		//設定カラー最小
					c.insert(std::make_pair("ColorMinValue", picojson::value((double)Color.getMinValue().toARGB())));
					effectBehavior->add(Lump::s32Data(Color.getMaxValue().toARGB()));		//設定カラー最大
					c.insert(std::make_pair("ColorMaxValue", picojson::value((double)Color.getMaxValue().toARGB())));
					break;
				}
				case SsEffectFunctionType::TransColor:
				{
					//カラーRGB：変化
					ParticleElementTransColor *element = (ParticleElementTransColor*)elementbase;
					SsU8cVValue Color = element->Color;
					effectBehavior->add(Lump::s32Data(Color.getMinValue().toARGB()));		//設定カラー最小
					c.insert(std::make_pair("ColorMinValue", picojson::value((double)Color.getMinValue().toARGB())));
					effectBehavior->add(Lump::s32Data(Color.getMaxValue().toARGB()));		//設定カラー最大
					c.insert(std::make_pair("ColorMaxValue", picojson::value((double)Color.getMaxValue().toARGB())));
					break;
				}
				case SsEffectFunctionType::AlphaFade:
				{
					//フェード
					ParticleElementAlphaFade *element = (ParticleElementAlphaFade*)elementbase;
					f32VValue  disprange = element->disprange; // mnagaku 頭小文字
					effectBehavior->add(Lump::floatData(disprange.getMinValue()));			//表示区間開始
					c.insert(std::make_pair("disprangeMinValue", picojson::value(disprange.getMinValue())));
					effectBehavior->add(Lump::floatData(disprange.getMaxValue()));			//表示区間終了
					c.insert(std::make_pair("disprangeMaxValue", picojson::value(disprange.getMaxValue())));
					break;
				}
				case SsEffectFunctionType::Size:
				{
					//スケール：生成時
					ParticleElementSize *element = (ParticleElementSize*)elementbase;
					f32VValue SizeX = element->SizeX;
					f32VValue SizeY = element->SizeY;
					f32VValue ScaleFactor = element->ScaleFactor;
					effectBehavior->add(Lump::floatData(SizeX.getMinValue()));				//幅倍率最小
					c.insert(std::make_pair("SizeXMinValue", picojson::value(SizeX.getMinValue())));
					effectBehavior->add(Lump::floatData(SizeX.getMaxValue()));				//幅倍率最大
					c.insert(std::make_pair("SizeXMaxValue", picojson::value(SizeX.getMaxValue())));
					effectBehavior->add(Lump::floatData(SizeY.getMinValue()));				//高さ倍率最小
					c.insert(std::make_pair("SizeYMinValue", picojson::value(SizeY.getMinValue())));
					effectBehavior->add(Lump::floatData(SizeY.getMaxValue()));				//高さ倍率最大
					c.insert(std::make_pair("SizeYMaxValue", picojson::value(SizeY.getMaxValue())));
					effectBehavior->add(Lump::floatData(ScaleFactor.getMinValue()));			//倍率最小
					c.insert(std::make_pair("ScaleFactorMinValue", picojson::value(ScaleFactor.getMinValue())));
					effectBehavior->add(Lump::floatData(ScaleFactor.getMaxValue()));			//倍率最大
					c.insert(std::make_pair("ScaleFactorMaxValue", picojson::value(ScaleFactor.getMaxValue())));
					break;
				}
				case SsEffectFunctionType::TransSize:
				{
					//スケール：変化
					ParticleElementTransSize *element = (ParticleElementTransSize*)elementbase;
					f32VValue SizeX = element->SizeX;
					f32VValue SizeY = element->SizeY;
					f32VValue ScaleFactor = element->ScaleFactor;
					effectBehavior->add(Lump::floatData(SizeX.getMinValue()));				//幅倍率最小
					c.insert(std::make_pair("SizeXMinValue", picojson::value(SizeX.getMinValue())));
					effectBehavior->add(Lump::floatData(SizeX.getMaxValue()));				//幅倍率最大
					c.insert(std::make_pair("SizeXMaxValue", picojson::value(SizeX.getMaxValue())));
					effectBehavior->add(Lump::floatData(SizeY.getMinValue()));				//高さ倍率最小
					c.insert(std::make_pair("SizeYMinValue", picojson::value(SizeY.getMinValue())));
					effectBehavior->add(Lump::floatData(SizeY.getMaxValue()));				//高さ倍率最大
					c.insert(std::make_pair("SizeYMaxValue", picojson::value(SizeY.getMaxValue())));
					effectBehavior->add(Lump::floatData(ScaleFactor.getMinValue()));			//倍率最小
					c.insert(std::make_pair("ScaleFactorMinValue", picojson::value(ScaleFactor.getMinValue())));
					effectBehavior->add(Lump::floatData(ScaleFactor.getMaxValue()));			//倍率最大
					c.insert(std::make_pair("ScaleFactorMaxValue", picojson::value(ScaleFactor.getMaxValue())));
					break;
				}
				case SsEffectFunctionType::PointGravity:
				{
					//重力点の追加
					ParticlePointGravity *element = (ParticlePointGravity*)elementbase;
					SsVector2   Position = element->Position;
					float		Power = element->Power;
					effectBehavior->add(Lump::floatData(Position.x));						//重力点X
					c.insert(std::make_pair("Position_x", picojson::value(Position.x)));
					effectBehavior->add(Lump::floatData(Position.y));						//重力点Y
					c.insert(std::make_pair("Position_y", picojson::value(Position.y)));
					effectBehavior->add(Lump::floatData(Power));							//パワー
					c.insert(std::make_pair("Power", picojson::value(Power)));
					break;
				}
				case SsEffectFunctionType::TurnToDirectionEnabled:
				{
					//進行方向に向ける
					ParticleTurnToDirectionEnabled *element = (ParticleTurnToDirectionEnabled*)elementbase;
					//コマンドがあれば有効
					effectBehavior->add(Lump::floatData(element->Rotation));				//方向オフセット
					c.insert(std::make_pair("Rotation", picojson::value(element->Rotation)));
					break;
				}
				case SsEffectFunctionType::InfiniteEmitEnabled:
				{
					//無限にする
					ParticleInfiniteEmitEnabled *element = (ParticleInfiniteEmitEnabled*)elementbase;
					//コマンドがあれば有効
					effectBehavior->add(Lump::s32Data(1));									//ダミーデータ
					c.insert(std::make_pair("flag", picojson::value(1.0)));
					break;
				}
				case SsEffectFunctionType::Base:
				default:
					//未使用のコマンドが含まれている
					std::cerr << "ワーニング：未使用のエフェクトコマンドが含まれている \n";
					break;
				}
				cs.push_back(picojson::value(c));
			}
			n.insert(std::make_pair("Behavior", picojson::value(cs)));
			ns.push_back(picojson::value(n));
		}
		e.insert(std::make_pair("effectNode", picojson::value(ns)));
		ssjson_effect.push_back(picojson::value(e));
	}
	ssjson.insert(std::make_pair("effectFileList", picojson::value(ssjson_effect)));

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
			ssjson.insert(std::make_pair("creatorComment", picojson::value(creatorComment)));
			out << picojson::value(ssjson);
		}
		else if (outputFormat == OUTPUT_FORMAT_FLAG_CSOURCE)
		{
			out.open((outPath + ".c").c_str(), std::ios_base::out);
			LumpExporter::saveCSource(out, encoding, lump, "topLabel", creatorComment);
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
#define APP_VERSION		"1.0.0 (Build: " __DATE__ " " __TIME__ ")"


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
	
	
	
	std::string creatorComment = "Created by " APP_NAME " v" APP_VERSION;
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


