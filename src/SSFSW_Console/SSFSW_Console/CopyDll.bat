rem call $(ProjectDir)CopyDll.bat

mkdir "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSFSW_Console\\AnyCPU"
mkdir "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSFSW_Console\\x64"
mkdir "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSFSW_Console\\x86"

copy "%BAT_PATH%..\\..\\..\\bin\\AnyCPU\\Release\\*.exe"    "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSFSW_Console\\AnyCPU"
copy "%BAT_PATH%..\\..\\..\\bin\\x64\\Release\\*.exe"    "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSFSW_Console\\x64"
copy "%BAT_PATH%..\\..\\..\\bin\\x86\\Release\\*.exe" "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSFSW_Console\\x86"



