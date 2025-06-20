﻿#include "DebugPrint.h"
#include "stdio.h"
#include <string>
#include <iostream>

#ifdef __GNUC__
	#include <stdarg.h>
#endif

#if _WIN32
	#include <Windows.h>
#endif


namespace spritestudio6
{


void DEBUG_PRINTF( const char* strFormat, ...   )
{
#if 0
	char strBuffer[1024];

	va_list arglist;
	va_start( arglist, strFormat);
#if _WIN32
	_vsnprintf(strBuffer, 1024, strFormat, arglist);
	va_end(arglist);
	OutputDebugStringA(strBuffer);
#else
	vsnprintf(strBuffer, 1024, strFormat, arglist);
	va_end(arglist);
#endif // MASTER_RELEASE

	std::cerr << strBuffer << "\n";

#endif

}

void	THROW_ERROR_MESSAGE_MAIN( std::string str , char* fname , size_t line )
{
	char	___str__buffer[1024];
	snprintf( ___str__buffer , 1024, "%s(%d) : %s \n" , fname , (int)line , str.c_str() );
	std::string ___err_message = ___str__buffer;

	DEBUG_PRINTF( ___str__buffer );

#ifndef _NOTUSE_EXCEPTION		
	throw ThrowErrorMessage( 0 , ___err_message );
#endif		

}


}	// namespace spritestudio6
