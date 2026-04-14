#ifndef __SSEFFECTRENDER_V3__
#define __SSEFFECTRENDER_V3__


#include "xorshift32.h"

#include <memory>
#include <utility>
#include <vector>
#include <algorithm>
#include <cstdint>
#include <cmath>

// SDK固有型のインクルード（互換性のため）
#include "ssplayer_types.h"

// MEMO: コンパイル設定
#define SPRITESTUDIO6SDK_LOOP_TYPE1 (0)
#define SPRITESTUDIO6SDK_LOOP_TYPE2 (0)
#define SPRITESTUDIO6SDK_LOOP_TYPE3 (1)

namespace spritestudio6
{

// 移植用の基本型定義
namespace effect_v2 {
    struct Vector2 {
        float x, y;
        Vector2() : x(0.0f), y(0.0f) {}
        Vector2(float x, float y) : x(x), y(y) {}
        // 互換性用
        Vector2(const SsVector2& v) : x(v.x), y(v.y) {}
        Vector2& operator=(const SsVector2& v) { x = v.x; y = v.y; return *this; }
    };

    struct Vector3 {
        float x, y, z;
        Vector3() : x(0.0f), y(0.0f), z(0.0f) {}
        Vector3(float x, float y, float z) : x(x), y(y), z(z) {}
        // 互換性用
        Vector3(const SsVector3& v) : x(v.x), y(v.y), z(v.z) {}
        Vector3& operator=(const SsVector3& v) { x = v.x; y = v.y; z = v.z; return *this; }
    };

    struct Color {
        uint8_t a, r, g, b;
        Color() : a(255), r(255), g(255), b(255) {}
        Color(uint8_t a, uint8_t r, uint8_t g, uint8_t b) : a(a), r(r), g(g), b(b) {}
        // 互換性用
        Color(const SsU8Color& c) : a(c.a), r(c.r), g(c.g), b(c.b) {}
        Color& operator=(const SsU8Color& c) { a = c.a; r = c.r; g = c.g; b = c.b; return *this; }
    };
}

class SsEffectModel;
class SsRenderEffectBase;
class SsEffectNode;
struct SsPartState;
class SsEffectRenderAtom;
class SsCell;
class SsEffectBehavior;
class SsCellMapList;
struct SsCellValue;


constexpr int32_t SEED_MAGIC = 7573;
constexpr int32_t LIFE_EXTEND_SCALE = 8;
constexpr int32_t LIFE_EXTEND_MIN = 64;


struct TimeAndValue
{
	float time;
	float value;
};


// v3.1 SoA structure for particle existence
struct ParticleExistSoA
{
	std::vector<int32_t> id;
	std::vector<int32_t> cycle;
	std::vector<int32_t> exist;
	std::vector<int32_t> born;
	std::vector<int32_t> stime;
	std::vector<int32_t> endtime;

	void resize(size_t size)
	{
		id.resize(size);
		cycle.resize(size);
		exist.resize(size);
		born.resize(size);
		stime.resize(size);
		endtime.resize(size);
	}

	void cleanup(size_t index)
	{
		id[index] = 0;
		cycle[index] = 0;
		exist[index] = 0;
		born[index] = 0;
		stime[index] = 0;
		endtime[index] = 0;
	}
};

// v2.1 Emitter mutable state
struct SsEffectEmitterState
{
	int32_t seedOffset;
	effect_v2::Vector2 position;
	ParticleExistSoA particleState;

	SsEffectEmitterState() : seedOffset(0), position(0, 0) {}

	void resize(size_t size) {
		particleState.resize(size);
	}
};

//v3.1
struct ParticleExistSt
{
	int32_t  id;
	int32_t	 cycle;
	int32_t	 exist;
    int32_t	 born;
	int32_t  stime;
	int32_t  endtime;

	inline void cleanup()
	{
		id = 0;
		cycle = 0;
		exist = 0;
		born = 0;
		stime = 0;
		endtime = 0;
	}
};


//v3.1
struct EmitPattern
{
	int32_t	  uid;
	int32_t   life;
    int32_t   cycle;
};

//最終描画用データ
struct ParticleDrawData
{
	int32_t	id;
	int32_t	pid;
	int32_t	stime;		//生成された開始時間
	int32_t lifetime;

	//描画用ワーク
	float	x;
	float	y;
	float	rot;
	float	direc;

    effect_v2::Color color;
    effect_v2::Vector2 scale;
};

// 前方宣言
struct EmitterParameter;
struct ParticleParameter;

namespace effect_v2 {

// パーティクルの描画に必要な結果データ
struct ParticleDrawState {
    float x, y;
    float scaleX, scaleY;
    float rotation;
    float direction;
    Color color;
};

// SIMD/SoAを見据えたバッチ処理用データ構造
struct ParticleStateBatch {
    static constexpr int32_t BATCH_SIZE = 4; // SIMD 128bit (float x 4) を想定
    
