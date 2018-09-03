#include "Controller/MainComponent.h"
#include "Model/Player.h"
#include "View/DocumentView3D.h"
#include "View/MainWindow.h"

ViewerMainWindow::ViewerMainWindow()
{
	myInst = this;
	colourSelectorWindow = nullptr;

	state.frame = 0;
	state.startFrame = 0;
	state.endFrame = 10;
	state.length = 10;
	state.fps = 30;

	auto * controller = MainContentComponent::get();
	state.startFrame.addListener(controller);
	state.endFrame.addListener(controller);
	state.length.addListener(controller);
	state.frame.addListener(controller);
	state.fps.addListener(controller);
	state.loop.addListener(controller);

	state.endFrame.addListener(this);

	build();
}

ViewerMainWindow::~ViewerMainWindow()
{
	delete colourSelectorWindow;
	colourSelectorWindow = nullptr;

	myInst = nullptr;
}

ViewerMainWindow * ViewerMainWindow::myInst = nullptr;
ViewerMainWindow * ViewerMainWindow::get()
{
	if (myInst == nullptr)
	{
		new ViewerMainWindow();
	}
	return myInst;
}

void ViewerMainWindow::resized()
{
	{
		Rectangle<int> area(getLocalBounds());

		if (menuBar)
		{
			menuBar->setBounds(area.removeFromTop(LookAndFeel::getDefaultLookAndFeel().getDefaultMenuBarHeight()));
		}
		if (controlPanel)
		{
			controlPanel->setBounds(area.removeFromBottom(50));
			auto area = controlPanel->getLocalBounds();
			button_start->setBounds(area.removeFromLeft(50));
			button_stop->setBounds(area.removeFromLeft(50));
			button_reset->setBounds(area.removeFromLeft(50));
			button_loop->setBounds(area.removeFromLeft(50));
			slider_frame->setBounds(area);
		}
		if (sidePanel)
		{
			sidePanel->setBounds(area.removeFromLeft(200));
		}
		if (opengl)
		{
			opengl->setBounds(area);
		}
	}
}

void ViewerMainWindow::build()
{
    //メニューバーの作成
	addAndMakeVisible(menuBar = new MenuBarComponent(this));
	// コントロールパネルの作成
	buildControlPanel();
	// サイドパネルの作成
	buildSidePanel();
	// GLウィンドウの作成
	buildGL();
}

void ViewerMainWindow::buildControlPanel()
{
	if (controlPanel)
	{
		removeChildComponent(controlPanel);
	}

	// ボタンの作成
	auto * cm = &MainContentComponent::get()->commandManager;

	button_start	= new TextButton("start");
	button_start->setCommandToTrigger(cm, CommandIDs::START, false);

	button_stop		= new TextButton("stop");
	button_stop->setCommandToTrigger(cm, CommandIDs::STOP, false);

	button_reset	= new TextButton("reset");
	button_reset->setCommandToTrigger(cm, CommandIDs::RESET, false);

	button_loop		= new TextButton("loop");
	button_loop->setClickingTogglesState(true);
	button_loop->getToggleStateValue().referTo(state.loop);

	// スライダーの作成
	buildSlider();

	controlPanel = new Component();
	controlPanel->addAndMakeVisible(button_start);
	controlPanel->addAndMakeVisible(button_stop);
	controlPanel->addAndMakeVisible(button_reset);
	controlPanel->addAndMakeVisible(button_loop);
	controlPanel->addAndMakeVisible(slider_frame);

	auto * player = Player::get();
	if (player->getState()->animeName == "Setup")
	{
		controlPanel->setEnabled(false);
	}

	addAndMakeVisible(controlPanel);
}

void ViewerMainWindow::buildSidePanel()
{
	if (sidePanel)
	{
		removeChildComponent(sidePanel);
	}

	sidePanel = new ConcertinaPanel();

	// ツリービューの作成
	buildTreeView();
	// プロパティーパネルの作成
	buildPropertyPanel();

	sidePanel->addPanel(-1, animeTreeView, false);
	sidePanel->expandPanelFully(animeTreeView, true);

	sidePanel->addPanel(-1, propertyPanel, false);
	sidePanel->setMaximumPanelSize(propertyPanel, propertyPanel->getTotalContentHeight());
	sidePanel->expandPanelFully(propertyPanel, true);

	addAndMakeVisible(sidePanel);
}

void ViewerMainWindow::buildGL()
{
	if (opengl)
	{
		removeChildComponent(opengl);
	}
	opengl = new DocumentView3D();
	addAndMakeVisible(opengl);
}

void ViewerMainWindow::openColourSelectorWindow()
{
	colourSelectorWindow = new ColourSelectorWindow("ColourSelectorWindow", Colours::black, DocumentWindow::TitleBarButtons::allButtons);

	Rectangle<int> area(0, 0, 300, 400);
	RectanglePlacement placement(RectanglePlacement::centred | RectanglePlacement::doNotResize);

	auto result = placement.appliedTo(area, getLocalBounds());
	colourSelectorWindow->setBounds(result);
	colourSelectorWindow->setUsingNativeTitleBar(true);
	colourSelectorWindow->setVisible(true);
}

