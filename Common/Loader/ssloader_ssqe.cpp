#include "ssloader_ssqe.h"
#include "ssFilesystem.h"

namespace spritestudio6
{


SsSequencePack*	ssloader_ssqe::Load(const std::string& filename )
{
	libXML::XMLDocument xml;

	FILE *fp = ssFilesystem::openFile(filename);
	if (fp == NULL) return 0;
	libXML::XMLError result = xml.LoadFile(fp);
	fclose(fp);

	SsSequencePack* sequence = new SsSequencePack();
	if ( libXML::XML_SUCCESS == result )
	{
		SsXmlIArchiver ar( xml.GetDocument() , "SpriteStudioSequencePack" );
		sequence->__Serialize( &ar );
	}else{
		delete sequence;
		sequence = 0;
	}

	return sequence;
}


SsSequence*	SsSequencePack::findSequence(SsString& name)
{

	for ( std::vector<SsSequence*>::iterator itr = sequenceList.begin()
		; itr != sequenceList.end() ; ++itr )
	{
		if ( (*itr)->name == name )
		{
			return (*itr);
		}
	}
	return 0;
}

}	// namespace spritestudio6
