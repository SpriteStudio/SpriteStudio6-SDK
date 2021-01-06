#include "BackGroudRender.h"
#include "GL/glew.h"
#include <GL/GL.h>
#include <string>
#include <iostream>
#include <Windows.h>


#include "ssplayer_render.h"
#include "ssplayer_render_gl.h"
#include "OpenGL/SSTextureGL.h"


static spritestudio6::SSTextureFactory* texfactory = nullptr;
static HGLRC context = nullptr;
static HDC deviceContext;


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


	SendMessage(window, WM_PAINT, 0, 0);
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

