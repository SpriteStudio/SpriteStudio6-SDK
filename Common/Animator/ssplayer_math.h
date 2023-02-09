#ifndef __SSPLAYER_MACRO__
#define __SSPLAYER_MACRO__

#include <math.h>
#include "sstypes.h"

namespace SpriteStudio
{
#if 0
	constexpr auto __PI__ = (3.14159265358979323846f);
	// #define RadianToDegree(Radian) ((double)Radian * (180.0f / __PI__))
	template<typename T> inline double RadianToDegree(T Radian)
	{
		return((double)Radian * (180.0f / __PI__));
	}
	// #define DegreeToRadian(Degree) ((double)Degree * (__PI__ / 180.0f))
	template<typename T> inline double DegreeToRadian(T Degree)
	{
		return((double)Degree * (__PI__ / 180.0f));
	}

#endif

	#define SPRITESTUDIO6SDK_foreach(T, c, i) for(T::iterator i = c.begin(); i!=c.end(); ++i)

	#define	SS_PI					( 3.14159265358979323846f )
	#define	RadianToDegree( _r )	( static_cast<float>( _r ) * ( 180.0f / SS_PI ) )
	#define	DegreeToRadian( _d )	( static_cast<float>( _d ) * ( SS_PI / 180.0f ) )

	#define	SS_MAX( _l, _r )		( ( ( _l ) > ( _r ) ) ? ( _l ) : ( _r ) )
	#define	SS_MIN( _l, _r )		( ( ( _l ) < ( _r ) ) ? ( _l ) : ( _r ) )

	#define	SS_SINF( _r )			( sinf( _r ) )
	#define	SS_COSF( _r )			( cosf( _r ) )

	class SsMath
	{
	private:
		SsMath();
		~SsMath();

	public:
		static int convPow2(int n);
		static bool isPow2(int n);

		static int Floor(const float X);
		static int Ceil(const float X);

		static double RoundTo(double dValue, int iDigit);

		static SsIRect adjustInside(const SsIRect& rect, const SsIRect& region);
	};


}	// namespace SpriteStudio

#endif
