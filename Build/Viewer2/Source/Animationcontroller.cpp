/*
  ==============================================================================

    renderAnimation.cpp
    Created: 26 Jan 2018 5:46:29pm
    Author:  kurooka

  ==============================================================================
*/
#ifndef _WIN32
#include <OpenGL/gl.h>
#include <OpenGL/glu.h>
#include <OpenGL/glext.h>
#else
#pragma warning( disable : 4819 )  
#include <GL/glew.h>
#include <GL/GL.h>
#pragma comment(lib,"glew32.lib")
#endif

#include "AnimationController.h"
#include "ssplayer_animedecode.h"

#include "ssHelper.h"

#include "OpenGL/SSTextureGL.h"


#include "ssplayer_render_gl.h"
#include "ssplayer_shader_gl.h"


#include "MainComponent.h"


SSAnimationController*	SSAnimationController::myInst = 0;

SSAnimationController::~SSAnimationController()
{
	if (m_player)
		delete m_player;

	if (m_cellmap)
		delete m_cellmap; //OpenGLスレッドで遅延実行しないとだめ

	m_player = 0;
	m_cellmap = 0;
	stopTimer();

}

SSAnimationController::SSAnimationController()
	: m_player(0) , m_cellmap(0), isReset(false)
{

	m_player = new SsAnimeDecoder();
	m_cellmap = new SsCellMapList();
	myInst = this;

	currentFrame = 0;
	stopTimer();


}
void	SSAnimationController::create()
{
	if (myInst == 0)
	{
		new SSAnimationController();
	}
}
void	SSAnimationController::destry()
{
	if (myInst) {
		delete myInst;
		myInst = 0;
	}
}
void	SSAnimationController::draw()
{
	if (myInst)
	{
		myInst->drawMain();
	}
}


void	SSAnimationController::stop()
{
	stopTimer();
}

void	SSAnimationController::play()
{
	startTimer( (int)(1000.0f / 60.0f) );
	currentFrame = 0;
}

void	SSAnimationController::reset()
{
	stop();
	currentFrame = 0;
}

void SSAnimationController::hiResTimerCallback()
{
	currentFrame += 1;

	if (m_player)
	{
		if (m_player->getAnimeEndFrame() < currentFrame)
		{
			currentFrame = 0;
		}
	}
}


void	SSAnimationController::drawMain()
{

	if (!SsCurrentRenderer::getRender())
	{
		GLenum err = glewInit();

		SsCurrentRenderer::SetCurrentRender(new SsRenderGL());
		SSTextureFactory*	texfactory = new SSTextureFactory(new SSTextureGL());
	}

	if (isReset)
	{
		m_cellmap->set(currentProj, currentAnimePack);

		if (m_cellmap->size() == 0)
		{
			DEBUG_PRINTF("error : cellmap array size == 0");
		}

	}
	if (m_player)
	{
		m_player->setPlayFrame(currentFrame);
		m_player->update(1);
		m_player->draw();
	}
}

void	SSAnimationController::loadListener(SsProject* pj)
{

	int packIndex = 0;
	int animeIndex = 0;

	if (pj->getAnimePackList().size() == 0) return;

	SsAnimePackList alist = pj->getAnimePackList();

	SsAnimePack* animepack = alist[packIndex];
	
	SsModel* model = &animepack->Model;

	SsAnimation* anime = animepack->animeList[animeIndex];

	currentProj = pj;
	currentAnimePack = animepack;
	isReset = true;

	m_player->setAnimation(model, anime, m_cellmap, pj);
	m_player->setPlayFrame(0);
	play();


	//ビュー系のアップデートを登録できるように修正する予定
	//MainContentComponent::getApp()->updateViews();


}


