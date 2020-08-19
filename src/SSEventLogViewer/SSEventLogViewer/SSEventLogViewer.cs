using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace SSEventLogViewer
{
    public partial class SSEventLogViewer : Form
    {
        private DateTime TrialTime = new DateTime(0);
        private DateTime LimitTime = new DateTime(2020, 08, 27, 00, 00, 00);

        private string UserName = "";

        public SSEventLogViewer()
        {
            InitializeComponent();
            Initialize();
        }

        private string g_ConnectionStr = @"Data Source=192.168.10.230,7100;Initial Catalog=arcon;Integrated Security=False;User ID=arconsa;Password=arconsa@pass0;Connect Timeout=5;Encrypt=False;TrustServerCertificate=False";
        //private string g_ConnectionStr = @"Data Source=127.0.0.1,1433;Initial Catalog=Eventlog;Integrated Security=False;User ID=eventsa;Password=eventsa@pass0;Connect Timeout=5;Encrypt=False;TrustServerCertificate=False";

        private void Initialize()
        {
            if (File.Exists(@"C:\SSEvent\EventLog\SSEvent.cfg"))
            {
                string[] lines = File.ReadAllLines(@"C:\SSEvent\EventLog\SSEvent.cfg");
                UserName = lines[1]; // or whatever.
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(@"C:\SSEvent\EventLog\Error.log"))
                {
                    sw.WriteLine("필요한 파일이 없습니다.");
                    sw.Close();
                }
            }

            TrialMode();

            if (DateTime.Compare(TrialTime.AddDays(7), DateTime.Now) != 1)
            {
                using (StreamWriter sw = new StreamWriter(@"C:\SSEvent\EventLog\Error.log"))
                {
                    sw.WriteLine("기간이 만료되었습니다.");
                    sw.Close();
                }
                Environment.Exit(0);
            }

            if (DateTime.Compare(LimitTime, DateTime.Now) != 1)
            { 
                using (StreamWriter sw = new StreamWriter(@"C:\SSEvent\EventLog\Error.log"))    
                {
                    sw.WriteLine("기간이 만료되었습니다.");
                    sw.Close();
                }
                Environment.Exit(0);
            }

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
        }

        private void TrialMode()
        {
            string DirPath = @"C:\Users\" + UserName + @"\AppData\Roaming\EventLogView" + @"\Option\";
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
    }
}
