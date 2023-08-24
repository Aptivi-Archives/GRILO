@echo off

for /f "tokens=* USEBACKQ" %%f in (`type version`) do set version=%%f
set releaseconfig=%1
if "%releaseconfig%" == "" set releaseconfig=Release

:packbin
echo Packing binary...
"%ProgramFiles%\WinRAR\rar.exe" a -ep1 -r -m5 %temp%/%version%-bin.rar "..\GRILO.Bootloader\bin\%releaseconfig%\net6.0\"
"%ProgramFiles%\WinRAR\rar.exe" a -ep1 -r -m5 %temp%/%version%-bin48.rar "..\GRILO.Bootloader\bin\%releaseconfig%\net48\"
"%ProgramFiles%\WinRAR\rar.exe" a -ep1 -r -m5 %temp%/%version%-demo.rar "..\GRILO.BootableAppDemo\bin\%releaseconfig%\net6.0\"
"%ProgramFiles%\WinRAR\rar.exe" a -ep1 -r -m5 %temp%/%version%-demo48.rar "..\GRILO.BootableAppDemo\bin\%releaseconfig%\net48\"
"%ProgramFiles%\WinRAR\rar.exe" a -ep1 -r -m5 %temp%/%version%-boot.rar "..\GRILO.Boot\bin\%releaseconfig%\netstandard2.0\"
if %errorlevel% == 0 goto :complete
echo There was an error trying to pack binary (%errorlevel%).
goto :finished

:complete
move %temp%\%version%-bin.rar
move %temp%\%version%-demo.rar
move %temp%\%version%-bin48.rar
move %temp%\%version%-demo48.rar
move %temp%\%version%-boot.rar

echo Pack successful.
:finished
