@echo off
setlocal
set CURDIR=%~dp0
@echo on

del /S/Q %CURDIR%\glfw-3.2.1.bin.WIN32.zip
del /S/Q %CURDIR%\glfw-3.2.1.bin.WIN64.zip
rmdir /S/Q %CURDIR%\glfw-3.2.1.bin.WIN32
rmdir /S/Q %CURDIR%\glfw-3.2.1.bin.WIN64
curl -L -o %CURDIR%\glfw-3.2.1.bin.WIN32.zip "https://github.com/glfw/glfw/releases/download/3.2.1/glfw-3.2.1.bin.WIN32.zip"
curl -L -o %CURDIR%\glfw-3.2.1.bin.WIN64.zip "https://github.com/glfw/glfw/releases/download/3.2.1/glfw-3.2.1.bin.WIN64.zip"

unzip %CURDIR%\glfw-3.2.1.bin.WIN32.zip -d %CURDIR%
unzip %CURDIR%\glfw-3.2.1.bin.WIN64.zip -d %CURDIR%
