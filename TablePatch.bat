@echo off
REM UTF-8 ?쒓? 異쒕젰???꾪빐 肄붾뱶?섏씠吏 蹂寃?(源⑥쭚 諛⑹?)
chcp 65001 > nul
setlocal enabledelayedexpansion

REM ==================================================================================
REM [?ㅼ젙 援ш컙]
REM ==================================================================================

SET "ROOT_DIR=%~dp0"
SET "TEMP_CSV_DIR=%ROOT_DIR%Temp_TableCSV"
SET "TEMP_ARTIFACT_DIR=%ROOT_DIR%Temp_MsgPackToolArtifact"

SET "DOWNLOADER_FOLDER=TableDownloader"
SET "DOWNLOADER_EXE_NAME=TableExporter.exe"

SET "GENERATOR_FOLDER=MsgPackTool"
SET "GENERATOR_EXE_NAME=MSgPackBinaryGenerator.exe"

REM ==================================================================================
REM [Step 0] ?덉쟾??寃??(Pre-flight Check)
REM ==================================================================================
echo [Pipeline] 0. 珥덇린 ?곹깭 ?먭? 以?..

REM 寃곌낵(Artifact) ?대뜑媛 ?대? ?덉쑝硫??ш퀬 諛⑹?瑜??꾪빐 以묐떒
if exist "%TEMP_ARTIFACT_DIR%" goto ERROR_ARTIFACT_EXISTS
goto STEP0_PASS

:ERROR_ARTIFACT_EXISTS
    echo.
    echo [CRITICAL ERROR] ----------------------------------------------------
    echo ?덉쟾???꾪빐 ?묒뾽??以묐떒?⑸땲??
    echo ?꾩떆 寃곌낵 ?대뜑媛 ?대? 議댁옱?⑸땲??
    echo "%TEMP_ARTIFACT_DIR%"
    echo ---------------------------------------------------------------------
    pause
    exit /b 1

:STEP0_PASS

REM ==================================================================================
REM [Step 0.5] CSV ?대뜑 ?ъ쟾 泥?냼 (Clean Before Download ONLY)
REM ==================================================================================
REM ?ㅼ슫濡쒕뱶 ?쒖옉 ?꾩뿉留?湲곗〈 ?뚯씪???뺣━?⑸땲?? (?묒뾽 ?꾩뿉???뺤씤???꾪빐 ?④꺼??

if not exist "%TEMP_CSV_DIR%" goto STEP05_MKDIR

    echo [Clean] ?ㅼ슫濡쒕뱶瑜??꾪빐 湲곗〈 CSV ?뚯씪?ㅼ쓣 ?뺣━?⑸땲??..
    REM ?먮윭 ?듭젣瑜??꾪빐 2>nul 異붽?
    del /q "%TEMP_CSV_DIR%\*.csv" 2>nul
    goto STEP05_DONE

:STEP05_MKDIR
    mkdir "%TEMP_CSV_DIR%"

:STEP05_DONE

REM ==================================================================================
REM [Step 1] ?뚯씠釉??ㅼ슫濡쒕뱶 (Python Tool)
REM ==================================================================================
echo.
echo [Pipeline] 1. 援ш? ?ㅽ봽?덈뱶?쒗듃 ?ㅼ슫濡쒕뱶 ?쒖옉...
echo --------------------------------------------------

pushd "%ROOT_DIR%%DOWNLOADER_FOLDER%"

if exist "%DOWNLOADER_EXE_NAME%" goto STEP1_RUN
    echo [ERROR] ?뚯씪??李얠쓣 ???놁뒿?덈떎: %CD%\%DOWNLOADER_EXE_NAME%
    popd
    goto ERROR_EXIT

:STEP1_RUN
    REM Config ?뺤씤 (?⑥닚 寃쎄퀬?? ??뚮Ц???곴??놁씠 泥댄겕??
    if exist "config.txt" goto STEP1_EXEC
    if exist "Config.txt" goto STEP1_EXEC
    echo [WARNING] 'config.txt'媛 蹂댁씠吏 ?딆뒿?덈떎. (?ㅽ뻾?먮뒗 臾몄젣 ?놁쓣 ???덉쓬)

:STEP1_EXEC
    "%DOWNLOADER_EXE_NAME%" --output "%TEMP_CSV_DIR%"

    if %ERRORLEVEL% EQU 0 goto STEP1_SUCCESS
    echo.
    echo [FAIL] ?뚯씠釉??ㅼ슫濡쒕뱶 ?ㅽ뙣.
    popd
    goto ERROR_EXIT

:STEP1_SUCCESS
    popd

REM ==================================================================================
REM [Step 2] MsgPack ?앹꽦 諛?諛고룷 (C# Tool)
REM ==================================================================================
echo.
echo [Pipeline] 2. MessagePack ?앹꽦 諛?諛고룷 ?쒖옉...
echo --------------------------------------------------

pushd "%ROOT_DIR%%GENERATOR_FOLDER%"

if exist "%GENERATOR_EXE_NAME%" goto STEP2_RUN
    echo [ERROR] ?뚯씪??李얠쓣 ???놁뒿?덈떎: %CD%\%GENERATOR_EXE_NAME%
    popd
    goto ERROR_EXIT

:STEP2_RUN
    "%GENERATOR_EXE_NAME%" -i "%TEMP_CSV_DIR%" -o "%TEMP_ARTIFACT_DIR%" -p Native

    if %ERRORLEVEL% EQU 0 goto STEP2_SUCCESS
    echo.
    echo [FAIL] MessagePack ?앹꽦 ?ㅽ뙣.
    popd
    goto ERROR_EXIT

:STEP2_SUCCESS
    popd

REM ==================================================================================
REM [Step 3] 泥?냼 (Cleanup)
REM ==================================================================================
echo.
echo [Pipeline] 3. ?꾩떆 ?뚯씪 ?뺣━(Cleanup)...
echo --------------------------------------------------

REM [蹂寃? ?ㅼ슫濡쒕뱶??CSV ?뚯씪? ?붾쾭源낆쓣 ?꾪빐 ??젣?섏? ?딄퀬 ?④꺼?〓땲??
echo   - CSV Temp Folder Kept: %TEMP_CSV_DIR%

REM MsgPack 寃곌낵 ?대뜑???앹꽦臾?Artifact)?대?濡??듭㎏濡???젣 (?덉쟾)
if exist "%TEMP_ARTIFACT_DIR%" (
    echo   - Deleting Artifact Temp: %TEMP_ARTIFACT_DIR%
    rmdir /s /q "%TEMP_ARTIFACT_DIR%"
)

echo.
echo ==================================================
echo      [SUCCESS] 紐⑤뱺 ?뚯씠?꾨씪?몄씠 ?꾨즺?섏뿀?듬땲??
echo ==================================================
pause
exit /b 0

:ERROR_EXIT
echo.
echo [Pipeline Aborted] ?ㅻ쪟濡??명빐 ?묒뾽??以묐떒?섏뿀?듬땲??
pause
exit /b 1
