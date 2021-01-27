#include "BackGroudRender.h"
#include <string>
#include <iostream>


#ifdef _WIN32

//#include <GL/GL.h>


#include <Windows.h>
#include <glad/glad.h>
#include <GLFW/glfw3.h>

//#include <GL/GL.h>
//#include <GL/glew.h>

#if USE_NATIVE_OSMESA
#define GLFW_EXPOSE_NATIVE_OSMESA
#include <GLFW/glfw3native.h>
#endif

//#include <GL/glew.h>
//#include <GL/GL.h>
//#pragma comment(lib, "OpenGL32.Lib")
#else

/*
#include <OpenGL/gl.h>
#include <OpenGL/glu.h>
#include <OpenGL/glext.h>
*/

//#include <GL/glew.h>
#include <glad/glad.h>
//#define GLFW_INCLUDE_NONE
#include <GLFW/glfw3.h>

#if USE_NATIVE_OSMESA
 #define GLFW_EXPOSE_NATIVE_OSMESA
 #include <GLFW/glfw3native.h>
#endif



#endif


#include "ssplayer_render.h"
#include "ssplayer_render_gl.h"
#include "OpenGL/SSTextureGL.h"

#define STB_IMAGE_WRITE_IMPLEMENTATION
#include "stb_image.h"
#include "stb_image_write.h"



static spritestudio6::SSTextureFactory* texfactory = nullptr;

#if WIN32
static HDC deviceContext;
static HGLRC context = nullptr;
#endif

GLuint FramebufferName = 0;
GLuint renderedTexture;
GLuint depthrenderbuffer;


bool ConverterOpenGLInit()
{

#if WIN32
/*
//	auto window = CreateWindow(TEXT("STATIC"), TEXT("Ss6Converter"), WS_CAPTION | WS_VISIBLE, 0, 0, 1024, 1024, 0, 0, 0, 0);
	auto window = CreateWindow(TEXT("STATIC"), TEXT("Ss6Converter"), WS_CAPTION , 0, 0, 1024, 1024, 0, 0, 0, 0);

	deviceContext = GetDC(window);

	const PIXELFORMATDESCRIPTOR pixelFormatDesc =
	{
		0, 1, PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER, 32, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 32, 0, 0, 0, 0, 0, 0, 0
	};
	SetPixelFormat(deviceContext, ChoosePixelFormat(deviceContext, &pixelFormatDesc), &pixelFormatDesc);
	context = wglCreateContext(deviceContext);
	wglMakeCurrent(deviceContext, context);

	GLenum err = glewInit();
	if (err != GLEW_OK) {
		std::cout << glewGetErrorString(err) << std::endl;
		ConverterOpenGLRelease();
		context = nullptr;
		return false;
	}

	spritestudio6::SsCurrentRenderer::SetCurrentRender(new spritestudio6::SsRenderGL());
	texfactory = new spritestudio6::SSTextureFactory(new spritestudio6::SSTextureGL());


	//レンダリングテクスチャのバインド
	int renderWidth = 1024;
	int renderHeight = 1024;

	SendMessage(window, WM_PAINT, 0, 0);

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

	//GLenum DrawBuffers[1] = { GL_COLOR_ATTACHMENT0 };
	//glDrawBuffers(1, DrawBuffers); 

	glBindFramebuffer(GL_FRAMEBUFFER, FramebufferName);
    */

	return true;
#else


	return false;
#endif
}

static void error_callback(int error, const char* description)
{
    fprintf(stderr, "Error: %s\n", description);
}

bool isOpenGLContextInitialized()
{

#if WIN32

	if (context == nullptr) return false;
#else
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

    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 2);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 0);
    glfwWindowHint(GLFW_VISIBLE, GLFW_FALSE);

    window = glfwCreateWindow(640, 480, "Simple example", NULL, NULL);
    if (!window)
    {
        glfwTerminate();
        exit(EXIT_FAILURE);
    }

    glfwMakeContextCurrent(window);
    
    //gladLoadGL(glfwGetProcAddress());

    if (!gladLoadGLLoader(reinterpret_cast<GLADloadproc>(glfwGetProcAddress)))
    {
        std::cerr << "Failed to initialize GLAD" << std::endl;
        return -1;
    }    


