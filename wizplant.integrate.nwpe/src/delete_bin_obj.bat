@echo off
setlocal enabledelayedexpansion
::����һ���ļ���
for /f "delims=" %%a in ('dir /ad /b ') do (
for /f "delims=" %%b in ('dir /ad /b %%a') do (
	if /i '%%b' == 'bin' (
		echo rd %%a\%%b /s /q
		rd %%a\%%b /s /q
 	) else	( 
		if /i '%%b' == 'obj' (
			echo rd %%a\%%b /s /q
			rd %%a\%%b /s /q
 		) else	( 
			if /i '%%b' == 'logs' (
				echo rd %%a\%%b /s /q
				rd %%a\%%b /s /q
 			)
        	)
        )
)
 rem set str=%%a 

)

pause