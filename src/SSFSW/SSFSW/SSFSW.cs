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
        public struct EvLogPara
        {
            public string LoadTime;
            public string EventID;
            public string UserName;
            public string DomainName;
            public string Subject;
            public string Login_ID;
            public string PC_IPAddress;
            public string PC_Port;
            public string Shared_Directory;
            public string Shared_FileDirectory;
            public string FileName;
            public string Type;
            public string AccessType;
            public string AccessTypeReason;
            public string EventTime;
            public string Information;

            public EvLogPara(string t_LoadTime, string t_EventID, string t_UserName, string t_DomainName, string t_Subject, string t_Login_ID, string t_PC_IPAddress, string t_PC_Port, string t_Shared_Directory, string t_Shared_FileDirectory, string t_FileName, string t_Type, string t_AccessType, string t_AccessTypeReason, string t_EventTime, string t_Information)
            {
                LoadTime = t_LoadTime;
                EventID = t_EventID;
                UserName = t_UserName;
                DomainName = t_DomainName;
                Subject = t_Subject;
                Login_ID = t_Login_ID;
                PC_IPAddress = t_PC_IPAddress;
                PC_Port = t_PC_Port;
                Shared_Directory = t_Shared_Directory;
                Shared_FileDirectory = t_Shared_FileDirectory;
                FileName = t_FileName;
                Type = t_Type;
                AccessType = t_AccessType;
                AccessTypeReason = t_AccessTypeReason;
                EventTime = t_EventTime;
                Information = t_Information;
            }
            public override string ToString() => $"({LoadTime},{EventID},{UserName},{DomainName},{Subject},{Login_ID},{PC_IPAddress},{PC_Port},{Shared_Directory},{Shared_FileDirectory},{FileName},{Type},{AccessType},{AccessTypeReason},{EventTime},{Information})";
        }

        static void InsertEventLog()
        {
            /////  EventLog Parameter   /////
            EvLogPara evlp = new EvLogPara("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            string FoldertoSearch = "";
            string[] LanguageFilter = { "File System", "Detailed File Share", "Removable Storage"};
            string[] DataComparer   = { "OldData", "NewData", };

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
                                    evlp.LoadTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.sss");
                                    evlp.EventID = eventInstance.Id.ToString();
                                    evlp.UserName = eventInstance.Properties[1].Value.ToString();
                                    evlp.DomainName = eventInstance.Properties[2].Value.ToString();
                                    evlp.Login_ID = eventInstance.Properties[3].Value.ToString();
                                    evlp.Information = eventInstance.Properties[4].Value.ToString();
                                    evlp.Subject = eventInstance.Properties[5].Value.ToString();
                                    evlp.PC_IPAddress = "";
                                    evlp.PC_Port = "";
                                    evlp.Shared_Directory = "";
                                    evlp.Shared_FileDirectory = eventInstance.Properties[6].Value.ToString();
                                    evlp.FileName = Path.GetFileName(eventInstance.Properties[6].Value.ToString());
                                    evlp.Type = "";
                                    evlp.AccessType = DataReplace(eventInstance.Properties[9].Value.ToString());
                                    evlp.AccessTypeReason = DataReplace(eventInstance.Properties[10].Value.ToString());
                                    evlp.EventTime = eventInstance.TimeCreated.Value.ToString("yyyy-MM-dd hh:mm:ss.sss");

                                    Console.WriteLine("==============================================================");
                                    Console.WriteLine("LoadTime " + evlp.LoadTime);
                                    Console.WriteLine("EventID : " + evlp.EventID);
                                    Console.WriteLine("UserName : " + evlp.UserName);
                                    Console.WriteLine("DomainName : " + evlp.DomainName);
                                    Console.WriteLine("Login_ID : " + evlp.Login_ID);
                                    Console.WriteLine("Information : " + evlp.Information);
                                    Console.WriteLine("Subject : " + evlp.Subject);
                                    Console.WriteLine("Shared_FileDirectory : " + evlp.Shared_FileDirectory);
                                    Console.WriteLine("FileName : " + evlp.FileName);
                                    Console.WriteLine("AccessType : \n" + evlp.AccessType);
                                    Console.WriteLine("AccessTypeReason : \n" + evlp.AccessTypeReason);
                                    Console.WriteLine("EventTime: " + evlp.EventTime);
                                    
                                }
                            }
                            //원격에서 접속해서 조작한 경우
                            if (eventInstance.Id.ToString() == "5145")
                            {
                                Console.WriteLine(eventInstance.Properties[9].Value.ToString());
                                if (eventInstance.Properties[9].Value.ToString().Replace("\\", "").Length != 0 && eventInstance.TaskDisplayName.ToString() == "Detailed File Share")
                                {
                                    evlp.LoadTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.sss");
                                    evlp.EventID = eventInstance.Id.ToString();
                                    evlp.UserName = eventInstance.Properties[1].Value.ToString();
                                    evlp.DomainName = eventInstance.Properties[2].Value.ToString();
                                    evlp.Subject = eventInstance.Properties[4].Value.ToString();
                                    evlp.Login_ID = eventInstance.Properties[3].Value.ToString();
                                    evlp.PC_IPAddress = eventInstance.Properties[5].Value.ToString();
                                    evlp.PC_Port = eventInstance.Properties[6].Value.ToString();
                                    evlp.Shared_Directory = eventInstance.Properties[7].Value.ToString();
                                    evlp.Shared_FileDirectory = eventInstance.Properties[8].Value.ToString();
                                    evlp.FileName = eventInstance.Properties[9].Value.ToString();
                                    evlp.Type = eventInstance.Properties[10].Value.ToString();
                                    evlp.AccessType = DataReplace(eventInstance.Properties[11].Value.ToString());
                                    evlp.AccessTypeReason = DataReplace(eventInstance.Properties[12].Value.ToString());
                                    evlp.EventTime = eventInstance.TimeCreated.Value.ToString("yyyy-MM-dd hh:mm:ss.sss");
                                    evlp.Information = "";

                                    Console.WriteLine("==============================================================");
                                    Console.WriteLine("LoadTime " + evlp.LoadTime);
                                    Console.WriteLine("EventID : " + evlp.EventID);
                                    Console.WriteLine("UserName : " + evlp.UserName);
                                    Console.WriteLine("DomainName : " + evlp.DomainName);
                                    Console.WriteLine("Subject : " + evlp.Subject);
                                    Console.WriteLine("Login_ID : " + evlp.Login_ID);
                                    Console.WriteLine("PC_IPAddress : " + evlp.PC_IPAddress);
                                    Console.WriteLine("PC_Port : " + evlp.PC_Port);
                                    Console.WriteLine("Shared_Directory: " + evlp.Shared_Directory);
                                    Console.WriteLine("Shared_FileDirectory : " + evlp.Shared_FileDirectory);
                                    Console.WriteLine("FileName: " + evlp.FileName);
                                    Console.WriteLine("Type : " + evlp.Type);
                                    Console.WriteLine("AccessType : \n" + evlp.AccessType);
                                    Console.WriteLine("AccessTypeReason : \n" + evlp.AccessTypeReason);
                                    Console.WriteLine("CreateTime : " + evlp.EventTime);
                                }
                            }
                            isStop = true;
                            break;
                        }
                        if (evlp.EventID != "")
                        {
                            sqlCon.Open();
                            sqlCmd.Connection = sqlCon;

                            //sqlCmd.CommandText = $"INSERT INTO arcon.dbo.EventLogView(time, id, username, PCname, subject, IPAddress, Port, Path, FilePath, Filename, type, AccessType, Reason, Createtime, Login_ID, Information, Count_index)" +
                            //                             $" VALUES ('" + LogCheckTime + "','" + ID + "','" + UserName + "','" + PCName + "','" + Subject + "','" + IPAddress + "','" + Port + "','" + Path + "','" + FilePath + "','" + FileName + "','" + Type + "','" + AccessType + "','" + Reason + "','" + CreateTime + "','" + Login_ID + "','" + Information + "','"+ m_index+"')";

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
    }
}
