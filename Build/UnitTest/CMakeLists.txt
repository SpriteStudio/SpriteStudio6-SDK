cmake_minimum_required(VERSION 3.11)

project(UnitTest)

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
    add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/../../Common UnitTest_Common)
endif()
add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/../Depends UnitTest_Depends)

set(UnitTest_SRCS
    gtest-all.cc
    gtest_main.cc
    unit_ssloader_ssae.cc
    unit_ssloader_sspj.cc
    unit_ssloader_ssce.cc
)

set(UnitTest_HEADERS
    gtest/gtest.h
)

include_directories(${CMAKE_CURRENT_SOURCE_DIR})
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Loader)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Animator)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper)

if(MSVC)
    add_definitions(/DGTEST_HAS_TR1_TUPLE=0)
else()
    add_definitions(-DGTEST_HAS_TR1_TUPLE=0)
endif()

add_executable(UnitTest ${UnitTest_SRCS} ${UnitTest_HEADERS})
target_link_libraries(UnitTest ssLoader)
target_link_libraries(UnitTest ssAnimator)
target_link_libraries(UnitTest ssHelper)

enable_testing()
add_test(NAME UnitTest COMMAND ${CMAKE_CURRENT_BINARY_DIR}/UnitTest)
