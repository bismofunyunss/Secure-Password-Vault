namespace Secure_Password_Vault
{
    partial class Login
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            userNameTxt = new TextBox();
            passTxt = new TextBox();
            logInBtn = new Button();
            createNewAccountBtn = new Button();
            loginBox = new GroupBox();
            pictureBox3 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            PasswordLbl = new Label();
            Usernamelbl = new Label();
            AttemptsNumber = new Label();
            AttemptsLeft = new Label();
            rememberMeCheckBox = new CheckBox();
            showPasswordCheckBox = new CheckBox();
            outputLbl = new Label();
            statusLbl = new Label();
            loginBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // userNameTxt
            // 
            userNameTxt.BackColor = Color.Black;
            userNameTxt.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            userNameTxt.ForeColor = Color.Gold;
            userNameTxt.Location = new Point(6, 96);
            userNameTxt.Name = "userNameTxt";
            userNameTxt.Size = new Size(519, 33);
            userNameTxt.TabIndex = 1;
            // 
            // passTxt
            // 
            passTxt.BackColor = Color.Black;
            passTxt.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            passTxt.ForeColor = Color.Gold;
            passTxt.Location = new Point(6, 177);
            passTxt.Name = "passTxt";
            passTxt.Size = new Size(519, 33);
            passTxt.TabIndex = 3;
            passTxt.UseSystemPasswordChar = true;
            // 
            // logInBtn
            // 
            logInBtn.BackColor = SystemColors.ControlDarkDark;
            logInBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            logInBtn.FlatAppearance.BorderSize = 3;
            logInBtn.FlatStyle = FlatStyle.Flat;
            logInBtn.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            logInBtn.ForeColor = Color.WhiteSmoke;
            logInBtn.Location = new Point(6, 216);
            logInBtn.Name = "logInBtn";
            logInBtn.Size = new Size(519, 44);
            logInBtn.TabIndex = 4;
            logInBtn.Text = "&Login";
            logInBtn.UseVisualStyleBackColor = false;
            logInBtn.Click += logInBtn_Click;
            // 
            // createNewAccountBtn
            // 
            createNewAccountBtn.BackColor = SystemColors.ControlDarkDark;
            createNewAccountBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            createNewAccountBtn.FlatAppearance.BorderSize = 3;
            createNewAccountBtn.FlatStyle = FlatStyle.Flat;
            createNewAccountBtn.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            createNewAccountBtn.ForeColor = Color.WhiteSmoke;
            createNewAccountBtn.Location = new Point(6, 266);
            createNewAccountBtn.Name = "createNewAccountBtn";
            createNewAccountBtn.Size = new Size(519, 44);
            createNewAccountBtn.TabIndex = 5;
            createNewAccountBtn.Text = "&Create New Account";
            createNewAccountBtn.UseVisualStyleBackColor = false;
            createNewAccountBtn.Click += createNewAccountBtn_Click;
            // 
            // loginBox
            // 
            loginBox.BackColor = SystemColors.ControlDarkDark;
            loginBox.Controls.Add(pictureBox3);
            loginBox.Controls.Add(pictureBox2);
            loginBox.Controls.Add(pictureBox1);
            loginBox.Controls.Add(PasswordLbl);
            loginBox.Controls.Add(Usernamelbl);
            loginBox.Controls.Add(AttemptsNumber);
            loginBox.Controls.Add(AttemptsLeft);
            loginBox.Controls.Add(rememberMeCheckBox);
            loginBox.Controls.Add(showPasswordCheckBox);
            loginBox.Controls.Add(outputLbl);
            loginBox.Controls.Add(userNameTxt);
            loginBox.Controls.Add(statusLbl);
            loginBox.Controls.Add(createNewAccountBtn);
            loginBox.Controls.Add(logInBtn);
            loginBox.Controls.Add(passTxt);
            loginBox.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            loginBox.ForeColor = Color.WhiteSmoke;
            loginBox.Location = new Point(12, 12);
            loginBox.Name = "loginBox";
            loginBox.Size = new Size(531, 410);
            loginBox.TabIndex = 6;
            loginBox.TabStop = false;
            loginBox.Text = "Login";
            // 
            // pictureBox3
            // 
            pictureBox3.Image = Properties.Resources.key__1_;
            pictureBox3.Location = new Point(119, 135);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(34, 36);
            pictureBox3.TabIndex = 20;
            pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.profile;
            pictureBox2.Location = new Point(119, 57);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(34, 33);
            pictureBox2.TabIndex = 19;
            pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.cyber_security__3_;
            pictureBox1.Location = new Point(460, 19);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(65, 71);
            pictureBox1.TabIndex = 18;
            pictureBox1.TabStop = false;
            // 
            // PasswordLbl
            // 
            PasswordLbl.AutoSize = true;
            PasswordLbl.Location = new Point(6, 146);
            PasswordLbl.Name = "PasswordLbl";
            PasswordLbl.Size = new Size(103, 25);
            PasswordLbl.TabIndex = 17;
            PasswordLbl.Text = "Password";
            // 
            // Usernamelbl
            // 
            Usernamelbl.AutoSize = true;
            Usernamelbl.Location = new Point(6, 65);
            Usernamelbl.Name = "Usernamelbl";
            Usernamelbl.Size = new Size(107, 25);
            Usernamelbl.TabIndex = 16;
            Usernamelbl.Text = "Username";
            // 
            // AttemptsNumber
            // 
            AttemptsNumber.AutoSize = true;
            AttemptsNumber.Location = new Point(502, 367);
            AttemptsNumber.Name = "AttemptsNumber";
            AttemptsNumber.Size = new Size(23, 25);
            AttemptsNumber.TabIndex = 15;
            AttemptsNumber.Text = "3";
            // 
            // AttemptsLeft
            // 
            AttemptsLeft.AutoSize = true;
            AttemptsLeft.Location = new Point(272, 367);
            AttemptsLeft.Name = "AttemptsLeft";
            AttemptsLeft.Size = new Size(224, 25);
            AttemptsLeft.TabIndex = 14;
            AttemptsLeft.Text = "Attempts Remaining ::";
            // 
            // rememberMeCheckBox
            // 
            rememberMeCheckBox.AutoSize = true;
            rememberMeCheckBox.Location = new Point(349, 316);
            rememberMeCheckBox.Name = "rememberMeCheckBox";
            rememberMeCheckBox.Size = new Size(176, 29);
            rememberMeCheckBox.TabIndex = 13;
            rememberMeCheckBox.Text = "Remember Me";
            rememberMeCheckBox.UseVisualStyleBackColor = true;
            // 
            // showPasswordCheckBox
            // 
            showPasswordCheckBox.AutoSize = true;
            showPasswordCheckBox.Location = new Point(6, 316);
            showPasswordCheckBox.Name = "showPasswordCheckBox";
            showPasswordCheckBox.Size = new Size(186, 29);
            showPasswordCheckBox.TabIndex = 12;
            showPasswordCheckBox.Text = "Show Password";
            showPasswordCheckBox.UseVisualStyleBackColor = true;
            showPasswordCheckBox.CheckedChanged += showPasswordCheckBox_CheckedChanged;
            // 
            // outputLbl
            // 
            outputLbl.AutoSize = true;
            outputLbl.Location = new Point(102, 367);
            outputLbl.Name = "outputLbl";
            outputLbl.Size = new Size(67, 25);
            outputLbl.TabIndex = 10;
            outputLbl.Text = "Idle...";
            // 
            // statusLbl
            // 
            statusLbl.AutoSize = true;
            statusLbl.Location = new Point(6, 367);
            statusLbl.Name = "statusLbl";
            statusLbl.Size = new Size(90, 25);
            statusLbl.TabIndex = 9;
            statusLbl.Text = "Status ::";
            // 
            // Login
            // 
            AcceptButton = logInBtn;
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            BackgroundImageLayout = ImageLayout.Center;
            ClientSize = new Size(555, 436);
            Controls.Add(loginBox);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Login";
            Text = "Login";
            Load += Login_Load;
            loginBox.ResumeLayout(false);
            loginBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private TextBox userNameTxt;
        private TextBox passTxt;
        private Button logInBtn;
        private Button createNewAccountBtn;
        private GroupBox loginBox;
        private Label statusLbl;
        private Label outputLbl;
        private CheckBox showPasswordCheckBox;
        private CheckBox rememberMeCheckBox;
        private Label AttemptsNumber;
        private Label AttemptsLeft;
        private Label PasswordLbl;
        private Label Usernamelbl;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
    }
}