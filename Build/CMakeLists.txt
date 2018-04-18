CMAKE_MINIMUM_REQUIRED(VERSION 2.6)
PROJECT(sssdk)

IF(WIN32)
    #環境で変えてくださいね
	#SET(BOOST_ROOT "C:/boost-1_64_0")
	#SET(BOOST_ROOT "C:/Boost/include/boost-1_64/")
	#SET(BOOST_PYTHON_LIB "C:/Boost/lib" )
	#SET(PYTHON_IHNCLUDE "C:/Python27/include")
	#SET(PYTHON_LIB "C:/Python27/libs")
ELSEIF(APPLE)
	SET(PYTHON_LIB "/usr/local/lib")
ELSEIF(CMAKE_COMPILER_IS_GNUCXX)
    ADD_DEFINITIONS(-std=gnu++14)
ENDIF()

link_directories(${SEARCH_LIB} ${BOOST_PYTHON_LIB} ${PYTHON_LIB})

ADD_LIBRARY(ssLoader 
../Common/Loader/tinyxml2/tinyxml2.cpp 
../Common/Loader/babel/babel.cpp 
../Common/Loader/babel/babel.h 
../Common/Loader/ssloader.h
../Common/Loader/ssloader_sspj.cpp 
../Common/Loader/ssloader_ssae.cpp 
../Common/Loader/ssloader_ssce.cpp 
../Common/Loader/ssloader_ssee.cpp 
../Common/Loader/SsEffectBehavior.cpp
../Common/Loader/SsEffectBehavior.h
../Common/Loader/SsEffectElement.h
../Common/Loader/SsEffectElement.cpp
../Common/Loader/ssstring_uty.cpp 
../Common/Loader/ssarchiver.cpp 
../Common/Loader/ssarchiver.h 
../Common/Loader/ssloader_sspj.h 
../Common/Loader/ssloader_ssae.h 
../Common/Loader/ssloader_ssce.h 
../Common/Loader/ssloader_ssee.h 
../Common/Loader/sstypes.cpp 
../Common/Loader/sstypes.h 
../Common/Loader/ssvalue.cpp 
../Common/Loader/ssvalue.h 
../Common/Loader/ssInterpolation.h 
../Common/Loader/ssInterpolation.cpp
../Common/Loader/ssattribute.cpp 
../Common/Loader/ssattribute.h 
../Common/Loader/ssstring_uty.h)


ADD_LIBRARY(ssAnimater
../Common/Animator/ssplayer_types.h
../Common/Animator/ssplayer_cellmap.h
../Common/Animator/ssplayer_cellmap.cpp
../Common/Animator/ssplayer_PartState.h
../Common/Animator/ssplayer_PartState.cpp
../Common/Animator/ssplayer_render.cpp
../Common/Animator/ssplayer_render.h
../Common/Animator/ssplayer_matrix.cpp
../Common/Animator/ssplayer_matrix.h
../Common/Animator/ssplayer_effect.cpp
../Common/Animator/ssplayer_effect.h
../Common/Animator/ssplayer_effectfunction.cpp
../Common/Animator/ssplayer_effectfunction.h
../Common/Animator/MersenneTwister.h
../Common/Animator/ssplayer_animedecode.cpp
../Common/Animator/ssplayer_animedecode.h
../Common/Animator/ssplayer_effect2.cpp
../Common/Animator/ssplayer_effect2.h
../Common/Animator/ssplayer_mesh.cpp
../Common/Animator/ssplayer_mesh.h
)

