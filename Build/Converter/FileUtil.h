#ifndef _FILE_UTIL_H_
#define _FILE_UTIL_H_

#include <string>
#include <vector>

namespace FileUtil
{
    std::string replaceBackslash(const std::string& path);
	
	std::string replaceExtension(const std::string& path, const std::string& oldExt, const std::string& newExt);

#ifdef _WIN32
    std::vector<std::string> findPath(const std::string& pattern);
#endif	// ifdef WIN32

	std::string getFilePath(const std::string& fullpath);
	std::string getFileName(const std::string& fullpath);
	std::string normalizeFilePath(const std::string& fullpath);

}

#endif	// _FILE_UTIL_H_
