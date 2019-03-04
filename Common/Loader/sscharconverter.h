#ifndef SSSDK_SSCHARCONVERTER_H
#define SSSDK_SSCHARCONVERTER_H

#include <string>

class SsCharConverter {
public:
	static std::string utf8_to_sjis(const std::string &src);
	static std::string convert_path_string(const std::string &str);
};

#endif //SSSDK_SSCHARCONVERTER_H
