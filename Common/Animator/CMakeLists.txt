include(${CMAKE_CURRENT_SOURCE_DIR}/../../cmake/initialize.cmake)
cmake_minimum_required(VERSION 3.19.2)

project(ssAnimator)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

OPTIONS()

CCACHE()

PREPARE()

set(ssAnimator_SRCS
    ssplayer_cellmap.cpp
    ssplayer_PartState.cpp
    ssplayer_render.cpp
    ssplayer_matrix.cpp
    ssplayer_effect.cpp
    ssplayer_effectfunction.cpp
    ssplayer_animedecode.cpp
    ssplayer_effect2.cpp
    ssplayer_mesh.cpp
    ssplayer_math.cpp
    ssplayer_fonttexmng.cpp
#    partvalues/sspartvalue.cpp
#    partvalues/sspartvalueshape.cpp
#    partvalues/sspartvaluetext.cpp
#    partvalues/sspartvaluenines.cpp

#shape/sspolygon.cpp
#shape/ssvertex.cpp

#shape/ssshape.cpp
#shape/ssshapearrow.cpp
#shape/ssshaperectangle.cpp
#shape/ssshapestar.cpp
#shape/ssshapetriangle.cpp

#nineslice/ssnines.cpp
font/ssfonttexture.cpp

)

set(ssAnimator_HEADERS
    ssplayer_types.h
    ssplayer_cellmap.h
    ssplayer_PartState.h
    ssplayer_render.h
    ssplayer_matrix.h
    ssplayer_effect.h
    ssplayer_effectfunction.h
    MersenneTwister.h
    ssplayer_animedecode.h
    ssplayer_effect2.h
    ssplayer_mesh.h
    ssplayer_math.h
    ${CMAKE_CURRENT_SOURCE_DIR}/../Loader/sstypes.h

    ssplayer_fonttexmng.h

#    partvalues/sspartvalue.h
#    partvalues/sspartvalueshape.h
#    partvalues/sspartvaluetext.h
#   partvalues/sspartvaluenines.h

#shape/sspolygon.h
#shape/ssvertex.h

#shape/ssshape.h
#shape/ssshapearrow.h
#shape/ssshaperectangle.h
#shape/ssshapestar.h
#shape/ssshapetriangle.h
#nineslice/ssnines.h
font/ssfonttexture.h


)

include_directories(${CMAKE_CURRENT_SOURCE_DIR})
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../Loader)

add_library(ssAnimator ${ssAnimator_SRCS} ${ssAnimator_HEADERS})
