#pragma once
#include "../JuceLibraryCode/JuceHeader.h"

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

	void setBackGroundColour(const Colour & colour);

private:
	Colour	backGroundColour;
	//==============================================================================
	// private member variables

	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(DocumentView3D)
};
