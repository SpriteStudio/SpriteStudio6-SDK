@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set Jucer2CMake=%BUILDDIR%\Viewer2\FRUT\prefix\FRUT\bin\Jucer2CMake.exe
@echo on

set BUILD_TYPE=Debug
if not "%1" == "" (
    set BUILD_TYPE=%1
)

pushd %BUILDDIR%\Viewer2
%Jucer2CMake%  reprojucer Viewer2.jucer FRUT\prefix\FRUT\cmake\Reprojucer.cmake  --juce-modules %CURDIR%\JUCE\modules

rmdir /S /Q cmakeBuild
mkdir cmakeBuild
pushd cmakeBuild
cmake -DCMAKE_BUILD_TYPE=%BUILD_TYPE% .. || exit /b 1
cmake --build . --target ALL_BUILD -- /p:Configuration=%BUILD_TYPE% || exit /b 1
popd
popd
