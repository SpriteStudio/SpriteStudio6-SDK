@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set DEPENDSDIR=%BUILDDIR%\Depends
set Jucer2Reprojucer=%BUILDDIR%\Viewer2\FRUT\prefix\FRUT\bin\Jucer2Reprojucer.exe
@echo on

set BUILD_TYPE=Debug
if not "%1" == "" (
    set BUILD_TYPE=%1
)

pushd %BUILDDIR%\Viewer2
%Jucer2Reprojucer% Viewer2.jucer FRUT\prefix\FRUT\cmake\Reprojucer.cmake  --juce-modules JUCE\modules

rmdir /S /Q cmakeBuild
mkdir cmakeBuild
pushd cmakeBuild
cmake -G "Visual Studio 15 2017 Win64" -DCMAKE_BUILD_TYPE=%BUILD_TYPE% ..
cmake --build . --target ALL_BUILD -- /p:Configuration=%BUILD_TYPE% || exit /b 1
popd
popd
