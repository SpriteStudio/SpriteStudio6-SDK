@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
@echo on

set BUILD_TYPE=Release
if not "%1" == "" (
    set BUILD_TYPE=%1
)

pushd "%BUILDDIR%\Converter"

rem Build flatc
pushd "flatbuffers"

git clean -fdx .
cmake -DCMAKE_BUILD_TYPE=%BUILD_TYPE% . || exit /b 1
cmake --build . --target ALL_BUILD -- /p:Configuration=%BUILD_TYPE% || exit /b 1
set FLATC="%BUILDDIR%\Converter\flatbuffers\%BUILD_TYPE%\flatc.exe"

popd

rem generate ssfb code
%FLATC% -c fbs\ssfb.fbs
%FLATC% -s fbs\ssfb.fbs

popd
