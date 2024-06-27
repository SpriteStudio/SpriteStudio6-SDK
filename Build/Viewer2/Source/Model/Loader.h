#pragma once
#include "../JuceLibraryCode/JuceHeader.h"

class AsyncAnimeLoader : public ThreadWithProgressWindow {
   private:
    int packIndex;
    int animeIndex;

   public:
    AsyncAnimeLoader();

    void run() override;

    void threadComplete(bool userPressedCancel) override;

    void setAnimeIndex(int _packIndex, int _animeIndex);
    JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(AsyncAnimeLoader)
};

class AsyncProjectLoader : public ThreadWithProgressWindow {
   private:
    String projectName;

   public:
    AsyncProjectLoader();

    void run() override;

    void threadComplete(bool userPressedCancel) override;

    void setProjectName(const String& name) { projectName = name; }
    JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(AsyncProjectLoader)
};
