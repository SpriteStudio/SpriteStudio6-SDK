/*
  ==============================================================================

    renderAnimation.cpp
    Created: 26 Jan 2018 5:46:29pm
    Author:  kurooka

  ==============================================================================
*/
#define GLEW_STATIC
#include <GL/glew.h>

#include "ssHelper.h"
#include "ssplayer_animedecode.h"
#include "OpenGL/SSTextureGL.h"
#include "ssplayer_render_gl.h"
#include "ssplayer_shader_gl.h"

#include "Controller/MainComponent.h"
#include "Model/Player.h"
#include "View/DocumentView3D.h"
#include "View/MainWindow.h"
#include <algorithm>
#include "babel/babel.h"

Player*	Player::myInst = 0;
Value	Player::State::packIndex;
Value	Player::State::animeIndex;
Value	Player::State::animeName;
Value	Player::State::startFrame;
Value	Player::State::endFrame;
Value	Player::State::length;
Value	Player::State::frame;
Value	Player::State::fps;
Value	Player::State::loop;

Player::~Player()
{
	stopTimer();
	myInst = nullptr;
}

Player * Player::get()
{
	if (myInst)
	{
		return myInst;
	}
	else
	{
		return myInst = new Player();
	}
}

Player::Player()
{
	myInst = this;

	currentState->packIndex		= 0;
	currentState->animeIndex	= 0;
	currentState->animeName		= "";
	currentState->startFrame	= 0;
	currentState->endFrame		= 10;
	currentState->length		= 10;
	currentState->frame			= 0;
	currentState->fps			= 30;
	currentState->loop			= false;

	auto * view = ViewerMainWindow::get();
	currentState->animeName.addListener(view);
	currentState->startFrame.addListener(view);
	currentState->endFrame.addListener(view);
	currentState->length.addListener(view);
	currentState->frame.addListener(view);
	currentState->fps.addListener(view);
	currentState->loop.addListener(view);

	requestSetProj = new RequestSetProj();
	requestSetAnime = new RequestSetAnime();

	statePlaying = new StatePlaying();
	statePaused = new StatePaused();
	stateLoading = new StateLoading();
	stateInitial = new StateInitial();

	currentState = stateInitial;

	stopTimer();
}

void	Player::stop()
{
	currentState->stop(this);
}

void	Player::play()
{
	currentState->start(this);
}

void	Player::reset()
{
	currentState->reset(this);
}

void Player::tick()
{
	int		frame		= currentState->frame.getValue();
	int		startFrame	= currentState->startFrame.getValue();
	int		endFrame	= currentState->endFrame.getValue();
	bool	loop		= currentState->loop.getValue();

	frame++;

	if (endFrame < frame)
	{
		if (loop)
		{
			frame = startFrame;
		}
		else
		{
			stop();
			frame = endFrame;
		}
	}

	currentState->frame = frame;
}

void Player::hiResTimerCallback()
{
	currentState->hiResTimerCallback(this);
}

void Player::loadProj(const String & name)
{
	currentState->loadProj(this, name);
}

void Player::setProj()
{
}

void Player::loadAnime(int packIndex, int animeIndex)
{
	currentState->loadAnime(this, packIndex, animeIndex);
}

void Player::setAnime()
{
	cellmap->set(currentProj, animePack);// この関数はGLのスレッドでやらないとだめ!!

	SsModel* model = &animePack->Model;
	SsAnimation * anime = animePack->animeList[(int)currentState->animeIndex.getValue()];

	decoder->setAnimation(model, anime, cellmap, currentProj);// この関数はGLのスレッドでやらないとだめ!!

	int startFrame	= static_cast<int>(decoder->getAnimeStartFrame());
	int endFrame	= static_cast<int>(decoder->getAnimeEndFrame());
	int fps			= static_cast<int>(decoder->getAnimeFPS());
	currentState->length		= endFrame;
	currentState->startFrame	= startFrame;
	currentState->frame			= startFrame;
	currentState->endFrame		= endFrame;
	currentState->fps			= fps;
}

void Player::initGL()
{
	decoder = new SsAnimeDecoder();
	cellmap = new SsCellMapList();

#if JUCE_WINDOWS
	GLenum err = glewInit();
#endif

	SsCurrentRenderer::SetCurrentRender(new SsRenderGL());
	texfactory = new SSTextureFactory(new SSTextureGL());
}

void Player::putRequest(IRequest * request)
{
	requestQueue.push(request);
}

std::queue<IRequest*>* Player::getRequestQueue()
{
	return &requestQueue;
}

Player::State * Player::getState()
{
	return currentState;
}

void Player::drawAnime()
{
	Player::get()->draw();
}

void Player::draw()
{
	currentState->draw(this);
}

void Player::changeState(State * newState)
{
	//currentState->onLeave(this);
	currentState = newState;
	//currentState->onEnter(this);
}

//--------------------------------------------------------------------
//
// デフォルトの実装
//
//--------------------------------------------------------------------
void Player::State::start(Player * p)
{
	changeState(p, p->statePlaying);

	float fps = p->currentState->fps.getValue();
	p->startTimer((int)(1000.0f / std::max(fps, 1.0f)));
}

void Player::State::stop(Player * p)
{
	p->stopTimer();

	changeState(p, p->statePaused);
}

void Player::State::reset(Player * p)
{
	p->stopTimer();
	p->currentState->frame = (int)p->currentState->startFrame.getValue();

	changeState(p, p->statePaused);
}

void Player::State::hiResTimerCallback(Player * p)
{
	p->tick();
}

void Player::State::draw(Player * p)
{
	if (p->decoder)
	{
		p->decoder->setPlayFrame((int)p->currentState->frame.getValue());
		p->decoder->update(1);
		p->decoder->draw();
	}
}

void Player::State::loadProj(Player * p, const String & name)
{
	changeState(p, p->stateLoading);

	// 文字コード変換
	std::string fileName = babel::auto_translate<>(name.toStdString());
	
	auto * proj = ssloader_sspj::Load(fileName);

	if (!proj)
	{
		AlertWindow::showMessageBoxAsync(
			AlertWindow::InfoIcon,
			"File Open Failed",
			"File Open Failed : " + name);

		changeState(p, p->stateInitial);
		return;
	}

	if (proj->getAnimePackList().size() == 0)
	{
		return;
	}

	// 最近開いたファイルリストに追加
	ViewerMainWindow::get()->addRecentlyOpenedFile(File(fileName));

	p->currentProj = proj;

	// GLのスレッドにリクエスト
	p->putRequest(p->requestSetProj);

	changeState(p, p->stateInitial);
}

void Player::State::loadAnime(Player * p, int packIndex, int animeIndex)
{
	changeState(p, p->stateLoading);

	p->currentState->packIndex = packIndex;
	p->currentState->animeIndex = animeIndex;

	SsAnimePackList alist = p->currentProj->getAnimePackList();
	//アニメパックを選択
	p->animePack = alist[(int)p->currentState->packIndex.getValue()];
	SsAnimation * anime = p->animePack->animeList[(int)p->currentState->animeIndex.getValue()];

	p->currentState->animeName	= anime->name.c_str();

	// GLのスレッドにリクエスト
	p->putRequest(p->requestSetAnime);

	changeState(p, p->statePaused);
}

void Player::State::changeState(Player * p, State * newState)
{
	p->currentState = newState;
}

void RequestSetProj::execute()
{
	Player::get()->setProj();
}

void RequestSetAnime::execute()
{
	Player::get()->setAnime();
}
