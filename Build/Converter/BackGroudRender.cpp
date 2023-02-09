#include "BackGroudRender.h"
#include <string>
#include <iostream>

#include "ssOpenGLSetting.h"
#if USE_NATIVE_OSMESA
 #define GLFW_EXPOSE_NATIVE_OSMESA
 #include <GLFW/glfw3native.h>
#endif

#include "ssplayer_render.h"
#include "ssplayer_render_gl.h"
#include "OpenGL/SSTextureGL.h"

#define STB_IMAGE_WRITE_IMPLEMENTATION
#include "stb_image.h"
#include "stb_image_write.h"




static SpriteStudio::SSTextureFactory* texfactory = nullptr;


GLuint FramebufferName = 0;
GLuint renderedTexture;
GLuint depthrenderbuffer;
GLFWwindow* window;

static bool isGPUInit = false;


static void error_callback(int error, const char* description)
{
    fprintf(stderr, "Error: %s\n", description);
}

bool ConverterOpenGLInit()
{

    GLuint vertex_buffer, vertex_shader, fragment_shader, program;
    GLint mvp_location, vpos_location, vcol_location;
    float ratio;
    int width, height;
    width = 325;
    height = 325;

    //    mat4x4 mvp;
    char* buffer;

    glfwSetErrorCallback(error_callback);

    glfwInitHint(GLFW_COCOA_MENUBAR, GLFW_FALSE);

    if (!glfwInit())
        exit(EXIT_FAILURE);

#ifdef _WIN32
    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 2);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 0);
#else
    // macOS
    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 2);
	glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);	/* 前方互換プロファイル */
	glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);	/* プロファイル */
#endif
    glfwWindowHint(GLFW_VISIBLE, GLFW_FALSE);

    window = glfwCreateWindow(width, height, "Simple example", NULL, NULL);
    if (!window)
    {
        glfwTerminate();
        return false;
    }

    glfwMakeContextCurrent(window);

//    gladLoadGL(glfwGetProcAddress);

    if (!gladLoadGLLoader(reinterpret_cast<GLADloadproc>(glfwGetProcAddress)))
    {
        std::cerr << "Failed to initialize GLAD" << std::endl;
        return false;
    }

    isGPUInit = true;


    SpriteStudio::SsCurrentRenderer::SetCurrentRender(new SpriteStudio::SsRenderGL());
    texfactory = new SpriteStudio::SSTextureFactory(new SpriteStudio::SSTextureGL());

    glGenFramebuffers(1, &FramebufferName);
    glBindFramebuffer(GL_FRAMEBUFFER, FramebufferName);

    glGenTextures(1, &renderedTexture);
    glBindTexture(GL_TEXTURE_2D, renderedTexture);

    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, 0);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);

    glGenRenderbuffers(1, &depthrenderbuffer);
    glBindRenderbuffer(GL_RENDERBUFFER, depthrenderbuffer);
    glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH_COMPONENT, width, height);
    glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, depthrenderbuffer);

    glFramebufferTexture(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, renderedTexture, 0);


    glBindFramebuffer(GL_FRAMEBUFFER, FramebufferName);

    return true;

}



bool isOpenGLContextInitialized()
{
	return isGPUInit;
}

//
void ConverterOpenGLClear(float canvasWidth, float canvasHeight, float pivotx, float pivoty)
//void ConverterOpenGLClear(int canvasWidth , int canvasHeight)
{

	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);
	//glClear(GL_COLOR_BUFFER_BIT);
	glDisable(GL_STENCIL_TEST);
	glEnable(GL_DEPTH_TEST);
	glColorMask(GL_TRUE, GL_TRUE, GL_TRUE, GL_TRUE);

	glClearColor( 0, 0, 0, 0.0f);

	//projection setup
    int width;// = canvasWidth;	//プロジェクト枠のサイズで確定させるのが良さそう
    int height;// = canvasHeight;

    glfwGetFramebufferSize(window, &width, &height);

	glMatrixMode(GL_PROJECTION);
	glViewport(0, 0, width, height);
	glLoadIdentity();

    float camX = -pivotx * ( canvasWidth / 2 );
    float camY = -pivoty * ( canvasHeight / 2);

    float _max = canvasWidth > canvasHeight ? canvasWidth : canvasHeight;

//    glOrtho( -width / 2, width / 2, -height / 2, height / 2, -width, height);
    //PNGレンダリングなので最初から転地する
    glOrtho( camX -_max / 2.0f,
             camX + _max / 2.0f,
             camY +_max / 2.0f,
             camY -_max / 2.0f,
                -1, 1);

    glMatrixMode(GL_MODELVIEW);
}

void  ConverterOpenGLDrawEnd()
{
    glFinish();
}

void  ConverterOpenGLRelease()
{


	delete texfactory;
	texfactory = nullptr;
}

void ConverterOpenGLOutputBitMapImage(const std::string filename)
{
	char* pcBitmap;
    char* buffer;
	int width;	//プロジェクト枠のサイズで確定させるのが良さそう
	int height;

    glfwGetFramebufferSize(window, &width, &height);
    GLenum err = glGetError();

    if (err != GL_NO_ERROR)
    {
        printf("glGetError = %x \n", err);
    }

//	GLenum		eFormat = GL_RGBA_INTEGER;
    GLenum		eFormat = GL_RGBA;
    
    pcBitmap = (char*)malloc(sizeof(char) * width * height * 4);

/*
#if USE_NATIVE_OSMESA
    glfwGetOSMesaColorBuffer(window, &width, &height, NULL, (void**) &pcBitmap);
#else
    glReadPixels(0, 0, width, height, eFormat, GL_UNSIGNED_BYTE, pcBitmap);
#endif
*/
    glReadBuffer(GL_COLOR_ATTACHMENT0);
    err = glGetError();
    if (err != GL_NO_ERROR)
    {
        printf("glGetError = %x \n", err);
    }

    glReadPixels(0, 0, width, height, eFormat, GL_UNSIGNED_BYTE, pcBitmap);
    err = glGetError();
    if (err != GL_NO_ERROR)
    {
        printf("glGetError = %x \n", err);
    }

    stbi_write_png(filename.c_str(), width, height, 4, (const void*)pcBitmap, 0);


}
