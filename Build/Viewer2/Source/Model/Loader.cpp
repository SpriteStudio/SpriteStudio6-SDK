#include "Loader.h"
#include "Player.h"
#include "View/MainWindow.h"
#include "Controller/MainComponent.h"

AsyncAnimeLoader::AsyncAnimeLoader()
	: ThreadWithProgressWindow("Loading...", true, false)
{
	setStatusMessage("Preparing...");
}

void AsyncAnimeLoader::run()
{
	auto* p = Player::get();
	p->changeState(p->stateLoading);

	setProgress(-1.0);
	setStatusMessage("Analysing animation...");

	p->setAnime(packIndex, animeIndex);

	wait(100);
}

void AsyncAnimeLoader::threadComplete(bool userPressedCancel)
{
	auto* pM = Player::get();
	auto* pC = MainContentComponent::get();

	int startFrame = static_cast<int>(pM->decoder->getAnimeStartFrame());
	int endFrame = static_cast<int>(pM->decoder->getAnimeEndFrame());
	int fps = static_cast<int>(pM->decoder->getAnimeFPS());

	SsAnimePackList alist = pM->currentProj->getAnimePackList();
	SsAnimePack * animePack = alist[packIndex];
	SsAnimation * anime = animePack->animeList[animeIndex];
	String animeName = anime->name.c_str();

	pM->currentState->packIndex = packIndex;
	pM->currentState->animeIndex = animeIndex;
	pM->getState()->animeName = animeName;
	pM->getState()->length = endFrame;
	pM->getState()->startFrame = startFrame;
	pM->getState()->frame = startFrame;
	pM->getState()->endFrame = endFrame;
	pM->getState()->fps = fps;

	pM->changeState(pM->statePaused);

	pC->repaint();
	delete this;
}

void AsyncAnimeLoader::setAnimeIndex(int _packIndex, int _animeIndex)
{
	packIndex = _packIndex;
	animeIndex = _animeIndex;
}

AsyncProjectLoader::AsyncProjectLoader()
	: ThreadWithProgressWindow("Loading...", true, false)
{
	setStatusMessage("Preparing...");
}

void AsyncProjectLoader::run()
{
	auto* p = Player::get();
	p->changeState(p->stateLoading);

	setProgress(-1.0);

	// 文字コード変換
	std::string fileName = babel::auto_translate<>(projectName.toStdString());

	setStatusMessage("Analysing project...");
	auto * proj = ssloader_sspj::Load(fileName);

	if (!proj)
	{
		AlertWindow::showMessageBoxAsync(
			AlertWindow::InfoIcon,
			"File Open Failed",
			"File Open Failed : " + projectName);

		return;
	}

	if (proj->getAnimePackList().size() == 0)
	{
		return;
	}

	p->unloadTexture();
	p->currentProj = proj;
	p->preloadTexture();

	// 最近開いたファイルリストに追加
	ViewerMainWindow::get()->addRecentlyOpenedFile(File(fileName));

	wait(100);
}

void AsyncProjectLoader::threadComplete(bool userPressedCancel)
{
	auto* pM = Player::get();
	auto* pV = ViewerMainWindow::get();
	auto* pC = MainContentComponent::get();

	pV->buildSidePanel();
	pV->resized();

	pM->changeState(pM->stateInitial);

	pC->repaint();
	delete this;
}
