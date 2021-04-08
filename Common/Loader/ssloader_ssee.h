﻿#ifndef __SSLOADER_SSEE__
#define __SSLOADER_SSEE__

#include "sstypes.h"
#include "ssarchiver.h"

#include "SsEffectBehavior.h"

#define SPRITESTUDIO6_SSEEVERSION "2.00.00"


class SimpleTree
{
public:
	SimpleTree *parent;
	SimpleTree *ctop;
	SimpleTree *prev;
	SimpleTree *next;

public:
	SimpleTree()
		: parent(0),ctop(0),prev(0),next(0)
	{}
	virtual ~SimpleTree()
	{
		destroy();
	}

	void	addChildEnd( SimpleTree* c ){ 
		if ( ctop == 0 )
		{
			ctop = c; 
		}else{
			ctop->addSiblingEnd( c );
		}
		c->parent = this;
	}
	void	addSiblingEnd( SimpleTree* c )
	{
		if ( next == 0 )
		{
			c->prev = this;
			next = c;
		}else{
			next->addSiblingEnd(c);
		}

		c->parent = this->parent;
	}

	void destroysub(SimpleTree *t)
	{
		if ( t->ctop )
		{
			destroysub(t->ctop);
		}
		if ( t->next )
		{
			destroysub(t->next);
		}

		t->ctop = 0;
		t->next = 0;
		t->prev = 0;
		delete t;
	}
	void destroy()
	{
		if ( this->ctop )
			destroysub(this->ctop);	
	}
};






class SsEffectNode : public SimpleTree
{
public:
	int						arrayIndex;
	int						parentIndex;
	SsEffectNodeType::_enum	type;
	bool					visible;

	SsEffectBehavior		behavior;

public:
	SsEffectNode() :
	  arrayIndex(0), parentIndex(0),	
		  type(SsEffectNodeType::invalid)
	{}
	~SsEffectNode(){}

	SSSERIALIZE_BLOCK
	{
		SSAR_DECLARE(arrayIndex);
		SSAR_DECLARE(parentIndex);		
		SSAR_DECLARE_ENUM(type);	
		SSAR_DECLARE(visible);	
		SSAR_STRUCT_DECLARE(behavior);	
	}


	SsEffectNodeType::_enum	GetType(){ return type; }

	SsEffectBehavior*	GetMyBehavior(){ return &behavior;}

};


class SsEffectModel
{
private:
	SsEffectNode* root;

public:
	std::vector<SsEffectNode*> nodeList;
	int			lockRandSeed; 	 // ランダムシード固定値
	bool    	isLockRandSeed;  // ランダムシードを固定するか否か
	int			fps;             //
	SsString	bgcolor;
	SsString	effectName;
	int			layoutScaleX;
	int			layoutScaleY;


public:
	SsEffectModel() : root(0)
	{}

	virtual ~SsEffectModel(){
		if (root)
		{
			delete root;
		}
		root = 0;
	}

    //アクセス
	SsEffectNode* GetRoot(){ return root;}


	//シリアライザ
	SSSERIALIZE_BLOCK
	{
		SSAR_DECLARE(lockRandSeed);
		SSAR_DECLARE(isLockRandSeed);
		SSAR_DECLARE(fps);
		SSAR_DECLARE(bgcolor);
		SSAR_DECLARE(layoutScaleX);
		if ( layoutScaleX == 0 ) layoutScaleX = 100;

		SSAR_DECLARE(layoutScaleY);
		if ( layoutScaleY == 0 ) layoutScaleY = 100;

		SSAR_DECLARE_LISTEX(nodeList,"node");

		//ツリーの構築
		if ( nodeList.size() > 0 )
		{
			root = nodeList[0];
			for ( size_t i = 1 ; i < nodeList.size() ; i++ )
			{
				int pi = nodeList[i]->parentIndex;
				if ( pi >= 0 )
				{
					nodeList[pi]->addChildEnd( nodeList[i] );
				}
			}
		}

	}

	void	EffectNodeLoader(ISsXmlArchiver* ar);

	const std::vector<SsEffectNode*>& getNodeList()
	{
		return nodeList;
	}


};


class SsEffectFile
{
public:
	SsString			version;
	SsEffectModel		effectData;  //親子構造＋各アトリビュート
	SsString			name;

	SsEffectFile(){}
	virtual ~SsEffectFile(){}

	SSSERIALIZE_BLOCK
	{
		SSAR_DECLARE_ATTRIBUTE(version);
		SSAR_DECLARE(name);
		SSAR_STRUCT_DECLARE( effectData );
		effectData.effectName = name;
	}

};


class SsProject;
//!sspjのローダークラスです。
class ssloader_ssee
{
public:
	ssloader_ssee(){}
	virtual ~ssloader_ssee(){}

	static SsEffectFile*	Load(const std::string& filename );
	static void	loadPostProcessing( SsEffectFile* file , SsProject* pj );
};





#endif
