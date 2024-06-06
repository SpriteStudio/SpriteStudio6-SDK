#include "ssloader_ssce.h"
#include "ssstring_uty.h"
#include "ssFilesystem.h"


namespace spritestudio6
{


SsCellMap*	ssloader_ssce::Parse(const char* xmlstr , size_t len, int* error)
{
	libXML::XMLDocument xml;

	libXML::XMLError xmlerr = xml.Parse(xmlstr, len);

	if ( libXML::XML_SUCCESS == xmlerr)
	{
		SsXmlIArchiver ar( xml.GetDocument() , "SpriteStudioCellMap" );

		SsCellMap* cellmap = new SsCellMap();
		cellmap->__Serialize( &ar );
		return cellmap;
	}
	else {
		if (error)
		{
			*error = xmlerr;
		}
	}

	return 0;
}


SsCellMap*	ssloader_ssce::Load(const std::string& filename )
{
	libXML::XMLDocument xml;

	FILE *fp = ssFilesystem::openFile(filename);
	if (fp == NULL) return 0;
	libXML::XMLError result = xml.LoadFile(fp);
	fclose(fp);

	SsCellMap* cellmap = new SsCellMap();
	if ( libXML::XML_SUCCESS == result )
	{
		SsXmlIArchiver ar( xml.GetDocument() , "SpriteStudioCellMap" );
		cellmap->__Serialize( &ar );
		cellmap->fname = path2file( filename );
	}else{
		delete cellmap;
		cellmap = 0;
	}

	return cellmap;
}




}	// namespace spritestudio6
