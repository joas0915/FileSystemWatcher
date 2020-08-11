using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Timers;
using System.Diagnostics.Eventing;
using System.Data.SqlClient;

namespace SSFSW
{

    class SSFSW
    {

        public static void DisplayEventAndLogInformation(string fileToSearch, DateTime actionTime)
        {
            StringBuilder sb = new StringBuilder();
            const string queryString = @"<QueryList>
                                          <Query Id=""0"" Path=""Security"">
                                            <Select Path=""Security"">*</Select>
                                          </Query>
                                        </QueryList>";
            EventLogQuery eventsQuery = new EventLogQuery("Security", PathType.LogName, queryString);
            eventsQuery.ReverseDirection = true;
            EventLogReader logReader = new EventLogReader(eventsQuery);
            bool isStop = false;
            for (EventRecord eventInstance = logReader.ReadEvent(); null != eventInstance; eventInstance = logReader.ReadEvent())
            {
                foreach (var VARIABLE in eventInstance.Properties)
                    if (VARIABLE.Value.ToString().ToLower().Contains(fileToSearch.ToLower()) && actionTime.ToString("d/M/yyyy HH:mm:ss") == eventInstance.TimeCreated.Value.ToString("d/M/yyyy HH:mm:ss"))
                    {
                        foreach (var VARIABLE2 in eventInstance.Properties) sb.AppendLine(VARIABLE2.Value.ToString());
                        Console.WriteLine("sb : " + sb.ToString());
                        Console.WriteLine((eventInstance.Properties.Count > 1) ? "eventInstance" + eventInstance.Properties[1].Value.ToString() : "eventInstance" + "n/a");
                        Console.WriteLine("fileToSearch" + fileToSearch);
                        isStop = true;
                        break;
                    }
                if (isStop) break;
                try
                {

                }
                catch (Exception e2)
                {
                    Console.WriteLine(e2.Message);
                }
            }

            //            return e;z
        }
        static void InsertEventLog()
        {
            //string FoldertoSearch = "Tester_5.txt";
            string FoldertoSearch = "TesterFolder";
            string AccessType = "";
            string AccessTypeReason = "";
            StringBuilder sb = new StringBuilder();
            const string queryString = @"<QueryList>
                                          <Query Id=""0"" Path=""Security"">
                                            <Select Path=""Security"">*</Select>
                                          </Query>
                                        </QueryList>";
            EventLogQuery eventsQuery = new EventLogQuery("Security", PathType.LogName, queryString);
            eventsQuery.ReverseDirection = true;
            EventLogReader logReader = new EventLogReader(eventsQuery);
            bool isStop = false;
            for (EventRecord eventInstance = logReader.ReadEvent(); null != eventInstance; eventInstance = logReader.ReadEvent())
            {
                foreach (var VARIABLE in eventInstance.Properties)
                    //Console.WriteLine(eventInstance.Properties[7].ToString().Contains(FoldertoSearch));
                    if (VARIABLE.Value.ToString().ToLower().Contains(FoldertoSearch.ToLower()))
                    {
                        AccessType = eventInstance.Properties[11].Value.ToString();
                        AccessType = AccessType.Replace("\r","");
                        AccessType = AccessType.Replace("\t", "");
                        AccessType = AccessType.Replace("%%4416", "ReadData (또는 ListDirectory)");
                        AccessType = AccessType.Replace("%%4417", "WriteData (또는 AddFile)");
                        AccessType = AccessType.Replace("%%4418", "AppendData (또는 AddSubdirectory 디렉터리 또는 CreatePipeInstance)");
                        AccessType = AccessType.Replace("%%4419", "ReadEA(레지스트리 개체의 경우 '하위 키 열거'입니다.)");
                        AccessType = AccessType.Replace("%%4420", "WriteEA");
                        AccessType = AccessType.Replace("%%4421", "DeleteChild");
                        AccessType = AccessType.Replace("%%4423", "ReadAttributes");
                        AccessType = AccessType.Replace("%%4424", "WriteAttributes");
                        AccessType = AccessType.Replace("%%1537", "DELETE");
                        AccessType = AccessType.Replace("%%1538", "READ_CONTROL");
                        AccessType = AccessType.Replace("%%1539", "WRITE_DAC");
                        AccessType = AccessType.Replace("%%1540", "WRITE_OWNER");
                        AccessType = AccessType.Replace("%%1541", "항목과");
                        AccessType = AccessType.Replace("%%1542", "ACCESS_SYS_SEC");

                        AccessTypeReason = eventInstance.Properties[12].Value.ToString();
                        AccessTypeReason = AccessTypeReason.Replace("\r", "");
                        AccessTypeReason = AccessTypeReason.Replace("\t", "");
                        AccessTypeReason = AccessTypeReason.Replace("%%4416", "ReadData (또는 ListDirectory)");
                        AccessTypeReason = AccessTypeReason.Replace("%%4417", "WriteData (또는 AddFile)");
                        AccessTypeReason = AccessTypeReason.Replace("%%4418", "AppendData (또는 AddSubdirectory 디렉터리 또는 CreatePipeInstance)");
                        AccessTypeReason = AccessTypeReason.Replace("%%4419", "ReadEA(레지스트리 개체의 경우 '하위 키 열거'입니다.)");
                        AccessTypeReason = AccessTypeReason.Replace("%%4420", "WriteEA");
                        AccessTypeReason = AccessTypeReason.Replace("%%4421", "DeleteChild");
                        AccessTypeReason = AccessTypeReason.Replace("%%4423", "ReadAttributes");
                        AccessTypeReason = AccessTypeReason.Replace("%%4424", "WriteAttributes");
                        AccessTypeReason = AccessTypeReason.Replace("%%1537", "DELETE");
                        AccessTypeReason = AccessTypeReason.Replace("%%1538", "READ_CONTROL");
                        AccessTypeReason = AccessTypeReason.Replace("%%1539", "WRITE_DAC");
                        AccessTypeReason = AccessTypeReason.Replace("%%1540", "WRITE_OWNER");
                        AccessTypeReason = AccessTypeReason.Replace("%%1541", "항목과");
                        AccessTypeReason = AccessTypeReason.Replace("%%1542", "ACCESS_SYS_SEC");

                        Console.WriteLine("==============================================================");
                        Console.WriteLine("Check " + DateTime.Now.ToString());
                        Console.WriteLine("ID " + eventInstance.Id.ToString());
                        Console.WriteLine("User : " + eventInstance.Properties[1].Value);
                        Console.WriteLine("PC Name : " + eventInstance.Properties[2].Value);
                        Console.WriteLine("Type : " + eventInstance.Properties[4].Value);
                        Console.WriteLine("IP Address : " + eventInstance.Properties[5].Value);
                        Console.WriteLine("Port : " + eventInstance.Properties[5].Value);
                        Console.WriteLine("Path: " + eventInstance.Properties[7].Value);
                        Console.WriteLine("FilePath : " + eventInstance.Properties[8].Value);
                        Console.WriteLine("File: " + eventInstance.Properties[9].Value);
                        Console.WriteLine("Type : " + eventInstance.Properties[10].Value);
                        Console.WriteLine("AccessType : \n" + AccessType);
                        Console.WriteLine("AccessType Reason : \n" + AccessTypeReason);
                        Console.WriteLine("CreateTime : " + eventInstance.TimeCreated.Value.ToString("d/M/yyyy HH:mm:ss"));
                        isStop = true;
                        break;

                        //foreach (var VARIABLE2 in eventInstance.Properties) sb.AppendLine(VARIABLE2.Value.ToString());
                        //Console.WriteLine("==============================================================");
                        //Console.WriteLine("Check " + DateTime.Now.ToString());
                        //Console.WriteLine("ID " + eventInstance.Id.ToString());
                        //Console.WriteLine("sb : " + sb.ToString());
                        //Console.WriteLine((eventInstance.Properties.Count > 1) ? "eventInstance" + eventInstance.Properties[1].Value.ToString() : "eventInstance" + "n/a");
                        //Console.WriteLine(eventInstance.TimeCreated.Value.ToString("d/M/yyyy HH:mm:ss"));
                        //isStop = true;
                        //break;
                    }
                if (isStop) break;
                try
                {
                }
                catch (Exception e2)
                {
                    Console.WriteLine(e2.Message);
                }
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            Timer CycleTimer = new System.Timers.Timer();
            CycleTimer.Interval = 1000;
            CycleTimer.Elapsed += new ElapsedEventHandler(timer_Elapsed); 
            CycleTimer.Start();

            Console.Read();
        }

        // 쓰레드풀의 작업쓰레드가 지정된 시간 간격으로
        // 아래 이벤트 핸들러 실행
        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            InsertEventLog();
        }
        static void FSW()
        {
            string watchPath = @"C:\TesterFolder";
            FileSystemWatcher fsw = new FileSystemWatcher(watchPath);
            fsw.NotifyFilter = NotifyFilters.FileName |
                NotifyFilters.DirectoryName |
                NotifyFilters.Size |
                NotifyFilters.LastAccess |
                NotifyFilters.CreationTime |
                NotifyFilters.Security |
                NotifyFilters.LastWrite;
            fsw.Filter = "*.*"; ; //감시할 파일 유형 선택 예) *.* 모든 파일 
            fsw.Created += new FileSystemEventHandler(Created);
            fsw.Changed += new FileSystemEventHandler(Changed);
            fsw.Renamed += new RenamedEventHandler(Renamed);
            //폴더외 이동시 Move와 Delete 구분불가 (Delete로 동일하게 표기)
            fsw.Deleted += new FileSystemEventHandler(Deleted);

            fsw.EnableRaisingEvents = true;

            Console.WriteLine("Wait until File Created ~~~");
            Console.Read();
        }
        private static void Created(object sender, FileSystemEventArgs e) { Console.WriteLine("{0} {1} {2}", WindowsIdentity.GetCurrent().Name,e.ChangeType.ToString(), e.FullPath); }
        private static void Deleted(object sender, FileSystemEventArgs e) { Console.WriteLine("{0} {1} {2}", WindowsIdentity.GetCurrent().Name, e.ChangeType.ToString(), e.FullPath); }
        private static void Changed(object source, FileSystemEventArgs e)
        {
            DisplayEventAndLogInformation(e.Name,DateTime.Now);
            Console.WriteLine("{0} {1} {2}", WindowsIdentity.GetCurrent().Name, e.ChangeType.ToString(), e.FullPath);
        }
        private static void Renamed(object source, RenamedEventArgs e) { Console.WriteLine("{0} {1} {2}", WindowsIdentity.GetCurrent().Name, e.ChangeType.ToString(), e.FullPath); }

    }
}
