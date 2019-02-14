#ifndef SSSDK_SSCHARCONVERTER_H
#define SSSDK_SSCHARCONVERTER_H

#include <string>

class SsCharConverter {
public:
    static std::string sjis_to_utf8(const std::string &src);
    static std::string utf8_to_sjis(const std::string &src);
};

#endif //SSSDK_SSCHARCONVERTER_H
