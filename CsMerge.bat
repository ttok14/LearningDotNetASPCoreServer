@echo off
setlocal enabledelayedexpansion

set "OUTPUT_FILE=MergedCode_RecursiveCMD.cs"
set "SEPARATOR=_------------------ "

REM 결과 파일을 초기화 (기존 내용 삭제)
> "%OUTPUT_FILE%" echo.
echo.
echo 📂 현재 폴더와 모든 하위 폴더의 .cs 파일을 재귀적으로 합치는 중...
echo.

REM 현재 폴더와 모든 하위 폴더(*.*)에서 .cs 파일을 재귀적으로 찾음 (FOR /R 옵션 사용)
REM %%f 변수에는 파일의 전체 경로가 저장됩니다. (예: C:\Project\SubDir\File.cs)
FOR /R %%f IN (*.cs) DO (
    REM 파일명만 추출
    set "FILE_PATH=%%f"
    set "FILE_NAME=%%~nxf"
    
    REM 파일의 경로(폴더)만 추출
    set "DIR_PATH=%%~dpf"
    
    REM 구분자 설정: _------------------ {폴더경로} {파일명.cs} ---------------
    echo %SEPARATOR%!DIR_PATH!!FILE_NAME! --------------- >> "%OUTPUT_FILE%"
    
    REM 파일 내용 합치기
    type "!FILE_PATH!" >> "%OUTPUT_FILE%"
    
    REM 가독성을 위한 빈 줄 추가
    echo. >> "%OUTPUT_FILE%"
    echo. >> "%OUTPUT_FILE%"
)

echo.
echo ✅ 작업이 완료되었습니다.
echo 결과 파일: %OUTPUT_FILE%
echo.
pause

endlocal