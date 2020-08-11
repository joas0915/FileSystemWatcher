MultiLanguageVer1.2

소속 : 수현소프트
작성 : 이현진 대리
날짜 : 2020.02.04
환경 : Window 10 Pro 64bit / 코딩환경 : 32bit 구성환경

실행파일 : MultiLanguage\\sample\\MultiLanguage.exe

UTF-8 타입의 TextFile을 지원합니다.
다른 언어체계의 경우 언어타입을 변경해서 사용하기를 권장합니다.

1. UTF8 환경의 TextFile
2. 솔루션 파일만 사용할 예정(.cs / .vb 등등) 
  *.cpp 해당안됨

생성되는 File : info.ini, result.csv

info.ini -> 언어변환에 사용되는 파일 입니다.
result.csv -> 선택된 솔루션 경로에 선택된 확장자 타입의 파일을 읽어 한글만 추출하여 csv 타입으로 출력합니다.
 -result.csv는 한번 진행 후, 다시 사용되는 것을 막기위해 자동으로 result년월일시분초.bak로 변환되고, 새로이 생성됩니다.
   Ex) reslut.csv -> result20200204012256.bak

MultiLanguage : Main Program으로 다국어 지원을 위해 사용될 프로그램
Stringdll : 변경되는 Main Program을 Test하기 위핸 Dll 생성 파일 (DLL Resource를 생성하는데 사용됩니다.)  * 원소스에서 사용되는 Source이므로, MultiLanguage 사용에 관여하지 않습니다.
TestSource : 생성된 DLL Resource를 사용하여 Data 출력을 확인하는 프로그램 입니다.  * 원소스에서 사용되는 Source이므로, MultiLanguage 사용에 관여하지 않습니다.

다국어 지원을 위해 제작된 프로그램입니다.

1. 초기 실행시, [신규진행]을 클릭합니다.
2. 수정을 원하는 파일의 확장자를 [확장자 타입]에 입력합니다.
3. 솔루션 폴더의 경로를 [솔루션 경로]에 입력합니다. (주소 복사&붙여넣기 또는 공란으로 사용가능)
  - 솔루션 폴더의 경로를 입력하지 않고, [CSV생성]을 누르게 되면 폴더 검색창이 나타납니다.
4. 언어설정의 경우 지금 접속한 OS의 언어정보를 읽어오는 역할입니다. (자동 설정)
5. 솔루션 경로, 확장자 타입의 입력이 완료 시 CSV 생성을 진행합니다.
6. 설정된 경로에 Result.csv 파일의 생성을 확인합니다.
7. [언어변환]의 버튼으로 다음 시퀀스를 진행합니다.
8. 파일의 내용이 변환되었습니다.

Ps1. 변환방식 "한글" -> LanValue(0); 같은 방식으로 변환됩니다.
  * Cpp 방식에서는 이러한 방법이 더 편리합니다.
Ps1. 변환방식 "한글" -> LanValue_n; 같은 방식으로 변환됩니다.  
  * CS 방식에서는 이러한 방법이 더 편리합니다.

9. [중간 수정 진행]를 사용하는 경우는 이미 한번 변환된 솔루션을 한번더 변경하기 위함입니다.
  - 한번 변환된 솔루션을 신규진행으로 진행하면, LanValue(0)에서 0번부터 진행되기 때문에 작동은 되나 다른값이 출력됩니다.
  - 때문에, 한번 변환된 솔루션은 중간수정 진행으로 작동하여 LanValue(n)에서 n의 최대값으로 시작하게 합니다.

10. 수정된 소스에서 작업을 한번만 더 진행하면 되겠습니다.
  - Cpp의 경우 LanValue(n)이 적용되는 함수를 설정하여, Resource를 연력해주어야 합니다. (중요)
  - CS의 경우 LanValue_n이 적용되는 Resource가 존재해야하고, 이를 연결해주어야합니다. (중요)
     * Ex) Resource1.LanValue_1으로 함수를 변경해줍니다.
	       Resource1에 String 타이브로 LanValue_1을 설정하고, 값을 입력합니다.

Ex) 원문 -> "한글1", "한글2",...."한글100"  
Ex) 최초 시작 -> LanValue(0),LanValue(1)....LanValue(100)
Ex) [신규진행] 후, 추가 -> LanValue(0),LanValue(1)....LanValue(100),"한글100","한글101"
Ex) 한번 변환된 솔루션을 [신규진행]으로 하는경우 ->LanValue(0),LanValue(1)....LanValue(100),LanValue(0),LanValue(1)
Ex) 한번 변환된 솔루션을 [중간수정진행]으로 하는경우 ->LanValue(0),LanValue(1)....LanValue(100),LanValue(101),LanValue(102)

중간수정진행은 한번 변환된 솔루션을 다시 진행하는 경우에 위와 같이 사용됩니다.

버그 및 문제가 발생하는 경우, 캡쳐자료와 솔루션파일을 첨부해주시면 진행이 빠르게 됩니다.

감사합니다.