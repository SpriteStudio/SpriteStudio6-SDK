#ifndef __ISSGraphTexture__
#define __ISSGraphTexture__

#include "../Loader/sstypes.h"
#include <map>

#include <memory>

namespace SpriteStudio
{


// nが2のべき乗かどうかチェックする
inline bool SsUtTextureisPow2(int n)
{
	for (int i = 0; i < 16; i++)
	{
		if (n == (1 << i)) return true;
	}
	return false;
}

class	SSTextureLoader
{
public:
	typedef uint8_t* DataHandle;
	static DataHandle InvalidDataHandle;

	typedef DataHandle (*PrototypeLoadImageFromFile)( const char* fileName, int* width, int* height, int* bpp );
	typedef void (*PrototypeDecodeEndImageFile)( DataHandle handle );
	typedef const char* (*PrototypeMessageGetFailureLoadFromFile)();
	typedef bool (*PrototypeCheckSizePow2)( int width, int height );

	//MEMO: 下記関数群に関数ポインタを定義することで、テクスチャの標準読込関数を変更することができます（ただし実行時には本関数ポインタ群を直接実行しないでください）。
	//      ※関数を変更する際には、FunctionCheckSizePow2以外の関数は全てワンセットで変更する必要があります（機能実装に使用しているライブラリなどが異なるため）。
	//      ※これらの関数ポインタをnullptrに設定すると、次回使用時に標準実装関数に強制的に書き戻されます。
	static PrototypeLoadImageFromFile FunctionLoadImageFromFile;							// テクスチャファイルのメモリ確保・ロードと初期解析処理（通常ISSTexture::Load関数から呼び出されます）
	static PrototypeDecodeEndImageFile FunctionDecodeEndImageFile;							// LoadImageFromFileで解析したテクスチャファイルのデコード終了（通常ISSTexture::Load関数から呼び出されます）
	static PrototypeMessageGetFailureLoadFromFile FunctionMessageGetFailureLoadFromFile;	// LoadImageFromFileでエラーが起こった場合のエラーメッセージの取得（通常ISSTexture::Load関数から呼び出されます） ※エラー取得できない場合nullptrを返してください
	static PrototypeCheckSizePow2 FunctionCheckSizePow2;									// テクスチャのXY辺長が2のn乗かのチェック

	//実行用関数群
	static DataHandle LoadImageFromFile( const char* fileName, int* width=nullptr, int* height=nullptr, int* bpp=nullptr );
	static void DecodeEndImageFile( DataHandle handle );
	static const char* MessageGetFailureLoadFromFile();
	static bool CheckSizePow2( int width, int height );
};

class SSTextureFactory;
class ISSTexture
{
private:
	friend SSTextureFactory;

	int refCount;

	int addref() { return ++refCount; }
	int	release(){ 
		return --refCount; }

	SsString filenamepath;
	SSTextureLoader::DataHandle dataHandle;

public:
	ISSTexture() : refCount(0), dataHandle(SSTextureLoader::InvalidDataHandle), filenamepath()
	{}

	virtual ~ISSTexture()
	{
	}

	virtual int	getWidth() = 0;
	virtual int	getHeight() = 0;
	virtual const char*	getFilename(){ return filenamepath.c_str(); }

	virtual bool	Load( const char* filename ) = 0;
	virtual ISSTexture* create() = 0;

	virtual bool loadFromMemory(uint8_t* ptr, int width, int height, int bpp) { return false; }


	bool	isPow2()
	{
		return SsUtTextureisPow2( getWidth() ) && SsUtTextureisPow2( getHeight() );
	}
};

class SsRawImage;


class	SSTextureFactory
{
private:
	static ISSTexture*			m_texture_base_class;
	static SSTextureFactory*	m_myInst;

	std::map<SsString, ISSTexture*> textureCache;


private:
	static ISSTexture*	create() { return m_texture_base_class->create(); }

public:
	SSTextureFactory(){}
	SSTextureFactory(ISSTexture* texture_base_class)
	{ 
		m_myInst = this ; m_texture_base_class = texture_base_class;
	}

	virtual ~SSTextureFactory()
	{
		if ( m_texture_base_class )
			delete m_texture_base_class;
		m_myInst = 0;
	}

	static bool	isExist(){ return m_myInst != 0; }
	
	static ISSTexture*	loadTexture(SsString filePath);

	static ISSTexture* create(SsRawImage* img, SsString Tag = "unkown");

	static void	releaseTexture(ISSTexture* tex);

	//参照ポインタを関係なしに強制的に削除する
	static void releaseTextureForced(ISSTexture* tex);
	static void	releaseTextureForced(SsString filePath);

	static bool	isExist(SsString filePath);
	static bool	isExist(ISSTexture* texture);

	static void releaseAllTexture();

};


}	// namespace SpriteStudio

#endif //ifdef __ISSGraphTexture__

