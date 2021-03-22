@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
@echo on

pushd %BUILDDIR%\Converter
rmdir /S /Q glfw
git clone https://github.com/glfw/glfw.git || exit /b 1

rmdir /S /Q filesystem
git clone https://github.com/gulrak/filesystem.git || exit /b 1

rmdir /S /Q zlib
git clone https://github.com/madler/zlib.git  || exit /b 1

rmdir /S /Q json
git clone https://github.com/nlohmann/json.git || exit /b 1


popd
