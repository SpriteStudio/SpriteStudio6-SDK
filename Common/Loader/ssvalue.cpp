#include "ssarchiver.h"
#include "ssstring_uty.h"
#include "ssvalue.h"
#include "sscharconverter.h"


//SsValue用のシリアライザ
void	SsValueSeriarizer( ISsXmlArchiver* ar , SsValue& v , const std::string key) 
{

	//インプット
	XMLElement* e = ar->getxml();
	if ( key != "" )
	{
		e = e->FirstChildElement( key.c_str() );
	}
	if ( e )
	{
		XMLElement* child = e->FirstChildElement();

		if ( child == 0 )
		{
			const char* str = e->GetText();

			if (str==0)
			{
				return ;
			}
			
			std::string enc_str = str;
			v = SsValueSeriarizer__MakeValue( enc_str.c_str() );
		}else{
			XMLElement* ce = child;

			SsHash hash;
			while(ce)
			{
				const char* str = ce->GetText();
				if ( str != 0 )
				{
					std::string enc_str = str;
					hash[ce->Name()] = SsValueSeriarizer__MakeValue( enc_str.c_str() );
					ce = (XMLElement*)ce->NextSibling();
				}else{
					//さらに子構造があるようだ
					//さらに子構造があるようだ
					//ce = 0 ;
					SsXmlIArchiver _ar(ce);

					SsValue tempv;
					SsValueSeriarizer( &_ar , tempv , "");
					hash[ce->Name()] = tempv;					
					ce = (XMLElement*)ce->NextSibling();
				}
			}

			v = SsValue(hash);

		}

		return ;
	}
	//assert(e);

}
