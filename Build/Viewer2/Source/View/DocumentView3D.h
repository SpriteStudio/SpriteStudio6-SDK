#pragma once
#include "../JuceLibraryCode/JuceHeader.h"

class SSTextureFactory;
class SsRenderGL;

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

	virtual void mouseDown(const MouseEvent& event);
	virtual void mouseMove(const MouseEvent& event);
	virtual void mouseUp(const MouseEvent& event);
	virtual void mouseDrag(const MouseEvent& event);
	virtual void mouseWheelMove(const MouseEvent& event, const MouseWheelDetails& wheel);

	Value	view_camera_x;
	Value	view_camera_y;
	Value	view_camera_scale;

private:
	std::unique_ptr<SSTextureFactory>	texfactory;
	std::unique_ptr<SsRenderGL>			rendererGL;
	Colour	backGroundColour;
	//==============================================================================
	// private member variables


	float	view_camera_org_x;
	float	view_camera_org_y;

	JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(DocumentView3D)
};
