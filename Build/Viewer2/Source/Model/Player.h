/*
  ==============================================================================

    renderAnimation.h
    Created: 26 Jan 2018 5:46:29pm
    Author:  kurooka

  ==============================================================================
*/

#pragma once
#include "../JuceLibraryCode/JuceHeader.h"

class SsProject;
class SsAnimePack;
class SsAnimeDecoder;
class SsCellMapList;
class SSTextureFactory;

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
		static Value	animepackIndex;
		static Value	animeIndex;
		static Value	animeName;
		static Value	sequencepackIndex;
		static Value	sequenceIndex;
		static Value	sequenceName;
		static Value	startFrame;
		static Value	endFrame;
		static Value	loopFrame;
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
		virtual void	loadSequence(Player * p, int packIndex, int sequenceIndex);
		virtual void	onEnter(Player *) {};
		virtual void	onLeave(Player *) {};
		JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(State)
	};

	State *	currentState;
	void	changeState(State * newState);

	class StatePlaying : public State
	{
	public:
		StatePlaying() {};
		void	start(Player *) override {}
		void	loadProj(Player *, const String &) override {}
		void	loadAnime(Player *, int, int) override {}
		JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(StatePlaying)
	};

	class StatePaused : public State
	{
	public:
		StatePaused() {};
		void	stop(Player *) override {}
		void	hiResTimerCallback(Player *) override {}
		JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(StatePaused)
	};

	class StateLoading : public State
	{
	public:
		StateLoading() {};
		void	start(Player *) override {}
		void	stop(Player *) override {}
		void	reset(Player *) override {}
		void	hiResTimerCallback(Player *) override {}
		void	draw(Player *) {};
		void	loadProj(Player *, const String &) override {}
		void	loadAnime(Player *, int, int) override {}
		JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(StateLoading)
	};

	class StateInitial : public State
	{
	public:
		StateInitial() {};
		void	start(Player *) override {}
		void	stop(Player *) override {}
		void	reset(Player *) override {}
		void	hiResTimerCallback(Player *) override {}
		void	draw(Player *) {};
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
	void	loadAnime(int packIndex, int animeIndex);
	void	loadSequence(int packIndex, int sequenceIndex);
	State *	getState();
	static void	drawAnime();

	// アニメーションの状態
	std::unique_ptr<StatePlaying>		statePlaying;
	std::unique_ptr<StatePaused>		statePaused;
	std::unique_ptr<StateLoading>		stateLoading;
	std::unique_ptr<StateInitial>		stateInitial;
	std::unique_ptr<SsProject>			currentProj;
	std::unique_ptr<SsAnimeDecoder>		decoder;
	SsCellMapList *						cellmap = nullptr; // decoderのデストラクタでdeleteされる

	friend class AsyncAnimeLoader;
	friend class AsyncSequenceLoader;
	friend class AsyncProjectLoader;

	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(Player)
};
