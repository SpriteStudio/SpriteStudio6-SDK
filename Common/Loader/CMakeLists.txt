cmake_minimum_required(VERSION 3.11)

project(ssLoader)

set(ssLoader_SRCS
    tinyxml2/tinyxml2.cpp
    babel/babel.cpp
    ssloader_sspj.cpp
    ssloader_ssae.cpp
    ssloader_ssce.cpp
    ssloader_ssee.cpp
    SsEffectBehavior.cpp
    SsEffectElement.cpp
    ssstring_uty.cpp
    ssarchiver.cpp
    sstypes.cpp
    ssvalue.cpp
    ssInterpolation.cpp
    ssattribute.cpp
)

set(ssLoader_HEADERS
    babel/babel.h
    ssloader.h
    SsEffectBehavior.h
    SsEffectElement.h
    ssarchiver.h
    ssloader_sspj.h
    ssloader_ssae.h
    ssloader_ssce.h
    ssloader_ssee.h
    sstypes.h
    ssvalue.h
    ssInterpolation.h
    ssattribute.h
    ssstring_uty.h
    ${CMAKE_CURRENT_SOURCE_DIR}/../Animator/ssplayer_animedecode.h
)

include_directories(${CMAKE_CURRENT_SOURCE_DIR})
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../Animator)

ADD_LIBRARY(ssLoader ${ssLoader_SRCS} ${ssLoader_HEADERS})
