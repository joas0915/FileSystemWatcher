 
@echo off


for /f "tokens=1-3 delims=- " %%A in ('echo %DATE%') do (set YY=%%A&SET MM=%%B&SET DD=%%C)



set /a DD=1%DD%-101
set /a MM=1%MM%-100


if  %DD% GTR 0 goto end
set /a MM=%MM%-1


if %MM% GTR 0 goto leap


set /a DD=31
set /a MM=12
set /a YY=%YY%-1
REM echo %YY%, %MM%, %DD% -> 2005, 12, 31
goto end



:leap
set /a TT=%YY%/4
set /a TT=%TT%*4
if %YY% neq %TT% goto mon%MM%
if %MM% neq 2 goto mon%MM%
set /a DD=29
goto end



:mon1
:mon3
:mon5
:mon7
:mon8
:mon10
set /a DD=31+%DD%
goto end


:mon2
set /a DD=28+%DD%
goto end



:mon4
:mon6
:mon9
:mon11
set /a DD=30+%DD%
goto end


:end
set /a DD=%DD% + 100
set DD=%DD:~1,2%
set /a MM=%MM% + 100
set MM=%MM:~1,2%


:: set date1=%YY%-%MM%-%DD%
:: set date2=%YY%-%MM%
:: set date3=%DD%
:: set date4=%YY%-%MM%-%DD%


:: echo %date1%
:: echo.
:: echo %date2%
:: echo.
:: echo %date3%
:: echo.
:: echo %date4%
:: echo.

set "EDATE=%YY%-%MM%-%DD%"
















