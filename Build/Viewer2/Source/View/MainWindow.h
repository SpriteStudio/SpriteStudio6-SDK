#pragma once
#include "../JuceLibraryCode/JuceHeader.h"

class ViewerTreeViewItem;
class ColourSelectorWindow;
class LicenseWindow;
class DocumentView3D;

enum PackType {
	None = 0,
	Animation,
	Sequence,
};

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
	void		openLicenseWindow();
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
		Value	animepackIndex;
		Value	animeIndex;
		Value	sequencepackIndex;
		Value	sequenceIndex;

		Value	camera_x;
		Value	camera_y;
		Value	camera_scale;
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

	std::unique_ptr<Slider>			slider_viewcamera_x;
	std::unique_ptr<Slider>			slider_viewcamera_y;
	std::unique_ptr<Slider>			slider_viewcamera_scale;


	SafePointer<ColourSelectorWindow>	colourSelectorWindow;
	SafePointer<LicenseWindow>		licenseWindow;
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
	ValueTree		createTreeItem(const String & name, PackType _packType, int _packIndex, int _itemIndex);
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
	PackType	getPackType();
	int		getPackIndex();
	int		getItemIndex();
private:
	ValueTree	tree;
	void		refreshSubItems();
	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(ViewerTreeViewItem)
};

class ColourSelectorWindow :	public DocumentWindow
{
public:
	ColourSelectorWindow(const String & name, const Colour & colour, int buttonsNeeded);
	void closeButtonPressed() override;
private:
	std::unique_ptr<ColourSelector> colourSelector;
	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(ColourSelectorWindow)
};

class LicenseWindow :	public DocumentWindow
{
public:
	LicenseWindow(const String & name, const Colour & colour, int buttonsNeeded);
	void closeButtonPressed() override;
private:
	std::unique_ptr<Label>				copyright;
	std::unique_ptr<TextEditor>			license;
	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(LicenseWindow)
};