//
//  Lump.h
//

#ifndef Lump_h
#define Lump_h

#include <cstdint>
#include <functional>
#include <memory>
#include <string>
#include <vector>

class LumpSet;

class Lump : public std::enable_shared_from_this<Lump> {
   public:
    enum class DataType {
        S16,
        S32,
        FLOAT,
        COLOR,
        STRING,
        SET
    };

    const DataType type;
    std::variant<int, float, std::string, std::shared_ptr<LumpSet>> data;

    std::string name;
    std::shared_ptr<Lump> parent;

    static auto s16Data(int value, const std::string& name = "") -> std::shared_ptr<Lump>;
    static auto s32Data(int value, const std::string& name = "") -> std::shared_ptr<Lump>;
    static auto floatData(float value, const std::string& name = "") -> std::shared_ptr<Lump>;
    static auto colorData(int color, const std::string& name = "") -> std::shared_ptr<Lump>;
    static auto stringData(const std::string& value, const std::string& name = "") -> std::shared_ptr<Lump>;
    static auto findLump(const std::shared_ptr<Lump>& lump, const std::string& name) -> std::shared_ptr<Lump>;
    // Tree
    // static std::vector<Lump*> filterTree(Lump *root, const std::function<bool(Lump *)> &callback);
    // static void walkTree(const Lump *root, const std::function<void(const Lump *)> &callback);
    void namechack();

    static auto set(const std::string& className, bool isReference = false, const std::string& name = "") -> std::shared_ptr<Lump>;

    void add(const std::shared_ptr<Lump>& lump);
    void addFirst(const std::shared_ptr<Lump>& lump);
    auto count() const -> size_t;
    auto getChild(std::size_t idx) -> std::shared_ptr<Lump>;
    auto getChildren() const -> std::vector<std::shared_ptr<Lump>>&;
    auto findChild(const std::function<bool(const std::shared_ptr<Lump>&)>& compCallback) -> std::shared_ptr<Lump>;

    ~Lump();

    explicit Lump(DataType type);
};

class LumpSet {
   public:
    enum ArrayType {
        NO_ARRAY,
        ARRAY,
        U16_ARRAY
    };

    using SetType = std::vector<std::shared_ptr<Lump>>;

    const std::string className;
    const ArrayType arrayType;
    const bool isReference;
    SetType set;

    LumpSet(const std::string& className, ArrayType arrayType, bool isReference);
    ~LumpSet();

    auto getChild(std::size_t idx) -> std::shared_ptr<Lump>;
    auto findChild(const std::function<bool(const std::shared_ptr<Lump>&)>& compCallback) -> std::shared_ptr<Lump>;
};

#endif
