namespace SSLogPusherViewer
{
    partial class SSLogPusherViewer
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.DB_ListView_Log = new System.Windows.Forms.ListView();
            this.TB_UserName = new System.Windows.Forms.TextBox();
            this.Button_User = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Button_File = new System.Windows.Forms.Button();
            this.TB_FileName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Button_IPAddress = new System.Windows.Forms.Button();
            this.TB_IPAddress = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.Start_DateTime = new System.Windows.Forms.DateTimePicker();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.End_DateTime = new System.Windows.Forms.DateTimePicker();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // DB_ListView_Log
            // 
            this.DB_ListView_Log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DB_ListView_Log.HideSelection = false;
            this.DB_ListView_Log.Location = new System.Drawing.Point(12, 130);
            this.DB_ListView_Log.Name = "DB_ListView_Log";
            this.DB_ListView_Log.Size = new System.Drawing.Size(1001, 477);
            this.DB_ListView_Log.TabIndex = 0;
            this.DB_ListView_Log.UseCompatibleStateImageBehavior = false;
            // 
            // TB_UserName
            // 
            this.TB_UserName.Location = new System.Drawing.Point(12, 20);
            this.TB_UserName.Name = "TB_UserName";
            this.TB_UserName.Size = new System.Drawing.Size(173, 21);
            this.TB_UserName.TabIndex = 1;
            // 
            // Button_User
            // 
            this.Button_User.Location = new System.Drawing.Point(197, 20);
            this.Button_User.Name = "Button_User";
            this.Button_User.Size = new System.Drawing.Size(127, 21);
            this.Button_User.TabIndex = 2;
            this.Button_User.Text = "UserSearch";
            this.Button_User.UseVisualStyleBackColor = true;
            this.Button_User.Click += new System.EventHandler(this.Button_User_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.Button_User);
            this.groupBox1.Controls.Add(this.TB_UserName);
            this.groupBox1.Location = new System.Drawing.Point(12, 71);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(330, 53);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "User Name";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.Button_File);
            this.groupBox2.Controls.Add(this.TB_FileName);
            this.groupBox2.Location = new System.Drawing.Point(348, 71);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(330, 53);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "File Name";
            // 
            // Button_File
            // 
            this.Button_File.Location = new System.Drawing.Point(197, 20);
            this.Button_File.Name = "Button_File";
            this.Button_File.Size = new System.Drawing.Size(127, 21);
            this.Button_File.TabIndex = 2;
            this.Button_File.Text = "FileSearch";
            this.Button_File.UseVisualStyleBackColor = true;
            this.Button_File.Click += new System.EventHandler(this.Button_File_Click);
            // 
            // TB_FileName
            // 
            this.TB_FileName.Location = new System.Drawing.Point(12, 20);
            this.TB_FileName.Name = "TB_FileName";
            this.TB_FileName.Size = new System.Drawing.Size(173, 21);
            this.TB_FileName.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.Button_IPAddress);
            this.groupBox3.Controls.Add(this.TB_IPAddress);
            this.groupBox3.Location = new System.Drawing.Point(684, 71);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(330, 53);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "IPAddress";
            // 
            // Button_IPAddress
            // 
            this.Button_IPAddress.Location = new System.Drawing.Point(197, 20);
            this.Button_IPAddress.Name = "Button_IPAddress";
            this.Button_IPAddress.Size = new System.Drawing.Size(127, 21);
            this.Button_IPAddress.TabIndex = 2;
            this.Button_IPAddress.Text = "IPAddressSearch";
            this.Button_IPAddress.UseVisualStyleBackColor = true;
            this.Button_IPAddress.Click += new System.EventHandler(this.Button_IPAddress_Click);
            // 
            // TB_IPAddress
            // 
            this.TB_IPAddress.Location = new System.Drawing.Point(12, 20);
            this.TB_IPAddress.Name = "TB_IPAddress";
            this.TB_IPAddress.Size = new System.Drawing.Size(173, 21);
            this.TB_IPAddress.TabIndex = 1;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.Start_DateTime);
            this.groupBox4.Location = new System.Drawing.Point(12, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(167, 53);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Start Date";
            // 
            // Start_DateTime
            // 
            this.Start_DateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Start_DateTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.Start_DateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.Start_DateTime.Location = new System.Drawing.Point(6, 20);
            this.Start_DateTime.Name = "Start_DateTime";
            this.Start_DateTime.Size = new System.Drawing.Size(155, 21);
            this.Start_DateTime.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.End_DateTime);
            this.groupBox5.Location = new System.Drawing.Point(185, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(157, 53);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "End Date";
            // 
            // End_DateTime
            // 
            this.End_DateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.End_DateTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.End_DateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.End_DateTime.Location = new System.Drawing.Point(6, 20);
            this.End_DateTime.Name = "End_DateTime";
            this.End_DateTime.Size = new System.Drawing.Size(145, 21);
            this.End_DateTime.TabIndex = 0;
            // 
            // SSLogPusherViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1025, 634);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.DB_ListView_Log);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "SSLogPusherViewer";
            this.Text = "SSLogPusherViewer";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView DB_ListView_Log;
        private System.Windows.Forms.TextBox TB_UserName;
        private System.Windows.Forms.Button Button_User;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Button_File;
        private System.Windows.Forms.TextBox TB_FileName;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button Button_IPAddress;
        private System.Windows.Forms.TextBox TB_IPAddress;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.DateTimePicker Start_DateTime;
        private System.Windows.Forms.DateTimePicker End_DateTime;
    }
}

