//
//  LumpExporter.h
//

#ifndef __LumpExporter__
#define __LumpExporter__

#include "Lump.h"
#include <iostream>
#include <string>

namespace LumpExporter {

	enum StringEncoding
	{
		UTF8,
		SJIS
	};

	void saveBinary(std::ostream& out, StringEncoding encoding, const Lump* lump, const std::string& creatorComment);

	void saveCSource(std::ostream& out, StringEncoding encoding, const Lump* lump, const std::string& topLabel, const std::string& creatorComment);

	void saveJson(std::ostream& out, StringEncoding encoding, const Lump* lump, const std::string& creatorComment);

	void saveSsfb(std::ostream &out, StringEncoding encoding, const Lump *lump, const std::string &creatorComment,
					  const std::vector<int16_t> &frameIndexVec);
}

#endif /* defined(__LumpExporter__) */
