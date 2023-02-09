#ifndef SSOPENGLNINESDRAWER_H
#define SSOPENGLNINESDRAWER_H

#include "../Loader/nineslice/ssnines.h"
namespace SpriteStudio
{
	class SsOpenGLNinesDrawer
	{
	public:
		SsOpenGLNinesDrawer();

		bool execute(const SsNines& nines, const float* vertices, const float* colors);
	};
};
#endif // SSOPENGLNINESDRAWER_H
