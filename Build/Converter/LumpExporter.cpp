//
//  LumpExporter.cpp
//

#include "LumpExporter.h"
#include "BinaryDataWriter.h"
#include <assert.h>
#include <cstdarg>
#include "babel/babel.h"
#include "picojson.h"
#include "flatbuffers/flatbuffers.h"
#include "flatbuffers/util.h"
#include "ssfb_generated.h"
#include "Lump.h"

namespace LumpExporter {


static std::string format(const char* fmt, std::va_list arg)
{
	char buffer[0x1000];
	vsprintf(buffer, fmt, arg);
	return std::string(buffer);
}

static std::string format(const char* fmt, ...)
{
	std::va_list arg;
	va_start(arg, fmt);
	std::string s = format(fmt, arg);
	va_end(arg);
	return s;
}

static std::string encode(const std::string& sjisStr, StringEncoding encoding)
{
	switch (encoding) {
		case UTF8: return babel::sjis_to_utf8(sjisStr.c_str());
		case SJIS: return sjisStr;
		default:
			break;
	}
	return sjisStr;
}





class CSourceExporter
{
public:
	static void save(std::ostream& out, StringEncoding encoding, const Lump* lump, const std::string& topLabel, const std::string& creatorComment)
	{
		CSourceExporter* exporter = new CSourceExporter();
		exporter->m_encoding = encoding;
		exporter->m_topLabel = topLabel;
		
		out << "// " << creatorComment << std::endl;
		
		const LumpSet* lset = lump->data.p;
		out << format("extern const %s %s;\n",
			lset->className.c_str(),
			topLabel.c_str());
		
		exporter->writeStrings(out, lump);
		exporter->writeReferenceLumpSet(out, lump);
		delete exporter;
	}

private:
	typedef std::map<const void*, std::string> LabelMapType;

	LabelMapType	m_labelMap;
	StringEncoding	m_encoding;
	std::string		m_topLabel;


	void writeStrings(std::ostream& out, const Lump* lump)
	{
		const LumpSet* lset = lump->data.p;

		for (LumpSet::SetType::const_iterator it = lset->set.begin(); it != lset->set.end(); it++)
		{
			const Lump* child = *it;
			if (child->type == Lump::SET)
			{
				writeStrings(out, child);
			}
		}
	
		for (LumpSet::SetType::const_iterator it = lset->set.begin(); it != lset->set.end(); it++)
		{
			const Lump* child = *it;
			if (child->type == Lump::STRING)
			{
				if (m_labelMap.find(child) == m_labelMap.end())
				{
					std::string label = format("label_%04d", m_labelMap.size());
					m_labelMap[child] = label;

					std::string str = encode(*child->data.s, m_encoding);

					out << format("static const char %s[] = \"%s\";\n", label.c_str(), str.c_str());
				}
			}
		}
	}
	

	union MixType
	{
		int i;
		float f;
	};

	void writeLumpSetBlock(std::ostream& out, const Lump* lump)
	{
		const LumpSet* lset = lump->data.p;

		out << format("{");

		// ノーマルのデータ構造
		if (lset->arrayType == LumpSet::NO_ARRAY || lset->arrayType == LumpSet::ARRAY)
		{
			bool second = false;
			for (LumpSet::SetType::const_iterator it = lset->set.begin();
				it != lset->set.end(); it++)
			{
				if (second) out << format(",");
				second = true;

				const Lump* child = *it;
				switch (child->type)
				{
					case Lump::S16:
					case Lump::S32:
						out << format("%d", child->data.i);
						break;
					case Lump::FLOAT:
						out << format("%f", child->data.f);
						break;
					case Lump::COLOR:
						out << format("%x", child->data.i);
						break;
					case Lump::STRING:
						out << format("(ss_offset)((char*)%s - (char*)&%s)/*%s*/", m_labelMap[child].c_str(),
							m_topLabel.c_str(),
							child->data.s->c_str());
						break;
					case Lump::SET:
						if (child->data.p->isReference)
						{
							out << format("(ss_offset)((char*)%s%s - (char*)&%s)",
								child->data.p->arrayType == LumpSet::NO_ARRAY ? "&" : "",
								m_labelMap[child].c_str(),
								m_topLabel.c_str());
						}
						else
						{
							if (second) out << format("\n");
							writeLumpSetBlock(out, child);
						}
						break;
					default:
						assert(false);
						break;
				}
			}
		}
		// u16型の配列
		else if (lset->arrayType == LumpSet::U16_ARRAY)
		{
			bool second = false;
			for (LumpSet::SetType::const_iterator it = lset->set.begin();
				it != lset->set.end(); it++)
			{
				if (second) out << format(",");
				second = true;

				const Lump* child = *it;
				switch (child->type)
				{
					case Lump::S16:
						out << format("0x%x", child->data.i);
						break;
					case Lump::S32:
						out << format("0x%x,0x%x", child->data.i & 0xffff, (child->data.i >> 16) & 0xffff);
						break;
					case Lump::FLOAT:
						{
							MixType mix;
							mix.f = child->data.f;
							int value = mix.i;
							out << format("0x%x,0x%x", value & 0xffff, (value >> 16) & 0xffff);
						}
						break;
					case Lump::COLOR:
						out << format("0x%x,0x%x", child->data.i & 0xffff, (child->data.i >> 16) & 0xffff);
						break;
					case Lump::STRING:
//						w.format("(ss_offset)((char*)%s - (char*)&%s)/*%s*/", m_labelMap[child].c_str(),
//							m_topLabel.c_str(),
//							child->data.s->c_str());
						break;
					case Lump::SET:
						// Not support
						assert(false);
						break;
					default:
						assert(false);
						break;
				}
			}
		}
			
		out << format("}");
	}


	void writeReferenceLumpSet(std::ostream& out, const Lump* lump, int callDepth = 0)
	{
		const LumpSet* lset = lump->data.p;
		
		for (LumpSet::SetType::const_iterator it = lset->set.begin();
			it != lset->set.end(); it++)
		{
			const Lump* child = *it;
			if (child->type == Lump::SET)
			{
				writeReferenceLumpSet(out, child, callDepth + 1);
			}
		}

		if (lset->isReference)
		{
			if (m_labelMap.find(lump) == m_labelMap.end())
			{
				std::string label = format("label_%04d", m_labelMap.size());
				m_labelMap[lump] = label;

				bool isStaticPrefix = callDepth > 0;

				out << format("%sconst %s %s%s = ",
					isStaticPrefix ? "static " : "",
					lset->className.c_str(),
					callDepth == 0 ? m_topLabel.c_str() : label.c_str(),
					lset->arrayType == LumpSet::NO_ARRAY ? "" : "[]"
					);

				writeLumpSetBlock(out, lump);

				out << format(";\n");
			}
		}
	}

};





class BinaryExporter
{
public:
	static void save(std::ostream& out, StringEncoding encoding, const Lump* lump, const std::string& creatorComment)
	{
		BinaryExporter* exporter = new BinaryExporter();
		exporter->m_encoding = encoding;

		BinaryDataWriter writer(out);

		// ヘッダー部を予約しておく
		writer.fill(0, 64);
		// creator情報埋め込み
		writer.writeString(creatorComment);
		writer.align(64);
		
		exporter->writeStrings(writer, lump);
		exporter->writeReferenceLumpSet(writer, lump);
		
		writer.fixReferences();
		
		delete exporter;
	}

private:
	typedef std::map<const void*, std::string> LabelMapType;

