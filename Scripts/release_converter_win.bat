@echo off
setlocal
set CURDIR=%~dp0
set BASEDIR=%CURDIR%..
set BUILDDIR=%BASEDIR%\Build
set TOOLSDIR=%BASEDIR%\Tools
set DEFAULT_QT_PREFIX=C:\Qt\6.8.2
if /I "%PROCESSOR_ARCHITECTURE%" == "AMD64" (
  set HOST_ARCH=x64
) else (
  set HOST_ARCH=%PROCESSOR_ARCHITECTURE%
)
set TARGET_ARCH=%HOST_ARCH%

@echo on

if not "%1" == "" (
  set TARGET_ARCH=%1
)
if /I "%TARGET_ARCH%" == "ARM64" (
  set DEFAULT_QT_PREFIX=%DEFAULT_QT_PREFIX%\msvc2022_arm64
) else (
  set DEFAULT_QT_PREFIX=%DEFAULT_QT_PREFIX%\msvc2022_64
)

if "%QT_PREFIX%" == "" (
    set QT_PREFIX=%DEFAULT_QT_PREFIX%
)

if /I "%TARGET_ARCH%" == "ARM64" (
  set QTPATHS=%QT_PREFIX%\bin\qtpaths6.bat
) else (
  set QTPATHS=%QT_PREFIX%\bin\qtpaths6.exe
)

call "%CURDIR%\build_converter_win.bat" Release %TARGET_ARCH% || exit /b 1
call "%CURDIR%\build_convertergui_win.bat" Release %TARGET_ARCH% || exit /b 1

pushd %BASEDIR%
rmdir /S /Q Ss6Converter
mkdir Ss6Converter

copy "%BUILDDIR%\Converter\build\Release\Ss6Converter.exe" Ss6Converter\
where ninja >nul 2>nul
if ERRORLEVEL 1 (
  robocopy "%BUILDDIR%\Ss6ConverterGUI\Ss6ConverterGUI\build\Release" Ss6Converter /E
) else (
  copy "%BUILDDIR%\Ss6ConverterGUI\Ss6ConverterGUI\build\Ss6ConverterGUI.exe" Ss6Converter\
  %QT_PREFIX%\..\msvc2022_64\bin\windeployqt6.exe --qtpaths %QTPATHS% Ss6Converter\
)
set ZIPNAME=Ss6Converter_%TARGET_ARCH%
powershell compress-archive Ss6Converter %ZIPNAME%.zip
move /y %ZIPNAME%.zip "%TOOLSDIR%\"
rmdir /S /Q Ss6Converter
popd