void ViewerMainWindow::setBackGroundColour(const Colour & c)
{
	if (opengl)
	{
		opengl->setBackGroundColour(c);
	}
}

void ViewerMainWindow::paint (Graphics& g)
{
}

StringArray ViewerMainWindow::getMenuBarNames()
{
	return { "File", "Tools"};
}

PopupMenu ViewerMainWindow::getMenuForIndex(int menuIndex, const String &)
{
	PopupMenu menu;

	if (menuIndex == 0)
	{
		menu.addItem(3000, "Open", true, false);
		menu.addSeparator();
		menu.addItem(3001, "Exit", true, false);
	}
	else
	if(menuIndex == 1)
	{
		menu.addItem(3002, "BackGroundColor", true, false);
	}
	return menu;
}

void ViewerMainWindow::valueChanged(Value & value)
{
	auto * view = ViewerMainWindow::get();
	auto * model = Player::get();

	//------------------------
	// モデルが変更された場合
	//------------------------
	if (value.refersToSameSourceAs(model->getState()->startFrame))
	{
		// startFrameが変更された場合の処理
		if (sliderShouldRebuild((int)value.getValue(), view->getState()->endFrame.getValue()))
		{
			buildControlPanel();
			resized();
		}
		view->getState()->startFrame = (int)value.getValue();
	}
	else
	if (value.refersToSameSourceAs(model->getState()->endFrame))
	{
		// endFrameが変更された場合の処理
		if (sliderShouldRebuild(view->getState()->startFrame.getValue(),(int)value.getValue()))
		{
			buildControlPanel();
			resized();
		}
		view->getState()->endFrame = (int)value.getValue();
	}
	else
	if (value.refersToSameSourceAs(model->getState()->length))
	{
		// lengthが変更された場合の処理
		buildControlPanel();
		resized();
		view->getState()->length = (int)value.getValue();
	}
	else
	if (value.refersToSameSourceAs(model->getState()->fps))
	{
		// fpsが変更された場合の処理
		view->getState()->fps = (int)value.getValue();
	}
	else
	if (value.refersToSameSourceAs(model->getState()->frame))
	{
		// frameが変更された場合の処理
		view->getState()->frame = (int)value.getValue();
	}
	else
	if (value.refersToSameSourceAs(model->getState()->animeName))
	{
		// animeNameが変更された場合の処理
	}

	//------------------------
	// ビューが変更された場合
	//------------------------
	if (value.refersToSameSourceAs(view->getState()->endFrame))
	{
		// endFrameが変更された場合の処理
		if (!view->getState()->endFrame.getValue().isInt())
		{
			view->getState()->endFrame = (int)value.getValue();
		}
	}
}

ViewerMainWindow::State * ViewerMainWindow::getState()
{
	return &state;
}

void ViewerMainWindow::onFullScreenModeItemTriggered()
{
	if (getPeer())
	{
		getPeer()->setFullScreen(true);
	}
	Desktop::getInstance().setKioskModeComponent(getTopLevelComponent(), false);

	resized();
}

void ViewerMainWindow::menuItemSelected(int menuItemID, int)
{
	DBG(menuItemID);

	switch (menuItemID)
	{
		case 3000:
		{
			FileChooser fc("Choose a file to open...",
				File::getCurrentWorkingDirectory(),
				"*.sspj",
				false);

			
			if (fc.browseForMultipleFilesToOpen())
			{
				juce::String fn = fc.getResults().getReference(0).getFullPathName();
				Player::get()->loadProj(fn);

				// ツリービューを作成
				buildSidePanel();
				resized();
			}
		}
		break;
		case 3001:
		{
            JUCEApplication::getInstance()->systemRequestedQuit();
		}
		break;
		case 3002:
		{
			openColourSelectorWindow();
		}
		break;
	}
}

bool ViewerMainWindow::sliderShouldRebuild(const int min, const int max)
{
	int startFrame	= getState()->startFrame.getValue();
	int endFrame	= getState()->endFrame.getValue();
	if (min < startFrame ||
		min > endFrame ||
		max < startFrame ||
		max > endFrame)
	{
		return true;
	}
	else
	{
		return false;
	}
}

void ViewerMainWindow::buildSlider()
{
	auto * model = Player::get();
	int startFrame_model	= model->getState()->startFrame.getValue();
	int length_model		= model->getState()->length.getValue();

	if (startFrame_model > length_model)
	{
		state.startFrame = startFrame_model;
		state.endFrame = startFrame_model;
	}

	if (length_model < 1)
	{
		startFrame_model = 0;
		length_model = 1;
	}

	slider_frame = new Slider(Slider::ThreeValueHorizontal, Slider::TextEntryBoxPosition::TextBoxLeft);
	slider_frame->setRange(startFrame_model, length_model, 1);
	slider_frame->setMinValue(startFrame_model);
	slider_frame->setMaxValue(length_model);

	state.startFrame	= startFrame_model;
	state.endFrame		= length_model;

	slider_frame->getMinValueObject().referTo(state.startFrame);
	slider_frame->getMaxValueObject().referTo(state.endFrame);
	slider_frame->getValueObject().referTo(state.frame);
	slider_frame->setInterceptsMouseClicks(true, true);
}

