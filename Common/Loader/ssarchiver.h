﻿#ifndef  __SSARCHIVER__
#define  __SSARCHIVER__

#include "./tinyxml2/tinyxml2.h"
#include "sstypes.h"
#include <string>
#include <vector>
#include <map>

using namespace tinyxml2;



/// シリアライズクラスのInput/Outputの状態
struct EnumSsArchiver
{
	enum Type
	{
		unkown = 0, //不明
		in,			//Input
		out,		//Output
	};
};

class SsXmlIArchiver;

//class SsXmlStringConverter
class SsXmlRangeValueConverter
{
public:
	SsXmlRangeValueConverter(){}
	virtual ~SsXmlRangeValueConverter(){}

	virtual bool	inputString( SsString value , SsString subvalue ) = 0;
};


/// アーカイバクラスのインターフェース
class ISsXmlArchiver
{
private:
	XMLElement*		m_xml;

public:

	virtual EnumSsArchiver::Type getType(){ return EnumSsArchiver::unkown;}


	ISsXmlArchiver(){}
	virtual ~ISsXmlArchiver(){}

	XMLElement*	getxml(){ return m_xml;} 

	void	firstChild(ISsXmlArchiver* ar , const char* element_name)
	{
		m_xml = ar->getxml()->FirstChildElement( element_name );
	}

	void	setDocumet( tinyxml2::XMLDocument* doc , const char* element_name)
	{
		m_xml = doc->FirstChildElement(element_name);
	}

	void	setElement(XMLElement* e )
	{
		m_xml = e;
	}

	virtual bool	dc( const char* name , int& member ) = 0;
	virtual bool	dc( const char* name , float& member ) = 0;
	virtual bool	dc( const char* name , bool& member ) = 0;
	virtual bool	dc( const char* name , std::vector<SsString>& list ) = 0;

	virtual bool	dc( const char* name , SsString& member ) = 0;
	virtual bool	dc( const char* name , SsPoint2& member ) = 0;
	virtual bool	dc( const char* name , SsCurve& member ) = 0;
	virtual bool	dc( const char* name , SsXmlRangeValueConverter& member ) = 0;

	virtual bool	dc(const char* name, std::vector<SsPoint2>& list) = 0;
	virtual bool	dc(const char* name, std::vector<SsTriangle>& list) = 0;
	virtual bool	dc(const char* name, std::map<SsString, int>& _map) = 0;


	virtual bool	dc_attr( const char* name , SsString& member ) = 0;
	virtual bool	dc_attr( const char* name , int& member ) = 0;



};


inline	static int	GetTextToInt(XMLElement* e , int default_value )
{
	int ret =default_value; 
	const char* v = e->GetText();
	if ( v )
	{
		ret = atoi(v);
	}
	return ret;
}


/// アーカイバクラス　インプットクラス
class SsXmlIArchiver : public ISsXmlArchiver
{
public:
	virtual EnumSsArchiver::Type getType(){ return EnumSsArchiver::in;}


	SsXmlIArchiver(ISsXmlArchiver* ar , const char* element_name)
	{
		firstChild( ar , element_name );
	}
	SsXmlIArchiver( tinyxml2::XMLDocument* doc , const char* element_name)
	{
		setDocumet( doc , element_name );
	}

	SsXmlIArchiver(XMLElement* e )
	{
		setElement( e );
	}


	virtual ~SsXmlIArchiver(){}
	

	virtual bool	dc( const char* name , int& member );
	virtual bool	dc( const char* name , float& member );
	virtual bool	dc( const char* name , SsString& member );
	virtual bool	dc( const char* name , bool& member );
	virtual bool	dc( const char* name , std::vector<SsString>& list );
	virtual bool	dc( const char* name , SsPoint2& member );
	virtual bool	dc( const char* name , SsCurve& member );
	virtual bool	dc( const char* name , SsTriangle& member);
//	virtual bool	dc( const char* name , SsBoneBind& member);
	
	virtual bool	dc( const char* name , SsXmlRangeValueConverter& member )
	{
		XMLElement* e = getxml()->FirstChildElement( name );
		SsXmlIArchiver _ar(e);

		SsString str;
		_ar.dc_attr( "value" , str );
		
		SsString str2;
		_ar.dc_attr( "subvalue" , str2 );

		return member.inputString(str,str2);		
	}

