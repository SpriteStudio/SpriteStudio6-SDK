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
void main()\n\
{\n\
	float	fDistance = params[0];\n\
	float	fDirection = params[1];\n\
	float	fPower = params[2];\n\
\n\
	if ( args[A_TW] <= 0.0 ) {\n\
		gl_FragColor = gl_Color;\n\
		return;\n\
	}\n\
\n\
	vec2	Coord;\n\
	float	fPixW;\n\
	float	fPixH;\n\
	int		iCount;\n\
	vec2	Vel;\n\
	vec4	Pixel;\n\
	float	PI = 3.14159265358979;\n\
\n\
	Coord = gl_TexCoord[0].st;\n\
\n\
	fPixW = args[A_U1];\n\
	fPixH = args[A_V1];\n\
\n\
	iCount = int( floor( abs( fDistance ) * 96.0 ) );\n\
\n\
	Vel = vec2( sin( fDirection * PI ) * fPixW, cos( fDirection * PI ) * fPixH ) * sign( fDistance );\n\
\n\
	Coord += Vel * iCount;\n\
\n\
	Coord -= Vel;\n\
	Pixel = getTexColor( Coord );\n\
\n\
	for ( int i = 1; i < iCount; i++ ) {\n\
		Coord -= Vel;\n\
		Pixel = mix( getTexColor( Coord ), Pixel, 0.96 * abs( fPower ) );\n\
	}\n\
\n\
	vec4	Blend = getBlendColor( Pixel );\n\
\n\
	gl_FragColor = Blend;\n\
}\n\
\n",
