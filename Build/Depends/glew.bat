@echo off
setlocal
set CURDIR=%~dp0
@echo on

del /S/Q %CURDIR%\glew-2.1.0
rmdir /S/Q %CURDIR%\glew-2.1.0-win32
curl -L -o %CURDIR%\glew-2.1.0-win32.zip https://github.com/nigels-com/glew/releases/download/glew-2.1.0/glew-2.1.0-win32.zip

unzip %CURDIR%\glew-2.1.0-win32.zip -d %CURDIR%