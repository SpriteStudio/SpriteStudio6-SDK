cmake_minimum_required(VERSION 3.11)

project(viewer_sample)

if(NOT TARGET ssHelper)
    add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/../../Common viewer_sample_Common)
endif()
add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/../Depends viewer_sample_Depends)

set(viewer_sample_common_SRCS
    ./source/myscene.cpp
)

set(viewer_sample_common_HEADERS
    ./source/myscene.h
)

set(viewer_sample_gl_SRCS
    ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper/OpenGL/SSTextureGL.cpp
    ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Drawer/ssplayer_render_gl.cpp
    ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Drawer/ssplayer_shader_gl.cpp
    ./source/main_gl.cpp
)

set(viewer_sample_gl_HEADERS
    ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper/OpenGL/SSTextureGL.h
    ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Drawer/ssplayer_render_gl.h
    ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Drawer/ssplayer_shader_gl.h
)

set(viewer_sample_dx9_SRCS
    ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper/DirectX/d3dsimple.cpp
    ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper/DirectX/SSTextureDX9.cpp
    ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Drawer/ssplayer_render_dx9.cpp
    ./source/main_dx9.cpp
)

set(viewer_sample_dx9_HEADERS
    ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper/DirectX/d3dsimple.h
    ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper/DirectX/SSTextureDX9.h
    ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Drawer/ssplayer_render_dx9.h
)

include_directories(${CMAKE_CURRENT_SOURCE_DIR})
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Loader)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Animator)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Drawer)

add_executable(viewer_sample_gl ${viewer_sample_common_SRCS}
                                ${viewer_sample_common_HEADERS}
                                ${viewer_sample_gl_SRCS}
                                ${viewer_sample_gl_HEADERS}
                                )
target_link_libraries(viewer_sample_gl ssLoader)
target_link_libraries(viewer_sample_gl ssAnimater)
target_link_libraries(viewer_sample_gl ssHelper)

if(MSVC)
    add_executable(viewer_sample_dx9 ${viewer_sample_common_SRCS}
                                     ${viewer_sample_common_HEADERS}
                                     ${viewer_sample_dx9_SRCS}
                                     ${viewer_sample_dx9_HEADERS}
                                     )
endif()

if(WIN32)
    SET_TARGET_PROPERTIES (viewer_sample_gl PROPERTIES LINK_FLAGS_DEBUG "/SUBSYSTEM:windows")
    SET_TARGET_PROPERTIES (viewer_sample_gl PROPERTIES LINK_FLAGS_RELEASE "/SUBSYSTEM:windows")
    SET_TARGET_PROPERTIES (viewer_sample_gl PROPERTIES LINK_FLAGS_MINSIZEREL "/SUBSYSTEM:windows")
    SET_TARGET_PROPERTIES (viewer_sample_gl PROPERTIES LINK_FLAGS_RELWITHDEBINFO "/SUBSYSTEM:windows")

    SET_TARGET_PROPERTIES (viewer_sample_dx9 PROPERTIES LINK_FLAGS_DEBUG "/SUBSYSTEM:windows")
    SET_TARGET_PROPERTIES (viewer_sample_dx9 PROPERTIES LINK_FLAGS_RELEASE "/SUBSYSTEM:windows")
    SET_TARGET_PROPERTIES (viewer_sample_dx9 PROPERTIES LINK_FLAGS_MINSIZEREL "/SUBSYSTEM:windows")
    SET_TARGET_PROPERTIES (viewer_sample_dx9 PROPERTIES LINK_FLAGS_RELWITHDEBINFO "/SUBSYSTEM:windows")
    ADD_DEFINITIONS(-_WIN32)
elseif(APPLE AND (${CMAKE_SYSTEM_NAME} MATCHES "Darwin"))
    SET(LINK_LIB4 "-lAntTweakBar -lglfw.3 -framework OpenGL -framework AppKit -framework IOKit" )
    ADD_DEFINITIONS(-APPLE)
    target_link_libraries(viewer_sample_gl ${LINK_LIB4})
endif()
