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
MainContentComponent::MainContentComponent()
{
	myInst = this;

	//日本語への対応
#if JUCE_WINDOWS
	getLookAndFeel().setDefaultSansSerifTypefaceName("Meiryo UI");
#elif JUCE_MAC
	getLookAndFeel().setDefaultSansSerifTypefaceName("YuGothic");
#endif

	setSize(1200, 800); //てきとう

	// babelを初期化
	babel::init_babel();

	// モデルの作成
	auto * animePlayer = Player::get();
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
}


ApplicationCommandTarget * MainContentComponent::getNextCommandTarget()
{
	return findFirstTargetParentComponent();
}

void MainContentComponent::getCommandInfo(CommandID commandID, ApplicationCommandInfo & result)
{
	switch (commandID)
	{
	case CommandIDs::START:
		result.setActive(true);
		break;
	case CommandIDs::STOP:
		result.setActive(true);
		break;
	case CommandIDs::RESET:
		result.setActive(true);
		break;
	case CommandIDs::LOAD_PROJECT:
		result.setActive(true);
		break;
	case CommandIDs::LOAD_ANIME:
		result.setActive(true);
		break;
	default:
		break;
	}
}

void MainContentComponent::getAllCommands(Array<CommandID>& c)
{
	Array<CommandID> commands
	{
		CommandIDs::START,
		CommandIDs::STOP,
		CommandIDs::RESET,
		CommandIDs::LOAD_PROJECT,
		CommandIDs::LOAD_ANIME,
	};

	c.addArray(commands);
}

bool MainContentComponent::perform(const InvocationInfo & info)
{
	auto * animePlayer = Player::get();
	switch (info.commandID)
	{
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
			auto *	animeTree = dynamic_cast<ViewerTreeViewItem*>(info.originatingComponent);
			if (animeTree)
			{
				int packIndex = animeTree->getPackIndex();
				int animeIndex = animeTree->getAnimeIndex();
				if (packIndex < 0 || animeIndex < 0)
				{
					break;
				}
				//アニメーションを読み込み
				animePlayer->loadAnime(packIndex, animeIndex);
			}
		}
		break;
	default:
		return false;
	}

	repaint();
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


MainContentComponent * MainContentComponent::get()
{
	return myInst;
}
