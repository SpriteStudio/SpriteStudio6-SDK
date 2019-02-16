#include "sscharconverter.h"

#ifdef __APPLE__
    #include "TargetConditionals.h"
    #ifdef TARGET_OS_MAC
        #include <CoreFoundation/CoreFoundation.h>
    #endif
#else

#endif

std::string SsCharConverter::sjis_to_utf8(const std::string &src) {
#if __APPLE__ && TARGET_OS_MAC
    CFStringRef srcStr = CFStringCreateWithBytes(kCFAllocatorDefault, (const UInt8 *) src.c_str(), src.length(), kCFStringEncodingShiftJIS, FALSE);
    if(!srcStr) {
        return nullptr;
    }

    CFIndex length = CFStringGetLength(srcStr);
    CFIndex maxSize = CFStringGetMaximumSizeForEncoding(length, kCFStringEncodingUTF8) + 1;
    char *buffer = (char *)calloc(maxSize, sizeof(char));
    if(!buffer) {
        CFRelease(srcStr);
        return nullptr;
    }

    CFStringGetCString(srcStr, buffer, maxSize, kCFStringEncodingUTF8);
    std::string dstStr(buffer);
    free(buffer); // If we failed
    CFRelease(srcStr);
    return dstStr;
#else
    return std::string(src);
#endif
}

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
#else
    return std::string(src);
#endif
}

std::string SsCharConverter::force_convert_to_utf8(const std::string &src) {
#if __APPLE__ && TARGET_OS_MAC
	// detect code
	CFStringEncoding srcEncoding;
	CFStringEncoding encodings[] = {
			kCFStringEncodingShiftJIS,
			kCFStringEncodingShiftJIS_X0213,
			kCFStringEncodingShiftJIS_X0213_MenKuTen,
			kCFStringEncodingUTF8,
			0,
	};
	int i = 0;
	CFStringRef srcStr;
	while(encodings[i] != 0){
		 srcStr = CFStringCreateWithBytes(kCFAllocatorDefault, (const UInt8 *) src.c_str(), src.length(), encodings[i], FALSE);
        if (srcStr != NULL) {
            srcEncoding = encodings[i];
            break;
        }
        i++;
    }
	if(srcStr == NULL) {
		// not found encoding
		return nullptr;
	}

    CFIndex length = CFStringGetLength(srcStr);
    CFIndex maxSize = CFStringGetMaximumSizeForEncoding(length, kCFStringEncodingUTF8) + 1;
    char *buffer = (char *)calloc(maxSize, sizeof(char));
    if(!buffer) {
        CFRelease(srcStr);
        return nullptr;
    }

    CFStringGetCString(srcStr, buffer, maxSize, kCFStringEncodingUTF8);
    std::string dstStr(buffer);
    free(buffer);
    CFRelease(srcStr);
    return dstStr;

#else
	return std::string(src);
#endif
}

