@echo off
setlocal
set CURDIR=%~dp0
@echo on

del /S/Q AntTweakBar_116.zip
rmdir /S/Q AntTweakBar
curl -L -o AntTweakBar_116.zip "https://osdn.net/frs/g_redir.php?m=kent&f=anttweakbar%%2FAntTweakBar_116.zip"
unzip AntTweakBar_116.zip
