//
//  Lump.h
//

#ifndef Lump_h
#define Lump_h

#include <memory>
#include <vector>
#include <string>
#include <functional>
#include <cstdint>

class LumpSet;


class Lump : public std::enable_shared_from_this<Lump>
{
public:
	enum DataType
	{
		S16,
		S32,
		FLOAT,
		COLOR,
		STRING,
		SET
	};

	const DataType	type;
	union
	{
		int				i;
		float			f;
		std::string*	s;
		LumpSet*		p;
	} data{};

	std::string name;
    std::shared_ptr<Lump> parent;

	static std::shared_ptr<Lump> s16Data(int value, const std::string& name = "");
	static std::shared_ptr<Lump> s32Data(int value, const std::string& name = "");
	static std::shared_ptr<Lump> floatData(float value, const std::string& name = "");
	static std::shared_ptr<Lump> colorData(int color, const std::string& name = "");
	static std::shared_ptr<Lump> stringData(const std::string& value, const std::string& name = "");
	static std::shared_ptr<Lump> findLump(const std::shared_ptr<Lump>& lump, const std::string& name);
	// Tree
	// static std::vector<Lump*> filterTree(Lump *root, const std::function<bool(Lump *)> &callback);
	// static void walkTree(const Lump *root, const std::function<void(const Lump *)> &callback);
	void namechack();

	static std::shared_ptr<Lump> set(const std::string& className, bool isReference = false, const std::string& name = "");

	void add(const std::shared_ptr<Lump>& lump);
	void addFirst(const std::shared_ptr<Lump>& lump);
	size_t count() const;
    std::shared_ptr<Lump> getChild(std::size_t idx);
    std::vector<std::shared_ptr<Lump>>& getChildren() const;
    std::shared_ptr<Lump> findChild(const std::function<bool(const std::shared_ptr<Lump>&)> &compCallback);

	~Lump();

    explicit Lump(DataType type);
};



class LumpSet
{
public:
	enum ArrayType
	{
		NO_ARRAY,
		ARRAY,
		U16_ARRAY
	};

	typedef std::vector<std::shared_ptr<Lump>> SetType;

	const std::string	className;
	const ArrayType		arrayType;
	const bool			isReference;
	SetType				set;

	LumpSet(const std::string& className, ArrayType arrayType, bool isReference);
	~LumpSet();

	std::shared_ptr<Lump> getChild(std::size_t idx);
    std::shared_ptr<Lump> findChild(const std::function<bool(const std::shared_ptr<Lump>&)> &compCallback);
};



#endif
