

#include "ssOpenGLSetting.h"
#include "ssplayer_shader_gl.h"

#include	<fstream>
#include	<iostream>
#include	<sstream>

#include	<memory>

namespace spritestudio6
{


SSOpenGLProgramObject*			glpgObject = 0 ;
std::unique_ptr<SSOpenGLShaderMan>	SSOpenGLShaderMan::m_Myinst;



using namespace std;


int SsGL_CheckShaderReady( void )
{
#ifdef _WIN32
//	if ( !GLEW_ARB_texture_rectangle ) return -1;
//    if (!( GLEW_ARB_vertex_shader && GLEW_ARB_fragment_shader )) return -1;
	//後でチェック 2/1


#endif
	const char* ptr = (const char*)glGetString(GL_SHADING_LANGUAGE_VERSION);
//    char* result;
	if (ptr != NULL)
	{
		if ( atof(ptr) < 1.2 )
		{
//			SsLogInfo( _D( "GLSL ready ... version %s NG." ), ptr );
			return -1;
		}else{
//			SsLogInfo( _D( "GLSL ready ... version %s OK." ), ptr );
		}
	}
	return 0;


  return -1;
}

SSOpenGLShader::SSOpenGLShader( const std::string&  name , const std::string& str, const GLenum shader_type )
	: myname( name ) , source( str )
{


#if USE_GLEW	
	h = glCreateShaderObjectARB( shader_type );
#else
	h = glCreateShader(shader_type);
#endif


	const char	*s = str.c_str();
	int l = str.length();

#if USE_GLEW	
	glShaderSourceARB( h, 1, &s, &l );
#else
	glShaderSource((GLuint)h, 1, &s, &l);
#endif

	if ( glGetError() != GL_NO_ERROR ) {
//		SsLogInfo( _D("ShaderObject::ShaderObject(): cannot set shader source: " ) , myname );
	}

	// compile

	Compile();


}

SSOpenGLShader::SSOpenGLShader( const std::string& filename, const GLenum shader_type )
	: myname( filename )
{

#if USE_GLEW	
	h = glCreateShaderObjectARB( shader_type );
#else
	h = glCreateShader((GLuint)shader_type);
#endif

	if ( glGetError() != GL_NO_ERROR ) {
//		SsLogInfo( _D( "SSOpenGLShader : cannot create shader object: ") , myname );
	}

	static std::string fname;

	ifstream	f_in( 
		filename.c_str() , ios::binary );

	if ( f_in.fail()) {
//		SsLogInfo( _D("ShaderObject::ShaderObject(): cannot open file: ") , filename );
	}
	ostringstream	str_out;
	str_out << f_in.rdbuf();
	source = str_out.str();
	f_in.close();

	// set shader source
	const char	*s = source.c_str();
	int l = source.length();

#if USE_GLEW	
	glShaderSourceARB( h, 1, &s, &l );
#else
	glShaderSource((GLuint)h, 1, &s, &l);
#endif

	if ( glGetError() != GL_NO_ERROR ) {
//		SsLogInfo( _D("ShaderObject::ShaderObject(): cannot set shader source: " ) , myname );
	}

	// compile

	Compile();
}






SSOpenGLShader::~SSOpenGLShader()
{

#if USE_GLEW	
	glDeleteObjectARB( h );
#else
	glDeleteShader((GLuint)h);
#endif

}

int SSOpenGLShader::Compile( void )
{
	// check whether error occurred

	if ( glGetError() != GL_NO_ERROR ) {
//		SsLogInfo( _D("GLSL Compile : errors " ) );
		return -1;
	}

	// compile
#if USE_GLEW	
	glCompileShaderARB( h );
#else
	glCompileShader((GLuint)h);
#endif
	// get errors

	GLint	result;

#if USE_GLEW	
	glGetObjectParameterivARB( h, GL_OBJECT_COMPILE_STATUS_ARB, &result );
#else
	glGetShaderiv((GLuint)h, GL_COMPILE_STATUS, &result);
#endif

	if ( glGetError() != GL_NO_ERROR || result == GL_FALSE ) {
//		SsLogInfo( _D("GLSL Compile: cannot compile shader: %s" ), myname );

		int	length;

#if USE_GLEW	
		glGetObjectParameterivARB( h, GL_OBJECT_INFO_LOG_LENGTH_ARB,
								   &length );
#else
		glGetShaderiv((GLuint)h, GL_INFO_LOG_LENGTH, &length);
#endif
		if ( length > 0 ) {
			int	l;
			// MEMO: ここはテンポラリを作って（すぐ消して）いるだけなので、スマートポインタ化していません
			GLcharARB *info_log = new GLcharARB[ length ];
#if USE_GLEW	
			glGetInfoLogARB( h, length, &l, info_log );
#else
			glGetShaderInfoLog( (GLuint)h, length, &l, info_log);
#endif

//			SsLogDbg( info_log );
			delete [] info_log;
		}
		return -1;
	}

	return 0;
}

#if USE_GLEW
#define GL_VERTEX_SHADER	GL_VERTEX_SHADER_ARB 
#define GL_FRAGMENT_SHADER	GL_FRAGMENT_SHADER_ARB 
#endif

//------------------------------------------------------------------
SSOpenGLVertexShader::SSOpenGLVertexShader( const std::string& filename )
	: SSOpenGLShader( filename, GL_VERTEX_SHADER){}


SSOpenGLVertexShader::SSOpenGLVertexShader( const std::string& name , const std::string& str )
	: SSOpenGLShader( name , str, GL_VERTEX_SHADER){}



SSOpenGLFragmentShader::SSOpenGLFragmentShader( const std::string& filename )
	: SSOpenGLShader( filename, GL_FRAGMENT_SHADER){}


SSOpenGLFragmentShader::SSOpenGLFragmentShader( const std::string& name , const std::string& str )
	: SSOpenGLShader( name , str, GL_FRAGMENT_SHADER){
}




//------------------------------------------------------------------

SSOpenGLProgramObject::SSOpenGLProgramObject()
{
	//glpgObject = this;
#if USE_GLEW
	h = glCreateProgramObjectARB();
#else
	h = glCreateProgram();
#endif

	if ( glGetError() != GL_NO_ERROR ) {
		//SsLogInfo( _D("SSOpenGLProgramObject: cannot create .. glCreateProgramObjectARB ") );
	}
}

SSOpenGLProgramObject::~SSOpenGLProgramObject()
{
#if USE_GLEW
	glDeleteObjectARB( h );
#else
	glDeleteProgram( (GLuint)h );
#endif
}

void SSOpenGLProgramObject::Attach( const SSOpenGLShader *s )
{

#if USE_GLEW
	glAttachObjectARB( h, s->GetHandle());
#else
	glAttachShader(h, s->GetHandle());
#endif

	if ( glGetError() != GL_NO_ERROR ) {
		//SsLogInfo( _D("SSOpenGLProgramObject: Attach error cannnot attach shader") );
	}
}

void 	SSOpenGLProgramObject::Disable( void ) 
{ 

#if USE_GLEW
	glUseProgramObjectARB( 0 );
#else
	glUseProgram(0);
#endif

}


GLint   SSOpenGLProgramObject::GetUniformLocation( const char *name )
{
#if USE_GLEW
    GLint ul = glGetUniformLocationARB( h, name );
#else
	GLint ul = glGetUniformLocation(h, name);

#endif


    if ( ul == -1 ) {
    }
    return ul;
}
GLint SSOpenGLProgramObject::GetAttribLocation( const char *name )
{
#if USE_GLEW
	GLint al = glGetAttribLocationARB( h, name );
#else
	GLint al = glGetAttribLocation(h, name);
#endif

    if ( al == -1 ) {
    }
    return al;
}


int
SSOpenGLProgramObject::Link( void )
{
	// check whether error occurred

	if ( glGetError() != GL_NO_ERROR ) {
		//SsLogInfo( _D("SSOpenGLProgramObject: Link error") );
		return -1;
	}

	// link

#if USE_GLEW
	glLinkProgramARB( h );
#else
	glLinkProgram(h);
#endif

	// get errors

	GLint	result;


#if USE_GLEW
	glGetObjectParameterivARB( h, GL_OBJECT_LINK_STATUS_ARB, &result );
#else
	glGetProgramiv(h, GL_LINK_STATUS, &result);	
#endif

	if ( glGetError() != GL_NO_ERROR || result == GL_FALSE ) {
		//SsLogInfo( _D("SSOpenGLProgramObject: cannot link shader object\n") );

		int	length;

#if USE_GLEW
		glGetObjectParameterivARB( h, GL_OBJECT_INFO_LOG_LENGTH_ARB,
								   &length );
#else
		glGetProgramiv(h, GL_INFO_LOG_LENGTH, &result);
#endif

		if ( length > 0 ) {
			// MEMO: ここはテンポラリを作って（すぐ消して）いるだけなので、スマートポインタ化していません
			int	l;
			GLcharARB *info_log = new GLcharARB[ length ];

#if USE_GLEW
			glGetInfoLogARB( h, length, &l, info_log );
#else
			glGetProgramInfoLog(h, length, &l, info_log);
#endif

			delete [] info_log;
		}
		return -1;
	}

	return 0;
}

void
SSOpenGLProgramObject::Use( void )
{
#if USE_GLEW
	glUseProgramObjectARB( h );
#else
	glUseProgram(h);
#endif

	if ( glGetError() != GL_NO_ERROR ) {
#if SPRITESTUDIO6SDK_PUT_UNIFORM_WARNING
//		SsLogInfo( _D("SSOpenGLProgramObject: glUseProgramObjectARB cannot use object\n") );
#endif
	}
}



}	// namespace spritestudio6
