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
cmake -DCMAKE_BUILD_TYPE=%BUILD_TYPE% .. || exit /b 1
cmake --build . --target ALL_BUILD --parallel -- /p:Configuration=%BUILD_TYPE% || exit /b 1
ctest -C %BUILD_TYPE% . || exit /b 1
popd
popd