IF(APPLE)
ADD_LIBRARY(ssHelper
../Common/Helper/Win/WinMain.cpp
../Common/Helper/IsshTexture.cpp
../Common/Helper/IsshTexture.h
../Common/Helper/sshTextureBMP.cpp
../Common/Helper/sshTextureBMP.h
../Common/Helper/stb_image.c
../Common/Helper/stb_image.h
../Common/Helper/sshObject.h
../Common/Helper/sshScene.cpp
../Common/Helper/sshTask.cpp
../Common/Helper/sshTask.h
../Common/Helper/XPFileOpenDlg.h
../Common/Helper/XPFileOpenDlg.cpp
../Common/Helper/DebugPrint.h
../Common/Helper/DebugPrint.cpp
../Common/Helper/MacOS/OSXFileOpen.h
../Common/Helper/MacOS/OSXFileOpen.mm)
ELSE()
ADD_LIBRARY(ssHelper
../Common/Helper/Win/WinMain.cpp
../Common/Helper/IsshTexture.cpp
../Common/Helper/IsshTexture.h
../Common/Helper/sshTextureBMP.cpp
../Common/Helper/sshTextureBMP.h
../Common/Helper/stb_image.c
../Common/Helper/stb_image.h
../Common/Helper/sshObject.h
../Common/Helper/sshScene.cpp
../Common/Helper/sshTask.cpp
../Common/Helper/sshTask.h
../Common/Helper/XPFileOpenDlg.h
../Common/Helper/XPFileOpenDlg.cpp
../Common/Helper/DebugPrint.h
../Common/Helper/DebugPrint.cpp
../Common/Helper/MacOS/OSXFileOpen.h)
ENDIF()


ADD_EXECUTABLE(viewer_sample_gl
../Common/Helper/OpenGL/SSTextureGL.cpp
../Common/Helper/OpenGL/SSTextureGL.h
../Common/Drawer/ssplayer_render_gl.h
../Common/Drawer/ssplayer_render_gl.cpp
../Common/Drawer/ssplayer_shader_gl.h
../Common/Drawer/ssplayer_shader_gl.cpp
./Viewer/source/main_gl.cpp
./Viewer/source/myscene.cpp
./Viewer/source/myscene.h
)



ADD_EXECUTABLE(viewer_sample_dx9
../Common/Helper/DirectX/d3dsimple.cpp
../Common/Helper/DirectX/d3dsimple.h
../Common/Helper/DirectX/SSTextureDX9.cpp
../Common/Helper/DirectX/SSTextureDX9.h
../Common/Drawer/ssplayer_render_dx9.h
../Common/Drawer/ssplayer_render_dx9.cpp
./Viewer/source/main_dx9.cpp
./Viewer/source/myscene.cpp
./Viewer/source/myscene.h
)

#IF(WIN32)
#ADD_EXECUTABLE(scriptbase_converter
#./PythonBinder/script_base_conv.cpp
#./PythonBinder/bind_SSXML.cpp
#./PythonBinder/bind_SSXML.h
#./PythonBinder/bind_Animation.cpp
#./PythonBinder/bind_Animation.h
#./PythonBinder/bind_SsPart.cpp
#./PythonBinder/bind_SsPart.h
#./PythonBinder/bind_SsPartAnime.cpp
#./PythonBinder/bind_SsPartAnime.h
#./PythonBinder/bind_SsAnimeDecoder.cpp
#./PythonBinder/bind_SsAnimeDecoder.h
#./PythonBinder/bind_AnimePack.cpp
#./PythonBinder/bind_AnimePack.h
#./PythonBinder/bind_SsAttribute.cpp
#./PythonBinder/bind_SsAttribute.h
#./PythonBinder/bind_SsKeyframe.cpp
#./PythonBinder/bind_SsKeyframe.h
#./PythonBinder/package_SpriteStudio.cpp
#./PythonBinder/package_SpriteStudio.h
#./PythonBinder/bind_Cellmap.cpp
#./PythonBinder/bind_Cellmap.h
#)
#ENDIF(WIN32)


ADD_EXECUTABLE(Ss6Converter
./Converter/main.cpp
./Converter/BinaryDataWriter.cpp
./Converter/BinaryDataWriter.h
./Converter/FileUtil.cpp
./Converter/FileUtil.h
./Converter/Lump.cpp
./Converter/Lump.h
./Converter/LumpExporter.cpp
./Converter/LumpExporter.h
./Converter/SsPlayerConverter.h
)


