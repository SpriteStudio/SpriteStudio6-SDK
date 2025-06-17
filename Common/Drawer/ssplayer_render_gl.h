#ifndef __SSPLAYER_RENDER_GL__
#define __SSPLAYER_RENDER_GL__

#include "../Animator/ssplayer_render.h"

namespace spritestudio6
{

struct SsPartState;
class SsMeshPart;


class SsRenderGL : public ISsRenderer
{
private:
	//static bool	m_isInit;

public:
	SsRenderGL(){}
	virtual ~SsRenderGL(){}

	static void clearShaderCache();
	virtual void	initialize();
	virtual void	renderSetup(SsAnimeDecoder* state);

	//通常アニメパーツ描画
	virtual void	renderPart( SsPartState* state );

	//メッシュパーツのレンダリング
	virtual void	renderMesh(SsMeshPart* mesh, float alpha);

	//effect用スプライト
	virtual void	renderSpriteSimple( float matrix[16], 
										int width, int height, 
										SsVector2& pivot , 
										SsVector2 uv1, SsVector2 uv2, 
										const SsFColor& color );

	virtual void	SetAlphaBlendMode(SsBlendType::_enum type);
	virtual void	SetTexture( SsCellValue* cell );
	virtual void	execMask(SsPartState* state);
	virtual void	clearMask();
	virtual void	enableMask(bool flag);




};

}	// namespace spritestudio6

#endif
