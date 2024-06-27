#include <stdio.h>
#include <cstdlib>
#include <iostream>

#include "stb_image.h"
#include "ssHelper.h"
#include "sshTextureBMP.h"

namespace spritestudio6 {

bool SSTextureBMP::Load(const char* fname) {
    int bpp;

    m_filename = "";

    SSTextureLoader::DataHandle image = SSTextureLoader::LoadImageFromFile(fname, &tex_width, &tex_height, &bpp);
    if (image == SSTextureLoader::InvalidDataHandle) {
        const char* msg = SSTextureLoader::MessageGetFailureLoadFromFile();
        if (msg) {
            std::cerr << fname << " err:" << msg << "\n";
        }
        return false;
    }

    m_filename = fname;

    SSTextureLoader::DecodeEndImageFile(image);
    return true;
}

}  // namespace spritestudio6
