#ifndef __ISSGraphTexture__
#define __ISSGraphTexture__

#include <sstypes.h>
#include <map>


// nが2のべき乗かどうかチェックする
inline bool SsUtTextureisPow2(int n)
{
	for (int i = 0; i < 16; i++)
	{
		if (n == (1 << i)) return true;
	}
	return false;
}

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

public:

	ISSTexture() : refCount(0)
	{}

	virtual ~ISSTexture() {
	}

	virtual int	getWidth() = 0;
	virtual int	getHeight() = 0;
	virtual const char*	getFilename(){ return filenamepath.c_str(); }

	virtual bool	Load( const char* filename ) = 0;
	virtual ISSTexture* create() = 0;

	bool	isPow2()
	{
		return SsUtTextureisPow2( getWidth() ) && SsUtTextureisPow2( getHeight() );
	}

};

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

	static void	releaseTexture(ISSTexture* tex);

	//参照ポインタを関係なしに強制的に削除する
	static void releaseTextureForced(ISSTexture* tex);
	static void	releaseTextureForced(SsString filePath);

	static bool	isExist(SsString filePath);
	static bool	isExist(ISSTexture* texture);

	static void releaseAllTexture();

};



#endif //ifdef __ISSGraphTexture__

