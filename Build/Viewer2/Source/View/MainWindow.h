﻿#pragma once
#include "../JuceLibraryCode/JuceHeader.h"

class ViewerTreeViewItem;
class ColourSelectorWindow;
class DocumentView3D;

class ViewerMainWindow :	public Component,
							public MenuBarModel,
							public Value::Listener
{
public:
	ViewerMainWindow();
	~ViewerMainWindow();
	static ViewerMainWindow *	get();
    void		paint (Graphics&) override;
    void		resized() override;
	void		menuItemSelected(int menuItemID, int /*topLevelMenuIndex*/) override;
	void		onFullScreenModeItemTriggered();
	StringArray	getMenuBarNames() override;
	PopupMenu	getMenuForIndex(int menuIndex, const String& /*menuName*/) override;
	void		valueChanged(Value & value) override;
	void		build();
	void		buildMenuBar();
	void		buildControlPanel();
	void		buildSidePanel();
	void		buildGL();
	void		openColourSelectorWindow();
	void		setBackGroundColour(const Colour & c);
	void		addRecentlyOpenedFile(const File & proj);
	OpenGLContext &	getOpenGLContext();
	struct State
	{
		Value	frame;
		Value	startFrame;
		Value	endFrame;
		Value	length;
		Value	fps;
		Value	loop;
		Value	packIndex;
		Value	animeIndex;
	};
	State *	getState();
    //==============================================================================
private:
	static ViewerMainWindow *	myInst;
	// メニューバー
	std::unique_ptr<MenuBarComponent>	menuBar;
	// OpenGL描画領域
	std::unique_ptr<DocumentView3D>	opengl;
	// ウィジェット
	std::unique_ptr<TextButton>		button_start;
	std::unique_ptr<TextButton>		button_stop;
	std::unique_ptr<TextButton>		button_reset;
	std::unique_ptr<TextButton>		button_loop;
	std::unique_ptr<Slider>			slider_frame;
	std::unique_ptr<TreeView>		animeTreeView;
	std::unique_ptr<PropertyPanel>	propertyPanel;
	Component::SafePointer<ColourSelectorWindow>	colourSelectorWindow;
	// レイアウト
	std::unique_ptr<Component>		controlPanel;
	std::unique_ptr<ConcertinaPanel>sidePanel;
	// 最近開いたファイルリスト
	RecentlyOpenedFilesList			recentlyOpenedFilesList;
	// ビューの状態
	State		state;
	ViewerTreeViewItem *			rootItem;

	bool			sliderShouldRebuild(const int min, const int max);
	void			buildSlider();
	void			buildTreeView();
	void			buildPropertyPanel();
	ValueTree		createTree();
	ValueTree		createTreeItem(const String & name, int _packIndex, int _animeIndex);
    //==============================================================================
	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(ViewerMainWindow)
};

class ViewerTreeViewItem :	public TreeViewItem,
							public Component
{
public:
	ViewerTreeViewItem(ValueTree t);
	~ViewerTreeViewItem();
	bool	mightContainSubItems() override;
	void	paintItem(Graphics& g, int width, int height) override;
	void	itemOpennessChanged(bool isNowOpen) override;
	void	itemClicked(const MouseEvent & e) override;
	int		getPackIndex();
	int		getAnimeIndex();
private:
	ValueTree	tree;
	void		refreshSubItems();
	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(ViewerTreeViewItem)
};

class ColourSelectorWindow :	public DocumentWindow
{
public:
	ColourSelectorWindow(const String & name, Colour backgroundColour, int buttonsNeeded);
	void closeButtonPressed() override;
private:
	std::unique_ptr<ColourSelector> selector;
	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(ColourSelectorWindow)
};

