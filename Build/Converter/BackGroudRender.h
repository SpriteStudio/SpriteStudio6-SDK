#ifndef __BACKGROUNDRENDER__
#define __BACKGROUNDRENDER__

#include <string>

bool ConverterOpenGLInit();
void ConverterOpenGLClear();
void ConverterOpenGLDrawEnd();
void ConverterOpenGLRelease();
void ConverterOpenGLOutputBitMapImage(const std::string filename);


#endif

