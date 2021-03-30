#ifndef __SSOPENGLSETTING__
#define __SSOPENGLSETTING__




#ifdef USE_GLEW
//#include <GL/GL.h>
    #define GLEW_STATIC
    #include <GL/glew.h>

    #ifndef _WIN32
        #include <OpenGL/gl.h>
        #include <OpenGL/glext.h>
    #endif
//    #include <GL/gl.h>
#else
    #include <glad/glad.h>
    //#include <glad/gl.h>
    #include <GLFW/glfw3.h>
#endif


#ifdef _WIN32
#pragma comment(lib, "OpenGL32.Lib")
#endif


#endif

