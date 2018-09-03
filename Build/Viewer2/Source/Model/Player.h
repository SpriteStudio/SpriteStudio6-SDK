/*
  ==============================================================================

    renderAnimation.h
    Created: 26 Jan 2018 5:46:29pm
    Author:  kurooka

  ==============================================================================
*/

#pragma once

#include "../JuceLibraryCode/JuceHeader.h"
#include "ssHelper.h"
#include "ssplayer_animedecode.h"
#include <queue>

class SsProject;
class SsAnimePack;
class SsAnimeDecoder;
class SsCellMapList;
class SSTextureFactory;
class IRequest;
class RequestSetProj;
class RequestSetAnime;

class Player : public juce::HighResolutionTimer
{
	void	draw();
	void	tick();
	void	hiResTimerCallback() override;

	static Player*	myInst;

	class State
	{
	public:
		State() {};
		virtual ~State() {};
		static Value	packIndex;
		static Value	animeIndex;
		static Value	animeName;
		static Value	startFrame;
		static Value	endFrame;
		static Value	length;
		static Value	frame;
		static Value	fps;
		static Value	loop;
	protected:
		friend class Player;
		// デフォルトの実装
		virtual void	start(Player * p);
		virtual void	stop(Player * p);
		virtual void	reset(Player * p);
		virtual void	hiResTimerCallback(Player * p);
		virtual void	draw(Player * p);
		virtual void	loadProj(Player * p, const String & name);
		virtual void	loadAnime(Player * p, int packIndex, int animeIndex);
		virtual void	changeState(Player * p, State * newState);
		virtual void	onEnter(Player * p) {};
		virtual void	onLeave(Player * p) {};
		JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(State)
	};

	State *	currentState;
	void	changeState(State * newState);

	class StatePlaying : public State
	{
	public:
		StatePlaying() {};
		void	start(Player * p) override {}
		void	loadProj(Player * p, const String & name) override {}
		void	loadAnime(Player * p, int packIndex, int animeIndex) override {}
		JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(StatePlaying)
	};

	class StatePaused : public State
	{
	public:
		StatePaused() {};
		void	stop(Player * p) override {}
		void	hiResTimerCallback(Player * p) override {}
		JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(StatePaused)
	};

	class StateLoading : public State
	{
	public:
		StateLoading() {};
		void	start(Player * p) override {}
		void	stop(Player * p) override {}
		void	reset(Player * p) override {}
		void	hiResTimerCallback(Player * p) override {}
		void	draw(Player * p) {};
		void	loadProj(Player * p, const String & name) override {}
		void	loadAnime(Player * p, int packIndex, int animeIndex) override {}
		JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(StateLoading)
	};

	class StateInitial : public State
	{
	public:
		StateInitial() {};
		void	start(Player * p) override {}
		void	stop(Player * p) override {}
		void	reset(Player * p) override {}
		void	hiResTimerCallback(Player * p) override {}
		void	draw(Player * p) {};
		JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(StateInitial)
	};

public:
	Player();
	virtual ~Player();

	static Player *	get();

	void	play();
	void	stop();
	void	reset();
	void	loadProj(const String & name);
	void	setProj();
	void	loadAnime(int packIndex, int animeIndex);
	void	setAnime();
	void	initGL();
	void	putRequest(IRequest* request);
	std::queue<IRequest*>*	getRequestQueue();
	std::queue<IRequest*>	requestQueue;
	ScopedPointer<RequestSetProj>	requestSetProj;
	ScopedPointer<RequestSetAnime>	requestSetAnime;
	State *	getState();
	static void	drawAnime();

	// アニメーションの状態
	ScopedPointer<StatePlaying>		statePlaying;
	ScopedPointer<StatePaused>		statePaused;
	ScopedPointer<StateLoading>		stateLoading;
	ScopedPointer<StateInitial>		stateInitial;
	ScopedPointer<SsProject>		currentProj;
	ScopedPointer<SsAnimeDecoder>	decoder;
	ScopedPointer<SSTextureFactory>	texfactory;
	SsAnimePack *					animePack = nullptr;
	SsCellMapList *					cellmap = nullptr;
	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(Player)
};

class IRequest
{
public:
	virtual void execute() = 0;
};

class RequestSetProj: public IRequest
{
	virtual void execute() override;
};

class RequestSetAnime : public IRequest
{
	virtual void execute() override;
};