	LabelMapType	m_labelMap;
	StringEncoding	m_encoding;


	void writeStrings(BinaryDataWriter& writer, const Lump* lump)
	{
		const LumpSet* lset = lump->data.p;

		for (LumpSet::SetType::const_iterator it = lset->set.begin(); it != lset->set.end(); it++)
		{
			const Lump* child = *it;
			if (child->type == Lump::SET)
			{
				writeStrings(writer, child);
			}
		}
	
		for (LumpSet::SetType::const_iterator it = lset->set.begin(); it != lset->set.end(); it++)
		{
			const Lump* child = *it;
			if (child->type == Lump::STRING)
			{
				if (m_labelMap.find(child) == m_labelMap.end())
				{
					std::string label = format("label_%04d", m_labelMap.size());
					m_labelMap[child] = label;
					
					std::string str = encode(*child->data.s, m_encoding);

					writer.setReference(label);
					writer.writeString(str);
				}
			}
		}
	}


	void writeLumpSetBlock(BinaryDataWriter& writer, const Lump* lump)
	{
		const LumpSet* lset = lump->data.p;

		// ノーマルのデータ構造
		if (lset->arrayType == LumpSet::NO_ARRAY || lset->arrayType == LumpSet::ARRAY)
		{
			for (LumpSet::SetType::const_iterator it = lset->set.begin();
				it != lset->set.end(); it++)
			{
				const Lump* child = *it;
				switch (child->type)
				{
					case Lump::S16:
						writer.writeShort(child->data.i);
						break;
					case Lump::S32:
						writer.writeInt(child->data.i);
						break;
					case Lump::FLOAT:
						writer.writeFloat(child->data.f);
						break;
					case Lump::COLOR:
						writer.writeInt(child->data.i);
						break;
					case Lump::STRING:
						assert(m_labelMap.find(child) != m_labelMap.end());
						writer.writeReference(m_labelMap[child]);
						break;
					case Lump::SET:
						if (child->data.p->isReference)
						{
							assert(m_labelMap.find(child) != m_labelMap.end());
							writer.writeReference(m_labelMap[child]);
						}
						else
						{
							writeLumpSetBlock(writer, child);
						}
						break;
					default:
						assert(false);
						break;
				}
			}
		}
		// u16型の配列
		else if (lset->arrayType == LumpSet::U16_ARRAY)
		{
			for (LumpSet::SetType::const_iterator it = lset->set.begin();
				it != lset->set.end(); it++)
			{
				const Lump* child = *it;
				switch (child->type)
				{
					case Lump::S16:
						writer.writeShort(child->data.i);
						break;
					case Lump::S32:
						writer.writeInt(child->data.i);
						break;
					case Lump::FLOAT:
						writer.writeFloat(child->data.f);
						break;
					case Lump::COLOR:
						writer.writeInt(child->data.i);
						break;
					case Lump::STRING:
						assert(m_labelMap.find(child) != m_labelMap.end());
						writer.writeReference(m_labelMap[child], false);	// 4バイト境界化しない
						break;
					case Lump::SET:
						// Not support
						assert(false);
						break;
					default:
						assert(false);
						break;
				}
			}
		}
	}


	void writeReferenceLumpSet(BinaryDataWriter& writer, const Lump* lump, int callDepth = 0)
	{
		const LumpSet* lset = lump->data.p;
		
		for (LumpSet::SetType::const_iterator it = lset->set.begin();
			it != lset->set.end(); it++)
		{
			const Lump* child = *it;
			if (child->type == Lump::SET)
			{
				writeReferenceLumpSet(writer, child, callDepth + 1);
			}
		}

		if (lset->isReference)
		{
			if (m_labelMap.find(lump) == m_labelMap.end())
			{
				std::string label = format("label_%04d", m_labelMap.size());
				m_labelMap[lump] = label;

				// トップはファイル先頭に書き込む
				if (callDepth == 0) writer.seekp(0);
				
				writer.setReference(label);
				writeLumpSetBlock(writer, lump);
			}
		}
	}

};


class JsonExporter
{
public:
	static void save(std::ostream& out, StringEncoding encoding, const Lump* lump,  const std::string& creatorComment)
	{
		JsonExporter* exporter = new JsonExporter();
		exporter->m_encoding = encoding;

		exporter->ssjson.clear();
		exporter->ssjson.insert(std::make_pair("creatorComment", picojson::value(creatorComment)));

		exporter->writeReferenceLumpSet(out, lump, exporter->ssjson);

		out << picojson::value(exporter->ssjson);

		delete exporter;
	}

private:
	picojson::object ssjson;

	typedef std::map<const void*, std::string> LabelMapType;

	LabelMapType	m_labelMap;
	StringEncoding	m_encoding;
	std::string		m_topLabel;


	void writeStrings(std::ostream& out, const Lump* lump)
	{
		const LumpSet* lset = lump->data.p;

		for (LumpSet::SetType::const_iterator it = lset->set.begin(); it != lset->set.end(); it++)
		{
			const Lump* child = *it;
			if (child->type == Lump::SET)
			{
				writeStrings(out, child);
			}
		}

		for (LumpSet::SetType::const_iterator it = lset->set.begin(); it != lset->set.end(); it++)
		{
			const Lump* child = *it;
			if (child->type == Lump::STRING)
			{
				if (m_labelMap.find(child) == m_labelMap.end())
				{
					std::string label = format("label_%04d", m_labelMap.size());
					m_labelMap[child] = label;

					std::string str = encode(*child->data.s, m_encoding);
				}
			}
		}
	}


	union MixType
	{
		int i;
		float f;
	};

