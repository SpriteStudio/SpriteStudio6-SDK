"\n\
// Copyright 2019 Web Technology Corp.\n\
#version 120\n\
uniform sampler2D	texture2d;\n\
uniform float		args[16];\n\
uniform float		params[16];\n\
\n\
const int	A_TW	= 0;\n\
const int	A_TH	= 1;\n\
const int	A_U1	= 2;\n\
const int	A_V1	= 3;\n\
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
float toOutlineValue( vec2 p, vec2 v, float fRatio )\n\
{\n\
	vec4	Pixel;\n\
	float	lo;\n\
	float	hi;\n\
	float	e = 1.0e-5;\n\
\n\
	lo = 0.0;\n\
\n\
	Pixel = getTexColor( p + vec2( -v.x, 0.0 ) );\n\
	lo += step( Pixel.a, fRatio );\n\
	Pixel = getTexColor( p + vec2( +v.x, 0.0 ) );\n\
	lo += step( Pixel.a, fRatio );\n\
	Pixel = getTexColor( p + vec2( 0.0, -v.y ) );\n\
	lo += step( Pixel.a, fRatio );\n\
	Pixel = getTexColor( p + vec2( 0.0, +v.y ) );\n\
	lo += step( Pixel.a, fRatio );\n\
\n\
	Pixel = getTexColor( p );\n\
\n\
	hi = step( fRatio + e, Pixel.a );\n\
\n\
	return	min( hi * lo, 1.0 );\n\
}\n\
\n\
void main()\n\
{\n\
	float	fThreshold = params[0];\n\
\n\
	if ( args[A_TW] <= 0.0 ) {\n\
		gl_FragColor = gl_Color;\n\
		return;\n\
	}\n\
\n\
	vec2	Coord;\n\
	float	fPixW;\n\
	float	fPixH;\n\
\n\
	Coord = gl_TexCoord[0].st;\n\
\n\
	fPixW = args[A_U1];\n\
	fPixH = args[A_V1];\n\
\n\
	float	v = toOutlineValue( Coord, vec2( fPixW, fPixH ), abs( fThreshold ) );\n\
\n\
	if ( v <= 0.0 ) {\n\
		discard;\n\
	}\n\
\n\
	vec4	Blend = getBlendColor( vec4( v ) );\n\
\n\
	gl_FragColor = Blend;\n\
}\n\
\n",
