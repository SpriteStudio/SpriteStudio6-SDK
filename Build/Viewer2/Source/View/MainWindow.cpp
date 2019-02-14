#include "ssHelper.h"
#include "Controller/MainComponent.h"
#include "Model/Player.h"
#include "View/DocumentView3D.h"
#include "View/MainWindow.h"
#include "View/GPL_v3.h"

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
	// 最近開いたファイルリストを保存
	auto * controller = MainContentComponent::get();
	String fileList = recentlyOpenedFilesList.toString();
	controller->getProperties().setValue("RecentlyOpenedFilesList", fileList);
	controller->getProperties().save();

	if (animeTreeView)
	{
		animeTreeView->deleteRootItem();
	}

	delete colourSelectorWindow;
	delete licenseWindow;

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
    // メニューバーの作成
	buildMenuBar();
	// コントロールパネルの作成
	buildControlPanel();
	// サイドパネルの作成
	buildSidePanel();
	// GLウィンドウの作成
	buildGL();
}

void ViewerMainWindow::buildMenuBar()
{
	menuBar.reset(new MenuBarComponent(this));
	addAndMakeVisible(menuBar.get());

	auto * controller = MainContentComponent::get();
	String fileList = controller->getProperties().getValue("RecentlyOpenedFilesList");
	recentlyOpenedFilesList.restoreFromString(fileList);
	recentlyOpenedFilesList.setMaxNumberOfItems(10);
}

void ViewerMainWindow::buildControlPanel()
{
	// ボタンの作成
	auto * cm = &MainContentComponent::get()->commandManager;

	button_start.reset(new TextButton("start"));
	button_start->setCommandToTrigger(cm, CommandIDs::START, false);

	button_stop.reset(new TextButton("stop"));
	button_stop->setCommandToTrigger(cm, CommandIDs::STOP, false);

	button_reset.reset(new TextButton("reset"));
	button_reset->setCommandToTrigger(cm, CommandIDs::RESET, false);

	button_loop.reset(new TextButton("loop"));
	button_loop->setClickingTogglesState(true);
	button_loop->getToggleStateValue().referTo(state.loop);

	// スライダーの作成
	buildSlider();

	controlPanel.reset(new Component());
	controlPanel->addAndMakeVisible(button_start.get());
	controlPanel->addAndMakeVisible(button_stop.get());
	controlPanel->addAndMakeVisible(button_reset.get());
	controlPanel->addAndMakeVisible(button_loop.get());
	controlPanel->addAndMakeVisible(slider_frame.get());

	auto * player = Player::get();
	if (player->getState()->animeName == "Setup")
	{
		controlPanel->setEnabled(false);
	}

	addAndMakeVisible(controlPanel.get());
}

void ViewerMainWindow::buildSidePanel()
{
	sidePanel.reset(new ConcertinaPanel());

	// ツリービューの作成
	buildTreeView();
	// プロパティーパネルの作成
	buildPropertyPanel();

	sidePanel->addPanel(-1, animeTreeView.get(), false);
	sidePanel->expandPanelFully(animeTreeView.get(), true);

	sidePanel->addPanel(-1, propertyPanel.get(), false);
	sidePanel->setMaximumPanelSize(propertyPanel.get(), propertyPanel->getTotalContentHeight());
	sidePanel->expandPanelFully(propertyPanel.get(), true);

	addAndMakeVisible(sidePanel.get());
}

void ViewerMainWindow::buildGL()
{
	opengl.reset(new DocumentView3D());
	addAndMakeVisible(opengl.get());
}

void ViewerMainWindow::openColourSelectorWindow()
{
	colourSelectorWindow = new ColourSelectorWindow("ColourSelectorWindow", Colours::black, DocumentWindow::TitleBarButtons::allButtons);

	Rectangle<int> area(0, 0, 300, 400);
	RectanglePlacement placement(RectanglePlacement::centred | RectanglePlacement::doNotResize);
	auto result = placement.appliedTo(area, getScreenBounds());
	colourSelectorWindow->setBounds(result);
	colourSelectorWindow->setUsingNativeTitleBar(false);
	colourSelectorWindow->setVisible(true);
}

void ViewerMainWindow::openLicenseWindow()
{
	licenseWindow = new LicenseWindow("Viewer2", LookAndFeel::getDefaultLookAndFeel().findColour(TextEditor::ColourIds::backgroundColourId), DocumentWindow::TitleBarButtons::closeButton);

	Rectangle<int> area(licenseWindow->getLocalBounds());
	RectanglePlacement placement(RectanglePlacement::centred | RectanglePlacement::doNotResize);
	auto result = placement.appliedTo(area, getScreenBounds());
	licenseWindow->setBounds(result);
	licenseWindow->setUsingNativeTitleBar(false);
	licenseWindow->setVisible(true);
}

void ViewerMainWindow::setBackGroundColour(const Colour & c)
{
	if (opengl)
	{
		opengl->setBackGroundColour(c);
	}
}

void ViewerMainWindow::addRecentlyOpenedFile(const File & proj)
{
	recentlyOpenedFilesList.addFile(proj);
}

