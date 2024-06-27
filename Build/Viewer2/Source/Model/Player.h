/*
  ==============================================================================

    renderAnimation.h
    Created: 26 Jan 2018 5:46:29pm
    Author:  kurooka

  ==============================================================================
*/

#pragma once
#include "../JuceLibraryCode/JuceHeader.h"

namespace spritestudio6 {
class SsProject;
class SsAnimePack;
class SsAnimeDecoder;
class SsCellMapList;
class SSTextureFactory;
}  // namespace spritestudio6

class Player : public juce::HighResolutionTimer {
    void draw();
    void tick();
    void hiResTimerCallback() override;

    static Player* myInst;

    class State {
       public:
        State() {};
        virtual ~State() {};
        static Value packIndex;
        static Value animeIndex;
        static Value animeName;
        static Value startFrame;
        static Value endFrame;
        static Value length;
        static Value frame;
        static Value fps;
        static Value loop;

       protected:
        friend class Player;
        // デフォルトの実装
        virtual void start(Player* p);
        virtual void stop(Player* p);
        virtual void reset(Player* p);
        virtual void hiResTimerCallback(Player* p);
        virtual void draw(Player* p);
        virtual void loadProj(Player* p, const String& name);
        virtual void loadAnime(Player* p, int packIndex, int animeIndex);
        virtual void onEnter(Player* p) {};
        virtual void onLeave(Player* p) {};
        JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(State)
    };

    State* currentState;
    void changeState(State* newState);

    class StatePlaying : public State {
       public:
        StatePlaying() {};
        void start(Player* p) override {}
        void loadProj(Player* p, const String& name) override {}
        void loadAnime(Player* p, int packIndex, int animeIndex) override {}
        JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(StatePlaying)
    };

    class StatePaused : public State {
       public:
        StatePaused() {};
        void stop(Player* p) override {}
        void hiResTimerCallback(Player* p) override {}
        JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(StatePaused)
    };

    class StateLoading : public State {
       public:
        StateLoading() {};
        void start(Player* p) override {}
        void stop(Player* p) override {}
        void reset(Player* p) override {}
        void hiResTimerCallback(Player* p) override {}
        void draw(Player* p) {};
        void loadProj(Player* p, const String& name) override {}
        void loadAnime(Player* p, int packIndex, int animeIndex) override {}
        JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(StateLoading)
    };

    class StateInitial : public State {
       public:
        StateInitial() {};
        void start(Player* p) override {}
        void stop(Player* p) override {}
        void reset(Player* p) override {}
        void hiResTimerCallback(Player* p) override {}
        void draw(Player* p) {};
        JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(StateInitial)
    };

   public:
    Player();
    virtual ~Player();

    static Player* get();

    void play();
    void stop();
    void reset();
    void loadProj(const String& name);
    void loadAnime(int packIndex, int animeIndex);
    State* getState();
    static void drawAnime();

    // アニメーションの状態
    std::unique_ptr<StatePlaying> statePlaying;
    std::unique_ptr<StatePaused> statePaused;
    std::unique_ptr<StateLoading> stateLoading;
    std::unique_ptr<StateInitial> stateInitial;
    std::unique_ptr<spritestudio6::SsProject> currentProj;
    std::unique_ptr<spritestudio6::SsAnimeDecoder> decoder;
    spritestudio6::SsCellMapList* cellmap = nullptr;  // decoderのデストラクタでdeleteされる

    friend class AsyncAnimeLoader;
    friend class AsyncProjectLoader;

    JUCE_DECLARE_NON_COPYABLE_WITH_LEAK_DETECTOR(Player)
};
