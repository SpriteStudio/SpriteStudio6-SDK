#ifndef __TKTEXTURE__
#define __TKTEXTURE__

//#include "../IsshTexture.h"
#include "../SsTextureFactory.h"

namespace SpriteStudio
{

class	SSTextureGL : public ISSTexture
{
public:
	GLuint	tex;
	int tex_width;
	int	tex_height;

public:
	SSTextureGL() : tex_width(0) , tex_height(0) , tex(0){}
	virtual ~SSTextureGL();
	bool Load( const char* fname );	

	virtual int	getWidth() { return tex_width; }
	virtual int	getHeight() { return tex_height; }

	virtual ISSTexture* create(){ return new SSTextureGL(); }
	virtual bool loadFromMemory(uint8_t* ptr, int width, int height ,int bpp);

};


}	// namespace SpriteStudio

#endif