	virtual bool	dc(const char* name, std::vector<SsPoint2>& list);
	virtual bool	dc(const char* name, std::vector<SsTriangle>& list);

	virtual bool	dc(const char* name, std::map<SsString,int>& _map);


	virtual bool	dc_attr( const char* name , SsString& member );
	virtual bool	dc_attr( const char* name , int& member );




	template<class myclass> bool	dc( const char* name , std::vector<myclass*>& list , const std::string key = "value" )
	{
		list.clear();
		XMLElement* e = getxml()->FirstChildElement( name );
		if (e == 0)return false;
		if ( key != "" )
			e = e->FirstChildElement( key.c_str() );

		while( e )
		{
			myclass* _temp = new myclass();
			SsXmlIArchiver _ar(e);
			_temp->__Serialize( &_ar );
			list.push_back( _temp );
			e = e->NextSiblingElement();
		}

		return true;
	}

	template<class myclass> bool	dc( const char* name , myclass& type )
	{
		SsString str;
		dc( name , str );
		__StringToEnum_( str , type);
		
		return false;
	}

	template<class myclass> bool	dc_attr( const char* name , myclass& type )
	{
		SsString str;
		dc_attr( name , str );
		__StringToEnum_( str , type);
		
		return false;
	}

};


#define SSSERIALIZE_BLOCK	void __Serialize(ISsXmlArchiver* ar)

#define	SSAR_DECLARE(t)  ar->dc(#t,t)
#define	SSAR_DECLARE_ATTRIBUTE(t)  ar->dc_attr(#t,t)

#define	SSAR_STRUCT_DECLARE(t)  {SsXmlIArchiver _ar( ar , #t );\
t.__Serialize( &_ar );}\

template<class myclass>
inline bool	__SSAR_DECLARE_LIST__( ISsXmlArchiver* ar , std::vector<myclass*>& list, const char* name ,const std::string key = "value" )
{
	if ( ar->getType() == EnumSsArchiver::in )
	{
		return (static_cast<SsXmlIArchiver*>(ar))->dc( name , list , key );
	}

	if ( ar->getType() == EnumSsArchiver::out )
	{
	}

	return false;
}


#define	SSAR_DECLARE_LIST(t)  __SSAR_DECLARE_LIST__( ar , t , #t)
#define	SSAR_DECLARE_LIST2(t,s)  __SSAR_DECLARE_LIST__( ar , t , s)
#define	SSAR_DECLARE_LISTEX(t,key)  __SSAR_DECLARE_LIST__( ar , t , #t , key )

template<class myclass>
inline bool	__SSAR_DECLARE_ENUM__( ISsXmlArchiver* ar ,myclass& type, const char* name  )
{
	if ( ar->getType() == EnumSsArchiver::in )
	{
		return (static_cast<SsXmlIArchiver*>(ar))->dc( name , type );
	}

	if ( ar->getType() == EnumSsArchiver::out )
	{
	}
	return false;
}

template<class myclass>
inline bool	__SSAR_DECLARE_ATTRIBUTE_ENUM__( ISsXmlArchiver* ar ,myclass& type, const char* name  )
{
	if ( ar->getType() == EnumSsArchiver::in )
	{
		return (static_cast<SsXmlIArchiver*>(ar))->dc_attr( name , type );
	}

	if ( ar->getType() == EnumSsArchiver::out )
	{
	}
	return false;
}



#define SSAR_DECLARE_ENUM(t) __SSAR_DECLARE_ENUM__( ar , t , #t)
#define SSAR_DECLARE_ATTRIBUTE_ENUM(t) __SSAR_DECLARE_ATTRIBUTE_ENUM__( ar , t , #t)


bool	StringToPoint2( const std::string& str , SsPoint2& point );
bool	StringToIRect( const std::string& str , SsIRect& rect );
bool	StringToTriangle(const std::string& str, SsTriangle& tri);


///SpriteStudio XMLデータ読み書きの初期化
void	SsArchiverInit();


#define AR_SELF_CHECK() if ( this->getxml() == 0 ) return false;


#endif
