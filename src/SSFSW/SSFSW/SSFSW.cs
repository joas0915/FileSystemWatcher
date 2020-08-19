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
using System.Security.Cryptography;

namespace SSFSW
{

    class SSFSW
    {
        public struct EvLogPara
        {
            public string LoadTime;
            public string EventID;
            public string UserName;
            public string DomainName;
            public string Subject;
            public string LogonID;
            public string PC_IPAddress;
            public string PC_Port;
            public string ShareName;
            public string ShareLocalPath;
            public string FileName;
            public string AccessMask;
            public string AccessList;
            public string AccessReason;
            public string EventTime;
            public string Information;

            public EvLogPara(string t_LoadTime, string t_EventID, string t_UserName, string t_DomainName, string t_Subject, string t_LogonID, string t_PC_IPAddress, string t_PC_Port, string t_ShareName, string t_ShareLocalPath, string t_FileName, string t_AccessMask, string t_AccessList, string t_AccessReason, string t_EventTime, string t_Information)
            {
                LoadTime = t_LoadTime;
                EventID = t_EventID;
                UserName = t_UserName;
                DomainName = t_DomainName;
                Subject = t_Subject;
                LogonID = t_LogonID;
                PC_IPAddress = t_PC_IPAddress;
                PC_Port = t_PC_Port;
                ShareName = t_ShareName;
                ShareLocalPath = t_ShareLocalPath;
                FileName = t_FileName;
                AccessMask = t_AccessMask;
                AccessList = t_AccessList;
                AccessReason = t_AccessReason;
                EventTime = t_EventTime;
                Information = t_Information;
            }
            public override string ToString() => $"({LoadTime},{EventID},{UserName},{DomainName},{Subject},{LogonID},{PC_IPAddress},{PC_Port},{ShareName},{ShareLocalPath},{FileName},{AccessMask},{AccessList},{AccessReason},{EventTime},{Information})";
        }

        static string[] LanguageFilter = new string[3];
        static string g_FoldertoSearch = "";
        static string g_UserName = "";
        static string g_FSWLog = "";

        static bool Triger = false;
        static bool TF = true;

        static DateTime TC_1 = new DateTime(0);
        static DateTime TC_2 = new DateTime(0);
        static DateTime Ev_TC = new DateTime(0);

        static DateTime TrialTime = new DateTime(0);
        static DateTime LimitTime = new DateTime(2020,08,27,00,00,00);

        static string[] lines = null;

