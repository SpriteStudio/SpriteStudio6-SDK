/*
  ==============================================================================

    This file was auto-generated!

  ==============================================================================
*/

#pragma once

#include "../JuceLibraryCode/JuceHeader.h"

class AnimeState;

enum CommandIDs
{
	START= 1,
	STOP,
	RESET,
	LOAD_PROJECT,
	LOAD_ANIME,
};

//==============================================================================
/*
    This component lives inside our window, and this is where you should put all
    your controls and content.
*/

// コマンド処理を行うコンポーネント
class MainContentComponent :	public Component,
								public ApplicationCommandTarget,
								public Value::Listener
{
public:
    //==============================================================================
    MainContentComponent();
    ~MainContentComponent();

	static	MainContentComponent *	get();
	ApplicationCommandTarget* getNextCommandTarget() override;
	void	getCommandInfo(CommandID commandID, ApplicationCommandInfo& result) override;
	void	getAllCommands(Array<CommandID>& c) override;
	bool	perform(const InvocationInfo& info) override;
	void	valueChanged(Value & value) override;
	void	resized() override;

	//ボタンのコマンドマネージャー
	ApplicationCommandManager	commandManager;
private:
	static MainContentComponent *	myInst;
    //==============================================================================
    JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR (MainContentComponent)
};
