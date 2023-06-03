@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set TOOLSDIR=%BASEDIR%\Tools
@echo on

call "%CURDIR%\release_converter_win_ninja.bat" Release || exit /b 1
call "%CURDIR%\release_viewer2_win_ninja.bat" Release || exit /b 1

