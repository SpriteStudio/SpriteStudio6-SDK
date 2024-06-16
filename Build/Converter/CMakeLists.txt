include(${CMAKE_CURRENT_SOURCE_DIR}/../../cmake/initialize.cmake)
cmake_minimum_required(VERSION 3.25)

project(Ss6Converter)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

OPTIONS()

CCACHE()

PREPARE()

if(NOT TARGET ssHelper)
    add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/../../Common ${PROJECT_NAME}_Common)
endif()

if(NOT TARGET glad)
    add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Libs/glad ${PROJECT_NAME}_glad)
endif()

# dependency libraries
message("start fetching dependency libraries")
include(FetchContent)

message("fetch zlib")
FetchContent_Declare(
        zlib
        GIT_REPOSITORY https://github.com/madler/zlib
        GIT_TAG        v1.3.1
)
FetchContent_MakeAvailable(zlib)
include_directories(${zlib_SOURCE_DIR})

message("fetch json")
set(JSON_BuildTests OFF CACHE INTERNAL "")
set(JSON_Install OFF CACHE INTERNAL "")
FetchContent_Declare(
    json
    GIT_REPOSITORY https://github.com/nlohmann/json.git
    GIT_TAG        v3.11.3)
FetchContent_GetProperties(json)
if(NOT json_POPULATED)
  set(JSON_BuildTests OFF CACHE INTERNAL "") # skip json unit test
  FetchContent_Populate(json)
  add_subdirectory(${json_SOURCE_DIR} ${json_BINARY_DIR} EXCLUDE_FROM_ALL)
  include_directories(${json_SOURCE_DIR}/include)
endif()

message("fetch flatbuffers")
FetchContent_Declare(
    flatbuffers
    GIT_REPOSITORY https://github.com/google/flatbuffers.git
    GIT_TAG        v24.3.25)
if (ENABLE_FLATC)
    set(FLATBUFFERS_BUILD_TESTS OFF CACHE INTERNAL "Disable flatbuffers unit test")
    set(FLATBUFFERS_INSTALL OFF CACHE INTERNAL "Disable flatbuffers install")
    set(FLATBUFFERS_BUILD_FLATC ON CACHE INTERNAL "Disable the build of the flatbuffers compiler")
    set(FLATBUFFERS_BUILD_FLATHASH OFF CACHE INTERNAL "Disable the build of flathash")
    FetchContent_MakeAvailable(flatbuffers)
    include_directories(${flatbuffers_SOURCE_DIR}/include)
else()
    FetchContent_GetProperties(flatbuffers)
    if(NOT flatbuffers_POPULATED)
        # Fetch the content using previously declared details
        FetchContent_Populate(flatbuffers)
        set(FlatBuffers_Library_SRCS
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/allocator.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/array.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/base.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/buffer.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/buffer_ref.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/code_generator.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/code_generators.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/default_allocator.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/detached_buffer.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/flatbuffer_builder.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/flatbuffers.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/flatc.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/flex_flat_util.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/flexbuffers.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/grpc.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/hash.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/idl.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/minireflect.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/pch/flatc_pch.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/pch/pch.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/reflection.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/reflection_generated.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/registry.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/stl_emulation.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/string.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/struct.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/table.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/util.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/vector.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/vector_downward.h
          ${flatbuffers_SOURCE_DIR}/include/flatbuffers/verifier.h
          ${flatbuffers_SOURCE_DIR}/src/idl_parser.cpp
          ${flatbuffers_SOURCE_DIR}/src/idl_gen_text.cpp
          ${flatbuffers_SOURCE_DIR}/src/reflection.cpp
          ${flatbuffers_SOURCE_DIR}/src/util.cpp
          )
        include_directories(${flatbuffers_SOURCE_DIR}/include)
    endif()
endif()

message("finish fetching dependency libraries")

# get a short commit hash
find_package(Git QUIET)

# git コマンドが存在する場合のみ実行
if(GIT_FOUND)
    execute_process(
            COMMAND git rev-parse --is-inside-work-tree
            WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
            RESULT_VARIABLE GIT_REPOSITORY_FOUND
            OUTPUT_QUIET
            ERROR_QUIET
    )

    if(GIT_REPOSITORY_FOUND EQUAL 0)
        execute_process(
                COMMAND git rev-parse --short HEAD
                WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
                OUTPUT_VARIABLE GIT_SHORT_COMMIT_HASH
                OUTPUT_STRIP_TRAILING_WHITESPACE
        )
    endif()
