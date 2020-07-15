#include "../Loader/sstypes.h"
#include "IsshTexture.h"
#include "../Helper/DebugPrint.h"


ISSTexture*	SSTextureFactory::m_texture_base_class = 0;
SSTextureFactory*	SSTextureFactory::m_myInst = 0;

ISSTexture*	SSTextureFactory::loadTexture(SsString filePath)
{
	if (m_myInst)
	{
		ISSTexture* _tex = 0;
		bool cached = false;
		if (m_myInst->textureCache.count(filePath) != 0)
		{
			if (m_myInst->textureCache[filePath] != 0)cached = true;
		}


		if (!cached)
		{
			//新規
			DEBUG_PRINTF("New Load Texture : %s \n", filePath.c_str());
			_tex = m_myInst->create();
			_tex->filenamepath = filePath;
 
			_tex->Load(filePath.c_str());
			if (!_tex)
			{
				delete _tex;
				return 0;
			}
			m_myInst->textureCache[filePath] = _tex;
			return _tex;
		}
		else {
			ISSTexture* _tex2 = m_myInst->textureCache[filePath];
			if (_tex2)
			{
				DEBUG_PRINTF("Texture Cached : %s \n", filePath.c_str());
				_tex2->addref();
				return _tex2;
			}
		}
	}
	return 0;
}

void	SSTextureFactory::releaseTextureForced(SsString filePath)
{
	if (m_myInst == 0)return;

	if (SSTextureFactory::isExist(filePath))
	{
		ISSTexture* tex = m_myInst->textureCache[filePath];
		m_myInst->textureCache[filePath] = 0;
		delete tex;
	}
}


void SSTextureFactory::releaseTextureForced(ISSTexture* tex)
{
	if (m_myInst == 0)return;

	if (SSTextureFactory::isExist(tex))
	{
		m_myInst->textureCache[tex->getFilename()] = 0;
		DEBUG_PRINTF("Release Texture refCount == 0  Deleted : %s \n", tex->getFilename());
		delete tex;
	}
}
void	SSTextureFactory::releaseTexture(ISSTexture* tex)
{
	if (m_myInst == 0)return;

	if (SSTextureFactory::isExist(tex))
	{
		int ret = tex->release();
		if (ret == 0)
		{
			m_myInst->textureCache[tex->getFilename()] = 0;
			DEBUG_PRINTF("Release Texture refCount == 0  Deleted : %s \n", tex->getFilename());
			delete tex;
		}
		else {
			DEBUG_PRINTF("Release Texture sub refCount : %s \n", tex->getFilename());
		}
	}else{
		DEBUG_PRINTF("The specified texture is not under management. \n");
	}
}


bool	SSTextureFactory::isExist(SsString filePath)
{
	if (m_myInst == 0)return false;

	if (m_myInst->textureCache.count(filePath) != 0) return true;
	return false;
}

bool	SSTextureFactory::isExist(ISSTexture* texture)
{
	if (m_myInst == 0)return false;

	for (auto i = m_myInst->textureCache.begin(); i != m_myInst->textureCache.end(); ++i) {
		//std::cout << i->first << " " << i->second << "\n";
		if (i->second == texture) {
			return true;
		}
	}

	return false;
}

void SSTextureFactory::releaseAllTexture()
{
	if (m_myInst == 0)return;

	for (auto i = m_myInst->textureCache.begin(); i != m_myInst->textureCache.end(); ++i) {
		delete i->second;
	}
	m_myInst->textureCache.clear();
}
