//
//  Lump.cpp
//

#include "Lump.h"
#include <assert.h>
#include <string>

static int cnt = 0;
Lump::Lump(DataType type) : type(type) 
{
	parent = NULL;
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

Lump* Lump::s16Data(int value, std::string name)
{
	Lump* v = new Lump(S16);
	v->data.i = value;
	if (name != "")
	{
		v->name = name;
	}
	v->namechack();
	return v;
}

Lump* Lump::s32Data(int value, std::string name)
{
	Lump* v = new Lump(S32);
	v->data.i = value;
	if (name != "")
	{
		v->name = name;
	}
	v->namechack();
	return v;
}

Lump* Lump::floatData(float value, std::string name)
{
	Lump* v = new Lump(FLOAT);
	v->data.f = value;
	if (name != "")
	{
		v->name = name;
	}
	v->namechack();
	return v;
}

Lump* Lump::colorData(int color, std::string name)
{
	Lump* v = new Lump(COLOR);
	v->data.i = color;
	if (name != "")
	{
		v->name = name;
	}
	v->namechack();
	return v;
}

Lump* Lump::stringData(const std::string& value, std::string name)
{
	Lump* v = new Lump(STRING);
	v->data.s = new std::string(value);
	if (name != "")
	{
		v->name = name;
	}
	v->namechack();
	return v;
}

void Lump::namechack( void )
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

Lump* Lump::set(const std::string& className, bool isReference, std::string name)
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

	Lump* v = new Lump(SET);
	v->data.p = new LumpSet(cname, arrayType, isReference);
	v->name = name;
	return v;
}

void Lump::add(Lump* lump)
{
	assert(type == SET);
	lump->parent = this;
	data.p->set.push_back(lump);
}

void Lump::addFirst(Lump* lump)
{
	assert(type == SET);
	lump->parent = this;
	data.p->set.insert(data.p->set.begin(), lump);
}

size_t Lump::count() const
{
	assert(type == SET);
	return static_cast<int>(data.p->set.size());
}




LumpSet::LumpSet(const std::string& className, ArrayType arrayType, bool isReference)
	: className(className)
	, arrayType(arrayType)
	, isReference(isReference)
{}

LumpSet::~LumpSet()
{
	for (SetType::reverse_iterator it = set.rbegin(); it != set.rend(); it++)
	{
		Lump* lump = *it;
		delete lump;
	}
}



