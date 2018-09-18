//
//  Lump.h
//

#ifndef Lump_h
#define Lump_h

#include <vector>
#include <string>
#include <functional>
#include <cstdint>


class LumpSet;


class Lump
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
	} data;

	std::string name;
	Lump* parent;

	static Lump* s16Data(int value, std::string name = "");
	static Lump* s32Data(int value, std::string name = "");
	static Lump* floatData(float value, std::string name = "");
	static Lump* colorData(int color, std::string name = "");
	static Lump* stringData(const std::string& value, std::string name = "");
	static Lump* findLump(const Lump* lump, std::string name);
	// Tree
	// static std::vector<Lump*> filterTree(Lump *root, const std::function<bool(Lump *)> &callback);
	// static void walkTree(const Lump *root, const std::function<void(const Lump *)> &callback);
	void namechack( void );

	static Lump* set(const std::string& className, bool isReference = false, std::string name = "");

	void add(Lump* lump);
	void addFirst(Lump* lump);
	size_t count() const;
	Lump* getChild(std::size_t idx);
    std::vector<Lump*> getChildren() const;
    Lump* findChild(const std::function<bool(const Lump *)> &compCallback);

	~Lump();

private:
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

	typedef std::vector<Lump*> SetType;

	const std::string	className;
	const ArrayType		arrayType;
	const bool			isReference;
	SetType				set;

	LumpSet(const std::string& className, ArrayType arrayType, bool isReference);
	~LumpSet();

	Lump* getChild(std::size_t idx);
	Lump* findChild(const std::function<bool(const Lump *)> &compCallback);
};



#endif
