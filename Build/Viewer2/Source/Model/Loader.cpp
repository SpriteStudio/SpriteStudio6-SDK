#include "ssplayer_animedecode.h"
#include "Loader.h"
#include "Player.h"
#include "View/MainWindow.h"
#include "Controller/MainComponent.h"
#include "sscharconverter.h"

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
		SsAnimePackList & alist = p->currentProj->getAnimePackList();
		SsAnimePack * animePack = alist[packIndex];
		SsAnimation * anime = animePack->animeList[animeIndex];
		SsModel* model = &animePack->Model;

		p->decoder.reset(new SsAnimeDecoder());
		p->cellmap = new SsCellMapList();
		p->cellmap->set(p->currentProj.get(), animePack);
		p->decoder->setAnimation(model, anime, p->cellmap, p->currentProj.get());

		int startFrame = static_cast<int>(p->decoder->getAnimeStartFrame());
		int endFrame = static_cast<int>(p->decoder->getAnimeEndFrame());
		int loopFrame = static_cast<int>(p->decoder->getAnimeLoopFrame());
		int fps = static_cast<int>(p->decoder->getAnimeFPS());
		String animeName = anime->name;

		p->currentState->animepackIndex = packIndex;
		p->currentState->animeIndex = animeIndex;
		p->getState()->animeName = animeName;
		p->getState()->length = endFrame;
		p->getState()->startFrame = startFrame;
		p->getState()->frame = startFrame;
		p->getState()->endFrame = endFrame;
		p->getState()->loopFrame = loopFrame;
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

AsyncSequenceLoader::AsyncSequenceLoader()
	: ThreadWithProgressWindow("Loading...", true, false)
{
	setStatusMessage("Preparing...");
}

void AsyncSequenceLoader::run()
{
	auto* p = Player::get();

	setProgress(-1.0);
	setStatusMessage("Analysing sequence...");

	// GLのスレッドにリクエスト
	ViewerMainWindow::get()->getOpenGLContext().executeOnGLThread([&](OpenGLContext& openGLContext)
	{
		p->changeState(p->stateLoading.get());
		SsSequencePackList & slist = p->currentProj->getSequencePackList();
		SsSequencePack * sequencePack = slist[packIndex];
		SsSequence * sequence = sequencePack->sequenceList[sequenceIndex];
		SsSequenceDecoder * decoder = new SsSequenceDecoder();

		p->decoder.reset(decoder);
		decoder->setSequence(sequence, p->currentProj.get());

		int startFrame = static_cast<int>(decoder->getAnimeStartFrame());
		int endFrame = static_cast<int>(decoder->getAnimeEndFrame());
		int loopFrame = static_cast<int>(decoder->getAnimeLoopFrame());
		int fps = static_cast<int>(decoder->getAnimeFPS());
		String sequenceName = sequence->name;

		p->currentState->sequencepackIndex = packIndex;
		p->currentState->sequenceIndex = sequenceIndex;
		p->getState()->sequenceName = sequenceName;
		p->getState()->length = endFrame;
		p->getState()->startFrame = startFrame;
		p->getState()->frame = startFrame;
		p->getState()->endFrame = endFrame;
		p->getState()->loopFrame = loopFrame;
		p->getState()->fps = fps;

		p->changeState(p->statePaused.get());
	}, true);

	wait(100);
}

void AsyncSequenceLoader::threadComplete(bool userPressedCancel)
{
	auto* pC = MainContentComponent::get();

	pC->repaint();
	delete this;
}

void AsyncSequenceLoader::setSequenceIndex(int _packIndex, int _sequenceIndex)
{
	packIndex = _packIndex;
	sequenceIndex = _sequenceIndex;
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

		std::string fileName = SsCharConverter::convert_path_string(projectName.toStdString());

		SsProject* proj = ssloader_sspj::Load(fileName);

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
