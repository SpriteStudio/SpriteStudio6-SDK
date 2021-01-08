#include "BackGroudRender.h"
#include "GL/glew.h"
#include <GL/GL.h>
#include <string>
#include <iostream>
#include <Windows.h>


#include "ssplayer_render.h"
#include "ssplayer_render_gl.h"
#include "OpenGL/SSTextureGL.h"

#define STB_IMAGE_WRITE_IMPLEMENTATION
#include "stb_image.h"
#include "stb_image_write.h"

static spritestudio6::SSTextureFactory* texfactory = nullptr;
static HGLRC context = nullptr;
static HDC deviceContext;
GLuint FramebufferName = 0;
GLuint renderedTexture;
GLuint depthrenderbuffer;


bool ConverterOpenGLInit()
{

#if WIN32
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


	return true;
#else
	return false;
#endif
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
	SwapBuffers(deviceContext);
}

void  ConverterOpenGLRelease()
{
	wglDeleteContext(context);
}

void ConverterOpenGLOutputBitMapImage(const std::string filename)
{
	char* pcBitmap;

	int width = 1024;	//プロジェクト枠のサイズで確定させるのが良さそう
	int height = 1024;

#ifdef	_WIN32
	GLenum		eFormat = GL_BGRA;
#else
	GLenum		eFormat = GL_RGBA;
#endif


	pcBitmap = (char*)malloc(sizeof(char) * width * height * 4);

	glReadBuffer(GL_COLOR_ATTACHMENT0);
	glReadPixels(0, 0, width, height, eFormat, GL_UNSIGNED_BYTE, pcBitmap);

	stbi_write_png(filename.c_str(), width, height, 4 , (const void*)pcBitmap, 0);


}