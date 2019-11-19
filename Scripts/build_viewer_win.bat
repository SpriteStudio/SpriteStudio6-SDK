@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set DEPENDSDIR=%BUILDDIR%\Depends
@echo on

set BUILD_TYPE=Debug
if not "%1" == "" (
    set BUILD_TYPE=%1
)

pushd %BUILDDIR%\Viewer
rmdir /S /Q build
mkdir build
pushd build
cmake -DCMAKE_BUILD_TYPE=%BUILD_TYPE% .. || exit /b 1
cmake --build . --target ALL_BUILD -- /p:Configuration=%BUILD_TYPE% || exit /b 1
copy %DEPENDSDIR%\AntTweakBar\lib\AntTweakBar64.dll %BUILD_TYPE%\
rem TODO: glew version
copy %DEPENDSDIR%\glew-2.1.0\bin\Release\x64\glew32.dll %BUILD_TYPE%\
popd
popd
