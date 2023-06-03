@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set TOOLSDIR=%BASEDIR%\Tools
set DEFAULT_QT_PREFIX=C:\Qt\6.5.1
@echo on

for /f "tokens=2 delims==" %%I in (
  'wmic cpu get architecture /value'
) do set "cpuArch=%%I"
if "%cpuArch%"=="12" (
  set ARCH=arm64
  set DEFAULT_QT_PREFIX=%DEFAULT_QT_PREFIX%\msvc2019_arm64
) else (
  set ARCH=x64
  set DEFAULT_QT_PREFIX=%DEFAULT_QT_PREFIX%\msvc2019_64
)
if "%QT_PREFIX%" == "" (
    set QT_PREFIX=%DEFAULT_QT_PREFIX%
)

call "%CURDIR%\build_converter_win_ninja.bat" Release || exit /b 1
call "%CURDIR%\build_convertergui_win_ninja.bat" Release || exit /b 1

pushd %BASEDIR%
rmdir /S /Q Ss6Converter
mkdir Ss6Converter
copy "%BUILDDIR%\Converter\build\Ss6Converter.exe" Ss6Converter\
copy "%BUILDDIR%\Ss6ConverterGUI\Ss6ConverterGUI\build\Ss6ConverterGUI.exe" Ss6Converter\ /E
if "%ARCH%"=="arm64" (
    set QTPATHS=%QT_PREFIX%\bin\qtpaths6.bat
) else (
    set QTPATHS=%QT_PREFIX%\bin\qtpaths6.exe
)
%QT_PREFIX%\..\msvc2019_64\bin\windeployqt6.exe --qtpaths %QTPATHS% --release Ss6Converter\

powershell compress-archive Ss6Converter Ss6Converter.zip
move /y Ss6Converter.zip "%TOOLSDIR%\"
rmdir /S /Q Ss6Converter
popd