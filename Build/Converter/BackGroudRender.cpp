#include "BackGroudRender.h"
#include "GL/glew.h"
#include <GL/GL.h>
#include <string>
#include <iostream>
#include <Windows.h>


bool OpenGLInit()
{
//	auto window = CreateWindow(TEXT("STATIC"), 0, WS_OVERLAPPEDWINDOW | WS_VISIBLE, 0, 0, 0, 0, 0, 0, 0, 0);
	auto window = CreateWindow(TEXT("STATIC"), 0, WS_POPUP , 0, 0, 0, 0, 0, 0, 0, 0);

	auto deviceContext = GetDC(window);

	const PIXELFORMATDESCRIPTOR pixelFormatDesc =
	{
		0, 1, PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER, 32, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 32, 0, 0, 0, 0, 0, 0, 0
	};
	SetPixelFormat(deviceContext, ChoosePixelFormat(deviceContext, &pixelFormatDesc), &pixelFormatDesc);
	auto context = wglCreateContext(deviceContext);
	wglMakeCurrent(deviceContext, context);

	GLenum err = glewInit();
	if (err == GLEW_OK) {
		return true;
	}
	else {
		std::cout << glewGetErrorString(err) << std::endl;

		return false;
	}


}
