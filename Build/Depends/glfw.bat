@echo off
setlocal
set CURDIR=%~dp0
@echo on

del /S/Q glfw-3.2.1.bin.WIN32.zip
del /S/Q glfw-3.2.1.bin.WIN64.zip
rmdir /S/Q glfw-3.2.1.bin.WIN32
rmdir /S/Q glfw-3.2.1.bin.WIN64
curl -L -O "https://github.com/glfw/glfw/releases/download/3.2.1/glfw-3.2.1.bin.WIN32.zip"
curl -L -O "https://github.com/glfw/glfw/releases/download/3.2.1/glfw-3.2.1.bin.WIN64.zip"

unzip glfw-3.2.1.bin.WIN32.zip
unzip glfw-3.2.1.bin.WIN64.zip
