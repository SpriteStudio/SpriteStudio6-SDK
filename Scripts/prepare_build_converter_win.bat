@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
@echo on

pushd %BUILDDIR%\Converter
rmdir /S /Q glfw
git clone https://github.com/glfw/glfw.git || exit /b 1

rmdir /S /Q cppfs
git clone https://github.com/cginternals/cppfs.git || exit /b 1


popd
