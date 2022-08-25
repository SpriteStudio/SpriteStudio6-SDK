#include "ssOpenGLSetting.h"

void initOpenGL()
{
#ifdef USE_GLEW
#if _WIN32
	// TODO: comment out for JUCE 6.1 and more
	GLenum err = glewInit();
#endif
#else
	if (!glfwInit())
		exit(EXIT_FAILURE);
	//juce::OpenGLShaderProgram
	if (!gladLoadGLLoader(reinterpret_cast<GLADloadproc>(glfwGetProcAddress)))
	{
		std::cerr << "Failed to initialize GLAD" << std::endl;
		return;
	}
#endif
}