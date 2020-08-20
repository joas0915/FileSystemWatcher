@echo **********************************************
@echo *****       SecuARCON Port Mapping       *****
@echo *****                                    *****
@echo *****       Suhyunsoft Co,.Ltd.          *****
@echo **********************************************

set /p arg=SecuARCON Port Mapping (on/off)? : 

@echo off
IF %arg% == on goto on
IF %arg% == off goto off

goto exit

@echo on
:on
set /p arg1=SecuARCON DB G/W IP 주소(ex. 192.168.0.1)? : 
@echo off
netsh interface portproxy add v4tov4 listenport=1433 listenaddress=0.0.0.0 connectport=1433 connectaddress=%arg1%
goto exit
   
:off:
@echo off
netsh interface portproxy reset

:exit
