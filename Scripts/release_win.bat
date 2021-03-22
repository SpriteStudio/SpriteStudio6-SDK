@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set TOOLSDIR=%BASEDIR%\Tools
@echo on

call "%CURDIR%\release_converter_win.bat" Release || exit /b 1
rem call "%CURDIR%\release_viewer2_win.bat" Release || exit /b 1

