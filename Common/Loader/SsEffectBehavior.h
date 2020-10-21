#ifndef __SSEFFECTBEHAVIOR__
#define __SSEFFECTBEHAVIOR__

#include "SsEffectElement.h"
#include "sstypes.h"
#include "ssvalue.h"

#if 1	/* Smart-Ptr */
#include <memory>
#else
#endif	/* Smart-Ptr */

namespace spritestudio6
{

class SsEffectElementBase;
class SsEffectRenderer;

class SsEffectBehavior
{
public:
#if 1	/* Smart-Ptr */
	std::vector<std::unique_ptr<SsEffectElementBase>> plist;
#else

	std::vector<SsEffectElementBase*> plist;
#endif	/* Smart-Ptr */

public:
    SsCell*							refCell;
	SsString						CellName;
	SsString						CellMapName;
	SsRenderBlendType::_enum		BlendType;

public:
	SsEffectBehavior() : refCell(0),BlendType(SsRenderBlendType::invalid) {}
#if 1	/* Smart-Ptr */
	virtual ~SsEffectBehavior();
#else
	virtual ~SsEffectBehavior(){}
#endif	/* Smart-Ptr */

	SPRITESTUDIO6SDK_SERIALIZE_BLOCK
	{
		SPRITESTUDIO6SDK_SSAR_DECLARE(CellName);
		SPRITESTUDIO6SDK_SSAR_DECLARE(CellMapName);
		SPRITESTUDIO6SDK_SSAR_DECLARE_ENUM( BlendType );

		if ( ar->getxml() )
			EffectElementLoader( ar );
	}

	SsEffectElementBase*	Factory(const char* name , libXML::XMLElement* e );

	void	EffectElementLoader(ISsXmlArchiver* ar);
	void	setup();
/*
	//エフェクトデータのセットアップ
	void	initalize( SsEffectRenderEmitter* emmiter){}
	void	updateEmmiter(SsEffectRenderEmitter* emmiter){}
	void	initializeParticle( SsEffectRenderEmitter* e , SsEffectRenderParticle* particle ){}
	void	updateParticle( SsEffectRenderEmitter* e , SsEffectRenderParticle* particle ){}
*/


};




}	// namespace spritestudio6

#endif