	void writeLumpSetBlock(std::ostream& out, const Lump* lump, picojson::object& ssjson)
	{
		const LumpSet* lset = lump->data.p;

		// ノーマルのデータ構造
		if (lset->arrayType == LumpSet::NO_ARRAY || lset->arrayType == LumpSet::ARRAY)
		{
			picojson::array arrayjson;
			arrayjson.clear();

			bool second = false;
			for (LumpSet::SetType::const_iterator it = lset->set.begin();
				it != lset->set.end(); it++)
			{

				const Lump* child = *it;

				switch (child->type)
				{
				case Lump::S16:
				case Lump::S32:
				case Lump::COLOR:
					if (lset->arrayType == LumpSet::ARRAY)
					{
						arrayjson.push_back(picojson::value((double)child->data.i));
					}
					else
					{
						ssjson.insert(std::make_pair(child->name, picojson::value((double)child->data.i)));
					}
					break;
				case Lump::FLOAT:
					if (lset->arrayType == LumpSet::ARRAY)
					{
						arrayjson.push_back(picojson::value((double)child->data.f));
					}
					else
					{
						ssjson.insert(std::make_pair(child->name, picojson::value((double)child->data.f)));
					}
					break;
				case Lump::STRING:
					if (lset->arrayType == LumpSet::ARRAY)
					{
						arrayjson.push_back(picojson::value(child->data.s->c_str()));
					}
					else
					{
						ssjson.insert(std::make_pair(child->name, picojson::value(child->data.s->c_str())));
					}
					break;
				case Lump::SET:

					if (lset->arrayType == LumpSet::ARRAY)
					{
						picojson::object json;
						json.clear();
						writeLumpSetBlock(out, child, json);
						arrayjson.push_back(picojson::value(json));
					}
					else
					{
						const LumpSet* clset = child->data.p;
						if (clset->arrayType == LumpSet::ARRAY)
						{
							//子供が配列の場合はそのまま出力する
							writeLumpSetBlock(out, child, ssjson);
						}
						else
						{
							picojson::object json;
							json.clear();
							writeLumpSetBlock(out, child, json);
							ssjson.insert(std::make_pair(child->name, picojson::value(picojson::value(json))));
						}
					}

					break;
				default:
					assert(false);
					break;
				}

			}
			if (lset->arrayType == LumpSet::ARRAY)
			{
				ssjson.insert(std::make_pair(lump->name, picojson::value(arrayjson)));
			}
		}

		// u16型の配列
		else if (lset->arrayType == LumpSet::U16_ARRAY)
		{
			picojson::array arrayjson;
			arrayjson.clear();

			picojson::object json;
			json.clear();

			for (LumpSet::SetType::const_iterator it = lset->set.begin();
				it != lset->set.end(); it++)
			{
				const Lump* child = *it;

				switch (child->type)
				{
				case Lump::S16:
				case Lump::S32:
				case Lump::COLOR:
					arrayjson.push_back(picojson::value((double)child->data.i));
//					json.insert(std::make_pair(child->name, picojson::value((double)child->data.i)));
					break;
				case Lump::FLOAT:
					arrayjson.push_back(picojson::value((double)child->data.f));
//					json.insert(std::make_pair(child->name, picojson::value((double)child->data.f)));
					break;
				case Lump::STRING:
					break;
				case Lump::SET:
					// Not support
					assert(false);
					break;
				default:
					assert(false);
					break;
				}
//				arrayjson.push_back(picojson::value(json));
			}
			ssjson.insert(std::make_pair(lump->name, picojson::value(arrayjson)));
//			ssjson.insert(std::make_pair(lump->name, picojson::value(json)));
		}
	}


	void writeReferenceLumpSet(std::ostream& out, const Lump* lump, picojson::object& ssjson, int callDepth = 0 )
	{
		const LumpSet* lset = lump->data.p;

		if (lset->isReference)
		{
			if (m_labelMap.find(lump) == m_labelMap.end())
			{
				std::string label = format("label_%04d", m_labelMap.size());
				m_labelMap[lump] = label;

				writeLumpSetBlock(out, lump, ssjson);
			}
		}
	}

};


#define GETS16(l) (int16_t)(l->data.i)
#define GETS32(l) (int32_t)(l->data.i)
#define GETFLOAT(l) (float)(l->data.f)
#define GETSTRING(l, enc) encode(*l->data.s, enc)
#define GETSSFBSTRING(builder, l, enc) builder.CreateString(GETSTRING(l, enc))

class SsfbExporter
{
public:
	static void save(std::ostream &out, StringEncoding encoding, const Lump *root, const std::string &creatorComment,
                        const std::vector<int16_t> &frameIndexVec)
	{
		if(encoding != UTF8) {
			throw "flatbuffers supports only UTF-8";
		}
		auto exporter = new SsfbExporter();
		exporter->m_encoding = encoding;
		exporter->m_root = root;
		exporter->m_frameIndexVec = frameIndexVec;
		exporter->m_frameIndex = 0;

		exporter->createHeader();

		exporter->createCells();

		exporter->createAnimePacks();

		exporter->createEffectFile();

		exporter->createProjectData();

		exporter->writeFile(out);

		delete exporter;
	}

private:
	StringEncoding	m_encoding{};
	const Lump *m_root{};
	std::vector<int16_t> m_frameIndexVec;
	int m_frameIndex;

	flatbuffers::FlatBufferBuilder ssfbBuilder;
	flatbuffers::Offset<ss::ssfb::ProjectData> ssfbProjectData;
	int32_t dataId{};
	int32_t version{};
	int32_t flags{};
	flatbuffers::Offset<flatbuffers::String> ssfbImageBaseDir;
	std::vector<flatbuffers::Offset<ss::ssfb::Cell>> ssfbCells;
	std::vector<flatbuffers::Offset<ss::ssfb::AnimePackData>> ssfbAnimePacks;
	std::vector<flatbuffers::Offset<ss::ssfb::EffectFile>> ssfbEffectFileList;

	void createHeader()
	{
		auto rootChildVec = m_root->getChildren();
		dataId = GETS32(rootChildVec[0]);
		version = GETS32(rootChildVec[1]);
		flags = GETS32(rootChildVec[2]);
		ssfbImageBaseDir = GETSSFBSTRING(ssfbBuilder, rootChildVec[3], m_encoding);
	}

	void createCells()
	{
		auto rootChildVec = m_root->getChildren();
		// 4:Cell
		auto cellsLump = rootChildVec[4];

		auto cellsVec = cellsLump->getChildren();
		for(auto cellItem : cellsVec) {
			auto cellItemVec = cellItem->getChildren();
			auto ssfbCellName = GETSSFBSTRING(ssfbBuilder, cellItemVec[0], m_encoding);
			flatbuffers::Offset<ss::ssfb::CellMap> ssfbCellMap;
			{
				auto cellMapVec = cellItemVec[1]->getChildren();

				auto ssfbCellMapName = GETSSFBSTRING(ssfbBuilder, cellMapVec[0], m_encoding);
				auto ssfbCellMapImagePath = GETSSFBSTRING(ssfbBuilder, cellMapVec[1], m_encoding);
				auto mapIndex = GETS16(cellMapVec[2]);
				auto wrapMode = GETS16(cellMapVec[3]);
				auto filterMode = GETS16(cellMapVec[4]);
				// 5:reserved(s16)

				ssfbCellMap = ss::ssfb::CreateCellMap(ssfbBuilder, ssfbCellMapName, ssfbCellMapImagePath, mapIndex, wrapMode, filterMode);
			}
			auto indexInCellMap = GETS16(cellItemVec[2]);
			auto x = GETS16(cellItemVec[3]);
			auto y = GETS16(cellItemVec[4]);
			auto width = GETS16(cellItemVec[5]);
			auto height = GETS16(cellItemVec[6]);
			// 7:reverse(s16)
			auto pivot_x = GETFLOAT(cellItemVec[8]);
			auto pivot_y = GETFLOAT(cellItemVec[9]);
			auto u1 = GETFLOAT(cellItemVec[10]);
			auto v1 = GETFLOAT(cellItemVec[11]);
			auto u2 = GETFLOAT(cellItemVec[12]);
			auto v2 = GETFLOAT(cellItemVec[13]);

			auto ssfbCell = ss::ssfb::CreateCell(ssfbBuilder, ssfbCellName, ssfbCellMap,
												 indexInCellMap, x, y, width, height,
												 pivot_x, pivot_y, u1, v1, u2, v2);
			ssfbCells.push_back(ssfbCell);
		}
	}

