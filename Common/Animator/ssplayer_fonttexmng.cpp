#include "ssplayer_fonttexmng.h"
#include "sstypes.h"
#include "../Loader/ssloader_sspj.h"


//#include "../Helper/IsshTexture.h"
#include "../Helper/DebugPrint.h"
#include "../Helper/SsRawImage.h"

namespace SpriteStudio
{
	FontTextureManager::FontTextureManager()
	{

	}

	FontTextureManager::FontTextureManager(SsProject* myproject)
	{
		loadTextureFromProject(myproject);
	}

	FontTextureManager::~FontTextureManager()
	{
		releaseRawImageAll();
	}

	std::pair<SsRawImage*, SsFontTexture*> FontTextureManager::findShareTexture(const SsString& text, const SsFontDesc& desc, const SsSize& sizeGrid, const SsSize& sizeFixed)
	{
		for (auto e : m_part_textures)
		{
			if (e.compare(text, desc, sizeGrid, sizeFixed))
			{
				return std::make_pair(e.image, e.fontTexture);
			}
		}

		ShareTextureDesc sharedesc;
		sharedesc.text = text;
		sharedesc.desc = desc;
		sharedesc.sizeGrid = sizeGrid;
		sharedesc.sizeFixed = sizeFixed;

		SsRawImage* image = new SsRawImage();
		image->create(sizeFixed.width(), sizeFixed.height());

		sharedesc.image = new SsRawImage();
		sharedesc.fontTexture = new SsFontTexture();
		//sharedesc.fontTexture->setProject(myproject);

		m_part_textures.push_back(sharedesc);

		return std::make_pair(sharedesc.image, sharedesc.fontTexture);
	}


	//RAWイメージを保持しておく
	//RAWイメージからパーツごとのテクスチャを生成するための素材としてキャッシュする
	bool FontTextureManager::loadTextureFromProject(SsProject* pj)
	{

		for (auto& item : pj->charmapDic)
		{
			SsCharMap* c = item.second.get();
			const std::list<SsCharMapPage*> pages = c->getCharMapPages();
			for (auto n : pages)
			{
				int width, height, bpp;

				SsString loadfile = n->getFilenamePath();

				if (m_charamap_images.count(loadfile) > 0)
				{
					DEBUG_PRINTF("Font RawImage Exist : %s \n", loadfile.c_str());

					n->imagePtr = m_charamap_images[loadfile].imgptr;
				}
				else {

					/*
					SSTextureLoader::DataHandle ptr = SSTextureLoader::LoadImageFromFile(loadfile.c_str(),
						&width,
						&height,
						&bpp
					);
					*/
					SsRawImage* image = new SsRawImage();
					bool ret = image->Load(loadfile.c_str());

					if (!ret)
					{
						DEBUG_PRINTF("Texture Load Failed : %s \n", loadfile.c_str());
					}

					CharaMapImage imageInfo;
					imageInfo.bpp = image->getBpp();
					imageInfo.width = image->getWidth();
					imageInfo.height = image->getHeight();
					imageInfo.imgptr = image;

					n->imagePtr = image;
					m_charamap_images[loadfile] = imageInfo;
					DEBUG_PRINTF("Font RawImage cached : %s \n", loadfile.c_str());
				}
			}
		}

		return true;
	}

	void FontTextureManager::releaseRawImageAll()
	{
		for (auto i : m_charamap_images)
		{
			//SSTextureLoader::DecodeEndImageFile(i.second.imgptr);//free
			delete i.second.imgptr;
		}
		m_charamap_images.clear();
	}

	void FontTextureManager::releasePartsTexture()
	{

	}


	FontTextureManager::CharaMapImage* FontTextureManager::getCharaMapImage(SsString name)
	{
		return 0;
	}


};