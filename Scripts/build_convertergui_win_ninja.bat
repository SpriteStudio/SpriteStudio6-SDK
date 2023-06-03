@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set VCDIR=C:\Program Files\Microsoft Visual Studio\2022
set DEFAULT_QT_PREFIX=C:\Qt\6.5.1
for /f "tokens=2 delims==" %%I in (
  'wmic cpu get architecture /value'
) do set "cpuArch=%%I"
if "%cpuArch%"=="12" (
  set ARCH=arm64
  set DEFAULT_QT_PREFIX=%DEFAULT_QT_PREFIX%\msvc2019_arm64
) else (
  set ARCH=x64
  set DEFAULT_QT_PREFIX=%DEFAULT_QT_PREFIX%\msvc2019_64
)

if "%QT_PREFIX%" == "" (
    set QT_PREFIX=%DEFAULT_QT_PREFIX%
)

@echo on
set BUILD_TYPE=Debug
if not "%1" == "" (
    set BUILD_TYPE=%1
)

if exist "%VCDIR%\Enterprise" (
    set VCVARSALL="%VCDIR%\Enterprise\VC\Auxiliary\Build\vcvarsall.bat"
) else if exist "%VCDIR%\Professional" (
    set VCVARSALL="%VCDIR%\Professional\VC\Auxiliary\Build\vcvarsall.bat"
) else (
    set VCVARSALL="%VCDIR%\Community\VC\Auxiliary\Build\vcvarsall.bat"
)

call %VCVARSALL% %ARCH%

pushd "%BUILDDIR%\Ss6ConverterGUI\Ss6ConverterGUI"
rmdir /S /Q build
mkdir build
pushd build
cmake -DCMAKE_BUILD_TYPE=%BUILD_TYPE% -G Ninja .. || exit /b 1
cmake --build . || exit /b 1
popd
popd
