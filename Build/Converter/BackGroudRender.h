#ifndef __BACKGROUNDRENDER__
#define __BACKGROUNDRENDER__

#include <string>

bool ConverterOpenGLInit();
bool isOpenGLContextInitialized();

//void ConverterOpenGLClear();
void ConverterOpenGLClear(float canvasWidth, float canvasHeight , float pivotx , float pivoty );

void ConverterOpenGLDrawEnd();
void ConverterOpenGLRelease();
void ConverterOpenGLOutputBitMapImage(const std::string filename);


#endif

