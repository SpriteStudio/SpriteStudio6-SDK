﻿#ifndef __SSPLAYER_CELLMAP__
#define __SSPLAYER_CELLMAP__



class SsAnimeDecoder;
class SsCelMapLinker;

///パーツが使用するセルの情報
struct SsCellValue
{
	SsCell*						cell;		///参照しているセル
	ISSTexture*					texture;	///テクスチャ
	SsVector2					uvs[5];		///使用するUV座標
	SsTexWrapMode::_enum		wrapMode;	///< テクスチャのラップモード
	SsTexFilterMode::_enum		filterMode;	///< テクスチャのフィルタモード

	SsCellValue() :  
		cell(0) ,  
		texture(0)
		{}
};

class SsCelMapLinker
{
public:
	SsCellMap*			cellMap;
	ISSTexture*	tex;

	std::map<SsString,SsCell*>	CellDic;

public:
	SsCelMapLinker()
		: cellMap(0) , tex(0)
	{}

	SsCelMapLinker(SsCellMap* cellmap, SsString filePath);

	virtual ~SsCelMapLinker()
	{
		CellDic.clear();

		if (tex)
		{
			SSTextureFactory::releaseTexture(tex);
			//delete tex;
			tex = 0;
		}
	}

	SsCell*	findCell( const SsString& name ){ return CellDic[name]; }
	
};

//プロジェクト全体で保持しているセルマップ
//現在はprojectのセルマップの列挙とssaeの列挙は同一
class	SsCellMapList
{
private:
	//同名セルマップは上書き
	std::map<SsString,SsCelMapLinker*>		CellMapDic;
	std::vector<SsCelMapLinker*>			CellMapList;//添え字参照用

	typedef std::map<SsString,SsCelMapLinker*>::iterator CellMapDicItr;
	SsString	CellMapPath;

private:
	void	addIndex(SsCellMap* cellmap);
	void	addMap(SsCellMap* cellmap);

public:
	SsCellMapList(){}
	virtual ~SsCellMapList()
	{
		for ( CellMapDicItr itr = CellMapDic.begin() ; itr != CellMapDic.end() ; itr ++)
		{
			delete itr->second;
		}

		for ( size_t i = 0 ; i < CellMapList.size(); i++ )
		{
			delete CellMapList[i];
		}
		CellMapList.clear();
		CellMapDic.clear();
	}

	void	clear();
	size_t	size(){ return CellMapList.size(); }

	void	setCellMapPath(  const SsString& filepath );

	//projectとanimepackからアニメーションの再生に必要なセルマップのリストを作成する
	//アニメパックのセルリストに登載されている順にセルマップを読み込みインデックス化する
	//SsProjectを介してセルを検索しているのはセルがそこにアレイで確保されているから
	//もし既に読み込み済みだったりする場合は、アニメパックのセルＩＤ順にセルマップを登録すればいい
	void	set(SsProject* proj , SsAnimePack* animepack );

	SsCelMapLinker*	getCellMapLink( const SsString& name );
	SsCelMapLinker*	getCellMapLink( int index )
	{	
		if (CellMapList.size() <= index) return 0;
		return CellMapList[index];
	}
	
	bool preloadTexture(SsProject* proj);
	bool unloadTexture(SsProject* proj = 0);

};



//void getCellValue( int cellMapid , SsString& cellName , SsCellValue& v );
void getCellValue( SsCellMapList* cellList, int cellMapid , SsString& cellName , SsCellValue& v );
void getCellValue( SsCellMapList* cellList, SsString& cellMapName , SsString& cellName , SsCellValue& v );

void calcUvs( SsCellValue* cellv );

#endif
