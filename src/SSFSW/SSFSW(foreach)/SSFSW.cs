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

        static void InsertEventLog()
        {
            /////  EventLog Parameter   /////
            EvLogPara nevlp = new EvLogPara("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            EvLogPara oevlp = new EvLogPara("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");

            string FoldertoSearch = "";
            string[] LanguageFilter = { "File System", "Detailed File Share", "Removable Storage"};

            if (File.Exists(@"C:\ARCON\aosvc.cfg"))
            {
                StreamReader rdr = new StreamReader(@"C:\ARCON\aosvc.cfg");
                FoldertoSearch = rdr.ReadLine();
                rdr.Close();
            }

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
            /////  EventLog Parameter   /////

            /////  DBConnect Parameter   /////
            string g_ConnectionStr = @"Data Source=192.168.10.230,7100;Initial Catalog=arcon;Integrated Security=False;User ID=arconsa;Password=arconsa@pass0;Connect Timeout=5;Encrypt=False;TrustServerCertificate=False";
            SqlCommand sqlCmd = new SqlCommand();
            SqlConnection sqlCon = new SqlConnection(g_ConnectionStr);
            /////  DBConnect Parameter   /////

            for (EventRecord eventInstance = logReader.ReadEvent(); null != eventInstance; eventInstance = logReader.ReadEvent())
            {
                foreach (var VARIABLE in eventInstance.Properties)
                {
                    try
                    {
                        if (!VARIABLE.Value.ToString().Contains(FoldertoSearch) && (eventInstance.TaskDisplayName.ToString() == "File System" || eventInstance.TaskDisplayName.ToString() == "Detailed File Share" || eventInstance.TaskDisplayName.ToString() == "Removable Storage"))
                        {
                            //내 PC에서 조작한경우
                            if (eventInstance.Id.ToString() == "4656")
                            {
                                if (eventInstance.Properties[6].Value.ToString().Replace(FoldertoSearch, "").Length != 0)
                                {
                                    nevlp.LoadTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.sss");
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
                                    nevlp.EventTime = eventInstance.TimeCreated.Value.ToString("yyyy-MM-dd hh:mm:ss.sss");

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
                            //원격에서 접속해서 조작한 경우
                            if (eventInstance.Id.ToString() == "5145")
                            {
                                if (eventInstance.Properties[9].Value.ToString().Replace("\\", "").Length != 0 && eventInstance.TaskDisplayName.ToString() == "Detailed File Share")
                                {
                                    nevlp.LoadTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.sss");
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
                                    nevlp.EventTime = eventInstance.TimeCreated.Value.ToString("yyyy-MM-dd hh:mm:ss.sss");
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
                            isStop = true;
                            break;
                        }
                        if (nevlp.EventID != "" && hasing(oevlp, nevlp))
                        {
                            sqlCon.Open();
                            sqlCmd.Connection = sqlCon;

                            sqlCmd.CommandText = $"INSERT INTO arcon.dbo.EventLogView(LoadTime, EventID, UserName, DomainName, Subject, PC_IPAddress, PC_Port, ShareName, ShareLocalPath, FileName, AccessMask, AccessList, AccessReason, EventTime, LogonID, Information)" +
                                                         $" VALUES ('" + nevlp.LoadTime + "','" + nevlp.EventID + "','" + nevlp.UserName + "','" + nevlp.DomainName + "','" + nevlp.Subject + "','" + nevlp.PC_IPAddress + "','" + nevlp.PC_Port + "','" + nevlp.ShareName + "','" + nevlp.ShareLocalPath + "','" + nevlp.FileName + "','" + nevlp.AccessMask + "','" + nevlp.AccessList + "','" + nevlp.AccessReason + "','" + nevlp.EventTime + "','" + nevlp.LogonID + "','" + nevlp.Information + "')";

                            oevlp = nevlp;
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
            CycleTimer.Interval = 10000;
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
            //m_Data = m_Data.Replace("D:(A;;FA;;;BA)", "");
            //m_Data = m_Data.Replace("D:(A;;FA;;;BA)", "");
            //m_Data = m_Data.Replace("D:(A;OICI;FA;;;WD)", "");
            //m_Data = m_Data.Replace("D:(A;OICI;FA;;;WD)", "");

            return m_Data;
        }
        static bool hasing(EvLogPara t_OldData, EvLogPara t_NewDate)
        {
            //if (t_OldData.GetHashCode() != t_NewDate.GetHashCode())
            if(!(t_OldData.Equals(t_NewDate)))
                return true;
            else
                return false;
        }
        // 쓰레드풀의 작업쓰레드가 지정된 시간 간격으로
        // 아래 이벤트 핸들러 실행
        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            InsertEventLog();
        }
    }
}
