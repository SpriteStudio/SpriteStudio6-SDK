#include <stdio.h>
#include <cstdlib>

#ifndef _WIN32
    #include <OpenGL/gl.h>
    #include <OpenGL/glu.h>
    #include <OpenGL/glext.h>
#else
    #include <GL/glew.h>
    #include <GL/GL.h>
#endif

#if 1	/* Smart-Ptr */
#else
#include "../stb_image.h"
#include "../ssHelper.h"
#endif	/* Smart-Ptr */
#include "SSTextureGL.h"


namespace spritestudio6
{

/* =====================================================================================
	テクスチャファイルの読み込み
===================================================================================== */
#if 1	/* Smart-Ptr */
#else
GLuint	LoadTextureGL( const char* Filename ,int& width , int& height)
{
	int bpp;
	stbi_uc* image = stbi_load( Filename, &width , &height , &bpp , 0 );
	if ( image == 0 ) return 0;

	int target = GL_TEXTURE_RECTANGLE_ARB;
	 
	if (SsUtTextureisPow2(width) &&
		SsUtTextureisPow2(height))
	{
		target = GL_TEXTURE_2D;
	}


	GLuint glyphTexture = 0;
	glGenTextures(1, &glyphTexture);
	glBindTexture(target, glyphTexture);

	glTexEnvf( GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_DECAL );
	glTexParameteri(target, GL_TEXTURE_WRAP_S, GL_REPEAT);
	glTexParameteri(target, GL_TEXTURE_WRAP_T, GL_REPEAT);
	glTexParameterf(target, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameterf(target, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	if ( bpp == 4 )
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
		glTexImage2D(target, 0, GL_RGBA, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, image);	
	}else if ( bpp == 3 )
	{
		glTexImage2D(target, 0, GL_RGBA, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, image);	
	}
	stbi_image_free (image);

	return glyphTexture;
}
#endif	/* Smart-Ptr */


SSTextureGL::~SSTextureGL()
{
#if 1	/* Smart-Ptr */
	if ( tex != 0 )
	{
		glDeleteTextures( 1 , &tex );
	}
#else
	glDeleteTextures( 1 , &tex );
#endif	/* Smart-Ptr */
}

bool SSTextureGL::Load( const char* fname )
{
#if 1	/* Smart-Ptr */
	int bpp;
	SSTextureLoader::DataHandle image = SSTextureLoader::LoadImageFromFile( fname, &tex_width , &tex_height , &bpp );
	if ( image == SSTextureLoader::InvalidDataHandle ) return false;

	int target = GL_TEXTURE_RECTANGLE_ARB;

	if ( SSTextureLoader::CheckSizePow2( tex_width, tex_height ) )
	{
		target = GL_TEXTURE_2D;
	}

	glGenTextures(1, &tex);
	if ( tex == 0 ) return false;
	glBindTexture( target, tex );

	glTexEnvf( GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_DECAL );
	glTexParameteri( target, GL_TEXTURE_WRAP_S, GL_REPEAT );
	glTexParameteri( target, GL_TEXTURE_WRAP_T, GL_REPEAT );
	glTexParameterf( target, GL_TEXTURE_MAG_FILTER, GL_NEAREST );
	glTexParameterf( target, GL_TEXTURE_MIN_FILTER, GL_NEAREST );
	if ( bpp == 4 )
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
		glTexImage2D( target, 0, GL_RGBA, tex_width, tex_height, 0, GL_RGBA, GL_UNSIGNED_BYTE, (const void*)image );
	}else if ( bpp == 3 )
	{
		glTexImage2D( target, 0, GL_RGBA, tex_width, tex_height, 0, GL_RGB, GL_UNSIGNED_BYTE, (const void*)image );
	}
	SSTextureLoader::DecodeEndImageFile( image );

	return true;
#else
	//int tex_width;
	//int tex_height;

	tex = LoadTextureGL( fname , tex_width , tex_height );
	return tex != 0;
#endif	/* Smart-Ptr */
}

}	// namespace spritestudio6
