#ifndef __SAMPLESCENE__
#define __SAMPLESCENE__

#include "ssHelper.h"
//#include "./player/ssplayer_animedecode.h"

class SsAnimeDecoder;
class SsCellMapList;


///サンプル用のシーンクラス　
class SampleScene : public tkScene
{
public:
	struct AnimePackSelecterType;
	struct SequencePackSelecterType;
	bool		isLoop;


private:

	int		sceneLoop;

	SsAnimeDecoder*	m_player;
	SsCellMapList*	m_cellmap;


	SsProject*	m_proj;
	int			m_select_animepack_index;
	int			m_select_anime_index;

	std::vector<AnimePackSelecterType*>	AnimePackSelecter;
	std::vector<SequencePackSelecterType*>	SequencePackSelecter;
	SsVector2	m_animeOffsetpos;
	int			m_nowPlayFrame;
	double		m_nowPlayFrameD;

	bool		m_isAnimeAutoPlay;

	//UIへの情報表示用
	int			m_InfoAnimeStartFrame;
	int			m_InfoAnimeEndFrame;
	int			m_InfoAnimeTotalFrame;
	int			m_InfoAnimeFps;

	float		m_Zoom;
	float		m_Speed;

	bool		m_isLoading;
	bool		m_framereset;
	bool		m_isSequence;
	int			m_nowPlaySequenceIndex;
	int			m_nowPlaySequenceRepeat;

public:
	SampleScene() : m_isAnimeAutoPlay (false),sceneLoop(0) , m_isLoading(false),m_framereset(false){}
	virtual ~SampleScene()
	{
		destroy();
	}

	virtual void	draw();
	virtual void	update(double delta);
	virtual void	init();
	virtual void	destroy();

	void	ProjectFileLoad();

	void	UIRebuild();

	void	AnimePackSelecterRelease();
	void	SequencePackSelecterRelease();
	void	ChangeAnimation( int packIndex , int animeIndex );
	void	ChangeSequence( int packIndex , int sequenceIndex );


	void	AnimePlay(){ m_isAnimeAutoPlay = true; }
	void	AnimePause(){ m_isAnimeAutoPlay = false; }
	void	AnimeReset();

};


#endif
