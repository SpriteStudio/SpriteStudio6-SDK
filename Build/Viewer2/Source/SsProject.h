/*
  ==============================================================================

    SsProject.h
    Created: 24 Jan 2018 4:23:01pm
    Author:  kurooka

  ==============================================================================
*/

#pragma once

class SsProject;

//プロジェクトfileなどを保持しているクラス


class SsDataHolder
{
private:
	SsProject*	project;



	static SsDataHolder* myInst;

public:
	SsDataHolder();
	virtual ~SsDataHolder();
	bool	loadSspj(const char* fname);

	static SsDataHolder* get(){
		return myInst;
	}

	static SsProject*	getProjectRawData() {
		return SsDataHolder::get()->project;
	}
};




//-----------------------------------------------
inline void createSsDataHolder()
{
	new	SsDataHolder();
}

inline void destroySsDataHolder()
{
	if (SsDataHolder::get())
		delete SsDataHolder::get();

}


void	Log(const char* txt);


