#ifndef SSOPENGLSHAPEDRAWER_H
#define SSOPENGLSHAPEDRAWER_H

#include "../Loader/shape/ssshape.h"
namespace SpriteStudio
{
	class SsOpenGLShapeDrawer
	{
	public:
		SsOpenGLShapeDrawer();

		bool execute(const SsShape& shape, const float* vertices, const float* colors);
	};
}
#endif // SSOPENGLSHAPEDRAWER_H
