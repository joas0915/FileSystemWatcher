@echo off

net stop SSLogPusher

sc delete SSLogPusher

del "C:\SSLogPusher\SSLogPusherService\x64\SSLogPusherService.exe"