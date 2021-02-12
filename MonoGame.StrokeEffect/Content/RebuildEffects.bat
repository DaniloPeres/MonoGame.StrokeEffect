setlocal

SET TWOMGFX="%UserProfile%\.dotnet\tools\mgcb.exe"

set expanded_list=
for /f "tokens=*" %%F in ('dir /b *.fx') do call set expanded_list=%%expanded_list%% "%%F"

call %TWOMGFX% %expanded_list% /Platform:WindowsStoreApp /outputDir:DirectX
call %TWOMGFX% %expanded_list% /Platform:Android /outputDir:OpenGL

endlocal

pause