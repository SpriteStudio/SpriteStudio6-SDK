@echo off
setlocal

choco install -y unzip || exit /b 1
choco install -y curl || exit /b 1
choco install -y cmake --installargs 'ADD_CMAKE_TO_PATH=System' || exit /b 1
choco install -y zip || exit /b 1
refreshenv || exit /b 1