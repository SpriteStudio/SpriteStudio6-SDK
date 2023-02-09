#ifndef __SSPLAYER_CELLMAP__
#define __SSPLAYER_CELLMAP__

#include "../Loader/sstypes.h"

#include <memory>

namespace SpriteStudio
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

	int convPow2(int n)
	{
		for (int i = 0; i < 30; i++)
		{
			if (n <= (1 << i))
			{
				return 1 << i;
			}
		}
		return 0x40000000;
	}

	SsCellValue& operator =(SsCellValue& r) {
		this->cell = r.cell;
		this->texture = r.texture;
		for (int i = 0; i < 5; i++)
		{
			uvs[i] = r.uvs[i];
		}
		this->wrapMode = r.wrapMode;
		this->filterMode = r.filterMode;

		return *this;
	}

	void calcUvs()
	{
		if (texture == nullptr) return;
		SsVector2 wh = SsVector2(texture->getWidth(), texture->getHeight());

		wh.x = convPow2(wh.x);
		wh.y = convPow2(wh.y);

		// 右上に向かって＋になる
		float left = cell->pos.x / wh.x;
		float right = (cell->pos.x + cell->size.x) / wh.x;
		// LB->RB->LT->RT 順
		// 頂点をZ順にしている都合上UV値は上下逆転させている
		float top = cell->pos.y / wh.y;
		float bottom = (cell->pos.y + cell->size.y) / wh.y;

		if (cell->rotated)
		{
			// 反時計回りに９０度回転されているため起こして描画されるようにしてやる。
			// 13
			// 02
			uvs[0].x = uvs[1].x = left;
			uvs[2].x = uvs[3].x = right;
			uvs[1].y = uvs[3].y = top;
			uvs[0].y = uvs[2].y = bottom;
		}
		else
		{
			// そのまま。頂点の順番は下記の通り
			// 01
			// 23
			uvs[0].x = uvs[2].x = left;
			uvs[1].x = uvs[3].x = right;
			uvs[0].y = uvs[1].y = top;
			uvs[2].y = uvs[3].y = bottom;
		}
	}
};


//cellMapは数値、文字列のみの情報としているので画像ファイルの実体と紐づける
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
	std::map<SsString, std::unique_ptr<SsCelMapLinker>>	CellMapDic;
	typedef std::map<SsString,std::unique_ptr<SsCelMapLinker>>::iterator CellMapDicItr;

	std::vector<std::unique_ptr<SsCelMapLinker>>	CellMapList;//添え字参照用

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
			itr->second.reset();
		}

		for ( size_t i = 0 ; i < CellMapList.size(); i++ )
		{
			CellMapList[i].reset();
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
		return CellMapList[index].get();
	}
	
	bool preloadTexture(SsProject* proj);
	bool unloadTexture(SsProject* proj = 0);

};



//void getCellValue( int cellMapid , SsString& cellName , SsCellValue& v );
void getCellValue( SsCellMapList* cellList, int cellMapid , SsString& cellName , SsCellValue& v );
void getCellValue( SsCellMapList* cellList, SsString& cellMapName , SsString& cellName , SsCellValue& v );

void calcUvs( SsCellValue* cellv );


}	// namespace SpriteStudio

#endif
