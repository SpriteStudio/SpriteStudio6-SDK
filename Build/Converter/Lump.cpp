//
//  Lump.cpp
//

#include <cassert>
#include <string>
#include "Lump.h"

static int cnt = 0;
Lump::Lump(DataType type) : type(type)
{
	parent = nullptr;
	name = "NoName" + std::to_string(cnt);
//	name = "NoName";
	cnt++;
}

Lump::~Lump()
{
	switch (type)
	{
		case S16:
		case S32:
		case FLOAT:
		case COLOR:
			break;
		case STRING:
			delete data.s;
			break;
		case SET:
			delete data.p;
			break;
	}
}

std::shared_ptr<Lump> Lump::s16Data(int value, const std::string& name)
{
	auto v = std::make_shared<Lump>(S16);
	v->data.i = value;
	if (!name.empty())
	{
		v->name = name;
	}
	v->namechack();
	return v;
}

std::shared_ptr<Lump> Lump::s32Data(int value, const std::string& name)
{
	auto v = std::make_shared<Lump>(S32);
	v->data.i = value;
	if (!name.empty())
	{
		v->name = name;
	}
	v->namechack();
	return v;
}

std::shared_ptr<Lump> Lump::floatData(float value, const std::string& name)
{
	auto v = std::make_shared<Lump>(FLOAT);
	v->data.f = value;
	if (!name.empty())
	{
		v->name = name;
	}
	v->namechack();
	return v;
}

std::shared_ptr<Lump> Lump::colorData(int color, const std::string& name)
{
	auto v = std::make_shared<Lump>(COLOR);
	v->data.i = color;
	if (!name.empty())
	{
		v->name = name;
	}
	v->namechack();
	return v;
}

std::shared_ptr<Lump> Lump::stringData(const std::string& value, const std::string& name)
{
	auto v = std::make_shared<Lump>(STRING);
	v->data.s = new std::string(value);
	if (!name.empty())
	{
		v->name = name;
	}
	v->namechack();
	return v;
}

std::shared_ptr<Lump> Lump::findLump(const std::shared_ptr<Lump>& lump, const std::string& name)
{
    std::shared_ptr<Lump> found = nullptr;
	if(lump == nullptr || name.empty())
		return nullptr;

	if(lump->name == name) {
		found = lump;
	} else {
		if (lump->type == Lump::DataType::SET) {
			const LumpSet *lset = lump->data.p;
			for (const auto& child : lset->set) {
				found = findLump(child, name);
				if(found != nullptr)
					break;
			}
		}
	}
	return found;
}

/*
std::vector<Lump *> Lump::filterTree(Lump *root, const std::function<bool(Lump *)> &callback)
{
	std::vector<Lump*> lumpVec;

	bool ret = callback(root);
	if(ret) {
		lumpVec.push_back(root);
	}

	if(root->type == Lump::DataType::SET) {
		LumpSet* lset = root->data.p;
		for (auto child : lset->set) {
			std::vector<Lump*> childLumpSet = Lump::filterTree(child, callback);
			lumpVec.insert(lumpVec.end(), childLumpSet.begin(), childLumpSet.end());
		}
	}

	return lumpVec;
}

void Lump::walkTree(const Lump *root, const std::function<void(const Lump *)> &callback)
{
	callback(root);

	if(root->type == Lump::DataType::SET) {
		LumpSet* lset = root->data.p;
		for (auto child : lset->set) {
			std::vector<Lump*> childLumpSet;
			walkTree(child, callback);
		}
	}
}
*/

void Lump::namechack()
{
/*
	if (parent)
	{
		const LumpSet* lset = parent->data.p;
		if (lset->arrayType != LumpSet::U16_ARRAY)
		{
			if (name == "NoName")
			{
				name = "";
			}
		}
	}
	if (name == "NoName")
	{
		name = "";
	}
*/
}

std::shared_ptr<Lump> Lump::set(const std::string& className, bool isReference, const std::string& name)
{
	size_t len = className.length();
	bool isArray = className.at(len-2) == '[' && className.at(len-1) == ']';
	bool isPointerArray = className.at(len-3) == '*';
	LumpSet::ArrayType arrayType = isArray ? LumpSet::ARRAY : LumpSet::NO_ARRAY;

	std::string cname = className;
	if (isArray)
	{
		// ポインタの配列のときは強制的に型を変える
		if (isPointerArray)
		{
			cname = "ss::ss_offset";
		}
		else
		{
			cname = cname.substr(0, len-2);

			// ss_u16型の配列のときは専用配列とする
			if (cname == "ss::ss_u16")
			{
				arrayType = LumpSet::U16_ARRAY;
			}
		}
	}

	auto v = std::make_shared<Lump>(SET);
	v->data.p = new LumpSet(cname, arrayType, isReference);
	v->name = name;
	return v;
}

void Lump::add(const std::shared_ptr<Lump>& lump)
{
	assert(type == SET);
	lump->parent = shared_from_this();
	data.p->set.push_back(lump);
}

void Lump::addFirst(const std::shared_ptr<Lump>& lump)
{
	assert(type == SET);
	lump->parent = shared_from_this();
	data.p->set.insert(data.p->set.begin(), lump);
}

size_t Lump::count() const
{
	assert(type == SET);
	return static_cast<int>(data.p->set.size());
}

std::shared_ptr<Lump> Lump::getChild(std::size_t idx)
{
    assert(type == SET);
    return data.p->getChild(idx);
}

std::vector<std::shared_ptr<Lump>>& Lump::getChildren() const {
    assert(type == SET);
    return data.p->set;
}

std::shared_ptr<Lump> Lump::findChild(const std::function<bool(const std::shared_ptr<Lump>&)> &compCallback) {
	assert(type == SET);
	return data.p->findChild(compCallback);
}

LumpSet::LumpSet(const std::string& className, ArrayType arrayType, bool isReference)
	: className(className)
	, arrayType(arrayType)
	, isReference(isReference)
{}

LumpSet::~LumpSet()
{
	for (auto& lump : set)
	{
		lump.reset();
	}
}

std::shared_ptr<Lump> LumpSet::getChild(std::size_t idx) {
    return this->set[idx];
}

std::shared_ptr<Lump> LumpSet::findChild(const std::function<bool(const std::shared_ptr<Lump>&)> &compCallback) {
    std::shared_ptr<Lump> findLump = nullptr;
	if(compCallback == nullptr)
		return findLump;

	for(const auto& lump : this->set) {
		bool ret = compCallback(lump);
		if(ret) {
			findLump = lump;
			break;
		}
	}
	return findLump;
}



