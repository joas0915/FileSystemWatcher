@echo off

net stop SSEvent

sc delete SSEvent

del "C:\SSEvent\SSEventLogService\x64\Release\SSEventLogService.exe"