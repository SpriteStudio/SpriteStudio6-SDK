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
		if (m_myInst->textureCache.count(filePath) == 0)
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


void	SSTextureFactory::releaseTexture(ISSTexture* tex)
{

	int ret = tex->release();
	if (ret == 0)
	{
		delete tex;
		m_myInst->textureCache[tex->getFilename()] = 0;
		DEBUG_PRINTF("Release Texture refCount == 0  Deleted : %s \n", tex->getFilename());
	}
	else {
		DEBUG_PRINTF("Release Texture sub refCount : %s \n", tex->getFilename());
	}

}
