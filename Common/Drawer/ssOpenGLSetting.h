#ifndef __SSOPENGLSETTING__
#define __SSOPENGLSETTING__



#define USE_GLEW (0)

#if USE_GLEW
#include <GL/GL.h>
#include <GL/glew.h>

#ifndef _WIN32
    #include <OpenGL/gl.h>
    #include <OpenGL/glext.h>
#endif

#else
    #include <glad/glad.h>
    #include <GLFW/glfw3.h>
#endif


#ifdef _WIN32
#pragma comment(lib, "OpenGL32.Lib")
#endif


#endif

