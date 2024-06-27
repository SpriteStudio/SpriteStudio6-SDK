/*
  ==============================================================================

    This file was auto-generated!

  ==============================================================================
*/

#pragma once

#include "../JuceLibraryCode/JuceHeader.h"

class AnimeState;

enum CommandIDs {
    OPEN = 1,
    START,
    STOP,
    RESET,
    LOAD_ANIME,
    EXIT,
};

//==============================================================================
/*
    This component lives inside our window, and this is where you should put all
    your controls and content.
*/

// コマンド処理を行うコンポーネント
class MainContentComponent : public Component,
                             public ApplicationCommandTarget,
                             public Value::Listener,
                             public FileDragAndDropTarget {
   public:
    //==============================================================================
    MainContentComponent();
    ~MainContentComponent();

    static MainContentComponent* get();
    ApplicationCommandTarget* getNextCommandTarget() override;
    void getCommandInfo(CommandID commandID, ApplicationCommandInfo& result) override;
    void getAllCommands(Array<CommandID>& c) override;
    bool perform(const InvocationInfo& info) override;
    void valueChanged(Value& value) override;
    void resized() override;

    // FileDragAndDropTarget interface
    bool isInterestedInFileDrag(const juce::StringArray& files) override;
    void filesDropped(const juce::StringArray& files, int x, int y) override;

    PropertiesFile& getProperties();
    ApplicationCommandManager commandManager;

   private:
    static MainContentComponent* myInst;
    // 設定ファイル
    static PropertiesFile::Options getPropertyFileOptions() {
        PropertiesFile::Options o;
        o.applicationName = "SpriteStudio_Viewer2";
        o.filenameSuffix = ".settings";
        o.folderName = "SpriteStudio_Viewer2";
        o.osxLibrarySubFolder = "Application Support/SpriteStudio_Viewer2";
        o.millisecondsBeforeSaving = 2000;
        return o;
    }
    PropertiesFile properties;
    //==============================================================================
    JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(MainContentComponent)
};
