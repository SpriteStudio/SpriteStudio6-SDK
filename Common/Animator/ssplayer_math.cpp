
#include "ssplayer_math.h"

namespace SpriteStudio
{


	SsMath::SsMath()
	{
	}

	SsMath::~SsMath()
	{
	}

	int SsMath::convPow2(int n)
	{
		for (int i = 0; i < 30; i++) {
			if (n <= (1 << i)) {
				return	1 << i;
			}
		}

		return	0x40000000;
	}

	bool SsMath::isPow2(int n)
	{
		for (int i = 0; i < 30; i++) {
			if (n == (1 << i)) {
				return	true;
			}
		}

		return	false;
	}

	int SsMath::Floor(const float X)
	{
		return	static_cast<int>(floor(X));
	}

	int SsMath::Ceil(const float X)
	{
		return	static_cast<int>(ceil(X));
	}

	double SsMath::RoundTo(double dValue, int iDigit)
	{
		double	dHi = floor(dValue);
		double	dLo = dValue - dHi;
		double	dMul = 1;

		if (iDigit < 0) {
			for (int i = 0; i > iDigit; i--) {
				dMul *= 10;
			}
			dValue = dHi + floor(dLo * dMul) / dMul;
		}
		else {
			for (int i = 0; i < iDigit; i++) {
				dMul *= 0.1;
			}
			dValue = floor(dHi * dMul) / dMul;
		}

		return	dValue;
	}

	SsIRect SsMath::adjustInside(const SsIRect& rect, const SsIRect& region)
	{
		SsIRect	result = rect;
		SsIRect	inside = region;

		inside.moveRight(inside.right() - result.width());
		inside.moveBottom(inside.bottom() - result.height());

		if (result.left() < inside.left()) {
			result.moveLeft(inside.left());
		}
		else
			if (result.left() > inside.right()) {
				result.moveLeft(inside.right());
			}

		if (result.top() < inside.top()) {
			result.moveTop(inside.top());
		}
		else
			if (result.top() > inside.bottom()) {
				result.moveTop(inside.bottom());
			}

		return	result;
	}


};	// namespace SpriteStudio