endif()
if(NOT DEFINED GIT_SHORT_COMMIT_HASH)
    set(GIT_SHORT_COMMIT_HASH "")
endif()
configure_file("${CMAKE_CURRENT_SOURCE_DIR}/version.h.in"
        "${CMAKE_CURRENT_BINARY_DIR}/version.h"
)

set(Ss6Converter_SRCS
    main.cpp
    BinaryDataWriter.cpp
    Lump.cpp
    LumpExporter.cpp
    sspkg.cpp

    ${zlib_SOURCE_DIR}/contrib/minizip/zip.c
    ${zlib_SOURCE_DIR}/contrib/minizip/ioapi.c
)

set(Ss6Converter_HEADERS
    ssfb_generated.h
    BinaryDataWriter.h
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
    message("fetch glfw")
    set(GLFW_BUILD_EXAMPLES OFF CACHE INTERNAL "")
    set(GLFW_BUILD_TESTS OFF CACHE INTERNAL "")
    set(GLFW_BUILD_DOCS OFF CACHE INTERNAL "")
    set(GLFW_INSTALL OFF CACHE INTERNAL "")
    set(GLFW_VULKAN_STATIC OFF CACHE INTERNAL "")
    FetchContent_Declare(
            glfw
            GIT_REPOSITORY https://github.com/glfw/glfw.git
            GIT_TAG        3.4)
    FetchContent_MakeAvailable(glfw)
    include_directories(${glfw_SOURCE_DIR}/include)
    include_directories(${glfw_SOURCE_DIR}/deps)

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

    add_definitions( -D_BACKBUFFER_RENDERING__)
endif()

add_executable(Ss6Converter ${Ss6Converter_SRCS} ${Ss6Converter_HEADERS} ${FlatBuffers_Library_SRCS})
target_include_directories(Ss6Converter PUBLIC "${CMAKE_CURRENT_BINARY_DIR}")

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

    set(PLATFORM_DEPS "${PLATFORM_DEPS} -framework OpenGL")
    set(PLATFORM_DEPS "${PLATFORM_DEPS} -framework AppKit")
    set(PLATFORM_DEPS "${PLATFORM_DEPS} -framework IOKit")

#    target_link_libraries(Ss6Converter "-framework OpenGL")
#    target_link_libraries(Ss6Converter "-framework AppKit")
#    target_link_libraries(Ss6Converter "-framework IOKit")
    ADD_DEFINITIONS(-DAPPLE)
endif()

target_link_libraries(Ss6Converter PRIVATE ssLoader ssAnimator ssHelper zlibstatic glfw glad nlohmann_json::nlohmann_json ${PLATFORM_DEPS})


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
add_test(NAME MeshBone COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/MeshBone/Knight.sspj)
add_test(NAME allAttributeV6Json COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f json ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/allAttributeV6/allAttributeV6.sspj)
add_test(NAME allPartsV6Json COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f json ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/allPartsV6/allPartsV6.sspj)
add_test(NAME effect_allcheckJson COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f json ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/effect_allcheck/effect_allcheck.sspj)
add_test(NAME MeshBoneJson COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f json ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/MeshBone/Knight.sspj)
add_test(NAME allAttributeV6Ssfb COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f ssfb ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/allAttributeV6/allAttributeV6.sspj)
add_test(NAME allPartsV6Ssfb COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f ssfb ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/allPartsV6/allPartsV6.sspj)
add_test(NAME effect_allcheckSsfb COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f ssfb ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/effect_allcheck/effect_allcheck.sspj)
add_test(NAME MeshBoneSsfb COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f ssfb ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/MeshBone/Knight.sspj)

# add_test(NAME allAttributeV6Sspkg COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f sspkg ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/allAttributeV6/allAttributeV6.sspj)
# add_test(NAME allPartsV6SSspkg COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f sspkg ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/allPartsV6/allPartsV6.sspj)
# add_test(NAME effect_allcheckSspkg COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f sspkg ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/effect_allcheck/effect_allcheck.sspj)
# add_test(NAME knight_Sspkg COMMAND ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter -f sspkg ${CMAKE_CURRENT_SOURCE_DIR}/../TestData/MeshBone/knight.sspj)