	void createAnimePacks()
	{
		auto rootChildVec = m_root->getChildren();
		// 5:AnimePackData
		auto animePackDataLump = rootChildVec[5];

		auto animePackDataVec = animePackDataLump->getChildren();
		for (auto animePackDataItem : animePackDataVec) {
			flatbuffers::Offset<flatbuffers::String> ssfbAnimePackDataName;
			std::vector<flatbuffers::Offset<ss::ssfb::PartData>> ssfbParts;
			std::vector<flatbuffers::Offset<ss::ssfb::AnimationData>> ssfbAnimations;

			auto animePackDataItemVec = animePackDataItem->getChildren();

			ssfbAnimePackDataName = GETSSFBSTRING(ssfbBuilder, animePackDataItemVec[0], m_encoding);
			ssfbParts = createParts(animePackDataItemVec[1]);
			ssfbAnimations = createAnimationDataList(animePackDataItemVec[2]);
			// auto numPart = GETS16(animePackDataItemVec[3]); // unused
			// auto numAnimations = GETS16(animePackDataItemVec[4]); // unused

			auto serializeSsfbParts = ssfbBuilder.CreateVector(ssfbParts);
			auto serializeSsfbAnimations = ssfbBuilder.CreateVector(ssfbAnimations);
			auto ssfbAnimePackData = ss::ssfb::CreateAnimePackData(ssfbBuilder, ssfbAnimePackDataName, serializeSsfbParts, serializeSsfbAnimations);
			ssfbAnimePacks.push_back(ssfbAnimePackData);
		}
	}

	std::vector<flatbuffers::Offset<ss::ssfb::PartData>> createParts(const Lump* lump)
	{
		std::vector<flatbuffers::Offset<ss::ssfb::PartData>> ssfbParts;
		auto partDataVec = lump->getChildren();
		for(auto partDataItem : partDataVec) {
			auto partDataItemVec = partDataItem->getChildren();

			auto ssfbPartDataName = GETSSFBSTRING(ssfbBuilder, partDataItemVec[0], m_encoding);
			auto index = GETS16(partDataItemVec[1]);
			auto parentIndex = GETS16(partDataItemVec[2]);
			auto type = (ss::ssfb::SsPartType)GETS16(partDataItemVec[3]);
			auto boundsType = GETS16(partDataItemVec[4]);
			auto alphaBlendType = GETS16(partDataItemVec[5]);
			// 6:reserve
			auto ssfbRefname = GETSSFBSTRING(ssfbBuilder, partDataItemVec[7], m_encoding);
			auto ssfbEffectfilename = GETSSFBSTRING(ssfbBuilder, partDataItemVec[8], m_encoding);
			auto ssfbColorLabel = GETSSFBSTRING(ssfbBuilder, partDataItemVec[9], m_encoding);
			auto maskInfluence = GETS16(partDataItemVec[10]);

			auto ssfbPartDataItem = ss::ssfb::CreatePartData(ssfbBuilder, ssfbPartDataName,
															 index, parentIndex, type, boundsType,
															 alphaBlendType, ssfbRefname, ssfbEffectfilename, ssfbColorLabel,
															 maskInfluence);
			ssfbParts.push_back(ssfbPartDataItem);
		}

		return ssfbParts;
	}

