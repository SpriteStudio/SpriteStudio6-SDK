﻿#ifndef __SSPLAYER_RENDER_GL__
#define __SSPLAYER_RENDER_GL__

#include "../Animator/ssplayer_render.h"

struct SsPartState;
class SsMeshPart;
class SSOpenGLVertexShader;
class SSOpenGLFragmentShader;
class SSOpenGLFragmentShader;

class SsRenderGL : public ISsRenderer
{
private:
	//static bool	m_isInit;
	SSOpenGLVertexShader*	vs;
	SSOpenGLFragmentShader*	fs1;
	SSOpenGLFragmentShader*	fs2;
public:
	SsRenderGL() : vs(0), fs1(0), fs2(0) {};
	virtual ~SsRenderGL();

	virtual void	initialize();
	virtual void	renderSetup();
	virtual void	renderPart( SsPartState* state );

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


	virtual void	renderMesh(SsMeshPart* mesh, float alpha);



};

#endif
