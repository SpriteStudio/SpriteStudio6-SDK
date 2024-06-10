#ifndef SSFILESYSTEM_H
#define SSFILESYSTEM_H

#include <string>
#include <cstdio>

namespace spritestudio6 {
  class ssFilesystem {
  public:
    static FILE* openFile(const std::string& filepath);
  };
}

#endif //SSFILESYSTEM_H