	std::vector<flatbuffers::Offset<ss::ssfb::AnimationData>> createAnimationDataList(const Lump *lump)
	{
		std::vector<flatbuffers::Offset<ss::ssfb::AnimationData>> ssfbAnimationDataList;

		auto animationDataVec = lump->getChildren();
		for(auto animationDataItem : animationDataVec) {
			auto ssAnimationDataVec = animationDataItem->getChildren();

			auto ssfbAnimationDataName = GETSSFBSTRING(ssfbBuilder, ssAnimationDataVec[0], m_encoding);
			// 1:AnimationInitialData
			std::vector<flatbuffers::Offset<ss::ssfb::AnimationInitialData>> ssfbDefaultData;
			{
				auto AnimationInitialDataVec = ssAnimationDataVec[1]->getChildren();
				for(auto AnimationInitialDataItem : AnimationInitialDataVec)
				{

					auto AnimationInitialDataItemVec = AnimationInitialDataItem->getChildren();
					auto index = GETS16(AnimationInitialDataItemVec[0]);
					// 1:reserve
					auto lowflag = GETS32(AnimationInitialDataItemVec[2]);
					auto highflag = GETS32(AnimationInitialDataItemVec[3]);
					auto priority = GETS16(AnimationInitialDataItemVec[4]);
					auto cellIndex = GETS16(AnimationInitialDataItemVec[5]);
					auto opacity = GETS16(AnimationInitialDataItemVec[6]);
					auto localopacity = GETS16(AnimationInitialDataItemVec[7]);
					auto masklimen = GETS16(AnimationInitialDataItemVec[8]);
					// 9:reserved
					auto posX = GETFLOAT(AnimationInitialDataItemVec[10]);
					auto posY = GETFLOAT(AnimationInitialDataItemVec[11]);
					auto posZ = GETFLOAT(AnimationInitialDataItemVec[12]);
					auto pivotX = GETFLOAT(AnimationInitialDataItemVec[13]);
					auto pivotY = GETFLOAT(AnimationInitialDataItemVec[14]);
					auto rotationX = GETFLOAT(AnimationInitialDataItemVec[15]);
					auto rotationY = GETFLOAT(AnimationInitialDataItemVec[16]);
					auto rotationZ = GETFLOAT(AnimationInitialDataItemVec[17]);
					auto scaleX = GETFLOAT(AnimationInitialDataItemVec[18]);
					auto scaleY = GETFLOAT(AnimationInitialDataItemVec[19]);
					auto localscaleX = GETFLOAT(AnimationInitialDataItemVec[20]);
					auto localscaleY = GETFLOAT(AnimationInitialDataItemVec[21]);
					auto size_X = GETFLOAT(AnimationInitialDataItemVec[22]);
					auto size_Y = GETFLOAT(AnimationInitialDataItemVec[23]);
					auto uv_move_X = GETFLOAT(AnimationInitialDataItemVec[24]);
					auto uv_move_Y = GETFLOAT(AnimationInitialDataItemVec[25]);
					auto uv_rotation = GETFLOAT(AnimationInitialDataItemVec[26]);
					auto uv_scale_X = GETFLOAT(AnimationInitialDataItemVec[27]);
					auto uv_scale_Y = GETFLOAT(AnimationInitialDataItemVec[28]);
					auto boundingRadius = GETFLOAT(AnimationInitialDataItemVec[29]);
					//インスタンス関連
					auto instanceValue_curKeyframe = GETS32(AnimationInitialDataItemVec[30]);
					auto instanceValue_startFrame = GETS32(AnimationInitialDataItemVec[31]);
					auto instanceValue_endFrame = GETS32(AnimationInitialDataItemVec[32]);
					auto instanceValue_loopNum = GETS32(AnimationInitialDataItemVec[33]);
					auto instanceValue_speed = GETFLOAT(AnimationInitialDataItemVec[34]);
					auto instanceValue_loopflag = GETS32(AnimationInitialDataItemVec[35]);
					//エフェクト関連
					auto effectValue_curKeyframe = GETS32(AnimationInitialDataItemVec[36]);
					auto effectValue_startTime = GETS32(AnimationInitialDataItemVec[37]);
					auto effectValue_speed = GETFLOAT(AnimationInitialDataItemVec[38]);
					auto effectValue_loopflag = GETS32(AnimationInitialDataItemVec[39]);

					auto item = ss::ssfb::CreateAnimationInitialData(ssfbBuilder, index,
																	 lowflag, highflag, priority, cellIndex, opacity, localopacity, masklimen, posX, posY, posZ, //
																	 pivotX, pivotY, rotationX, rotationY, rotationZ, scaleX, scaleY, localscaleX, localscaleY, size_X, size_Y,
																	 uv_move_X, uv_move_Y, uv_rotation, uv_scale_X, uv_scale_Y, boundingRadius,
																	 instanceValue_curKeyframe, instanceValue_startFrame, instanceValue_endFrame, instanceValue_loopNum, instanceValue_speed, instanceValue_loopflag,
																	 effectValue_curKeyframe, effectValue_startTime, effectValue_speed, effectValue_loopflag);
					ssfbDefaultData.push_back(item);
				}
			}
			// 2:frameDataIndexArray
			std::vector<flatbuffers::Offset<ss::ssfb::frameDataIndex>> ssfbFrameData;
			{
				auto frameDataIndexArrayVec = ssAnimationDataVec[2]->getChildren();

				// TODO: 複数の型を float vector に格納しているため修正する
				std::vector<float> ssfbFrameData2;
				for(auto frameDataIndexArrayItem : frameDataIndexArrayVec) {
					auto frameDataVec = frameDataIndexArrayItem->getChildren();
					for(auto frameDataItem : frameDataVec) {
						switch (frameDataItem->type) {
							case Lump::DataType::S16:
								// TODO: int16_t の型を float vector に格納しているため修正する
								ssfbFrameData2.push_back(GETFLOAT(frameDataItem));
								break;
							case Lump::DataType::S32:
								// TODO: int32_t の型を float vector に格納しているため修正する
								ssfbFrameData2.push_back(GETFLOAT(frameDataItem));
								break;
							case Lump::DataType::FLOAT:
								ssfbFrameData2.push_back(GETFLOAT(frameDataItem));
								break;
							case Lump::DataType::COLOR:
								// TODO: int32_t(color) の型を float vector に格納しているため修正する
								ssfbFrameData2.push_back(GETFLOAT(frameDataItem));
								break;
							default:
								break;
						}
					}
				}
				auto serializeSsfbFrameData2 = ssfbBuilder.CreateVector(ssfbFrameData2);
				auto item = ss::ssfb::CreateframeDataIndex(ssfbBuilder, serializeSsfbFrameData2);
				ssfbFrameData.push_back(item);
			}
			// 3:userDataIndexArray
			std::vector<flatbuffers::Offset<ss::ssfb::userDataPerFrame>> ssfbUserData;
			{
				if(ssAnimationDataVec[3]->type == Lump::DataType::SET) {

					std::vector<flatbuffers::Offset<ss::ssfb::userDataItem>> ssfbUserDataItemData;

					auto userDataIndexArrayVec = ssAnimationDataVec[3]->getChildren();
					for(auto userDataIndexArrayItem : userDataIndexArrayVec) {
						if(userDataIndexArrayItem->type != Lump::DataType::SET) {
							continue;
						}

						std::vector<flatbuffers::Offset<void>> ssfbDataArray;
						std::vector<uint8_t> ssfbDataArrayType;

						auto userDataIndexArrayItemVec = userDataIndexArrayItem->getChildren();
						auto num = GETS16(userDataIndexArrayItemVec[0]);
						int idx = 1;
						for(int i=0; i<num; i++) {
							auto flags = GETS16(userDataIndexArrayItemVec[idx++]);
							auto arrayIndex = GETS16(userDataIndexArrayItemVec[idx++]);
							if(flags & (int16_t)(ss::ssfb::USER_DATA_FLAG::USER_DATA_FLAG_INTEGER)) {
								auto integer = GETS32(userDataIndexArrayItemVec[idx++]);
								auto item = ss::ssfb::CreateuserDataInteger(ssfbBuilder, integer);
								ssfbDataArray.push_back(item.Union());
								ssfbDataArrayType.push_back(ss::ssfb::USER_DATA_FLAG_INTEGER);
							}
							if(flags & (int16_t)(ss::ssfb::USER_DATA_FLAG::USER_DATA_FLAG_RECT)) {
								auto rect_x = GETS32(userDataIndexArrayItemVec[idx++]);
								auto rect_y = GETS32(userDataIndexArrayItemVec[idx++]);
								auto rect_w = GETS32(userDataIndexArrayItemVec[idx++]);
								auto rect_h = GETS32(userDataIndexArrayItemVec[idx++]);

								auto item = ss::ssfb::CreateuserDataRect(ssfbBuilder, rect_x, rect_y, rect_w, rect_h);
								ssfbDataArray.push_back(item.Union());
								ssfbDataArrayType.push_back(ss::ssfb::USER_DATA_FLAG_RECT);
							}
							if(flags & (int16_t)(ss::ssfb::USER_DATA_FLAG::USER_DATA_FLAG_POINT)) {
								auto point_x = GETS32(userDataIndexArrayItemVec[idx++]);
								auto point_y = GETS32(userDataIndexArrayItemVec[idx++]);

								auto item = ss::ssfb::CreateuserDataPoint(ssfbBuilder, point_x, point_y);
								ssfbDataArray.push_back(item.Union());
								ssfbDataArrayType.push_back(ss::ssfb::USER_DATA_FLAG_POINT);

							}
							if(flags & (int16_t)(ss::ssfb::USER_DATA_FLAG::USER_DATA_FLAG_STRING)) {
								auto str_length = GETS16(userDataIndexArrayItemVec[idx++]);
								auto ssfbStr = GETSSFBSTRING(ssfbBuilder, userDataIndexArrayItemVec[idx++], m_encoding);

								auto item = ss::ssfb::CreateuserDataString(ssfbBuilder, str_length, ssfbStr);
								ssfbDataArray.push_back(item.Union());
								ssfbDataArrayType.push_back(ss::ssfb::USER_DATA_FLAG_STRING);
							}

							auto serializeSsfbDataArrayType = ssfbBuilder.CreateVector(ssfbDataArrayType);
							auto serializeSsfbDataArray = ssfbBuilder.CreateVector(ssfbDataArray);
							auto item = ss::ssfb::CreateuserDataItem(ssfbBuilder, static_cast<int16_t>(flags),
																	 arrayIndex,
																	 serializeSsfbDataArrayType,
																	 serializeSsfbDataArray);
							ssfbUserDataItemData.push_back(item);
						}

						auto serializeSsfbUserDataItemData = ssfbBuilder.CreateVector(ssfbUserDataItemData);

						auto frame = this->m_frameIndexVec[this->m_frameIndex++];
						auto item = ss::ssfb::CreateuserDataPerFrame(ssfbBuilder, frame, serializeSsfbUserDataItemData);
						ssfbUserData.push_back(item);
					}
				}
			}
			// 4:LabelDataIndexArray
			std::vector<flatbuffers::Offset<ss::ssfb::labelDataItem>> ssfbLabelData;
			{
				if(ssAnimationDataVec[4]->type == Lump::DataType::SET) {
					auto LabelDataIndexArrayVec = ssAnimationDataVec[4]->getChildren();
					for(auto LabelDataIndexArrayItem : LabelDataIndexArrayVec) {
						auto labelDataVec = LabelDataIndexArrayItem->getChildren();
						auto ssfbLabelDataItemName = GETSSFBSTRING(ssfbBuilder, labelDataVec[0], m_encoding);
						auto time = GETS16(labelDataVec[1]);

						auto item = ss::ssfb::CreatelabelDataItem(ssfbBuilder, ssfbLabelDataItemName, time);
						ssfbLabelData.push_back(item);
					}
				}
			}

			// 5:meshDataUV
			std::vector<flatbuffers::Offset<ss::ssfb::meshDataUV>> ssfbMeshsDataUV;
			{
				auto meshDataUVVec = ssAnimationDataVec[5]->getChildren();
				std::vector<float> ssfbUV;
				for(auto meshDataUVItem : meshDataUVVec) {
					auto frameDataVec = meshDataUVItem->getChildren();
					for(auto frameDataItem : frameDataVec) {
						ssfbUV.push_back(GETFLOAT(frameDataItem));
					}

					auto serializeSsfbUV = ssfbBuilder.CreateVector(ssfbUV);
					auto item = ss::ssfb::CreatemeshDataUV(ssfbBuilder, serializeSsfbUV);
					ssfbMeshsDataUV.push_back(item);
				}
			}
			// 6:meshsDataIndices
			std::vector<flatbuffers::Offset<ss::ssfb::meshDataIndices>> ssfbMeshsDataIndices;
			{
				auto meshsDataIndicesVec = ssAnimationDataVec[6]->getChildren();
				std::vector<float> ssfbIndices;
				for(auto meshsDataIndicesItem : meshsDataIndicesVec) {
					auto meshsDataVec = meshsDataIndicesItem->getChildren();
					for(auto meshDataItem : meshsDataVec) {
						ssfbIndices.push_back(GETFLOAT(meshDataItem));
					}

					auto serializeSsfbIndices = ssfbBuilder.CreateVector(ssfbIndices);
					auto item = ss::ssfb::CreatemeshDataIndices(ssfbBuilder, serializeSsfbIndices);
					ssfbMeshsDataIndices.push_back(item);
				}
			}
			auto startFrames = GETS16(ssAnimationDataVec[7]);
			auto endFrames = GETS16(ssAnimationDataVec[8]);
			auto totalFrames = GETS16(ssAnimationDataVec[9]);
			auto fps = GETS16(ssAnimationDataVec[10]);
			auto labelNum = GETS16(ssAnimationDataVec[11]);
			auto canvasSizeW = GETS16(ssAnimationDataVec[12]);
			auto canvasSizeH = GETS16(ssAnimationDataVec[13]);
			// 14:reserved
			auto canvasPvotX = GETFLOAT(ssAnimationDataVec[15]);
			auto canvasPvotY = GETFLOAT(ssAnimationDataVec[16]);

			auto serializeSsfbDefaultData = ssfbBuilder.CreateVector(ssfbDefaultData);
			auto serializeSsfbMeshsDataUV = ssfbBuilder.CreateVector(ssfbMeshsDataUV);
			auto serializeSsfbMeshsDataIndices = ssfbBuilder.CreateVector(ssfbMeshsDataIndices);
/*
                auto serializeSsfbFrameData = ssfbBuilder.CreateVector(ssfbFrameData);
                auto serializeSsfbUserData = ssfbBuilder.CreateVector(ssfbUserData);
                auto serializeSsfbLabelData = ssfbBuilder.CreateVector(ssfbLabelData);

                auto item = ss::ssfb::CreateAnimationData(ssfbBuilder, ssfbAnimationDataName,
                                                          serializeSsfbDefaultData, serializeSsfbFrameData, serializeSsfbUserData,
                                                          serializeSsfbLabelData, serializeSsfbMeshsDataUV, serializeSsfbMeshsDataIndices,
                                                          startFrames, endFrames, totalFrames, fps, labelNum, canvasSizeW, canvasSizeH, canvasPvotX, canvasPvotY);
                ssfbAnimations.push_back(item);
                 */
		}
		/*
        auto serializeSsfbParts = ssfbBuilder.CreateVector(ssfbParts);
        auto serializeSsfbAnimations = ssfbBuilder.CreateVector(ssfbAnimations);
        auto ssfbAnimePackData = ss::ssfb::CreateAnimePackData(ssfbBuilder, ssfbAnimePackDataName, serializeSsfbParts, serializeSsfbAnimations);
        ssfbAnimePacks.push_back(ssfbAnimePackData);
         */

		return ssfbAnimationDataList;
	}

