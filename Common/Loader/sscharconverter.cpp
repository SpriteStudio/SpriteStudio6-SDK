#include "sscharconverter.h"

#ifdef __APPLE__
    #include "TargetConditionals.h"
    #ifdef TARGET_OS_MAC
        #include <CoreFoundation/CoreFoundation.h>
    #endif
#elif _WIN32 || _WIN64
    #include <Windows.h>
    #include <locale.h>
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
    auto const srcSize = src.size();
    auto const srcWCharSize = ::MultiByteToWideChar(CP_UTF8, 0U, src.data(), srcSize, nullptr, 0U);
    std::vector<wchar_t> srcWCharData(srcWCharSize, L'\0');
    if (::MultiByteToWideChar(CP_UTF8, 0U, src.data(), srcSize, srcWCharData.data(), (int)srcWCharSize) == 0) {
        throw std::system_error{ static_cast<int>(::GetLastError()), std::system_category() };
    }
//    srcWCharData.resize(std::char_traits<wchar_t>::length(srcWCharData.data()));
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

std::string SsCharConverter::sjis_to_utf8(const std::string &src) {
#if __APPLE__ && TARGET_OS_MAC
    /* MEMO: �����A�ǂȂ���Mac�ł��L�ڂ��肢�ł��܂���ł��傤���H Yuzu. */
    /*       Mac�̏ꍇ�́AConverter�̃R�}���h���C���̉�͂ł����g�p���Ȃ��Ǝv���܂��B */

    // TODO: impl
    return std::string(src);
#elif _WIN32 || _WIN64
    enum Constant {
        SIZE_BUFFER_WTCHAR = 2048,
    };
    wchar_t buffer[Constant::SIZE_BUFFER_WTCHAR];   //�I�[�������݂̒����ł��邱�Ƃɒ���
    size_t srcWCharSize = 0;
    // MEMO: src����xc_str��ʂ���data()�Ƀo�b�t�@���`������Ă��邩�ۏ؂��Ȃ��̂ŁAc_str�����Ă����i�W���I��std::string�̎����Ȃ���v�Ȃ͂��Ȃ̂ŁA�O�̂��߁j�B
    if (::_mbstowcs_s_l(&srcWCharSize, buffer, Constant::SIZE_BUFFER_WTCHAR, src.c_str(), _TRUNCATE, ::_create_locale(LC_ALL, "jpn")) != 0) {
        throw std::system_error{ errno, std::system_category() };
    }
    //MEMO: ��Ƀo�b�t�@���w�蒷�Őݒu����.data()�ɒ��ڃ����_�����O���Ă��܂��ƁA�o�b�t�@�ɏI�[����������Ȃ��ꍇ�����邽�߁c�c
    std::wstring srcWCharData(buffer);
    srcWCharData.shrink_to_fit();
    srcWCharSize = srcWCharData.size(); // �Ď擾

    auto const destByteSize = ::WideCharToMultiByte(CP_UTF8, 0U, srcWCharData.data(), srcWCharSize, nullptr, 0, nullptr, nullptr);
    std::vector<char> dest(destByteSize, L'\0');
    if (::WideCharToMultiByte(CP_UTF8, 0U, srcWCharData.data(), srcWCharSize, dest.data(), destByteSize, nullptr, nullptr) == 0) {
        throw std::system_error{ static_cast<int>(::GetLastError()), std::system_category() };
    }
//    dest.shrink_to_fit();

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
