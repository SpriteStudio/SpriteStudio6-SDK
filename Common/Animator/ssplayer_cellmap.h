#ifndef __SSPLAYER_CELLMAP__
#define __SSPLAYER_CELLMAP__

#if 1	/* Smart-Ptr */
#include "sstypes.h"

#include <memory>
#else
#endif	/* Smart-Ptr */

namespace spritestudio6
{

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
#if 1	/* Smart-Ptr */
	std::map<SsString, std::unique_ptr<SsCelMapLinker>>	CellMapDic;
	typedef std::map<SsString,std::unique_ptr<SsCelMapLinker>>::iterator CellMapDicItr;

	std::vector<std::unique_ptr<SsCelMapLinker>>	CellMapList;//添え字参照用
#else
	std::map<SsString,SsCelMapLinker*>		CellMapDic;
	typedef std::map<SsString,SsCelMapLinker*>::iterator CellMapDicItr;

	std::vector<SsCelMapLinker*>			CellMapList;//添え字参照用
#endif	/* Smart-Ptr */

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
#if 1	/* Smart-Ptr */
			itr->second.reset();
#else
			delete itr->second;
#endif	/* Smart-Ptr */
		}

		for ( size_t i = 0 ; i < CellMapList.size(); i++ )
		{
#if 1	/* Smart-Ptr */
			CellMapList[i].reset();
#else
			delete CellMapList[i];
#endif	/* Smart-Ptr */
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
#if 1	/* Smart-Ptr */
		return CellMapList[index].get();
#else
		return CellMapList[index];
#endif	/* Smart-Ptr */
	}
	
	bool preloadTexture(SsProject* proj);
	bool unloadTexture(SsProject* proj = 0);

};



//void getCellValue( int cellMapid , SsString& cellName , SsCellValue& v );
void getCellValue( SsCellMapList* cellList, int cellMapid , SsString& cellName , SsCellValue& v );
void getCellValue( SsCellMapList* cellList, SsString& cellMapName , SsString& cellName , SsCellValue& v );

void calcUvs( SsCellValue* cellv );


}	// namespace spritestudio6

#endif
