using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Timers;
using System.Diagnostics.Eventing;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

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
            /////  EventLog Parameter   /////
            string FoldertoSearch = @"C:\TesterFolder";
            string LogCheckTime = "";
            string ID = "";
            string Login_ID = "";
            string UserName= "";
            string PCName = "";
            string Subject= "";
            string IPAddress= "";
            string Port= "";
            string Path= "";
            string FilePath= "";
            string FileName= "";
            string Type= "";
            string AccessType = "";
            string Reason = "";
            string CreateTime= "";
            string Information = "";
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
            int m_index = 0;
            /////  EventLog Parameter   /////

            /////  DBConnect Parameter   /////
            string g_ConnectionStr = @"Data Source=192.168.10.230,7100;Initial Catalog=arcon;Integrated Security=False;User ID=arconsa;Password=arconsa@pass0;Connect Timeout=5;Encrypt=False;TrustServerCertificate=False";
            //string SQL_CONNSTR = @"Network Library=DBMSSOCN;Data Source=192.168.10.230,7100;Initial Catalog=OTP_TEST_DB;User Id=arconsa;Password=arconsa@pass0";

            SqlConnection sqlCon = new SqlConnection(g_ConnectionStr);
            SqlCommand sqlCmd = new SqlCommand();
            /////  DBConnect Parameter   /////

            for (EventRecord eventInstance = logReader.ReadEvent(); null != eventInstance; eventInstance = logReader.ReadEvent())
            {
                foreach (var VARIABLE in eventInstance.Properties)
                {
                    try
                    {
                        if (!VARIABLE.Value.ToString().Contains(FoldertoSearch) && (eventInstance.TaskDisplayName.ToString() == "File System" || eventInstance.TaskDisplayName.ToString() == "Detailed File Share"))
                        {
                            //내 PC에서 조작한경우
                            if (eventInstance.Id.ToString() == "4656")
                            {
                                if (eventInstance.Properties[6].Value.ToString().Replace(@"C:\TesterFolder", "").Length != 0)
                                {
                                    m_index++;
                                    LogCheckTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.sss");
                                    ID = eventInstance.Id.ToString();
                                    UserName = eventInstance.Properties[1].Value.ToString();
                                    PCName = eventInstance.Properties[2].Value.ToString();
                                    Login_ID = eventInstance.Properties[3].Value.ToString();
                                    Information = eventInstance.Properties[4].Value.ToString();
                                    Subject = eventInstance.Properties[5].Value.ToString();
                                    IPAddress = "";
                                    Port = "";
                                    Path = "";
                                    FilePath = eventInstance.Properties[6].Value.ToString();
                                    FileName = "";
                                    Type = "";
                                    AccessType = DataReplace(eventInstance.Properties[9].Value.ToString());
                                    Reason = DataReplace(eventInstance.Properties[10].Value.ToString());
                                    CreateTime = eventInstance.TimeCreated.Value.ToString("yyyy-MM-dd hh:mm:ss.sss");

                                    Console.WriteLine("==============================================================");
                                    Console.WriteLine("Check " + LogCheckTime);
                                    Console.WriteLine("ID : " + ID);
                                    Console.WriteLine("User : " + UserName);
                                    Console.WriteLine("PC Name : " + PCName);
                                    Console.WriteLine("Login_ID : " + Login_ID);
                                    Console.WriteLine("Information : " + Information);
                                    Console.WriteLine("Subject : " + Subject);
                                    Console.WriteLine("FilePath : " + FilePath);
                                    Console.WriteLine("AccessType : \n" + AccessType);
                                    Console.WriteLine("Reason : \n" + Reason);
                                    Console.WriteLine("CreateTime: " + CreateTime);
                                }
                            }
                            //원격에서 접속해서 조작한 경우
                            if (eventInstance.Id.ToString() == "5145")
                            {
                                if (eventInstance.Properties[9].Value.ToString().Replace("\\", "").Length != 0 && eventInstance.Properties[4].Value.ToString() == "File")
                                {
                                    m_index++;
                                    LogCheckTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.sss");
                                    ID = eventInstance.Id.ToString();
                                    UserName = eventInstance.Properties[1].Value.ToString();
                                    PCName = eventInstance.Properties[2].Value.ToString();
                                    Login_ID = eventInstance.Properties[3].Value.ToString();
                                    Information = "";
                                    Subject = eventInstance.Properties[4].Value.ToString();
                                    IPAddress = eventInstance.Properties[5].Value.ToString();
                                    Port = eventInstance.Properties[6].Value.ToString();
                                    Path = eventInstance.Properties[7].Value.ToString();
                                    FilePath = eventInstance.Properties[8].Value.ToString();
                                    FileName = eventInstance.Properties[9].Value.ToString();
                                    Type = eventInstance.Properties[10].Value.ToString();
                                    AccessType = DataReplace(eventInstance.Properties[11].Value.ToString());
                                    Reason = DataReplace(eventInstance.Properties[12].Value.ToString());
                                    CreateTime = eventInstance.TimeCreated.Value.ToString("yyyy-MM-dd hh:mm:ss.sss");

                                    Console.WriteLine("==============================================================");
                                    Console.WriteLine("Check " + LogCheckTime);
                                    Console.WriteLine("ID : " + ID);
                                    Console.WriteLine("User : " + UserName);
                                    Console.WriteLine("PC Name : " + PCName);
                                    Console.WriteLine("Login_ID : " + Login_ID);
                                    Console.WriteLine("Subject : " + Subject);
                                    Console.WriteLine("IP Address : " + IPAddress);
                                    Console.WriteLine("Port : " + Port);
                                    Console.WriteLine("Path: " + Path);
                                    Console.WriteLine("FilePath : " + FilePath);
                                    Console.WriteLine("File: " + FileName);
                                    Console.WriteLine("Type : " + Type);
                                    Console.WriteLine("AccessType : \n" + AccessType);
                                    Console.WriteLine("Reason : \n" + Reason);
                                    Console.WriteLine("CreateTime : " + CreateTime);
                                }
                            }
                            isStop = true;
                            break;
                        }
                        if (ID != "")
                        {
                            sqlCon.Open();
                            sqlCmd.Connection = sqlCon;

                            sqlCmd.CommandText = $"INSERT INTO arcon.dbo.EventLogView(time, id, username, PCname, subject, IPAddress, Port, Path, FilePath, Filename, type, AccessType, Reason, Createtime, Login_ID, Information, Count_index)" +
                                                         $" VALUES ('" + LogCheckTime + "','" + ID + "','" + UserName + "','" + PCName + "','" + Subject + "','" + IPAddress + "','" + Port + "','" + Path + "','" + FilePath + "','" + FileName + "','" + Type + "','" + AccessType + "','" + Reason + "','" + CreateTime + "','" + Login_ID + "','" + Information + "','"+ m_index+"')";

                            sqlCmd.ExecuteNonQuery();
                            sqlCon.Close();
                            
                            eventsQuery.Session.ClearLog("Security");
                        }
                        if (isStop) break;
                    }
                    catch (Exception e2)
                    {
                        sqlCon.Close();
                        Console.WriteLine(e2.Message);
                    }
                }
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            Timer CycleTimer = new System.Timers.Timer();
            CycleTimer.Interval = 5000;
            CycleTimer.Elapsed += new ElapsedEventHandler(timer_Elapsed); 
            CycleTimer.Start();

            Console.Read();
        }
        static string DataReplace(string p_Data)
        {
            string m_Data = p_Data;
            m_Data = m_Data.Replace("\r", "");
            m_Data = m_Data.Replace("\t", "");
            m_Data = m_Data.Replace("%%1537", "DELETE");
            m_Data = m_Data.Replace("%%1538", "READ_CONTROL");
            m_Data = m_Data.Replace("%%1539", "WRITE_DAC");
            m_Data = m_Data.Replace("%%1540", "WRITE_OWNER");
            m_Data = m_Data.Replace("%%1541", "SYNCHRONIZE");
            m_Data = m_Data.Replace("%%1542", "ACCESS_SYS_SEC");
            m_Data = m_Data.Replace("%%1801", "SYNCHRONIZE");
            m_Data = m_Data.Replace("%%4416", "ReadData (또는 ListDirectory)");
            m_Data = m_Data.Replace("%%4417", "WriteData (또는 AddFile)");
            m_Data = m_Data.Replace("%%4418", "AppendData (또는 AddSubdirectory 디렉터리 또는 CreatePipeInstance)");
            m_Data = m_Data.Replace("%%4419", "ReadEA(레지스트리 개체의 경우 [하위 키 열거]입니다.)");
            m_Data = m_Data.Replace("%%4420", "WriteEA");
            m_Data = m_Data.Replace("%%4421", "DeleteChild");
            m_Data = m_Data.Replace("%%4423", "ReadAttributes");
            m_Data = m_Data.Replace("%%4424", "WriteAttributes");
            m_Data = m_Data.Replace("D:(A;;FA;;;BA)", "");
            m_Data = m_Data.Replace("D:(A;;FA;;;BA)", "");
            m_Data = m_Data.Replace("D:(A;OICI;FA;;;WD)", "");
            m_Data = m_Data.Replace("D:(A;OICI;FA;;;WD)", "");

            return m_Data;
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
