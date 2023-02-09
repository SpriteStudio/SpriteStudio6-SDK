#ifndef __SSLOADER_SSSE__
#define __SSLOADER_SSSE__

#include "sstypes.h"
#include "ssarchiver.h"

#define SPRITESTUDIO6_SSSEVERSION "1.00.00"

namespace SpriteStudio
{


struct SsSoundFile
{
public:
	SsString Alias;
	SsString filename;
	int msec;
	//int temp_index;
	//bool    exist;  //ファイルが存在するかどうか


	SsSoundFile() : msec(0) {}

	SPRITESTUDIO6SDK_SERIALIZE_BLOCK
	{
		SPRITESTUDIO6SDK_SSAR_DECLARE(Alias);
		SPRITESTUDIO6SDK_SSAR_DECLARE(filename);
		SPRITESTUDIO6SDK_SSAR_DECLARE(msec);
	}

};


struct SsSoudFileList
{
public:
	int id; //どちらかというよUID
	SsString Alias;
	std::vector<SsSoundFile*> sounds;


	SsSoudFileList() : id(0), Alias("")
	{
	}

	~SsSoudFileList() {
		for (auto i : sounds)
		{
			delete i;
		}
		sounds.clear();
	}

	SPRITESTUDIO6SDK_SERIALIZE_BLOCK
	{
		SPRITESTUDIO6SDK_SSAR_DECLARE(id);
		SPRITESTUDIO6SDK_SSAR_DECLARE(Alias);
		SPRITESTUDIO6SDK_SSAR_DECLARE_LIST(sounds);
	}

};


class SsAudioPack 
{
public:
	SsString		version;
	SsString		name;
	SsSoudFileList  table;



	SPRITESTUDIO6SDK_SERIALIZE_BLOCK
	{
		SPRITESTUDIO6SDK_SSAR_DECLARE_ATTRIBUTE(version);
		SPRITESTUDIO6SDK_SSAR_DECLARE(name);
		SPRITESTUDIO6SDK_SSAR_STRUCT_DECLARE(table);

	}

};


/*
* @class ssloader_ssse
* @brief ssseファイルをロードするためのクラスです。
*/
class	ssloader_ssse
{
public:
	ssloader_ssse(){}
	virtual ~ssloader_ssse(){}

	///ssseファイル名を指定しロードが成功したらそのSsSequencePackのポインタを返します。
	static SsAudioPack*	Load(const std::string& filename );

};





}	// namespace SpriteStudio

#endif
