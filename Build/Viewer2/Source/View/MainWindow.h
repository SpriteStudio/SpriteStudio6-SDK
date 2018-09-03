#pragma once
#include "../JuceLibraryCode/JuceHeader.h"
#include "ssHelper.h"

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
	void		buildControlPanel();
	void		buildSidePanel();
	void		buildGL();
	void		openColourSelectorWindow();
	void		setBackGroundColour(const Colour & c);
	struct State
	{
		Value	frame;
		Value	startFrame;
		Value	endFrame;
		Value	length;
		Value	fps;
		Value	loop;
	};
	State *	getState();
    //==============================================================================
private:
	friend class WidgetCreator;
	static ViewerMainWindow *	myInst;
	// メニューバー
	ScopedPointer<MenuBarComponent>	menuBar;
	// OpenGL描画領域
	ScopedPointer<DocumentView3D>	opengl;
	// ウィジェット
	ScopedPointer<TextButton>		button_start;
	ScopedPointer<TextButton>		button_stop;
	ScopedPointer<TextButton>		button_reset;
	ScopedPointer<TextButton>		button_loop;
	ScopedPointer<Slider>			slider_frame;
	ScopedPointer<TreeView>			animeTreeView;
	ScopedPointer<PropertyPanel>	propertyPanel;
	Component::SafePointer<ColourSelectorWindow>	colourSelectorWindow;
	// レイアウト
	ScopedPointer<Component>		controlPanel;
	ScopedPointer<ConcertinaPanel>	sidePanel;
	// ビューの状態
	State		state;
	ScopedPointer<ViewerTreeViewItem>	rootItem;

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
	String	getUniqueName() const override;
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
	ScopedPointer<ColourSelector> selector;
	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(ColourSelectorWindow)
};

