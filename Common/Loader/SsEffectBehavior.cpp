#include "SsEffectBehavior.h"

#include <memory>
#include <utility>

namespace spritestudio6
{

SsEffectBehavior::~SsEffectBehavior()
{
	for( std::vector<std::unique_ptr<SsEffectElementBase>>::iterator itr = plist.begin();
			itr != plist.end(); itr++)
	{
		itr->reset();
	}
}

SsEffectElementBase*	SsEffectBehavior::Factory(const char* name , libXML::XMLElement* e )
{
	SsEffectElementBase * ret = 0;

	if ( strcmp( name , "Basic" ) == 0 ) ret = new ParticleElementBasic();
	else if ( strcmp( name , "OverWriteSeed" ) == 0 ) ret = new ParticleElementRndSeedChange();
	else if ( strcmp( name , "Delay" ) == 0 ) ret = new ParticleElementDelay();
	else if ( strcmp( name , "Gravity" ) == 0 ) ret = new ParticleElementGravity();
	else if ( strcmp( name , "init_position" ) == 0 ) ret = new ParticleElementPosition();
//	else if ( strcmp( name , "trans_position" ) == 0 ) ret = new ParticleElementTransPosition();
	else if ( strcmp( name , "init_rotation" ) == 0 ) ret = new ParticleElementRotation();
	else if ( strcmp( name , "trans_rotation" ) == 0 ) ret = new ParticleElementRotationTrans();
	else if ( strcmp( name , "trans_speed" ) == 0 ) ret = new ParticleElementTransSpeed();
	else if ( strcmp( name , "add_tangentiala" ) == 0 ) ret = new ParticleElementTangentialAcceleration();
	else if ( strcmp( name , "init_vertexcolor" ) == 0 ) ret = new ParticleElementInitColor();
	else if ( strcmp( name , "trans_vertexcolor" ) == 0 ) ret = new ParticleElementTransColor();
	else if ( strcmp( name , "trans_colorfade" ) == 0 ) ret = new ParticleElementAlphaFade();
	else if ( strcmp( name , "init_size" ) == 0 ) ret = new ParticleElementSize();
	else if ( strcmp( name , "trans_size" ) == 0 ) ret = new ParticleElementTransSize();
	else if ( strcmp( name , "add_pointgravity" ) == 0 ) ret = new ParticlePointGravity();
	else if ( strcmp( name , "TurnToDirection" ) == 0 ) ret = new ParticleTurnToDirectionEnabled();
	else if ( strcmp( name , "InfiniteEmit" ) == 0 ) ret = new ParticleInfiniteEmitEnabled();

	if ( ret )
	{
		SsXmlIArchiver _ar(e);
		ret->__Serialize( &_ar );
	}

	return ret;
}


void	SsEffectBehavior::EffectElementLoader(ISsXmlArchiver* ar)
{
	SsXmlIArchiver list_ar( ar , "list" );

	libXML::XMLElement* e = list_ar.getxml()->FirstChildElement();

	while( e )
	{
		const char* value = e->Attribute( "name" );

		SsEffectElementBase* v = Factory( value , e );
		if ( v )
		{
			plist.push_back( std::move( std::unique_ptr<SsEffectElementBase>(v) ) );
		}
		e = e->NextSiblingElement();
	}

}

}	// namespace spritestudio6