        static void InsertEventLog()
        {

            if (Triger)
            {
                Console.WriteLine("Already On");
                return;
            }
            else
            {
                Console.WriteLine("LogSearchStart");
                Triger = true;
            }
            
            EvLogPara nevlp = new EvLogPara("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            EvLogPara oevlp = new EvLogPara("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");

            TC_2 = TC_1;

            //if (File.Exists(@"C:\ARCON\aosvc.cfg"))
            //{
            //    StreamReader rdr = new StreamReader(@"C:\ARCON\aosvc.cfg");
            //    FoldertoSearch = rdr.ReadLine();
            //    rdr.Close();
            //}

            StringBuilder sb = new StringBuilder();
            const string queryString = @"<QueryList>
                                          <Query Id=""0"" Path=""Security"">
                                            <Select Path=""Security"">*</Select>
                                          </Query>
                                        </QueryList>";
            EventLogQuery eventsQuery = new EventLogQuery("Security", PathType.LogName, queryString);
            eventsQuery.ReverseDirection = true;
            EventLogReader logReader = new EventLogReader(eventsQuery);

            //string g_ConnectionStr = @"Data Source=192.168.10.230,7100;Initial Catalog=arcon;Integrated Security=False;User ID=arconsa;Password=arconsa@pass0;Connect Timeout=5;Encrypt=False;TrustServerCertificate=False";
            string g_ConnectionStr = @"Data Source=127.0.0.1,1433;Initial Catalog=Eventlog;Integrated Security=False;User ID=eventsa;Password=eventsa@pass0;Connect Timeout=5;Encrypt=False;TrustServerCertificate=False";
            SqlCommand sqlCmd = new SqlCommand();
            SqlConnection sqlCon = new SqlConnection(g_ConnectionStr);

            for (EventRecord eventInstance = logReader.ReadEvent(); null != eventInstance; eventInstance = logReader.ReadEvent())
            {
                foreach (var VARIABLE in eventInstance.Properties)
                {
                    if (!VARIABLE.Value.ToString().Contains(g_FoldertoSearch))
                        continue;
                    if (!TF)
                    {
                        if (DateTime.Compare(TC_2, (DateTime)eventInstance.TimeCreated) != -1)
                            continue;
                    }
                    try
                    {
                        if ((eventInstance.TaskDisplayName.ToString() == LanguageFilter[0] || eventInstance.TaskDisplayName.ToString() == LanguageFilter[1] || eventInstance.TaskDisplayName.ToString() == LanguageFilter[2]))
                        {
                            if (eventInstance.Id.ToString() == "4656")//본인 PC
                            {
                                if (eventInstance.Properties[6].Value.ToString().Replace(g_FoldertoSearch, "").Length != 0)
                                {
                                    nevlp.LoadTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.sss");
                                    nevlp.EventID = eventInstance.Id.ToString();
                                    nevlp.UserName = eventInstance.Properties[1].Value.ToString();
                                    nevlp.DomainName = eventInstance.Properties[2].Value.ToString();
                                    nevlp.LogonID = eventInstance.Properties[3].Value.ToString();
                                    nevlp.Information = eventInstance.Properties[4].Value.ToString();
                                    nevlp.Subject = eventInstance.Properties[5].Value.ToString();
                                    nevlp.PC_IPAddress = "";
                                    nevlp.PC_Port = "";
                                    nevlp.ShareName = "";
                                    nevlp.ShareLocalPath = eventInstance.Properties[6].Value.ToString();
                                    nevlp.FileName = Path.GetFileName(eventInstance.Properties[6].Value.ToString());
                                    nevlp.AccessMask = "";
                                    nevlp.AccessList = DataReplace(eventInstance.Properties[9].Value.ToString());
                                    nevlp.AccessReason = DataReplace(eventInstance.Properties[10].Value.ToString());
                                    nevlp.EventTime = eventInstance.TimeCreated.Value.ToString("yyyy-MM-dd HH:mm:ss.sss");

                                    Console.WriteLine("==============================================================");
                                    Console.WriteLine("LoadTime " + nevlp.LoadTime);
                                    Console.WriteLine("EventID : " + nevlp.EventID);
                                    Console.WriteLine("UserName : " + nevlp.UserName);
                                    Console.WriteLine("DomainName : " + nevlp.DomainName);
                                    Console.WriteLine("LogonID : " + nevlp.LogonID);
                                    Console.WriteLine("Information : " + nevlp.Information);
                                    Console.WriteLine("Subject : " + nevlp.Subject);
                                    Console.WriteLine("ShareLocalPath : " + nevlp.ShareLocalPath);
                                    Console.WriteLine("FileName : " + nevlp.FileName);
                                    Console.WriteLine("AccessList : \n" + nevlp.AccessList);
                                    Console.WriteLine("AccessReason : \n" + nevlp.AccessReason);
                                    Console.WriteLine("EventTime: " + nevlp.EventTime);
                                }
                            }
                            if (eventInstance.Id.ToString() == "5145")//공유 폴더
                            {
                                if (eventInstance.Properties[9].Value.ToString().Replace("\\", "").Length != 0 && eventInstance.TaskDisplayName.ToString() == LanguageFilter[1])
                                {
                                    nevlp.LoadTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.sss");
                                    nevlp.EventID = eventInstance.Id.ToString();
                                    nevlp.UserName = eventInstance.Properties[1].Value.ToString();
                                    nevlp.DomainName = eventInstance.Properties[2].Value.ToString();
                                    nevlp.Subject = eventInstance.Properties[4].Value.ToString();
                                    nevlp.LogonID = eventInstance.Properties[3].Value.ToString();
                                    nevlp.PC_IPAddress = eventInstance.Properties[5].Value.ToString();
                                    nevlp.PC_Port = eventInstance.Properties[6].Value.ToString();
                                    nevlp.ShareName = eventInstance.Properties[7].Value.ToString();
                                    nevlp.ShareLocalPath = eventInstance.Properties[8].Value.ToString();
                                    nevlp.FileName = eventInstance.Properties[9].Value.ToString();
                                    nevlp.AccessMask = eventInstance.Properties[10].Value.ToString();
                                    nevlp.AccessList = DataReplace(eventInstance.Properties[11].Value.ToString());
                                    nevlp.AccessReason = DataReplace(eventInstance.Properties[12].Value.ToString());
                                    nevlp.EventTime = eventInstance.TimeCreated.Value.ToString("yyyy-MM-dd HH:mm:ss.sss");
                                    nevlp.Information = "";

                                    Console.WriteLine("==============================================================");
                                    Console.WriteLine("LoadTime " + nevlp.LoadTime);
                                    Console.WriteLine("EventID : " + nevlp.EventID);
                                    Console.WriteLine("UserName : " + nevlp.UserName);
                                    Console.WriteLine("DomainName : " + nevlp.DomainName);
                                    Console.WriteLine("Subject : " + nevlp.Subject);
                                    Console.WriteLine("LogonID : " + nevlp.LogonID);
                                    Console.WriteLine("PC_IPAddress : " + nevlp.PC_IPAddress);
                                    Console.WriteLine("PC_Port : " + nevlp.PC_Port);
                                    Console.WriteLine("ShareName: " + nevlp.ShareName);
                                    Console.WriteLine("ShareLocalPath : " + nevlp.ShareLocalPath);
                                    Console.WriteLine("FileName: " + nevlp.FileName);
                                    Console.WriteLine("AccessMask : " + nevlp.AccessMask);
                                    Console.WriteLine("AccessList : \n" + nevlp.AccessList);
                                    Console.WriteLine("AccessReason : \n" + nevlp.AccessReason);
                                    Console.WriteLine("CreateTime : " + nevlp.EventTime);
                                }
                            }
                        }
                        if (nevlp.EventID != "" && hasing(oevlp, nevlp))
                        {
                            sqlCon.Open();
                            sqlCmd.Connection = sqlCon;

                            sqlCmd.CommandText = $"INSERT INTO Eventlog.dbo.EventLogView(LoadTime, EventID, UserName, DomainName, Subject, PC_IPAddress, PC_Port, ShareName, ShareLocalPath, FileName, AccessMask, AccessList, AccessReason, EventTime, LogonID, Information)" +
                                                         $" VALUES ('" + nevlp.LoadTime + "','" + nevlp.EventID + "','" + nevlp.UserName + "','" + nevlp.DomainName + "','" + nevlp.Subject + "','" + nevlp.PC_IPAddress + "','" + nevlp.PC_Port + "','" + nevlp.ShareName + "','" + nevlp.ShareLocalPath + "','" + nevlp.FileName + "','" + nevlp.AccessMask + "','" + nevlp.AccessList + "','" + nevlp.AccessReason + "','" + nevlp.EventTime + "','" + nevlp.LogonID + "','" + nevlp.Information + "')";

                            oevlp = nevlp;
                            sqlCmd.ExecuteNonQuery();
                            sqlCon.Close();

                            if(DateTime.Compare(TC_1, (DateTime)eventInstance.TimeCreated) == -1)
                                TC_1 = (DateTime)eventInstance.TimeCreated;

                            TF = false;

                            Console.WriteLine("insert Data");
                        }
                    }
                    catch (Exception e2)
                    {
                        sqlCon.Close();
                        Console.WriteLine(e2.Message);
                    }
                }
            }
            Triger = false;
            //Event Delete
            //eventsQuery.Session.ClearLog("Security");
        }

        static void Main(string[] args)
        {
            if (File.Exists(@"C:\SSEvent\EventLog\SSEvent.cfg"))
            {
                try
                {
                    string[] lines = File.ReadAllLines(@"C:\SSEvent\EventLog\SSEvent.cfg");
                    g_FoldertoSearch = lines[0];
                    g_UserName = lines[1];
                }
                catch (Exception e2)
                {
                    using (StreamWriter sw = new StreamWriter(@"C:\SSEvent\EventLog\Error.log"))
                    {
                        sw.WriteLine("필요한 파라메터가 없습니다.");
                        sw.WriteLine(e2.Message);
                        sw.Close();
                    }
                }
            }

            //Initialize();
            //if (DateTime.Compare(TrialTime.AddDays(7), DateTime.Now) != 1)
            //    return;
            //if (DateTime.Compare(LimitTime, DateTime.Now) != 1)
            //    return;
            //
            //Console.WriteLine("Start");
            //LogWrite("Start");
            //Timer CycleTimer = new System.Timers.Timer();
            ////CycleTimer.Interval = 5 * 60 * 1000;
            //CycleTimer.Interval = 5000;
            //CycleTimer.Elapsed += new ElapsedEventHandler(timer_Elapsed); 
            //CycleTimer.Start();
            FileWatch();

            Console.Read();
        }

        static void Initialize()
        {
            TrialMode();
            string[] LanguageTypeCheck_EN = { "Application Generated", "Certification Services", "Detailed File Share", "File Share", "File System", "Filtering Platform Connection", "Filtering Platform Packet Drop", "Handle Manipulation", "Kernel Object", "Other Object Access Events", "Registry ", "SAM", "Removable Storage" };
            string[] LanguageTypeCheck_KR = { "응용 프로그램 생성됨", "인증 서비스", "세부 파일 공유", "파일 공유", "파일 시스템", "필터링 플랫폼 연결", "필터링 플랫폼 패킷 삭제", "핸들 조작", "커널 개체", "기타 개체 액세스 이벤트", "레지스트리", "SAM", "이동식 저장소" };
            bool m_Flag = true;

            const string queryString = @"<QueryList>
                                          <Query Id=""0"" Path=""Security"">
                                            <Select Path=""Security"">*</Select>
                                          </Query>
                                        </QueryList>";
            EventLogQuery eventsQuery = new EventLogQuery("Security", PathType.LogName, queryString);
            EventLogReader logReader = new EventLogReader(eventsQuery);
            for (EventRecord eventInstance = logReader.ReadEvent(); null != eventInstance; eventInstance = logReader.ReadEvent())
            {
                if (eventInstance != null && m_Flag)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        if (eventInstance.TaskDisplayName.ToString() == LanguageTypeCheck_EN[i])
                        {
                            m_Flag = false;
                            LanguageFilter[0] = "File System";
                            LanguageFilter[1] = "Detailed File Share";
                            LanguageFilter[2] = "Removable Storage";
                            Console.WriteLine("English");
                        }
                        if (eventInstance.TaskDisplayName.ToString() == LanguageTypeCheck_KR[i])
                        {
                            m_Flag = false;
                            LanguageFilter[0] = "파일 시스템";
                            LanguageFilter[1] = "세부 파일 공유";
                            LanguageFilter[2] = "이동식 저장소";
                            Console.WriteLine("Korean");
                        }
                    }
                }
                else
                    break;
            }
        }

