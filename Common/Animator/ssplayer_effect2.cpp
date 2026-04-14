
#include <stdio.h>
#include <cstdlib>

#include "../Loader/ssloader.h"

#include "ssplayer_animedecode.h"
#include "ssplayer_effect2.h"
#include "ssplayer_macro.h"
#include "ssplayer_matrix.h"
#include "ssplayer_render.h"
#include "ssplayer_effectfunction.h"

// MEMO: cpp側でのファイルスコープなので大丈夫かとは思うが、念のためSPRITESTUDIO6SDK_を接頭しておく
//       ※ちなみにどちらも使用していない。
#define SPRITESTUDIO6SDK_DEBUG_DISP (0)
#define SPRITESTUDIO6SDK_BUILD_ERROR_0418 (0)

namespace spritestudio6
{

namespace {
	// 内部用数学ヘルパー (SDK依存を避けるため)
	float getVector2Length(const effect_v2::Vector2& v) {
		return std::sqrt(v.x * v.x + v.y * v.y);
	}

	effect_v2::Vector2 normalizeVector2(const effect_v2::Vector2& v) {
		float len = getVector2Length(v);
		if (len == 0) return effect_v2::Vector2(0, 0);
		return effect_v2::Vector2(v.x / len, v.y / len);
	}

	float getAngle360(const effect_v2::Vector2& v1, const effect_v2::Vector2& v2) {
		// SsVector2::get_angle_360 の代替
		float cos_val = v1.x * v2.x + v1.y * v2.y;
		float sin_val = v1.x * v2.y - v1.y * v2.x;
		float angle = std::atan2(sin_val, cos_val);
		return angle;
	}

    constexpr float PI = 3.1415926535f;
    float degreeToRadian(float degree) {
        return degree * PI / 180.0f;
    }
}

static u8 blendNumber( u8 a , u8 b , float rate )
{
	return (u8)((int32_t)( a + ( b - a ) * rate ));
}

static float blendFloat( float a,float b , float rate )
{
	return   ( a + ( b - a ) * rate );
}


float outQuad(float t,float totaltime,float max ,float min )
{
	if( totaltime == 0.0f ) return 0.0f;

	if ( t > totaltime ) t = totaltime;
	max -= min;
	t /= totaltime;
	return -max*t*(t-2.0f)+min;
}

namespace effect_v2 {

namespace {
	void calcPositionAndSpeed(xorshift32& rand, const ParticleParameter& p, float t, float life, float& outX, float& outY) {
		float rad = p.angle + (rand.genrand_float32() * p.angleVariance - p.angleVariance / 2.0f);
		float speed = p.speed + (p.speed2 * rand.genrand_float32());

		float addr = 0;
		if (p.useTanAccel) {
			float accel = p.tangentialAccel + (rand.genrand_float32() * p.tangentialAccel2);
			float _speed = (speed <= 0) ? 0.1f : speed;
			float l = life * _speed * 0.2f; //円の半径
			float c = 3.14f * l;
			addr = (accel / c) * t;
		}

		float x = std::cos(rad + addr) * speed * t;
		float y = std::sin(rad + addr) * speed * t;

		if (p.useTransSpeed) {
			float transspeed = p.transSpeed + (p.transSpeed2 * rand.genrand_float32());
			float speedadd = transspeed / life;

			float addtx = std::cos(rad + addr) * speed;
			float addtx_trans = std::cos(rad + addr) * speedadd;
			float addx = ((addtx_trans * t) + addtx) * (t + 1.0f) / 2.0f;

			float addty = std::sin(rad + addr) * speed;
			float addty_trans = std::sin(rad + addr) * speedadd;
			float addy = ((addty_trans * t) + addty) * (t + 1.0f) / 2.0f;

			x = addx;
			y = addy;
		}

		if (p.useGravity) {
			float t2 = t * t;
			x += (0.5f * p.gravity.x * t2);
			y += (0.5f * p.gravity.y * t2);
		}

		outX = x;
		outY = y;
	}

	void calcOffset(xorshift32& rand, const ParticleParameter& p, float& ox, float& oy) {
		ox = 0; oy = 0;
		if (p.useOffset) {
			ox = p.offset.x + (p.offset2.x * rand.genrand_float32());
			oy = p.offset.y + (p.offset2.y * rand.genrand_float32());
		}
	}

