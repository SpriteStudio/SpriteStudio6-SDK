"\n\
// ブラーのような効果\n\
// params[0]: -1.0 ~ 1.0 フォーカスのずれ度合いのようになる。詳細は後述\n\
\n\
#version 120\n\
uniform sampler2D	texture2d;\n\
uniform float		args[16];\n\
uniform float		params[16];\n\
\n\
const int	A_TW	= 0;\n\
const int	A_TH	= 1;\n\
\n\
void main()\n\
{\n\
	if (args[A_TW] == 0) {\n\
		gl_FragColor = gl_Color;\n\
		return;\n\
	}\n\
\n\
	// pix_* はUV座標系での１ピクセルサイズ\n\
	float pix_w = 1.0 / args[A_TW];\n\
	float pix_h = 1.0 / args[A_TH];\n\
\n\
	// param[0]は -1.0 ~ 1.0 であるため、最大でも隣のピクセルまでしか混ぜないため\n\
	// 適当な数字を掛けて離れたピクセルを参照できるようにしている。\n\
	float	defocused_x = params[0] * pix_w * 16;\n\
	float	defocused_y = params[0] * pix_h * 16;\n\
\n\
	vec2	tc;\n\
	tc.x = gl_TexCoord[0].x;\n\
	tc.y = gl_TexCoord[0].y;\n\
\n\
	vec4	in_color = gl_Color;\n\
\n\
	vec4 out_color = texture2D(texture2d, tc) * in_color;\n\
	out_color += texture2D(texture2d, vec2(tc.x + defocused_x, tc.y + defocused_y)) * in_color;\n\
	out_color += texture2D(texture2d, vec2(tc.x + defocused_x, tc.y)) * in_color;\n\
	out_color += texture2D(texture2d, vec2(tc.x , tc.y + defocused_y)) * in_color;\n\
	out_color += texture2D(texture2d, vec2(tc.x - defocused_x, tc.y - defocused_y)) * in_color;\n\
	out_color += texture2D(texture2d, vec2(tc.x + defocused_x, tc.y - defocused_y)) * in_color;\n\
	out_color += texture2D(texture2d, vec2(tc.x - defocused_x, tc.y + defocused_y)) * in_color;\n\
	out_color += texture2D(texture2d, vec2(tc.x - defocused_x, tc.y)) * in_color;\n\
	out_color += texture2D(texture2d, vec2(tc.x , tc.y - defocused_y)) * in_color;\n\
	out_color = out_color / 9;\n\
\n\
    gl_FragColor = out_color;\n\
 }\n\
\n",
