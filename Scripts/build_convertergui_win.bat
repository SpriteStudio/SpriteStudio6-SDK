@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set DEFAULT_QT_PREFIX=C:\Qt\6.5.1
for /f "tokens=2 delims==" %%I in (
  'wmic cpu get architecture /value'
) do set "cpuArch=%%I"
if "%cpuArch%"=="12" (
  set DEFAULT_QT_PREFIX=%DEFAULT_QT_PREFIX%\msvc2019_arm64
) else (
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

pushd "%BUILDDIR%\Ss6ConverterGUI\Ss6ConverterGUI"
rmdir /S /Q build
mkdir build
pushd build
cmake -DCMAKE_BUILD_TYPE=%BUILD_TYPE% .. || exit /b 1
cmake --build . --target ALL_BUILD --parallel -- /p:Configuration=%BUILD_TYPE% || exit /b 1
popd
popd
