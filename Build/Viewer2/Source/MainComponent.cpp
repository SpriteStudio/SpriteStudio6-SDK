/*
  ==============================================================================

    This file was auto-generated!

  ==============================================================================
*/

#include "MainComponent.h"
#include "Animationcontroller.h"

//==============================================================================
MainContentComponent::MainContentComponent()
{
	createSsDataHolder();
	SSAnimationController::create();

	//日本語への対応
#if JUCE_MAC || JUCE_WINDOWS
	getLookAndFeel().setDefaultSansSerifTypefaceName("Meiryo UI");
#endif

    	//メニューバーの作成
	addAndMakeVisible(menuBar = new MenuBarComponent(this));
	addAndMakeVisible(opengl = new DocumentView3D());

	

	setSize(1200, 800); //てきとう
}

MainContentComponent::~MainContentComponent()
{
	destroySsDataHolder();
	SSAnimationController::destry();

}

void MainContentComponent::paint (Graphics& g)
{
}

void MainContentComponent::resized()
{
	Rectangle<int> area(getLocalBounds());

	if (menuBar)
	{
		menuBar->setBounds(area.removeFromTop(LookAndFeel::getDefaultLookAndFeel().getDefaultMenuBarHeight()));
	}
	if (opengl)
	{
		opengl->setBounds(area);
	}

    // This is called when the MainContentComponent is resized.
    // If you add any child components, this is where you should
    // update their positions.
}



//-----------------------------------------------------------

//----------------------------------------------------------

DocumentView3D::DocumentView3D()
{

	//普通に子として配置したコンポーネントは通常通り描画されるらしい
//	addAndMakeVisible(commandPanel = new DocumentView3DCommandPanel());

	setSize(800, 600);
}

DocumentView3D::~DocumentView3D()
{
	shutdownOpenGL();
}

void DocumentView3D::initialise()
{
}

void DocumentView3D::shutdown()
{
}

void DocumentView3D::render()
{
	if (!this->openGLContext.isActive())return;

	Rectangle<int> area(getLocalBounds());

	//表示のクリア
	OpenGLHelpers::clear(Colours::black);

	//ビューポートの設定
	int width = area.getWidth();
	int height = area.getHeight();

	//int fbwidth = width;
	//int fbheight = fbheight;

	glMatrixMode(GL_PROJECTION);
	glViewport(0, 0, width, height);
	glLoadIdentity();
	glOrtho(-width / 2, width / 2, -height / 2, height / 2, -2048, 2048);


	//レンダーステート設定(ループ初期化）
	glDisable(GL_STENCIL_TEST);
	glEnable(GL_DEPTH_TEST);
	glColorMask(GL_TRUE, GL_TRUE, GL_TRUE, GL_TRUE);

	//描画
	SSAnimationController::draw();


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
	Rectangle<int> area(getLocalBounds());

	//パネルの配置とりあえず左下はじ



}