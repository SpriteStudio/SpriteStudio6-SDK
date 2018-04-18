/*
  ==============================================================================

    This file was auto-generated!

  ==============================================================================
*/

#pragma once

#include "../JuceLibraryCode/JuceHeader.h"
#include "SsProject.h"




class DocumentView3D : public OpenGLAppComponent
{
public:
	//==============================================================================
	DocumentView3D();

	~DocumentView3D();

	void initialise() override;

	void shutdown() override;

	void render() override;

	void paint(Graphics&) override;

	void resized() override;

private:
	//==============================================================================
	// private member variables


	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(DocumentView3D)
};



//==============================================================================
/*
    This component lives inside our window, and this is where you should put all
    your controls and content.
*/
class MainContentComponent   : public Component,
								public MenuBarModel, //,
								public ApplicationCommandTarget
{
public:
    //==============================================================================
    MainContentComponent();
    ~MainContentComponent();

    void paint (Graphics&) override;
    void resized() override;

    
	StringArray getMenuBarNames() override
	{
		return { "File" };
	}

	PopupMenu getMenuForIndex(int menuIndex, const String& /*menuName*/) override
	{
		PopupMenu menu;

		if (menuIndex == 0)
		{
			//menu.addCommandItem();

			menu.addItem(3000, "Open", true, false);
			menu.addSeparator();
			menu.addItem(3003, "Exit", true, false);
		}
		else {
		}

		return menu;
	}
	void onFullScreenModeItemTriggered() 
	{
		if (getPeer())
		{
			getPeer()->setFullScreen(true);
		}
		Desktop::getInstance().setKioskModeComponent(getTopLevelComponent(), false);
	}

	void menuItemSelected(int menuItemID, int /*topLevelMenuIndex*/) override
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

					SsDataHolder::get()->loadSspj(fn.getCharPointer());

				}
			}
			break;
		}

	}    
   
	ApplicationCommandTarget* getNextCommandTarget() override
	{
		return findFirstTargetParentComponent();
	}
	void getCommandInfo(CommandID commandID, ApplicationCommandInfo& result)
	{
	}

	void getAllCommands(Array<CommandID>& commands) override
	{
	}

	bool perform(const InvocationInfo& info) override
	{
		return true;
	}

private:

	ScopedPointer<MenuBarComponent> menuBar;
	ScopedPointer<DocumentView3D> opengl;


    //==============================================================================
    JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR (MainContentComponent)
};