        static void FileWatchLogWrite(string str)
        {
            string DirPath = @"C:\Users\" + g_UserName + @"\AppData\Roaming\EventLogView" + @"\FileWatchLog\" + DateTime.Today.ToString("yyyyMMdd");
            string FilePath = DirPath + "\\EventLog_" + DateTime.Today.ToString("yyyyMMdd") + ".log";
            string temp;
            //string DirPath = @"C:\EventLog\SSEvent" + @"\Log\" + DateTime.Today.ToString("yyyyMMdd");
            //string FilePath = DirPath + "\\Log_" + DateTime.Today.ToString("yyyyMMdd") + ".log";
            //string temp;

            DirectoryInfo di = new DirectoryInfo(DirPath);
            FileInfo fi = new FileInfo(FilePath);
            try
            {
                if (!di.Exists) Directory.CreateDirectory(DirPath);
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(FilePath))
                    {
                        temp = string.Format("[{0}] {1}", DateTime.Now, str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(FilePath))
                    {
                        temp = string.Format("[{0}] {1}", DateTime.Now, str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //로그 파일
        static void LogWrite(string str)
        {
            string DirPath = @"C:\Users\"+ g_UserName + @"\AppData\Roaming\EventLogView" + @"\Log\" + DateTime.Today.ToString("yyyyMMdd");
            //string DirPath = @"C:\Users\Lee\AppData\Roaming\EventLogView" + @"\Log\" + DateTime.Today.ToString("yyyyMMdd");
            string FilePath = DirPath + "\\EventLog_" + DateTime.Today.ToString("yyyyMMdd") + ".log";
            string temp;

            DirectoryInfo di = new DirectoryInfo(DirPath);
            FileInfo fi = new FileInfo(FilePath);

            try
            {
                if (!di.Exists) Directory.CreateDirectory(DirPath);
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(FilePath))
                    {
                        temp = string.Format("[{0}] {1}", DateTime.Now, str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(FilePath))
                    {
                        temp = string.Format("[{0}] {1}", DateTime.Now, str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void TrialMode()
        {
            string DirPath = @"C:\Users\" + g_UserName + @"\AppData\Roaming\EventLogView" + @"\Option\";
            string FilePath = DirPath + "\\SerialNumber.ini";
            string temp;

            DirectoryInfo di = new DirectoryInfo(DirPath);
            FileInfo fi = new FileInfo(FilePath);

            try
            {
                if (!di.Exists) Directory.CreateDirectory(DirPath);
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(FilePath))
                    {
                        //Trial AES256
                        temp = string.Format("hoUX64SudQXYNJJhTKMogQ==");
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
                var info = new FileInfo(FilePath);
                TrialTime = info.CreationTime;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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
            //m_Data = m_Data.Replace("D:(A;;FA;;;BA)", "");
            //m_Data = m_Data.Replace("D:(A;;FA;;;BA)", "");
            //m_Data = m_Data.Replace("D:(A;OICI;FA;;;WD)", "");
            //m_Data = m_Data.Replace("D:(A;OICI;FA;;;WD)", "");

            return m_Data;
        }

        static void FileWatch()
        {
            FileSystemWatcher fsw = new FileSystemWatcher(g_FoldertoSearch);
            fsw.NotifyFilter = NotifyFilters.FileName |
                NotifyFilters.DirectoryName |
                NotifyFilters.Size |
                NotifyFilters.LastAccess |
                NotifyFilters.CreationTime |
                NotifyFilters.Security |
                NotifyFilters.LastWrite;
            fsw.Filter = "*.*"; ; //감시할 파일 유형 선택 예) *.* 모든 파일 
            fsw.Renamed += new RenamedEventHandler(Renamed);
            fsw.Created += new FileSystemEventHandler(Created);
            fsw.Changed += new FileSystemEventHandler(Changed);
            fsw.Deleted += new FileSystemEventHandler(Deleted);

            fsw.EnableRaisingEvents = true;

            Console.WriteLine("Wait until File Created ~~~");
            Console.Read();
        }

        private static void Created(object sender, FileSystemEventArgs e) { g_FSWLog = String.Format("{0} {1}", e.ChangeType.ToString(), e.FullPath); FileWatchLogWrite(g_FSWLog); }
        private static void Deleted(object sender, FileSystemEventArgs e) { g_FSWLog = String.Format("{0} {1}", e.ChangeType.ToString(), e.FullPath); FileWatchLogWrite(g_FSWLog); }
        private static void Changed(object source, FileSystemEventArgs e) { g_FSWLog = String.Format("{0} {1}", e.ChangeType.ToString(), e.FullPath); FileWatchLogWrite(g_FSWLog); }
        private static void Renamed(object source, RenamedEventArgs e) { g_FSWLog = String.Format("{0} {1}", e.ChangeType.ToString(), e.FullPath); FileWatchLogWrite(g_FSWLog); }

        static bool hasing(EvLogPara t_OldData, EvLogPara t_NewDate)
        {
            if(!(t_OldData.Equals(t_NewDate)))
                return true;
            else
                return false;
        }

        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            InsertEventLog();
        }
    }
}
