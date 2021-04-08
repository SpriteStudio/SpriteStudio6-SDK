#ifndef __DEBUG_PRINTF__
#define __DEBUG_PRINTF__

#include <string>

namespace spritestudio6
{

void DEBUG_PRINTF( const char* strFormat, ...   );

struct ThrowErrorMessage{
	std::string message;
	int	error_no;

	ThrowErrorMessage( int no , std::string str ){ error_no = no ; message = str; }
};

// MEMO: 外部名前空間からアクセスされた時の防備で、念のため完全修飾してあります。
#define SPRITESTUDIO6SDK_THROW_ERROR_MESSAGE(str) \
{\
spritestudio6::THROW_ERROR_MESSAGE_MAIN( str , __FILE__ , __LINE__ );\
}\

void	THROW_ERROR_MESSAGE_MAIN( std::string str , char* fname , size_t line );


}	// namespace spritestudio6

#endif