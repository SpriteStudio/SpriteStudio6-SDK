@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
@echo on

set BUILD_TYPE=Debug
if not "%1" == "" (
    set BUILD_TYPE=%1
)
pushd "%BUILDDIR%\Converter"
rmdir /S /Q build
mkdir build
pushd build
cmake -G "Visual Studio 15 2017 Win64" -DCMAKE_BUILD_TYPE=%BUILD_TYPE% ..
cmake --build . --target ALL_BUILD -- /p:Configuration=%BUILD_TYPE% || exit /b 1
ctest -C %BUILD_TYPE% . || exit /b 1
popd
popd