@echo off
setlocal enabledelayedexpansion

:: 1. 대상 폴더 설정 및 초기화
set "TARGET_DIR=CopiedSources"

:: 기존 폴더가 존재하면 하위 디렉토리와 파일을 모두 강제 삭제 (/S /Q)
if exist "%TARGET_DIR%" (
    echo [Info] Cleaning existing %TARGET_DIR%...
    rd /s /q "%TARGET_DIR%"
)
mkdir "%TARGET_DIR%"

:: 2. srcCatalogs.txt 확인
if not exist "srcCatalogs.txt" (
    echo [Error] srcCatalogs.txt not found.
    pause
    exit /b
)

:: 3. 파일 읽기 및 복사 실행
for /f "usebackq delims=" %%A in ("srcCatalogs.txt") do (
    set "RAW_PATH=%%A"
    set "FIXED_PATH=!RAW_PATH:/=\!"
    
    if "!FIXED_PATH:~-1!"=="\" (
        echo [Folder] !FIXED_PATH!
        xcopy "!FIXED_PATH!*.cs" "%TARGET_DIR%\!FIXED_PATH!" /S /I /Y /C
    ) else (
        echo [File] !FIXED_PATH!
        echo f | xcopy "!FIXED_PATH!" "%TARGET_DIR%\!FIXED_PATH!" /Y /I /C
    )
)

echo.
echo Operation Completed.
pause