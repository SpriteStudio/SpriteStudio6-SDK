@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set TOOLSDIR=%BASEDIR%\Tools
for /f "tokens=2 delims==" %%I in (
  'wmic cpu get architecture /value'
) do set "cpuArch=%%I"
if "%cpuArch%"=="12" (
  set HOST_ARCH=arm64
) else (
  set HOST_ARCH=x64
)
set TARGET_ARCH=%HOST_ARCH%
@echo on

if not "%1" == "" (
  set TARGET_ARCH=%1
)

call "%CURDIR%\release_converter_win.bat" %TARGET_ARCH% || exit /b 1
call "%CURDIR%\release_viewer2_win.bat" %TARGET_ARCH% || exit /b 1