	void createEffectFile() {
		auto rootChildVec = m_root->getChildren();
		auto effectFileLump = rootChildVec[6];
		// 6:EffectFile

		auto effectFileLumpVec = effectFileLump->getChildren();
		for(auto effectFileLumpItem : effectFileLumpVec) {
			auto effectFileLumpItemVec = effectFileLumpItem->getChildren();
			auto ssfbEffectFileName = GETSSFBSTRING(ssfbBuilder, effectFileLumpItemVec[0], m_encoding);
			auto fps = GETS16(effectFileLumpItemVec[1]);
			auto isLockRandSeed = GETS16(effectFileLumpItemVec[2]);
			auto LockRandSeed = GETS16(effectFileLumpItemVec[3]);
			auto layoutScaleX = GETS16(effectFileLumpItemVec[4]);
			auto layoutScaleY = GETS16(effectFileLumpItemVec[5]);
			auto numNodeList = GETS16(effectFileLumpItemVec[6]);
			auto EffectNodeArray = effectFileLumpItemVec[7];
			auto EffectNodeArrayVec = EffectNodeArray->getChildren();

			std::vector<flatbuffers::Offset<ss::ssfb::EffectNode>> ssfbEffectNode;
			for(auto EffectNodeArrayItem : EffectNodeArrayVec) {
				auto EffectNodeVec = EffectNodeArrayItem->getChildren();

				auto arrayIndex = GETS16(EffectNodeVec[0]);
				auto parentIndex = GETS16(EffectNodeVec[1]);
				auto type = GETS16(EffectNodeVec[2]);
				auto cellIndex = GETS16(EffectNodeVec[3]);
				auto blendType = GETS16(EffectNodeVec[4]);
				auto numBehavior = GETS16(EffectNodeVec[5]);
				auto effectBehaviorArrayVec = EffectNodeVec[6]->getChildren();

				std::vector<flatbuffers::Offset<void>> ssfbEffectNodeBehavior;
				std::vector<uint8_t> ssfbEffectNodeBehaviorType;
				for(auto effectBehaviorArrayItem : effectBehaviorArrayVec) {
					auto effectBehaviorArrayItemVec = effectBehaviorArrayItem->getChildren();

					auto SsEffectFunctionType = (ss::ssfb::EffectNodeBehavior)GETS32(effectBehaviorArrayItemVec[0]);
					switch(SsEffectFunctionType) {
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementBasic: {
							auto priority = GETS32(effectBehaviorArrayItemVec[1]);
							auto maximumParticle = GETS32(effectBehaviorArrayItemVec[2]);
							auto attimeCreate = GETS32(effectBehaviorArrayItemVec[3]);
							auto interval = GETS32(effectBehaviorArrayItemVec[4]);
							auto lifetime = GETS32(effectBehaviorArrayItemVec[5]);
							auto speedMinValue = GETFLOAT(effectBehaviorArrayItemVec[6]);
							auto speedMaxValue = GETFLOAT(effectBehaviorArrayItemVec[7]);
							auto lifespanMinValue = GETS32(effectBehaviorArrayItemVec[8]);
							auto lifespanMaxValue = GETS32(effectBehaviorArrayItemVec[9]);
							auto angle = GETFLOAT(effectBehaviorArrayItemVec[10]);
							auto angleVariance = GETFLOAT(effectBehaviorArrayItemVec[11]);

							auto item = ss::ssfb::CreateEffectParticleElementBasic(ssfbBuilder, SsEffectFunctionType, priority,
																				   maximumParticle, attimeCreate, interval, lifetime,
																				   speedMinValue, speedMaxValue, lifespanMinValue, lifespanMaxValue, angle, angleVariance);
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementRndSeedChange: {
							auto Seed = GETS32(effectBehaviorArrayItemVec[1]);

							auto item = ss::ssfb::CreateEffectParticleElementRndSeedChange(ssfbBuilder, Seed);
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementDelay: {
							auto DelayTime = GETS32(effectBehaviorArrayItemVec[1]);

							auto item = ss::ssfb::CreateEffectParticleElementDelay(ssfbBuilder, DelayTime);
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementGravity: {
							auto Gravity_x = GETS32(effectBehaviorArrayItemVec[1]);
							auto Gravity_y = GETS32(effectBehaviorArrayItemVec[2]);

							auto item = ss::ssfb::CreateEffectParticleElementGravity(ssfbBuilder, Gravity_x , Gravity_y);
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementPosition: {
							auto OffsetXMinValue = GETFLOAT(effectBehaviorArrayItemVec[1]);
							auto OffsetXMaxValue = GETFLOAT(effectBehaviorArrayItemVec[2]);
							auto OffsetYMinValue = GETFLOAT(effectBehaviorArrayItemVec[3]);
							auto OffsetYMaxValue = GETFLOAT(effectBehaviorArrayItemVec[4]);

							auto item = ss::ssfb::CreateEffectParticleElementPosition(ssfbBuilder,
																					  OffsetXMinValue,
																					  OffsetXMaxValue,
																					  OffsetYMinValue,
																					  OffsetYMaxValue);
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementRotation: {
							auto RotationMinValue = GETFLOAT(effectBehaviorArrayItemVec[1]);
							auto RotationMaxValue = GETFLOAT(effectBehaviorArrayItemVec[2]);
							auto RotationAddMinValue = GETFLOAT(effectBehaviorArrayItemVec[3]);
							auto RotationAddMaxValue = GETFLOAT(effectBehaviorArrayItemVec[4]);

							auto item = ss::ssfb::CreateEffectParticleElementRotation(ssfbBuilder,
																					  RotationMinValue,
																					  RotationMaxValue,
																					  RotationAddMinValue,
																					  RotationAddMaxValue);
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementRotationTrans: {
							auto RotationFactor = GETFLOAT(effectBehaviorArrayItemVec[1]);
							auto EndLifeTimePer = GETFLOAT(effectBehaviorArrayItemVec[2]);

							auto item = ss::ssfb::CreateEffectParticleElementRotationTrans(ssfbBuilder, RotationFactor, EndLifeTimePer);
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementTransSpeed: {
							auto SpeedMinValue = GETFLOAT(effectBehaviorArrayItemVec[1]);
							auto SpeedMaxValue = GETFLOAT(effectBehaviorArrayItemVec[2]);

							auto item = ss::ssfb::CreateEffectParticleElementTransSpeed(ssfbBuilder, SpeedMinValue, SpeedMaxValue);
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementTangentialAcceleration: {
							auto AccelerationMinValue = GETFLOAT(effectBehaviorArrayItemVec[1]);
							auto AccelerationMaxValue = GETFLOAT(effectBehaviorArrayItemVec[2]);

							auto item = ss::ssfb::CreateEffectParticleElementTangentialAcceleration(ssfbBuilder, AccelerationMinValue, AccelerationMaxValue);
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementInitColor: {
							auto ColorMinValue = GETS32(effectBehaviorArrayItemVec[1]);
							auto ColorMaxValue = GETS32(effectBehaviorArrayItemVec[2]);

							// TODO: uint32
							auto item = ss::ssfb::CreateEffectParticleElementInitColor(ssfbBuilder, ColorMinValue, ColorMaxValue);
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementTransColor: {
							auto ColorMinValue = GETS32(effectBehaviorArrayItemVec[1]);
							auto ColorMaxValue = GETS32(effectBehaviorArrayItemVec[2]);

							// TODO: uint32
							auto item = ss::ssfb::CreateEffectParticleElementTransColor(ssfbBuilder, ColorMinValue, ColorMaxValue);
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementAlphaFade: {
							auto disprangeMinValue = GETFLOAT(effectBehaviorArrayItemVec[1]);
							auto disprangeMaxValue = GETFLOAT(effectBehaviorArrayItemVec[2]);

							auto item = ss::ssfb::CreateEffectParticleElementAlphaFade(ssfbBuilder, disprangeMinValue, disprangeMaxValue);
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementSize: {
							auto SizeXMinValue = GETFLOAT(effectBehaviorArrayItemVec[1]);
							auto SizeXMaxValue = GETFLOAT(effectBehaviorArrayItemVec[2]);
							auto SizeYMinValue = GETFLOAT(effectBehaviorArrayItemVec[3]);
							auto SizeYMaxValue = GETFLOAT(effectBehaviorArrayItemVec[4]);
							auto ScaleFactorMinValue = GETFLOAT(effectBehaviorArrayItemVec[5]);
							auto ScaleFactorMaxValue = GETFLOAT(effectBehaviorArrayItemVec[6]);

							auto item = ss::ssfb::CreateEffectParticleElementSize(ssfbBuilder,
																				  SizeXMinValue, SizeXMaxValue,
																				  SizeYMinValue, SizeYMaxValue,
																				  ScaleFactorMinValue, ScaleFactorMaxValue);
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementTransSize: {
							auto SizeXMinValue = GETFLOAT(effectBehaviorArrayItemVec[1]);
							auto SizeXMaxValue = GETFLOAT(effectBehaviorArrayItemVec[2]);
							auto SizeYMinValue = GETFLOAT(effectBehaviorArrayItemVec[3]);
							auto SizeYMaxValue = GETFLOAT(effectBehaviorArrayItemVec[4]);
							auto ScaleFactorMinValue = GETFLOAT(effectBehaviorArrayItemVec[5]);
							auto ScaleFactorMaxValue = GETFLOAT(effectBehaviorArrayItemVec[6]);

							auto item = ss::ssfb::CreateEffectParticleElementTransSize(ssfbBuilder,
																					   SizeXMinValue, SizeXMaxValue,
																					   SizeYMinValue, SizeYMaxValue,
																					   ScaleFactorMinValue, ScaleFactorMaxValue);
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticlePointGravity: {
							auto Position_x = GETFLOAT(effectBehaviorArrayItemVec[1]);
							auto Position_y = GETFLOAT(effectBehaviorArrayItemVec[2]);
							auto Power = GETFLOAT(effectBehaviorArrayItemVec[3]);

							auto item = ss::ssfb::CreateEffectParticlePointGravity(ssfbBuilder, Position_x, Position_y, Power);
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleTurnToDirectionEnabled: {
							auto Rotation = GETFLOAT(effectBehaviorArrayItemVec[1]);

							auto item = ss::ssfb::CreateEffectParticleTurnToDirectionEnabled(ssfbBuilder, Rotation);
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleInfiniteEmitEnabled: {
							auto flag = GETS32(effectBehaviorArrayItemVec[1]);

							auto item = ss::ssfb::CreateEffectParticleInfiniteEmitEnabled(ssfbBuilder, flag);
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_NONE:
						default:
							break;
					}
					auto serializeSsfbEffectNodeBehaviorType = ssfbBuilder.CreateVector(ssfbEffectNodeBehaviorType);
					auto serializeSsfbEffectNodeBehavior = ssfbBuilder.CreateVector(ssfbEffectNodeBehavior);
					auto ssfbEffectNodeItem = ss::ssfb::CreateEffectNode(ssfbBuilder,
																		 static_cast<int16_t>(arrayIndex),
																		 static_cast<int16_t>(parentIndex),
																		 type,
																		 static_cast<int16_t>(cellIndex),
																		 blendType,
																		 static_cast<int16_t>(ssfbEffectNodeBehavior.size()),
																		 serializeSsfbEffectNodeBehaviorType,
																		 serializeSsfbEffectNodeBehavior);
					ssfbEffectNode.push_back(ssfbEffectNodeItem);
				}
				auto serializeSsfbEffectNode = ssfbBuilder.CreateVector(ssfbEffectNode);
				auto ssfbEffectFile = ss::ssfb::CreateEffectFile(ssfbBuilder, ssfbEffectFileName,
																 fps, isLockRandSeed, LockRandSeed,
																 layoutScaleX, layoutScaleY,
																 numBehavior,serializeSsfbEffectNode);
				ssfbEffectFileList.push_back(ssfbEffectFile);
			}
		}
	}

	void createProjectData()
	{
		auto serializeSsfbCells = ssfbBuilder.CreateVector(ssfbCells);
		auto serializeSsfbAnimePackData = ssfbBuilder.CreateVector(ssfbAnimePacks);
		auto serializeSsfbEffectFileList = ssfbBuilder.CreateVector(ssfbEffectFileList);
		ssfbProjectData = ss::ssfb::CreateProjectData(ssfbBuilder, dataId, version, 0,
													  ssfbImageBaseDir,
													  serializeSsfbCells,
													  serializeSsfbAnimePackData,
													  serializeSsfbEffectFileList,
													  static_cast<int16_t>(ssfbCells.size()),
													  static_cast<int16_t>(ssfbAnimePacks.size()),
													  static_cast<int16_t>(ssfbEffectFileList.size()));
		ssfbBuilder.Finish(ssfbProjectData);
	}

	void writeFile(std::ostream& out) {
		out.write(reinterpret_cast<const char *>(ssfbBuilder.GetBufferPointer()), ssfbBuilder.GetSize());
	}
};
#undef GETS16
#undef GETS32
#undef GETFLOAT
#undef GETSTRING
#undef GETSSFBSTRING


void saveBinary(std::ostream& out, StringEncoding encoding, const Lump* lump, const std::string& creatorComment)
{
	BinaryExporter::save(out, encoding, lump, creatorComment);
}

void saveCSource(std::ostream& out, StringEncoding encoding, const Lump* lump, const std::string& topLabel, const std::string& creatorComment)
{
	CSourceExporter::save(out, encoding, lump, topLabel, creatorComment);
}

void saveJson(std::ostream& out, StringEncoding encoding, const Lump* lump, const std::string& creatorComment)
{
	JsonExporter::save(out, encoding, lump, creatorComment);
}

void saveSsfb(std::ostream &out, StringEncoding encoding, const Lump *lump, const std::string &creatorComment,
              const std::vector<int16_t> &frameIndexVec)
{
    SsfbExporter::save(out, encoding, lump, creatorComment, frameIndexVec);
}


}	// namespace LumpExporter

