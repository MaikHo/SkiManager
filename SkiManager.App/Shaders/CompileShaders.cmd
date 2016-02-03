@echo off
setlocal
pushd "%~dp0"

set WindowsSdkDir = "C:\Program Files (x86)\Windows Kits\10"
set WindowsSdkVersion = "10.0.10586.0"

set INCLUDEPATH="C:\Program Files (x86)\Windows Kits\10\Include\10.0.10586.0\um"

if not exist %INCLUDEPATH%\d2d1effecthelpers.hlsli ( 
    echo d2d1effecthelpers.hlsli not found.
    goto WRONG_COMMAND_PROMPT
)

call :COMPILE NormalMapFromHeightMapShader.hlsl || goto END
call :COMPILE TerrainShader.hlsl   || goto END

goto END

:COMPILE
    echo.
    echo Compiling %1

    "C:\Program Files (x86)\Windows Kits\10\bin\x86\fxc.exe" %1 /nologo /T lib_4_0_level_9_3_ps_only /D D2D_FUNCTION /D D2D_ENTRY=main /Fl %~n1.fxlib /I %INCLUDEPATH%                        || exit /b
    "C:\Program Files (x86)\Windows Kits\10\bin\x86\fxc.exe" %1 /nologo /T ps_4_0_level_9_3 /D D2D_FULL_SHADER /D D2D_ENTRY=main /E main /setprivate %~n1.fxlib /Fo:%~n1.bin /I %INCLUDEPATH% || exit /b

    del %~n1.fxlib
    exit /b

:WRONG_COMMAND_PROMPT
echo Please run from a Developer Command Prompt for VS2015.

:END
popd
exit /b %errorlevel%

