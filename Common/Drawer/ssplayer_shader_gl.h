#ifndef __SSOPENGLSHADER__
#define __SSOPENGLSHADER__

#include <string>
#include <vector>

#include <memory>
#include <utility>

#define SPRITESTUDIO6SDK_PUT_UNIFORM_WARNING (1)

namespace spritestudio6 {

class SSOpenGLShader {
   public:
#if USE_GLEW
    GLhandleARB h;
#else
    GLuint h;
#endif

   protected:
    std::string myname;
    std::string source;

   public:
    SSOpenGLShader(const std::string& str, const GLenum shader_type);
    SSOpenGLShader(const std::string& name, const std::string& str, const GLenum shader_type);
    virtual ~SSOpenGLShader();
    virtual int Compile(void);

#if USE_GLEW
    inline GLhandleARB GetHandle(void) const { return h; }
#else
    inline GLuint GetHandle(void) const { return h; }
#endif
};

class SSOpenGLVertexShader : public SSOpenGLShader {
   public:
    SSOpenGLVertexShader(const std::string& str);
    SSOpenGLVertexShader(const std::string& name, const std::string& str);
    virtual ~SSOpenGLVertexShader() {}
};

class SSOpenGLFragmentShader : public SSOpenGLShader {
   public:
    SSOpenGLFragmentShader(const std::string& filename);
    SSOpenGLFragmentShader(const std::string& name, const std::string& str);
    virtual ~SSOpenGLFragmentShader() {}
};

class SSOpenGLProgramObject {
   public:
#if USE_GLEW
    GLhandleARB h;
#else
    GLuint h;
#endif

   protected:
   public:
    SSOpenGLProgramObject();
    virtual ~SSOpenGLProgramObject();

#if USE_GLEW
    GLhandleARB GetID() { return h; }
#else
    GLuint GetID() { return h; }
#endif

    virtual void Attach(const SSOpenGLShader* s);
    virtual int Link(void);
    virtual void Use(void);

    inline void Enable(void) { Use(); }
    void Disable(void);
    GLint GetUniformLocation(const char* name);

    GLint GetAttribLocation(const char* name);
};

extern SSOpenGLProgramObject* glpgObject;  // カレントシェーダーオブジェクト

class SSOpenGLShaderMan {
   private:
    std::vector<std::unique_ptr<SSOpenGLProgramObject>> m_shader_list;
    static std::unique_ptr<SSOpenGLShaderMan> m_Myinst;

   public:
    SSOpenGLShaderMan() {}
    virtual ~SSOpenGLShaderMan() {}

    static SSOpenGLProgramObject* SetCurrent(int index) {
        SSOpenGLShaderMan* myInstRaw = m_Myinst.get();
        glpgObject = (myInstRaw->m_shader_list[index]).get();
        return glpgObject;
    }

    static void Create() {
        if (!m_Myinst)
            m_Myinst.reset(new SSOpenGLShaderMan());
    }

    static void Destory() {
        m_Myinst->_Destroy();
    }

    void _Destroy() {
        for (std::vector<std::unique_ptr<SSOpenGLProgramObject>>::iterator itr = m_shader_list.begin();
             itr != m_shader_list.end(); itr++) {
            itr->reset();
        }
        (m_Myinst.get())->m_shader_list.clear();

        if (m_Myinst) {
            m_Myinst.reset();
        }
    }

    static void PushPgObject(SSOpenGLProgramObject* obj) {
        (m_Myinst.get())->m_shader_list.push_back(std::move(std::unique_ptr<SSOpenGLProgramObject>(obj)));
    }
};

int SsGL_CheckShaderReady(void);
extern SSOpenGLShaderMan* glshaderMan;

}  // namespace spritestudio6

#endif
