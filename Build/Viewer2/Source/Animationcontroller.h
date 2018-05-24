/*
  ==============================================================================

    renderAnimation.h
    Created: 26 Jan 2018 5:46:29pm
    Author:  kurooka

  ==============================================================================
*/

#pragma once

#include "ssHelper.h"
#include "../JuceLibraryCode/JuceHeader.h"

class SsAnimeDecoder;
class SsCellMapList;



class SSAnimationController : public juce::HighResolutionTimer
{
public:
	SSAnimationController();
	virtual ~SSAnimationController();

	static SSAnimationController*	get()
	{
		return myInst;
	}

	static void	draw();
	static void	create();
	static void	destry();

	void	loadListener(SsProject* pj);

	void	stop();
	void	play();
	void	reset();

	virtual void hiResTimerCallback();

	SsProject* getProject() { return currentProj;
	}


private:
	SsAnimeDecoder*	m_player;
	SsCellMapList*	m_cellmap;

	bool			isReset;
		
	static SSAnimationController*	myInst;

	void	drawMain();

	SsProject*		currentProj;
	SsAnimePack*	currentAnimePack;

	double				currentFrame;
	bool	fileLoadTriger;


	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(SSAnimationController)




};