OpenGLContext & ViewerMainWindow::getOpenGLContext()
{
	return opengl->openGLContext;
}

void ViewerMainWindow::paint (Graphics& g)
{
}

StringArray ViewerMainWindow::getMenuBarNames()
{
	return { "File", "Tools", "Help"};
}

PopupMenu ViewerMainWindow::getMenuForIndex(int menuIndex, const String &)
{
	PopupMenu menu;
	PopupMenu subMenu;
	auto * cm = &MainContentComponent::get()->commandManager;

	if (menuIndex == 0)
	{
		menu.addCommandItem(cm, CommandIDs::OPEN);
		menu.addSeparator();
		recentlyOpenedFilesList.createPopupMenuItems(subMenu, 3010, true, true);
		menu.addSubMenu("Open Recent", subMenu);
		menu.addSeparator();
		menu.addCommandItem(cm, CommandIDs::EXIT);
	}
	else
	if(menuIndex == 1)
	{
		menu.addItem(3100, "BackGroundColor", true, false);
	}
	else
	if(menuIndex == 2)
	{
		menu.addItem(3200, "About", true, false);
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
		buildControlPanel();
		resized();
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
		case 3100:
		{
			openColourSelectorWindow();
		}
		break;
		case 3200:
		{
			openLicenseWindow();
		}
		break;
	}

	if (3010 <= menuItemID && 3019 >= menuItemID)
	{
		int index = menuItemID - 3010;
		String fn = recentlyOpenedFilesList.getFile(index).getFullPathName();
		Player::get()->loadProj(fn);
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

	slider_frame.reset(new Slider(Slider::ThreeValueHorizontal, Slider::TextEntryBoxPosition::TextBoxLeft));
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

	SsProject * proj = Player::get()->currentProj.get();
	auto root = createTreeItem("proj", -1, -1);
	SsAnimePackList & alist = proj->getAnimePackList();
	for (int i = 0; i < alist.size(); i++)
	{
		// アニメパック名
		SsAnimePack* animepack = alist[i];
		//String animepackName(babel::auto_translate<std::string>(animepack->name, babel::base_encoding::utf8));
		String animepackName(animepack->name);
		ValueTree ssae = createTreeItem(animepackName, i, -1);
		for (int j = 0; j < animepack->animeList.size(); j++)
		{
			SsAnimation* anime = animepack->animeList[j];
			// アニメ名
			//String animeName(babel::auto_translate<std::string>(anime->name, babel::base_encoding::utf8));
			String animeName(anime->name);
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
	if (animeTreeView)
	{
		animeTreeView->deleteRootItem();
	}
	ValueTree root = createTree();
	rootItem = new ViewerTreeViewItem(root);
	animeTreeView.reset(new TreeView());
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

	propertyPanel.reset(new PropertyPanel("Property"));
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
	ViewerMainWindow::get()->getState()->packIndex = getPackIndex();
	ViewerMainWindow::get()->getState()->animeIndex = getAnimeIndex();
	//コマンドを発行
	ApplicationCommandTarget::InvocationInfo info(CommandIDs::LOAD_ANIME);
	MainContentComponent::get()->commandManager.invoke(info, true);
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


ColourSelectorWindow::ColourSelectorWindow(const String & name, const Colour & colour, int buttonsNeeded)
	: DocumentWindow(name, colour, buttonsNeeded)
{
	colourSelector.reset(new ColourSelector(ColourSelector::showColourAtTop | ColourSelector::showSliders | ColourSelector::showColourspace));
	colourSelector->setCurrentColour(colour);
	setContentOwned(colourSelector.get(), false);
	enterModalState();
}

void ColourSelectorWindow::closeButtonPressed()
{
	ViewerMainWindow::get()->setBackGroundColour(colourSelector->getCurrentColour());
	exitModalState(0);
	delete this;
}

LicenseWindow::LicenseWindow(const String & name, const Colour & colour, int buttonsNeeded)
	: DocumentWindow(name, colour, buttonsNeeded)
{
	Justification centre(Justification::Flags::centred);
	Rectangle<int> area(Rectangle<int>(0, 0, 500, 500));
	auto * layout = new Component();
	layout->setBounds(area);

	// copyright
	String text;
	text	
		<< "Copyright(c) 2018, Web Technology Corp." << newLine
		<< "All rights reserved.";
	copyright.reset(new Label);
	copyright->setText(text, NotificationType::dontSendNotification);
	copyright->setJustificationType(centre);
	copyright->setBounds(area.removeFromTop(50));
	layout->addAndMakeVisible(copyright.get());

	// license
	String licenseText(GPL_v3::GPL_v3_txt);
	license.reset(new TextEditor());
	license->setMultiLine(true);
	license->setReadOnly(true);
	license->setFont(Font(15.0f));
	license->setText(licenseText);
	license->setBounds(area);
	layout->addAndMakeVisible(license.get());

	setContentOwned(layout, true);
	enterModalState();
}

void LicenseWindow::closeButtonPressed()
{
	exitModalState(0);
	delete this;
}