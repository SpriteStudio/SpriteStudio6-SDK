#include "ssloader.h"
#include "ssInterpolation.h"

namespace SpriteStudio
{

//---------------------------------------------------------------------------
/**
	等速運動
*/
//---------------------------------------------------------------------------
static float	// nowにおける補間された値
linear_(
	float	start,	// 始点
	float	end,	// 終点
	float	now)	// 現在の時間 0.f~1.f
{
	return start + (now * (end - start));
}

//---------------------------------------------------------------------------
/**
	加速運動
*/
//---------------------------------------------------------------------------
static float	// nowにおける補間された値
accelerating_(
	float	start,	// 始点
	float	end,	// 終点
	float	now)	// 現在の時間 0.f~1.f
{
	float rate = now * now;
	return start + (rate * (end - start));
}

//---------------------------------------------------------------------------
/**
	減速運動
*/
//---------------------------------------------------------------------------
static float	// nowにおける補間された値
decelerating_(
	float	start,	// 始点
	float	end,	// 終点
	float	now)	// 現在の時間 0.f~1.f
{
	float time = 1 - now;
	float rate = 1 - time * time;
	return start + (rate * (end - start));
}


static float bezier_(float start, float end, float time, const SsCurve * c)
{
	//値が変化しない場合は左キーを補間値とする
	if ((start == end) && (c->startValue == 0.0f) && (c->endValue == 0.0f))
	{
		return start;
	}


	float fCurrentPos = (c->endKeyTime - c->startKeyTime) * time + c->startKeyTime;

	float fRet = end;
	float fCurrentCalc = 0.5f;
	float fCalcRange = 0.5f;

	float fTemp1;
	float fTemp2;
	float fTemp3;

	float fCurrentX;

	for(int iLoop = 0; iLoop < 8; iLoop++ )
	{// more count of loop, better precision increase
		fTemp1 = 1.0f - fCurrentCalc;
		fTemp2 = fTemp1 * fTemp1;
		fTemp3 = fTemp2 * fTemp1;
		fCurrentX = ( fTemp3 * c->startKeyTime ) +
					( 3 * fTemp2 * fCurrentCalc * (c->startTime + c->startKeyTime) ) +
					( 3 * fTemp1 * fCurrentCalc * fCurrentCalc * (c->endTime + c->endKeyTime) ) +
					( fCurrentCalc * fCurrentCalc * fCurrentCalc * c->endKeyTime);

		fCalcRange /= 2.0f;
		if( fCurrentX > fCurrentPos )
		{
			fCurrentCalc -= fCalcRange;
		}
		else
		{
			fCurrentCalc += fCalcRange;
		}
	}

	// finally calculate with current value
	fTemp1 = 1.0f - fCurrentCalc;
	fTemp2 = fTemp1 * fTemp1;
	fTemp3 = fTemp2 * fTemp1;
	fRet = ( fTemp3 * start ) +
				( 3 * fTemp2 * fCurrentCalc * (c->startValue + start) ) +
				( 3 * fTemp1 * fCurrentCalc * fCurrentCalc * (c->endValue + end) ) +
				( fCurrentCalc * fCurrentCalc * fCurrentCalc * end );

	return fRet;
}

/**
	エルミートでは c->startTime, c->endTime は必要ない

	スロープ値を事前計算しておけばカーブ計算用パラメータは１つになる
	が、ベジェと共用するためこのままの形にしておく。
*/
static float hermite_(float start, float end, float time, const SsCurve * c)
{
	float t2 = time * time;
	float t3 = t2 * time;
	float result =
		(2 * t3 - 3 * t2 + 1) * start +
		(-2 * t3 + 3 * t2) * end +
		(t3 - 2 * t2 + time) * (c->startValue - start) +
		(t3 - t2) * (c->endValue - end);
	return result;
}

#if 1 
//add SS 7.1

static float easeIn(float start, float end, float time, float easeingRate)
{
    float t = powf(time, easeingRate);
    return linear_(start, end, t);
}
static float easeOut(float start, float end, float time, float easeingRate)
{
    float t = powf(time, 1 / easeingRate);
    return linear_(start, end, t);
}
static float easeInOut(float start, float end, float time, float easeingRate)
{
    time *= 2;
    float t = 0;
    if (time < 1)
    {
        t = 0.5f * powf(time, easeingRate);
    }
    else
    {
        t = 1.0f - 0.5f * powf(2 - time, easeingRate);
    }

    return linear_(start, end, t);
}

static float easeExponentialIn(float start, float end, float time, float easeingRate)
{
    float t = (time == 0 ? 0 : powf(2, 10 * (time / 1 - 1)) - 1 * 0.001f);
    return linear_(start, end, t);
}

static float easeExponentialOut(float start, float end, float time, float easeingRate)
{
    float t = (time == 1 ? 1 : (-powf(2, -10 * time / 1) + 1));
    return linear_(start, end, t);
}

static float easeExponentialInOut(float start, float end, float time, float easeingRate)
{
    time /= 0.5f;
    float t = 0;
    if (time < 1)
    {
        t = 0.5f * powf(2, 10 * (time - 1));
    }
    else
    {
        t = 0.5f * (-powf(2, -10 * (time - 1)) + 2);
    }

    return linear_(start, end, t);
}

#define M_PI_2     1.57079632679489661923   // pi/2
#define M_PI       3.14159265358979323846   // pi
#define M_PI_X_2 (float)M_PI * 2.0f



static float  easeSineIn(float start, float end, float time, float easeingRate)
{
    float t = (-1 * cosf(time * (float)M_PI_2) + 1);
    return linear_(start, end, t);
}

static float easeSineOut(float start, float end, float time, float easeingRate)
{
    float t = (sinf(time * (float)M_PI_2));
    return linear_(start, end, t);
}

static float easeSineInOut(float start, float end, float time, float easeingRate)
{
    float t = (-0.5f * (cosf((float)M_PI * time) - 1));
    return linear_(start, end, t);
}

//set period of the wave in radians.
static float easeElasticIn(float start, float end, float time, float fPeriod)
{
    float newT = 0;
    if (time == 0 || time == 1)
    {
        newT = time;
    }
    else
    {
        float s = fPeriod / 4;
        time = time - 1;
        newT = -powf(2, 10 * time) * sinf((time - s) * M_PI_X_2 / fPeriod);
    }

    return linear_(start, end, newT);
}

static float easeElasticOut(float start, float end, float time, float fPeriod)
{
    float newT = 0;
    if (time == 0 || time == 1)
    {
        newT = time;
    }
    else
    {
        float s = fPeriod / 4;
        newT = powf(2, -10 * time) * sinf((time - s) * M_PI_X_2 / fPeriod) + 1;
    }

    return linear_(start, end, newT);
}

static float easeElasticInOut(float start, float end, float time, float fPeriod)
{
    float newT = 0;
    if (time == 0 || time == 1)
    {
        newT = time;
    }
    else
    {
        time = time * 2;
        if (!fPeriod)
        {
            fPeriod = 0.3f * 1.5f;
        }

        float s = fPeriod / 4;

        time = time - 1;
        if (time < 0)
        {
            newT = -0.5f * powf(2, 10 * time) * sinf((time - s) * M_PI_X_2 / fPeriod);
        }
        else
        {
            newT = powf(2, -10 * time) * sinf((time - s) * M_PI_X_2 / fPeriod) * 0.5f + 1;
        }
    }

    return linear_(start, end, newT);

}

static float bounceTime(float time)
{
    if (time < 1 / 2.75)
    {
        return 7.5625f * time * time;
    }
    else
        if (time < 2 / 2.75)
        {
            time -= 1.5f / 2.75f;
            return 7.5625f * time * time + 0.75f;
        }
        else
            if (time < 2.5 / 2.75)
            {
                time -= 2.25f / 2.75f;
                return 7.5625f * time * time + 0.9375f;
            }

    time -= 2.625f / 2.75f;
    return 7.5625f * time * time + 0.984375f;
}

static float easeBounceIn(float start, float end, float time, float easeingRate)
{
    float newT = 1 - bounceTime(1 - time);
    return linear_(start, end, newT);
}

static float easeBounceOut(float start, float end, float time, float easeingRate)
{
    float newT = bounceTime(time);
    return linear_(start, end, newT);
}

static float easeBounceInOut(float start, float end, float time, float easeingRate)
{
    float newT = 0;
    if (time < 0.5f)
    {
        time = time * 2;
        newT = (1 - bounceTime(1 - time)) * 0.5f;
    }
    else
    {
        newT = bounceTime(time * 2 - 1) * 0.5f + 0.5f;
    }

    return linear_(start, end, newT);
}

static float easeBackIn(float start, float end, float time, float easeingRate)
{
    float overshoot = 1.70158f;
    float newT = (time * time * ((overshoot + 1) * time - overshoot));
    return linear_(start, end, newT);

}

static float easeBackOut(float start, float end, float time, float easeingRate)
{
    float overshoot = 1.70158f;

    time = time - 1;
    float newT = (time * time * ((overshoot + 1) * time + overshoot) + 1);
    return linear_(start, end, newT);

}

static float easeBackInOut(float start, float end, float time, float easeingRate)
{
    float overshoot = 1.70158f * 1.525f;

    time = time * 2;
    float newT = 0;
    if (time < 1)
    {
        newT = ((time * time * ((overshoot + 1) * time - overshoot)) / 2);
    }
    else
    {
        time = time - 2;
        newT = ((time * time * ((overshoot + 1) * time + overshoot)) / 2 + 1);
    }
    return linear_(start, end, newT);
}

#endif

/// SsVector2 のメンバ全体の補間
//SsVector2	SsInterpolate(SsInterpolationType::_enum ipType, float time, SsVector2 start, SsVector2 end, const SsCurve * curve)
SsVector2	SsInterpolate(SsInterpolationType::_enum ipType, float easingRate, float time, SsVector2 start, SsVector2 end, const SsCurve* curve)
{
	SsVector2 out;
	out.x = SsInterpolate(ipType, easingRate , time, start.x, end.x, curve);
	out.y = SsInterpolate(ipType, easingRate , time, start.y, end.y, curve);

	return(out);
}

//----------------------------------------------------------------------------
/**
	タイプを指定して補間する
*/
//----------------------------------------------------------------------------
//float	SsInterpolate(SsInterpolationType::_enum type, float time, float start, float end, const SsCurve * curve)
float   SsInterpolate(SsInterpolationType::_enum type, float easingRate, float time, float start, float end, const SsCurve* curve)
{
    //float easingRate = 1.0f;
	float r = start;
	switch (type)
	{
	case SsInterpolationType::none:
//		r = start;
		break;
	case SsInterpolationType::linear:
		r = linear_(start, end, time);
		break;
	case SsInterpolationType::acceleration:
		r = accelerating_(start, end, time);
		break;
	case SsInterpolationType::deceleration:
		r = decelerating_(start, end, time);
		break;
	case SsInterpolationType::bezier:
		r = bezier_(start, end, time, curve);
		break;
	case SsInterpolationType::hermite:
		r = hermite_(start, end, time, curve);
		break;
    //add SS7.1
    case SsInterpolationType::easeIn:
        //qDebug() << "easingRate" << easingRate;
        r = easeIn(start, end, time, easingRate);
        break;
    case SsInterpolationType::easeOut:
        r = easeOut(start, end, time, easingRate);
        break;
    case SsInterpolationType::easeInOut:
        r = easeInOut(start, end, time, easingRate);
        break;
    case SsInterpolationType::easeExponentialIn:
        r = easeExponentialIn(start, end, time, easingRate);
        break;
    case SsInterpolationType::easeExponentialOut:
        r = easeExponentialOut(start, end, time, easingRate);
        break;
    case SsInterpolationType::easeExponentialInOut:
        r = easeExponentialInOut(start, end, time, easingRate);
        break;
    case SsInterpolationType::easeSineIn:
        r = easeSineIn(start, end, time, easingRate);
        break;
    case SsInterpolationType::easeSineOut:
        r = easeSineOut(start, end, time, easingRate);
        break;
    case SsInterpolationType::easeSineInOut:
        r = easeSineInOut(start, end, time, easingRate);
        break;
    case SsInterpolationType::easeElasticIn:
        r = easeElasticIn(start, end, time, easingRate);
        break;
    case SsInterpolationType::easeElasticOut:
        r = easeElasticOut(start, end, time, easingRate);
        break;
    case SsInterpolationType::easeElasticInOut:
        r = easeElasticInOut(start, end, time, easingRate);
        break;
    case SsInterpolationType::easeBounceIn:
        r = easeBounceIn(start, end, time, easingRate);
        break;
    case SsInterpolationType::easeBounceOut:
        r = easeBounceOut(start, end, time, easingRate);
        break;
    case SsInterpolationType::easeBounceInOut:
        r = easeBounceInOut(start, end, time, easingRate);
        break;
    case SsInterpolationType::easeBackIn:
        r = easeBackIn(start, end, time, easingRate);
        break;
    case SsInterpolationType::easeBackOut:
        r = easeBackOut(start, end, time, easingRate);
        break;
    case SsInterpolationType::easeBackInOut:
        r = easeBackInOut(start, end, time, easingRate);
        break;
	default:
		//SS_ASSERT_ID(type);
		break;
	}
	return r;
}

}	// namespace SpriteStudio
