rem call $(ProjectDir)CopyDll.bat

mkdir "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSLogPusherService\\AnyCPU"
mkdir "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSLogPusherService\\x64"
mkdir "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSLogPusherService\\x86"

copy "%BAT_PATH%..\\..\\..\\bin\\AnyCPU\\Release\\*.exe"    "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSLogPusherService\\AnyCPU"
copy "%BAT_PATH%..\\..\\..\\bin\\x64\\Release\\*.exe"    "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSLogPusherService\\x64"
copy "%BAT_PATH%..\\..\\..\\bin\\x86\\Release\\*.exe" "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSLogPusherService\\x86"



