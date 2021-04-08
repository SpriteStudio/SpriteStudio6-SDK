﻿#ifndef __SSLOADER_SSCE__
#define __SSLOADER_SSCE__

#include "sstypes.h"
#include "ssarchiver.h"

#define SPRITESTUDIO6_SSCEVERSION "2.00.00"

///パーツに使用される画素の矩形範囲を示した構造です。
class SsCell 
{
public:
	//--------- ランタイム用データとして保存すべきもの
	SsString	name;			///< セル名称
	SsPoint2	pos;			///< 左上の座標
	SsPoint2	size;			///< WHピクセルサイズ
	SsPoint2	pivot;			///< WHピクセルサイズ
//	SsVector2	pivot;			///< 原点。size /2 が中央=0,0になる。
	bool		rotated;		///< 左方向に９０度回転されている。uvs の割り当てが変わる。

	SsPoint2	parentSize;		//親テクスチャのサイズ
	//---------- メッシュ化機能 --------------
	bool ismesh;

	//エディット用のポイントリスト
	std::vector<SsPoint2>	 innerPoint;
	std::vector<SsPoint2>	 outerPoint;

	//実際に使用されるメッシュ構造
	std::vector<SsPoint2>   		meshPointList;  //ポイントリスト
	std::vector<SsTriangle>  		meshTriList;    //トライアングルリスト

	SsMeshDivType::_enum			divtype;
	int	divw;
	int	divh;

	SsCell(){}
	virtual ~SsCell() {
/*
		for (std::vector<SsVector2*>::iterator itr = innerPoint.begin();
			itr != innerPoint.end(); itr++) delete (*itr);
		for (std::vector<SsVector2*>::iterator itr = outerPoint.begin();
			itr != outerPoint.end(); itr++) delete (*itr);
		for (std::vector<SsVector2*>::iterator itr = meshPointList.begin();
			itr != meshPointList.end(); itr++) delete (*itr);
		for (std::vector<SsTriangle*>::iterator itr = meshTriList.begin();
			itr != meshTriList.end(); itr++) delete (*itr);
*/
	}


	///シリアライズのための宣言です。
	SSSERIALIZE_BLOCK
	{
		SSAR_DECLARE( name );
		SSAR_DECLARE( pos );
		SSAR_DECLARE( size );
		SSAR_DECLARE( pivot );
		SSAR_DECLARE( rotated );

		SSAR_DECLARE(ismesh);
		//SsVerctor2のリストのシリアライズが必要
		SSAR_DECLARE(innerPoint );
		SSAR_DECLARE(outerPoint);
		SSAR_DECLARE(meshPointList);
		SSAR_DECLARE(meshTriList);
		SSAR_DECLARE_ENUM(divtype);
		SSAR_DECLARE(divw);
		SSAR_DECLARE(divh);
	}
};



//!セルマップデータを表現するためのクラスです。
/*!
@class SsCellMap
@breif セルマップは１つのテクスチャファイルとそのテクスチャ内でパーツとして使用する矩形範囲を示したセルをコンテナとして保持するための構造です。<BR>
<BR>
このデータコンテナはエディット用として下記を読み飛ばします。<BR>
imagePathAtImport;///< インポート時の参照元画像のフルパス<BR>
packInfoFilePath;	///< パック情報ファイル。TexturePacker 等のデータをインポートした場合のみ有効<BR>
texPackSettings;	///< パック時の参照情報<BR>
*/
class SsCellMap
{
public:
	SsString					version;
	SsString					fname;				///< セルマップのファイルネーム
	SsString					name;				///< このセルマップの名称です。
	SsString					imagePath;			///< 参照画像ファイルパス。プロジェクトの画像基準相対
	SsPoint2					pixelSize;			///< 画像のピクセルWHサイズ
	bool						overrideTexSettings;///< テクスチャ設定をプロジェクトの設定ではなく下記設定を使う
	SsTexWrapMode::_enum		wrapMode;			///< テクスチャのラップモード
	SsTexFilterMode::_enum		filterMode;			///< テクスチャのフィルタモード

	std::vector<SsCell*>		cells;

	SsString					loadFilepath;		///<実際にロードした場所


	SsCellMap(){}
	virtual ~SsCellMap()
	{
		for ( std::vector<SsCell*>::iterator itr = cells.begin() ; 
			itr != cells.end() ; itr ++ ) delete (*itr);	
	}

	///シリアライズのための宣言です。
	SSSERIALIZE_BLOCK
	{
		SSAR_DECLARE_ATTRIBUTE(version);
		SSAR_DECLARE( name );
		SSAR_DECLARE( imagePath );
		SSAR_DECLARE( pixelSize );
		SSAR_DECLARE( overrideTexSettings );
		SSAR_DECLARE_ENUM( wrapMode );
		SSAR_DECLARE_ENUM( filterMode );

		SSAR_DECLARE_LISTEX( cells ,"cell" );
	}
};


//!sspjのローダークラスです。
class ssloader_ssce
{
public:
	ssloader_ssce(){}
	virtual ~ssloader_ssce(){}

	static SsCellMap*	Load(const std::string& filename );

};


#endif
