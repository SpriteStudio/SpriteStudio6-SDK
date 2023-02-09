#ifndef UNICODE_CONV_H_
#define UNICODE_CONV_H_

#include <array>
#include <string>

#ifndef uint8_t
typedef unsigned char uint8_t;
#endif



template <class type, size_t num>
class wtarray
{
	type	_array[num];
	size_t	_arraySize;
	type	_overrun;

public:

	wtarray(wtarray &src) : _arraySize(num)
	{
		for (size_t i = 0; i < _arraySize; i++)
			this->_array[i] = src._array[i];
	}

	wtarray() : _arraySize(num) {}
	virtual ~wtarray() {}

	type operator[](size_t index) const
	{
		if (index >= _arraySize)
		{
			return _overrun;	//添え字オーバーの場合
		}
		return _array[index];
	}


	type& operator[](size_t index)
	{
		if (index >= _arraySize)
		{
			return _overrun;	//添え字オーバーの場合
		}
		return _array[index];
	}

	size_t	byteOf() { return sizeof(type)*_arraySize; }	
	size_t	sizeOf() { return _arraySize; }


};




bool ConvChU8ToU16(const wtarray<char, 4>& u8Ch, wtarray<char16_t, 2>& u16Ch);
bool ConvChU8ToU32(const wtarray<char, 4>& u8Ch, char32_t& u32Ch);

bool ConvChU16ToU8(const wtarray<char16_t, 2>& u16Ch, wtarray<char, 4>& u8Ch);
bool ConvChU16ToU32(const wtarray<char16_t, 2>& u16Ch, char32_t& u32Ch);

bool ConvChU32ToU8(const char32_t u32Ch, wtarray<char, 4>& u8Ch);
bool ConvChU32ToU16(const char32_t u32Ch, wtarray<char16_t, 2>& u16Ch);

bool ConvU8ToU16(const std::string& u8Str, std::u16string& u16Str);
bool ConvU8ToU32(const std::string& u8Str, std::u32string& u32Str);

bool ConvU16ToU8(const std::u16string& u16Str, std::string& u8Str);
bool ConvU16ToU32(const std::u16string& u16Str, std::u32string& u32Str);

bool ConvU32ToU8(const std::u32string& u32Str, std::string& u8Str);
bool ConvU32ToU16(const std::u32string& u32Str, std::u16string& u16Str);





#endif  // UNICODE_CONV_H_
