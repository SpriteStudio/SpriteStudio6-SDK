@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set VCDIR=C:\Program Files\Microsoft Visual Studio\2022
set DEFAULT_QT_PREFIX=C:\Qt\6.9.0
if /I "%PROCESSOR_ARCHITECTURE%" == "AMD64" (
  set HOST_ARCH=x64
) else (
  set HOST_ARCH=%PROCESSOR_ARCHITECTURE%
)
set TARGET_ARCH=%HOST_ARCH%

if exist "%VCDIR%\Enterprise" (
  set VCVARSALL="%VCDIR%\Enterprise\VC\Auxiliary\Build\vcvarsall.bat"
) else if exist "%VCDIR%\Professional" (
  set VCVARSALL="%VCDIR%\Professional\VC\Auxiliary\Build\vcvarsall.bat"
) else (
  set VCVARSALL="%VCDIR%\Community\VC\Auxiliary\Build\vcvarsall.bat"
)

@echo on
set BUILD_TYPE=Debug
if not "%1" == "" (
    set BUILD_TYPE=%1
)

if not "%2" == "" (
  set TARGET_ARCH=%2
)
if /I "%TARGET_ARCH%" == "ARM64" (
  set DEFAULT_QT_PREFIX=%DEFAULT_QT_PREFIX%\msvc2022_arm64
) else (
  set DEFAULT_QT_PREFIX=%DEFAULT_QT_PREFIX%\msvc2022_64
)

if "%QT_PREFIX%" == "" (
    set QT_PREFIX=%DEFAULT_QT_PREFIX%
)

echo %QT_PREFIX%

pushd "%BUILDDIR%\Ss6ConverterGUI\Ss6ConverterGUI"
rmdir /S /Q build
mkdir build
pushd build

where ninja >nul 2>nul
if ERRORLEVEL 1 (
  cmake -A %TARGET_ARCH% -DCMAKE_BUILD_TYPE=%BUILD_TYPE% .. || exit /b 1
  cmake --build . --target ALL_BUILD --parallel -- /p:Configuration=%BUILD_TYPE% || exit /b 1
) else (

  if /I "%TARGET_ARCH%" == "%HOST_ARCH%" (
      call %VCVARSALL% %TARGET_ARCH%
  ) else (
      call %VCVARSALL% %HOST_ARCH%_%TARGET_ARCH%
  )

  cmake -G Ninja -DCMAKE_BUILD_TYPE=%BUILD_TYPE% -DCMAKE_SYSTEM_PROCESSOR=%TARGET_ARCH% .. || exit /b 1
  cmake --build . --parallel || exit /b 1
)

popd
popd
