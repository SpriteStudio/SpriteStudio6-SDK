#include "../Loader/sstypes.h"
#include "SsTextureFactory.h"

#include "SsRawImage.h"
#include "../Helper/DebugPrint.h"

#include "stb_image.h"

#include "../Libs/avir/avir.h"

namespace SpriteStudio
{




SsRawImage::~SsRawImage() 
{
	if (is_autorelease)
	{
		delete rawimage;
	}
}


void SsRawImage::create(int width, int height , SsColor color)
{
	if (rawimage)
	{
		delete rawimage;
	}

	this->width = width;
	this->height = height;
	this->bpp = 4;

	int arraysize = width * height * 4;
//	rawimage.reset( new uint8_t[arraysize] );
	rawimage = new uint8_t[arraysize];

	uint8_t* ptr = rawimage;


	fill(color);
}


void SsRawImage::fill(SsColor color)
{
	uint8_t* ptr = rawimage;
	for (int i = 0; i < width * height * this->bpp ; i += 4)
	{
		ptr[i + 0] = color.r;
		ptr[i + 1] = color.g;
		ptr[i + 2] = color.b;
		ptr[i + 3] = color.a;
	}
}

static uint8_t* getImagePtr(uint8_t* ptr, int bpp , int x, int y , int width)
{
	//uint8_t* ptr = img->getRaw();
	//int bpp = img->getBpp();

	uint8_t* ret = ptr + width * bpp* y + (x * bpp);

	return ret;
}

bool SsRawImage::loadFromMemory(uint8_t* ptr, int width, int height, int bpp)
{
	create(width, height);

	for (int oy = 0; oy < height; oy++)
	{
		for (int ox = 0; ox < width; ox++)
		{
			uint8_t* srcPtr = getImagePtr(ptr, 4 , ox,  oy , width);
			uint8_t* dstPtr = getImagePtr(this->rawimage, 4 , ox,  oy , width);

			dstPtr[0] = srcPtr[0];
			dstPtr[1] = srcPtr[1];
			dstPtr[2] = srcPtr[2];
			dstPtr[3] = srcPtr[3];
		}
	}

	return true;
}


bool SsRawImage::Bitblt(SsRawImage* src, SsRectI srcRect, SsRectI dstRect)
{

	int InBufSize = srcRect.width() * srcRect.height() * 4;
	int OutBufSize = dstRect.width()* dstRect.height() * 4;


	uint8_t* InBuf = new uint8_t[InBufSize];
	uint8_t* OutBuf = new uint8_t[OutBufSize];


	//一旦バッファにコピー
	for (int oy = 0; oy < srcRect.height() ; oy++)
	{
		for (int ox = 0; ox < srcRect.width() ; ox++)
		{
			uint8_t* srcPtr = getImagePtr(src->rawimage, 4, srcRect.x() + ox, srcRect.y() + oy, src->width);
			uint8_t* dstPtr = getImagePtr(InBuf, 4, ox,  oy, srcRect.width() );

			dstPtr[0] = srcPtr[0];
			dstPtr[1] = srcPtr[1];
			dstPtr[2] = srcPtr[2];
			dstPtr[3] = srcPtr[3];
		}
	}

	//Dstサイズにリサイズ
	avir::CImageResizer<> ImageResizer(8);
	ImageResizer.resizeImage(
		InBuf, srcRect.width(), srcRect.height(), 0,
		OutBuf, dstRect.width(), dstRect.height(), 4, 0);


	//リサイズ後バッファからコピー
	for (int oy = 0; oy < dstRect.height(); oy++)
	{
		for (int ox = 0; ox < dstRect.width(); ox++)
		{
			uint8_t* srcPtr = getImagePtr(OutBuf, 4 , ox,  oy , dstRect.width() );
			uint8_t* dstPtr = getImagePtr(this->rawimage, 4,  dstRect.x() + ox, dstRect.y() + oy , this->width);

			dstPtr[0] = srcPtr[0];
			dstPtr[1] = srcPtr[1];
			dstPtr[2] = srcPtr[2];
			dstPtr[3] = srcPtr[3];
		}
	}

	delete InBuf;
	delete OutBuf;

	return true;
}

bool	SsRawImage::Load(const char* filename)
{ 
	SSTextureLoader::DataHandle ptr = SSTextureLoader::LoadImageFromFile(filename,
		&this->width,
		&this->height,
		&this->bpp
	);
	if (ptr == 0) return false;
	rawimage = ptr;

	return true; 
}



}	// namespace SpriteStudio
