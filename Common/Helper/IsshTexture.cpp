#include "../Loader/sstypes.h"
#include "IsshTexture.h"
#include "../Helper/DebugPrint.h"

#ifndef _NOTUSE_STBI
#include "stb_image.h"
#endif

namespace spritestudio6 {

static SSTextureLoader::DataHandle defaultLoadImageFromFile(const char* fileName, int* width, int* height, int* bpp) {
#ifdef _NOTUSE_STBI
    void* image = 0;
#else
    stbi_uc* image = stbi_load(fileName, width, height, bpp, 0);
#endif
    if (image == nullptr) {  // エラー時処理
        return SSTextureLoader::InvalidDataHandle;
    }

    return (SSTextureLoader::DataHandle)image;
}
static void defaultDecodeEndImageFile(SSTextureLoader::DataHandle handle) {
    if (handle != SSTextureLoader::InvalidDataHandle) {
#ifndef _NOTUSE_STBI
        stbi_image_free((void*)handle);
#endif
    }
}
static const char* defaultMessageGetFailureLoadFromFile() {
#ifdef _NOTUSE_STBI
    return 0;
#else
    return (stbi_failure_reason());

#endif
}
static bool defaultCheckSizePow2(int width, int height) {
    bool rv = true;

    if (width > 0) rv &= SsUtTextureisPow2(width);
    if (height > 0) rv &= SsUtTextureisPow2(height);

    return rv;
}

SSTextureLoader::DataHandle SSTextureLoader::InvalidDataHandle = nullptr;
SSTextureLoader::PrototypeLoadImageFromFile SSTextureLoader::FunctionLoadImageFromFile = defaultLoadImageFromFile;
SSTextureLoader::PrototypeDecodeEndImageFile SSTextureLoader::FunctionDecodeEndImageFile = defaultDecodeEndImageFile;
SSTextureLoader::PrototypeMessageGetFailureLoadFromFile SSTextureLoader::FunctionMessageGetFailureLoadFromFile = defaultMessageGetFailureLoadFromFile;
SSTextureLoader::PrototypeCheckSizePow2 SSTextureLoader::FunctionCheckSizePow2 = defaultCheckSizePow2;

SSTextureLoader::DataHandle SSTextureLoader::LoadImageFromFile(const char* fileName, int* width, int* height, int* bpp) {
    if (FunctionLoadImageFromFile == nullptr) {
        FunctionLoadImageFromFile = defaultLoadImageFromFile;
    }
    return (FunctionLoadImageFromFile(fileName, width, height, bpp));
}
void SSTextureLoader::DecodeEndImageFile(SSTextureLoader::DataHandle handle) {
    if (FunctionDecodeEndImageFile == nullptr) {
        FunctionDecodeEndImageFile = defaultDecodeEndImageFile;
    }
    FunctionDecodeEndImageFile(handle);
}
const char* SSTextureLoader::MessageGetFailureLoadFromFile() {
    if (FunctionMessageGetFailureLoadFromFile == nullptr) {
        FunctionMessageGetFailureLoadFromFile = defaultMessageGetFailureLoadFromFile;
    }
    return (FunctionMessageGetFailureLoadFromFile());
}
bool SSTextureLoader::CheckSizePow2(int width, int height) {
    if (FunctionCheckSizePow2 == nullptr) {
        FunctionCheckSizePow2 = defaultCheckSizePow2;
    }
    return (FunctionCheckSizePow2(width, height));
}

ISSTexture* SSTextureFactory::m_texture_base_class = 0;
SSTextureFactory* SSTextureFactory::m_myInst = 0;

ISSTexture* SSTextureFactory::loadTexture(SsString filePath) {
    if (m_myInst) {
        ISSTexture* _tex = 0;
        bool cached = false;
        if (m_myInst->textureCache.count(filePath) != 0) {
            if (m_myInst->textureCache[filePath] != 0) cached = true;
        }

        if (!cached) {
            // 新規
            DEBUG_PRINTF("New Load Texture : %s \n", filePath.c_str());
            _tex = m_myInst->create();
            _tex->filenamepath = filePath;

            if (_tex->Load(filePath.c_str()) == false) {
                delete _tex;
                return 0;
            }
            m_myInst->textureCache[filePath] = _tex;
            return _tex;
        } else {
            _tex = m_myInst->textureCache[filePath];
            if (_tex) {
                DEBUG_PRINTF("Texture Cached : %s \n", filePath.c_str());
                _tex->addref();
                return _tex;
            }
        }
    }
    return 0;
}

void SSTextureFactory::releaseTextureForced(SsString filePath) {
    if (m_myInst == 0) return;

    if (SSTextureFactory::isExist(filePath)) {
        ISSTexture* tex = m_myInst->textureCache[filePath];
        m_myInst->textureCache[filePath] = 0;
        delete tex;
    }
}

void SSTextureFactory::releaseTextureForced(ISSTexture* tex) {
    if (m_myInst == 0) return;

    if (SSTextureFactory::isExist(tex)) {
        m_myInst->textureCache[tex->getFilename()] = 0;
        DEBUG_PRINTF("Release Texture refCount == 0  Deleted : %s \n", tex->getFilename());
        delete tex;
    }
}
void SSTextureFactory::releaseTexture(ISSTexture* tex) {
    if (m_myInst == 0) return;

    if (SSTextureFactory::isExist(tex)) {
        int ret = tex->release();
        if (ret == 0) {
            m_myInst->textureCache[tex->getFilename()] = 0;
            DEBUG_PRINTF("Release Texture refCount == 0  Deleted : %s \n", tex->getFilename());
            delete tex;
        } else {
            DEBUG_PRINTF("Release Texture sub refCount : %s \n", tex->getFilename());
        }
    } else {
        DEBUG_PRINTF("The specified texture is not under management. \n");
    }
}

bool SSTextureFactory::isExist(SsString filePath) {
    if (m_myInst == 0) return false;

    if (m_myInst->textureCache.count(filePath) != 0) return true;
    return false;
}

bool SSTextureFactory::isExist(ISSTexture* texture) {
    if (m_myInst == 0) return false;

    for (auto i = m_myInst->textureCache.begin(); i != m_myInst->textureCache.end(); ++i) {
        // std::cout << i->first << " " << i->second << "\n";
        if (i->second == texture) {
            return true;
        }
    }

    return false;
}

void SSTextureFactory::releaseAllTexture() {
    if (m_myInst == 0) return;

    for (auto i = m_myInst->textureCache.begin(); i != m_myInst->textureCache.end(); ++i) {
        delete i->second;
    }
    m_myInst->textureCache.clear();
}

}  // namespace spritestudio6
