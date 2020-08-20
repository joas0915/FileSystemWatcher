rem call $(ProjectDir)CopyDll.bat

mkdir "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSLogPusherViewer\\AnyCPU"
mkdir "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSLogPusherViewer\\x64"
mkdir "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSLogPusherViewer\\x86"

copy "%BAT_PATH%..\\..\\..\\bin\\AnyCPU\\Release\\*.exe"    "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSLogPusherViewer\\AnyCPU"
copy "%BAT_PATH%..\\..\\..\\bin\\x64\\Release\\*.exe"    "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSLogPusherViewer\\x64"
copy "%BAT_PATH%..\\..\\..\\bin\\x86\\Release\\*.exe" "%BAT_PATH%..\\..\\..\\..\\..\\..\\Sample\\SSLogPusher\\SSLogPusherViewer\\x86"



