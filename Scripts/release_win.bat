@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set TOOLSDIR=%BASEDIR%\Tools
set QT_PREFIX=C:\Qt\5.11.1\msvc2017_64
@echo on

call %CURDIR%\release_converter_win.bat Release || exit /b 1
call %CURDIR%\release_viewer_win.bat Release || exit /b 1
call %CURDIR%\release_viewer2_win.bat Release || exit /b 1
