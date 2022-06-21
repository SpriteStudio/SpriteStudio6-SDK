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
rmdir /S /Q build
mkdir build
pushd build
cmake -DCMAKE_BUILD_TYPE=%BUILD_TYPE% . || exit /b 1
cmake --build . --target ALL_BUILD --parallel -- /p:Configuration=%BUILD_TYPE% || exit /b 1
set FLATC="%BUILDDIR%\Converter\build\_deps\flatbuffers-build\flatc.exe"
popd

rem generate ssfb code
%FLATC% -c fbs\ssfb.fbs --gen-compare --gen-object-api --gen-all --cpp-ptr-type std::shared_ptr
%FLATC% -n fbs\ssfb.fbs
%FLATC% -T fbs\ssfb.fbs

popd
