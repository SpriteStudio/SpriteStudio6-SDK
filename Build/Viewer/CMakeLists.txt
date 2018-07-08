cmake_minimum_required(VERSION 3.11)

project(viewer_sample)

set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# command options
option(ENABLE_CCACHE "Enable ccache?" ON)
# enable ccache
find_program(CCACHE_EXE ccache)
if(CCACHE_EXE)
    if(ENABLE_CCACHE)
        message(STATUS "Enable ccache")
        if(CMAKE_C_COMPILER_LAUNCHER)
            set(CMAKE_C_COMPILER_LAUNCHER "${CMAKE_C_COMPILER_LAUNCHER}" "${CCACHE_EXE}")
        else()
            set(CMAKE_C_COMPILER_LAUNCHER "${CCACHE_EXE}")
        endif()
        if(CMAKE_CXX_COMPILER_LAUNCHER)
            set(CMAKE_CXX_COMPILER_LAUNCHER "${CMAKE_CXX_COMPILER_LAUNCHER}" "${CCACHE_EXE}")
        else()
            set(CMAKE_CXX_COMPILER_LAUNCHER "${CCACHE_EXE}")
        endif()
    endif()
endif()


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

if(APPLE AND (${CMAKE_SYSTEM_NAME} MATCHES "Darwin"))
    set(DEPSDIR "/usr/local/opt" FORCE) # homebrew prefix
else()
    set(DEPSDIR ${CMAKE_CURRENT_SOURCE_DIR}/../Depends)
endif()
Depends_Lib(${DEPSDIR})

add_executable(viewer_sample_gl ${viewer_sample_common_SRCS}
                                ${viewer_sample_common_HEADERS}
                                ${viewer_sample_gl_SRCS}
                                ${viewer_sample_gl_HEADERS}
                                )
target_link_libraries(viewer_sample_gl ssLoader)
target_link_libraries(viewer_sample_gl ssAnimator)
target_link_libraries(viewer_sample_gl ssHelper)

if(MSVC)
    target_link_libraries(viewer_sample_gl AntTweakBar${ANTTWEAKBAR_LIB_ARCH}.lib)
    target_link_libraries(viewer_sample_gl glew32.lib)
    target_link_libraries(viewer_sample_gl glfw3.lib)

    SET_TARGET_PROPERTIES (viewer_sample_gl PROPERTIES LINK_FLAGS_DEBUG "/SUBSYSTEM:windows")
    SET_TARGET_PROPERTIES (viewer_sample_gl PROPERTIES LINK_FLAGS_RELEASE "/SUBSYSTEM:windows")
    SET_TARGET_PROPERTIES (viewer_sample_gl PROPERTIES LINK_FLAGS_MINSIZEREL "/SUBSYSTEM:windows")
    SET_TARGET_PROPERTIES (viewer_sample_gl PROPERTIES LINK_FLAGS_RELWITHDEBINFO "/SUBSYSTEM:windows")
    ADD_DEFINITIONS(/D_WIN32)

elseif(APPLE AND (${CMAKE_SYSTEM_NAME} MATCHES "Darwin"))
    target_link_libraries(viewer_sample_gl AntTweakBar)
    target_link_libraries(viewer_sample_gl glfw.3)
    target_link_libraries(viewer_sample_gl "-framework OpenGL")
    target_link_libraries(viewer_sample_gl "-framework AppKit")
    target_link_libraries(viewer_sample_gl "-framework IOKit")
    ADD_DEFINITIONS(-DAPPLE)
endif()

#if(MSVC)
#    add_executable(viewer_sample_dx9 ${viewer_sample_common_SRCS}
#                                     ${viewer_sample_common_HEADERS}
#                                     ${viewer_sample_dx9_SRCS}
#                                     ${viewer_sample_dx9_HEADERS}
#                                     )
#endif()
#if(WIN32)
#    ADD_DEFINITIONS(/D_WIN32)
#    SET_TARGET_PROPERTIES (viewer_sample_dx9 PROPERTIES LINK_FLAGS_DEBUG "/SUBSYSTEM:windows")
#    SET_TARGET_PROPERTIES (viewer_sample_dx9 PROPERTIES LINK_FLAGS_RELEASE "/SUBSYSTEM:windows")
#    SET_TARGET_PROPERTIES (viewer_sample_dx9 PROPERTIES LINK_FLAGS_MINSIZEREL "/SUBSYSTEM:windows")
#    SET_TARGET_PROPERTIES (viewer_sample_dx9 PROPERTIES LINK_FLAGS_RELWITHDEBINFO "/SUBSYSTEM:windows")
#endif()
