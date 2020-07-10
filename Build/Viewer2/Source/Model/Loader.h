#pragma once
#include "../JuceLibraryCode/JuceHeader.h"

class AsyncAnimeLoader : public ThreadWithProgressWindow
{
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

class AsyncSequenceLoader : public ThreadWithProgressWindow
{
private:
	int packIndex;
	int sequenceIndex;
public:
	AsyncSequenceLoader();

	void run() override;

	void threadComplete(bool userPressedCancel) override;

	void setSequenceIndex(int _packIndex, int _sequenceIndex);
	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(AsyncSequenceLoader)
};

class AsyncProjectLoader : public ThreadWithProgressWindow
{
private:
	String	projectName;
public:
	AsyncProjectLoader();

	void run() override;

	void threadComplete(bool userPressedCancel) override;

	void setProjectName(const String & name) { projectName = name; }
	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(AsyncProjectLoader)
};
