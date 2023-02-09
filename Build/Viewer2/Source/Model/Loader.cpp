
#include "ssplayer_animedecode.h"

#include "Loader.h"
#include "Player.h"
#include "View/MainWindow.h"
#include "Controller/MainComponent.h"
#include "sscharconverter.h"

//#include "ssloader.h"
//#include "ssplayer_animedecode.h"

AsyncAnimeLoader::AsyncAnimeLoader()
	: ThreadWithProgressWindow("Loading...", true, false)
{
	setStatusMessage("Preparing...");
}

void AsyncAnimeLoader::run()
{
	auto* p = Player::get();

	setProgress(-1.0);
	setStatusMessage("Analysing animation...");

	// GLのスレッドにリクエスト
	ViewerMainWindow::get()->getOpenGLContext().executeOnGLThread([&](OpenGLContext& openGLContext)
	{
		p->changeState(p->stateLoading.get());
		SpriteStudio::SsAnimePackList & alist = p->currentProj->getAnimePackList();
		SpriteStudio::SsAnimePack * animePack = alist[packIndex].get();
		SpriteStudio::SsAnimation * anime = animePack->animeList[animeIndex];
		SpriteStudio::SsModel* model = &animePack->Model;

		p->decoder.reset(new SpriteStudio::SsAnimeDecoder());
		p->cellmap = new SpriteStudio::SsCellMapList();
		p->cellmap->set(p->currentProj.get(), animePack);

		p->fonttextureMng = new SpriteStudio::FontTextureManager(p->currentProj.get());

		p->decoder->setFontManager(p->fonttextureMng);
		p->decoder->setAnimation(model, anime, p->cellmap, p->currentProj.get());

		int startFrame = static_cast<int>(p->decoder->getAnimeStartFrame());
		int endFrame = static_cast<int>(p->decoder->getAnimeEndFrame());
		int fps = static_cast<int>(p->decoder->getAnimeFPS());
		String animeName = anime->name;

		p->currentState->packIndex = packIndex;
		p->currentState->animeIndex = animeIndex;
		p->getState()->animeName = animeName;
		p->getState()->length = endFrame;
		p->getState()->startFrame = startFrame;
		p->getState()->frame = startFrame;
		p->getState()->endFrame = endFrame;
		p->getState()->fps = fps;

		p->changeState(p->statePaused.get());
	}, true);

	wait(100);
}

void AsyncAnimeLoader::threadComplete(bool userPressedCancel)
{
	auto* pC = MainContentComponent::get();

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

	setProgress(-1.0);
	setStatusMessage("Analysing project...");

	// GLのスレッドにリクエスト
	ViewerMainWindow::get()->getOpenGLContext().executeOnGLThread([&](OpenGLContext& openGLContext)
	{
		p->changeState(p->stateLoading.get());

//		std::string fileName = SpriteStudio::SsCharConverter::convert_path_string(projectName.toStdString());
		std::string fileName = projectName.toStdString();

		SpriteStudio::SsProject* proj = SpriteStudio::ssloader_sspj::Load(fileName);

		if (!proj)
		{
			AlertWindow::showMessageBoxAsync(
				AlertWindow::InfoIcon,
				"File Open Failed",
				"File Open Failed : " + projectName);
		}
		else
		if (proj->getAnimePackList().size() == 0)
		{
			delete proj;
		}
		else
		{
			p->currentProj.reset(proj);

			// 最近開いたファイルリストに追加
			ViewerMainWindow::get()->addRecentlyOpenedFile(File(projectName));
		}

		p->changeState(p->stateInitial.get());
	}, true);

	wait(100);
}

void AsyncProjectLoader::threadComplete(bool userPressedCancel)
{
	auto* pV = ViewerMainWindow::get();
	auto* pC = MainContentComponent::get();

	pV->buildSidePanel();
	pV->resized();

	pC->repaint();
	delete this;
}
