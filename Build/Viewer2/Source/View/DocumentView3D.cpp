#include <GL/glew.h>
#include "OpenGL/SSTextureGL.h"
#include "ssplayer_render_gl.h"
#include "ssplayer_shader_gl.h"
#include "Controller/MainComponent.h"
#include "Model/Player.h"
#include "View/DocumentView3D.h"
#include "View/MainWindow.h"

DocumentView3D::DocumentView3D()
{
	setSize(800, 600);
	backGroundColour = Colours::black;
}

DocumentView3D::~DocumentView3D()
{
	shutdownOpenGL();
}

void DocumentView3D::initialise()
{
#if JUCE_WINDOWS
	GLenum err = glewInit();
#endif

	rendererGL.reset(new SsRenderGL());
	SsCurrentRenderer::SetCurrentRender(rendererGL.get());
	texfactory.reset(new SSTextureFactory(new SSTextureGL()));
}

void DocumentView3D::shutdown()
{
	SSTextureFactory::releaseAllTexture();
	SSOpenGLShaderMan::Destory();
}

void DocumentView3D::render()
{
	if (!this->openGLContext.isActive())return;

	Rectangle<int> area(getLocalBounds());

	//ビューポートの設定
	int width = area.getWidth();
	int height = area.getHeight();

	//表示のクリア
	OpenGLHelpers::clear(backGroundColour);

	glMatrixMode(GL_PROJECTION);
	glViewport(0, 0, width, height);
	glLoadIdentity();
	glOrtho(-width / 2, width / 2, -height / 2, height / 2, -2048, 2048);

	//レンダーステート設定(ループ初期化）
	glDisable(GL_STENCIL_TEST);
	glEnable(GL_DEPTH_TEST);
	glColorMask(GL_TRUE, GL_TRUE, GL_TRUE, GL_TRUE);

	//描画
	Player::drawAnime();
}

void DocumentView3D::paint(Graphics&)
{
	// You can add your component specific drawing code here!
	// This will draw over the top of the openGL background.
	//OpenGLAfterRender(g);
}

void DocumentView3D::resized()
{
	// This is called when the MainContentComponent is resized.
	// If you add any child components, this is where you should
	// update their positions.
}

void DocumentView3D::setBackGroundColour(const Colour & colour)
{
	backGroundColour = colour;
}
