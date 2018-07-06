@echo off
setlocal
set CURDIR=%~dp0
@echo on

del /S/Q glew-2.1.0
rmdir /S/Q glew-2.1.0-win32
curl -L -O https://github.com/nigels-com/glew/releases/download/glew-2.1.0/glew-2.1.0-win32.zip

unzip glew-2.1.0-win32.zip

