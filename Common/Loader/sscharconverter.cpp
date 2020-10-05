#include "sscharconverter.h"

#ifdef __APPLE__
    #include "TargetConditionals.h"
    #ifdef TARGET_OS_MAC
        #include <CoreFoundation/CoreFoundation.h>
    #endif
#elif _WIN32 || _WIN64
    #include <Windows.h>
    #include <vector>
    #include <system_error>
#else

#endif

namespace spritestudio6
{

std::string SsCharConverter::utf8_to_sjis(const std::string &src) {
#if __APPLE__ && TARGET_OS_MAC
    CFStringRef srcStr = CFStringCreateWithBytes(kCFAllocatorDefault, (const UInt8 *) src.c_str(), src.length(), kCFStringEncodingUTF8, FALSE);
    if(!srcStr) {
        return nullptr;
    }

    CFIndex length = CFStringGetLength(srcStr);
    CFIndex maxSize = CFStringGetMaximumSizeForEncoding(length, kCFStringEncodingShiftJIS) + 1;
    char *buffer = (char *)calloc(maxSize, sizeof(char));
    if(!buffer) {
        CFRelease(srcStr);
        return nullptr;
    }

    CFStringGetCString(srcStr, buffer, maxSize, kCFStringEncodingShiftJIS);
    std::string dstStr(buffer);
    free(buffer); // If we failed
    CFRelease(srcStr);
    return dstStr;
#elif _WIN32 || _WIN64
    auto const srcWCharSize = ::MultiByteToWideChar(CP_UTF8, 0U, src.data(), -1, nullptr, 0U);
    std::vector<wchar_t> srcWCharData(srcWCharSize, L'\0');
    if (::MultiByteToWideChar(CP_UTF8, 0U, src.data(), -1, srcWCharData.data(), (int)(srcWCharData.size())) == 0) {
        throw std::system_error{ static_cast<int>(::GetLastError()), std::system_category() };
    }
    srcWCharData.resize(std::char_traits<wchar_t>::length(srcWCharData.data()));
    srcWCharData.shrink_to_fit();

    std::wstring srcWChar(srcWCharData.begin(), srcWCharData.end());
    auto const destSize = ::WideCharToMultiByte(CP_ACP, 0U, srcWChar.data(), -1, nullptr, 0, nullptr, nullptr);
    std::vector<char> dest(destSize, '\0');
    if (::WideCharToMultiByte(CP_ACP, 0U, srcWChar.data(), -1, dest.data(), (int)(dest.size()), nullptr, nullptr) == 0) {
        throw std::system_error{ static_cast<int>(::GetLastError()), std::system_category() };
    }
    dest.resize(std::char_traits<char>::length(dest.data()));
    dest.shrink_to_fit();
    return std::string(dest.begin(), dest.end());
#else
    // TODO: impl
    return std::string(src);
#endif
}

std::string SsCharConverter::convert_path_string(const std::string &str) {
    std::string dst;
#if _WIN32 || _WIN64
    dst = SsCharConverter::utf8_to_sjis(str);
#else
    // pass through
    dst = str;
#endif
    return dst;
}

}   // namespace spritestudio6