#if 0
    // NOTE: OpenGL error checks have been omitted for brevity

    glGenBuffers(1, &vertex_buffer);
    glBindBuffer(GL_ARRAY_BUFFER, vertex_buffer);
    glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

    vertex_shader = glCreateShader(GL_VERTEX_SHADER);
    glShaderSource(vertex_shader, 1, &vertex_shader_text, NULL);
    glCompileShader(vertex_shader);

    fragment_shader = glCreateShader(GL_FRAGMENT_SHADER);
    glShaderSource(fragment_shader, 1, &fragment_shader_text, NULL);
    glCompileShader(fragment_shader);

    program = glCreateProgram();
    glAttachShader(program, vertex_shader);
    glAttachShader(program, fragment_shader);
    glLinkProgram(program);

    mvp_location = glGetUniformLocation(program, "MVP");
    vpos_location = glGetAttribLocation(program, "vPos");
    vcol_location = glGetAttribLocation(program, "vCol");

    glEnableVertexAttribArray(vpos_location);
    glVertexAttribPointer(vpos_location, 2, GL_FLOAT, GL_FALSE,
                          sizeof(vertices[0]), (void*) 0);
    glEnableVertexAttribArray(vcol_location);
    glVertexAttribPointer(vcol_location, 3, GL_FLOAT, GL_FALSE,
                          sizeof(vertices[0]), (void*) (sizeof(float) * 2));

    glfwGetFramebufferSize(window, &width, &height);
    ratio = width / (float) height;

    glViewport(0, 0, width, height);
    glClear(GL_COLOR_BUFFER_BIT);

    mat4x4_ortho(mvp, -ratio, ratio, -1.f, 1.f, 1.f, -1.f);

    glUseProgram(program);
    glUniformMatrix4fv(mvp_location, 1, GL_FALSE, (const GLfloat*) mvp);
    glDrawArrays(GL_TRIANGLES, 0, 3);
    glFinish();

#if USE_NATIVE_OSMESA
    glfwGetOSMesaColorBuffer(window, &width, &height, NULL, (void**) &buffer);
#else
    buffer = calloc(4, width * height);
    glReadPixels(0, 0, width, height, GL_RGBA, GL_UNSIGNED_BYTE, buffer);
#endif

    // Write image Y-flipped because OpenGL
    stbi_write_png("offscreen.png",
                   width, height, 4,
                   buffer + (width * 4 * (height - 1)),
                   -width * 4);

#if USE_NATIVE_OSMESA
    // Here is where there's nothing
#else
    free(buffer);
#endif

    glfwDestroyWindow(window);

    glfwTerminate();
#endif
#endif

	return true;
}


void ConverterOpenGLClear()
{

#if WIN32

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


#endif

}

void  ConverterOpenGLDrawEnd()
{
#if WIN32
	SwapBuffers(deviceContext);
#else
#endif
}

void  ConverterOpenGLRelease()
{
#if WIN32
	wglDeleteContext(context);
#else
#endif
	delete texfactory;
	texfactory = nullptr;
}

void ConverterOpenGLOutputBitMapImage(const std::string filename)
{
	char* pcBitmap;

	int width = 1024;	//プロジェクト枠のサイズで確定させるのが良さそう
	int height = 1024;

#ifdef	_WIN32
/*
	GLenum		eFormat = GL_BGRA;
	pcBitmap = (char*)malloc(sizeof(char) * width * height * 4);

	glReadBuffer(GL_COLOR_ATTACHMENT0);
	glReadPixels(0, 0, width, height, eFormat, GL_UNSIGNED_BYTE, pcBitmap);

	stbi_write_png(filename.c_str(), width, height, 4 , (const void*)pcBitmap, 0);
*/
#else
	GLenum		eFormat = GL_RGBA;
#endif




}