#include "ssloader_ssqe.h"


SsSequencePack*	ssloader_ssqe::Load(const std::string& filename )
{

	SsSequencePack* sequence = new SsSequencePack();

	XMLDocument xml;
	if ( XML_SUCCESS == xml.LoadFile( filename.c_str() ) )
	{
		SsXmlIArchiver ar( xml.GetDocument() , "SpriteStudioSequencePack" );
		sequence->__Serialize( &ar );
	}else{
		delete sequence;
		sequence = 0;
	}

	return sequence;
}


SsSequence*	SsSequencePack::findSequence(SsString& _name)
{

	for ( std::vector<SsSequence*>::iterator itr = sequenceList.begin()
		; itr != sequenceList.end() ; ++itr )
	{
		if ( (*itr)->name == _name )
		{
			return (*itr);
		}
	}
	return 0;
}
