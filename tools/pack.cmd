@echo off

for /f "tokens=* USEBACKQ" %%f in (`type version`) do set version=%%f
set releaseconfig=%1
if "%releaseconfig%" == "" set releaseconfig=Release

:packbin
echo Packing binary...
"%ProgramFiles%\7-Zip\7z.exe" a -tzip %temp%/%version%-bin.zip "..\GRILO.Bootloader\bin\%releaseconfig%\net8.0\*"
"%ProgramFiles%\7-Zip\7z.exe" a -tzip %temp%/%version%-bin48.zip "..\GRILO.Bootloader\bin\%releaseconfig%\net48\*"
"%ProgramFiles%\7-Zip\7z.exe" a -tzip %temp%/%version%-demo.zip "..\GRILO.BootableAppDemo\bin\%releaseconfig%\net8.0\*"
"%ProgramFiles%\7-Zip\7z.exe" a -tzip %temp%/%version%-demo48.zip "..\GRILO.BootableAppDemo\bin\%releaseconfig%\net48\*"
"%ProgramFiles%\7-Zip\7z.exe" a -tzip %temp%/%version%-boot.zip "..\GRILO.Boot\bin\%releaseconfig%\netstandard2.0\*"
if %errorlevel% == 0 goto :complete
echo There was an error trying to pack binary (%errorlevel%).
goto :finished

:complete
move %temp%\%version%-bin.zip
move %temp%\%version%-demo.zip
move %temp%\%version%-bin48.zip
move %temp%\%version%-demo48.zip
move %temp%\%version%-boot.zip

echo Pack successful.
:finished