	float calcRotation(xorshift32& rand, const ParticleParameter& p, float t, float life) {
		if (!p.useRotation) return 0.0f;

		float rot = p.rotation + (rand.genrand_float32() * p.rotation2);
		float add = p.rotationAdd + (rand.genrand_float32() * p.rotationAdd2);

		if (p.useRotationTrans) {
			float lastt = life * p.endLifeTimePer;
			if (lastt == 0) {
				rot += (add * p.rotationFactor) * t;
			} else {
				float addf = (add * p.rotationFactor - add) / lastt;
				float mod_t = std::max(0.0f, t - lastt);
				float nowt = std::min(t, lastt);
				float final_soul = add + addf * nowt;
				float addrf = (final_soul + add) * (nowt + 1.0f) / 2.0f;
				addrf -= add;
				addrf += (mod_t * final_soul);
				rot += addrf;
			}
		} else {
			rot += (add * t);
		}
		return rot;
	}

	Color calcColor(xorshift32& rand, const ParticleParameter& p, float lifeper) {
		Color color(255, 255, 255, 255);

		if (p.useColor) {
			color.a = p.initColor.a + (u8)(rand.genrand_float32() * p.initColor2.a);
			color.r = p.initColor.r + (u8)(rand.genrand_float32() * p.initColor2.r);
			color.g = p.initColor.g + (u8)(rand.genrand_float32() * p.initColor2.g);
			color.b = p.initColor.b + (u8)(rand.genrand_float32() * p.initColor2.b);
		}

		if (p.useTransColor) {
			Color ecolor;
			ecolor.a = p.transColor.a + (u8)(rand.genrand_float32() * p.transColor2.a);
			ecolor.r = p.transColor.r + (u8)(rand.genrand_float32() * p.transColor2.r);
			ecolor.g = p.transColor.g + (u8)(rand.genrand_float32() * p.transColor2.g);
			ecolor.b = p.transColor.b + (u8)(rand.genrand_float32() * p.transColor2.b);

			color.a = blendNumber(color.a, ecolor.a, lifeper);
			color.r = blendNumber(color.r, ecolor.r, lifeper);
			color.g = blendNumber(color.g, ecolor.g, lifeper);
			color.b = blendNumber(color.b, ecolor.b, lifeper);
		}

		if (p.useAlphaFade) {
			float start = p.alphaFade;
			float end = p.alphaFade2;
			float per = lifeper * 100.0f;

			if (per < start && start > 0.0f) {
				float alpha = (start - per) / start;
				color.a = (u8)((float)color.a * (1.0f - alpha));
			} else if (per > end) {
				if (end >= 100.0f) {
					color.a = 0;
				} else {
					float alpha = std::min(1.0f, (per - end) / (100.0f - end));
					color.a = (u8)((float)color.a * (1.0f - alpha));
				}
			}
		}
		return color;
	}

	void calcScale(xorshift32& rand, const ParticleParameter& p, float lifeper, float& outScaleX, float& outScaleY) {
		float scaleX = 1.0f;
		float scaleY = 1.0f;
		float scalefactor = 1.0f;

		if (p.useInitScale) {
			scaleX = p.scale.x + (rand.genrand_float32() * p.scaleRange.x);
			scaleY = p.scale.y + (rand.genrand_float32() * p.scaleRange.y);
			scalefactor = p.scaleFactor + (rand.genrand_float32() * p.scaleFactor2);
		}

		if (p.useTransScale) {
			float s2_x = p.transscale.x + (rand.genrand_float32() * p.transscaleRange.x);
			float s2_y = p.transscale.y + (rand.genrand_float32() * p.transscaleRange.y);
			float sf2 = p.transscaleFactor + (rand.genrand_float32() * p.transscaleFactor2);

			scaleX = blendFloat(scaleX, s2_x, lifeper);
			scaleY = blendFloat(scaleY, s2_y, lifeper);
			scalefactor = blendFloat(scalefactor, sf2, lifeper);
		}

		outScaleX = scaleX * scalefactor;
		outScaleY = scaleY * scalefactor;
	}

