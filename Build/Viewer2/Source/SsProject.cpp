/*
  ==============================================================================

    SsProject.cpp
    Created: 24 Jan 2018 4:23:01pm
    Author:  kurooka

  ==============================================================================
*/
#include "ssloader.h"

#include "SsProject.h"
#include "AnimationController.h"



SsDataHolder* SsDataHolder::myInst = 0;

SsDataHolder::SsDataHolder() :project (0)
{
	myInst = this;
}

SsDataHolder::~SsDataHolder()
{
	if (project)delete project;
	project = 0;

}

bool	SsDataHolder::loadSspj(const char* fname)
{

	try {
		project = ssloader_sspj::Load(fname);

		if (project)
		{
//			Log("sspj file Load Succeed");
		}
		else {
//			Log("sspj file Load Error");
			return 0;
		}
	}
	catch (...)
	{
//		Log("sspj file Load Error");
	}


	SSAnimationController::get()->loadListener(project);


	return (project!=0);
}

