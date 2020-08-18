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

        //private string g_ConnectionStr = @"Data Source=192.168.10.230,7100;Initial Catalog=arcon;Integrated Security=False;User ID=arconsa;Password=arconsa@pass0;Connect Timeout=5;Encrypt=False;TrustServerCertificate=False";
        private string g_ConnectionStr = @"Data Source=127.0.0.1,1433;Initial Catalog=Eventlog;Integrated Security=False;User ID=eventsa;Password=eventsa@pass0;Connect Timeout=5;Encrypt=False;TrustServerCertificate=False";

        private void Initialize()
        {
            if (File.Exists(@"C:\EventLog\SSEvent.cfg"))
            {
                string[] lines = File.ReadAllLines(@"C:\EventLog\SSEvent.cfg");
                UserName = lines[1]; // or whatever.
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(@"C:\EventLog\Error.log"))
                {
                    sw.WriteLine("필요한 파일이 없습니다.");
                    sw.Close();
                }
            }

            TrialMode();

            if (DateTime.Compare(TrialTime.AddDays(7), DateTime.Now) != 1)
                Environment.Exit(0);
            if (DateTime.Compare(LimitTime, DateTime.Now) != 1)
                Environment.Exit(0);

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
                    ListViewItem AddList = new ListViewItem(new string[] { sqlRed.GetDateTime(0).ToString(), sqlRed.GetInt32(1).ToString(), sqlRed.GetString(2).ToString(), sqlRed.GetString(3).ToString(), sqlRed.GetString(4).ToString(), sqlRed.GetString(5).ToString(), sqlRed.GetInt32(6).ToString(), sqlRed.GetString(7).ToString(), sqlRed.GetString(8).ToString(), sqlRed.GetString(9).ToString(), sqlRed.GetString(10).ToString(), sqlRed.GetString(11).ToString(), sqlRed.GetString(12).ToString(), sqlRed.GetDateTime(13).ToString(), sqlRed.GetString(14).ToString(), sqlRed.GetString(15).ToString()});
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
                    ListViewItem AddList = new ListViewItem(new string[] { sqlRed.GetDateTime(0).ToString(), sqlRed.GetInt32(1).ToString(), sqlRed.GetString(2).ToString(), sqlRed.GetString(3).ToString(), sqlRed.GetString(4).ToString(), sqlRed.GetString(5).ToString(), sqlRed.GetInt32(6).ToString(), sqlRed.GetString(7).ToString(), sqlRed.GetString(8).ToString(), sqlRed.GetString(9).ToString(), sqlRed.GetString(10).ToString(), sqlRed.GetString(11).ToString(), sqlRed.GetString(12).ToString(), sqlRed.GetDateTime(13).ToString(), sqlRed.GetString(14).ToString(), sqlRed.GetString(15).ToString()});
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
                    ListViewItem AddList = new ListViewItem(new string[] { sqlRed.GetDateTime(0).ToString(), sqlRed.GetInt32(1).ToString(), sqlRed.GetString(2).ToString(), sqlRed.GetString(3).ToString(), sqlRed.GetString(4).ToString(), sqlRed.GetString(5).ToString(), sqlRed.GetInt32(6).ToString(), sqlRed.GetString(7).ToString(), sqlRed.GetString(8).ToString(), sqlRed.GetString(9).ToString(), sqlRed.GetString(10).ToString(), sqlRed.GetString(11).ToString(), sqlRed.GetString(12).ToString(), sqlRed.GetDateTime(13).ToString(), sqlRed.GetString(14).ToString(), sqlRed.GetString(15).ToString() });
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
