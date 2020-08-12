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

namespace SSEventLogViewer
{
    public partial class SSEventLogViewer : Form
    {
        public SSEventLogViewer()
        {
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            DB_ListView_Log.Clear();
            DB_ListView_Log.View = View.Details;
            DB_ListView_Log.GridLines = true;
            DB_ListView_Log.FullRowSelect = true;
            DB_ListView_Log.Columns.Add("SearchTime", 150);
            DB_ListView_Log.Columns.Add("ID", 50);
            DB_ListView_Log.Columns.Add("UserID", 70);
            DB_ListView_Log.Columns.Add("PCName", 70);
            DB_ListView_Log.Columns.Add("Subject", 50);
            DB_ListView_Log.Columns.Add("IPAddress", 100);
            DB_ListView_Log.Columns.Add("Port", 70);
            DB_ListView_Log.Columns.Add("Path", 150);
            DB_ListView_Log.Columns.Add("FilePath", 150);
            DB_ListView_Log.Columns.Add("Filename", 150);
            DB_ListView_Log.Columns.Add("type", 100);
            DB_ListView_Log.Columns.Add("AccessType", 100);
            DB_ListView_Log.Columns.Add("Reason", 100);
            DB_ListView_Log.Columns.Add("CreateTime", 150);
            DB_ListView_Log.Columns.Add("Login_ID", 50);
            DB_ListView_Log.Columns.Add("Information", 50);
        }
        private void Button_User_Click(object sender, EventArgs e)
        {
            Initialize();
            /////  DBConnect Parameter   /////
            string g_ConnectionStr = @"Data Source=192.168.10.230,7100;Initial Catalog=arcon;Integrated Security=False;User ID=arconsa;Password=arconsa@pass0;Connect Timeout=5;Encrypt=False;TrustServerCertificate=False";
            //string SQL_CONNSTR = @"Network Library=DBMSSOCN;Data Source=192.168.10.230,7100;Initial Catalog=OTP_TEST_DB;User Id=arconsa;Password=arconsa@pass0";

            SqlConnection sqlCon = new SqlConnection(g_ConnectionStr);
            SqlCommand sqlCmd = new SqlCommand();
            SqlDataReader sqlRed;
            string m_User = TB_UserName.Text;

            /////  DBConnect Parameter   /////
            try
            {
                sqlCon.Open();
                sqlCmd.Connection = sqlCon;

                sqlCmd.CommandText = $"select DISTINCT * From arcon.dbo.eventlogview where username like '%" + m_User + "%'";
                sqlRed = sqlCmd.ExecuteReader();
                while (sqlRed.Read())
                {
                    if (m_User == "")
                        break;
                    ListViewItem AddList = new ListViewItem(new string[] { sqlRed.GetDateTime(0).ToString(), sqlRed.GetInt32(1).ToString(), sqlRed.GetString(2).ToString(), sqlRed.GetString(3).ToString(), sqlRed.GetString(4).ToString(), sqlRed.GetString(5).ToString(), sqlRed.GetInt32(6).ToString(), sqlRed.GetString(7).ToString(), sqlRed.GetString(8).ToString(), sqlRed.GetString(9).ToString(), sqlRed.GetString(10).ToString(), sqlRed.GetString(11).ToString(), sqlRed.GetString(12).ToString(), sqlRed.GetDateTime(13).ToString(), sqlRed.GetString(14).ToString(), sqlRed.GetString(15).ToString(), sqlRed.GetInt32(16).ToString(), sqlRed.GetDateTime(0).ToString() });
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
            string g_ConnectionStr = @"Data Source=192.168.10.230,7100;Initial Catalog=arcon;Integrated Security=False;User ID=arconsa;Password=arconsa@pass0;Connect Timeout=5;Encrypt=False;TrustServerCertificate=False";
            //string SQL_CONNSTR = @"Network Library=DBMSSOCN;Data Source=192.168.10.230,7100;Initial Catalog=OTP_TEST_DB;User Id=arconsa;Password=arconsa@pass0";

            SqlConnection sqlCon = new SqlConnection(g_ConnectionStr);
            SqlCommand sqlCmd = new SqlCommand();
            SqlDataReader sqlRed;
            string m_File = TB_FileName.Text;
            /////  DBConnect Parameter   /////
            try
            {
                sqlCon.Open();
                sqlCmd.Connection = sqlCon;

                sqlCmd.CommandText = $"select DISTINCT * From arcon.dbo.eventlogview where FilePath like '%" + m_File + "%'";
                sqlRed = sqlCmd.ExecuteReader();
                while(sqlRed.Read())
                {
                    if (m_File == "")
                        break;
                    ListViewItem AddList = new ListViewItem(new string[] { sqlRed.GetDateTime(0).ToString(), sqlRed.GetInt32(1).ToString(), sqlRed.GetString(2).ToString(), sqlRed.GetString(3).ToString(), sqlRed.GetString(4).ToString(), sqlRed.GetString(5).ToString(), sqlRed.GetInt32(6).ToString(), sqlRed.GetString(7).ToString(), sqlRed.GetString(8).ToString(), sqlRed.GetString(9).ToString(), sqlRed.GetString(10).ToString(), sqlRed.GetString(11).ToString(), sqlRed.GetString(12).ToString(), sqlRed.GetDateTime(13).ToString(), sqlRed.GetString(14).ToString(), sqlRed.GetString(15).ToString(), sqlRed.GetInt32(16).ToString(), sqlRed.GetDateTime(0).ToString()});
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
