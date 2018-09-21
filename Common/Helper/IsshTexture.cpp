#include "sstypes.h"
#include "IsshTexture.h"
#include "../Helper/DebugPrint.h"


ISSTexture*	SSTextureFactory::m_texture_base_class = 0;
SSTextureFactory*	SSTextureFactory::m_myInst = 0;

ISSTexture*	SSTextureFactory::loadTexture(SsString filePath)
{
	if (m_myInst)
	{
		ISSTexture* tex = 0;
		bool cached = false;
		if (m_myInst->textureCache.count(filePath) != 0)
		{
			if (m_myInst->textureCache[filePath] != 0)cached = true;
		}


		if (!cached)
		{
			//新規
			DEBUG_PRINTF("New Load Texture : %s \n", filePath.c_str());
			tex = m_myInst->create();
			tex->filenamepath = filePath;
 
			tex->Load(filePath.c_str());
			if (!tex)
			{
				delete tex;
				return 0;
			}
			m_myInst->textureCache[filePath] = tex;
			return tex;
		}
		else {
			ISSTexture* tex = m_myInst->textureCache[filePath];
			if (tex)
			{
				DEBUG_PRINTF("Texture Cached : %s \n", filePath.c_str());
				tex->addref();
				return tex;
			}
		}
	}
	return 0;
}

void	SSTextureFactory::releaseTextureForced(SsString filePath)
{
	if (SSTextureFactory::isExist(filePath))
	{
		ISSTexture* tex = m_myInst->textureCache[filePath];
		m_myInst->textureCache[filePath] = 0;
		delete tex;
	}
}


void SSTextureFactory::releaseTextureForced(ISSTexture* tex)
{
	if (SSTextureFactory::isExist(tex))
	{
		m_myInst->textureCache[tex->getFilename()] = 0;
		DEBUG_PRINTF("Release Texture refCount == 0  Deleted : %s \n", tex->getFilename());
		delete tex;
	}
}
void	SSTextureFactory::releaseTexture(ISSTexture* tex)
{

	int ret = tex->release();
	if (ret == 0)
	{
		if (SSTextureFactory::isExist(tex))
		{
			m_myInst->textureCache[tex->getFilename()] = 0;
			DEBUG_PRINTF("Release Texture refCount == 0  Deleted : %s \n", tex->getFilename());
			delete tex;
		}
		else {
			DEBUG_PRINTF("The specified texture is not under management. \n");
		}
	}
	else {
		DEBUG_PRINTF("Release Texture sub refCount : %s \n", tex->getFilename());
	}

}


bool	SSTextureFactory::isExist(SsString filePath)
{
	if (m_myInst->textureCache.count(filePath) != 0) return true;
	return false;
}

bool	SSTextureFactory::isExist(ISSTexture* texture)
{
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
	for (auto i = m_myInst->textureCache.begin(); i != m_myInst->textureCache.end(); ++i) {
		delete i->second;
	}
	m_myInst->textureCache.clear();
}
