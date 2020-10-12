#ifndef __SSOPENGLSHADER__
#define __SSOPENGLSHADER__

#include <string>
#include <vector>

#if 1	/* Smart-Ptr */
#include <memory>
#include <utility>
#else
#endif	/* Smart-Ptr */

#define SPRITESTUDIO6DSK_PUT_UNIFORM_WARNIG	(1)

namespace spritestudio6
{

class	SSOpenGLShader {
public:
	GLhandleARB	h;

protected:
	std::string	myname;
	std::string	source;

public:
	SSOpenGLShader( const std::string& str, const GLenum shader_type );
	SSOpenGLShader( const std::string& name, const std::string& str, const GLenum shader_type );
	virtual	~SSOpenGLShader();
	virtual	int Compile( void );
	inline GLhandleARB GetHandle( void )	const { return h; }
};


class	SSOpenGLVertexShader : public SSOpenGLShader
{
public:
	SSOpenGLVertexShader( const std::string& str );
	SSOpenGLVertexShader( const std::string& name , const std::string& str );
	virtual	~SSOpenGLVertexShader(){}

};


class	SSOpenGLFragmentShader : public SSOpenGLShader
{
public:
	SSOpenGLFragmentShader( const std::string& filename );
	SSOpenGLFragmentShader( const std::string& name , const std::string& str );
	virtual	~SSOpenGLFragmentShader(){}

};


class	SSOpenGLProgramObject {
public:
	GLhandleARB	h;

protected:

public:
	SSOpenGLProgramObject(  );
	virtual	~SSOpenGLProgramObject(  );

	GLhandleARB GetID(){ return h; }

	virtual	void	Attach( const SSOpenGLShader *s );
	virtual	int		Link( void );
	virtual void	Use( void );

	inline	void 	Enable( void ) { Use(); }
    void 	Disable( void );
	GLint GetUniformLocation( const char *name );

	GLint GetAttribLocation( const char *name );
};

extern 	SSOpenGLProgramObject*			glpgObject;    //カレントシェーダーオブジェクト

class   SSOpenGLShaderMan
{
private:
#if 1	/* Smart-Ptr */
    std::vector<std::unique_ptr<SSOpenGLProgramObject>> m_shader_list;
	static	std::unique_ptr<SSOpenGLShaderMan>	m_Myinst;
#else
    std::vector<SSOpenGLProgramObject*> m_shader_list;
    static	SSOpenGLShaderMan*			m_Myinst;
#endif	/* Smart-Ptr */
public:
	SSOpenGLShaderMan(){}
	virtual ~SSOpenGLShaderMan(){}

	static SSOpenGLProgramObject*	SetCurrent(int index)
	{
#if 1	/* Smart-Ptr */
		SSOpenGLShaderMan* myInstRaw = m_Myinst.get();
		glpgObject = (myInstRaw->m_shader_list[index]).get();
#else
		glpgObject = m_Myinst->m_shader_list[index];
#endif	/* Smart-Ptr */
		return glpgObject;
	}

	static  void	Create()
	{
#if 1	/* Smart-Ptr */
		if ( m_Myinst == false )
			m_Myinst.reset( new SSOpenGLShaderMan() );
#else
		if ( m_Myinst == 0)
			m_Myinst = new  SSOpenGLShaderMan();
#endif	/* Smart-Ptr */
	}

	static	void	Destory()
	{
		m_Myinst->_Destroy();
	}

	void		_Destroy()
	{
#if 1	/* Smart-Ptr */
		for ( std::vector<std::unique_ptr<SSOpenGLProgramObject>>::iterator itr = m_shader_list.begin();
				itr != m_shader_list.end() ; itr++ )
		{
			itr->reset();
		}
		(m_Myinst.get())->m_shader_list.clear();

		if ( m_Myinst )
		{
			m_Myinst.reset();
		}
#else
		for ( std::vector<SSOpenGLProgramObject*>::iterator itr = m_shader_list.begin();
				itr != m_shader_list.end() ; itr++ )
		{
			delete (*itr);
		}
       	m_Myinst->m_shader_list.clear();

		if ( m_Myinst != 0)
        {
			delete m_Myinst;
			m_Myinst = 0;
		}
#endif	/* Smart-Ptr */
	}

	static void	PushPgObject(SSOpenGLProgramObject *obj)
	{
#if 1	/* Smart-Ptr */
		(m_Myinst.get())->m_shader_list.push_back( std::move( std::unique_ptr( obj ) ) );
#else
    	m_Myinst->m_shader_list.push_back(obj);
#endif	/* Smart-Ptr */
	}

};



int SsGL_CheckShaderReady( void );
extern	SSOpenGLShaderMan*              glshaderMan;

}	// namespace spritestudio6

#endif

