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
	float	fPower = params[0];\n\
	float	fDecay = params[1];\n\
	float	fColor = params[2];\n\
\n\
	if ( args[A_TW] <= 0.0 ) {\n\
		gl_FragColor = gl_Color;\n\
		return;\n\
	}\n\
\n\
	float	e = 1.0e-10;\n\
	float	p = abs( fPower );\n\
	float	d = abs( fDecay );\n\
	vec2	uv1 = vec2( args[A_U1] + e, args[A_V1] + e );\n\
	vec2	c = vec2( args[A_CU], args[A_CV] ) / uv1;\n\
	vec2	rb = vec2( args[A_RU], args[A_BV] ) / uv1;\n\
	float	l = min( abs( rb.x - c.x ), abs( rb.y - c.y ) ) + e;\n\
	vec2	t = gl_TexCoord[0].st / uv1;\n\
	vec2	v = c - t;\n\
\n\
	float	fR = max( p / ( length( v / l ) + e ) - p, 0.0 );\n\
	float	fL = max( p - length( v / l ), 0.0 );\n\
\n\
	fR = clamp( mix( fR, fL, d ), 0.0, 1.0 );\n\
\n\
	vec4	Pixel = vec4( mix( vec3( 1.0 ), getTexColor( gl_TexCoord[0].st ).rgb, fColor ), fR );\n\
	vec4	Blend = getBlendColor( Pixel );\n\
\n\
	gl_FragColor = Blend;\n\
}\n\
\n",
