﻿#include <stdio.h>
#include <cstdlib>


#include "ssOpenGLSetting.h"


#include <map>
#include <memory>

#include "../Helper/OpenGL/SSTextureGL.h"

#include "../Animator/ssplayer_animedecode.h"
#include "../Animator/ssplayer_matrix.h"

#include "ssplayer_render_gl.h"
#include "ssplayer_shader_gl.h"

#include "ssplayer_cellmap.h"
#include "ssplayer_mesh.h"

#ifdef _WIN32
	#define SS_FASTCALL __fastcall
#else
	#define SS_FASTCALL
#endif

#define SPRITESTUDIO6SDK_PROGRAMABLE_SHADER_ON (1)

namespace spritestudio6
{


//ISsRenderer*	SsCurrentRenderer::m_currentrender = 0;

static const char* glshader_sprite_vs =
#include "GLSL/sprite.vs"


static const char* glshader_sprite_fs =
#include "GLSL/sprite.fs"


static const char* glshader_sprite_fs_pot =
#include "GLSL/sprite_pot.fs"


class SSOpenGLProgramObject;

struct ShaderSetting
{
	const char*	name;
	const char*	vs;
	const char*	fs;
};

//MEMO: 現在デストラクト時に（定義リソースが）自動解放されることを期待しています。
//      ※明示的な解放タイミングが見当たらないため。
static std::map<SsString, std::unique_ptr<SSOpenGLProgramObject>>	s_DefaultShaderMap;

static const ShaderSetting glshader_default[] =
{
	{
		"system::default",
#include "GLSL/default.vs"
#include "GLSL/default.fs"
	},
	{
		"ss-blur",
#include "GLSL/ss-common.vs"
#include "GLSL/ss-blur.fs"
	},
	{
		"ss-bmask",
#include "GLSL/ss-common.vs"
#include "GLSL/ss-bmask.fs"
	},
	{
		"ss-circle",
#include "GLSL/ss-common.vs"
#include "GLSL/ss-circle.fs"
	},
	{
		"ss-hsb",
#include "GLSL/ss-common.vs"
#include "GLSL/ss-hsb.fs"
	},
	{
		"ss-move",
#include "GLSL/ss-common.vs"
#include "GLSL/ss-move.fs"
	},
	{
		"ss-noise",
#include "GLSL/ss-common.vs"
#include "GLSL/ss-noise.fs"
	},
	{
		"ss-outline",
#include "GLSL/ss-common.vs"
#include "GLSL/ss-outline.fs"
	},
	{
		"ss-pix",
#include "GLSL/ss-common.vs"
#include "GLSL/ss-pix.fs"
	},
	{
		"ss-scatter",
#include "GLSL/ss-common.vs"
#include "GLSL/ss-scatter.fs"
	},
	{
		"ss-sepia",
#include "GLSL/ss-common.vs"
#include "GLSL/ss-sepia.fs"
	},
	{
		"ss-spot",
#include "GLSL/ss-common.vs"
#include "GLSL/ss-spot.fs"
	},
	{
		"ss-step",
#include "GLSL/ss-common.vs"
#include "GLSL/ss-step.fs"
	},
	{
		"ss-wave",
#include "GLSL/ss-common.vs"
#include "GLSL/ss-wave.fs"
	},
	{
		NULL,
		NULL,
		NULL,
	},
};


//bool SsRender::m_isInit = false;


enum{
	PG_SHADER_NPOT,
	PG_SHADER_POT,
};




//6.2対応
//パーツカラー、ミックス、頂点
/// カラー値を byte(0~255) -> float(0.0~1.0) に変換する。
inline float SS_FASTCALL floatFromByte_(u8 color8)
{
	return static_cast<float>(color8) / 255.f;
}

/// RGBA の各値を byte(0~255) -> float(0.0~1.0) に変換し、配列 dest の[0,1,2,3] に設定する。
inline void SS_FASTCALL rgbaByteToFloat_(float* dest, const SsColorBlendValue& src)
{
	const SsColor* srcColor = &src.rgba;

	dest[0] = floatFromByte_((u8)srcColor->r);
	dest[1] = floatFromByte_((u8)srcColor->g);
	dest[2] = floatFromByte_((u8)srcColor->b);
	dest[3] = floatFromByte_((u8)srcColor->a);
}

struct ShaderVArg;
static int			s_iVArgCount	= 0;
static ShaderVArg*	s_pVArg			= NULL;

/// // RGB=100%テクスチャ、A=テクスチャｘ頂点カラーの設定にする。
static void SS_FASTCALL setupTextureCombinerTo_NoBlendRGB_MultiplyAlpha_()
{

	// カラーは１００％テクスチャ
	glTexEnvi(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_COMBINE);
	glTexEnvi(GL_TEXTURE_ENV, GL_COMBINE_RGB, GL_REPLACE);
	glTexEnvi(GL_TEXTURE_ENV, GL_SOURCE0_RGB, GL_TEXTURE0);
	// αだけ合成
	glTexEnvi(GL_TEXTURE_ENV, GL_COMBINE_ALPHA, GL_MODULATE);
	glTexEnvi(GL_TEXTURE_ENV, GL_SOURCE0_ALPHA, GL_TEXTURE0);
	glTexEnvi(GL_TEXTURE_ENV, GL_SOURCE1_ALPHA, GL_PRIMARY_COLOR);
	glTexEnvi(GL_TEXTURE_ENV, GL_OPERAND0_ALPHA, GL_SRC_ALPHA);
	glTexEnvi(GL_TEXTURE_ENV, GL_OPERAND1_ALPHA, GL_SRC_ALPHA);
}


/**
ブレンドタイプに応じたテクスチャコンバイナの設定を行う (パーツカラー用)

rateOrAlpha		ミックス時のみ参照される。単色では Rate に、頂点単位では Alpha になる。
target			ミックス時のみ参照される。

参考：http://www.opengl.org/sdk/docs/man/xhtml/glTexEnv.xml
*/
static void SS_FASTCALL setupSimpleTextureCombiner_for_PartsColor_(SsBlendType::_enum type, float rateOrAlpha, SsColorBlendTarget::_enum target)
{
	//static const float oneColor[4] = {1.f,1.f,1.f,1.f};
	float constColor[4] = { 0.5f,0.5f,0.5f,rateOrAlpha };
	static const GLuint funcs[] = { GL_INTERPOLATE, GL_MODULATE, GL_ADD, GL_SUBTRACT };
	GLuint func = funcs[(int)type];
	GLuint srcRGB = GL_TEXTURE0;
	GLuint dstRGB = GL_PRIMARY_COLOR;

	// true:  頂点αをブレンドする。
	// false: constColor のαをブレンドする。
	bool combineAlpha = true;

	switch (static_cast<int>(type))
	{
	case SsBlendType::mix:
	case SsBlendType::mul:
	case SsBlendType::add:
	case SsBlendType::sub:
		glTexEnvi(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_COMBINE);
		// rgb
		glTexEnvi(GL_TEXTURE_ENV, GL_COMBINE_RGB, func);

		// mix の場合、特殊
		if (type == SsBlendType::mix)
		{
			if (target == SsColorBlendTarget::whole)
			{
				// 全体なら、const 値で補間する
				glTexEnvi(GL_TEXTURE_ENV, GL_SOURCE2_RGB, GL_CONSTANT);
				glTexEnvfv(GL_TEXTURE_ENV, GL_TEXTURE_ENV_COLOR, constColor);
			}
			else
			{
				// 頂点カラーのアルファをテクスチャに対する頂点カラーの割合にする。
				glTexEnvi(GL_TEXTURE_ENV, GL_SOURCE2_RGB, GL_PRIMARY_COLOR);

				combineAlpha = false;
			}
			// 強度なので 1 に近付くほど頂点カラーが濃くなるよう SOURCE0 を頂点カラーにしておく。
			std::swap(srcRGB, dstRGB);
		}
		glTexEnvi(GL_TEXTURE_ENV, GL_SOURCE0_RGB, srcRGB);
		glTexEnvi(GL_TEXTURE_ENV, GL_SOURCE1_RGB, dstRGB);
		glTexEnvi(GL_TEXTURE_ENV, GL_OPERAND0_RGB, GL_SRC_COLOR);
		glTexEnvi(GL_TEXTURE_ENV, GL_OPERAND1_RGB, GL_SRC_COLOR);
		break;
	case SsBlendType::screen:
	case SsBlendType::exclusion:
	case SsBlendType::invert:
	default:
		glTexEnvi(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_COMBINE);
		glTexEnvi(GL_TEXTURE_ENV, GL_SOURCE1_RGB, GL_TEXTURE0);
		glTexEnvi(GL_TEXTURE_ENV, GL_SOURCE2_RGB, GL_CONSTANT);
		glTexEnvi(GL_TEXTURE_ENV, GL_COMBINE_RGB, GL_MODULATE);
		break;
	}

	if (combineAlpha)
	{
		// alpha は常に掛け算
		glTexEnvi(GL_TEXTURE_ENV, GL_COMBINE_ALPHA, GL_MODULATE);
		glTexEnvi(GL_TEXTURE_ENV, GL_SOURCE0_ALPHA, GL_TEXTURE0);
		glTexEnvi(GL_TEXTURE_ENV, GL_SOURCE1_ALPHA, GL_PRIMARY_COLOR);
		glTexEnvi(GL_TEXTURE_ENV, GL_OPERAND0_ALPHA, GL_SRC_ALPHA);
		glTexEnvi(GL_TEXTURE_ENV, GL_OPERAND1_ALPHA, GL_SRC_ALPHA);
	}
	else
	{
#if 1
		// 浮いた const 値を頂点αの代わりにブレンドする。v6.2.0+ 2018/06/21 endo
		glTexEnvi(GL_TEXTURE_ENV, GL_COMBINE_ALPHA, GL_MODULATE);
		glTexEnvi(GL_TEXTURE_ENV, GL_SOURCE0_ALPHA, GL_TEXTURE0);
		glTexEnvi(GL_TEXTURE_ENV, GL_SOURCE1_ALPHA, GL_CONSTANT);
		glTexEnvfv(GL_TEXTURE_ENV, GL_TEXTURE_ENV_COLOR, constColor);
		glTexEnvi(GL_TEXTURE_ENV, GL_OPERAND0_ALPHA, GL_SRC_ALPHA);
		glTexEnvi(GL_TEXTURE_ENV, GL_OPERAND1_ALPHA, GL_SRC_ALPHA);
#else
		// ミックス＋頂点単位の場合αブレンドはできない。
		// αはテクスチャを100%使えれば最高だが、そうはいかない。
		glTexEnvi(GL_TEXTURE_ENV, GL_COMBINE_ALPHA, GL_REPLACE);
		glTexEnvi(GL_TEXTURE_ENV, GL_SOURCE0_ALPHA, GL_TEXTURE0);
		glTexEnvi(GL_TEXTURE_ENV, GL_OPERAND0_ALPHA, GL_SRC_ALPHA);
#endif
	}
}

static SSOpenGLProgramObject* createProgramObject( const SsString& name, const SsString& vs, const SsString& fs )
{
	SSOpenGLVertexShader*	pVs = new SSOpenGLVertexShader( name, vs );
	SSOpenGLFragmentShader*	pFs = new SSOpenGLFragmentShader( name, fs );
	SSOpenGLProgramObject*	pPo = new SSOpenGLProgramObject();

	pPo->Attach( pVs );
	pPo->Attach( pFs );

	if ( pPo->Link() != 0 ) {
		if ( pVs )
			delete pVs;
		if ( pVs )
			delete pFs;
		if ( pPo )
			delete pPo;

		pVs = nullptr;
		pFs = nullptr;
		pPo = nullptr;
	}

	return	pPo;
}

void SsRenderGL::clearShaderCache()
{
	if ( s_pVArg ) free( s_pVArg );
	s_iVArgCount = 0;
	s_pVArg = NULL;
}

void	SsRenderGL::initialize()
{
//	if ( m_isInit ) return ;
	SSOpenGLShaderMan::Create();
	SSOpenGLVertexShader*	vs = new SSOpenGLVertexShader( "basic_vs" , glshader_sprite_vs );
	SSOpenGLFragmentShader*	fs1 = new SSOpenGLFragmentShader( "normal_fs" , glshader_sprite_fs );
	SSOpenGLFragmentShader*	fs2 = new SSOpenGLFragmentShader( "pot_fs" ,glshader_sprite_fs_pot );

	SSOpenGLProgramObject* pgo1 = new SSOpenGLProgramObject();
	SSOpenGLProgramObject* pgo2 = new SSOpenGLProgramObject();

	pgo1->Attach( vs );
	pgo1->Attach( fs1 );
	pgo1->Link();
	SSOpenGLShaderMan::PushPgObject( pgo1 );

	pgo2->Attach( vs );
	pgo2->Attach( fs2 );
	pgo2->Link();
	SSOpenGLShaderMan::PushPgObject( pgo2 );

	s_DefaultShaderMap.clear();
	for ( int i = 0; glshader_default[i].name != nullptr; i++ ) {
		s_DefaultShaderMap[glshader_default[i].name].reset( createProgramObject( glshader_default[i].name, glshader_default[i].vs, glshader_default[i].fs ) );
	}

//	m_isInit = true;

}

void	SsRenderGL::renderSetup(SsAnimeDecoder* state)
{
	glDisableClientState( GL_COLOR_ARRAY );
	glTexEnvi(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);

	glEnable(GL_TEXTURE_2D);
	glBindTexture(GL_TEXTURE_2D, 0 );

	glEnable(GL_BLEND);
	glDisable(GL_DEPTH_TEST);
	glEnable(GL_ALPHA_TEST);
	glAlphaFunc(GL_GREATER, 0.0);

	glBlendEquation( GL_FUNC_ADD );

/*
	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
	glEnable(GL_BLEND);
	glEnable(GL_TEXTURE_2D);

	glDisable(GL_DEPTH_TEST);
	glEnable(GL_ALPHA_TEST);
	glAlphaFunc(GL_GREATER, 0.0);
*/

	glMatrixMode(GL_MODELVIEW);
	glLoadIdentity();

}


void	SsRenderGL::SetAlphaBlendMode(SsBlendType::_enum type)
{
	glBlendEquation( GL_FUNC_ADD );

	switch ( static_cast<int>(type) )
	{
	case SsBlendType::mix:				//< 0 ブレンド（ミックス）
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
		break;
	case SsBlendType::mul:				//< 1 乗算
		glBlendFunc(GL_ZERO, GL_SRC_COLOR);
		break;
	case SsBlendType::add:				//< 2 加算
		glBlendFunc(GL_SRC_ALPHA, GL_ONE);
		break;
	case SsBlendType::sub:				//< 3 減算
		// TODO SrcAlpha を透明度として使えない
		glBlendEquation( GL_FUNC_REVERSE_SUBTRACT );

#if USE_GLEW
		glBlendFuncSeparateEXT( GL_SRC_ALPHA, GL_ONE, GL_ZERO, GL_DST_ALPHA );
#else
		glBlendFuncSeparate(GL_SRC_ALPHA, GL_ONE, GL_ZERO, GL_DST_ALPHA);
#endif
		break;
	case SsBlendType::mulalpha: 		//< 4 α乗算
		glBlendFunc(GL_DST_COLOR, GL_ONE_MINUS_SRC_ALPHA);
		break;
	case SsBlendType::screen: 			//< 5 スクリーン
		glBlendFunc(GL_ONE_MINUS_DST_COLOR, GL_ONE);
		break;
	case SsBlendType::exclusion:		//< 6 除外
		glBlendFunc(GL_ONE_MINUS_DST_COLOR, GL_ONE_MINUS_SRC_COLOR);
		break;
	case SsBlendType::invert: 			//< 7 反転
		glBlendFunc(GL_ONE_MINUS_DST_COLOR, GL_ZERO);
		break;
	}
}

void	SsRenderGL::renderSpriteSimple( float matrix[16], 
									int width, int height, 
									SsVector2& pivot , 
									SsVector2 uv1, SsVector2 uv2, 
									const SsFColor& color )
{
	glPushMatrix();

	// update で計算しておいた行列をロード
	glLoadMatrixf(matrix);

 	float w = width / 2.0f ;
	float h = height / 2.0f ;

	glColor4f(color.r, color.g, color.b, color.a);


	float vtx[8];
	vtx[0] =  -w - pivot.x; 
	vtx[1] =   h - pivot.y;

	vtx[2] =  -w - pivot.x; 
	vtx[3] =  -h - pivot.y;

	vtx[4] =   w - pivot.x; 
	vtx[5] =  -h - pivot.y;

	vtx[6] =   w - pivot.x; 
	vtx[7] =   h - pivot.y;
	glVertexPointer(2, GL_FLOAT, 0, vtx);

	float tuv[8];
	tuv[0] = uv1.x; tuv[1] = uv1.y;
	tuv[2] = uv1.x; tuv[3] = uv2.y;
	tuv[4] = uv2.x; tuv[5] = uv2.y;
	tuv[6] = uv2.x; tuv[7] = uv1.y;
	glTexCoordPointer(2, GL_FLOAT, 0, tuv);
	
	glEnableClientState(GL_VERTEX_ARRAY);
	glEnableClientState(GL_TEXTURE_COORD_ARRAY);
	glDrawArrays(GL_QUADS, 0, 4);
	glDisableClientState(GL_TEXTURE_COORD_ARRAY);
	glDisableClientState(GL_VERTEX_ARRAY);


	glPopMatrix();

}

void	SsRenderGL::SetTexture( SsCellValue* cellvalue )
{
	int		gl_target = GL_TEXTURE_2D;
	bool texture_is_pow2 = true;

	if ( cellvalue == 0 ){
		return ;
	}
	SsCell * cell = cellvalue->cell;
	if ( cell == 0 ){
		return ;
	}

	ISSTexture*	texture = cellvalue->texture;
	if ( texture == 0 ){
		return ;
	}

	SsPoint2 texturePixelSize;
	texturePixelSize.x = cellvalue->texture->getWidth();
	texturePixelSize.y = cellvalue->texture->getHeight();

	if (texture)
	{
		// テクスチャのサイズが2のべき乗かチェック
		if ( texture->isPow2() )
		{
			// 2のべき乗
			texture_is_pow2 = true;
			gl_target = GL_TEXTURE_2D;
		}
		else
		{
			// 2のべき乗ではない:NPOTテクスチャ
			texture_is_pow2 = false;
#if USE_GLEW
			gl_target = GL_TEXTURE_RECTANGLE_ARB;
#else
			gl_target = GL_TEXTURE_RECTANGLE;
#endif
		}


		glEnable(gl_target);

		SSTextureGL* tex_gl = (SSTextureGL*)texture;
		glBindTexture(gl_target, tex_gl->tex);

	}

}

void	SsRenderGL::clearMask()
{

	glClear(  GL_STENCIL_BUFFER_BIT );

	enableMask(false);
}

void	SsRenderGL::enableMask(bool flag)
{

	if (flag)
	{
		glEnable(GL_STENCIL_TEST);
	}else{
		glDisable(GL_STENCIL_TEST);
		glColorMask(GL_TRUE, GL_TRUE, GL_TRUE, GL_TRUE);
	}
}

void	SsRenderGL::execMask(SsPartState* state)
{

	glEnable(GL_STENCIL_TEST);
	if (state->partType == SsPartType::mask)
	//if(0)
	{

		glColorMask(GL_FALSE, GL_FALSE, GL_FALSE, GL_FALSE);

		if (!(state->maskInfluence )) { //マスクが有効では無い＝重ね合わせる

			glStencilFunc(GL_ALWAYS, 1, ~0);  //常に通過
			glStencilOp(GL_KEEP, GL_KEEP, GL_REPLACE);
			//描画部分を1へ
		}
		else {
			glStencilFunc(GL_ALWAYS, 1, ~0);  //常に通過
			glStencilOp(GL_KEEP, GL_KEEP, GL_INCR);
		}


		glEnable(GL_ALPHA_TEST);

		//この設定だと
		//1.0fでは必ず抜けないため非表示フラグなし（＝1.0f)のときの挙動は考えたほうがいい

		//不透明度からマスク閾値へ変更
		float mask_alpha = (float)(255 - state->masklimen ) / 255.0f;
		glAlphaFunc(GL_GREATER, mask_alpha);
		;
		state->alpha = 1.0f;
	}
	else {

		if ((state->maskInfluence )) //パーツに対してのマスクが有効か否か
		{
			glColorMask(GL_TRUE, GL_TRUE, GL_TRUE, GL_TRUE);
			glStencilFunc(GL_NOTEQUAL, 0x1, 0x1);  //1と等しい
			glStencilOp(GL_KEEP, GL_KEEP, GL_KEEP);
		}
		else {
			glColorMask(GL_TRUE, GL_TRUE, GL_TRUE, GL_TRUE);
			glDisable(GL_STENCIL_TEST);
		}

		// 常に無効
		glDisable(GL_ALPHA_TEST);

	}

}

/**
中央頂点(index=4)のRGBA%値の計算

[in/out] colors, rates の [0~3] の平均値を [4] に入れる。
*/
inline void SS_FASTCALL calcCenterVertexColor(float* colors, float* rates, float* vertexID)
{
	float a, r, g, b, rate;
	a = r = g = b = rate = 0;
	for (int i = 0; i < 4; i++)
	{
		int idx = i * 4;
		a += colors[idx++];
		r += colors[idx++];
		g += colors[idx++];
		b += colors[idx++];
		rate += rates[i];
	}

	//きれいな頂点変形への対応
	vertexID[4 * 2] = 4;
	vertexID[4 * 2 + 1] = 4;

	int idx = 4 * 4;
	colors[idx++] = a / 4.0f;
	colors[idx++] = r / 4.0f;
	colors[idx++] = g / 4.0f;
	colors[idx++] = b / 4.0f;
	rates[4] = rate / 4.0f;
}

struct ShaderVArg
{
	float	fSrcRatio;
	float	fDstRatio;
	float	fDstSrcRatio;
	float	fReserved;
};

struct ShaderFArg
{
	float	fTexW;
	float	fTexH;
	float	fPixTX;
	float	fPixTY;
	float	fCoordLU;
	float	fCoordTV;
	float	fCoordCU;
	float	fCoordCV;
	float	fCoordRU;
	float	fCoordBV;
	float	fPMA;
	float	fReserved1;
	float	fReserved2;
	float	fReserved3;
	float	fReserved4;
	float	fReserved5;
};

void	SsRenderGL::renderMesh(SsMeshPart* mesh , float alpha )
{
	if (mesh == 0)return;
	
	glPushMatrix();


	if (alpha == 0.0f)
	{
		return;
	}

	glMatrixMode(GL_MODELVIEW);
	if (mesh->isBind)
	{
		//glLoadMatrixf(mesh->myPartState->matrix);
		glLoadIdentity();
	}
	else {
		glLoadMatrixf(mesh->myPartState->matrixLocal);
	}


	bool texture_is_pow2 = true;
	int	gl_target = GL_TEXTURE_2D;
	if (mesh->targetTexture)
	{

		// テクスチャのサイズが2のべき乗かチェック
		if (mesh->targetTexture->isPow2())
		{
			// 2のべき乗
			texture_is_pow2 = true;
			gl_target = GL_TEXTURE_2D;
		}
		else
		{
			// 2のべき乗ではない:NPOTテクスチャ
			texture_is_pow2 = false;
#if USE_GLEW
			gl_target = GL_TEXTURE_RECTANGLE_ARB;
#else
			gl_target = GL_TEXTURE_RECTANGLE;
#endif
		}


		glEnable(gl_target);

		SSTextureGL* tex_gl = (SSTextureGL*)mesh->targetTexture;
		glBindTexture(gl_target, tex_gl->tex);

	}

	SsPartState* state = mesh->myPartState;

	// パーツカラーの指定
	std::vector<float>& colorsRaw = *(mesh->colors.get());
	if (state->is_parts_color)
	{

		// パーツカラーがある時だけブレンド計算する
		float setcol[4];
		if (state->partsColorValue.target == SsColorBlendTarget::whole)
		{
			// 単色
			const SsColorBlendValue& cbv = state->partsColorValue.color;
			setupSimpleTextureCombiner_for_PartsColor_(state->partsColorValue.blendType, cbv.rate, state->partsColorValue.target);
			rgbaByteToFloat_(setcol, cbv);
		}
		else
		{
			//4頂点はとりあえず左上のRGBAと Rate 
			const SsColorBlendValue& cbv = state->partsColorValue.colors[0];
			// コンバイナの設定は常に単色として行う。
			setupSimpleTextureCombiner_for_PartsColor_(state->partsColorValue.blendType, cbv.rate, SsColorBlendTarget::whole);
			rgbaByteToFloat_(setcol, cbv);
			if (state->partsColorValue.blendType == SsBlendType::mix)
			{
				setcol[3] = 1; // ミックス-頂点単位では A に Rate が入っておりアルファは無効なので1にしておく。
			}
		}
		for (size_t i = 0; i < mesh->ver_size; i++)
		{
			colorsRaw[i * 4 + 0] = setcol[0];
			colorsRaw[i * 4 + 1] = setcol[1];
			colorsRaw[i * 4 + 2] = setcol[2];
			colorsRaw[i * 4 + 3] = setcol[3] *alpha; // 不透明度を適用する。
		}
	}
	else {
		//WaitColor;
		//ウェイトカラーの合成色を頂点カラーとして使用（パーセント円の流用
		for (size_t i = 0; i < mesh->ver_size; i++)
		{
			colorsRaw[i * 4 + 0] = 1.0f;
			colorsRaw[i * 4 + 1] = 1.0f;
			colorsRaw[i * 4 + 2] = 1.0f;
			colorsRaw[i * 4 + 3] = alpha;
		}

		glTexEnvi(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_COMBINE);
		glTexEnvi(GL_TEXTURE_ENV, GL_COMBINE_RGB, GL_REPLACE);
		glTexEnvi(GL_TEXTURE_ENV, GL_SOURCE0_RGB, GL_TEXTURE0);
		// αだけ合成
		glTexEnvi(GL_TEXTURE_ENV, GL_COMBINE_ALPHA, GL_MODULATE);
		glTexEnvi(GL_TEXTURE_ENV, GL_SOURCE0_ALPHA, GL_TEXTURE0);
		glTexEnvi(GL_TEXTURE_ENV, GL_SOURCE1_ALPHA, GL_PRIMARY_COLOR);
		glTexEnvi(GL_TEXTURE_ENV, GL_OPERAND0_ALPHA, GL_SRC_ALPHA);
		glTexEnvi(GL_TEXTURE_ENV, GL_OPERAND1_ALPHA, GL_SRC_ALPHA);

	}

	glEnableClientState(GL_TEXTURE_COORD_ARRAY);
	glEnableClientState(GL_COLOR_ARRAY);
	glEnableClientState(GL_VERTEX_ARRAY);

	// UV 配列を指定する
	glTexCoordPointer(2, GL_FLOAT, 0, (GLvoid *)((mesh->uvs.get())->data()));

	// 頂点色を指定する
	glColorPointer(4, GL_FLOAT, 0, (GLvoid *)((mesh->colors.get())->data()));

	// 頂点バッファの設定
	glVertexPointer(3, GL_FLOAT, 0, (GLvoid *)((mesh->draw_vertices.get())->data()));

	glDrawElements(GL_TRIANGLES, mesh->tri_size * 3, GL_UNSIGNED_SHORT, (mesh->indices.get())->data());

	glPopMatrix();

	if (texture_is_pow2 == false)
	{
		glDisable(gl_target);
	}
	glDisable(GL_TEXTURE_2D);

	//ブレンドモード　減算時の設定を戻す
	glBlendEquation(GL_FUNC_ADD);

}


void	SsRenderGL::renderPart( SsPartState* state )
{
	bool texture_is_pow2 = true;
	bool color_blend_v4 = false;
	float fTexW = 16.0f;
	float fTexH = 16.0f;
	float fPixTX = 1.0f;
	float fPixTY = 1.0f;
	float fCoordLU = 0.0f;
	float fCoordTV = 0.0f;
	float fCoordCU = 0.0f;
	float fCoordCV = 0.0f;
	float fCoordRU = 0.0f;
	float fCoordBV = 0.0f;
	float vertexID[10];
//	bool colorBlendEnabled = false;
	bool partsColorEnabled = false;
	bool alphaBlendMix = false;

	int		gl_target = GL_TEXTURE_2D;
	float	rates[5];

	SSOpenGLProgramObject*	pPrgObject = nullptr;

	if ( state->hide ) return ; //非表示なので処理をしない


	SsCell * cell = state->cellValue.cell;
	if ( cell == 0 ) return ;

	ISSTexture*	texture = state->cellValue.texture;
	if ( texture == 0 ) return ;

	SsPoint2 texturePixelSize;
	texturePixelSize.x = state->cellValue.texture->getWidth();
	texturePixelSize.y = state->cellValue.texture->getHeight();

	execMask(state);

	//Ver6 ローカル不透明度対応
	float 	alpha = state->alpha;
	if (state->localalpha != 1.0f)
	{
		alpha = state->localalpha;
	}

	if (cell)
	{
		for ( int i = 0 ; i < 5 ; i++ ) rates[i] = 0.0f;
	}else{
		for ( int i = 0 ; i < 5 ; i++ ) rates[i] = 1.0f;
	}

	if (cell)
	{
		// テクスチャのサイズが2のべき乗かチェック
		if ( texture->isPow2() )
		{
			// 2のべき乗
			texture_is_pow2 = true;
			gl_target = GL_TEXTURE_2D;
		}
		else
		{
			// 2のべき乗ではない:NPOTテクスチャ
			texture_is_pow2 = false;
#if USE_GLEW
			gl_target = GL_TEXTURE_RECTANGLE_ARB;
#else
			gl_target = GL_TEXTURE_RECTANGLE;
#endif
		}


		glEnable(gl_target);

#if SPRITESTUDIO6SDK_PROGRAMABLE_SHADER_ON
		if ( state->is_shader ) {
			std::map<SsString, std::unique_ptr<SSOpenGLProgramObject>>::const_iterator it = s_DefaultShaderMap.find( state->shaderValue.id );
			if ( it != s_DefaultShaderMap.end() ) {
				pPrgObject = s_DefaultShaderMap[state->shaderValue.id].get();
			}
		}
		if ( !pPrgObject ) {
			std::map<SsString, std::unique_ptr<SSOpenGLProgramObject>>::const_iterator it = s_DefaultShaderMap.find( "system::default" );
			if ( it != s_DefaultShaderMap.end() ) {
				pPrgObject = s_DefaultShaderMap["system::default"].get();
			}
		}

//		if ( glpgObject )
		if ( pPrgObject )
		{
			glActiveTexture(GL_TEXTURE0);
		}
#endif

		SSTextureGL* tex_gl = (SSTextureGL*)texture;
		glBindTexture(gl_target, tex_gl->tex);

		// フィルタ
		GLint filterMode;
		SsTexFilterMode::_enum fmode = state->cellValue.filterMode;
		SsTexWrapMode::_enum wmode = state->cellValue.wrapMode;


		switch (static_cast<int>(fmode))
		{
		default:
		case SsTexFilterMode::nearlest:
			filterMode = GL_NEAREST;
			break;
		case SsTexFilterMode::linear:
			filterMode = GL_LINEAR;
			break;
		}

		glTexParameteri(gl_target, GL_TEXTURE_MIN_FILTER, filterMode);
		glTexParameteri(gl_target, GL_TEXTURE_MAG_FILTER, filterMode);

		// ラップモード
		GLint wrapMode; 
		switch (wmode)
		{
		default:
		case SsTexWrapMode::clamp:
			wrapMode = GL_CLAMP_TO_EDGE;
			if (texture_is_pow2 == false)
			{
				wrapMode = GL_CLAMP;
			}
			break;
		case SsTexWrapMode::repeat:
			wrapMode = GL_REPEAT;	// TODO 要動作確認
			break;
		case SsTexWrapMode::mirror:
			wrapMode = GL_MIRRORED_REPEAT;	// TODO 要動作確認
			break;
		}

		glTexParameteri(gl_target, GL_TEXTURE_WRAP_S, wrapMode);
		glTexParameteri(gl_target, GL_TEXTURE_WRAP_T, wrapMode);

		// セルが持つUV値をまず設定
		std::memcpy( state->uvs, state->cellValue.uvs, sizeof( state->uvs ));

		// UV アニメの適用
		glMatrixMode(GL_TEXTURE);
		glLoadIdentity();

		float uvw = state->cellValue.uvs[3].x + state->cellValue.uvs[0].x;
		float uvh = state->cellValue.uvs[3].y + state->cellValue.uvs[0].y;
        SsVector2 uv_trans;

		if (texture_is_pow2)
		{
			uv_trans.x = state->uvTranslate.x;
			uv_trans.y = state->uvTranslate.y;

			fTexW = texturePixelSize.x;
			fTexH = texturePixelSize.y;
		}
		else
		{
			uv_trans.x = state->uvTranslate.x * texturePixelSize.x;
			uv_trans.y = state->uvTranslate.y * texturePixelSize.y;


			//中心座標を計算
			uvw*= texturePixelSize.x;
			uvh*= texturePixelSize.y;

			fTexW = texturePixelSize.x;
			fTexH = texturePixelSize.y;
		}

		uvw/=2.0f;
		uvh/=2.0f;

		glTranslatef( uvw + uv_trans.x , uvh + uv_trans.y , 0 );
		glRotatef( state->uvRotation, 0.0, 0.0, 1.0);

		float	uvsh = state->uvScale.x;
		float	uvsv = state->uvScale.y;

		if ( state->imageFlipH ) uvsh*=-1;
		if ( state->imageFlipV ) uvsv*=-1;

		glScalef( uvsh , uvsv , 0.0);
		glTranslatef( -uvw , -uvh , 0 );

		glMatrixMode(GL_MODELVIEW);
		
		// オリジナルUVをまず指定する
		// 左下が 0,0 Ｚ字順
		static const int sUvOrders[][4] = {
			{0, 1, 2, 3},	// フリップなし
			{1, 0, 3, 2},	// 水平フリップ
			{2, 3, 0, 1},	// 垂直フリップ
			{3, 2, 1, 0},	// 両方フリップ
		};

    	//memset( state->uvs , 0 , sizeof(float) * 10 );

		// イメージのみのフリップ対応

		int order = ( state->hFlip == true ? 1 : 0 ) + ( state->vFlip == true ? 1 :0 ) * 2;
		float	uvs[10];
		memset(uvs, 0, sizeof(float) * 10);
		const int * uvorder = &sUvOrders[order][0];
		if (texture_is_pow2)
		{
			fPixTX = 1.0f / texturePixelSize.x;
			fPixTY = 1.0f / texturePixelSize.y;
		}
		fCoordCU = 0.0f;
		fCoordCV = 0.0f;
		for (int i = 0; i < 4; ++i)
		{
			int idx = *uvorder;
			if (texture_is_pow2 == false)
			{
				// GL_TEXTURE_RECTANGLE_ARBではuv座標系が0～1ではなくピクセルになるので変換
				uvs[idx * 2] = state->cellValue.uvs[i].x * texturePixelSize.x;
				uvs[idx * 2 + 1] = state->cellValue.uvs[i].y * texturePixelSize.y;

#if SPRITESTUDIO6SDK_USE_TRIANGLE_FIN
				//きれいな頂点変形への対応
				uvs[4 * 2] += uvs[idx * 2];
				uvs[4 * 2 + 1] += uvs[idx * 2 + 1];
#endif
			}
			else
			{
				uvs[idx * 2] = state->cellValue.uvs[i].x;
				uvs[idx * 2 + 1] = state->cellValue.uvs[i].y;

#if SPRITESTUDIO6SDK_USE_TRIANGLE_FIN
				//きれいな頂点変形への対応
				uvs[4 * 2] += uvs[idx * 2];
				uvs[4 * 2 + 1] += uvs[idx * 2 + 1];
#endif
			}

			if ( i == 0 ) {
				fCoordLU = uvs[idx * 2];
				fCoordTV = uvs[idx * 2 + 1];
				fCoordRU = uvs[idx * 2];
				fCoordBV = uvs[idx * 2 + 1];
			}else{
				fCoordLU = fCoordLU > uvs[idx * 2] ? uvs[idx * 2] : fCoordLU;
				fCoordTV = fCoordTV > uvs[idx * 2 + 1] ? uvs[idx * 2 + 1] : fCoordTV;
				fCoordRU = fCoordRU < uvs[idx * 2] ? uvs[idx * 2] : fCoordRU;
				fCoordBV = fCoordBV < uvs[idx * 2 + 1] ? uvs[idx * 2 + 1] : fCoordBV;
			}

			fCoordCU += uvs[idx * 2];
			fCoordCV += uvs[idx * 2 + 1];

			++uvorder;
		}

#if SPRITESTUDIO6SDK_USE_TRIANGLE_FIN
		//きれいな頂点変形への対応
		uvs[4*2]/=4.0f;
		uvs[4*2+1]/=4.0f;

#endif

		fCoordCU *= 0.25f;
		fCoordCV *= 0.25f;

		// UV 配列を指定する
		glEnableClientState(GL_TEXTURE_COORD_ARRAY);
		glTexCoordPointer(2, GL_FLOAT, 0, (GLvoid *)uvs);
	}else{
	   //セルがない状態というのは本当は有効な状態ではないような気がするがNANが入るので何かを入れる
	   for ( int i =0 ; i < 10 ; i ++ ) state->uvs[i] = 0;

		glEnableClientState(GL_TEXTURE_COORD_ARRAY);
		glTexCoordPointer(2, GL_FLOAT, 0, (GLvoid *)state->uvs);
		//テクスチャのバインドを解除する　状況が不定になるため
        glBindTexture(gl_target, 0);
	}

	// αブレンドの設定
	SetAlphaBlendMode( state->alphaBlendType );

	//メッシュの場合描画
	if (state->partType == SsPartType::mesh)
	{
		this->renderMesh(state->meshPart.get() , alpha );
		return;
	}

	// パーツカラーの指定
	if (state->is_parts_color)
	{
		partsColorEnabled = true;
		//6.2対応　パーツカラー、頂点、ミックス時に不透明度を適用する
		// パーツカラーがある時だけブレンド計算する
		if (state->partsColorValue.target == SsColorBlendTarget::whole)
		{
			// 単色
			const SsColorBlendValue& cbv = state->partsColorValue.color;
			setupSimpleTextureCombiner_for_PartsColor_(state->partsColorValue.blendType, cbv.rate, state->partsColorValue.target);
			rgbaByteToFloat_(state->colors, cbv);
			state->colors[0 + 3] *= alpha; // 不透明度を掛ける
			rates[0] = 1.0f;
			vertexID[0] = 0;

			// 残り３つは先頭のをコピー
			for (int i = 1; i < 5; ++i)
			{
				memcpy(state->colors + i * 4, state->colors, sizeof(state->colors[0]) * 4);
				rates[i] = 1.0f;
				vertexID[i * 2] = 0;
				vertexID[i * 2 + 1] = 0;
				//vertexID[i] = 0;
			}
		}
		else
		{
			// 頂点単位
			setupSimpleTextureCombiner_for_PartsColor_(state->partsColorValue.blendType, alpha, state->partsColorValue.target); // ミックス用に const に不透明度を入れておく。

			for (int i = 0; i < 4; ++i)
			{
				const SsColorBlendValue& cbv = state->partsColorValue.colors[i];
				rgbaByteToFloat_(state->colors + i * 4, cbv);

				// 不透明度も掛ける (ミックスの場合、Rate として扱われるので不透明度を掛けてはいけない)
				if (state->partsColorValue.blendType != SsBlendType::mix)
				{
					state->colors[i * 4 + 3] *= alpha;
				}
				rates[i] = 1.0f;
				vertexID[i * 2] = i;
				vertexID[i * 2 + 1] = i;
			}

#if SPRITESTUDIO6SDK_USE_TRIANGLE_FIN
			// 中央頂点(index=4)のRGBA%値の計算
			calcCenterVertexColor(state->colors, rates, vertexID);
#else
			// クリアしとく
			state->colors[4 * 4 + 0] = 0;
			state->colors[4 * 4 + 1] = 0;
			state->colors[4 * 4 + 2] = 0;
			state->colors[4 * 4 + 3] = 0;
			rates[4] = 0;
#endif
		}
	}
	else
	{
		// パーツカラー無し
		for (int i = 0; i < 5; ++i)
			state->colors[i * 4 + 3] = alpha;

		// RGB=100%テクスチャ、A=テクスチャｘ頂点カラーの設定にする。
		setupTextureCombinerTo_NoBlendRGB_MultiplyAlpha_();
	}


	glEnableClientState(GL_COLOR_ARRAY);
	glColorPointer(4, GL_FLOAT, 0, (GLvoid *)state->colors);


	// 頂点バッファの設定
	glEnableClientState(GL_VERTEX_ARRAY);

	glVertexPointer(3, GL_FLOAT, 0, (GLvoid *)state->vertices);

	glPushMatrix();

	// update で計算しておいた行列をロード
	glMatrixMode(GL_MODELVIEW);
//	glLoadMatrixf(state->matrix);
	glLoadMatrixf(state->matrixLocal);	//Ver6 ローカルスケール対応

	GLint VertexLocation = -1;
	if (state->noCells)
	{
		//セルが無いので描画を行わない
	}else{

#if SPRITESTUDIO6SDK_PROGRAMABLE_SHADER_ON

	if ( state->is_shader )
	{
//      if ( glpgObject )
		if ( pPrgObject )
		{
			GLint uid;
			int		type = (int)state->partsColorValue.blendType;

			if ( state->meshPart && cell && cell->ismesh )
			{
				int		iCount = state->meshPart->targetCell->meshPointList.size();

				if ( s_iVArgCount < iCount ) {
					clearShaderCache();
					s_pVArg = (ShaderVArg*)malloc( sizeof( ShaderVArg ) * iCount );
					s_iVArgCount = iCount;
				}

				for ( int i = 0; i < iCount; i++ ) {
					s_pVArg[i].fSrcRatio = type <= 1 ? 1.0f - rates[0] : 1.0f;
					s_pVArg[i].fDstRatio = type == 3 ? -rates[0] : rates[0];
					s_pVArg[i].fDstSrcRatio = type == 1 ? 1.0f : 0.0f;
					s_pVArg[i].fReserved = 0.0f;
				}
			}else{
				int		iCount = 5;

				if ( s_iVArgCount < iCount ) {
					clearShaderCache();
					s_pVArg = (ShaderVArg*)malloc( sizeof( ShaderVArg ) * iCount );
					s_iVArgCount = iCount;
				}

				for ( int i = 0; i < iCount; i++ ) {
					s_pVArg[i].fSrcRatio = type <= 1 ? 1.0f - rates[i] : 1.0f;
					s_pVArg[i].fDstRatio = type == 3 ? -rates[i] : rates[i];
					s_pVArg[i].fDstSrcRatio = type == 1 ? 1.0f : 0.0f;
					s_pVArg[i].fReserved = 0.0f;
				}
			}

			VertexLocation = pPrgObject->GetAttribLocation( "varg" );
			if ( VertexLocation >= 0 ) {
				glVertexAttribPointer( VertexLocation , 4 , GL_FLOAT , GL_FALSE, 0, s_pVArg);//GL_FALSE→データを正規化しない
				glEnableVertexAttribArray(VertexLocation);//有効化
			}

			//シェーダのセットアップ
			pPrgObject->Enable();

			uid = pPrgObject->GetUniformLocation( "args" );
			if ( uid >= 0 ) {
				ShaderFArg	args;

				args.fTexW = fTexW;
				args.fTexH = fTexH;
				args.fPixTX = fPixTX;
				args.fPixTY = fPixTY;
				args.fCoordLU = fCoordLU;
				args.fCoordTV = fCoordTV;
				args.fCoordCU = fCoordCU;
				args.fCoordCV = fCoordCV;
				args.fCoordRU = fCoordRU;
				args.fCoordBV = fCoordBV;
				args.fPMA = 0.0f;
				args.fReserved1 = 0.0f;
				args.fReserved2 = 0.0f;
				args.fReserved3 = 0.0f;
				args.fReserved4 = 0.0f;
				args.fReserved5 = 0.0f;

				if (
					( !state->cellValue.texture )
				) {
					memset( &args, 0, sizeof( args ) );
				}

				glUniform1fv( uid, sizeof( args ) / sizeof( float ), (float*)&args );
			}

			uid = pPrgObject->GetUniformLocation( "params" );
			if ( uid >= 0 ) {
				glUniform1fv( uid, sizeof( state->shaderValue.param ) / sizeof( float ), state->shaderValue.param );
			}
		}
	}

#endif
#if SPRITESTUDIO6SDK_USE_TRIANGLE_FIN
		if ( state->is_vertex_transform || state->is_parts_color)
		{
			static const GLubyte indices[] = { 4 , 3, 1, 0, 2 , 3};
			glDrawElements(GL_TRIANGLE_FAN, 6, GL_UNSIGNED_BYTE, indices);
		}else{
			glDrawArrays(GL_TRIANGLE_STRIP, 0, 4);

		}
#else
		// 頂点配列を描画
		glDrawArrays(GL_TRIANGLE_STRIP, 0, 4);

#endif
	}

#if SPRITESTUDIO6SDK_PROGRAMABLE_SHADER_ON
//	if ( glpgObject )
	{
		if ( state->is_shader )
		{
//			if ( glpgObject )
			if ( pPrgObject )
			{
				if ( VertexLocation >= 0 ) {
					glDisableVertexAttribArray(VertexLocation);//無効化
				}
				pPrgObject->Disable();
			}
		}
	}
#endif

	glPopMatrix();

	if (texture_is_pow2 == false)
	{
		glDisable(gl_target);
	}
	glDisable(GL_TEXTURE_2D);

	//ブレンドモード　減算時の設定を戻す
	glBlendEquation( GL_FUNC_ADD );

}

}	// namespace spritestudio6
