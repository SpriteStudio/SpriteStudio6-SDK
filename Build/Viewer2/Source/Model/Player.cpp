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

Player*	Player::myInst = 0;
Value	Player::State::animepackIndex;
Value	Player::State::animeIndex;
Value	Player::State::animeName;
Value	Player::State::sequencepackIndex;
Value	Player::State::sequenceIndex;
Value	Player::State::sequenceName;
Value	Player::State::startFrame;
Value	Player::State::endFrame;
Value	Player::State::loopFrame;
Value	Player::State::length;
Value	Player::State::frame;
Value	Player::State::fps;
Value	Player::State::loop;

Player::~Player()
{
	stopTimer();
	myInst = nullptr;

	Player::State::animepackIndex = 0;
	Player::State::animeIndex = 0;
	Player::State::animeName = 0;
	Player::State::sequencepackIndex = 0;
	Player::State::sequenceIndex = 0;
	Player::State::sequenceName = 0;
	Player::State::startFrame = 0;
	Player::State::endFrame = 0;
	Player::State::loopFrame = 0;
	Player::State::length = 0;
	Player::State::frame = 0;
	Player::State::fps = 0;
	Player::State::loop = 0;
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

	currentState->animepackIndex	= 0;
	currentState->animeIndex		= 0;
	currentState->animeName			= "";
	currentState->sequencepackIndex	= 0;
	currentState->sequenceIndex		= 0;
	currentState->sequenceName		= "";
	currentState->startFrame		= 0;
	currentState->endFrame			= 10;
	currentState->loopFrame			= 0;
	currentState->length			= 10;
	currentState->frame				= 0;
	currentState->fps				= 30;
	currentState->loop				= false;

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
	int		loopFrame	= currentState->loopFrame.getValue();
	bool	loop		= currentState->loop.getValue();

	frame++;

	if (endFrame < frame)
	{
		if (loop)
		{
			frame = loopFrame > startFrame ? ( loopFrame < endFrame ? loopFrame : endFrame ) : startFrame;
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

void Player::loadSequence(int packIndex, int sequenceIndex)
{
	currentState->loadSequence(this, packIndex, sequenceIndex);
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

void Player::State::loadSequence(Player * p, int packIndex, int sequenceIndex)
{
	auto* asyncSequenceLoader = new AsyncSequenceLoader();
	asyncSequenceLoader->setSequenceIndex(packIndex, sequenceIndex);
	asyncSequenceLoader->launchThread();
}
