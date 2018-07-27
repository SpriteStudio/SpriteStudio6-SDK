@echo off
setlocal
set CURDIR=%~dp0
@echo on

cd %CURDIR%
del /S/Q %CURDIR%\AntTweakBar_116.zip
rmdir /S/Q %CURDIR%\AntTweakBar
curl -L -o %CURDIR%\AntTweakBar_116.zip "https://osdn.net/frs/g_redir.php?m=kent&f=anttweakbar%%2FAntTweakBar_116.zip"
unzip %CURDIR%\AntTweakBar_116.zip -d %CURDIR%