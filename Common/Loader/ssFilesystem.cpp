#include "ssFilesystem.h"

#include "sscharconverter.h"
#if _WIN32 || _WIN64
#include <Windows.h>
#endif

using namespace spritestudio6;

FILE *ssFilesystem::openFile(const std::string &filepath) {
    FILE *fp = NULL;
#if _WIN32 || _WIN64
    int wchars_num = MultiByteToWideChar(CP_UTF8, 0, filepath.c_str(), -1, NULL, 0);
    std::wstring wstr(wchars_num, 0);
    MultiByteToWideChar(CP_UTF8, 0, filepath.c_str(), -1, &wstr[0], wchars_num);

    _wfopen_s(&fp, wstr.c_str(), L"rb");
#else
    fp = fopen( SsCharConverter::convert_path_string(filepath).c_str(), "rb");
#endif
    return fp;
}

