//
//  LumpExporter.cpp
//

#include "LumpExporter.h"

#include <cassert>
#include <cstdarg>

#include "BinaryDataWriter.h"
#include "Lump.h"
#include "flatbuffers/flatbuffers.h"
#include "picojson.h"
#include "sscharconverter.h"
#include "ssfb_generated.h"

namespace LumpExporter {

static std::string format(const char* fmt, std::va_list arg) {
    char buffer[0x1000];
    vsnprintf(buffer, 0x1000, fmt, arg);
    return {buffer};
}

static std::string format(const char* fmt, ...) {
    std::va_list arg;
    va_start(arg, fmt);
    std::string s = format(fmt, arg);
    va_end(arg);
    return s;
}

static std::string encode(const std::string& str, StringEncoding encoding) {
    switch (encoding) {
        case UTF8:
            return str;
        case SJIS:
            return spritestudio6::SsCharConverter::utf8_to_sjis(str);  // TODO:
        default:
            break;
    }
    return str;
}

class CSourceExporter {
   public:
    static void save(std::ostream& out, StringEncoding encoding, const std::shared_ptr<Lump>& lump, const std::string& topLabel, const std::string& creatorComment) {
        auto* exporter = new CSourceExporter();
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
    typedef std::map<const std::shared_ptr<Lump>, std::string> LabelMapType;

    LabelMapType m_labelMap;
    StringEncoding m_encoding;
    std::string m_topLabel;

    void writeStrings(std::ostream& out, const std::shared_ptr<Lump>& lump) {
        const LumpSet* lset = lump->data.p;

        for (const auto& child : lset->set) {
            if (child->type == Lump::SET) {
                writeStrings(out, child);
            }
        }

        for (const auto& child : lset->set) {
            if (child->type == Lump::STRING) {
                if (m_labelMap.find(child) == m_labelMap.end()) {
                    std::string label = format("label_%04d", m_labelMap.size());
                    m_labelMap[child] = label;

                    std::string str = encode(*child->data.s, m_encoding);

                    out << format("static const char %s[] = \"%s\";\n", label.c_str(), str.c_str());
                }
            }
        }
    }

    union MixType {
        int i;
        float f;
    };

    void writeLumpSetBlock(std::ostream& out, const std::shared_ptr<Lump>& lump) {
        const LumpSet* lset = lump->data.p;

        out << format("{");

        // ノーマルのデータ構造
        if (lset->arrayType == LumpSet::NO_ARRAY || lset->arrayType == LumpSet::ARRAY) {
            bool second = false;
            for (const auto& child : lset->set) {
                if (second) out << format(",");
                second = true;

                switch (child->type) {
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
                        if (child->data.p->isReference) {
                            out << format("(ss_offset)((char*)%s%s - (char*)&%s)",
                                          child->data.p->arrayType == LumpSet::NO_ARRAY ? "&" : "",
                                          m_labelMap[child].c_str(),
                                          m_topLabel.c_str());
                        } else {
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
        else if (lset->arrayType == LumpSet::U16_ARRAY) {
            bool second = false;
            for (const auto& child : lset->set) {
                if (second) out << format(",");
                second = true;

                switch (child->type) {
                    case Lump::S16:
                        out << format("0x%x", child->data.i);
                        break;
                    case Lump::S32:
                        out << format("0x%x,0x%x", child->data.i & 0xffff, (child->data.i >> 16) & 0xffff);
                        break;
                    case Lump::FLOAT: {
                        MixType mix{};
                        mix.f = child->data.f;
                        int value = mix.i;
                        out << format("0x%x,0x%x", value & 0xffff, (value >> 16) & 0xffff);
                    } break;
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

    void writeReferenceLumpSet(std::ostream& out, const std::shared_ptr<Lump>& lump, int callDepth = 0) {
        const LumpSet* lset = lump->data.p;

        for (const auto& child : lset->set) {
            if (child->type == Lump::SET) {
                writeReferenceLumpSet(out, child, callDepth + 1);
            }
        }

        if (lset->isReference) {
            if (m_labelMap.find(lump) == m_labelMap.end()) {
                std::string label = format("label_%04d", m_labelMap.size());
                m_labelMap[lump] = label;

                bool isStaticPrefix = callDepth > 0;

                out << format("%sconst %s %s%s = ",
                              isStaticPrefix ? "static " : "",
                              lset->className.c_str(),
                              callDepth == 0 ? m_topLabel.c_str() : label.c_str(),
                              lset->arrayType == LumpSet::NO_ARRAY ? "" : "[]");

                writeLumpSetBlock(out, lump);

                out << format(";\n");
            }
        }
    }
};

class BinaryExporter {
   public:
    static void save(std::ostream& out, StringEncoding encoding, const std::shared_ptr<Lump>& lump, const std::string& creatorComment) {
        auto* exporter = new BinaryExporter();
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
    typedef std::map<const std::shared_ptr<Lump>, std::string> LabelMapType;

    LabelMapType m_labelMap;
    StringEncoding m_encoding;

    void writeStrings(BinaryDataWriter& writer, const std::shared_ptr<Lump>& lump) {
        const LumpSet* lset = lump->data.p;

        for (const auto& child : lset->set) {
            if (child->type == Lump::SET) {
                writeStrings(writer, child);
            }
        }

        for (const auto& child : lset->set) {
            if (child->type == Lump::STRING) {
                if (m_labelMap.find(child) == m_labelMap.end()) {
                    std::string label = format("label_%04d", m_labelMap.size());
                    m_labelMap[child] = label;

                    std::string str = encode(*child->data.s, m_encoding);

                    writer.setReference(label);
                    writer.writeString(str);
                }
            }
        }
    }

    void writeLumpSetBlock(BinaryDataWriter& writer, const std::shared_ptr<Lump>& lump) {
        const auto* lset = lump->data.p;

        // ノーマルのデータ構造
        if (lset->arrayType == LumpSet::NO_ARRAY || lset->arrayType == LumpSet::ARRAY) {
            for (const auto& child : lset->set) {
                switch (child->type) {
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
                        if (child->data.p->isReference) {
                            assert(m_labelMap.find(child) != m_labelMap.end());
                            writer.writeReference(m_labelMap[child]);
                        } else {
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
        else if (lset->arrayType == LumpSet::U16_ARRAY) {
            for (const auto& child : lset->set) {
                switch (child->type) {
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
                        writer.writeReference(m_labelMap[child], false);  // 4バイト境界化しない
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

    void writeReferenceLumpSet(BinaryDataWriter& writer, const std::shared_ptr<Lump>& lump, int callDepth = 0) {
        const LumpSet* lset = lump->data.p;

        for (const auto& child : lset->set) {
            if (child->type == Lump::SET) {
                writeReferenceLumpSet(writer, child, callDepth + 1);
            }
        }

        if (lset->isReference) {
            if (m_labelMap.find(lump) == m_labelMap.end()) {
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

class JsonExporter {
   public:
    static void save(std::ostream& out, StringEncoding encoding, const std::shared_ptr<Lump>& lump, const std::string& creatorComment) {
        auto* exporter = new JsonExporter();
        exporter->m_encoding = encoding;

        exporter->ssjson.clear();
        exporter->ssjson.insert(std::make_pair("creatorComment", picojson::value(creatorComment)));

        exporter->writeReferenceLumpSet(out, lump, exporter->ssjson);

        out << picojson::value(exporter->ssjson);

        delete exporter;
    }

   private:
    picojson::object ssjson;

    typedef std::map<const std::shared_ptr<Lump>, std::string> LabelMapType;

    LabelMapType m_labelMap;
    StringEncoding m_encoding;
    std::string m_topLabel;

    void writeStrings(std::ostream& out, const std::shared_ptr<Lump>& lump) {
        const LumpSet* lset = lump->data.p;

        for (const auto& child : lset->set) {
            if (child->type == Lump::SET) {
                writeStrings(out, child);
            }
        }

        for (const auto& child : lset->set) {
            if (child->type == Lump::STRING) {
                if (m_labelMap.find(child) == m_labelMap.end()) {
                    std::string label = format("label_%04d", m_labelMap.size());
                    m_labelMap[child] = label;

                    std::string str = encode(*child->data.s, m_encoding);
                }
            }
        }
    }

    union MixType {
        int i;
        float f;
    };

    void writeLumpSetBlock(std::ostream& out, const std::shared_ptr<Lump>& lump, picojson::object& ssjson) {
        const LumpSet* lset = lump->data.p;

        // ノーマルのデータ構造
        if (lset->arrayType == LumpSet::NO_ARRAY || lset->arrayType == LumpSet::ARRAY) {
            picojson::array arrayjson;
            arrayjson.clear();

            bool second = false;
            for (const auto& child : lset->set) {
                switch (child->type) {
                    case Lump::S16:
                    case Lump::S32:
                    case Lump::COLOR:
                        if (lset->arrayType == LumpSet::ARRAY) {
                            arrayjson.push_back(picojson::value((double)child->data.i));
                        } else {
                            ssjson.insert(std::make_pair(child->name, picojson::value((double)child->data.i)));
                        }
                        break;
                    case Lump::FLOAT:
                        if (lset->arrayType == LumpSet::ARRAY) {
                            arrayjson.push_back(picojson::value((double)child->data.f));
                        } else {
                            ssjson.insert(std::make_pair(child->name, picojson::value((double)child->data.f)));
                        }
                        break;
                    case Lump::STRING:
                        if (lset->arrayType == LumpSet::ARRAY) {
                            arrayjson.push_back(picojson::value(child->data.s->c_str()));
                        } else {
                            ssjson.insert(std::make_pair(child->name, picojson::value(child->data.s->c_str())));
                        }
                        break;
                    case Lump::SET:

                        if (lset->arrayType == LumpSet::ARRAY) {
                            picojson::object json;
                            json.clear();
                            writeLumpSetBlock(out, child, json);
                            arrayjson.push_back(picojson::value(json));
                        } else {
                            const LumpSet* clset = child->data.p;
                            if (clset->arrayType == LumpSet::ARRAY) {
                                // 子供が配列の場合はそのまま出力する
                                writeLumpSetBlock(out, child, ssjson);
                            } else {
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
            if (lset->arrayType == LumpSet::ARRAY) {
                ssjson.insert(std::make_pair(lump->name, picojson::value(arrayjson)));
            }
        }

        // u16型の配列
        else if (lset->arrayType == LumpSet::U16_ARRAY) {
            picojson::array arrayjson;
            arrayjson.clear();

            picojson::object json;
            json.clear();

            for (const auto& child : lset->set) {
                switch (child->type) {
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

    void writeReferenceLumpSet(std::ostream& out, const std::shared_ptr<Lump>& lump, picojson::object& ssjson, int callDepth = 0) {
        const LumpSet* lset = lump->data.p;

        if (lset->isReference) {
            if (m_labelMap.find(lump) == m_labelMap.end()) {
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

class SsfbExporter {
   public:
    static void save(std::ostream& out, StringEncoding encoding, const std::shared_ptr<Lump>& root, const std::string& creatorComment, const std::vector<int16_t>& frameIndexVec) {
        if (encoding != UTF8) {
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
    StringEncoding m_encoding{};
    std::shared_ptr<Lump> m_root{nullptr};
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

    using CellMapCompare = std::function<bool(const struct ss::ssfb::CellMapT&, const struct ss::ssfb::CellMapT&)>;
    CellMapCompare cellMapCompare = [](const struct ss::ssfb::CellMapT& lhs, const struct ss::ssfb::CellMapT& rhs) {
        return std::tie(lhs.name, lhs.imagePath, lhs.index, lhs.wrapmode, lhs.filtermode) <
               std::tie(rhs.name, rhs.imagePath, rhs.index, rhs.wrapmode, rhs.filtermode);
    };
    std::map<struct ss::ssfb::CellMapT, flatbuffers::Offset<ss::ssfb::CellMap>, CellMapCompare> m_ssfbCellMapsMap{cellMapCompare};
    std::vector<flatbuffers::Offset<ss::ssfb::CellMap>> m_ssfbCellMaps;

    using AnimationInitialDataCompare = std::function<bool(const struct ss::ssfb::AnimationInitialDataT&, const struct ss::ssfb::AnimationInitialDataT&)>;
    AnimationInitialDataCompare animationInitialDataCompare = [](const struct ss::ssfb::AnimationInitialDataT& lhs, const struct ss::ssfb::AnimationInitialDataT& rhs) {
        return std::tie(lhs.index, lhs.lowflag, lhs.highflag, lhs.priority, lhs.cellIndex, lhs.opacity, lhs.localopacity, lhs.masklimen, lhs.positionX, lhs.positionY,
                        lhs.positionZ, lhs.pivotX, lhs.pivotY, lhs.rotationX, lhs.rotationY, lhs.rotationZ, lhs.scaleX, lhs.scaleY, lhs.localscaleX, lhs.localscaleY, lhs.size_X,
                        lhs.size_Y, lhs.uv_move_X, lhs.uv_move_Y, lhs.uv_rotation, lhs.uv_scale_X, lhs.uv_scale_Y, lhs.boundingRadius, lhs.instanceValue_curKeyframe, lhs.instanceValue_startFrame,
                        lhs.instanceValue_endFrame, lhs.instanceValue_loopNum, lhs.instanceValue_speed, lhs.instanceValue_loopflag, lhs.effectValue_curKeyframe, lhs.effectValue_startTime, lhs.effectValue_speed, lhs.effectValue_loopflag) <
               std::tie(rhs.index, rhs.lowflag, rhs.highflag, rhs.priority, rhs.cellIndex, rhs.opacity, rhs.localopacity, rhs.masklimen, rhs.positionX, rhs.positionY,
                        rhs.positionZ, rhs.pivotX, rhs.pivotY, rhs.rotationX, rhs.rotationY, rhs.rotationZ, rhs.scaleX, rhs.scaleY, rhs.localscaleX, rhs.localscaleY, rhs.size_X,
                        rhs.size_Y, rhs.uv_move_X, rhs.uv_move_Y, rhs.uv_rotation, rhs.uv_scale_X, rhs.uv_scale_Y, rhs.boundingRadius, rhs.instanceValue_curKeyframe, rhs.instanceValue_startFrame,
                        rhs.instanceValue_endFrame, rhs.instanceValue_loopNum, rhs.instanceValue_speed, rhs.instanceValue_loopflag, rhs.effectValue_curKeyframe, rhs.effectValue_startTime, rhs.effectValue_speed, rhs.effectValue_loopflag);
    };
    std::map<struct ss::ssfb::AnimationInitialDataT, flatbuffers::Offset<ss::ssfb::AnimationInitialData>, AnimationInitialDataCompare> m_ssfbAnimationInitialDataMap{animationInitialDataCompare};
    std::vector<flatbuffers::Offset<ss::ssfb::AnimationInitialData>> m_ssfbAnimationInitialDataVec;

    using PartDataCompare = std::function<bool(const struct ss::ssfb::PartDataT&, const struct ss::ssfb::PartDataT&)>;
    PartDataCompare partDataCompare = [](const struct ss::ssfb::PartDataT& lhs, const struct ss::ssfb::PartDataT& rhs) {
        return std::tie(lhs.name, lhs.index, lhs.parentIndex, lhs.type, lhs.boundsType, lhs.alphaBlendType, lhs.refname, lhs.effectfilename, lhs.colorLabel, lhs.maskInfluence) <
               std::tie(rhs.name, rhs.index, rhs.parentIndex, rhs.type, rhs.boundsType, rhs.alphaBlendType, rhs.refname, rhs.effectfilename, rhs.colorLabel, rhs.maskInfluence);
    };
    std::map<struct ss::ssfb::PartDataT, flatbuffers::Offset<ss::ssfb::PartData>, PartDataCompare> m_ssfbPartDataMap{partDataCompare};
    std::vector<flatbuffers::Offset<ss::ssfb::PartData>> m_ssfbPartDataVec;

    using VectorUint32Compare = std::function<bool(const std::vector<uint32_t>&, const std::vector<uint32_t>&)>;
    VectorUint32Compare vectorUint32Compare = [](const std::vector<uint32_t>& lhs, const std::vector<uint32_t>& rhs) {
        return lhs < rhs;
    };
    std::map<std::vector<uint32_t>, flatbuffers::Offset<flatbuffers::Vector<uint32_t>>, VectorUint32Compare> m_ssfbUint32VecMap{vectorUint32Compare};
    std::vector<flatbuffers::Offset<flatbuffers::Vector<uint32_t>>> m_ssfbUint32VecVec;

    using VectorFloatCompare = std::function<bool(const std::vector<float>&, const std::vector<float>&)>;
    VectorFloatCompare vectorFloatCompare = [](const std::vector<float>& lhs, const std::vector<float>& rhs) {
        return lhs < rhs;
    };
    std::map<std::vector<float>, flatbuffers::Offset<flatbuffers::Vector<float>>, VectorFloatCompare> m_ssfbFloatVecMap{vectorFloatCompare};
    std::vector<flatbuffers::Offset<flatbuffers::Vector<float>>> m_ssfbFloatVecVec;

    using meshDataUVCompare = std::function<bool(const struct ss::ssfb::meshDataUVT&, const struct ss::ssfb::meshDataUVT&)>;
    meshDataUVCompare ddd = [](const struct ss::ssfb::meshDataUVT& lhs, const struct ss::ssfb::meshDataUVT& rhs) {
        return lhs.uv < rhs.uv;
    };
    std::map<struct ss::ssfb::meshDataUVT, flatbuffers::Offset<ss::ssfb::meshDataUV>, meshDataUVCompare> m_ssfbMeshDataUVMap{ddd};
    std::vector<flatbuffers::Offset<ss::ssfb::meshDataUV>> m_ssfbMeshDataUVVec;

    using meshDataIndicesCompare = std::function<bool(const struct ss::ssfb::meshDataIndicesT&, const struct ss::ssfb::meshDataIndicesT&)>;
    meshDataIndicesCompare eee = [](const struct ss::ssfb::meshDataIndicesT& lhs, const struct ss::ssfb::meshDataIndicesT& rhs) {
        return lhs.indices < rhs.indices;
    };
    std::map<struct ss::ssfb::meshDataIndicesT, flatbuffers::Offset<ss::ssfb::meshDataIndices>, meshDataIndicesCompare> m_ssfbMeshDataIndicesMap{eee};

    std::vector<flatbuffers::Offset<ss::ssfb::meshDataIndices>> m_ssfbMeshDataIndicesVec;

    using partStateCompare = std::function<bool(const struct ss::ssfb::partStateT&, const struct ss::ssfb::partStateT&)>;
    partStateCompare ggg = [](const struct ss::ssfb::partStateT& lhs, const struct ss::ssfb::partStateT& rhs) {
        return std::tie(lhs.index, lhs.flag1, lhs.flag2, lhs.data) <
               std::tie(rhs.index, rhs.flag1, rhs.flag2, rhs.data);
    };
    std::map<struct ss::ssfb::partStateT, flatbuffers::Offset<ss::ssfb::partState>, partStateCompare> m_ssfbPartStateMap{ggg};
    std::vector<flatbuffers::Offset<ss::ssfb::partState>> m_ssfbPartStateVec;

    using frameDataIndexCompare = std::function<bool(const struct ss::ssfb::frameDataIndexT&, const struct ss::ssfb::frameDataIndexT&)>;
    frameDataIndexCompare hhh = [](const struct ss::ssfb::frameDataIndexT& lhs, const struct ss::ssfb::frameDataIndexT& rhs) {
        return lhs.states < rhs.states;
    };
    std::map<struct ss::ssfb::frameDataIndexT, flatbuffers::Offset<ss::ssfb::frameDataIndex>, frameDataIndexCompare> m_ssfbFrameDataIndexMap{hhh};
    std::vector<struct ss::ssfb::frameDataIndexT> m_frameDataIndexVec;
    std::vector<flatbuffers::Offset<ss::ssfb::frameDataIndex>> m_ssfbFrameDataIndexVec;

    enum {
        USER_DATA_FLAG_INTEGER = 1 << 0,
        USER_DATA_FLAG_RECT = 1 << 1,
        USER_DATA_FLAG_POINT = 1 << 2,
        USER_DATA_FLAG_STRING = 1 << 3
    };

    void createHeader() {
        const auto& rootChildVec = m_root->getChildren();
        m_dataId = GETS32(rootChildVec[0]);
        m_version = GETS32(rootChildVec[1]);
        m_flags = GETS32(rootChildVec[2]);
        m_ssfbImageBaseDir = GETSSFBSTRING(m_ssfbBuilder, rootChildVec[3], m_encoding);
    }

    void createCells() {
        const auto& rootChildVec = m_root->getChildren();
        // 4:Cell
        const auto& cellsLump = rootChildVec[4];

        const auto& cellsVec = cellsLump->getChildren();
        for (const auto& cellItem : cellsVec) {
            auto& cellItemVec = cellItem->getChildren();
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

    flatbuffers::Offset<ss::ssfb::CellMap> createSharedCellMap(const std::shared_ptr<Lump>& lump) {
        flatbuffers::Offset<ss::ssfb::CellMap> cellMap;

        auto& cellMapVec = lump->getChildren();

        struct ss::ssfb::CellMapT cellMapT;
        cellMapT.name = GETSTRING(cellMapVec[0], m_encoding);
        cellMapT.imagePath = GETSTRING(cellMapVec[1], m_encoding);
        cellMapT.index = GETS16(cellMapVec[2]);
        cellMapT.wrapmode = GETS16(cellMapVec[3]);
        cellMapT.filtermode = GETS16(cellMapVec[4]);
        // 5:reserved(s16)

        // search same cellMap from cellMap caches.
        auto it = m_ssfbCellMapsMap.find(cellMapT);
        if (it == m_ssfbCellMapsMap.end()) {
            // not found
            // create ssfb cellMap
            auto ssfbCellMapName = m_ssfbBuilder.CreateSharedString(cellMapT.name);
            auto ssfbCellMapImagePath = m_ssfbBuilder.CreateSharedString(cellMapT.imagePath);
            cellMap = ss::ssfb::CreateCellMap(m_ssfbBuilder, ssfbCellMapName, ssfbCellMapImagePath,
                                              cellMapT.index, cellMapT.wrapmode, cellMapT.filtermode);
            // cache ssfb cellMap
            m_ssfbCellMapsMap[cellMapT] = cellMap;
            m_ssfbCellMaps.push_back(cellMap);
        } else {
            // found
            cellMap = it->second;
        }

        return cellMap;
    }

    flatbuffers::Offset<ss::ssfb::AnimationInitialData> createSharedAnimationInitialData(const std::shared_ptr<Lump>& lump) {
        flatbuffers::Offset<ss::ssfb::AnimationInitialData> animationInitialData;
        auto& AnimationInitialDataItemVec = lump->getChildren();

        struct ss::ssfb::AnimationInitialDataT animationInitialDataT;

        animationInitialDataT.index = GETS16(AnimationInitialDataItemVec[0]);
        // 1:reserve
        animationInitialDataT.lowflag = GETS32(AnimationInitialDataItemVec[2]);
        animationInitialDataT.highflag = GETS32(AnimationInitialDataItemVec[3]);
        animationInitialDataT.priority = GETS16(AnimationInitialDataItemVec[4]);
        animationInitialDataT.cellIndex = GETS16(AnimationInitialDataItemVec[5]);
        animationInitialDataT.opacity = GETS16(AnimationInitialDataItemVec[6]);
        animationInitialDataT.localopacity = GETS16(AnimationInitialDataItemVec[7]);
        animationInitialDataT.masklimen = GETS16(AnimationInitialDataItemVec[8]);
        // 9:reserved
        animationInitialDataT.positionX = GETFLOAT(AnimationInitialDataItemVec[10]);
        animationInitialDataT.positionY = GETFLOAT(AnimationInitialDataItemVec[11]);
        animationInitialDataT.positionZ = GETFLOAT(AnimationInitialDataItemVec[12]);
        animationInitialDataT.pivotX = GETFLOAT(AnimationInitialDataItemVec[13]);
        animationInitialDataT.pivotY = GETFLOAT(AnimationInitialDataItemVec[14]);
        animationInitialDataT.rotationX = GETFLOAT(AnimationInitialDataItemVec[15]);
        animationInitialDataT.rotationY = GETFLOAT(AnimationInitialDataItemVec[16]);
        animationInitialDataT.rotationZ = GETFLOAT(AnimationInitialDataItemVec[17]);
        animationInitialDataT.scaleX = GETFLOAT(AnimationInitialDataItemVec[18]);
        animationInitialDataT.scaleY = GETFLOAT(AnimationInitialDataItemVec[19]);
        animationInitialDataT.localscaleX = GETFLOAT(AnimationInitialDataItemVec[20]);
        animationInitialDataT.localscaleY = GETFLOAT(AnimationInitialDataItemVec[21]);
        animationInitialDataT.size_X = GETFLOAT(AnimationInitialDataItemVec[22]);
        animationInitialDataT.size_Y = GETFLOAT(AnimationInitialDataItemVec[23]);
        animationInitialDataT.uv_move_X = GETFLOAT(AnimationInitialDataItemVec[24]);
        animationInitialDataT.uv_move_Y = GETFLOAT(AnimationInitialDataItemVec[25]);
        animationInitialDataT.uv_rotation = GETFLOAT(AnimationInitialDataItemVec[26]);
        animationInitialDataT.uv_scale_X = GETFLOAT(AnimationInitialDataItemVec[27]);
        animationInitialDataT.uv_scale_Y = GETFLOAT(AnimationInitialDataItemVec[28]);
        animationInitialDataT.boundingRadius = GETFLOAT(AnimationInitialDataItemVec[29]);
        // インスタンス関連
        animationInitialDataT.instanceValue_curKeyframe = GETS32(AnimationInitialDataItemVec[30]);
        animationInitialDataT.instanceValue_startFrame = GETS32(AnimationInitialDataItemVec[31]);
        animationInitialDataT.instanceValue_endFrame = GETS32(AnimationInitialDataItemVec[32]);
        animationInitialDataT.instanceValue_loopNum = GETS32(AnimationInitialDataItemVec[33]);
        animationInitialDataT.instanceValue_speed = GETFLOAT(AnimationInitialDataItemVec[34]);
        animationInitialDataT.instanceValue_loopflag = GETS32(AnimationInitialDataItemVec[35]);
        // エフェクト関連
        animationInitialDataT.effectValue_curKeyframe = GETS32(AnimationInitialDataItemVec[36]);
        animationInitialDataT.effectValue_startTime = GETS32(AnimationInitialDataItemVec[37]);
        animationInitialDataT.effectValue_speed = GETFLOAT(AnimationInitialDataItemVec[38]);
        animationInitialDataT.effectValue_loopflag = GETS32(AnimationInitialDataItemVec[39]);

        // search same cellMap from cellMap caches.
        auto result = m_ssfbAnimationInitialDataMap.find(animationInitialDataT);
        // auto result = std::find(m_animationInitialDataVec.begin(), m_animationInitialDataVec.end(), animationInitialDataT);
        if (result == m_ssfbAnimationInitialDataMap.end()) {
            // not found

            // create ssfb partData
            // animationInitialData = m_ssfbBuilder.Create
            animationInitialData = ss::ssfb::CreateAnimationInitialData(m_ssfbBuilder,
                                                                        animationInitialDataT.index,
                                                                        animationInitialDataT.lowflag,
                                                                        animationInitialDataT.highflag,
                                                                        animationInitialDataT.priority,
                                                                        animationInitialDataT.cellIndex,
                                                                        animationInitialDataT.opacity,
                                                                        animationInitialDataT.localopacity,
                                                                        animationInitialDataT.masklimen,
                                                                        animationInitialDataT.positionX,
                                                                        animationInitialDataT.positionY,
                                                                        animationInitialDataT.positionZ,
                                                                        animationInitialDataT.pivotX,
                                                                        animationInitialDataT.pivotY,
                                                                        animationInitialDataT.rotationX,
                                                                        animationInitialDataT.rotationY,
                                                                        animationInitialDataT.rotationZ,
                                                                        animationInitialDataT.scaleX,
                                                                        animationInitialDataT.scaleY,
                                                                        animationInitialDataT.localscaleX,
                                                                        animationInitialDataT.localscaleY,
                                                                        animationInitialDataT.size_X,
                                                                        animationInitialDataT.size_Y,
                                                                        animationInitialDataT.uv_move_X,
                                                                        animationInitialDataT.uv_move_Y,
                                                                        animationInitialDataT.uv_rotation,
                                                                        animationInitialDataT.uv_scale_X,
                                                                        animationInitialDataT.uv_scale_Y,
                                                                        animationInitialDataT.boundingRadius,
                                                                        animationInitialDataT.instanceValue_curKeyframe,
                                                                        animationInitialDataT.instanceValue_startFrame,
                                                                        animationInitialDataT.instanceValue_endFrame,
                                                                        animationInitialDataT.instanceValue_loopNum,
                                                                        animationInitialDataT.instanceValue_speed,
                                                                        animationInitialDataT.instanceValue_loopflag,
                                                                        animationInitialDataT.effectValue_curKeyframe,
                                                                        animationInitialDataT.effectValue_startTime,
                                                                        animationInitialDataT.effectValue_speed,
                                                                        animationInitialDataT.effectValue_loopflag);
            // cache ssfb cellMap
            m_ssfbAnimationInitialDataMap[animationInitialDataT] = animationInitialData;
            m_ssfbAnimationInitialDataVec.push_back(animationInitialData);
        } else {
            // found
            animationInitialData = m_ssfbAnimationInitialDataMap[animationInitialDataT];
        }

        return animationInitialData;
    }

    flatbuffers::Offset<ss::ssfb::PartData> createSharedPartData(const std::shared_ptr<Lump>& lump) {
        flatbuffers::Offset<ss::ssfb::PartData> partData;
        auto& partDataItemVec = lump->getChildren();

        struct ss::ssfb::PartDataT partDataT;

        partDataT.name = GETSTRING(partDataItemVec[0], m_encoding);
        partDataT.index = GETS16(partDataItemVec[1]);
        partDataT.parentIndex = GETS16(partDataItemVec[2]);
        partDataT.type = (ss::ssfb::SsPartType)GETS16(partDataItemVec[3]);
        partDataT.boundsType = GETS16(partDataItemVec[4]);
        partDataT.alphaBlendType = GETS16(partDataItemVec[5]);
        partDataT.refname = GETSTRING(partDataItemVec[7], m_encoding);
        partDataT.effectfilename = GETSTRING(partDataItemVec[8], m_encoding);
        partDataT.colorLabel = GETSTRING(partDataItemVec[9], m_encoding);
        partDataT.maskInfluence = GETS16(partDataItemVec[10]);

        // search same cellMap from cellMap caches.
        auto result = m_ssfbPartDataMap.find(partDataT);
        if (result == m_ssfbPartDataMap.end()) {
            // not found

            // create ssfb partData
            auto ssfbPartDataName = m_ssfbBuilder.CreateSharedString(partDataT.name);
            auto ssfbRefname = m_ssfbBuilder.CreateSharedString(partDataT.refname);
            auto ssfbEffectfilename = m_ssfbBuilder.CreateSharedString(partDataT.effectfilename);
            auto ssfbColorLabel = m_ssfbBuilder.CreateSharedString(partDataT.colorLabel);

            partData = ss::ssfb::CreatePartData(m_ssfbBuilder, ssfbPartDataName, partDataT.index, partDataT.parentIndex, (ss::ssfb::SsPartType)partDataT.type,
                                                partDataT.boundsType, partDataT.alphaBlendType, ssfbRefname, ssfbEffectfilename, ssfbColorLabel,
                                                partDataT.maskInfluence);
            // cache ssfb cellMap
            m_ssfbPartDataMap[partDataT] = partData;
            m_ssfbPartDataVec.push_back(partData);
        } else {
            // found
            partData = result->second;
        }

        return partData;
    }

    flatbuffers::Offset<flatbuffers::Vector<uint32_t>> createSharedUint32Vec(const std::vector<uint32_t>& vec) {
        flatbuffers::Offset<flatbuffers::Vector<uint32_t>> ssfbVec;

        auto result = m_ssfbUint32VecMap.find(vec);
        if (result == m_ssfbUint32VecMap.end()) {
            // not found

            // create ssfb vec
            ssfbVec = m_ssfbBuilder.CreateVector(vec);

            // cache ssfb vec
            m_ssfbUint32VecMap[vec] = ssfbVec;
            m_ssfbUint32VecVec.push_back(ssfbVec);
        } else {
            ssfbVec = m_ssfbUint32VecMap[vec];
        }

        return ssfbVec;
    }

    flatbuffers::Offset<flatbuffers::Vector<float>> createSharedFloatVec(const std::vector<float>& vec) {
        flatbuffers::Offset<flatbuffers::Vector<float>> ssfbVec;

        auto result = m_ssfbFloatVecMap.find(vec);
        // auto result = std::find(m_floatVecVec.begin(), m_floatVecVec.end(), vec);
        if (result == m_ssfbFloatVecMap.end()) {
            // not found

            // create ssfb vec
            ssfbVec = m_ssfbBuilder.CreateVector(vec);

            // cache ssfb vec
            m_ssfbFloatVecMap[vec] = ssfbVec;
            m_ssfbFloatVecVec.push_back(ssfbVec);
        } else {
            ssfbVec = m_ssfbFloatVecMap[vec];
        }

        return ssfbVec;
    }

    flatbuffers::Offset<ss::ssfb::meshDataUV> createSharedMeshDataUV(const std::vector<float>& uvPrimitive, const flatbuffers::Offset<flatbuffers::Vector<float>>& uv) {
        flatbuffers::Offset<ss::ssfb::meshDataUV> meshDataUV;

        struct ss::ssfb::meshDataUVT meshDataUVT;
        meshDataUVT.uv = uvPrimitive;

        auto result = m_ssfbMeshDataUVMap.find(meshDataUVT);
        if (result == m_ssfbMeshDataUVMap.end()) {
            // not found

            // create ssfb vec
            meshDataUV = ss::ssfb::CreatemeshDataUV(m_ssfbBuilder, uv);

            // cache ssfb vec
            m_ssfbMeshDataUVMap[meshDataUVT] = meshDataUV;
            m_ssfbMeshDataUVVec.push_back(meshDataUV);
        } else {
            meshDataUV = m_ssfbMeshDataUVMap[meshDataUVT];
        }

        return meshDataUV;
    }

    flatbuffers::Offset<ss::ssfb::meshDataIndices> createSharedMeshDataIndices(const std::vector<float>& indicesPrimitive, const flatbuffers::Offset<flatbuffers::Vector<float>>& indices) {
        flatbuffers::Offset<ss::ssfb::meshDataIndices> meshDataIndices;

        struct ss::ssfb::meshDataIndicesT meshDataIndicesT;
        meshDataIndicesT.indices = indicesPrimitive;
        auto result = m_ssfbMeshDataIndicesMap.find(meshDataIndicesT);
        if (result == m_ssfbMeshDataIndicesMap.end()) {
            // not found

            // create ssfb vec
            meshDataIndices = ss::ssfb::CreatemeshDataIndices(m_ssfbBuilder, indices);

            // cache ssfb vec
            m_ssfbMeshDataIndicesMap[meshDataIndicesT] = meshDataIndices;
            m_ssfbMeshDataIndicesVec.push_back(meshDataIndices);
        } else {
            meshDataIndices = m_ssfbMeshDataIndicesMap[meshDataIndicesT];
        }

        return meshDataIndices;
    }

    flatbuffers::Offset<ss::ssfb::partState>
    createSharedPartState(int16_t index, uint32_t flag1, uint32_t flag2, const std::vector<uint32_t>& dataPrimitive) {
        flatbuffers::Offset<ss::ssfb::partState> partState;

        struct ss::ssfb::partStateT partStateT;
        partStateT.index = index;
        partStateT.flag1 = flag1;
        partStateT.flag2 = flag2;
        partStateT.data = dataPrimitive;
        auto result = m_ssfbPartStateMap.find(partStateT);
        // auto result = std::find(m_partStateVec.begin(), m_partStateVec.end(), partStateT);
        if (result == m_ssfbPartStateMap.end()) {
            // not found
            auto serializePartStateData = createSharedUint32Vec(dataPrimitive);
            partState = ss::ssfb::CreatepartState(m_ssfbBuilder, partStateT.index, partStateT.flag1, partStateT.flag2,
                                                  serializePartStateData);

            m_ssfbPartStateMap[partStateT] = partState;
            m_ssfbPartStateVec.push_back(partState);
        } else {
            partState = m_ssfbPartStateMap[partStateT];
        }

        return partState;
    }

    flatbuffers::Offset<ss::ssfb::frameDataIndex> createSharedFrameDataIndex(const std::vector<ss::ssfb::partStateT>& statesPrimitive) {
        flatbuffers::Offset<ss::ssfb::frameDataIndex> frameDataIndex;

        struct ss::ssfb::frameDataIndexT frameDataIndexT1;
        for (const auto& state : statesPrimitive) {
            std::unique_ptr<ss::ssfb::partStateT> p(new ss::ssfb::partStateT());
            p->index = state.index;
            p->flag1 = state.flag1;
            p->flag2 = state.flag2;
            p->data = state.data;
            frameDataIndexT1.states.push_back(std::move(p));
        }
        auto result = std::find_if(m_frameDataIndexVec.begin(), m_frameDataIndexVec.end(), [&frameDataIndexT1](const struct ss::ssfb::frameDataIndexT& item) {
            if (frameDataIndexT1.states.size() != item.states.size())
                return false;

            int idx = 0;
            for (auto& p : frameDataIndexT1.states) {
                auto& i = item.states[idx];
                if (p->index != i->index) return false;
                if (p->flag1 != i->flag1) return false;
                if (p->flag2 != i->flag2) return false;
                if (p->data != i->data) return false;

                idx++;
            }
            return true;
        });
        if (result == m_frameDataIndexVec.end()) {
            // not found

            std::vector<flatbuffers::Offset<ss::ssfb::partState>> vec;
            for (auto i : statesPrimitive) {
                auto item = createSharedPartState(i.index, i.flag1, i.flag2, i.data);
                vec.push_back(item);
            }

            auto serializeVec = m_ssfbBuilder.CreateVector(vec);
            // create ssfb vec
            frameDataIndex = ss::ssfb::CreateframeDataIndex(m_ssfbBuilder, serializeVec);

            // cache ssfb vec
            m_frameDataIndexVec.push_back(std::move(frameDataIndexT1));
            m_ssfbFrameDataIndexVec.push_back(frameDataIndex);
        } else {
            auto idx = std::distance(m_frameDataIndexVec.begin(), result);
            frameDataIndex = m_ssfbFrameDataIndexVec[idx];
        }

        return frameDataIndex;
    }

    void createAnimePacks() {
        const auto& rootChildVec = m_root->getChildren();
        // 5:AnimePackData
        const auto animePackDataLump = rootChildVec[5];

        const auto& animePackDataVec = animePackDataLump->getChildren();
        for (const auto& animePackDataItem : animePackDataVec) {
            flatbuffers::Offset<flatbuffers::String> ssfbAnimePackDataName;
            std::vector<flatbuffers::Offset<ss::ssfb::PartData>> ssfbParts;
            std::vector<flatbuffers::Offset<ss::ssfb::AnimationData>> ssfbAnimations;

            const auto& animePackDataItemVec = animePackDataItem->getChildren();

            ssfbAnimePackDataName = GETSSFBSTRING(m_ssfbBuilder, animePackDataItemVec[0], m_encoding);
            ssfbParts = createParts(animePackDataItemVec[1]);                   // PartData
            ssfbAnimations = createAnimationDataList(animePackDataItemVec[2]);  // AnimationData
            // auto numPart = GETS16(animePackDataItemVec[3]); // unused
            // auto numAnimations = GETS16(animePackDataItemVec[4]); // unused

            auto serializeSsfbParts = m_ssfbBuilder.CreateVector(ssfbParts);
            auto serializeSsfbAnimations = m_ssfbBuilder.CreateVector(ssfbAnimations);
            auto ssfbAnimePackData = ss::ssfb::CreateAnimePackData(m_ssfbBuilder, ssfbAnimePackDataName, serializeSsfbParts, serializeSsfbAnimations);
            m_ssfbAnimePacks.push_back(ssfbAnimePackData);
        }
    }

    std::vector<flatbuffers::Offset<ss::ssfb::PartData>> createParts(const std::shared_ptr<Lump>& lump) {
        std::vector<flatbuffers::Offset<ss::ssfb::PartData>> ssfbParts;

        const auto& partDataVec = lump->getChildren();
        for (const auto& partDataItem : partDataVec) {
            auto ssfbPartDataItem = createSharedPartData(partDataItem);
            ssfbParts.push_back(ssfbPartDataItem);
        }

        return ssfbParts;
    }

    std::vector<flatbuffers::Offset<ss::ssfb::AnimationData>> createAnimationDataList(const std::shared_ptr<Lump>& lump) {
        std::vector<flatbuffers::Offset<ss::ssfb::AnimationData>> ssfbAnimationDataList;

        const auto& animationDataVec = lump->getChildren();
        for (const auto& animationDataItem : animationDataVec) {
            auto ssAnimationDataVec = animationDataItem->getChildren();

            // 1:AnimationInitialData

            std::vector<flatbuffers::Offset<ss::ssfb::AnimationInitialData>> ssfbDefaultData;
            {
                const auto& AnimationInitialDataVec = ssAnimationDataVec[1]->getChildren();
                for (const auto& AnimationInitialDataItem : AnimationInitialDataVec) {
                    auto item = createSharedAnimationInitialData(AnimationInitialDataItem);
                    ssfbDefaultData.push_back(item);
                }
            }

            // 5:meshDataUV
            std::vector<flatbuffers::Offset<ss::ssfb::meshDataUV>> ssfbMeshsDataUV;
            {
                const auto& meshDataUVVec = ssAnimationDataVec[5]->getChildren();
                for (const auto& meshDataUVItem : meshDataUVVec) {
                    std::vector<float> ssfbUV;
                    const auto& frameDataVec = meshDataUVItem->getChildren();
                    for (const auto& frameDataItem : frameDataVec) {
                        switch (frameDataItem->type) {
                            case Lump::DataType::S32:
                                ssfbUV.push_back((float)(GETS32(frameDataItem)));
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
                const auto& meshsDataIndicesVec = ssAnimationDataVec[6]->getChildren();
                for (const auto& meshsDataIndicesItem : meshsDataIndicesVec) {
                    std::vector<float> ssfbIndices;
                    const auto& meshsDataVec = meshsDataIndicesItem->getChildren();
                    for (const auto& meshDataItem : meshsDataVec) {
                        ssfbIndices.push_back((float)(GETS32(meshDataItem)));
                    }

                    auto serializeSsfbIndices = createSharedFloatVec(ssfbIndices);
                    auto item = createSharedMeshDataIndices(ssfbIndices, serializeSsfbIndices);
                    ssfbMeshsDataIndices.push_back(item);
                }
            }
            // 2:frameDataIndexArray

            std::vector<flatbuffers::Offset<ss::ssfb::frameDataIndex>> ssfbFrameData;
            {
                const auto& frameDataIndexArrayVec = ssAnimationDataVec[2]->getChildren();

                for (const auto& frameDataIndexArrayItem : frameDataIndexArrayVec) {
                    const auto& frameDataVec = frameDataIndexArrayItem->getChildren();

                    struct ss::ssfb::partStateT partStateTItem;
                    std::vector<struct ss::ssfb::partStateT> partStateTVec;

                    uint32_t flag1;
                    uint32_t flag2;
                    int outPartsCount = -1;
                    int16_t index;
                    std::string tagname;
                    std::vector<uint32_t> partStateData;
                    for (const auto& frameDataItem : frameDataVec) {
                        if (frameDataItem->name.find("part_") != std::string::npos &&
                            frameDataItem->name.find("_index") != std::string::npos) {
                            if (outPartsCount != -1) {
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
                            flag1 = 0;
                            flag2 = 0;
                            partStateData = {};
                            continue;
                        }

                        if (frameDataItem->name == tagname + "flag1") {
                            flag1 = GETU32(frameDataItem);
                            continue;
                        }

                        if (frameDataItem->name == tagname + "flag2") {
                            flag2 = GETU32(frameDataItem);
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
                            case Lump::DataType::COLOR: {
                                auto rgba = GETU32(frameDataItem);
                                partStateData.push_back((rgba & 0xffff0000) >> 16);
                                partStateData.push_back(rgba & 0xffff);
                            } break;
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
            std::vector<flatbuffers::Offset<ss::ssfb::userDataPerFrame>> ssfbUserData;
            {
                if (ssAnimationDataVec[3]->type == Lump::DataType::SET) {
                    const auto& userDataIndexArrayVec = ssAnimationDataVec[3]->getChildren();
                    for (const auto& userDataIndexArrayItem : userDataIndexArrayVec) {
                        if (userDataIndexArrayItem->type != Lump::DataType::SET) {
                            continue;
                        }
                        std::vector<flatbuffers::Offset<ss::ssfb::userDataItem>> ssfbUserDataItemData;

                        std::vector<flatbuffers::Offset<void>> ssfbDataArray;
                        std::vector<uint8_t> ssfbDataArrayType;

                        const auto& userDataIndexArrayItemVec = userDataIndexArrayItem->getChildren();
                        auto num = GETS16(userDataIndexArrayItemVec[0]);
                        int idx = 1;
                        for (int i = 0; i < num; i++) {
                            auto flags = GETS16(userDataIndexArrayItemVec[idx++]);
                            auto arrayIndex = GETS16(userDataIndexArrayItemVec[idx++]);
                            if (flags & (int16_t)(USER_DATA_FLAG_INTEGER)) {
                                auto integer = GETS32(userDataIndexArrayItemVec[idx++]);
                                auto item = m_ssfbBuilder.CreateStruct(ss::ssfb::userDataInteger(integer));
                                ssfbDataArray.push_back(item.Union());
                                ssfbDataArrayType.push_back(ss::ssfb::userDataValue_userDataInteger);
                            }
                            if (flags & (int16_t)USER_DATA_FLAG_RECT) {
                                auto rect_x = GETS32(userDataIndexArrayItemVec[idx++]);
                                auto rect_y = GETS32(userDataIndexArrayItemVec[idx++]);
                                auto rect_w = GETS32(userDataIndexArrayItemVec[idx++]);
                                auto rect_h = GETS32(userDataIndexArrayItemVec[idx++]);

                                auto item = m_ssfbBuilder.CreateStruct(ss::ssfb::userDataRect(rect_x, rect_y, rect_w, rect_h));
                                ssfbDataArray.push_back(item.Union());
                                ssfbDataArrayType.push_back(ss::ssfb::userDataValue_userDataRect);
                            }
                            if (flags & (int16_t)(USER_DATA_FLAG_POINT)) {
                                auto point_x = GETS32(userDataIndexArrayItemVec[idx++]);
                                auto point_y = GETS32(userDataIndexArrayItemVec[idx++]);

                                auto item = m_ssfbBuilder.CreateStruct(ss::ssfb::userDataPoint(point_x, point_y));
                                ssfbDataArray.push_back(item.Union());
                                ssfbDataArrayType.push_back(ss::ssfb::userDataValue_userDataPoint);
                            }
                            if (flags & (int16_t)(USER_DATA_FLAG_STRING)) {
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
                if (ssAnimationDataVec[4]->type == Lump::DataType::SET) {
                    const auto& LabelDataIndexArrayVec = ssAnimationDataVec[4]->getChildren();
                    for (const auto& LabelDataIndexArrayItem : LabelDataIndexArrayVec) {
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
        const auto& rootChildVec = m_root->getChildren();
        const auto& effectFileLump = rootChildVec[6];
        // 6:EffectFile

        const auto& effectFileLumpVec = effectFileLump->getChildren();
        for (const auto& effectFileLumpItem : effectFileLumpVec) {
            const auto& effectFileLumpItemVec = effectFileLumpItem->getChildren();
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
            for (const auto& EffectNodeArrayItem : EffectNodeArrayVec) {
                const auto& EffectNodeVec = EffectNodeArrayItem->getChildren();

                auto arrayIndex = GETS16(EffectNodeVec[0]);
                auto parentIndex = GETS16(EffectNodeVec[1]);
                auto type = GETS16(EffectNodeVec[2]);
                auto cellIndex = GETS16(EffectNodeVec[3]);
                auto blendType = GETS16(EffectNodeVec[4]);
                auto numBehavior = GETS16(EffectNodeVec[5]);
                const auto& effectBehaviorArrayVec = EffectNodeVec[6]->getChildren();

                std::vector<flatbuffers::Offset<void>> ssfbEffectNodeBehavior;
                std::vector<uint8_t> ssfbEffectNodeBehaviorType;
                for (const auto& effectBehaviorArrayItem : effectBehaviorArrayVec) {
                    const auto& effectBehaviorArrayItemVec = effectBehaviorArrayItem->getChildren();

                    auto SsEffectFunctionType = (ss::ssfb::EffectNodeBehavior)GETS32(effectBehaviorArrayItemVec[0]);
                    switch (SsEffectFunctionType) {
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
                                                             (int16_t)(ssfbEffectNode.size()), serializeSsfbEffectNode);
            m_ssfbEffectFileList.push_back(ssfbEffectFile);
        }
    }

    void createProjectData() {
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
        out.write(reinterpret_cast<const char*>(m_ssfbBuilder.GetBufferPointer()), m_ssfbBuilder.GetSize());
    }
};
#undef GETS16
#undef GETS32
#undef GETU32
#undef GETFLOAT
#undef GETSTRING
#undef GETSSFBSTRING

void saveBinary(std::ostream& out, StringEncoding encoding, const std::shared_ptr<Lump>& lump, const std::string& creatorComment) {
    BinaryExporter::save(out, encoding, lump, creatorComment);
}

void saveCSource(std::ostream& out, StringEncoding encoding, const std::shared_ptr<Lump>& lump, const std::string& topLabel, const std::string& creatorComment) {
    CSourceExporter::save(out, encoding, lump, topLabel, creatorComment);
}

void saveJson(std::ostream& out, StringEncoding encoding, const std::shared_ptr<Lump>& lump, const std::string& creatorComment) {
    JsonExporter::save(out, encoding, lump, creatorComment);
}

void saveSsfb(std::ostream& out, StringEncoding encoding, const std::shared_ptr<Lump>& lump, const std::string& creatorComment, const std::vector<int16_t>& frameIndexVec) {
    SsfbExporter::save(out, encoding, lump, creatorComment, frameIndexVec);
}

}  // namespace LumpExporter
