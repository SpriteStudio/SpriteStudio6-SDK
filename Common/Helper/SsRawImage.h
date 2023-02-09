#ifndef __SSRAWIMAGE__
#define __SSRAWIMAGE__

#include "../Loader/sstypes.h"
#include "SsTextureFactory.h"


namespace SpriteStudio
{
	//ビットマップイメージのロード、作成、操作を提供する

#if 1
class SsRawImage : public ISSTexture
{
private:

//	std::unique_ptr<uint8_t> rawimage;
	uint8_t* rawimage;
	int width;
	int height;
	int bpp;

	bool is_autorelease;

public : 
	SsRawImage() : ISSTexture() , rawimage(nullptr) , width(0) ,height(0) , is_autorelease(true)
	{

	}

	SsRawImage(int _width, int _height) : SsRawImage()
	{

		create(_width, _height);
	}

	virtual ~SsRawImage();
//	void create(int width, int height, int bitdepth)

	//RGBAで作成する
//	void create(int width, int height, SsColor color = SsColor( 255,255,255,255 ));
	void create(int width, int height, SsColor color = SsColor(0,0,0,0));
	void create_and_copy();

	uint8_t* getRaw() { return rawimage; }

	virtual int	getWidth() override { return width; }
	virtual int	getHeight() override { return height;  }
	virtual int getBpp() { return bpp; }

	virtual bool	Load(const char* filename);
	virtual ISSTexture* create() { return 0; }

	virtual bool loadFromMemory(uint8_t* ptr, int width, int height, int bpp);

	void fill(SsColor color);

	//ImageからImageへの転送(リサイズあり）
	bool Bitblt(SsRawImage* src, SsRectI srcRect, SsRectI dstRect);


};
#endif

}	// namespace SpriteStudio

#endif //ifdef __ISSGraphTexture__

