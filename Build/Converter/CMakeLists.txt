cmake_minimum_required(VERSION 3.15)

project(Ss6Converter)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

if(MSVC)
add_compile_options("$<$<C_COMPILER_ID:MSVC>:/utf-8>")
add_compile_options("$<$<CXX_COMPILER_ID:MSVC>:/utf-8>")
endif()

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
    add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/../../Common Ss6Converter_Common)
endif()

if(NOT TARGET glad)
    add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Libs/glad Ss6Converter_glad)
endif()

# dependency libraries
message("start fetching dependency libraries")
include(FetchContent)

message("fetch zlib")
FetchContent_Declare(
        zlib
        GIT_REPOSITORY https://github.com/madler/zlib
        GIT_TAG        v1.2.11
)
FetchContent_MakeAvailable(zlib)
include_directories(${zlib_SOURCE_DIR})

message("fetch json")
FetchContent_Declare(
    json
    GIT_REPOSITORY https://github.com/nlohmann/json.git
    GIT_TAG        v3.7.3)
FetchContent_GetProperties(json)
if(NOT json_POPULATED)
  set(JSON_BuildTests OFF CACHE INTERNAL "") # skip json unit test
  FetchContent_Populate(json)
  add_subdirectory(${json_SOURCE_DIR} ${json_BINARY_DIR} EXCLUDE_FROM_ALL)
endif()

message("fetch flatbuffers")
FetchContent_Declare(
    flatbuffers
    GIT_REPOSITORY https://github.com/google/flatbuffers.git
    GIT_TAG        v1.12.1)
FetchContent_MakeAvailable(flatbuffers)
include_directories(${flatbuffers_SOURCE_DIR}/include)

message("finish fetching dependency libraries")

set(Ss6Converter_SRCS
    main.cpp
    BinaryDataWriter.cpp
    FileUtil.cpp
    Lump.cpp
    LumpExporter.cpp
    sspkg.cpp

    ${zlib_SOURCE_DIR}/contrib/minizip/zip.c
    ${zlib_SOURCE_DIR}/contrib/minizip/ioapi.c
)

set(Ss6Converter_HEADERS
    ssfb_generated.h
    BinaryDataWriter.h
    FileUtil.h
    Lump.h
    LumpExporter.h
    SsPlayerConverter.h
    sspkg.h
    ${zlib_SOURCE_DIR}/contrib/minizip/zip.h
)

include_directories(${CMAKE_CURRENT_SOURCE_DIR})
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Loader)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Animator)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper)


# for win & mac suport thumbnail
if(WIN32 OR APPLE)
    

    list(APPEND Ss6Converter_SRCS 
        BackGroudRender.cpp
        ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Drawer/ssplayer_render_gl.cpp
        ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Drawer/ssplayer_shader_gl.cpp
        ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper/OpenGL/SSTextureGL.cpp
    )

    list(APPEND Ss6Converter_HEADERS 
        BackGroudRender.h
        ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Drawer/ssplayer_render_gl.h
        ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Drawer/ssplayer_shader_gl.h
        ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper/OpenGL/SSTextureGL.h
        ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper/stb_image_write.h
     )

    include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Drawer)
    include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Libs/glad/include)
    include_directories(${CMAKE_CURRENT_SOURCE_DIR}/glfw/deps)

    add_definitions( -D_BACKBUFFER_RENDERING__) 
endif()

add_executable(Ss6Converter ${Ss6Converter_SRCS} ${Ss6Converter_HEADERS})



include_directories(glfw/include)
include_directories(filesystem/include)
include_directories(plog/include)


add_subdirectory(glfw)


if(NOT (WIN32 OR APPLE))
    # For Linux
    target_link_libraries(Ss6Converter ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter_Common/Loader/libssLoader.a)
    target_link_libraries(Ss6Converter ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter_Common/Animator/libssAnimator.a)
    target_link_libraries(Ss6Converter ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter_Common/Helper/libssHelper.a)
endif()

set(PLATFORM_DEPS "")
if(APPLE AND (${CMAKE_SYSTEM_NAME} MATCHES "Darwin"))
    set(PLATFORM_DEPS "-framework CoreFoundation")
endif()

# add use opengl setting
if(MSVC)

    ADD_DEFINITIONS(/D_WIN32)

elseif(APPLE AND (${CMAKE_SYSTEM_NAME} MATCHES "Darwin"))
#    find_package (glfw3 REQUIRED)

    #include_directories(/usr/local/include)
    #link_directories(/usr/local/lib)

    set(PLATFORM_DEPS "${PLATFORM_DEPS} -framework OpenGL")
    set(PLATFORM_DEPS "${PLATFORM_DEPS} -framework AppKit")
    set(PLATFORM_DEPS "${PLATFORM_DEPS} -framework IOKit")

#    target_link_libraries(Ss6Converter "-framework OpenGL")
#    target_link_libraries(Ss6Converter "-framework AppKit")
#    target_link_libraries(Ss6Converter "-framework IOKit")
    ADD_DEFINITIONS(-DAPPLE)
endif()

target_link_libraries(Ss6Converter PRIVATE ssLoader ssAnimator ssHelper zlibstatic flatbuffers glfw glad nlohmann_json::nlohmann_json ${PLATFORM_DEPS})


# --- If we are on OS X copy all the embedded libraries to the app bundle
#if(APPLE AND (${CMAKE_SYSTEM_NAME} MATCHES "Darwin"))
#    install(CODE "
#        include(BundleUtilities)
#        set(BU_CHMOD_BUNDLE_ITEMS TRUE)
#        fixup_bundle(${CMAKE_INSTALL_PREFIX}/viewer_sample_gl.app \"\" \"\")
#    " COMPONENT Runtime)
#endif()



enable_testing()
add_test(NAME allAttributeV6 COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/allAttributeV6/allAttributeV6.sspj)
add_test(NAME allPartsV6 COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/allPartsV6/allPartsV6.sspj)
add_test(NAME effect_allcheck COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/effect_allcheck/effect_allcheck.sspj)
add_test(NAME allAttributeV6Json COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f json ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/allAttributeV6/allAttributeV6.sspj)
add_test(NAME allPartsV6Json COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f json ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/allPartsV6/allPartsV6.sspj)
add_test(NAME effect_allcheckJson COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f json ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/effect_allcheck/effect_allcheck.sspj)
add_test(NAME allAttributeV6Ssfb COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f ssfb ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/allAttributeV6/allAttributeV6.sspj)
add_test(NAME allPartsV6Ssfb COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f ssfb ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/allPartsV6/allPartsV6.sspj)
add_test(NAME effect_allcheckSsfb COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f ssfb ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/effect_allcheck/effect_allcheck.sspj)

#add_test(NAME allAttributeV6Sspkg COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f sspkg ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/allAttributeV6/allAttributeV6.sspj)
#add_test(NAME allPartsV6SSspkg COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f sspkg ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/allPartsV6/allPartsV6.sspj)
#add_test(NAME effect_allcheckSspkg COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f sspkg ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/effect_allcheck/effect_allcheck.sspj)
#add_test(NAME knight_Sspkg COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f sspkg ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/MeshBone/knight.sspj)






