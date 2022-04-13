#include "View/DocumentView3D.h"
using namespace juce::gl;

#include "ssOpenGLSetting.h"
#include "OpenGL/SSTextureGL.h"
#include "ssplayer_render_gl.h"
#include "ssplayer_shader_gl.h"
#include "Controller/MainComponent.h"
#include "Model/Player.h"
#include "View/MainWindow.h"

DocumentView3D::DocumentView3D()
{
	setSize(800, 600);
	backGroundColour = Colours::black;

	view_camera_x = 0.0f;
	view_camera_y = 0.0f;
	view_camera_scale = 1.0f;


	auto v = ViewerMainWindow::get();
	view_camera_x.addListener(v);
	view_camera_y.addListener(v);
	view_camera_scale.addListener(v);


}

DocumentView3D::~DocumentView3D()
{
	shutdownOpenGL();
}

void DocumentView3D::initialise()
{
#if JUCE_WINDOWS
        // TODO: comment out for JUCE 6.1 and more
	// GLenum err = glewInit();
#endif
	
/*
	if (!glfwInit())
		exit(EXIT_FAILURE);

	if (!gladLoadGLLoader(reinterpret_cast<GLADloadproc>(glfwGetProcAddress)))
	{
		std::cerr << "Failed to initialize GLAD" << std::endl;
		return ;
	}
*/
	rendererGL.reset(new spritestudio6::SsRenderGL());
	spritestudio6::SsCurrentRenderer::SetCurrentRender(rendererGL.get());
	texfactory.reset(new spritestudio6::SSTextureFactory(new spritestudio6::SSTextureGL()));
}

void DocumentView3D::shutdown()
{
	spritestudio6::SSTextureFactory::releaseAllTexture();
	spritestudio6::SSOpenGLShaderMan::Destory();
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

	float x = view_camera_x.getValue();
	float y = view_camera_y.getValue();
	float scale = view_camera_scale.getValue();

    glTranslatef( x , y, 0);
    glScalef(scale, scale, 0);
//	glTranslatef(view_camera_xf, view_camera_yf, 0);

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


void DocumentView3D::mouseDown(const MouseEvent& event)
{
	view_camera_org_x = view_camera_x.getValue();
	view_camera_org_y = view_camera_y.getValue();


}

void DocumentView3D::mouseMove(const MouseEvent& event)
{

}

void DocumentView3D::mouseUp(const MouseEvent& event)
{
}

void DocumentView3D::mouseDrag(const MouseEvent& event)
{

	view_camera_x = view_camera_org_x - ( event.mouseDownPosition.x - event.x ) ;
	view_camera_y = view_camera_org_y + ( event.mouseDownPosition.y - event.y ) ;

}

void DocumentView3D::mouseWheelMove(const MouseEvent& event, const MouseWheelDetails& wheel)
{

	//DBG(wheel.deltaY);

	if ( (float)view_camera_scale.getValue() > 0.0f && wheel.deltaY < -0.1f )
	{
		view_camera_scale = (float)view_camera_scale.getValue() - 0.1f;
	}

	if ((float)view_camera_scale.getValue() < 5.0f && wheel.deltaY > 0.1f)
	{
		view_camera_scale = (float)view_camera_scale.getValue() + 0.1f;
	}

}

