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



static spritestudio6::SSTextureFactory* texfactory = nullptr;


GLuint FramebufferName = 0;
GLuint renderedTexture;
GLuint depthrenderbuffer;



static void error_callback(int error, const char* description)
{
    fprintf(stderr, "Error: %s\n", description);
}

bool ConverterOpenGLInit()
{

    GLFWwindow* window;
    GLuint vertex_buffer, vertex_shader, fragment_shader, program;
    GLint mvp_location, vpos_location, vcol_location;
    float ratio;
    int width, height;

    //    mat4x4 mvp;
    char* buffer;

    glfwSetErrorCallback(error_callback);

    glfwInitHint(GLFW_COCOA_MENUBAR, GLFW_FALSE);

    if (!glfwInit())
        exit(EXIT_FAILURE);

    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 2);
    glfwWindowHint(GLFW_VISIBLE, GLFW_FALSE);

    window = glfwCreateWindow(640, 480, "Simple example", NULL, NULL);
    if (!window)
    {
        glfwTerminate();
        exit(EXIT_FAILURE);
    }

    glfwMakeContextCurrent(window);
    gladLoadGL(glfwGetProcAddress);
    //gladLoadGL(glXGetProcAddressARB());

/*
    if (!gladLoadGLLoader(reinterpret_cast<GLADloadproc>(glfwGetProcAddress)))
    {
        std::cerr << "Failed to initialize GLAD" << std::endl;
        return -1;
    }
*/
    spritestudio6::SsCurrentRenderer::SetCurrentRender(new spritestudio6::SsRenderGL());
    texfactory = new spritestudio6::SSTextureFactory(new spritestudio6::SSTextureGL());

    //レンダリングテクスチャのバインド
    int renderWidth = 1024;
    int renderHeight = 1024;

    //SendMessage(window, WM_PAINT, 0, 0);

    glGenFramebuffers(1, &FramebufferName);
    glBindFramebuffer(GL_FRAMEBUFFER, FramebufferName);

    glGenTextures(1, &renderedTexture);
    glBindTexture(GL_TEXTURE_2D, renderedTexture);

    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, renderWidth, renderHeight, 0, GL_RGB, GL_UNSIGNED_BYTE, 0);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);

    glGenRenderbuffers(1, &depthrenderbuffer);
    glBindRenderbuffer(GL_RENDERBUFFER, depthrenderbuffer);
    glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH_COMPONENT, renderWidth, renderHeight);
    glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, depthrenderbuffer);

    glFramebufferTexture(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, renderedTexture, 0);


    glBindFramebuffer(GL_FRAMEBUFFER, FramebufferName);


    return true;

}



bool isOpenGLContextInitialized()
{
	return true;
}


void ConverterOpenGLClear()
{

	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);
	//glClear(GL_COLOR_BUFFER_BIT);
	glDisable(GL_STENCIL_TEST);
	glEnable(GL_DEPTH_TEST);
	glColorMask(GL_TRUE, GL_TRUE, GL_TRUE, GL_TRUE);

	glClearColor( 0, 0, 0, 0.0f);

	//projection setup
	int width = 1024;	//プロジェクト枠のサイズで確定させるのが良さそう
	int height = 1024;

	glMatrixMode(GL_PROJECTION);
	glViewport(0, 0, width, height);
	glLoadIdentity();
	glOrtho(-width / 2, width / 2, -height / 2, height / 2, -2048, 2048);



}

void  ConverterOpenGLDrawEnd()
{
    glFinish();
	//SwapBuffers(deviceContext);

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
	int width = 1024;	//プロジェクト枠のサイズで確定させるのが良さそう
	int height = 1024;

	GLenum		eFormat = GL_RGBA_INTEGER;

    pcBitmap = (char*)malloc(sizeof(char) * width * height * 4);
/*
#if USE_NATIVE_OSMESA
    glfwGetOSMesaColorBuffer(window, &width, &height, NULL, (void**) &pcBitmap);
#else
    glReadPixels(0, 0, width, height, GL_RGBA, GL_UNSIGNED_BYTE, pcBitmap);
#endif
*/
    glReadBuffer(GL_COLOR_ATTACHMENT0);
    glReadPixels(0, 0, width, height, eFormat, GL_UNSIGNED_BYTE, pcBitmap);
    GLenum err = glGetError();
    
    printf("glGetError = %x¥n",err);
    
    stbi_write_png(filename.c_str(), width, height, 4, (const void*)pcBitmap, 0);



}
