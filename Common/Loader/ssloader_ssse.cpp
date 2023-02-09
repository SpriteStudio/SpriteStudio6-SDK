#include "ssloader_ssse.h"

namespace SpriteStudio
{


	SsAudioPack* ssloader_ssse::Load(const std::string& filename )
{

		SsAudioPack* audioPack = new SsAudioPack();

	libXML::XMLDocument xml;
	if ( libXML::XML_SUCCESS == xml.LoadFile( filename.c_str() ) )
	{
		SsXmlIArchiver ar( xml.GetDocument() , "SpriteStudioSoundList" );
		audioPack->__Serialize( &ar );
	}else{
		delete audioPack;
		audioPack = 0;
	}

	return audioPack;
}


}	// namespace SpriteStudio
