cmake_minimum_required(VERSION 3.11)

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

set(FlatBuffers_Library_SRCS
    flatbuffers/include/flatbuffers/code_generators.h
    flatbuffers/include/flatbuffers/base.h
    flatbuffers/include/flatbuffers/flatbuffers.h
    flatbuffers/include/flatbuffers/hash.h
    flatbuffers/include/flatbuffers/idl.h
    flatbuffers/include/flatbuffers/util.h
    flatbuffers/include/flatbuffers/reflection.h
    flatbuffers/include/flatbuffers/reflection_generated.h
    flatbuffers/include/flatbuffers/stl_emulation.h
    flatbuffers/include/flatbuffers/flexbuffers.h
    flatbuffers/include/flatbuffers/registry.h
    flatbuffers/include/flatbuffers/minireflect.h
    flatbuffers/src/code_generators.cpp
    flatbuffers/src/idl_parser.cpp
    flatbuffers/src/idl_gen_text.cpp
    flatbuffers/src/reflection.cpp
    flatbuffers/src/util.cpp
)
include_directories(flatbuffers/include)

set(Ss6Converter_SRCS
    ${FlatBuffers_Library_SRCS}
    main.cpp
    BinaryDataWriter.cpp
    FileUtil.cpp
    Lump.cpp
    LumpExporter.cpp
)

set(Ss6Converter_HEADERS
    ssfb_generated.h
    BinaryDataWriter.h
    FileUtil.h
    Lump.h
    LumpExporter.h
    SsPlayerConverter.h
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
#        ${CMAKE_CURRENT_SOURCE_DIR}/glfw/deps/glad_gl.c
    )

    list(APPEND Ss6Converter_HEADERS 
        BackGroudRender.h
        ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Drawer/ssplayer_render_gl.h
        ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Drawer/ssplayer_shader_gl.h
        ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper/OpenGL/SSTextureGL.h
        ${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Helper/stb_image_write.h
 #       ${CMAKE_CURRENT_SOURCE_DIR}/glfw/deps/glad/gl.h
    )


    include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Common/Drawer)
    include_directories(${CMAKE_CURRENT_SOURCE_DIR}/glfw/deps)
#    include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../../Build/Viewer2/lib/glew-2.1.0/include)

if(APPLE)
    #for install brew 
#    include_directories(/usr/local/Cellar/glfw/3.3.2/)
endif()

    add_definitions( -D_BACKBUFFER_RENDERING__) 
endif()

add_executable(Ss6Converter ${Ss6Converter_SRCS} ${Ss6Converter_HEADERS})
target_link_libraries(Ss6Converter ssLoader)
target_link_libraries(Ss6Converter ssAnimator)
target_link_libraries(Ss6Converter ssHelper)


include_directories(glad/include)
include_directories(glfw/include)

add_subdirectory(glad)
add_subdirectory(glfw)


if(NOT (WIN32 OR APPLE))
    # For Linux
    target_link_libraries(Ss6Converter ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter_Common/Loader/libssLoader.a)
    target_link_libraries(Ss6Converter ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter_Common/Animator/libssAnimator.a)
    target_link_libraries(Ss6Converter ${CMAKE_CURRENT_BINARY_DIR}/Ss6Converter_Common/Helper/libssHelper.a)
endif()

if(APPLE AND (${CMAKE_SYSTEM_NAME} MATCHES "Darwin"))
    target_link_libraries(Ss6Converter "-framework CoreFoundation")
endif()


target_link_libraries(Ss6Converter glfw)
target_link_libraries(Ss6Converter glad)

# add use opengl setting
if(MSVC)
#    find_package (glfw REQUIRED)

#    target_link_libraries(Ss6Converter glew32.lib)
#    target_link_libraries(Ss6Converter glfw3.lib)


#    SET_TARGET_PROPERTIES (Ss6Converter PROPERTIES LINK_FLAGS_DEBUG "/SUBSYSTEM:windows")
#    SET_TARGET_PROPERTIES (Ss6Converter PROPERTIES LINK_FLAGS_RELEASE "/SUBSYSTEM:windows")
#    SET_TARGET_PROPERTIES (Ss6Converter PROPERTIES LINK_FLAGS_MINSIZEREL "/SUBSYSTEM:windows")
#    SET_TARGET_PROPERTIES (Ss6Converter PROPERTIES LINK_FLAGS_RELWITHDEBINFO "/SUBSYSTEM:windows")


    ADD_DEFINITIONS(/D_WIN32)

elseif(APPLE AND (${CMAKE_SYSTEM_NAME} MATCHES "Darwin"))
#    find_package (glfw3 REQUIRED)

    #include_directories(/usr/local/include)
    #link_directories(/usr/local/lib)

    target_link_libraries(Ss6Converter "-framework OpenGL")
    target_link_libraries(Ss6Converter "-framework AppKit")
    target_link_libraries(Ss6Converter "-framework IOKit")
    ADD_DEFINITIONS(-DAPPLE)
endif()


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






