#include <stdio.h>
#include <cstdlib>
#include <iostream>

#if 1	/* Smart-Ptr */
#else
#include "stb_image.h"
#include "ssHelper.h"
#endif	/* Smart-Ptr */
#include "sshTextureBMP.h"

namespace spritestudio6
{

bool SSTextureBMP::Load( const char* fname )
{
#if 1	/* Smart-Ptr */
	int bpp;

	m_filename = "";

	SSTextureLoader::DataHandle image = SSTextureLoader::LoadImageFromFile( fname, &tex_width , &tex_height , &bpp );
	if( image == SSTextureLoader::InvalidDataHandle )
	{
		const char* msg = SSTextureLoader::MessageGetFailureLoadFromFile();
		if( msg )
		{
			std::cerr << fname << " err:" << msg << "\n";
		}
		return false;
	}

	m_filename = fname;
	
	SSTextureLoader::DecodeEndImageFile(image);
	return true;
#else
	int bpp;

	m_filename = "";

	stbi_uc* image = stbi_load( fname, &tex_width , &tex_height , &bpp , 0 );
	if ( image == 0 )
	{
		const char* msg = stbi_failure_reason();
		std::cerr << fname << " err:" << msg << "\n";
		return false;
	}

	m_filename = fname;
	
	stbi_image_free (image);
	return true;
#endif	/* Smart-Ptr */
}



}	// namespace spritestudio6
