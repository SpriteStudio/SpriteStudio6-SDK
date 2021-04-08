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
const int	A_LU	= 4;\n\
const int	A_TV	= 5;\n\
const int	A_CU	= 6;\n\
const int	A_CV	= 7;\n\
const int	A_RU	= 8;\n\
const int	A_BV	= 9;\n\
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
	float	fPhase = params[0];\n\
	float	fDirection = params[1];\n\
\n\
	if ( args[A_TW] <= 0.0 ) {\n\
		gl_FragColor = gl_Color;\n\
		return;\n\
	}\n\
\n\
	float	PI = 3.14159265358979;\n\
	float	e = 1.0e-10;\n\
	vec2	uv1 = vec2( args[A_U1] + e, args[A_V1] + e );\n\
	vec2	tx = gl_TexCoord[0].st / uv1;\n\
	vec2	c = vec2( args[A_CU], args[A_CV] ) / uv1;\n\
	vec2	lt = vec2( args[A_LU], args[A_TV] ) / uv1;\n\
	vec2	rb = vec2( args[A_RU], args[A_BV] ) / uv1;\n\
	vec2	d = rb - lt;\n\
	vec2	v = tx - c;\n\
	float	r = min( abs( rb.x - c.x ), abs( rb.y - c.y ) ) + e;\n\
	float	dis = length( v );\n\
\n\
	if ( dis > r ) {\n\
		discard;\n\
	}\n\
\n\
	vec2	nv = normalize( v );\n\
	float	uu = 1.0 - dis / r;\n\
	float	vv = ( atan( nv.y, nv.x ) / PI + 1.0 ) * 0.5 + fPhase;\n\
\n\
	if ( vv < 0.0 ) {\n\
		vv += 1.0;\n\
	}\n\
	if ( vv > 1.0 ) {\n\
		vv -= 1.0;\n\
	}\n\
\n\
	if ( fDirection <= 0.0 ) {\n\
		vv = 1.0 - vv;\n\
	}\n\
\n\
	vec2	st = d * vec2( uu, vv ) * uv1;\n\
\n\
	vec4	Pixel = getTexColor( vec2( args[A_LU], args[A_TV] ) + st );\n\
	vec4	Blend = getBlendColor( Pixel );\n\
\n\
	gl_FragColor = Blend;\n\
}\n\
\n",
