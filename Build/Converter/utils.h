#ifndef SSSDK_UTIL_H
#define SSSDK_UTIL_H

inline bool isWindows() {
#if _WIN32 || _WIN64
    return true;
#else
    return false;
#endif
}

#endif  // SSSDK_UTIL_H
