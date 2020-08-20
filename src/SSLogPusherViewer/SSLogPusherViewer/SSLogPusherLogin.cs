using System;
using System.Windows.Forms;

namespace SSLogPusherViewer
{
    public partial class SSLogPusherLogin : Form
    {
        private string g_LoginUserName = "";
        private string g_LoginPassword = "";

        public SSLogPusherLogin()
        {
            InitializeComponent();
            this.MaximizeBox = false;
        }

        private void Button_Login_Click(object sender, EventArgs e)
        {
            g_LoginUserName = TB_UserName.Text;
            g_LoginPassword = TB_Password.Text;

            if (g_LoginUserName == "admin" && g_LoginPassword == "1q2w3e4r")
            {
                this.Hide();

                SSLogPusherViewer Viewer = new SSLogPusherViewer();
                Viewer.ShowDialog();
            }
            else
            {
                MessageBox.Show("계정정보를 확인해주세요.");
            }
        }

        private void Button_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
