cmake_minimum_required(VERSION 3.11)

project(UnitTest)

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
target_link_libraries(UnitTest ssAnimater)
target_link_libraries(UnitTest ssHelper)

enable_testing()
add_test(NAME UnitTest COMMAND ${CMAKE_CURRENT_BINARY_DIR}/UnitTest)