	void applyPointGravity(const ParticleParameter& p, float& x, float& y, float ox, float oy, const Vector2& position, float t, float life) {
		Vector2 v(p.gravityPos.x - (ox + position.x), p.gravityPos.y - (oy + position.y));
		Vector2 nv = normalizeVector2(v);

		float gp = p.gravityPower;
		if (gp > 0) {
            float dist = std::sqrt(v.x * v.x + v.y * v.y);
			float len = std::max(0.1f, dist);
			float et = (len / gp) * 0.90f;
			float gt = std::min(t, et * 0.90f);

			x += nv.x * gp * gt;
			y += nv.y * gp * gt;

			float blend = outQuad(gt, et, 0.9f, 0.0f) + (t / life * 0.1f);
			x = blendFloat(x, p.gravityPos.x, blend);
			y = blendFloat(y, p.gravityPos.y, blend);
		} else {
			x += nv.x * gp * t;
			y += nv.y * gp * t;
		}
	}

	void calculatePositionOnly(
		xorshift32& rand,
		const ParticleParameter& p,
		float t,
		float life,
		const Vector2& position,
		float& outX,
		float& outY
	) {
		float x, y;
		calcPositionAndSpeed(rand, p, t, life, x, y);

		float ox, oy;
		calcOffset(rand, p, ox, oy);

		outX = x + ox + position.x;
		outY = y + oy + position.y;

		if (p.usePGravity) {
			applyPointGravity(p, outX, outY, ox, oy, position, t, life);
		}
	}
}

void EffectSimulator::calculateState(
	float currentTime,
	const ParticleExistSt& drawe,
	const EmitterParameter& emitterParams,
	const ParticleParameter& particleParams,
	int32_t emitterSeed,
	int32_t seedOffset,
	const std::vector<unsigned long>& seedList,
	size_t seedTableLen,
	const Vector2& position,
	int32_t pid,
	ParticleDrawState& outState
)
{
	float _t = (float)(currentTime - (float)drawe.stime);
	float _life = (float)((float)drawe.endtime - (float)drawe.stime);

	if (_life == 0) return;
	float _lifeper = (float)(_t / _life);

	unsigned long pseed = seedList[drawe.id % seedTableLen];
	unsigned long seed = (uint32_t)(pseed + (unsigned long)emitterSeed + (unsigned long)pid + (unsigned long)seedOffset);

	{
		xorshift32 rand;
		rand.init_genrand((unsigned long)seed);

		// 乱数の消費順序を維持するために、各要素の計算関数を元の順番で呼び出す
		float x, y;
		calcPositionAndSpeed(rand, particleParams, _t, _life, x, y);

		float ox, oy;
		calcOffset(rand, particleParams, ox, oy);

		outState.rotation = calcRotation(rand, particleParams, _t, _life);
		outState.color = calcColor(rand, particleParams, _lifeper);
		calcScale(rand, particleParams, _lifeper, outState.scaleX, outState.scaleY);

		outState.x = x + ox + position.x;
		outState.y = y + oy + position.y;

		if (particleParams.usePGravity) {
			applyPointGravity(particleParams, outState.x, outState.y, ox, oy, position, _t, _life);
		}
	}

	outState.direction = 0.0f;
	if (particleParams.useTurnDirec) {
		xorshift32 rand;
		rand.init_genrand((unsigned long)seed);

		float nextX, nextY;
		calculatePositionOnly(rand, particleParams, _t + 1.0f, _life, position, nextX, nextY);

		outState.direction = getAngle360(Vector2(1, 0), Vector2(outState.x - nextX, outState.y - nextY)) 
                             + (float)(degreeToRadian(90.0f) + degreeToRadian(particleParams.direcRotAdd));
	}
}

void EffectSimulator::calculateStateBatch(
	float currentTime,
	const ParticleExistSt drawe[ParticleStateBatch::BATCH_SIZE],
	const EmitterParameter& emitterParams,
	const ParticleParameter& particleParams,
	int32_t emitterSeed,
	int32_t seedOffset,
	const std::vector<unsigned long>& seedList,
	size_t seedTableLen,
	const Vector2& position,
	const int32_t pids[ParticleStateBatch::BATCH_SIZE],
	ParticleStateBatch& outBatch
)
{
	for (int32_t i = 0; i < ParticleStateBatch::BATCH_SIZE; ++i) {
		ParticleDrawState ds;
		calculateState(currentTime, drawe[i], emitterParams, particleParams, emitterSeed, seedOffset, seedList, seedTableLen, position, pids[i], ds);
		outBatch.x[i] = ds.x;
		outBatch.y[i] = ds.y;
		outBatch.scaleX[i] = ds.scaleX;
		outBatch.scaleY[i] = ds.scaleY;
		outBatch.rotation[i] = ds.rotation;
		outBatch.direction[i] = ds.direction;
		outBatch.color[i] = ds.color;
	}
}

} // namespace effect_v2

//現在時間から産出される位置を求める
//time変数から求められる式とする
//パーティクル座標計算のコア
void	SsEffectEmitter::updateParticle(float time, int32_t id, int32_t pid, int32_t seedOffset, int32_t stime, int32_t endtime, const effect_v2::Vector2& emitterPos, ParticleDrawData* p )
{
	effect_v2::ParticleDrawState ds;
	ParticleExistSt drawe;
	drawe.id = id;
	drawe.stime = stime;
	drawe.endtime = endtime;

	effect_v2::EffectSimulator::calculateState(
		time,
		drawe,
		this->emitter,
		this->particle,
		this->emitterSeed,
		seedOffset,
		*(this->seedList.get()),
		this->seedTableLen,
		emitterPos,
		pid,
		ds
	);

	p->x = ds.x;
	p->y = ds.y;
	p->rot = ds.rotation;
	p->direc = ds.direction;
	p->color = ds.color;
	p->scale.x = ds.scaleX;
	p->scale.y = ds.scaleY;
}


bool compare_life( const EmitPattern& left,  const EmitPattern& right)
{
	if ( left.life == right.life )
	{
        if ( left.uid < right.uid ) return true;

	}

	return left.life < right.life ;
}

void	SsEffectEmitter::precalculate2()
{
	rand.init_genrand( (unsigned long)emitterSeed );

	emitPatterns.clear();
	//_lifeExtend.clear();
	offsetPatterns.clear();

	if ( emitter.emitnum < 1 ) emitter.emitnum = 1;

	int32_t cycle =  (int32_t)(( (float)(emitter.emitmax * emitter.interval)  / (float)emitter.emitnum ) + 0.5f) ;
    int32_t group =  emitter.emitmax / emitter.emitnum;

	int32_t extendsize = emitter.emitmax*LIFE_EXTEND_SCALE;
    if ( extendsize < LIFE_EXTEND_MIN ) extendsize = LIFE_EXTEND_MIN;




	int32_t shot = 0;
	int32_t offset = (int32_t)(particle.delay);
	for ( int32_t i = 0 ; i < emitter.emitmax ; i++ )
	{
		if ( shot >= emitter.emitnum )
		{
			shot = 0;
			offset+= emitter.interval;
		}
		offsetPatterns.push_back(offset);
		shot++;
	}


	for ( int32_t i = 0 ; i < extendsize ; i++ )
	{
		EmitPattern e;
		e.uid = i;
		e.life = emitter.particleLife + (int32_t)(emitter.particleLife2 * rand.genrand_float32());
		e.cycle = cycle;

		if ( e.life > cycle )
		{
			e.cycle = e.life;
		}

		emitPatterns.push_back( e );
	}


	seedList.reset();

    particleListBufferSize = (size_t)emitter.emitmax;


	rand.init_genrand((unsigned long)(emitterSeed));

	seedTableLen = particleListBufferSize * 3;
	seedList.reset( new std::vector<unsigned long>(seedTableLen) );
	//各パーティクルＩＤから参照するシード値をテーブルとして作成する
	std::vector<unsigned long>&	seedListRaw = *(seedList.get());
	for ( size_t i = 0 ; i < seedTableLen ; i++ )
	{
    	seedListRaw[i] = rand.genrand_uint32();
	}
}



//----------------------------------------------------------------------------------




void SsEffectEmitter::updateEmitter( SsEffectEmitterState& state, float _time , int32_t slide ) 
{
	int32_t onum = (int32_t)(offsetPatterns.size());
	int32_t pnum = (int32_t)(emitPatterns.size());
	slide = slide * SEED_MAGIC;

	for ( int32_t i = 0 ; i < onum ; i ++ )
	{
		int32_t slide_num = ( i + slide ) % pnum;

		EmitPattern* targetEP = &emitPatterns[slide_num];

		float t = _time - (float)offsetPatterns[i];

		state.particleState.exist[i] = false;
		state.particleState.born[i] = false;

		if ( targetEP->cycle != 0 )
		{
			int32_t loopnum = (int32_t)(t / (float)targetEP->cycle);
			int32_t cycle_top = loopnum * targetEP->cycle;

			state.particleState.cycle[i] = loopnum;

			state.particleState.stime[i] = cycle_top + offsetPatterns[i];
			state.particleState.endtime[i] = state.particleState.stime[i] + targetEP->life;// + _lifeExtend[slide_num];

			if ( (float)state.particleState.stime[i] <= _time &&  (float)state.particleState.endtime[i] > _time )
			{
				state.particleState.exist[i] = true;
				state.particleState.born[i] = true;
			}

			if ( !this->emitter.Infinite )
			{
				if ( state.particleState.stime[i] >= this->emitter.life ) //エミッターが終了している
				{
					state.particleState.exist[i] = false;    //作られてない

					//最終的な値に計算し直し <-事前計算しておくといいかも・
					int32_t t_limit = this->emitter.life - offsetPatterns[i];
					int32_t loopnum_limit = t_limit / targetEP->cycle;

					int32_t cycle_top_limit = loopnum_limit * targetEP->cycle;

					state.particleState.stime[i] = cycle_top_limit + offsetPatterns[i];

					state.particleState.endtime[i] = state.particleState.stime[i] + targetEP->life;// + _lifeExtend[slide_num];
					state.particleState.born[i] = false;
				}else{
					state.particleState.born[i] = true;
				}
			}

			if ( t < 0.0f ){
				 state.particleState.exist[i] = false;
				 state.particleState.born[i] = false;
			}
		}
	}

}


void SsEffectRenderV2::particleDraw(int32_t emitterIndex, float time, int32_t parentIndex, ParticleDrawData* plp)
{
	float t = time;

	if (emitterIndex < 0 || emitterIndex >= (int32_t)emitterList.size()) return;
	SsEffectEmitter* e = emitterList[emitterIndex].get();
	if (e == nullptr) return;

	SsEffectEmitterState& emitterState = state.emitterStates[emitterIndex];

	int32_t pnum = e->getParticleIdMax();

	int slide = (parentIndex < 0) ? 0 : plp->id;

	e->updateEmitter(emitterState, time, slide);


	for (int32_t id = 0; id < pnum; id++)
	{
		if (!emitterState.particleState.born[id]) continue;

		float targettime = ((float)t + 0.0f);
		ParticleDrawData lp;
		ParticleDrawData pp;
		pp.x = 0; pp.y = 0;

		lp.id = id + emitterState.particleState.cycle[id];
		lp.stime = (int32_t)emitterState.particleState.stime[id];
		lp.lifetime = (int32_t)emitterState.particleState.endtime[id];
		lp.pid = 0;
		lp.rot = 0;

		if (parentIndex >= 0)lp.pid = plp->id;

		//if ( lp.stime == lp.lifetime ) continue;

		//if ( lp.stime <= targettime && lp.lifetime >= targettime)
		if (emitterState.particleState.exist[id])
		{

			if (parentIndex >= 0)
			{
				SsEffectEmitter* parentEmitter = emitterList[parentIndex].get();
				SsEffectEmitterState& parentEmitterState = state.emitterStates[parentIndex];
				if (parentEmitter)
				{
					//親から描画するパーティクルの初期位置を調べる
					pp.id = plp->id;
					pp.stime = plp->stime;
					pp.lifetime = plp->lifetime;
					pp.pid = plp->pid;
					//パーティクルが発生した時間の親の位置を取る

					#if 0 // memo: ptime は参照されない。2025/07/09
					int32_t ptime = lp.stime + pp.stime;
					if (ptime > lp.lifetime) ptime = lp.lifetime;
					#endif

					//逆算はデバッグしずらいかもしれない
					parentEmitter->updateParticle((float)lp.stime + pp.stime, 
						plp->id + parentEmitterState.particleState.cycle[plp->id], 
						plp->pid, 
						state.seedOffset,
						parentEmitterState.particleState.stime[plp->id], 
						parentEmitterState.particleState.endtime[plp->id], 
						parentEmitterState.position,
						&pp);
					emitterState.position.x = pp.x;
					emitterState.position.y = pp.y;
				}

			}


			e->updateParticle(targettime, 
				id + emitterState.particleState.cycle[id], 
				lp.pid, 
				state.seedOffset,
				emitterState.particleState.stime[id], 
				emitterState.particleState.endtime[id], 
				emitterState.position,
				&lp);

			if (_rendererDelegate)
			{
				effect_v2::ParticleDrawState pds;
				pds.x = lp.x * layoutScale.x;
				pds.y = lp.y * layoutScale.y;
				pds.scaleX = lp.scale.x;
				pds.scaleY = lp.scale.y;
				pds.rotation = lp.rot;
				pds.direction = lp.direc;

				float parentAlpha = (parentState) ? parentState->alpha : 1.0f;
				pds.color.a = (u8)((float)lp.color.a * parentAlpha);
                pds.color.r = lp.color.r;
                pds.color.g = lp.color.g;
                pds.color.b = lp.color.b;

				_rendererDelegate->drawSprite(e->dispCell.get(), pds, (int32_t)e->refData->BlendType);
			}


		}

	}



}



//パラメータをコピーする
void	SsEffectRenderV2::initEmitter( SsEffectEmitter* e , SsEffectNode* node)
{

	e->refData = node->GetMyBehavior();
	e->refCell = e->refData->refCell;

    if (!e->dispCell) e->dispCell = std::make_unique<SsCellValue>();

	//セルの初期化
	SsCelMapLinker* link = this->curCellMapManager->getCellMapLink( e->refData->CellMapName );

	if ( link )
	{
		SsCell * cell = link->findCell( e->refData->CellName );
		
		getCellValue(	this->curCellMapManager , 
			e->refData->CellMapName ,
			e->refData->CellName , 
			*e->dispCell ); 
	}else{
		DEBUG_PRINTF( "cell not found : %s , %s\n" , 
			e->refData->CellMapName.c_str(), 
			e->refData->CellName.c_str()
			);
	}

	SsEffectFunctionExecuter::initializeEffect( e->refData , e );

	e->emitterSeed = (int32_t)this->mySeed;

	if ( e->particle.userOverrideRSeed )
	{
		e->emitterSeed = e->particle.overrideRSeed;

	}else{
		if ( this->effectData->isLockRandSeed )
		{
			e->emitterSeed = (int32_t)(this->effectData->lockRandSeed+1) * SEED_MAGIC;
		}
	}

	e->emitter.life += (int32_t)e->particle.delay;//ディレイ分加算
}


void	SsEffectRenderV2::clearEmitterList()
{
	for ( size_t i = 0 ; i < this->emitterList.size(); i++)
	{
		emitterList[i].reset();
	}

	emitterList.clear();
	updateList.clear();
	state.emitterStates.clear();

}



void	SsEffectRenderV2::setEffectData(SsEffectModel* data)
{
	effectData = data;

    reload();

}


void	SsEffectRenderV2::update()
{

	if ( !state.isPlay ) return;

	state.targetFrame = state.currentFrame;

	if ( !this->Infinite )
	{
		if ( this->isLoop() ) //自動ループの場合
		{
			if ( state.currentFrame > (float)getEffectTimeLength() )
			{
				state.targetFrame = (float)((int32_t)state.currentFrame % getEffectTimeLength());
				int32_t l = (int32_t)( state.currentFrame / (float)(getEffectTimeLength()) );
				setSeedOffset( l );
			}
		}
	}

}

void	SsEffectRenderV2::draw()
{
    if ( state.currentFrame < 0 ) return;

	for ( size_t i = 0 ; i < updateList.size() ; i++ )
	{
		SsEffectEmitter* e = updateList[i];
		if ( e )
		{
			// Seed offset is now in state.seedOffset, and it's passed down to particleDraw
		}
	}

	for ( size_t i = 0 ; i < updateList.size() ; i++ )
	{
		SsEffectEmitter* e = updateList[i];

		if ( e->parentIndex > 0 )
		{
			SsEffectEmitter* parentEmitter = emitterList[e->parentIndex].get();
			SsEffectEmitterState& parentEmitterState = state.emitterStates[e->parentIndex];
			if (parentEmitter)
			{
				//グローバルの時間で現在親がどれだけ生成されているのかをチェックする
				parentEmitter->updateEmitter(parentEmitterState, state.targetFrame, 0);

				int32_t loopnum = parentEmitter->getParticleIdMax();
				for (int32_t n = 0; n < loopnum; n++)
				{
					if (parentEmitterState.particleState.born[n])
					{
						ParticleDrawData lp;
						lp.stime = (int32_t)parentEmitterState.particleState.stime[n];
						lp.lifetime = (int32_t)parentEmitterState.particleState.endtime[n];
						lp.id = n;
						lp.pid = 0;

						float targettime = (state.targetFrame + 0.0f);
						float ptime = (targettime - (float)lp.stime);

						particleDraw(e->uid, ptime, e->parentIndex, &lp);
					}
				}
			}
		}else{
			particleDraw( e->uid , state.targetFrame );
		}
	}

}


bool compare_priority( SsEffectEmitter* left,  SsEffectEmitter* right)
{
	if ( left->priority == right->priority )
	{
		if ( left->uid < right->uid ) return true;
	}
  //	return true;
  return left->priority < right->priority ;
}


void    SsEffectRenderV2::setup(std::vector<std::unique_ptr<SsEffectEmitter>> emitters, bool isInfiniteParam)
{
	state.currentFrame = 0;
	stop();
	clearEmitterList();

	emitterList = std::move(emitters);
	Infinite = isInfiniteParam;

	state.emitterStates.resize(emitterList.size());

	effectTimeLength = 0;
	for (size_t i = 0; i < this->emitterList.size(); i++)
	{
		SsEffectEmitter* emitter = emitterList[i].get();
		if (emitter != nullptr)
		{
			emitter->uid = (int32_t)i;
			emitter->precalculate2();
			state.emitterStates[i].resize((size_t)emitter->emitter.emitmax);

			int32_t pi = emitter->parentIndex;
			if (pi == 0) // ルート直下
			{
				emitter->globalTime = emitter->getTimeLength();
				updateList.push_back(emitter);
			}
			else if (pi >= 0 && pi < (int32_t)emitterList.size())
			{
				SsEffectEmitter* emitterPI = emitterList[pi].get();
				emitter->globalTime = (int32_t)(emitter->getTimeLength()) + (int32_t)(emitterPI->getTimeLength());
				updateList.push_back(emitter);
			}

			if (emitter->globalTime > effectTimeLength)
			{
				effectTimeLength = emitter->globalTime;
			}
		}
	}
	//プライオリティソート
	std::sort(updateList.begin(), updateList.end(), compare_priority);
}

void    SsEffectRenderV2::reload()
{
	if (!effectData) return;

	//Modelに記載されているエミッターのリストを一時的に作成
	const std::vector<SsEffectNode*>& list = this->effectData->getNodeList();
	int32_t cnum_count = (int32_t)list.size();
	std::unique_ptr<int32_t[]> cnum(new int32_t[cnum_count]);
	for (int32_t i = 0; i < cnum_count; i++) cnum[i] = 0;

	std::vector<std::unique_ptr<SsEffectEmitter>> tempEmitters;
	bool _Infinite = false;

	layoutScale.x = (float)(this->effectData->layoutScaleX) / 100.0f;
	layoutScale.y = (float)(this->effectData->layoutScaleY) / 100.0f;

	for (size_t i = 0; i < (size_t)cnum_count; i++)
	{
		SsEffectNode* node = list[i];
		if (node->GetType() == SsEffectNodeType::emmiter)
		{
			std::unique_ptr<SsEffectEmitter> e(new SsEffectEmitter());
			e->parentIndex = node->parentIndex;
			if (e->parentIndex != 0)
			{
				e->parentIndex = list[e->parentIndex]->parentIndex;
			}

			cnum[e->parentIndex]++;
			if (cnum[e->parentIndex] > 10)
			{
				_isWarningData = true;
				tempEmitters.push_back(nullptr);
				continue;
			}

			if (e->parentIndex != 0)
			{
				int32_t a = list[e->parentIndex]->parentIndex;
				if (a != 0 && list[a]->parentIndex > 0)
				{
					_isWarningData = true;
					tempEmitters.push_back(nullptr);
					continue;
				}
			}

			initEmitter(e.get(), node);
			if (e->emitter.Infinite) _Infinite = true;
			tempEmitters.push_back(std::move(e));
		}
		else
		{
			tempEmitters.push_back(nullptr);
		}
	}

	setup(std::move(tempEmitters), _Infinite);
}


int32_t  SsEffectRenderV2::getEffectTimeLength()
{

	return effectTimeLength;
}


int32_t	SsEffectRenderV2::getCurrentFps(){
	if (effectData)
	{
		if ( effectData->fps == 0 ) return 30;

		return (int32_t)effectData->fps;
	}
	return 30;
}

}	// namespace spritestudio6
