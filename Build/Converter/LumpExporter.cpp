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

	flatbuffers::FlatBufferBuilder m_ssfbBuilder;
	flatbuffers::Offset<ss::ssfb::ProjectData> m_ssfbProjectData;
	int32_t m_dataId{};
	int32_t m_version{};
	int32_t m_flags{};
	flatbuffers::Offset<flatbuffers::String> m_ssfbImageBaseDir;
	std::vector<flatbuffers::Offset<ss::ssfb::Cell>> m_ssfbCells;
	std::vector<flatbuffers::Offset<ss::ssfb::AnimePackData>> m_ssfbAnimePacks;
	std::vector<flatbuffers::Offset<ss::ssfb::EffectFile>> m_ssfbEffectFileList;

	struct CellMapPrimitive {
        std::string name;
        std::string imagePath;
        int16_t mapIndex{};
        int16_t wrapMode{};
        int16_t filterMode{};
	};
	std::vector<std::shared_ptr<struct CellMapPrimitive>> m_cellMaps;
	std::vector<flatbuffers::Offset<ss::ssfb::CellMap>> m_ssfbCellMaps;

	struct AnimationInitialDataPrimitive {
		int16_t index;
		int32_t lowflag;
		int32_t highflag;
		int16_t priority;
		int16_t cellIndex;
		int16_t opacity;
		int16_t localopacity;
		int16_t masklimen;
		float posX;
		float posY;
		float posZ;
		float pivotX;
		float pivotY;
		float rotationX;
		float rotationY;
		float rotationZ;
		float scaleX;
		float scaleY;
		float localscaleX;
		float localscaleY;
		float size_X;
		float size_Y;
		float uv_move_X;
		float uv_move_Y;
		float uv_rotation;
		float uv_scale_X;
		float uv_scale_Y;
		float boundingRadius;
		int32_t instanceValue_curKeyframe;
		int32_t instanceValue_startFrame;
		int32_t instanceValue_endFrame;
		int32_t instanceValue_loopNum;
		float instanceValue_speed;
		int32_t instanceValue_loopflag;
		int32_t effectValue_curKeyframe;
		int32_t effectValue_startTime;
		float effectValue_speed;
		int32_t effectValue_loopflag;
	};
	std::vector<std::shared_ptr<struct AnimationInitialDataPrimitive>> m_animationInitialDataVec;
	std::vector<flatbuffers::Offset<ss::ssfb::AnimationInitialData>> m_ssfbAnimationInitialDataVec;

	struct PartDataPrimitive {
		std::string name;
		int16_t index;
		int16_t parentIndex;
		int16_t type;
		int16_t boundsType;
		int16_t alphaBlendType;
		std::string refname;
		std::string effectfilename;
		std::string colorLabel;
		int16_t maskInfluence;
	};
	std::vector<std::shared_ptr<struct PartDataPrimitive>> m_partDataVec;
	std::vector<flatbuffers::Offset<ss::ssfb::PartData>> m_ssfbPartDataVec;

	std::vector<std::vector<uint32_t>> m_uint32VecVec;
	std::vector<flatbuffers::Offset<flatbuffers::Vector<uint32_t>>> m_ssfbUint32VecVec;

	std::vector<std::vector<float>> m_floatVecVec;
	std::vector<flatbuffers::Offset<flatbuffers::Vector<float>>> m_ssfbFloatVecVec;

	struct meshDataUVPrimitive {
        std::vector<float> uv;
	};
	std::vector<std::shared_ptr<struct meshDataUVPrimitive>> m_meshDataUVVec;
	std::vector<flatbuffers::Offset<ss::ssfb::meshDataUV>> m_ssfbMeshDataUVVec;

	struct meshDataIndicesPrimitive {
        std::vector<float> indices;
	};
	std::vector<std::shared_ptr<struct meshDataIndicesPrimitive>> m_meshDataIndicesVec;
	std::vector<flatbuffers::Offset<ss::ssfb::meshDataIndices>> m_ssfbMeshDataIndicesVec;

	struct FrameDataIndexPrimitive {
		std::vector<uint32_t> data;
	};
	std::vector<std::shared_ptr<struct FrameDataIndexPrimitive>> m_frameDataIndexVec;
	std::vector<flatbuffers::Offset<ss::ssfb::frameDataIndex>> m_ssfbFrameDataIndexVec;

	enum {
		USER_DATA_FLAG_INTEGER	= 1 << 0,
		USER_DATA_FLAG_RECT	= 1 << 1,
		USER_DATA_FLAG_POINT	= 1 << 2,
		USER_DATA_FLAG_STRING	= 1 << 3
	};

	void createHeader()
	{
		auto rootChildVec = m_root->getChildren();
		m_dataId = GETS32(rootChildVec[0]);
		m_version = GETS32(rootChildVec[1]);
		m_flags = GETS32(rootChildVec[2]);
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

		std::shared_ptr<struct CellMapPrimitive> cellMapPrimitive(new struct CellMapPrimitive);
		cellMapPrimitive->name = GETSTRING(cellMapVec[0], m_encoding);
		cellMapPrimitive->imagePath = GETSTRING(cellMapVec[1], m_encoding);
		cellMapPrimitive->mapIndex = GETS16(cellMapVec[2]);
		cellMapPrimitive->wrapMode = GETS16(cellMapVec[3]);
		cellMapPrimitive->filterMode = GETS16(cellMapVec[4]);
		// 5:reserved(s16)

		// search same cellMap from cellMap caches.
		auto result = std::find_if(m_cellMaps.begin(), m_cellMaps.end(), [&cellMapPrimitive](const std::shared_ptr<struct CellMapPrimitive> &item) {
			if(cellMapPrimitive->name != item->name)
				return false;
			if(cellMapPrimitive->imagePath != item->imagePath)
				return false;
			if(cellMapPrimitive->mapIndex != item->mapIndex)
				return false;
			if(cellMapPrimitive->wrapMode != item->wrapMode)
				return false;
			if(cellMapPrimitive->filterMode != item->filterMode)
				return false;

			return true;
		});
		if (result == m_cellMaps.end()) {
			// not found

			// create ssfb cellMap
			auto ssfbCellMapName = m_ssfbBuilder.CreateSharedString(cellMapPrimitive->name);
			auto ssfbCellMapImagePath = m_ssfbBuilder.CreateSharedString(cellMapPrimitive->imagePath);
			cellMap = ss::ssfb::CreateCellMap(m_ssfbBuilder, ssfbCellMapName, ssfbCellMapImagePath,
											  cellMapPrimitive->mapIndex, cellMapPrimitive->wrapMode, cellMapPrimitive->filterMode);
			// cache ssfb cellMap
			m_cellMaps.push_back(cellMapPrimitive);
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

		std::shared_ptr<struct AnimationInitialDataPrimitive> animationInitialDataPrimitive(new struct AnimationInitialDataPrimitive);

		animationInitialDataPrimitive->index = GETS16(AnimationInitialDataItemVec[0]);
		// 1:reserve
		animationInitialDataPrimitive->lowflag = GETS32(AnimationInitialDataItemVec[2]);
		animationInitialDataPrimitive->highflag = GETS32(AnimationInitialDataItemVec[3]);
		animationInitialDataPrimitive->priority = GETS16(AnimationInitialDataItemVec[4]);
		animationInitialDataPrimitive->cellIndex = GETS16(AnimationInitialDataItemVec[5]);
		animationInitialDataPrimitive->opacity = GETS16(AnimationInitialDataItemVec[6]);
		animationInitialDataPrimitive->localopacity = GETS16(AnimationInitialDataItemVec[7]);
		animationInitialDataPrimitive->masklimen = GETS16(AnimationInitialDataItemVec[8]);
		// 9:reserved
		animationInitialDataPrimitive->posX = GETFLOAT(AnimationInitialDataItemVec[10]);
		animationInitialDataPrimitive->posY = GETFLOAT(AnimationInitialDataItemVec[11]);
		animationInitialDataPrimitive->posZ = GETFLOAT(AnimationInitialDataItemVec[12]);
		animationInitialDataPrimitive->pivotX = GETFLOAT(AnimationInitialDataItemVec[13]);
		animationInitialDataPrimitive->pivotY = GETFLOAT(AnimationInitialDataItemVec[14]);
		animationInitialDataPrimitive->rotationX = GETFLOAT(AnimationInitialDataItemVec[15]);
		animationInitialDataPrimitive->rotationY = GETFLOAT(AnimationInitialDataItemVec[16]);
		animationInitialDataPrimitive->rotationZ = GETFLOAT(AnimationInitialDataItemVec[17]);
		animationInitialDataPrimitive->scaleX = GETFLOAT(AnimationInitialDataItemVec[18]);
		animationInitialDataPrimitive->scaleY = GETFLOAT(AnimationInitialDataItemVec[19]);
		animationInitialDataPrimitive->localscaleX = GETFLOAT(AnimationInitialDataItemVec[20]);
		animationInitialDataPrimitive->localscaleY = GETFLOAT(AnimationInitialDataItemVec[21]);
		animationInitialDataPrimitive->size_X = GETFLOAT(AnimationInitialDataItemVec[22]);
		animationInitialDataPrimitive->size_Y = GETFLOAT(AnimationInitialDataItemVec[23]);
		animationInitialDataPrimitive->uv_move_X = GETFLOAT(AnimationInitialDataItemVec[24]);
		animationInitialDataPrimitive->uv_move_Y = GETFLOAT(AnimationInitialDataItemVec[25]);
		animationInitialDataPrimitive->uv_rotation = GETFLOAT(AnimationInitialDataItemVec[26]);
		animationInitialDataPrimitive->uv_scale_X = GETFLOAT(AnimationInitialDataItemVec[27]);
		animationInitialDataPrimitive->uv_scale_Y = GETFLOAT(AnimationInitialDataItemVec[28]);
		animationInitialDataPrimitive->boundingRadius = GETFLOAT(AnimationInitialDataItemVec[29]);
		//インスタンス関連
		animationInitialDataPrimitive->instanceValue_curKeyframe = GETS32(AnimationInitialDataItemVec[30]);
		animationInitialDataPrimitive->instanceValue_startFrame = GETS32(AnimationInitialDataItemVec[31]);
		animationInitialDataPrimitive->instanceValue_endFrame = GETS32(AnimationInitialDataItemVec[32]);
		animationInitialDataPrimitive->instanceValue_loopNum = GETS32(AnimationInitialDataItemVec[33]);
		animationInitialDataPrimitive->instanceValue_speed = GETFLOAT(AnimationInitialDataItemVec[34]);
		animationInitialDataPrimitive->instanceValue_loopflag = GETS32(AnimationInitialDataItemVec[35]);
		//エフェクト関連
		animationInitialDataPrimitive->effectValue_curKeyframe = GETS32(AnimationInitialDataItemVec[36]);
		animationInitialDataPrimitive->effectValue_startTime = GETS32(AnimationInitialDataItemVec[37]);
		animationInitialDataPrimitive->effectValue_speed = GETFLOAT(AnimationInitialDataItemVec[38]);
		animationInitialDataPrimitive->effectValue_loopflag = GETS32(AnimationInitialDataItemVec[39]);

		// search same cellMap from cellMap caches.
		auto result = std::find_if(m_animationInitialDataVec.begin(), m_animationInitialDataVec.end(), [&animationInitialDataPrimitive](const std::shared_ptr<struct AnimationInitialDataPrimitive> &item) {
			if(animationInitialDataPrimitive->index != item->index)
				return false;
			if(animationInitialDataPrimitive->lowflag != item->lowflag)
				return false;
			if(animationInitialDataPrimitive->highflag != item->highflag)
				return false;
			if(animationInitialDataPrimitive->priority != item->priority)
				return false;
			if(animationInitialDataPrimitive->cellIndex != item->cellIndex)
				return false;
			if(animationInitialDataPrimitive->opacity != item->opacity)
				return false;
			if(animationInitialDataPrimitive->localopacity != item->localopacity)
				return false;
			if(animationInitialDataPrimitive->masklimen != item->masklimen)
				return false;
			if(animationInitialDataPrimitive->posX != item->posX)
				return false;
			if(animationInitialDataPrimitive->posY != item->posY)
				return false;
			if(animationInitialDataPrimitive->posZ != item->posZ)
				return false;
			if(animationInitialDataPrimitive->pivotX != item->pivotX)
				return false;
			if(animationInitialDataPrimitive->pivotY != item->pivotY)
				return false;
			if(animationInitialDataPrimitive->rotationX != item->rotationX)
				return false;
			if(animationInitialDataPrimitive->rotationY != item->rotationY)
				return false;
			if(animationInitialDataPrimitive->rotationZ != item->rotationZ)
				return false;
			if(animationInitialDataPrimitive->scaleX != item->scaleX)
				return false;
			if(animationInitialDataPrimitive->scaleY != item->scaleY)
				return false;
			if(animationInitialDataPrimitive->localscaleX != item->localscaleX)
				return false;
			if(animationInitialDataPrimitive->localscaleY != item->localscaleY)
				return false;
			if(animationInitialDataPrimitive->size_X != item->size_X)
				return false;
			if(animationInitialDataPrimitive->size_Y != item->size_Y)
				return false;
			if(animationInitialDataPrimitive->uv_move_X != item->uv_move_X)
				return false;
			if(animationInitialDataPrimitive->uv_move_Y != item->uv_move_Y)
				return false;
			if(animationInitialDataPrimitive->uv_rotation != item->uv_rotation)
				return false;
			if(animationInitialDataPrimitive->uv_scale_X != item->uv_scale_X)
				return false;
			if(animationInitialDataPrimitive->uv_scale_Y != item->uv_scale_Y)
				return false;
			if(animationInitialDataPrimitive->boundingRadius != item->boundingRadius)
				return false;
			if(animationInitialDataPrimitive->instanceValue_curKeyframe != item->instanceValue_curKeyframe)
				return false;
			if(animationInitialDataPrimitive->instanceValue_startFrame != item->instanceValue_startFrame)
				return false;
			if(animationInitialDataPrimitive->instanceValue_endFrame != item->instanceValue_endFrame)
				return false;
			if(animationInitialDataPrimitive->instanceValue_loopNum != item->instanceValue_loopNum)
				return false;
			if(animationInitialDataPrimitive->instanceValue_speed != item->instanceValue_speed)
				return false;
			if(animationInitialDataPrimitive->instanceValue_loopflag != item->instanceValue_loopflag)
				return false;
			if(animationInitialDataPrimitive->effectValue_curKeyframe != item->effectValue_curKeyframe)
				return false;
			if(animationInitialDataPrimitive->effectValue_startTime != item->effectValue_startTime)
				return false;
			if(animationInitialDataPrimitive->effectValue_speed != item->effectValue_speed)
				return false;
			if(animationInitialDataPrimitive->effectValue_loopflag != item->effectValue_loopflag)
				return false;
			return true;
		});
		if (result == m_animationInitialDataVec.end()) {
			// not found

			// create ssfb partData
			//animationInitialData = m_ssfbBuilder.Create
			animationInitialData = ss::ssfb::CreateAnimationInitialData(m_ssfbBuilder,
																		animationInitialDataPrimitive->index,
																		animationInitialDataPrimitive->lowflag,
																		animationInitialDataPrimitive->highflag,
																		animationInitialDataPrimitive->priority,
																		animationInitialDataPrimitive->cellIndex,
																		animationInitialDataPrimitive->opacity,
																		animationInitialDataPrimitive->localopacity,
																		animationInitialDataPrimitive->masklimen,
																		animationInitialDataPrimitive->posX,
																		animationInitialDataPrimitive->posY,
																		animationInitialDataPrimitive->posZ,
																		animationInitialDataPrimitive->pivotX,
																		animationInitialDataPrimitive->pivotY,
																		animationInitialDataPrimitive->rotationX,
																		animationInitialDataPrimitive->rotationY,
																		animationInitialDataPrimitive->rotationZ,
																		animationInitialDataPrimitive->scaleX,
																		animationInitialDataPrimitive->scaleY,
																		animationInitialDataPrimitive->localscaleX,
																		animationInitialDataPrimitive->localscaleY,
																		animationInitialDataPrimitive->size_X,
																		animationInitialDataPrimitive->size_Y,
																		animationInitialDataPrimitive->uv_move_X,
																		animationInitialDataPrimitive->uv_move_Y,
																		animationInitialDataPrimitive->uv_rotation,
																		animationInitialDataPrimitive->uv_scale_X,
																		animationInitialDataPrimitive->uv_scale_Y,
																		animationInitialDataPrimitive->boundingRadius,
																		animationInitialDataPrimitive->instanceValue_curKeyframe,
																		animationInitialDataPrimitive->instanceValue_startFrame,
																		animationInitialDataPrimitive->instanceValue_endFrame,
																		animationInitialDataPrimitive->instanceValue_loopNum,
																		animationInitialDataPrimitive->instanceValue_speed,
																		animationInitialDataPrimitive->instanceValue_loopflag,
																		animationInitialDataPrimitive->effectValue_curKeyframe,
																		animationInitialDataPrimitive->effectValue_startTime,
																		animationInitialDataPrimitive->effectValue_speed,
																		animationInitialDataPrimitive->effectValue_loopflag);
			// cache ssfb cellMap
			m_animationInitialDataVec.push_back(animationInitialDataPrimitive);
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

		std::shared_ptr<struct PartDataPrimitive> partDataPrimitive(new struct PartDataPrimitive);

		partDataPrimitive->name = GETSTRING(partDataItemVec[0], m_encoding);
		partDataPrimitive->index = GETS16(partDataItemVec[1]);
		partDataPrimitive->parentIndex = GETS16(partDataItemVec[2]);
		partDataPrimitive->type = (ss::ssfb::SsPartType)GETS16(partDataItemVec[3]);
		partDataPrimitive->boundsType = GETS16(partDataItemVec[4]);
		partDataPrimitive->alphaBlendType = GETS16(partDataItemVec[5]);
		partDataPrimitive->refname = GETSTRING(partDataItemVec[7], m_encoding);
		partDataPrimitive->effectfilename = GETSTRING(partDataItemVec[8], m_encoding);
		partDataPrimitive->colorLabel = GETSTRING(partDataItemVec[9], m_encoding);
		partDataPrimitive->maskInfluence = GETS16(partDataItemVec[10]);

		// search same cellMap from cellMap caches.
		auto result = std::find_if(m_partDataVec.begin(), m_partDataVec.end(), [&partDataPrimitive](const std::shared_ptr<struct PartDataPrimitive> &item) {
			if (partDataPrimitive->name != item->name)
				return false;
			if (partDataPrimitive->index != item->index)
				return false;
			if (partDataPrimitive->parentIndex != item->parentIndex)
				return false;
			if (partDataPrimitive->type != item->type)
				return false;
			if (partDataPrimitive->boundsType != item->boundsType)
				return false;
			if (partDataPrimitive->alphaBlendType != item->alphaBlendType)
				return false;
			if (partDataPrimitive->refname != item->refname)
				return false;
			if (partDataPrimitive->effectfilename != item->effectfilename)
				return false;
			if (partDataPrimitive->colorLabel != item->colorLabel)
				return false;
			if (partDataPrimitive->maskInfluence != item->maskInfluence)
				return false;
			return true;
		});
		if (result == m_partDataVec.end()) {
			// not found

			// create ssfb partData
			auto ssfbPartDataName =  m_ssfbBuilder.CreateSharedString(partDataPrimitive->name);
			auto ssfbRefname = m_ssfbBuilder.CreateSharedString(partDataPrimitive->refname);
			auto ssfbEffectfilename = m_ssfbBuilder.CreateSharedString(partDataPrimitive->effectfilename);
			auto ssfbColorLabel = m_ssfbBuilder.CreateSharedString(partDataPrimitive->colorLabel);

			partData = ss::ssfb::CreatePartData(m_ssfbBuilder, ssfbPartDataName,partDataPrimitive->index, partDataPrimitive->parentIndex, (ss::ssfb::SsPartType)partDataPrimitive->type,
												partDataPrimitive->boundsType, partDataPrimitive->alphaBlendType, ssfbRefname, ssfbEffectfilename, ssfbColorLabel,
												partDataPrimitive->maskInfluence);
			// cache ssfb cellMap
			m_partDataVec.push_back(partDataPrimitive);
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

		auto result = std::find_if(m_uint32VecVec.begin(), m_uint32VecVec.end(), [&vec](const std::vector<uint32_t> &item) {
			return vec == item;
		});
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

		auto result = std::find_if(m_floatVecVec.begin(), m_floatVecVec.end(), [&vec](const std::vector<float> &item) {
			return vec == item;
		});
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

	flatbuffers::Offset<ss::ssfb::meshDataUV> createSharedMeshDataUV(const std::vector<float> &uvPrimitive, const flatbuffers::Offset<flatbuffers::Vector<float>> &uv) {
		flatbuffers::Offset<ss::ssfb::meshDataUV> meshDataUV;

		std::shared_ptr<struct meshDataUVPrimitive> meshDataUVPrimitive(new struct meshDataUVPrimitive);
		meshDataUVPrimitive->uv = uvPrimitive;
		auto result = std::find_if(m_meshDataUVVec.begin(), m_meshDataUVVec.end(), [&meshDataUVPrimitive](const std::shared_ptr<struct meshDataUVPrimitive> &item) {
			return meshDataUVPrimitive->uv == item->uv;
		});
		if (result == m_meshDataUVVec.end()) {
			// not found

			// create ssfb vec
			meshDataUV = ss::ssfb::CreatemeshDataUV(m_ssfbBuilder, uv);

			// cache ssfb vec
			m_meshDataUVVec.push_back(meshDataUVPrimitive);
			m_ssfbMeshDataUVVec.push_back(meshDataUV);
		} else {
			auto idx = std::distance(m_meshDataUVVec.begin(), result);
			meshDataUV = m_ssfbMeshDataUVVec[idx];
		}

		return meshDataUV;
	}

	flatbuffers::Offset<ss::ssfb::meshDataIndices> createSharedMeshDataIndices(const std::vector<float> &indicesPrimitive, const flatbuffers::Offset<flatbuffers::Vector<float>> &indices) {
		flatbuffers::Offset<ss::ssfb::meshDataIndices> meshDataIndices;

		std::shared_ptr<struct meshDataIndicesPrimitive> meshDataIndicesPrimitive(new struct meshDataIndicesPrimitive);
		meshDataIndicesPrimitive->indices = indicesPrimitive;
		auto result = std::find_if(m_meshDataIndicesVec.begin(), m_meshDataIndicesVec.end(), [&meshDataIndicesPrimitive](const std::shared_ptr<struct meshDataIndicesPrimitive> &item) {
			return meshDataIndicesPrimitive->indices == item->indices;
		});
		if (result == m_meshDataIndicesVec.end()) {
			// not found

			// create ssfb vec
			meshDataIndices = ss::ssfb::CreatemeshDataIndices(m_ssfbBuilder, indices);

			// cache ssfb vec
			m_meshDataIndicesVec.push_back(meshDataIndicesPrimitive);
			m_ssfbMeshDataIndicesVec.push_back(meshDataIndices);
		} else {
			auto idx = std::distance(m_meshDataIndicesVec.begin(), result);
			meshDataIndices = m_ssfbMeshDataIndicesVec[idx];
		}

		return meshDataIndices;
	}

	flatbuffers::Offset<ss::ssfb::frameDataIndex> createSharedFrameDataIndex(const std::vector<uint32_t> &dataPrimitive, const flatbuffers::Offset<flatbuffers::Vector<uint32_t>> &data) {
		flatbuffers::Offset<ss::ssfb::frameDataIndex> frameDataIndex;

		std::shared_ptr<struct FrameDataIndexPrimitive> frameDataIndexPrimitive(new struct FrameDataIndexPrimitive);
		frameDataIndexPrimitive->data = dataPrimitive;
		auto result = std::find_if(m_frameDataIndexVec.begin(), m_frameDataIndexVec.end(), [&frameDataIndexPrimitive](const std::shared_ptr<struct FrameDataIndexPrimitive> &item) {
			return frameDataIndexPrimitive->data == item->data;
		});
		if (result == m_frameDataIndexVec.end()) {
			// not found

			// create ssfb vec
			frameDataIndex = ss::ssfb::CreateframeDataIndex(m_ssfbBuilder, data);

			// cache ssfb vec
			m_frameDataIndexVec.push_back(frameDataIndexPrimitive);
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
			std::vector<flatbuffers::Offset<ss::ssfb::meshDataUV>> ssfbMeshsDataUV;
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
			std::vector<flatbuffers::Offset<ss::ssfb::meshDataIndices>> ssfbMeshsDataIndices;
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

			std::vector<flatbuffers::Offset<ss::ssfb::frameDataIndex>> ssfbFrameData;
			{
				auto frameDataIndexArrayVec = ssAnimationDataVec[2]->getChildren();

				for(auto frameDataIndexArrayItem : frameDataIndexArrayVec) {
					auto frameDataVec = frameDataIndexArrayItem->getChildren();

					std::vector<uint32_t > ssfbFrameData2;
					for(auto frameDataItem : frameDataVec) {
						switch (frameDataItem->type) {
							case Lump::DataType::S16:
								ssfbFrameData2.push_back(GETS16(frameDataItem));
								break;
							case Lump::DataType::S32:
								ssfbFrameData2.push_back(GETS32(frameDataItem));
								break;
							case Lump::DataType::FLOAT:
								ssfbFrameData2.push_back(GETS32(frameDataItem));
								break;
							case Lump::DataType::COLOR:
								{
									auto rgba = GETU32(frameDataItem);
									ssfbFrameData2.push_back((rgba & 0xffff0000) >> 16);
									ssfbFrameData2.push_back(rgba & 0xffff);
								}
								break;
							default:
								break;
						}
					}
					auto serializeSsfbFrameData2 = createSharedUint32Vec(ssfbFrameData2);
					auto item = createSharedFrameDataIndex(ssfbFrameData2, serializeSsfbFrameData2);
					ssfbFrameData.push_back(item);
				}
			}
			// 3:userDataIndexArray
			std::vector<flatbuffers::Offset<ss::ssfb::userDataPerFrame>> ssfbUserData;
			{
				if(ssAnimationDataVec[3]->type == Lump::DataType::SET) {

					auto userDataIndexArrayVec = ssAnimationDataVec[3]->getChildren();
					for(auto userDataIndexArrayItem : userDataIndexArrayVec) {
						if(userDataIndexArrayItem->type != Lump::DataType::SET) {
							continue;
						}
						std::vector<flatbuffers::Offset<ss::ssfb::userDataItem>> ssfbUserDataItemData;

						std::vector<flatbuffers::Offset<void>> ssfbDataArray;
						std::vector<uint8_t> ssfbDataArrayType;

						auto userDataIndexArrayItemVec = userDataIndexArrayItem->getChildren();
						auto num = GETS16(userDataIndexArrayItemVec[0]);
						int idx = 1;
						for(int i=0; i<num; i++) {
							auto flags = GETS16(userDataIndexArrayItemVec[idx++]);
							auto arrayIndex = GETS16(userDataIndexArrayItemVec[idx++]);
							if(flags & (int16_t)(USER_DATA_FLAG_INTEGER)) {
								auto integer = GETS32(userDataIndexArrayItemVec[idx++]);
								auto item = m_ssfbBuilder.CreateStruct(ss::ssfb::userDataInteger(integer));
								ssfbDataArray.push_back(item.Union());
								ssfbDataArrayType.push_back(ss::ssfb::userDataValue_userDataInteger);
							}
							if(flags & (int16_t)USER_DATA_FLAG_RECT) {
								auto rect_x = GETS32(userDataIndexArrayItemVec[idx++]);
								auto rect_y = GETS32(userDataIndexArrayItemVec[idx++]);
								auto rect_w = GETS32(userDataIndexArrayItemVec[idx++]);
								auto rect_h = GETS32(userDataIndexArrayItemVec[idx++]);

								auto item = m_ssfbBuilder.CreateStruct(ss::ssfb::userDataRect(rect_x, rect_y, rect_w, rect_h));
								ssfbDataArray.push_back(item.Union());
								ssfbDataArrayType.push_back(ss::ssfb::userDataValue_userDataRect);
							}
							if(flags & (int16_t)(USER_DATA_FLAG_POINT)) {
								auto point_x = GETS32(userDataIndexArrayItemVec[idx++]);
								auto point_y = GETS32(userDataIndexArrayItemVec[idx++]);

								auto item = m_ssfbBuilder.CreateStruct(ss::ssfb::userDataPoint(point_x, point_y));
								ssfbDataArray.push_back(item.Union());
								ssfbDataArrayType.push_back(ss::ssfb::userDataValue_userDataPoint);
							}
							if(flags & (int16_t)(USER_DATA_FLAG_STRING)) {
								auto str_length = GETS16(userDataIndexArrayItemVec[idx++]);
								auto ssfbStr = GETSSFBSTRING(m_ssfbBuilder, userDataIndexArrayItemVec[idx++], m_encoding);

								auto item = ss::ssfb::CreateuserDataString(m_ssfbBuilder, str_length, ssfbStr);
								ssfbDataArray.push_back(item.Union());
								ssfbDataArrayType.push_back(ss::ssfb::userDataValue_userDataString);
							}

							auto serializeSsfbDataArrayType = m_ssfbBuilder.CreateVector(ssfbDataArrayType);
							auto serializeSsfbDataArray = m_ssfbBuilder.CreateVector(ssfbDataArray);
							auto item = ss::ssfb::CreateuserDataItem(m_ssfbBuilder, static_cast<int16_t>(flags),
																	 arrayIndex,
																	 serializeSsfbDataArrayType,
																	 serializeSsfbDataArray);
							ssfbUserDataItemData.push_back(item);
						}

						auto serializeSsfbUserDataItemData = m_ssfbBuilder.CreateVector(ssfbUserDataItemData);

						auto frame = this->m_frameIndexVec[this->m_frameIndex++];
						auto item = ss::ssfb::CreateuserDataPerFrame(m_ssfbBuilder, frame, serializeSsfbUserDataItemData);
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
						auto ssfbLabelDataItemName = GETSSFBSTRING(m_ssfbBuilder, labelDataVec[0], m_encoding);
						auto time = GETS16(labelDataVec[1]);

						auto item = ss::ssfb::CreatelabelDataItem(m_ssfbBuilder, ssfbLabelDataItemName, time);
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

	void createEffectFile() {
		auto rootChildVec = m_root->getChildren();
		auto effectFileLump = rootChildVec[6];
		// 6:EffectFile

		auto effectFileLumpVec = effectFileLump->getChildren();
		for(auto effectFileLumpItem : effectFileLumpVec) {
			auto effectFileLumpItemVec = effectFileLumpItem->getChildren();
			auto ssfbEffectFileName = GETSSFBSTRING(m_ssfbBuilder, effectFileLumpItemVec[0], m_encoding);
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
							auto item = m_ssfbBuilder.CreateStruct(
									ss::ssfb::EffectParticleElementBasic(SsEffectFunctionType, priority,
									                                     maximumParticle, attimeCreate, interval, lifetime,
									                                     speedMinValue, speedMaxValue, lifespanMinValue, lifespanMaxValue, angle, angleVariance));
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementRndSeedChange: {
							auto Seed = GETS32(effectBehaviorArrayItemVec[1]);

							auto item = m_ssfbBuilder.CreateStruct(ss::ssfb::EffectParticleElementRndSeedChange(Seed));
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementDelay: {
							auto DelayTime = GETS32(effectBehaviorArrayItemVec[1]);

							auto item = m_ssfbBuilder.CreateStruct(ss::ssfb::EffectParticleElementDelay(DelayTime));
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementGravity: {
							auto Gravity_x = GETFLOAT(effectBehaviorArrayItemVec[1]);
							auto Gravity_y = GETFLOAT(effectBehaviorArrayItemVec[2]);

							auto item = m_ssfbBuilder.CreateStruct(ss::ssfb::EffectParticleElementGravity(Gravity_x, Gravity_y));
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementPosition: {
							auto OffsetXMinValue = GETFLOAT(effectBehaviorArrayItemVec[1]);
							auto OffsetXMaxValue = GETFLOAT(effectBehaviorArrayItemVec[2]);
							auto OffsetYMinValue = GETFLOAT(effectBehaviorArrayItemVec[3]);
							auto OffsetYMaxValue = GETFLOAT(effectBehaviorArrayItemVec[4]);

							auto item = m_ssfbBuilder.CreateStruct(
									ss::ssfb::EffectParticleElementPosition(OffsetXMinValue,
									                                        OffsetXMaxValue,
									                                        OffsetYMinValue,
									                                        OffsetYMaxValue));
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementRotation: {
							auto RotationMinValue = GETFLOAT(effectBehaviorArrayItemVec[1]);
							auto RotationMaxValue = GETFLOAT(effectBehaviorArrayItemVec[2]);
							auto RotationAddMinValue = GETFLOAT(effectBehaviorArrayItemVec[3]);
							auto RotationAddMaxValue = GETFLOAT(effectBehaviorArrayItemVec[4]);

							auto item = m_ssfbBuilder.CreateStruct(
									ss::ssfb::EffectParticleElementRotation(RotationMinValue,
									                                        RotationMaxValue,
									                                        RotationAddMinValue,
									                                        RotationAddMaxValue));
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementRotationTrans: {
							auto RotationFactor = GETFLOAT(effectBehaviorArrayItemVec[1]);
							auto EndLifeTimePer = GETFLOAT(effectBehaviorArrayItemVec[2]);

							auto item = m_ssfbBuilder.CreateStruct(
									ss::ssfb::EffectParticleElementRotationTrans(RotationFactor, EndLifeTimePer));
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementTransSpeed: {
							auto SpeedMinValue = GETFLOAT(effectBehaviorArrayItemVec[1]);
							auto SpeedMaxValue = GETFLOAT(effectBehaviorArrayItemVec[2]);

							auto item = m_ssfbBuilder.CreateStruct(
									ss::ssfb::EffectParticleElementTransSpeed(SpeedMinValue, SpeedMaxValue));
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementTangentialAcceleration: {
							auto AccelerationMinValue = GETFLOAT(effectBehaviorArrayItemVec[1]);
							auto AccelerationMaxValue = GETFLOAT(effectBehaviorArrayItemVec[2]);

							auto item = m_ssfbBuilder.CreateStruct(
									ss::ssfb::EffectParticleElementTangentialAcceleration(AccelerationMinValue, AccelerationMaxValue));
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementInitColor: {
							auto ColorMinValue = GETS32(effectBehaviorArrayItemVec[1]);
							auto ColorMaxValue = GETS32(effectBehaviorArrayItemVec[2]);

							// TODO: uint32
							auto item = m_ssfbBuilder.CreateStruct(
									ss::ssfb::EffectParticleElementInitColor(ColorMinValue, ColorMaxValue));
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementTransColor: {
							auto ColorMinValue = GETS32(effectBehaviorArrayItemVec[1]);
							auto ColorMaxValue = GETS32(effectBehaviorArrayItemVec[2]);

							// TODO: uint32
							auto item = m_ssfbBuilder.CreateStruct(
									ss::ssfb::EffectParticleElementTransColor(ColorMinValue, ColorMaxValue));
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleElementAlphaFade: {
							auto disprangeMinValue = GETFLOAT(effectBehaviorArrayItemVec[1]);
							auto disprangeMaxValue = GETFLOAT(effectBehaviorArrayItemVec[2]);

							auto item = m_ssfbBuilder.CreateStruct(
									ss::ssfb::EffectParticleElementAlphaFade(disprangeMinValue, disprangeMaxValue));
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

							auto item = m_ssfbBuilder.CreateStruct(
									ss::ssfb::EffectParticleElementSize(SizeXMinValue, SizeXMaxValue,
									                                    SizeYMinValue, SizeYMaxValue,
									                                    ScaleFactorMinValue, ScaleFactorMaxValue));
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

							auto item = m_ssfbBuilder.CreateStruct(
									ss::ssfb::EffectParticleElementTransSize(SizeXMinValue, SizeXMaxValue,
									                                         SizeYMinValue, SizeYMaxValue,
									                                         ScaleFactorMinValue, ScaleFactorMaxValue));
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticlePointGravity: {
							auto Position_x = GETFLOAT(effectBehaviorArrayItemVec[1]);
							auto Position_y = GETFLOAT(effectBehaviorArrayItemVec[2]);
							auto Power = GETFLOAT(effectBehaviorArrayItemVec[3]);

							auto item = m_ssfbBuilder.CreateStruct(
									ss::ssfb::EffectParticlePointGravity(Position_x, Position_y, Power));
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleTurnToDirectionEnabled: {
							auto Rotation = GETFLOAT(effectBehaviorArrayItemVec[1]);

							auto item = m_ssfbBuilder.CreateStruct(ss::ssfb::EffectParticleTurnToDirectionEnabled(Rotation));
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_EffectParticleInfiniteEmitEnabled: {
							auto flag = GETS32(effectBehaviorArrayItemVec[1]);

							auto item = m_ssfbBuilder.CreateStruct(ss::ssfb::EffectParticleInfiniteEmitEnabled(flag));
							ssfbEffectNodeBehavior.push_back(item.Union());
							ssfbEffectNodeBehaviorType.push_back(SsEffectFunctionType);
							break;
						}
						case ss::ssfb::EffectNodeBehavior::EffectNodeBehavior_NONE:
						default:
							break;
					}
				}
				auto serializeSsfbEffectNodeBehaviorType = m_ssfbBuilder.CreateVector(ssfbEffectNodeBehaviorType);
				auto serializeSsfbEffectNodeBehavior = m_ssfbBuilder.CreateVector(ssfbEffectNodeBehavior);
				auto ssfbEffectNodeItem = ss::ssfb::CreateEffectNode(m_ssfbBuilder,
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
			auto serializeSsfbEffectNode = m_ssfbBuilder.CreateVector(ssfbEffectNode);
			auto ssfbEffectFile = ss::ssfb::CreateEffectFile(m_ssfbBuilder, ssfbEffectFileName,
                                                             fps, isLockRandSeed, LockRandSeed,
                                                             layoutScaleX, layoutScaleY,
                                                             ssfbEffectNode.size(), serializeSsfbEffectNode);
			m_ssfbEffectFileList.push_back(ssfbEffectFile);
		}
	}

	void createProjectData()
	{
		auto serializeSsfbCells = m_ssfbBuilder.CreateVector(m_ssfbCells);
		auto serializeSsfbAnimePackData = m_ssfbBuilder.CreateVector(m_ssfbAnimePacks);
		auto serializeSsfbEffectFileList = m_ssfbBuilder.CreateVector(m_ssfbEffectFileList);
		m_ssfbProjectData = ss::ssfb::CreateProjectData(m_ssfbBuilder, m_dataId, m_version, 0,
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

