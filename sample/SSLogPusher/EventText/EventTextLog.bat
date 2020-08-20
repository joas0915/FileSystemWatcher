
echo %DATE%

call sdate.bat
echo [sdate %SDATE%]

call edate.bat
echo [edate %EDATE%]

set "START_DATE=%SDATE%T15:00:00.000Z"
set "END_DATE=%EDATE%T14:59:59.999Z"
echo EventLog Save = %START_DATE% ~ %END_DATE%

wevtutil qe Security /q:"*[System[TimeCreated[@SystemTime>='%START_DATE%' and @SystemTime<='%END_DATE%']]]" /f:text > "C:\SSEvent\EventText\EventTextLog\%EDATE%_EventTextLog.txt" /rd:false /c:1000000000
