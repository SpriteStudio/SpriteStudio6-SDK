/*
  ==============================================================================

    renderAnimation.cpp
    Created: 26 Jan 2018 5:46:29pm
    Author:  kurooka

  ==============================================================================
*/
#include "ssHelper.h"
#include "ssplayer_animedecode.h"
#include "Controller/MainComponent.h"
#include "Model/Player.h"
#include "View/DocumentView3D.h"
#include "View/MainWindow.h"
#include "Loader.h"
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

	statePlaying.reset(new StatePlaying());
	statePaused.reset(new StatePaused());
	stateLoading.reset(new StateLoading());
	stateInitial.reset(new StateInitial());

	currentState = stateInitial.get();

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

void Player::loadAnime(int packIndex, int animeIndex)
{
	currentState->loadAnime(this, packIndex, animeIndex);
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
	p->changeState(p->statePlaying.get());

	float fps = p->currentState->fps.getValue();
	p->startTimer((int)(1000.0f / std::max(fps, 1.0f)));
}

void Player::State::stop(Player * p)
{
	p->stopTimer();

	p->changeState(p->statePaused.get());
}

void Player::State::reset(Player * p)
{
	p->stopTimer();
	p->currentState->frame = (int)p->currentState->startFrame.getValue();

	p->changeState(p->statePaused.get());
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
	auto* projectLoader = new AsyncProjectLoader();
	projectLoader->setProjectName(name);
	projectLoader->launchThread();
}

void Player::State::loadAnime(Player * p, int packIndex, int animeIndex)
{
	auto* asyncAnimeLoader = new AsyncAnimeLoader();
	asyncAnimeLoader->setAnimeIndex(packIndex, animeIndex);
	asyncAnimeLoader->launchThread();
}
