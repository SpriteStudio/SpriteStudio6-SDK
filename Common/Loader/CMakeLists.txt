include(${CMAKE_CURRENT_SOURCE_DIR}/../../cmake/initialize.cmake)
cmake_minimum_required(VERSION 3.19.2)

project(ssLoader)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

OPTIONS()

CCACHE()

PREPARE()

set(ssLoader_SRCS
    tinyxml2/tinyxml2.cpp
    ssloader_sspj.cpp
    ssloader_ssae.cpp
    ssloader_ssce.cpp
    ssloader_ssee.cpp
    ssloader_ssqe.cpp
    ssloader_ssse.cpp
    SsEffectBehavior.cpp
    SsEffectElement.cpp
    ssstring_uty.cpp
    ssarchiver.cpp
    sstypes.cpp
    ssvalue.cpp
    ssInterpolation.cpp
    ssattribute.cpp
    sscharconverter.cpp
    sscharmap.cpp


partvalues/sspartvalue.cpp
partvalues/sspartvalueshape.cpp
partvalues/sspartvaluetext.cpp
partvalues/sspartvaluenines.cpp

shape/sspolygon.cpp
shape/ssvertex.cpp

shape/ssshape.cpp
shape/ssshapearrow.cpp
shape/ssshaperectangle.cpp
shape/ssshapestar.cpp
shape/ssshapetriangle.cpp

nineslice/ssnines.cpp

text/ssfontdesc.cpp
)
#if(APPLE AND (${CMAKE_SYSTEM_NAME} MATCHES "Darwin"))
#    set(ssLoader_SRCS ${ssLoader_SRCS} sscharconverter_mac.mm)
#endif()

set(ssLoader_HEADERS
    ssloader.h
    SsEffectBehavior.h
    SsEffectElement.h
    ssarchiver.h
    ssloader_sspj.h
    ssloader_ssae.h
    ssloader_ssce.h
    ssloader_ssee.h
    ssloader_ssqe.h
    ssloader_ssse.h
    sstypes.h
    ssvalue.h
    ssInterpolation.h
    ssattribute.h
    ssstring_uty.h
#    ${CMAKE_CURRENT_SOURCE_DIR}/../Animator/ssplayer_animedecode.h
    sscharconverter.h
    sscharmap.h

partvalues/sspartvalue.h
partvalues/sspartvalueshape.h
partvalues/sspartvaluetext.h
partvalues/sspartvaluenines.h


shape/sspolygon.h
shape/ssvertex.h
shape/ssshape.h
shape/ssshapearrow.h
shape/ssshaperectangle.h
shape/ssshapestar.h
shape/ssshapetriangle.h
nineslice/ssnines.h

text/ssfontdesc.h

)

include_directories(${CMAKE_CURRENT_SOURCE_DIR})
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../Animator)


#if(APPLE AND (${CMAKE_SYSTEM_NAME} MATCHES "Darwin"))
#    set(CMAKE_OBJC_FLAGS "-x objective-c++")
#endif()
ADD_LIBRARY(ssLoader ${ssLoader_SRCS} ${ssLoader_HEADERS})
