@echo off
setlocal
set CURDIR=%~dp0
@echo on

call %CURDIR%\AntTweakBar.bat
call %CURDIR%\glew.bat
call %CURDIR%\glfw.bat
