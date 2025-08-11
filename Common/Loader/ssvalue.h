#ifndef __SSVALUE__
#define __SSVALUE__

#include "ssarchiver.h"
#include "ssstring_uty.h"
#include <map>
#include <vector>
#include <cassert>

namespace spritestudio6
{

class SsValue;

typedef	wchar_t		SsChar;
typedef std::vector<SsValue>		SsArray;
typedef std::map<SsString,SsValue>	SsHash;

//SsValue用のシリアライザ
void	SsValueSeriarizer( ISsXmlArchiver* ar , SsValue& v , const std::string key = "value" );


class SsValue{
public:
	enum{
		unknown,
		string_type,
		int_type,
		float_type,
		boolean_type,
		hash_type,
		array_type,
	};

public:
	int			type;
	SsString	name;
	SsString	org_txt;

	union{
		SsString*				_str;
		int						_int;	
		float					_float;
		bool					_bool;
		SsArray*				_array;
		SsHash*					_hash;
		void*					_ptr;
	};

	int		_int_temp;
	float	_float_temp;
	bool	_bool_temp;


	SsValue(): type(unknown) { init(); }

	explicit SsValue(bool b) : type(boolean_type) { init(); _bool = b; }
	explicit SsValue(int n, char* org = 0) : type(int_type) {
		init();
		_int = n; 
		if (org)
			org_txt = SsString(org);
	}
	explicit SsValue(float n, char* org = 0) : type(float_type)
	{ 
		init();
		_float = n; 
		if (org)
			org_txt = SsString(org);

	}
	explicit SsValue(SsString& str)   { init(); type = string_type; _str = new SsString(str); }
	explicit SsValue(const char* str)   { init(); type = string_type; _str = new SsString(str); }
	explicit SsValue(SsArray& n)
	{ 
		init();
		type = array_type; 
		_array = new SsArray(n);
	}


	explicit SsValue(SsHash& n)  { init(); type = hash_type; _hash = new SsHash(n); }


    SsValue(const SsValue& x)
	{
		*this = x;
	}
    SsValue& operator=(const SsValue& x)
	{
		type = x.type;
		name = x.name;
		org_txt = x.org_txt;

		// This is not enough because temp values must be determined by the value of my type which is string, int, float and boolean.
		_int_temp = x._int_temp;
		_float_temp = x._float_temp;
		_bool_temp = x._bool_temp;

		switch (x.type)
		{
		default:
		case unknown:
			_ptr = x._ptr;
			break;
		case string_type:
			_str = new SsString(*x._str);
			_float_temp = (float)double_from_string(x._str->c_str());
			_int_temp = atoi(_str->c_str());
			break;
		case int_type:
			_int = x._int;
			_float_temp = (float)_int;
			_bool_temp = _int == 1 ? true : false;
			break;
		case float_type:
			_float = x._float;
			_int_temp = (int)_float;
			_bool_temp = _float > 0.0f ? true : false;
			break;
		case boolean_type:
			_bool = x._bool;
			_int_temp = _bool ? 1 : 0;
			_float_temp = _bool ? 1.0f : 0.0f;
			break;
		case hash_type:
			_hash = new SsHash(*x._hash);
			break;
		case array_type:
			_array = new SsArray(*x._array);
			break;
		}
		return *this;
	}

	void	init()
	{
		_ptr = nullptr;
		_int_temp = 0;
		_float_temp = 0.0f;
		_bool_temp = false;
	}

	virtual ~SsValue() {
		if (type == string_type && _str) {
			delete _str;
			return;
		}

		if (type == array_type && _array) {
			delete _array;
			return;
		}
		if (type == hash_type && _hash)
		{
			delete _hash;
			return;
		}
	}

	template <typename T> bool is() const;
	template <typename T> const T& get() const;
	template <typename T> T& get();

	const SsValue& operator[](const std::string& key)  const
	{
		static SsValue r_value;

		if ( type == hash_type )
		{
			SsHash::const_iterator i = _hash->find(key);
			return i != _hash->end() ? i->second : r_value;
		}

		assert(1);
		return *this;
	}

	bool	IsExistHashkey( const std::string& key ) const
	{
		if ( type == hash_type )
		{
			SsHash::const_iterator i = _hash->find(key);
			return i != _hash->end();
		}

		return false;
	}

	SPRITESTUDIO6SDK_SERIALIZE_BLOCK
	{
		SsValueSeriarizer( ar , *this ,"" );	
	}
	
};

template <> inline const SsString& SsValue::get<SsString>() const {
	static SsString ret = "";
	if ( this->type != string_type )
	{
		if (this->type == int_type )
		{
			//char tmp[64+1];
			//sprintf_s( tmp , 64 , "%d" , _int );
			//ret = tmp;
			ret = org_txt;
		}

		return ret;
	}
	return *_str;
}
template <> inline SsString& SsValue::get<SsString>() {
	static SsString ret = "";
	if ( this->type != string_type )
	{
		if ( this->type == float_type )
		{
			ret = std::to_string( (long double)_float);
		}else if ( this->type == int_type )
		{
#ifdef _WIN32
            ret = std::to_string( (int)_int);
#else
            ret = std::to_string( (int)(_int) );
#endif
		}
		return ret;
	}
	return *_str;
}

template <> inline const int& SsValue::get<int>() const {
	if ( this->type == float_type )
	{
		return _int_temp;
	}else{
		return _int;
	}
}
template <> inline int& SsValue::get<int>() {
	if ( this->type == float_type )
	{
		return _int_temp;
	}else{
		return _int;
	}
}

template <> inline const float& SsValue::get<float>() const {
	if ( this->type == float_type )
	{
		return _float;
	}else{
		return _float_temp;
	}
}
template <> inline float& SsValue::get<float>() {
	if ( this->type == float_type )
	{
		return _float;
	}else{
		return _float_temp;
	}
}

template <> inline const bool& SsValue::get<bool>() const {
	if ( this->type == boolean_type )
	{
		return _bool;
	}else{
		return _bool_temp;
	}
}
template <> inline bool& SsValue::get<bool>() {
	if ( this->type == boolean_type )
	{
		return _bool;
	}else{
		return _bool_temp;
	}
}

template <> inline const SsArray& SsValue::get<SsArray>() const {
	return *_array;
}
template <> inline SsArray& SsValue::get<SsArray>() {
	return *_array;
}


template <> inline const SsHash& SsValue::get<SsHash>() const {
	return *_hash;
}
template <> inline SsHash& SsValue::get<SsHash>() {
	return *_hash;
}



template <> inline bool SsValue::is<bool>() const {
	return type == boolean_type;
}	

template <> inline bool SsValue::is<int>() const {
	return type == int_type;
}	

template <> inline bool SsValue::is<float>() const {
	return type == float_type;
}
template <> inline bool SsValue::is<SsString>() const {
	return type == string_type;
}

template <> inline bool SsValue::is<SsArray>() const {
	return type == array_type;
}

template <> inline bool SsValue::is<SsHash>() const {
	return type == hash_type;
}




inline static  SsValue	SsValueSeriarizer__MakeValue( const char* v )
{
    std::string temp = v;
	bool has_period;

	if ( is_digit_string( temp , &has_period) )
	{
		if ( has_period )
		{
			return SsValue( (float)double_from_string( v ) , (char*)v );
		}

		return SsValue( (int)atoi( v ), (char*)v);

	}else{
		return  SsValue( v );
	}

}



}	// namespace spritestudio6

#endif