    // 4パーティクル分のデータを一括保持
    float x[BATCH_SIZE];
    float y[BATCH_SIZE];
    float scaleX[BATCH_SIZE];
    float scaleY[BATCH_SIZE];
    float rotation[BATCH_SIZE];
    float direction[BATCH_SIZE];
    Color color[BATCH_SIZE];
};

class IEffectRendererDelegate {
public:
    virtual ~IEffectRendererDelegate() = default;
    
    // 計算済みの状態を受け取り、実際の描画APIを叩く
    virtual void drawSprite(
        const SsCellValue* dispCell,
        const ParticleDrawState& state,
        int32_t blendType // SDKのEnumから独立させるためint32_tに
    ) = 0;
};

class EffectSimulator {
public:
    // 単一パーティクルの計算（純粋関数）
    static void calculateState(
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
    );

    // SIMD最適化用：複数パーティクルの同時計算（インターフェースのみ準備）
    static void calculateStateBatch(
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
    );
};

} // namespace effect_v2


//エミッターが持つパラメータ
//エディタやファイルから入力される
struct EmitterParameter
{
	int32_t	life;
	int32_t interval;
	int32_t	emitnum;		//一度に排出される個数
	int32_t emitmax;		//最大排出数
	int32_t	particleLife;	//生成されるパーティクルの寿命
	int32_t particleLife2;	//寿命最大値
    bool Infinite;      // 元に戻す: Infinite

	int32_t	loopStart;
	int32_t loopEnd;
	int32_t loopLen;
	int32_t	loopGen;


	//テスト用デフォルト
	EmitterParameter() : life(15),interval(1),emitnum(2),emitmax(32),particleLife(15),particleLife2(15),Infinite(false){}

};


//パーティクルが持つパラメータ
//エディタやファイルから入力される
struct ParticleParameter
{

	effect_v2::Vector2 	scale;

	effect_v2::Color   startcolor; //スタート時のカラー
	effect_v2::Color   endcolor;   //終了時のカラー

	//初速
	float		speed;		//初速
	float		speed2;		//初速最大値


	float		angle;		  //向いてる角度
	float       angleVariance;// 元に戻す: angleVariance

	bool		useGravity;
	effect_v2::Vector2	gravity;


	bool		useOffset;
	effect_v2::Vector2   offset;
	effect_v2::Vector2   offset2;

	bool        useRotation;
	float		rotation;
	float		rotation2;

	float		rotationAdd;
	float		rotationAdd2;

	bool		useRotationTrans;
	float		rotationFactor;
	float		endLifeTimePer;

	bool        useTanAccel;
	float		tangentialAccel;
	float		tangentialAccel2;

	bool        useColor;
	effect_v2::Color   initColor;
	effect_v2::Color   initColor2;

	bool		useTransColor;
	effect_v2::Color   transColor;
	effect_v2::Color   transColor2;

	bool        useInitScale;
	effect_v2::Vector2   scaleRange;
	float		scaleFactor;
	float		scaleFactor2;

	bool        useTransScale;
	effect_v2::Vector2   transscale;
	effect_v2::Vector2   transscaleRange; // リネーム戻し: transscaleRange
	float		transscaleFactor;
	float		transscaleFactor2;

	float		delay;

	bool		usePGravity;
	effect_v2::Vector2	gravityPos;
	float		gravityPower;

	bool		useAlphaFade;
	float       alphaFade;
	float       alphaFade2;

	bool		useTransSpeed;
	float	   	transSpeed;
	float	   	transSpeed2;

	bool		useTurnDirec;
    float		direcRotAdd;

    bool		userOverrideRSeed;
	int32_t		overrideRSeed;


	ParticleParameter(): delay(0.0f), overrideRSeed(0), userOverrideRSeed(false) {}
};


//エミッタ動作クラス
class SsEffectEmitter
{
public:
	// SsCellValueはまだ残すが、将来的にID化を検討
	std::unique_ptr<SsCellValue> dispCell;


	int32_t				priority;

	EmitterParameter  	emitter;
	ParticleParameter   particle;
	xorshift32			rand;


	int32_t				emitterSeed;

	//生成用のリングバッファ
	std::vector<EmitPattern>    	emitPatterns;
	std::vector<int32_t>			offsetPatterns;
	
	//事前計算バッファ
	int32_t						particleIdMax;

	size_t						particleListBufferSize;
	std::unique_ptr<std::vector<unsigned long>>	seedList;

    int32_t						parentIndex;

	SsCell*						refCell;    //描画用セル
	SsEffectBehavior*           refData;	//データ更新用

	int32_t						globalTime;
	size_t						seedTableLen;