ValueTree ViewerMainWindow::createTreeItem(const String & name, int _packIndex, int _animeIndex)
{
	ValueTree t("Item");
	t.setProperty("name", name, nullptr);
	t.setProperty("packIndex", _packIndex, nullptr);
	t.setProperty("animeIndex", _animeIndex, nullptr);
	return t;
}

ValueTree ViewerMainWindow::createTree()
{
	if (Player::get()->currentProj == 0)
	{
		return createTreeItem("", 0, -1);
	}

	SsProject * proj = Player::get()->currentProj;
	auto root = createTreeItem("proj", -1, -1);
	SsAnimePackList alist = proj->getAnimePackList();
	for (int i = 0; i < alist.size(); i++)
	{
		// アニメパック名
		SsAnimePack* animepack = alist[i];
		String animepackName(babel::auto_translate<std::string>(animepack->name, babel::base_encoding::utf8));
		ValueTree ssae = createTreeItem(animepackName, i, -1);
		for (int j = 0; j < animepack->animeList.size(); j++)
		{
			SsAnimation* anime = animepack->animeList[j];
			// アニメ名
			String animeName(babel::auto_translate<std::string>(anime->name, babel::base_encoding::utf8));
			if (animeName == "Setup")
			{
				ssae.addChild(createTreeItem(animeName, i, j), 0, nullptr);
			}
			else
			{
				ssae.addChild(createTreeItem(animeName, i, j), -1, nullptr);
			}
		}
		root.appendChild(ssae, nullptr);
	}
	return root;
}

void ViewerMainWindow::buildTreeView()
{
	ValueTree root = createTree();
	rootItem = new ViewerTreeViewItem(root);
	animeTreeView = new TreeView();
	animeTreeView ->setDefaultOpenness(true);
	animeTreeView ->setRootItem(rootItem);
}

void ViewerMainWindow::buildPropertyPanel()
{
	Array<PropertyComponent*> items;
	items.add(new TextPropertyComponent(state.startFrame, "startFrame", 10, false, false));
	items.add(new TextPropertyComponent(state.endFrame, "endFrame", 10, false, false));
	items.add(new TextPropertyComponent(state.length, "length", 10, false, false));
	items.add(new SliderPropertyComponent(state.fps, "fps", 1, 60, 1));

	propertyPanel = new PropertyPanel("Property");
	propertyPanel->addProperties(items);
}

//-------------------------------------
// ViewerTreeViewItemの実装
//-------------------------------------

ViewerTreeViewItem::ViewerTreeViewItem(ValueTree t)
	: tree(t)
{
}

ViewerTreeViewItem::~ViewerTreeViewItem()
{
}

String ViewerTreeViewItem::getUniqueName() const
{
	return tree["name"].toString();
}

bool ViewerTreeViewItem::mightContainSubItems()
{
	return tree.getNumChildren() > 0;
}

void ViewerTreeViewItem::paintItem(Graphics & g, int width, int height)
{
	g.setColour(LookAndFeel::getDefaultLookAndFeel().findColour(TextEditor::ColourIds::textColourId));
	g.drawText(tree["name"].toString(),4, 0, width - 4, height,Justification::centredLeft, true);
}

void ViewerTreeViewItem::itemOpennessChanged(bool isNowOpen)
{
	if (isNowOpen && getNumSubItems() == 0)
		refreshSubItems();
	else
		clearSubItems();
}

void ViewerTreeViewItem::itemClicked(const MouseEvent & e)
{
	//コマンドを発行
	ApplicationCommandTarget::InvocationInfo info(CommandIDs::LOAD_ANIME);
	info.originatingComponent = this;
	MainContentComponent::get()->invoke(info, false);
}

int ViewerTreeViewItem::getPackIndex()
{
	return tree["packIndex"];
}

int ViewerTreeViewItem::getAnimeIndex()
{
	return tree["animeIndex"];
}

void ViewerTreeViewItem::refreshSubItems()
{
	clearSubItems();

	for (int i = 0; i < tree.getNumChildren(); ++i)
	{
		addSubItem(new ViewerTreeViewItem(tree.getChild(i)));
	}
}


ColourSelectorWindow::ColourSelectorWindow(const String & name, Colour colour, int buttonsNeeded)
	: DocumentWindow(name, colour, buttonsNeeded)
{
	selector = new ColourSelector(ColourSelector::showColourAtTop | ColourSelector::showSliders | ColourSelector::showColourspace);
	selector->setCurrentColour(colour);
	setContentOwned(selector, false);
}

void ColourSelectorWindow::closeButtonPressed()
{
	ViewerMainWindow::get()->setBackGroundColour(selector->getCurrentColour());
	delete this;
}
