@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set DEPENDSDIR=%BUILDDIR%\Depends
@echo on

pushd %BUILDDIR%\Viewer2
rmdir /S /Q JUCE
rmdir /S /Q FRUT
git clone --branch=5.4.4 --depth=1 --single-branch https://github.com/WeAreROLI/JUCE.git || exit /b 1
git clone https://github.com/McMartin/FRUT.git || exit /b 1

rem create Jucer2Reprojucer command
pushd FRUT
mkdir build 
pushd build
cmake .. -DCMAKE_BUILD_TYPE=Release -DCMAKE_INSTALL_PREFIX=../prefix -DJUCE_ROOT=..\..\JUCE || exit /b 1
cmake --build . --target install || exit /b 1
popd
popd

popd
