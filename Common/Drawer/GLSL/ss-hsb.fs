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
vec4 toHSB( vec4 color )\n\
{\n\
	vec4	K = vec4( 0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0 );\n\
	vec4	p = mix( vec4( color.bg, K.wz ), vec4( color.gb, K.xy ), step( color.b, color.g ) );\n\
	vec4	q = mix( vec4( p.xyw, color.r ), vec4( color.r, p.yzx ), step( p.x, color.r ) );\n\
\n\
	float	d = q.x - min( q.w, q.y );\n\
	float	e = 1.0e-10;\n\
\n\
	return	vec4( abs( q.z + ( q.w - q.y ) / ( 6.0 * d + e ) ), d / ( q.x + e ), q.x, color.a );\n\
}\n\
\n\
vec4 toRGB( vec4 hsb )\n\
{\n\
	vec4	K = vec4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );\n\
	vec3	p = abs( fract( hsb.xxx + K.xyz ) * 6.0 - K.www );\n\
\n\
	return	vec4( hsb.z * mix( K.xxx, clamp( p - K.xxx, 0.0, 1.0 ), hsb.y ), hsb.w );\n\
}\n\
\n\
vec4 shiftHSB( vec4 hsb, float fRatioH, float fRatioS, float fRatioB )\n\
{\n\
	vec4	shift = hsb;\n\
\n\
	shift.x += fRatioH;\n\
	shift.y = clamp( hsb.y + fRatioS, 0.0, 1.0 );\n\
	shift.z = clamp( hsb.z + fRatioB, 0.0, 1.0 );\n\
\n\
	if ( shift.x < 0.0 ) {\n\
		shift.x += 1.0;\n\
	}\n\
	if ( shift.x > 1.0 ) {\n\
		shift.x -= 1.0;\n\
	}\n\
\n\
	return	shift;\n\
}\n\
\n\
vec4 adjustHSB( vec4 color, float fRatioH, float fRatioS, float fRatioB )\n\
{\n\
	vec4	hsb;\n\
\n\
	hsb = toHSB( color );\n\
\n\
	hsb = shiftHSB( hsb, fRatioH, fRatioS, fRatioB );\n\
\n\
	return	toRGB( hsb );\n\
}\n\
\n\
void main()\n\
{\n\
	float	fHue = params[0];\n\
	float	fSaturation = params[1];\n\
	float	fBrightness = params[2];\n\
\n\
	if ( args[A_TW] <= 0.0 ) {\n\
		gl_FragColor = adjustHSB( gl_Color, fHue, fSaturation, fBrightness );\n\
		return;\n\
	}\n\
\n\
	vec4	Pixel = getTexColor( gl_TexCoord[0].st );\n\
	vec4	Blend = getBlendColor( adjustHSB( Pixel, fHue, fSaturation, fBrightness ) );\n\
\n\
	gl_FragColor = Blend;\n\
}\n\
\n",