	int32_t						uid;

public:
	SsEffectEmitter() :
			parentIndex(-1),
			seedList(),
			particleListBufferSize(180*100),  //生成出来るパーティクルの最大値
			globalTime(0)
	{
		emitterSeed = SEED_MAGIC;
	}
	virtual ~SsEffectEmitter()
	{
		seedList.reset();
	}

#if  SPRITESTUDIO6SDK_LOOP_TYPE3

	int32_t	getParticleIdMax() const { return (int32_t)(offsetPatterns.size()); }

	void	updateEmitter( SsEffectEmitterState& state, float time  , int32_t slide );

#else

	int32_t	getParticleIdMax() const { return particleIdMax; }
#endif


	int32_t	getTimeLength() const { return emitter.life + ( emitter.particleLife + emitter.particleLife2); }

	//現在時間から産出される位置を求める
	//time変数から求められる式とする
	void	updateParticle(float time, int32_t id, int32_t pid, int32_t seedOffset, int32_t stime, int32_t endtime, const effect_v2::Vector2& emitterPos, ParticleDrawData* p );

	//パーティクルの発生間隔を事前計算する
	//ここで出力が確定する

	void	precalculate2();

};

// v2.1 Render mutable state
struct SsEffectRenderStateV2
{
	float			currentFrame;
	float			targetFrame;
	int32_t			seedOffset;
	bool			isPlay;
	bool			isPause;
	bool			isLoop;

	std::vector<SsEffectEmitterState> emitterStates;

	SsEffectRenderStateV2() : currentFrame(0), targetFrame(0), seedOffset(0), isPlay(false), isPause(false), isLoop(false) {}
};

class SsEffectRenderV2
{
public:

	//エフェクトのパラメータデータ
	SsEffectModel*		effectData;

	//Modelに記載されているエミッタのリスト
	std::vector<std::unique_ptr<SsEffectEmitter>>	emitterList;

	//MEMO: updateListは、更新用でemitterListの内容への参照なのでスマートポインタ化しない（所有権を保持しない）
	std::vector<SsEffectEmitter*>   updateList;

	//ランダムシード
	uint32_t		mySeed;

	effect_v2::Vector3		layoutPosition;
	effect_v2::Vector2		layoutScale;

	SsEffectRenderStateV2 state;

	int32_t          effectTimeLength;

    bool			Infinite;	// 元に戻す: Infinite

	SsPartState*	parentState;

	bool			isIntegerFrame;

	SsCellMapList*	curCellMapManager;/// セルマップのリスト（アニメデコーダーからもらう

	bool		_isWarningData;

	effect_v2::IEffectRendererDelegate* _rendererDelegate;
public:


protected:
	void 	particleDraw(int32_t emitterIndex , float t , int32_t parentIndex = -1 , ParticleDrawData* plp = 0 );
	void	initEmitter( SsEffectEmitter* e , SsEffectNode* node);

	void	clearEmitterList();

public:
	SsEffectRenderV2() : effectTimeLength(0) ,isIntegerFrame(true), _rendererDelegate(nullptr)
	{
		mySeed = 0;
	}
	virtual ~SsEffectRenderV2()
	{
		clearEmitterList();
	}

	void setRendererDelegate(effect_v2::IEffectRendererDelegate* delegate) { _rendererDelegate = delegate; }

	virtual void    play(){ state.isPause = false; state.isPlay = true; }
	virtual void	stop(){ state.isPlay = false;}
	virtual void    pause(){ state.isPause = true; state.isPlay = false;}
	virtual void	setLoop(bool flag){ state.isLoop = flag; }
	virtual bool	isPlay(){return state.isPlay;}
	virtual bool	isPause(){return state.isPause;}
	virtual bool	isLoop(){return state.isLoop;}

	virtual void	setEffectData(SsEffectModel* data);


	virtual void	setSeed( uint32_t seed )
	{
   		mySeed = seed * (uint32_t)SEED_MAGIC;
	}


	virtual void	setFrame( float frame )
	{
    	state.currentFrame = frame;
	}

    virtual float	getFrame(){ return state.currentFrame; }

	virtual void	update();
	virtual void	draw();

	virtual void    setup(std::vector<std::unique_ptr<SsEffectEmitter>> emitters, bool isInfinite);
	virtual void    reload();

    virtual int32_t  getEffectTimeLength();

//	virtual int		getVersion(){ return EFFECTRENDERVERSION_V2; }

	virtual  void	setParentAnimeState( SsPartState* state ){ parentState = state; }

	virtual int32_t	getCurrentFps();

	void	setCellmapManager( SsCellMapList* plist ) { curCellMapManager = plist; }

	bool	getPlayStatus(void){
		return(state.isPlay);
	}

	void	setSeedOffset( int32_t offset ) { 
		if ( effectData->isLockRandSeed )
		{
			state.seedOffset = 0;
		}else{
			state.seedOffset = offset; 
		}
	}

	virtual bool	isInfinity(){ return Infinite; }
    virtual bool	isWarning(){ return _isWarningData; }

};

}	// namespace spritestudio6

#endif
