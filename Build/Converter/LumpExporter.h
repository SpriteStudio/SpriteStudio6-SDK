//
//  LumpExporter.h
//

#ifndef __LumpExporter__
#define __LumpExporter__

#include <iostream>
#include <string>

#include "Lump.h"

namespace LumpExporter {

enum StringEncoding {
    UTF8,
    SJIS
};

void saveBinary(std::ostream& out, StringEncoding encoding, const std::shared_ptr<Lump>& lump, const std::string& creatorComment);

void saveCSource(std::ostream& out, StringEncoding encoding, const std::shared_ptr<Lump>& lump, const std::string& topLabel, const std::string& creatorComment);

void saveJson(std::ostream& out, StringEncoding encoding, const std::shared_ptr<Lump>& lump, const std::string& creatorComment);

void saveSsfb(std::ostream& out, StringEncoding encoding, const std::shared_ptr<Lump>& lump, const std::string& creatorComment, const std::vector<int16_t>& frameIndexVec);
}  // namespace LumpExporter

#endif /* defined(__LumpExporter__) */
