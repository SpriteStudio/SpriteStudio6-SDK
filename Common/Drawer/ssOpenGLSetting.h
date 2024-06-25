#ifndef __SSOPENGLSETTING__
#define __SSOPENGLSETTING__

#ifdef USE_GLEW
#if _WIN32 || _WIN64
#define GLEW_STATIC
#include <GL/gl.h>
#include <GL/glew.h>
#elif __APPLE__
#include <OpenGL/OpenGL.h>
#include <OpenGL/gl.h>
#include <OpenGL/gl3.h>
#include <OpenGL/glext.h>
#else
#include <GL/gl.h>
#endif
#else
#include <glad/glad.h>
// #include <glad/gl.h>
#include <GLFW/glfw3.h>
#endif

#ifdef _WIN32
#pragma comment(lib, "OpenGL32.Lib")
#endif

#endif
