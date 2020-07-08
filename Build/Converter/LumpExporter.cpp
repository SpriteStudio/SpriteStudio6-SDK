//
//  LumpExporter.cpp
//

#include "LumpExporter.h"
#include "BinaryDataWriter.h"
#include <assert.h>
#include <cstdarg>
#include <flatbuffers/idl.h>
#include "sscharconverter.h"
#include "picojson.h"
#include "flatbuffers/flatbuffers.h"
#include "ssfb_generated.h"
#include "Lump.h"
#include "ssfb_fbs_array.h"

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

static std::string encode(const std::string& str, StringEncoding encoding)
{
	switch (encoding) {
		case UTF8: return str;
		case SJIS: return SsCharConverter::utf8_to_sjis(str); // TODO:
		default:
			break;
	}
	return str;
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
#define GETU32(l) (uint32_t)(l->data.i)
#define GETFLOAT(l) (float)(l->data.f)
#define GETSTRING(l, enc) encode(*l->data.s, enc)
#define GETSSFBSTRING(builder, l, enc) builder.CreateSharedString(GETSTRING(l, enc))

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
	std::string m_curEffectFile;

	flatbuffers::FlatBufferBuilder m_ssfbBuilder;
	flatbuffers::Offset<ss::ssfb::ProjectData> m_ssfbProjectData;
	int32_t m_dataId{};
	int32_t m_version{};
	flatbuffers::Offset<flatbuffers::String> m_ssfbImageBaseDir;
	std::vector<flatbuffers::Offset<ss::ssfb::Cell>> m_ssfbCells;
	std::vector<flatbuffers::Offset<ss::ssfb::AnimePackData>> m_ssfbAnimePacks;
	std::vector<flatbuffers::Offset<ss::ssfb::EffectFile>> m_ssfbEffectFileList;

	std::vector<std::shared_ptr<struct ss::ssfb::CellMapT>> m_cellMaps;
	std::vector<flatbuffers::Offset<ss::ssfb::CellMap>> m_ssfbCellMaps;

	std::vector<std::shared_ptr<struct ss::ssfb::AnimationInitialDataT>> m_animationInitialDataVec;
	std::vector<flatbuffers::Offset<ss::ssfb::AnimationInitialData>> m_ssfbAnimationInitialDataVec;

	std::vector<std::shared_ptr<struct ss::ssfb::PartDataT>> m_partDataVec;
	std::vector<flatbuffers::Offset<ss::ssfb::PartData>> m_ssfbPartDataVec;

	std::vector<std::vector<uint32_t>> m_uint32VecVec;
	std::vector<flatbuffers::Offset<flatbuffers::Vector<uint32_t>>> m_ssfbUint32VecVec;

	std::vector<std::vector<float>> m_floatVecVec;
	std::vector<flatbuffers::Offset<flatbuffers::Vector<float>>> m_ssfbFloatVecVec;

	std::vector<std::shared_ptr<struct ss::ssfb::MeshDataUVT>> m_meshDataUVVec;
	std::vector<flatbuffers::Offset<ss::ssfb::MeshDataUV>> m_ssfbMeshDataUVVec;

	std::vector<std::shared_ptr<struct ss::ssfb::MeshDataIndicesT>> m_meshDataIndicesVec;
	std::vector<flatbuffers::Offset<ss::ssfb::MeshDataIndices>> m_ssfbMeshDataIndicesVec;

	std::vector<std::shared_ptr<struct ss::ssfb::PartStateT>> m_partStateVec;
	std::vector<flatbuffers::Offset<ss::ssfb::PartState>> m_ssfbPartStateVec;

	std::vector<std::shared_ptr<struct ss::ssfb::FrameDataIndexT>> m_frameDataIndexVec;
	std::vector<flatbuffers::Offset<ss::ssfb::FrameDataIndex>> m_ssfbFrameDataIndexVec;

	std::vector<std::shared_ptr<struct ss::ssfb::EffectParticleElementBasicT>> m_effectParticleElementBasicVec;
    std::vector<flatbuffers::Offset<ss::ssfb::EffectParticleElementBasic>> m_ssfbEffectParticleElementBasicVec;

	std::vector<std::shared_ptr<struct ss::ssfb::EffectNodeT>> m_effectNodeVec;
	std::vector<flatbuffers::Offset<ss::ssfb::EffectNode>> m_ssfbEffectNodeVec;

    std::map<std::string, std::map<int, std::shared_ptr<struct ss::ssfb::EffectNodeT>>> m_effectNodeCatalog;
	std::map<std::string, std::map<int, flatbuffers::Offset<ss::ssfb::EffectNode>>> m_ssfbEffectNodeCatalog;

	std::map<std::string, flatbuffers::Offset<ss::ssfb::EffectFile>> m_ssfbEffectFileCatalog;
	std::vector<std::shared_ptr<struct ss::ssfb::EffectFileT>> m_effectFileVec;
	std::vector<flatbuffers::Offset<ss::ssfb::EffectFile>> m_ssfbEffectFileVec;

	void createHeader()
	{
		auto rootChildVec = m_root->getChildren();
		m_dataId = GETS32(rootChildVec[0]);
		m_version = GETS32(rootChildVec[1]);
		// GETS32(rootChildVec[2]); // unuse
		m_ssfbImageBaseDir = GETSSFBSTRING(m_ssfbBuilder, rootChildVec[3], m_encoding);
	}

	void createCells()
	{
		auto rootChildVec = m_root->getChildren();
		// 4:Cell
		auto cellsLump = rootChildVec[4];

		auto cellsVec = cellsLump->getChildren();
		for(auto cellItem : cellsVec) {
			auto cellItemVec = cellItem->getChildren();
			auto ssfbCellMap = createSharedCellMap(cellItemVec[1]);
			auto ssfbCellName = GETSSFBSTRING(m_ssfbBuilder, cellItemVec[0], m_encoding);

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

			auto ssfbCell = ss::ssfb::CreateCell(m_ssfbBuilder, ssfbCellName, ssfbCellMap,
												 indexInCellMap, x, y, width, height,
												 pivot_x, pivot_y, u1, v1, u2, v2);
			m_ssfbCells.push_back(ssfbCell);
		}
	}

	flatbuffers::Offset<ss::ssfb::CellMap> createSharedCellMap(const Lump *lump)
	{
		flatbuffers::Offset<ss::ssfb::CellMap> cellMap;

		auto cellMapVec = lump->getChildren();

		std::shared_ptr<struct ss::ssfb::CellMapT> cellMapT(new ss::ssfb::CellMapT());
		cellMapT->name = GETSTRING(cellMapVec[0], m_encoding);
		cellMapT->image_path = GETSTRING(cellMapVec[1], m_encoding);
		cellMapT->index = GETS16(cellMapVec[2]);
		cellMapT->wrap_mode = (ss::ssfb::TexWrapMode)GETS16(cellMapVec[3]);
		cellMapT->filter_mode = (ss::ssfb::TexFilterMode)GETS16(cellMapVec[4]);
		// 5:reserved(s16)

		// search same cellMap from cellMap caches.
		auto result = std::find_if(m_cellMaps.begin(), m_cellMaps.end(), [cellMapT](const std::shared_ptr<ss::ssfb::CellMapT> &c) {
		    return *(c.get()) == *(cellMapT.get());
		});
		if (result == m_cellMaps.end()) {
			// not found

			// create ssfb cellMap
			auto ssfbCellMapName = m_ssfbBuilder.CreateSharedString(cellMapT->name);
			auto ssfbCellMapImagePath = m_ssfbBuilder.CreateSharedString(cellMapT->image_path);
			cellMap = ss::ssfb::CreateCellMap(m_ssfbBuilder, ssfbCellMapName, ssfbCellMapImagePath,
											  cellMapT->index, cellMapT->wrap_mode, cellMapT->filter_mode);
			// cache ssfb cellMap
			m_cellMaps.push_back(cellMapT);
			m_ssfbCellMaps.push_back(cellMap);
		} else {
			// found
			auto idx = std::distance(m_cellMaps.begin(), result);
			cellMap = m_ssfbCellMaps[idx];
		}

		return cellMap;
	}

	flatbuffers::Offset<ss::ssfb::AnimationInitialData> createSharedAnimationInitialData(const Lump *lump)
	{
		flatbuffers::Offset<ss::ssfb::AnimationInitialData> animationInitialData;
		auto AnimationInitialDataItemVec = lump->getChildren();

		std::shared_ptr<struct ss::ssfb::AnimationInitialDataT> animationInitialDataT(new ss::ssfb::AnimationInitialDataT());

		animationInitialDataT->index = GETS16(AnimationInitialDataItemVec[0]);
		// 1:reserve
		animationInitialDataT->lowflag = GETS32(AnimationInitialDataItemVec[2]);
		animationInitialDataT->highflag = GETS32(AnimationInitialDataItemVec[3]);
		animationInitialDataT->priority = GETS16(AnimationInitialDataItemVec[4]);
		animationInitialDataT->cell_index = GETS16(AnimationInitialDataItemVec[5]);
		animationInitialDataT->opacity = GETS16(AnimationInitialDataItemVec[6]);
		animationInitialDataT->local_opacity = GETS16(AnimationInitialDataItemVec[7]);
		animationInitialDataT->masklimen = GETS16(AnimationInitialDataItemVec[8]);
		// 9:reserved
		animationInitialDataT->position_x = GETFLOAT(AnimationInitialDataItemVec[10]);
		animationInitialDataT->position_y = GETFLOAT(AnimationInitialDataItemVec[11]);
		animationInitialDataT->position_z = GETFLOAT(AnimationInitialDataItemVec[12]);
		animationInitialDataT->pivot_x = GETFLOAT(AnimationInitialDataItemVec[13]);
		animationInitialDataT->pivot_y = GETFLOAT(AnimationInitialDataItemVec[14]);
		animationInitialDataT->rotation_x = GETFLOAT(AnimationInitialDataItemVec[15]);
		animationInitialDataT->rotation_y = GETFLOAT(AnimationInitialDataItemVec[16]);
		animationInitialDataT->rotation_z = GETFLOAT(AnimationInitialDataItemVec[17]);
		animationInitialDataT->scale_x = GETFLOAT(AnimationInitialDataItemVec[18]);
		animationInitialDataT->scale_y = GETFLOAT(AnimationInitialDataItemVec[19]);
		animationInitialDataT->local_scale_x = GETFLOAT(AnimationInitialDataItemVec[20]);
		animationInitialDataT->local_scale_y = GETFLOAT(AnimationInitialDataItemVec[21]);
		animationInitialDataT->size_x = GETFLOAT(AnimationInitialDataItemVec[22]);
		animationInitialDataT->size_y = GETFLOAT(AnimationInitialDataItemVec[23]);
		animationInitialDataT->uv_move_x = GETFLOAT(AnimationInitialDataItemVec[24]);
		animationInitialDataT->uv_move_y = GETFLOAT(AnimationInitialDataItemVec[25]);
		animationInitialDataT->uv_rotation = GETFLOAT(AnimationInitialDataItemVec[26]);
		animationInitialDataT->uv_scale_x = GETFLOAT(AnimationInitialDataItemVec[27]);
		animationInitialDataT->uv_scale_y = GETFLOAT(AnimationInitialDataItemVec[28]);
		animationInitialDataT->bounding_radius = GETFLOAT(AnimationInitialDataItemVec[29]);
		//インスタンス関連
		animationInitialDataT->instance_value_cur_keyframe = GETS32(AnimationInitialDataItemVec[30]);
		animationInitialDataT->instance_value_start_frame = GETS32(AnimationInitialDataItemVec[31]);
		animationInitialDataT->instance_value_end_frame = GETS32(AnimationInitialDataItemVec[32]);
		animationInitialDataT->instance_value_loop_num = GETS32(AnimationInitialDataItemVec[33]);
		animationInitialDataT->instance_value_speed = GETFLOAT(AnimationInitialDataItemVec[34]);
		animationInitialDataT->instance_value_loop_flag = GETS32(AnimationInitialDataItemVec[35]);
		//エフェクト関連
		animationInitialDataT->effect_value_cur_keyframe = GETS32(AnimationInitialDataItemVec[36]);
		animationInitialDataT->effect_value_start_time = GETS32(AnimationInitialDataItemVec[37]);
		animationInitialDataT->effect_value_speed = GETFLOAT(AnimationInitialDataItemVec[38]);
		animationInitialDataT->effect_value_loop_flag = GETS32(AnimationInitialDataItemVec[39]);

		// search same cellMap from cellMap caches.
		auto result = std::find_if(m_animationInitialDataVec.begin(), m_animationInitialDataVec.end(), [animationInitialDataT](const std::shared_ptr<ss::ssfb::AnimationInitialDataT> &c) {
		    return *(c.get()) == *(animationInitialDataT.get());
		});
		if (result == m_animationInitialDataVec.end()) {
			// not found

			// create ssfb partData
			//animationInitialData = m_ssfbBuilder.Create
			animationInitialData = ss::ssfb::CreateAnimationInitialData(m_ssfbBuilder, animationInitialDataT.get());
			// cache ssfb cellMap
			m_animationInitialDataVec.push_back(animationInitialDataT);
			m_ssfbAnimationInitialDataVec.push_back(animationInitialData);
		} else {
			// found
			auto idx = std::distance(m_animationInitialDataVec.begin(), result);
			animationInitialData = m_ssfbAnimationInitialDataVec[idx];
		}

		return animationInitialData;
	}

	flatbuffers::Offset<ss::ssfb::PartData> createSharedPartData(const Lump *lump)
	{
		flatbuffers::Offset<ss::ssfb::PartData> partData;
		auto partDataItemVec = lump->getChildren();

		std::shared_ptr<struct ss::ssfb::PartDataT> partDataT(new ss::ssfb::PartDataT());

		partDataT->name = GETSTRING(partDataItemVec[0], m_encoding);
		partDataT->index = GETS16(partDataItemVec[1]);
		partDataT->parent_index = GETS16(partDataItemVec[2]);
		partDataT->type = (ss::ssfb::SsPartType)GETS16(partDataItemVec[3]);
		partDataT->bounds_type = (ss::ssfb::BoundsType)GETS16(partDataItemVec[4]);
		partDataT->alpha_blend_type = (ss::ssfb::BlendType )GETS16(partDataItemVec[5]);
		partDataT->refname = GETSTRING(partDataItemVec[7], m_encoding);
		auto effectfilename = GETSTRING(partDataItemVec[8], m_encoding);
		auto effect_result = std::find_if(m_effectFileVec.begin(), m_effectFileVec.end(), [effectfilename](const std::shared_ptr<ss::ssfb::EffectFileT> &c) {
            return effectfilename == c->name;
		});
		int ssfbEffectFileIdx = -1;
		if(effect_result == m_effectFileVec.end()) {
		    partDataT->effect = nullptr;
		} else {
		    partDataT->effect = *effect_result;
		    ssfbEffectFileIdx = std::distance(m_effectFileVec.begin(), effect_result);
		}

		partDataT->colorLabel = GETSTRING(partDataItemVec[9], m_encoding);
		partDataT->mask_influence = (bool)GETS16(partDataItemVec[10]);

		// search same cellMap from cellMap caches.
		auto result = std::find_if(m_partDataVec.begin(), m_partDataVec.end(), [partDataT](const std::shared_ptr<ss::ssfb::PartDataT> &c) {
		    return *(c.get()) == *(partDataT.get());
		});
		if (result == m_partDataVec.end()) {
			// not found

			// create ssfb partData
			auto ssfbPartDataName =  m_ssfbBuilder.CreateSharedString(partDataT->name);
			auto ssfbRefname = m_ssfbBuilder.CreateSharedString(partDataT->refname);
			auto ssfbEffectFile = (ssfbEffectFileIdx >=0)? m_ssfbEffectFileVec[ssfbEffectFileIdx] : 0;
			auto ssfbColorLabel = m_ssfbBuilder.CreateSharedString(partDataT->colorLabel);

			partData = ss::ssfb::CreatePartData(m_ssfbBuilder, ssfbPartDataName,partDataT->index, partDataT->parent_index, (ss::ssfb::SsPartType)partDataT->type,
												partDataT->bounds_type, partDataT->alpha_blend_type, ssfbRefname, ssfbEffectFile, ssfbColorLabel,
												partDataT->mask_influence);
			// cache ssfb cellMap
			m_partDataVec.push_back(partDataT);
			m_ssfbPartDataVec.push_back(partData);
		} else {
			// found
			auto idx = std::distance(m_partDataVec.begin(), result);
			partData = m_ssfbPartDataVec[idx];
		}

		return partData;
	}

	flatbuffers::Offset<flatbuffers::Vector<uint32_t>> createSharedUint32Vec(const std::vector<uint32_t> &vec) {
		flatbuffers::Offset<flatbuffers::Vector<uint32_t>> ssfbVec;

		auto result = std::find(m_uint32VecVec.begin(), m_uint32VecVec.end(), vec);
		if (result == m_uint32VecVec.end()) {
			// not found

			// create ssfb vec
			ssfbVec = m_ssfbBuilder.CreateVector(vec);

			// cache ssfb vec
			m_uint32VecVec.push_back(vec);
			m_ssfbUint32VecVec.push_back(ssfbVec);
		} else {
			auto idx = std::distance(m_uint32VecVec.begin(), result);
			ssfbVec = m_ssfbUint32VecVec[idx];
		}

		return ssfbVec;
	}

	flatbuffers::Offset<flatbuffers::Vector<float>> createSharedFloatVec(const std::vector<float> &vec) {
		flatbuffers::Offset<flatbuffers::Vector<float>> ssfbVec;

		auto result = std::find(m_floatVecVec.begin(), m_floatVecVec.end(), vec);
		if (result == m_floatVecVec.end()) {
			// not found

			// create ssfb vec
			ssfbVec = m_ssfbBuilder.CreateVector(vec);

			// cache ssfb vec
			m_floatVecVec.push_back(vec);
			m_ssfbFloatVecVec.push_back(ssfbVec);
		} else {
			auto idx = std::distance(m_floatVecVec.begin(), result);
			ssfbVec = m_ssfbFloatVecVec[idx];
		}

		return ssfbVec;
	}

	flatbuffers::Offset<ss::ssfb::MeshDataUV> createSharedMeshDataUV(const std::vector<float> &uvPrimitive, const flatbuffers::Offset<flatbuffers::Vector<float>> &uv) {
		flatbuffers::Offset<ss::ssfb::MeshDataUV> meshDataUV;

		std::shared_ptr<struct ss::ssfb::MeshDataUVT> meshDataUVT(new ss::ssfb::MeshDataUVT());
		meshDataUVT->uv = uvPrimitive;

		auto result = std::find_if(m_meshDataUVVec.begin(), m_meshDataUVVec.end(), [meshDataUVT](const std::shared_ptr<ss::ssfb::MeshDataUVT> &c) {
		    return *(c.get()) == *(meshDataUVT.get());
		});
		if (result == m_meshDataUVVec.end()) {
			// not found

			// create ssfb vec
			meshDataUV = ss::ssfb::CreateMeshDataUV(m_ssfbBuilder, uv);

			// cache ssfb vec
			m_meshDataUVVec.push_back(meshDataUVT);
			m_ssfbMeshDataUVVec.push_back(meshDataUV);
		} else {
			auto idx = std::distance(m_meshDataUVVec.begin(), result);
			meshDataUV = m_ssfbMeshDataUVVec[idx];
		}

		return meshDataUV;
	}

	flatbuffers::Offset<ss::ssfb::MeshDataIndices> createSharedMeshDataIndices(const std::vector<float> &indicesPrimitive, const flatbuffers::Offset<flatbuffers::Vector<float>> &indices) {
		flatbuffers::Offset<ss::ssfb::MeshDataIndices> meshDataIndices;

		std::shared_ptr<struct ss::ssfb::MeshDataIndicesT> meshDataIndicesT(new ss::ssfb::MeshDataIndicesT());
		meshDataIndicesT->indices = indicesPrimitive;
		auto result = std::find_if(m_meshDataIndicesVec.begin(), m_meshDataIndicesVec.end(), [meshDataIndicesT](const std::shared_ptr<ss::ssfb::MeshDataIndicesT> &c) {
		    return *(c.get()) == *(meshDataIndicesT.get());
		});
		if (result == m_meshDataIndicesVec.end()) {
			// not found

			// create ssfb vec
			meshDataIndices = ss::ssfb::CreateMeshDataIndices(m_ssfbBuilder, indices);

			// cache ssfb vec
			m_meshDataIndicesVec.push_back(meshDataIndicesT);
			m_ssfbMeshDataIndicesVec.push_back(meshDataIndices);
		} else {
			auto idx = std::distance(m_meshDataIndicesVec.begin(), result);
			meshDataIndices = m_ssfbMeshDataIndicesVec[idx];
		}

		return meshDataIndices;
	}
	
	flatbuffers::Offset<ss::ssfb::PartState>
	createSharedPartState(int16_t index, ss::ssfb::PartFlag flag1, ss::ssfb::PartFlag2 flag2, const std::vector<uint32_t> &dataPrimitive) {
		flatbuffers::Offset<ss::ssfb::PartState> partState;
		
		std::shared_ptr<struct ss::ssfb::PartStateT> partStateT(new ss::ssfb::PartStateT());
		partStateT->index = index;
		partStateT->flag1 = flag1;
		partStateT->flag2 = flag2;
		partStateT->data = dataPrimitive;
		auto result = std::find_if(m_partStateVec.begin(), m_partStateVec.end(), [partStateT](const std::shared_ptr<ss::ssfb::PartStateT> &c) {
		    return *(c.get()) == *(partStateT.get());
		});
		if (result == m_partStateVec.end()) {
			// not found
			auto serializePartStateData = createSharedUint32Vec(dataPrimitive);
			partState = ss::ssfb::CreatePartState(m_ssfbBuilder, partStateT->index, partStateT->flag1, partStateT->flag2, serializePartStateData);

			m_partStateVec.push_back(partStateT);
			m_ssfbPartStateVec.push_back(partState);
		} else {
			auto idx = std::distance(m_partStateVec.begin(), result);
			partState = m_ssfbPartStateVec[idx];
		}

		return partState;
	}

	flatbuffers::Offset<ss::ssfb::FrameDataIndex> createSharedFrameDataIndex(const std::vector<ss::ssfb::PartStateT> &statesPrimitive) {
		flatbuffers::Offset<ss::ssfb::FrameDataIndex> frameDataIndex;

		std::shared_ptr<struct ss::ssfb::FrameDataIndexT> frameDataIndexT1(new ss::ssfb::FrameDataIndexT());
		for(auto state : statesPrimitive) {
			std::unique_ptr<ss::ssfb::PartStateT> p(new ss::ssfb::PartStateT());
			p->index = state.index;
			p->flag1 = state.flag1;
			p->flag2 = state.flag2;
			p->data = state.data;
			frameDataIndexT1->states.push_back(std::move(p));
		}
		auto result = std::find_if(m_frameDataIndexVec.begin(), m_frameDataIndexVec.end(), [&frameDataIndexT1](const struct std::shared_ptr<ss::ssfb::FrameDataIndexT> &item) {
			if(frameDataIndexT1->states.size() != item->states.size())
				return false;

			int idx = 0;
			for(auto &p : frameDataIndexT1->states) {
				auto &i = item->states[idx];
				if(p->index != i->index) return false;
				if(p->flag1 != i->flag1) return false;
				if(p->flag2 != i->flag2) return false;
				if(p->data != i->data) return false;

				idx++;
			}
			return true;
		});
		if (result == m_frameDataIndexVec.end()) {
			// not found

			std::vector<flatbuffers::Offset<ss::ssfb::PartState>> vec;
			for (auto i : statesPrimitive) {
				auto item = createSharedPartState(i.index, i.flag1, i.flag2, i.data);
				vec.push_back(item);
			}

			auto serializeVec = m_ssfbBuilder.CreateVector(vec);
			// create ssfb vec
			frameDataIndex = ss::ssfb::CreateFrameDataIndex(m_ssfbBuilder, serializeVec);

			// cache ssfb vec
			m_frameDataIndexVec.push_back(std::move(frameDataIndexT1));
			m_ssfbFrameDataIndexVec.push_back(frameDataIndex);
		} else {
			auto idx = std::distance(m_frameDataIndexVec.begin(), result);
			frameDataIndex = m_ssfbFrameDataIndexVec[idx];
		}

		return frameDataIndex;
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

			ssfbAnimePackDataName = GETSSFBSTRING(m_ssfbBuilder, animePackDataItemVec[0], m_encoding);
			ssfbParts = createParts(animePackDataItemVec[1]); // PartData
			ssfbAnimations = createAnimationDataList(animePackDataItemVec[2]); // AnimationData
			// auto numPart = GETS16(animePackDataItemVec[3]); // unused
			// auto numAnimations = GETS16(animePackDataItemVec[4]); // unused

			auto serializeSsfbParts = m_ssfbBuilder.CreateVector(ssfbParts);
			auto serializeSsfbAnimations = m_ssfbBuilder.CreateVector(ssfbAnimations);
			auto ssfbAnimePackData = ss::ssfb::CreateAnimePackData(m_ssfbBuilder, ssfbAnimePackDataName, serializeSsfbParts, serializeSsfbAnimations);
			m_ssfbAnimePacks.push_back(ssfbAnimePackData);
		}
	}

	std::vector<flatbuffers::Offset<ss::ssfb::PartData>> createParts(const Lump* lump)
	{
		std::vector<flatbuffers::Offset<ss::ssfb::PartData>> ssfbParts;

		auto partDataVec = lump->getChildren();
		for(auto partDataItem : partDataVec) {
			auto ssfbPartDataItem = createSharedPartData(partDataItem);
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

			// 1:AnimationInitialData

			std::vector<flatbuffers::Offset<ss::ssfb::AnimationInitialData>> ssfbDefaultData;
			{
				auto AnimationInitialDataVec = ssAnimationDataVec[1]->getChildren();
				for(auto AnimationInitialDataItem : AnimationInitialDataVec)
				{
					auto item = createSharedAnimationInitialData(AnimationInitialDataItem);
					ssfbDefaultData.push_back(item);
				}
			}

			// 5:meshDataUV
			std::vector<flatbuffers::Offset<ss::ssfb::MeshDataUV>> ssfbMeshsDataUV;
			{
				auto meshDataUVVec = ssAnimationDataVec[5]->getChildren();
				for(auto meshDataUVItem : meshDataUVVec) {
				    std::vector<float> ssfbUV;
					auto frameDataVec = meshDataUVItem->getChildren();
					for(auto frameDataItem : frameDataVec) {
						switch (frameDataItem->type) {
						case Lump::DataType::S32:
							ssfbUV.push_back(GETS32(frameDataItem));
							break;
						case Lump::DataType::FLOAT:
							ssfbUV.push_back(GETFLOAT(frameDataItem));
							break;
						default:
							break;
						}
					}

					auto serializeSsfbUV = createSharedFloatVec(ssfbUV);
					auto item = createSharedMeshDataUV(ssfbUV, serializeSsfbUV);
					ssfbMeshsDataUV.push_back(item);
				}
			}
			// 6:meshsDataIndices
			std::vector<flatbuffers::Offset<ss::ssfb::MeshDataIndices>> ssfbMeshsDataIndices;
			{
				auto meshsDataIndicesVec = ssAnimationDataVec[6]->getChildren();
				for(auto meshsDataIndicesItem : meshsDataIndicesVec) {
				    std::vector<float> ssfbIndices;
					auto meshsDataVec = meshsDataIndicesItem->getChildren();
					for(auto meshDataItem : meshsDataVec) {
						ssfbIndices.push_back(GETS32(meshDataItem));
					}

					auto serializeSsfbIndices = createSharedFloatVec(ssfbIndices);
					auto item = createSharedMeshDataIndices(ssfbIndices, serializeSsfbIndices);
					ssfbMeshsDataIndices.push_back(item);
				}
			}
			// 2:frameDataIndexArray

			std::vector<flatbuffers::Offset<ss::ssfb::FrameDataIndex>> ssfbFrameData;
			{
				auto frameDataIndexArrayVec = ssAnimationDataVec[2]->getChildren();

				for(auto frameDataIndexArrayItem : frameDataIndexArrayVec) {
					auto frameDataVec = frameDataIndexArrayItem->getChildren();

					struct ss::ssfb::PartStateT partStateTItem;
					std::vector<struct ss::ssfb::PartStateT> partStateTVec;

					ss::ssfb::PartFlag flag1;
					ss::ssfb::PartFlag2 flag2;
					int outPartsCount = -1;
					int16_t index;
					std::string tagname;
					std::vector<uint32_t > partStateData;
					for(auto frameDataItem : frameDataVec) {
						if(frameDataItem->name.find("part_") != std::string::npos &&
						   frameDataItem->name.find("_index") != std::string::npos) {
							if(outPartsCount != -1) {
								partStateTItem.index = index;
								partStateTItem.flag1 = flag1;
								partStateTItem.flag2 = flag2;
								partStateTItem.data = partStateData;

								partStateTVec.push_back(partStateTItem);
								outPartsCount++;
							} else {
								outPartsCount = 0;
							}

							index = GETS16(frameDataItem);
							tagname = "part_" + std::to_string(outPartsCount) + "_";
							flag1 = ss::ssfb::PartFlag::PartFlag_NONE;
							flag2 = ss::ssfb::PartFlag2::PartFlag2_NONE;
							partStateData = {};
							continue;
						}

						if(frameDataItem->name == tagname + "flag1") {
							flag1 = (ss::ssfb::PartFlag)GETU32(frameDataItem);
							continue;
						}

						if(frameDataItem->name == tagname + "flag2") {
							flag2 = (ss::ssfb::PartFlag2)GETU32(frameDataItem);
							continue;
						}

						switch (frameDataItem->type) {
							case Lump::DataType::S16:
								partStateData.push_back(GETS16(frameDataItem));
								break;
							case Lump::DataType::S32:
								partStateData.push_back(GETS32(frameDataItem));
								break;
							case Lump::DataType::FLOAT:
								partStateData.push_back(GETS32(frameDataItem));
								break;
							case Lump::DataType::COLOR:
								{
									auto rgba = GETU32(frameDataItem);
									partStateData.push_back((rgba & 0xffff0000) >> 16);
									partStateData.push_back(rgba & 0xffff);
								}
								break;
							default:
								break;
						}
					}

					partStateTItem.index = index;
					partStateTItem.flag1 = flag1;
					partStateTItem.flag2 = flag2;
					partStateTItem.data = partStateData;
					partStateTVec.push_back(partStateTItem);

					auto item = createSharedFrameDataIndex(partStateTVec);
					ssfbFrameData.push_back(item);
				}
			}
			// 3:userDataIndexArray
			std::vector<flatbuffers::Offset<ss::ssfb::UserDataPerFrame>> ssfbUserData;
			{
				if(ssAnimationDataVec[3]->type == Lump::DataType::SET) {

					auto userDataIndexArrayVec = ssAnimationDataVec[3]->getChildren();
					for(auto userDataIndexArrayItem : userDataIndexArrayVec) {
						if(userDataIndexArrayItem->type != Lump::DataType::SET) {
							continue;
						}
						std::vector<flatbuffers::Offset<ss::ssfb::UserDataItem>> ssfbUserDataItemData;

						std::vector<flatbuffers::Offset<void>> ssfbDataArray;
						std::vector<uint8_t> ssfbDataArrayType;

						auto userDataIndexArrayItemVec = userDataIndexArrayItem->getChildren();
						auto num = GETS16(userDataIndexArrayItemVec[0]);
						int idx = 1;

						for(int i=0; i<num; i++) {
                            int integer = 0;
                            int rect_x = 0;
                            int rect_y = 0;
                            int rect_w = 0;
                            int rect_h = 0;
                            int point_x = 0;
                            int point_y = 0;
                            int user_string_length = 0;
                            flatbuffers::Offset<flatbuffers::String> user_string = 0;

							auto flags = (ss::ssfb::UserDataFlag)GETS16(userDataIndexArrayItemVec[idx++]);
							auto arrayIndex = GETS16(userDataIndexArrayItemVec[idx++]);

							if(flags & ss::ssfb::UserDataFlag::UserDataFlag_Integer) {
                                integer = GETS32(userDataIndexArrayItemVec[idx++]);
							}
							if(flags & ss::ssfb::UserDataFlag::UserDataFlag_Rect) {
								rect_x = GETS32(userDataIndexArrayItemVec[idx++]);
								rect_y = GETS32(userDataIndexArrayItemVec[idx++]);
								rect_w = GETS32(userDataIndexArrayItemVec[idx++]);
								rect_h = GETS32(userDataIndexArrayItemVec[idx++]);
							}
							if(flags & ss::ssfb::UserDataFlag::UserDataFlag_Point) {
								point_x = GETS32(userDataIndexArrayItemVec[idx++]);
								point_y = GETS32(userDataIndexArrayItemVec[idx++]);
							}
							if(flags & ss::ssfb::UserDataFlag::UserDataFlag_String) {
								user_string_length = GETS16(userDataIndexArrayItemVec[idx++]);
								user_string = GETSSFBSTRING(m_ssfbBuilder, userDataIndexArrayItemVec[idx++], m_encoding);
							}
							auto item = ss::ssfb::CreateUserDataItem(m_ssfbBuilder, flags, arrayIndex, integer, rect_x, rect_y, rect_w, rect_h, point_x, point_y, user_string_length, user_string);
							ssfbUserDataItemData.push_back(item);
						}
						auto serializeSsfbUserDataItemData = m_ssfbBuilder.CreateVector(ssfbUserDataItemData);

						auto frame = this->m_frameIndexVec[this->m_frameIndex++];
						auto item = ss::ssfb::CreateUserDataPerFrame(m_ssfbBuilder, frame, serializeSsfbUserDataItemData);
						ssfbUserData.push_back(item);
					}
				}
			}
			// 4:LabelDataIndexArray
			std::vector<flatbuffers::Offset<ss::ssfb::LabelDataItem>> ssfbLabelData;
			{
				if(ssAnimationDataVec[4]->type == Lump::DataType::SET) {
					auto LabelDataIndexArrayVec = ssAnimationDataVec[4]->getChildren();
					for(auto LabelDataIndexArrayItem : LabelDataIndexArrayVec) {
						auto labelDataVec = LabelDataIndexArrayItem->getChildren();
						auto ssfbLabelDataItemName = GETSSFBSTRING(m_ssfbBuilder, labelDataVec[0], m_encoding);
						auto time = GETS16(labelDataVec[1]);

						auto item = ss::ssfb::CreateLabelDataItem(m_ssfbBuilder, ssfbLabelDataItemName, time);
						ssfbLabelData.push_back(item);
					}
				}
			}

			auto ssfbAnimationDataName = GETSSFBSTRING(m_ssfbBuilder, ssAnimationDataVec[0], m_encoding);
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

			auto serializeSsfbDefaultData = m_ssfbBuilder.CreateVector(ssfbDefaultData);
			auto serializeSsfbMeshsDataUV = m_ssfbBuilder.CreateVector(ssfbMeshsDataUV);
			auto serializeSsfbMeshsDataIndices = m_ssfbBuilder.CreateVector(ssfbMeshsDataIndices);

            auto serializeSsfbFrameData = m_ssfbBuilder.CreateVector(ssfbFrameData);
            auto serializeSsfbUserData = m_ssfbBuilder.CreateVector(ssfbUserData);
            auto serializeSsfbLabelData = m_ssfbBuilder.CreateVector(ssfbLabelData);

            auto item = ss::ssfb::CreateAnimationData(m_ssfbBuilder, ssfbAnimationDataName,
                                                      serializeSsfbDefaultData, serializeSsfbFrameData, serializeSsfbUserData,
                                                      serializeSsfbLabelData, serializeSsfbMeshsDataUV, serializeSsfbMeshsDataIndices,
                                                      startFrames, endFrames, totalFrames, fps, labelNum, canvasSizeW, canvasSizeH, canvasPvotX, canvasPvotY);

            ssfbAnimationDataList.push_back(item);
		}

		return ssfbAnimationDataList;
	}

	flatbuffers::Offset<ss::ssfb::EffectNode> createSharedEffectNode(const Lump *EffectNodeArrayItem, std::shared_ptr<ss::ssfb::EffectNodeT> &effectNodeItem) {
        auto EffectNodeVec = EffectNodeArrayItem->getChildren();

        //std::shared_ptr<ss::ssfb::EffectNodeT> effectNodeItem(new ss::ssfb::EffectNodeT());
        flatbuffers::Offset<ss::ssfb::EffectNode> ssfbEffectNodeItem;

        effectNodeItem->array_index = GETS16(EffectNodeVec[0]);
        effectNodeItem->parent_index = GETS16(EffectNodeVec[1]);
        effectNodeItem->type = (ss::ssfb::EffectNodeType) GETS16(EffectNodeVec[2]);
        effectNodeItem->cell_index = GETS16(EffectNodeVec[3]);
        effectNodeItem->blend_type = (ss::ssfb::EffectRenderBlendType) GETS16(EffectNodeVec[4]);
        // GETS16(EffectNodeVec[5]); unused

        auto effectBehaviorArrayVec = EffectNodeVec[6]->getChildren();

        uint32_t behaviorFlags = ss::ssfb::EffectBehaviorFlags_NONE;
        std::shared_ptr<ss::ssfb::EffectParticleElementBasicT> basicBehavior(new ss::ssfb::EffectParticleElementBasicT());
        flatbuffers::Offset<ss::ssfb::EffectParticleElementBasic> ssfbBasicBehavior;

        for (auto effectBehaviorArrayItem : effectBehaviorArrayVec) {
            auto effectBehaviorArrayItemVec = effectBehaviorArrayItem->getChildren();
            auto SsEffectFunctionType = (ss::ssfb::SsEffectFunctionType) GETS32(effectBehaviorArrayItemVec[0]);
            switch (SsEffectFunctionType) {
                case ss::ssfb::SsEffectFunctionType_Basic: {
                    behaviorFlags |= ss::ssfb::EffectBehaviorFlags_Basic;
                    basicBehavior->priority = GETS32(effectBehaviorArrayItemVec[1]);
                    basicBehavior->maximum_particle = GETS32(effectBehaviorArrayItemVec[2]);
                    basicBehavior->attime_create = GETS32(effectBehaviorArrayItemVec[3]);
                    basicBehavior->interval = GETS32(effectBehaviorArrayItemVec[4]);
                    basicBehavior->lifetime = GETS32(effectBehaviorArrayItemVec[5]);
                    basicBehavior->speed_min_value = GETFLOAT(effectBehaviorArrayItemVec[6]);
                    basicBehavior->speed_max_value = GETFLOAT(effectBehaviorArrayItemVec[7]);
                    basicBehavior->lifespan_min_value = GETS32(effectBehaviorArrayItemVec[8]);
                    basicBehavior->lifespan_max_value = GETS32(effectBehaviorArrayItemVec[9]);
                    basicBehavior->angle = GETFLOAT(effectBehaviorArrayItemVec[10]);
                    basicBehavior->angle_variance = GETFLOAT(effectBehaviorArrayItemVec[11]);

                    auto result = std::find_if(m_effectParticleElementBasicVec.begin(), m_effectParticleElementBasicVec.end(),
                        [basicBehavior](const std::shared_ptr<ss::ssfb::EffectParticleElementBasicT> &c) {
                            return *(c.get()) == *(basicBehavior.get());
                        });
                    if (result == m_effectParticleElementBasicVec.end()) {
                        // not found
                        ssfbBasicBehavior = ss::ssfb::CreateEffectParticleElementBasic(m_ssfbBuilder, basicBehavior.get());
                        m_effectParticleElementBasicVec.push_back(basicBehavior);
                        m_ssfbEffectParticleElementBasicVec.push_back(ssfbBasicBehavior);
                    } else {
                        // found cache
                        auto idx = std::distance(m_effectParticleElementBasicVec.begin(), result);
                        ssfbBasicBehavior = m_ssfbEffectParticleElementBasicVec[idx];
                    }
                    effectNodeItem->basic_behavior = basicBehavior;
                    break;
                }
                case ss::ssfb::SsEffectFunctionType_RndSeedChange: {
                    behaviorFlags |= ss::ssfb::EffectBehaviorFlags_RndSeedChange;
                    effectNodeItem->seed = GETS32(effectBehaviorArrayItemVec[1]);
                    break;
                }
                case ss::ssfb::SsEffectFunctionType_Delay: {
                    behaviorFlags |= ss::ssfb::EffectBehaviorFlags_Delay;
                    effectNodeItem->delay_time = GETS32(effectBehaviorArrayItemVec[1]);
                    break;
                }
                case ss::ssfb::SsEffectFunctionType_Gravity: {
                    behaviorFlags |= ss::ssfb::EffectBehaviorFlags_Gravity;
                    effectNodeItem->gravity_x = GETFLOAT(effectBehaviorArrayItemVec[1]);
                    effectNodeItem->gravity_y = GETFLOAT(effectBehaviorArrayItemVec[2]);
                    break;
                }
                case ss::ssfb::SsEffectFunctionType_Position: {
                    behaviorFlags |= ss::ssfb::EffectBehaviorFlags_Position;
                    effectNodeItem->offset_x_min_value = GETFLOAT(effectBehaviorArrayItemVec[1]);
                    effectNodeItem->offset_x_max_value = GETFLOAT(effectBehaviorArrayItemVec[2]);
                    effectNodeItem->offset_y_min_value = GETFLOAT(effectBehaviorArrayItemVec[3]);
                    effectNodeItem->offset_y_max_value = GETFLOAT(effectBehaviorArrayItemVec[4]);
                    break;
                }
                case ss::ssfb::SsEffectFunctionType_Rotation: {
                    behaviorFlags |= ss::ssfb::EffectBehaviorFlags_Rotation;
                    effectNodeItem->rotation_min_value = GETFLOAT(effectBehaviorArrayItemVec[1]);
                    effectNodeItem->rotation_max_value = GETFLOAT(effectBehaviorArrayItemVec[2]);
                    effectNodeItem->rotation_add_min_value = GETFLOAT(effectBehaviorArrayItemVec[3]);
                    effectNodeItem->rotation_add_max_value = GETFLOAT(effectBehaviorArrayItemVec[4]);
                    break;
                }
                case ss::ssfb::SsEffectFunctionType_TransRotation: {
                    behaviorFlags |= ss::ssfb::EffectBehaviorFlags_TransRotation;
                    effectNodeItem->rotation_factor = GETFLOAT(effectBehaviorArrayItemVec[1]);
                    effectNodeItem->end_life_time_per = GETFLOAT(effectBehaviorArrayItemVec[2]);
                    break;
                }
                case ss::ssfb::SsEffectFunctionType_TransSpeed: {
                    behaviorFlags |= ss::ssfb::EffectBehaviorFlags_TransSpeed;
                    effectNodeItem->speed_min_value = GETFLOAT(effectBehaviorArrayItemVec[1]);
                    effectNodeItem->speed_max_value = GETFLOAT(effectBehaviorArrayItemVec[2]);
                    break;
                }
                case ss::ssfb::SsEffectFunctionType_TangentialAcceleration: {
                    behaviorFlags |= ss::ssfb::EffectBehaviorFlags_TangentialAcceleration;
                    effectNodeItem->acceleration_min_value = GETFLOAT(effectBehaviorArrayItemVec[1]);
                    effectNodeItem->acceleration_max_value = GETFLOAT(effectBehaviorArrayItemVec[2]);
                    break;
                }
                case ss::ssfb::SsEffectFunctionType_InitColor: {
                    behaviorFlags |= ss::ssfb::EffectBehaviorFlags_InitColor;
                    effectNodeItem->init_color_min_value = GETU32(effectBehaviorArrayItemVec[1]);
                    effectNodeItem->init_color_max_value = GETU32(effectBehaviorArrayItemVec[2]);
                    break;
                }
                case ss::ssfb::SsEffectFunctionType_TransColor: {
                    behaviorFlags |= ss::ssfb::EffectBehaviorFlags_TransColor;
                    effectNodeItem->trans_color_min_value = GETU32(effectBehaviorArrayItemVec[1]);
                    effectNodeItem->trans_color_max_value = GETU32(effectBehaviorArrayItemVec[2]);
                    break;
                }
                case ss::ssfb::SsEffectFunctionType_AlphaFade: {
                    behaviorFlags |= ss::ssfb::EffectBehaviorFlags_AlphaFade;
                    effectNodeItem->disprange_min_value = GETFLOAT(effectBehaviorArrayItemVec[1]);
                    effectNodeItem->disprange_max_value = GETFLOAT(effectBehaviorArrayItemVec[2]);
                    break;
                }
                case ss::ssfb::SsEffectFunctionType_Size: {
                    behaviorFlags |= ss::ssfb::EffectBehaviorFlags_Size;
                    effectNodeItem->size_x_min_value = GETFLOAT(effectBehaviorArrayItemVec[1]);
                    effectNodeItem->size_x_max_value = GETFLOAT(effectBehaviorArrayItemVec[2]);
                    effectNodeItem->size_y_min_value = GETFLOAT(effectBehaviorArrayItemVec[3]);
                    effectNodeItem->size_y_max_value = GETFLOAT(effectBehaviorArrayItemVec[4]);
                    effectNodeItem->scale_factor_min_value = GETFLOAT(effectBehaviorArrayItemVec[5]);
                    effectNodeItem->scale_factor_max_value = GETFLOAT(effectBehaviorArrayItemVec[6]);
                    break;
                }
                case ss::ssfb::SsEffectFunctionType_TransSize: {
                    behaviorFlags |= ss::ssfb::EffectBehaviorFlags_TransSize;
                    effectNodeItem->trans_size_x_min_value = GETFLOAT(effectBehaviorArrayItemVec[1]);
                    effectNodeItem->trans_size_x_max_value = GETFLOAT(effectBehaviorArrayItemVec[2]);
                    effectNodeItem->trans_size_y_min_value = GETFLOAT(effectBehaviorArrayItemVec[3]);
                    effectNodeItem->trans_size_y_max_value = GETFLOAT(effectBehaviorArrayItemVec[4]);
                    effectNodeItem->trans_scale_factor_min_value = GETFLOAT(effectBehaviorArrayItemVec[5]);
                    effectNodeItem->trans_scale_factor_max_value = GETFLOAT(effectBehaviorArrayItemVec[6]);
                    break;
                }
                case ss::ssfb::SsEffectFunctionType_PointGravity: {
                    behaviorFlags |= ss::ssfb::EffectBehaviorFlags_PointGravity;
                    effectNodeItem->point_gravity_position_x = GETFLOAT(effectBehaviorArrayItemVec[1]);
                    effectNodeItem->point_gravity_position_y = GETFLOAT(effectBehaviorArrayItemVec[2]);
                    effectNodeItem->point_gravity_power = GETFLOAT(effectBehaviorArrayItemVec[3]);
                    break;
                }
                case ss::ssfb::SsEffectFunctionType_TurnToDirectionEnabled: {
                    behaviorFlags |= ss::ssfb::EffectBehaviorFlags_TurnToDirectionEnabled;
                    effectNodeItem->rotation = GETFLOAT(effectBehaviorArrayItemVec[1]);
                    break;
                }
                case ss::ssfb::SsEffectFunctionType_InfiniteEmitEnabled: {
                    behaviorFlags |= ss::ssfb::EffectBehaviorFlags_InfiniteEmitEnabled;
                    effectNodeItem->infinitie_emit_flag = GETS32(effectBehaviorArrayItemVec[1]);
                    break;
                }
                default:
                    break;
            }
        }

        auto result = std::find_if(m_effectNodeVec.begin(), m_effectNodeVec.end(), [effectNodeItem](const std::shared_ptr<ss::ssfb::EffectNodeT> &c) {
            return *(c.get()) == *(effectNodeItem.get());
        });
        if (result ==  m_effectNodeVec.end()) {
            // not found
            ssfbEffectNodeItem = ss::ssfb::CreateEffectNode(m_ssfbBuilder,
                                                            effectNodeItem->array_index,
                                                            effectNodeItem->parent_index,
                                                            effectNodeItem->type,
                                                            effectNodeItem->cell_index,
                                                            effectNodeItem->blend_type,
                                                            ssfbBasicBehavior,
                                                            effectNodeItem->behavior_flags,
                                                            effectNodeItem->seed,
                                                            effectNodeItem->delay_time,
                                                            effectNodeItem->gravity_x,
                                                            effectNodeItem->gravity_y,
                                                            effectNodeItem->offset_x_min_value,
                                                            effectNodeItem->offset_x_max_value,
                                                            effectNodeItem->offset_y_min_value,
                                                            effectNodeItem->offset_y_max_value,
                                                            effectNodeItem->rotation_min_value,
                                                            effectNodeItem->rotation_max_value,
                                                            effectNodeItem->rotation_add_min_value,
                                                            effectNodeItem->rotation_add_max_value,
                                                            effectNodeItem->rotation_factor,
                                                            effectNodeItem->end_life_time_per,
                                                            effectNodeItem->speed_min_value,
                                                            effectNodeItem->speed_max_value,
                                                            effectNodeItem->acceleration_min_value,
                                                            effectNodeItem->acceleration_max_value,
                                                            effectNodeItem->init_color_min_value,
                                                            effectNodeItem->init_color_max_value,
                                                            effectNodeItem->trans_color_min_value,
                                                            effectNodeItem->trans_color_max_value,
                                                            effectNodeItem->disprange_min_value,
                                                            effectNodeItem->disprange_max_value,
                                                            effectNodeItem->size_x_min_value,
                                                            effectNodeItem->size_x_max_value,
                                                            effectNodeItem->size_y_min_value,
                                                            effectNodeItem->size_y_max_value,
                                                            effectNodeItem->scale_factor_min_value,
                                                            effectNodeItem->scale_factor_max_value,
                                                            effectNodeItem->trans_size_x_min_value,
                                                            effectNodeItem->trans_size_x_max_value,
                                                            effectNodeItem->trans_size_y_min_value,
                                                            effectNodeItem->trans_size_y_max_value,
                                                            effectNodeItem->trans_scale_factor_min_value,
                                                            effectNodeItem->trans_scale_factor_max_value,
                                                            effectNodeItem->point_gravity_position_x,
                                                            effectNodeItem->point_gravity_position_y,
                                                            effectNodeItem->point_gravity_power,
                                                            effectNodeItem->rotation,
                                                            effectNodeItem->infinitie_emit_flag);
            m_effectNodeVec.push_back(effectNodeItem);
            m_ssfbEffectNodeVec.push_back(ssfbEffectNodeItem);
            m_effectNodeCatalog[m_curEffectFile][effectNodeItem->array_index] = effectNodeItem;
            m_ssfbEffectNodeCatalog[m_curEffectFile][effectNodeItem->array_index] = ssfbEffectNodeItem;
        } else {
            // found
            auto idx = std::distance(m_effectNodeVec.begin(), result);
            ssfbEffectNodeItem = m_ssfbEffectNodeVec[idx];

            m_effectNodeCatalog[m_curEffectFile][effectNodeItem->array_index] = effectNodeItem;
            m_ssfbEffectNodeCatalog[m_curEffectFile][effectNodeItem->array_index] = ssfbEffectNodeItem;
        }
        return ssfbEffectNodeItem;
    }

	void createEffectFile() {
		auto rootChildVec = m_root->getChildren();
		auto effectFileLump = rootChildVec[6];
		// 6:EffectFile

		auto effectFileLumpVec = effectFileLump->getChildren();
		for(auto effectFileLumpItem : effectFileLumpVec) {
			auto effectFileLumpItemVec = effectFileLumpItem->getChildren();

            std::shared_ptr<ss::ssfb::EffectFileT> effectFile(new ss::ssfb::EffectFileT());
		    flatbuffers::Offset<ss::ssfb::EffectFile> ssfbEffectFile;

			m_curEffectFile = GETSTRING(effectFileLumpItemVec[0], m_encoding);
			effectFile->name = m_curEffectFile;
			auto ssfbEffectFileName = GETSSFBSTRING(m_ssfbBuilder, effectFileLumpItemVec[0], m_encoding);
			effectFile->fps = GETS16(effectFileLumpItemVec[1]);
			effectFile->is_lock_rand_seed = GETS16(effectFileLumpItemVec[2]);
			effectFile->lock_rand_seed = GETS16(effectFileLumpItemVec[3]);
			effectFile->layout_scale_x = GETS16(effectFileLumpItemVec[4]);
			effectFile->layout_scale_y = GETS16(effectFileLumpItemVec[5]);
            // auto numNodeList = GETS16(effectFileLumpItemVec[6]); unused

			auto EffectNodeArray = effectFileLumpItemVec[7];
			auto EffectNodeArrayVec = EffectNodeArray->getChildren();
			std::vector<flatbuffers::Offset<ss::ssfb::EffectNode>> ssfbEffectNode;
			for(auto EffectNodeArrayItem : EffectNodeArrayVec) {
			    std::shared_ptr<ss::ssfb::EffectNodeT> effectNodeItem(new ss::ssfb::EffectNodeT());
                auto ssfbEffectNodeItem = createSharedEffectNode(EffectNodeArrayItem, effectNodeItem);

                ssfbEffectNode.push_back(ssfbEffectNodeItem);
                effectFile->effect_node.push_back(effectNodeItem);
            }
			auto serializeSsfbEffectNode = m_ssfbBuilder.CreateVector(ssfbEffectNode);
            ssfbEffectFile = ss::ssfb::CreateEffectFile(m_ssfbBuilder, ssfbEffectFileName,
                                                        effectFile->fps, effectFile->is_lock_rand_seed, effectFile->lock_rand_seed,
                                                        effectFile->layout_scale_x, effectFile->layout_scale_y,
                                                        serializeSsfbEffectNode);
			m_ssfbEffectFileList.push_back(ssfbEffectFile);
			m_ssfbEffectFileCatalog[effectFile->name] = ssfbEffectFile;

			m_effectFileVec.push_back(effectFile);
			m_ssfbEffectFileVec.push_back(ssfbEffectFile);
		}
	}

	void createProjectData()
	{
		auto serializeSsfbCells = m_ssfbBuilder.CreateVector(m_ssfbCells);
		auto serializeSsfbAnimePackData = m_ssfbBuilder.CreateVector(m_ssfbAnimePacks);
		auto serializeSsfbEffectFileList = m_ssfbBuilder.CreateVector(m_ssfbEffectFileList);
		m_ssfbProjectData = ss::ssfb::CreateProjectData(m_ssfbBuilder, m_dataId, m_version,
													  m_ssfbImageBaseDir,
													  serializeSsfbCells,
													  serializeSsfbAnimePackData,
													  serializeSsfbEffectFileList,
													  static_cast<int16_t>(m_ssfbCells.size()),
													  static_cast<int16_t>(m_ssfbAnimePacks.size()),
													  static_cast<int16_t>(m_ssfbEffectFileList.size()));

		m_ssfbBuilder.Finish(m_ssfbProjectData);
	}

	void writeFile(std::ostream& out) {
		out.write(reinterpret_cast<const char *>(m_ssfbBuilder.GetBufferPointer()), m_ssfbBuilder.GetSize());
		/*
		// ssfb json
		flatbuffers::Parser parser;
		bool ret = parser.Parse(reinterpret_cast<const char *>(fbs_ssfb_fbs));
		parser.opts.generate_name_strings = true;
		parser.opts.output_enum_identifiers = true;
		parser.opts.output_default_scalars_in_json = true;
		parser.opts.output_enum_identifiers = true;
		parser.opts.strict_json = true;
		std::string jsongen;
		flatbuffers::GenerateText(parser, m_ssfbBuilder.GetBufferPointer(), &jsongen);
		 */
	}
};
#undef GETS16
#undef GETS32
#undef GETU32
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

