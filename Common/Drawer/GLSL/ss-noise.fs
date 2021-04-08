"\n\
// Copyright 2019 Web Technology Corp.\n\
#version 120\n\
uniform sampler2D	texture2d;\n\
uniform float		args[16];\n\
uniform float		params[16];\n\
\n\
const int	A_TW	= 0;\n\
const int	A_TH	= 1;\n\
const int	A_PM	= 10;\n\
\n\
vec4 getTexColor( vec2 c )\n\
{\n\
	vec4	p = texture2D( texture2d, c );\n\
\n\
	if ( args[A_PM] <= 0.0 ) {\n\
		return	p;\n\
	}\n\
	if ( p.a <= 0.0 ) {\n\
		return	vec4( 0.0 );\n\
	}\n\
\n\
	return	vec4( p.rgb / p.a, p.a );\n\
}\n\
\n\
vec4 getBlendColor( vec4 p )\n\
{\n\
	return	vec4( p.rgb * gl_TexCoord[1].x + mix( vec3( 1.0 ), p.rgb, gl_TexCoord[1].z ) * gl_Color.rgb * gl_TexCoord[1].y, p.a * gl_Color.a );\n\
}\n\
\n\
float rand( vec2 p )\n\
{\n\
	return	fract( sin( dot( p, vec2( 12.9898, 78.233 ) ) ) * 43758.5453 );\n\
}\n\
\n\
void main()\n\
{\n\
	float	fPower = params[0];\n\
	float	fColor = params[1];\n\
	float	fPhase = params[2];\n\
\n\
	if ( args[A_TW] <= 0.0 ) {\n\
		gl_FragColor = gl_Color;\n\
		return;\n\
	}\n\
\n\
	vec2	Coord;\n\
	vec2	u;\n\
	vec4	Pixel;\n\
\n\
	Coord = gl_TexCoord[0].st;\n\
	u = vec2( floor( Coord.x * args[A_TW] ) / args[A_TW], floor( Coord.y * args[A_TH] ) / args[A_TH] );\n\
\n\
	Pixel = getTexColor( Coord );\n\
\n\
	vec2	t = u + vec2( fPhase );\n\
	vec3	c;\n\
\n\
	c.r = rand( t );\n\
	c.g = mix( c.r, rand( t * c.r ), fColor );\n\
	c.b = mix( c.g, rand( t * c.g ), fColor );\n\
\n\
	Pixel.rgb = clamp( mix( Pixel.rgb, c.rgb, fPower ), 0.0, 1.0 );\n\
\n\
	vec4	Blend = getBlendColor( Pixel );\n\
\n\
	gl_FragColor = Blend;\n\
}\n\
\n",
