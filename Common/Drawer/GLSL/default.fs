"\n\
#version 120															\n\
uniform sampler2D	texture2d;											\n\
uniform float		args[16];											\n\
uniform float		params[16];											\n\
																		\n\
const int	A_TW	= 0;												\n\
const int	A_TH	= 1;												\n\
const int	A_PM	= 10;												\n\
																		\n\
vec4 getTexColor( vec2 c )												\n\
{																		\n\
	vec4	p = texture2D( texture2d, c );								\n\
																		\n\
	if ( args[A_PM] <= 0.0 ) {											\n\
		return	p;														\n\
	}																	\n\
	if ( p.a <= 0.0 ) {													\n\
		return	vec4( 0.0 );											\n\
	}																	\n\
																		\n\
	return	vec4( p.rgb / p.a, p.a );									\n\
}																		\n\
																		\n\
vec4 getBlendColor( vec4 p )											\n\
{																		\n\
	return	vec4( p.rgb * gl_TexCoord[1].x + mix( vec3( 1.0 ), p.rgb, gl_TexCoord[1].z ) * gl_Color.rgb * gl_TexCoord[1].y, p.a * gl_Color.a );		\n\
}																		\n\
																		\n\
void main()																\n\
{																		\n\
	float	fUnused = params[0];										\n\
																		\n\
	if ( args[A_TW] <= 0.0 ) {											\n\
		gl_FragColor = gl_Color;										\n\
		return;															\n\
	}																	\n\
																		\n\
	vec4	Pixel = getTexColor( gl_TexCoord[0].st );					\n\
	vec4	Blend = getBlendColor( Pixel );								\n\
																		\n\
	gl_FragColor = Blend;												\n\
}																		\n\
\n",
