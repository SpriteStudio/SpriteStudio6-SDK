cmake_minimum_required(VERSION 3.11)

project(scriptbase_converter)

if(NOT TARGET ssHelper)
    add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/../../Common scriptbase_converter_Common)
endif()
add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/../Depends scriptbase_converter_Depends)

set(scriptbase_converter_SRCS
    script_base_conv.cpp
    bind_SSXML.cpp
    bind_Animation.cpp
    bind_SsPart.cpp
    bind_SsPartAnime.cpp
    bind_SsAnimeDecoder.cpp
    bind_AnimePack.cpp
    bind_SsAttribute.cpp
    bind_SsKeyframe.cpp
    package_SpriteStudio.cpp
    bind_Cellmap.cpp
)

set(scriptbase_converter_HEADERS
    bind_SSXML.h
    bind_Animation.h
    bind_SsPart.h
    bind_SsPartAnime.h
    bind_SsAnimeDecoder.h
    bind_AnimePack.h
    bind_SsAttribute.h
    bind_SsKeyframe.h
    package_SpriteStudio.h
    bind_Cellmap.h
)

include_directories(${CMAKE_CURRENT_SOURCE_DIR})
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Loader)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Animator)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper)

add_executable(scriptbase_converter ${scriptbase_converter_SRCS} ${scriptbase_converter_HEADERS})
target_link_libraries(scriptbase_converter ssLoader)
target_link_libraries(scriptbase_converter ssAnimater)
target_link_libraries(scriptbase_converter ssHelper)
