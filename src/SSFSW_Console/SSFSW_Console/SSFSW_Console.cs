using System;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Text;
using System.Timers;

namespace SSFSW_Console
{
    class SSLogPusherClass
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

        static public string[] LanguageFilter = new string[3];
        static public string g_FoldertoSearch = "";
        static public string g_UserName = "";
        static public string g_FSWLog = "";

        static public bool Triger = false;
        static public bool TF = true;

        static public DateTime TC_1 = new DateTime(0);
        static public DateTime TC_2 = new DateTime(0);
        static public DateTime Ev_TC = new DateTime(0);

        static public DateTime g_TrialTime = new DateTime(0);
        static public DateTime g_LimitTime = new DateTime(2020, 09, 30, 23, 59, 00);
        static public int g_LimitDay = 40;

        static public double g_Time_Interval = 5000;

        static public string ParameterFolder = @"C:\SSLogPusher\PusherLog\SSLogPusher.cfg";
        static public string ProcessLogPath = @"C:\SSLogPusher\PusherLog\ProcessLog.log";

        static public void InsertEventLog()
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

                            if (DateTime.Compare(TC_1, (DateTime)eventInstance.TimeCreated) == -1)
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
        }

        static public void FileCheck()
        {
            Console.WriteLine("FileCheck Start");
            if (File.Exists(ParameterFolder))
            {
                try
                {
                    Console.WriteLine("FileLoad Success");
                    string[] lines = File.ReadAllLines(ParameterFolder);
                    g_FoldertoSearch = lines[0];
                    g_UserName = lines[1];
                }
                catch (Exception e2)
                {
                    using (StreamWriter sw = new StreamWriter(ProcessLogPath))
                    {
                        Console.WriteLine("필요한 파라메터가 없습니다." + e2.Message);
                        sw.Close();
                    }
                }
            }
            else
            {
                Console.WriteLine("FileLoad Fail");
            }
            Console.WriteLine("FileCheck Clear");
        }

        static public void Initialize()
        {
            Console.WriteLine("Initialize Start");
            FileCheck();
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
            Console.WriteLine("Initialize Clear");
        }

        static public void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            InsertEventLog();
        }

        static public void FileWatch()
        {
            Console.WriteLine("FSW Start");

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

            Console.WriteLine("FSW Clear");
        }

        static public void LogWrite(string str)
        {
            string DirPath = @"C:\Users\" + g_UserName + @"\AppData\Roaming\EventLogView" + @"\Log\" + DateTime.Today.ToString("yyyyMMdd");
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

        static public bool TrialCheck()
        {
            Console.WriteLine("TrialCheck Start");

            if (DateTime.Compare(g_TrialTime.AddDays(g_LimitDay), DateTime.Now) != 1)
            {
                Console.WriteLine("기간이 만료되었습니다.");
                return true;
            }

            if (DateTime.Compare(g_LimitTime, DateTime.Now) != 1)
            {
                Console.WriteLine("기간이 만료되었습니다.");
                return true;
            }

            Console.WriteLine("TrialCheck Clear");

            return false;
        }

        static public void TrialMode()
        {
            Console.WriteLine("TrialMode Start");
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
                g_TrialTime = info.CreationTime;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("TrialMode Clear");
        }

        static public bool hasing(EvLogPara t_OldData, EvLogPara t_NewDate)
        {
            if (!(t_OldData.Equals(t_NewDate)))
                return true;
            else
                return false;
        }

        static public string DataReplace(string p_Data)
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

        static public void FileWatchLogWrite(string str)
        {
            Console.WriteLine("FileWatchLogWrite Start");
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
            Console.WriteLine("FileWatchLogWrite Clear");
        }
                
        private  static void Created(object sender, FileSystemEventArgs e) { g_FSWLog = String.Format("{0} {1}", e.ChangeType.ToString(), e.FullPath); FileWatchLogWrite(g_FSWLog); }
        private  static void Deleted(object sender, FileSystemEventArgs e) { g_FSWLog = String.Format("{0} {1}", e.ChangeType.ToString(), e.FullPath); FileWatchLogWrite(g_FSWLog); }
        private  static void Changed(object source, FileSystemEventArgs e) { g_FSWLog = String.Format("{0} {1}", e.ChangeType.ToString(), e.FullPath); FileWatchLogWrite(g_FSWLog); }
        private  static void Renamed(object source, RenamedEventArgs e) { g_FSWLog = String.Format("{0} {1}", e.ChangeType.ToString(), e.FullPath); FileWatchLogWrite(g_FSWLog); }

    }
    class SSFSW_Console
    {
        static void Main(string[] args)
        {
            SSLogPusherClass.FileCheck();

            Console.WriteLine("ServiceStart");

            SSLogPusherClass.Initialize();

            if (SSLogPusherClass.TrialCheck())
                return;

            Timer CycleTimer = new System.Timers.Timer();
            CycleTimer.Interval = SSLogPusherClass.g_Time_Interval;
            CycleTimer.Elapsed += new ElapsedEventHandler(SSLogPusherClass.timer_Elapsed);
            CycleTimer.Start();

            SSLogPusherClass.FileWatch();

            Console.WriteLine("Service Start Success");

            SSLogPusherClass.LogWrite("Service Start Success");

            Console.Read();
        }
    }
}
