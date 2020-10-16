using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace SSLogPusherViewer
{
    public partial class SSLogPusherViewer : Form
    {
        private string g_FoldertoSearch = "";
        private string g_UserName = "";
        private string ParameterFolder = @"C:\SSLogPusher\PusherLog\SSLogPusher.cfg";
        private string ProcessLogPath = @"C:\SSLogPusher\PusherLog\ProcessLog.log";

        public DateTime g_TrialTime = new DateTime(0);
        public DateTime g_LimitTime = new DateTime(2020, 09, 30, 23, 59, 00);

        public int g_LimitDay = 40;

        public SSLogPusherViewer()
        {
            InitializeComponent();
            Initialize();
        }

        //private string g_ConnectionStr = @"Data Source=192.168.10.230,7100;Initial Catalog=arcon;Integrated Security=False;User ID=arconsa;Password=arconsa@pass0;Connect Timeout=5;Encrypt=False;TrustServerCertificate=False";
        private string g_ConnectionStr = @"Data Source=127.0.0.1,1433;Initial Catalog=Eventlog;Integrated Security=False;User ID=eventsa;Password=eventsa@pass0;Connect Timeout=5;Encrypt=False;TrustServerCertificate=False";

        private void Initialize()
        {
            ProcessLog("Initialize Start");
            FileCheck();
            TrialMode();

            if (TrialCheck())
                Application.Exit();

            this.MaximizeBox = false;
            DB_ListView_Log.Clear();
            DB_ListView_Log.View = View.Details;
            DB_ListView_Log.GridLines = true;
            DB_ListView_Log.FullRowSelect = true;
            DB_ListView_Log.Columns.Add("LoadTime", 150);
            DB_ListView_Log.Columns.Add("EventID", 50);
            DB_ListView_Log.Columns.Add("UserName", 70);
            DB_ListView_Log.Columns.Add("DomainName", 70);
            DB_ListView_Log.Columns.Add("Subject", 50);
            DB_ListView_Log.Columns.Add("PC_IPAddress", 100);
            DB_ListView_Log.Columns.Add("PC_Port", 70);
            DB_ListView_Log.Columns.Add("ShareName", 150);
            DB_ListView_Log.Columns.Add("ShareLocalPath", 150);
            DB_ListView_Log.Columns.Add("FileName", 150);
            DB_ListView_Log.Columns.Add("AccessMask", 100);
            DB_ListView_Log.Columns.Add("AccessList", 100);
            DB_ListView_Log.Columns.Add("AccessReason", 100);
            DB_ListView_Log.Columns.Add("EventTime", 150);
            DB_ListView_Log.Columns.Add("LogonID", 50);
            DB_ListView_Log.Columns.Add("Information", 50);
            DB_ListView_Log.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            ProcessLog("Initialize Clear");
        }

        public void FileCheck()
        {
            ProcessLog("FileCheck Start");
            if (File.Exists(ParameterFolder))
            {
                try
                {
                    ProcessLog("FileLoad Success");
                    string[] lines = File.ReadAllLines(ParameterFolder);
                    g_FoldertoSearch = lines[0];
                    g_UserName = lines[1];
                }
                catch (Exception e2)
                {
                    using (StreamWriter sw = new StreamWriter(ProcessLogPath))
                    {
                        ProcessLog("필요한 파라메터가 없습니다." + e2.Message);
                        sw.Close();
                    }
                }
            }
            else
            {
                ProcessLog("FileLoad Fail");
            }
            ProcessLog("FileCheck Clear");
        }

        public bool TrialCheck()
        {
            ProcessLog("TrialCheck Start");

            if (DateTime.Compare(g_TrialTime.AddDays(g_LimitDay), DateTime.Now) != 1)
            {
                ProcessLog("기간이 만료되었습니다.");
                return true;
            }

            if (DateTime.Compare(g_LimitTime, DateTime.Now) != 1)
            {
                ProcessLog("기간이 만료되었습니다.");
                return true;
            }

            ProcessLog("TrialCheck Clear");

            return false;
        }

        public void ProcessLog(string str)
        {
            string FilePath = ProcessLogPath;

            string temp = "";

            FileInfo fi = new FileInfo(FilePath);
            try
            {
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

        private void TrialMode()
        {
            ProcessLog("TrialMode Start");
            string DirPath = @"C:\Users\" + g_UserName + @"\AppData\Roaming\PusherLogView" + @"\Option\";
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
            ProcessLog("TrialMode Clear");
        }

        private string WordFilter(string t_Getstring)
        {
            t_Getstring = t_Getstring.Replace("ReadData","파일 데이터를 읽을 수 있는 권한, ");
            t_Getstring = t_Getstring.Replace("WriteData","파일에 데이터를 쓸 수 있는 권한, ");
            t_Getstring = t_Getstring.Replace("AppendData","파일에 데이터를 추가할 수 있는 권한, ");
            t_Getstring = t_Getstring.Replace("ReadEA","파일 특성을 읽을 수 있는 권한, ");
            t_Getstring = t_Getstring.Replace("WriteEA", "파일 특성을 쓸 수 있는 권한, ");
            t_Getstring = t_Getstring.Replace("ReadAttributes","파일 특성을 읽을 수 있는 권한, ");
            t_Getstring = t_Getstring.Replace("WriteAttributes","파일 특성을 쓸 수 있는 권한, ");
            t_Getstring = t_Getstring.Replace("DeleteChild","렉터리와 여기에 포함 된 모든 파일을 삭제할 수 있는 권한, ");
            t_Getstring = t_Getstring.Replace("DELETE","개체를 삭제할 수 있는 권한, ");
            t_Getstring = t_Getstring.Replace("READ_CONTROL","개체의 보안 설명자에 있는 정보를 읽을 수 있는 권한, ");
            t_Getstring = t_Getstring.Replace("WRITE_DAC","개체의 보안 설명자에서 DACL (임의 액세스 제어 목록)을 수정할 수 있는 권한, ");
            t_Getstring = t_Getstring.Replace("WRITE_OWNER","개체의 보안 설명자에서 소유자를 변경할 수 있는 권한, ");
            t_Getstring = t_Getstring.Replace("항목과","동기화에 개체를 사용할 수 있는 권한, ");
            t_Getstring = t_Getstring.Replace("SYNCHRONIZER","동기화에 개체를 사용할 수 있는 권한, ");
            t_Getstring = t_Getstring.Replace("SYNCHRONIZE","동기화에 개체를 사용할 수 있는 권한, ");
            t_Getstring = t_Getstring.Replace("ACCESS_SYS_SEC","개체의 보안 설명자에서 SACL을 가져오거나 설정 하는 기능을 제어, ");

            //()내용삭제
            int m_FindFront = 0;
            int m_FindBack = 0;
            while (m_FindFront != -1)
            {
                m_FindFront = t_Getstring.IndexOf('(');
                m_FindBack = t_Getstring.IndexOf(')');
                if(m_FindFront != -1 || m_FindBack != -1)
                    t_Getstring = t_Getstring.Remove(m_FindFront,(m_FindBack - m_FindFront+1));
            }
            return t_Getstring;
        }

        private void Button_User_Click(object sender, EventArgs e)
        {
            Initialize();
            /////  DBConnect Parameter   /////
            
            SqlConnection sqlCon = new SqlConnection(g_ConnectionStr);
            SqlCommand sqlCmd = new SqlCommand();
            SqlDataReader sqlRed;
            string m_User = TB_UserName.Text;

            /////  DBConnect Parameter   /////
            try
            {
                sqlCon.Open();
                sqlCmd.Connection = sqlCon;

                sqlCmd.CommandText = $"select DISTINCT * from Eventlog.dbo.Eventlogview where Username like '%" + m_User + "%'" + " and EventTime >= '" + this.Start_DateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "' and EventTime <= '" + this.End_DateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                //sqlCmd.CommandText = $"select DISTINCT * From Eventlog.dbo.eventlogview where UserName like '%" + m_User + "%'";
                sqlRed = sqlCmd.ExecuteReader();
                while (sqlRed.Read())
                {
                    if (m_User == "")
                        break;
                    ListViewItem AddList = new ListViewItem(new string[] { sqlRed.GetDateTime(0).ToString(), sqlRed.GetInt32(1).ToString(), sqlRed.GetString(2).ToString(), sqlRed.GetString(3).ToString(), sqlRed.GetString(4).ToString(), sqlRed.GetString(5).ToString(), sqlRed.GetInt32(6).ToString(), sqlRed.GetString(7).ToString(), sqlRed.GetString(8).ToString(), sqlRed.GetString(9).ToString(), sqlRed.GetString(10).ToString(), WordFilter(sqlRed.GetString(11).ToString()), WordFilter(sqlRed.GetString(12).ToString()), sqlRed.GetDateTime(13).ToString(), sqlRed.GetString(14).ToString(), sqlRed.GetString(15).ToString()});
                    DB_ListView_Log.Items.Add(AddList);
                }
                //sqlCmd.ExecuteNonQuery();
                sqlCon.Close();
            }
            catch (Exception Error)
            {
                sqlCon.Close();
                Console.WriteLine(Error.Message);
            }
        }

        private void Button_File_Click(object sender, EventArgs e)
        {
            Initialize();
            /////  DBConnect Parameter   /////

            SqlConnection sqlCon = new SqlConnection(g_ConnectionStr);
            SqlCommand sqlCmd = new SqlCommand();
            SqlDataReader sqlRed;
            string m_File = TB_FileName.Text;
            /////  DBConnect Parameter   /////
            try
            {
                sqlCon.Open();
                sqlCmd.Connection = sqlCon;

                sqlCmd.CommandText = $"select DISTINCT * from Eventlog.dbo.Eventlogview where FileName like '%" + m_File + "%'" + " and EventTime >= '" + this.Start_DateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "' and EventTime <= '" + this.End_DateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                //sqlCmd.CommandText = $"select DISTINCT * From Eventlog.dbo.eventlogview where FileName like '%" + m_File + "%'";
                sqlRed = sqlCmd.ExecuteReader();
                while(sqlRed.Read())
                {
                    if (m_File == "")
                        break;
                    ListViewItem AddList = new ListViewItem(new string[] { sqlRed.GetDateTime(0).ToString(), sqlRed.GetInt32(1).ToString(), sqlRed.GetString(2).ToString(), sqlRed.GetString(3).ToString(), sqlRed.GetString(4).ToString(), sqlRed.GetString(5).ToString(), sqlRed.GetInt32(6).ToString(), sqlRed.GetString(7).ToString(), sqlRed.GetString(8).ToString(), sqlRed.GetString(9).ToString(), sqlRed.GetString(10).ToString(), WordFilter(sqlRed.GetString(11).ToString()), WordFilter(sqlRed.GetString(12).ToString()), sqlRed.GetDateTime(13).ToString(), sqlRed.GetString(14).ToString(), sqlRed.GetString(15).ToString() });
                    DB_ListView_Log.Items.Add(AddList);
                }
                //sqlCmd.ExecuteNonQuery();
                sqlCon.Close();
            }
            catch (Exception Error)
            {
                sqlCon.Close();
                Console.WriteLine(Error.Message);
            }
        }

        private void Button_IPAddress_Click(object sender, EventArgs e)
        {
            Initialize();
            /////  DBConnect Parameter   /////

            SqlConnection sqlCon = new SqlConnection(g_ConnectionStr);
            SqlCommand sqlCmd = new SqlCommand();
            SqlDataReader sqlRed;
            string m_Address = TB_IPAddress.Text;
            /////  DBConnect Parameter   /////
            try
            {
                sqlCon.Open();
                sqlCmd.Connection = sqlCon;

                sqlCmd.CommandText = $"select DISTINCT * from Eventlog.dbo.Eventlogview where PC_IPAddress like '%" + m_Address + "%'" + " and EventTime >= '" + this.Start_DateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "' and EventTime <= '" + this.End_DateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                //sqlCmd.CommandText = $"select DISTINCT * From Eventlog.dbo.eventlogview where PC_IPAddress like '%" + m_Address + "%'";
                sqlRed = sqlCmd.ExecuteReader();
                while (sqlRed.Read())
                {
                    if (m_Address == "")
                        break;
                    ListViewItem AddList = new ListViewItem(new string[] { sqlRed.GetDateTime(0).ToString(), sqlRed.GetInt32(1).ToString(), sqlRed.GetString(2).ToString(), sqlRed.GetString(3).ToString(), sqlRed.GetString(4).ToString(), sqlRed.GetString(5).ToString(), sqlRed.GetInt32(6).ToString(), sqlRed.GetString(7).ToString(), sqlRed.GetString(8).ToString(), sqlRed.GetString(9).ToString(), sqlRed.GetString(10).ToString(), WordFilter(sqlRed.GetString(11).ToString()), WordFilter(sqlRed.GetString(12).ToString()), sqlRed.GetDateTime(13).ToString(), sqlRed.GetString(14).ToString(), sqlRed.GetString(15).ToString() });
                    DB_ListView_Log.Items.Add(AddList);
                }
                //sqlCmd.ExecuteNonQuery();
                sqlCon.Close();
            }
            catch (Exception Error)
            {
                sqlCon.Close();
                Console.WriteLine(Error.Message);
            }
        }

        private void SSLogPusherViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
