#include <stdio.h>
#include <cstdlib>


#include "../../Drawer/ssOpenGLSetting.h"

/*
#ifndef _WIN32
    #include <OpenGL/gl.h>
    #include <OpenGL/glu.h>
    #include <OpenGL/glext.h>
#else
    #include <GL/glew.h>
    #include <GL/GL.h>
#endif
*/

#include "SSTextureGL.h"
#include "../Helper/DebugPrint.h"

namespace SpriteStudio
{

/* =====================================================================================
	テクスチャファイルの読み込み
===================================================================================== */
SSTextureGL::~SSTextureGL()
{
	if ( tex != 0 )
	{
		glDeleteTextures( 1 , &tex );
	}
}

bool SSTextureGL::Load( const char* fname )
{
	int bpp;
	SSTextureLoader::DataHandle image = SSTextureLoader::LoadImageFromFile( fname, &tex_width , &tex_height , &bpp );
	if ( image == SSTextureLoader::InvalidDataHandle ) return false;

	loadFromMemory(image, tex_width, tex_height, bpp);

	SSTextureLoader::DecodeEndImageFile( image );

	return true;
}

bool SSTextureGL::loadFromMemory(uint8_t* image, int width, int height, int bpp)
{

#if USE_GLEW
	int target = GL_TEXTURE_RECTANGLE_ARB;
#else
	int target = GL_TEXTURE_RECTANGLE;
#endif

	if (SSTextureLoader::CheckSizePow2(tex_width, tex_height))
	{
		target = GL_TEXTURE_2D;
	}

	glGenTextures(1, &tex);
	if (tex == 0) return false;
	glBindTexture(target, tex);

	tex_width = width;
	tex_height = height;

	glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_DECAL);
	glTexParameteri(target, GL_TEXTURE_WRAP_S, GL_REPEAT);
	glTexParameteri(target, GL_TEXTURE_WRAP_T, GL_REPEAT);
	glTexParameterf(target, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameterf(target, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	if (bpp == 4)
	{
		/*
				//Ver6 ではストレートアルファで処理を行うのでコメントにする
				stbi_uc *ip = image;
				for ( int i = 0 ; i < width * height ; i++ )
				{
					stbi_uc* r = ip; ip ++;
					stbi_uc* g = ip; ip ++;
					stbi_uc* b = ip; ip ++;
					stbi_uc* a = ip; ip ++;
		//			if ( *a == 0 )
					{
						//*r = *g = *b = 0xff;
						int _a = *a;
						int _r = *r;
						int _g = *g;
						int _b = *b;
						*r = ( _r * _a) >> 8 ;
						*g = ( _g * _a) >> 8 ;
						*b = ( _b * _a) >> 8 ;
					}
				}
		*/
		glTexImage2D(target, 0, GL_RGBA, tex_width, tex_height, 0, GL_RGBA, GL_UNSIGNED_BYTE, (const void*)image);
		
		//for (int i = 0; i < tex_width * tex_height ;  i++)
		//{
		//	DEBUG_PRINTF( "%d \n" , image[i] );
		//}
	}
	else if (bpp == 3)
	{
		glTexImage2D(target, 0, GL_RGBA, tex_width, tex_height, 0, GL_RGB, GL_UNSIGNED_BYTE, (const void*)image);
	}


	return true;
}
}	// namespace SpriteStudio