IF (WIN32)
    SET_TARGET_PROPERTIES (viewer_sample_gl PROPERTIES LINK_FLAGS_DEBUG "/SUBSYSTEM:windows")
    SET_TARGET_PROPERTIES (viewer_sample_gl PROPERTIES LINK_FLAGS_RELEASE "/SUBSYSTEM:windows")
    SET_TARGET_PROPERTIES (viewer_sample_gl PROPERTIES LINK_FLAGS_MINSIZEREL "/SUBSYSTEM:windows")
    SET_TARGET_PROPERTIES (viewer_sample_gl PROPERTIES LINK_FLAGS_RELWITHDEBINFO "/SUBSYSTEM:windows")

    SET_TARGET_PROPERTIES (viewer_sample_dx9 PROPERTIES LINK_FLAGS_DEBUG "/SUBSYSTEM:windows")
    SET_TARGET_PROPERTIES (viewer_sample_dx9 PROPERTIES LINK_FLAGS_RELEASE "/SUBSYSTEM:windows")
    SET_TARGET_PROPERTIES (viewer_sample_dx9 PROPERTIES LINK_FLAGS_MINSIZEREL "/SUBSYSTEM:windows")
    SET_TARGET_PROPERTIES (viewer_sample_dx9 PROPERTIES LINK_FLAGS_RELWITHDEBINFO "/SUBSYSTEM:windows")


ENDIF (WIN32)

IF(WIN32)
#	SET(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} /subsystem:windows")
	SET(LINK_LIB1 "ssLoader")
	SET(LINK_LIB2 "ssAnimater")
	SET(LINK_LIB3 "ssHelper")
	SET(LINK_LIB4 "")

    ADD_DEFINITIONS(-D_WIN32 )
	SET(SEARCH_LIB "" )
ELSEIF(APPLE)
	SET(SEARCH_DIR "/usr/local/include" )
	SET(SEARCH_LIB "/usr/local/lib" )
	SET(LINK_LIB1 "ssLoader")
	SET(LINK_LIB2 "ssAnimater")
	SET(LINK_LIB3 "ssHelper")	
#	SET(LINK_LIB4 "libAntTweakBar.dylib libglfw3.a -framework OpenGL -framework AppKit -framework IOKit" )
	SET(LINK_LIB4 "" )

    ADD_DEFINITIONS(-APPLE)
ELSEIF(CMAKE_COMPILER_IS_GNUCXX)
	SET(SEARCH_DIR "/usr/local/include" )
	SET(SEARCH_LIB "/usr/local/lib" )
	SET(LINK_LIB3 "ssLoader")
	SET(LINK_LIB1 "ssAnimater")
	SET(LINK_LIB2 "ssHelper")
	SET(LINK_LIB4 "" )
ENDIF()

#link_directories( SEARCH_LIB )
include_directories(;${SEARCH_DIR} ; ${PYTHON_IHNCLUDE} ; ${BOOST_ROOT} ; ../Common/Loader/ ;../Common/Helper/ ; ../Common/Drawer/ ; ../Common/Animator)
target_link_libraries(viewer_sample_gl ${LINK_LIB1} ${LINK_LIB2} ${LINK_LIB3} ${LINK_LIB4} )

#IF (WIN32)
#target_link_libraries(scriptbase_converter ${LINK_LIB1} ${LINK_LIB2} ${LINK_LIB3} ${LINK_LIB4} )
#ENDIF(WIN32)

IF (WIN32)
target_link_libraries(viewer_sample_dx9 ${LINK_LIB1} ${LINK_LIB2} ${LINK_LIB3} ${LINK_LIB4})
ENDIF()


target_link_libraries(Ss6Converter ${LINK_LIB1} ${LINK_LIB2} ${LINK_LIB3} )




