#ifndef __SSLOADER_SSQE__
#define __SSLOADER_SSQE__

#include "sstypes.h"
#include "ssarchiver.h"

#define SPRITESTUDIO6_SSQEVERSION "1.00.00"

class SsSequence;





class SsSequence
{
public:
	SsString	name;							/// シーケンスの名称

public:
	SsSequence(){}
	virtual ~SsSequence()
	{
	}

	///シリアライズのための宣言です。
	SSSERIALIZE_BLOCK
	{
		SSAR_DECLARE( name );
	}
};

/**
*@class SsSequencePack
*@brief アニメーションを組み合わせた構造とその構造を使用するシーケンスを格納するデータです。
アニメーションの組み合わせ構造をSsSequenceで定義しています。
*/
class SsSequencePack
{
public:
	SsString					version;
	SsString					name;			//!< シーケンスパック名称
	std::vector<SsSequence*>	sequenceList;	//!< 格納されている子シーケンスのリスト

	SsSequencePack(){}
	virtual ~SsSequencePack()
	{
		for ( std::vector<SsSequence*>::iterator itr = sequenceList.begin() ; 
			itr != sequenceList.end() ; itr ++ ) delete (*itr);		
	}

	///シリアライズのための宣言です。
	SSSERIALIZE_BLOCK
	{
		SSAR_DECLARE_ATTRIBUTE(version);
		SSAR_DECLARE( name );
		SSAR_DECLARE_LISTEX( sequenceList , "sequence" );


	}

	//シーケンス名からシーケンスを取得する
	SsSequence*	findSequence(SsString& name);

};

/*
* @class ssloader_ssqe
* @brief ssqeファイルをロードするためのクラスです。
*/
class	ssloader_ssqe
{
public:
	ssloader_ssqe(){}
	virtual ~ssloader_ssqe(){}

	///ssqeファイル名を指定しロードが成功したらそのSsSequencePackのポインタを返します。
	static SsSequencePack*	Load(const std::string& filename );

};






#endif
