/*
  ==============================================================================

    This file was auto-generated!

  ==============================================================================
*/

#include "Controller/MainComponent.h"
#include "Model/Player.h"
#include "View/DocumentView3D.h"
#include "View/MainWindow.h"

MainContentComponent* MainContentComponent::myInst = nullptr;

//==============================================================================
MainContentComponent::MainContentComponent() : properties(getPropertyFileOptions())
{
	myInst = this;

	//日本語への対応
#if JUCE_WINDOWS
	getLookAndFeel().setDefaultSansSerifTypefaceName("Meiryo UI");
#elif JUCE_MAC
	getLookAndFeel().setDefaultSansSerifTypefaceName("YuGothic");
#endif

	commandManager.registerAllCommandsForTarget(this);

	setSize(1200, 800); //てきとう

	// モデルの作成
	//Player::get();
	// ビューの作成
	auto * mainWindow = ViewerMainWindow::get();
	addAndMakeVisible(mainWindow);
}

MainContentComponent::~MainContentComponent()
{
	// ビューの解体
	delete ViewerMainWindow::get();
	// モデルの解体
	delete Player::get();

	myInst = nullptr;
}


ApplicationCommandTarget * MainContentComponent::getNextCommandTarget()
{
	return findFirstTargetParentComponent();
}

void MainContentComponent::getCommandInfo(CommandID commandID, ApplicationCommandInfo & result)
{
	switch (commandID)
	{
	case CommandIDs::OPEN:
		result.setActive(true);
		result.shortName = "Open";
		break;
	case CommandIDs::START:
		result.setActive(true);
		result.shortName = "Start";
		break;
	case CommandIDs::STOP:
		result.setActive(true);
		result.shortName = "Stop";
		break;
	case CommandIDs::RESET:
		result.setActive(true);
		result.shortName = "Reset";
		break;
	case CommandIDs::LOAD_ANIME:
		result.setActive(true);
		result.shortName = "Load anime";
		break;
	case CommandIDs::LOAD_SEQUENCE:
		result.setActive(true);
		result.shortName = "Load sequence";
		break;
	case CommandIDs::EXIT:
		result.setActive(true);
		result.shortName = "Exit";
		break;
	default:
		break;
	}
}

void MainContentComponent::getAllCommands(Array<CommandID>& c)
{
	Array<CommandID> commands
	{
		CommandIDs::OPEN,
		CommandIDs::START,
		CommandIDs::STOP,
		CommandIDs::RESET,
		CommandIDs::LOAD_ANIME,
		CommandIDs::LOAD_SEQUENCE,
		CommandIDs::EXIT,
	};

	c.addArray(commands);
}

bool MainContentComponent::perform(const InvocationInfo & info)
{
	auto * animePlayer = Player::get();
	switch (info.commandID)
	{
	case CommandIDs::OPEN:
	{
		FileChooser fc("Choose a file to open...", File::getCurrentWorkingDirectory(), "*.sspj", false);
		if (fc.browseForMultipleFilesToOpen())
		{
			juce::String fn = fc.getResults().getReference(0).getFullPathName();
			// プロジェクトを読み込み
			animePlayer->loadProj(fn);
		}
		break;
	}
	case CommandIDs::START:
		animePlayer->play();
		break;
	case CommandIDs::STOP:
		animePlayer->stop();
		break;
	case CommandIDs::RESET:
		animePlayer->reset();
		break;
	case CommandIDs::LOAD_ANIME:
	{
		int animepackIndex = ViewerMainWindow::get()->getState()->animepackIndex.getValue();
		int animeIndex = ViewerMainWindow::get()->getState()->animeIndex.getValue();
		if (animepackIndex < 0 || animeIndex < 0)
		{
			break;
		}
		//アニメーションを読み込み
		animePlayer->loadAnime(animepackIndex, animeIndex);
		break;
	}
	case CommandIDs::LOAD_SEQUENCE:
	{
		int sequencepackIndex = ViewerMainWindow::get()->getState()->sequencepackIndex.getValue();
		int sequenceIndex = ViewerMainWindow::get()->getState()->sequenceIndex.getValue();
		if (sequencepackIndex < 0 || sequenceIndex < 0)
		{
			break;
		}
		//シーケンスを読み込み
		animePlayer->loadSequence(sequencepackIndex, sequenceIndex);
		break;
	}
	case CommandIDs::EXIT:
	{
		JUCEApplication::getInstance()->systemRequestedQuit();
		break;
	}
	default:
		return false;
	}
	return true;
}

void MainContentComponent::valueChanged(Value & value)
{
	auto * view = ViewerMainWindow::get();
	auto * model = Player::get();

	if (value.refersToSameSourceAs(view->get()->getState()->startFrame))
	{
		model->getState()->startFrame = (int)value.getValue();
	}
	else
	if (value.refersToSameSourceAs(view->get()->getState()->endFrame))
	{
		model->getState()->endFrame = (int)value.getValue();
	}
	else
	if (value.refersToSameSourceAs(view->get()->getState()->length))
	{
	}
	else
	if (value.refersToSameSourceAs(view->get()->getState()->frame))
	{
		model->getState()->frame = (int)value.getValue();
	}
	else
	if (value.refersToSameSourceAs(view->get()->getState()->fps))
	{
		model->getState()->fps = (int)value.getValue();
	}
	else
	if (value.refersToSameSourceAs(view->get()->getState()->loop))
	{
		model->getState()->loop = (bool)value.getValue();
	}
}

void MainContentComponent::resized()
{
	auto * mainWindow = ViewerMainWindow::get();
	mainWindow->setBounds(getLocalBounds());
}

PropertiesFile & MainContentComponent::getProperties()
{
	return properties;
}


MainContentComponent * MainContentComponent::get()
{
	return myInst;
}
