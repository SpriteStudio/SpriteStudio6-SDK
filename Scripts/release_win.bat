@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set TOOLSDIR=%BASEDIR%\Tools
set HOST_ARCH=%PROCESSOR_ARCHITECTURE%
set TARGET_ARCH=%HOST_ARCH%
@echo on

if not "%1" == "" (
  set TARGET_ARCH=%1
)

call "%CURDIR%\release_converter_win.bat" %TARGET_ARCH% || exit /b 1
call "%CURDIR%\release_viewer2_win.bat" %TARGET_ARCH% || exit /b 1

