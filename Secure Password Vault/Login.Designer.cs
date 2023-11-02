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
            userNameLbl = new Label();
            userNameTxt = new TextBox();
            passWordLbl = new Label();
            passTxt = new TextBox();
            logInBtn = new Button();
            createNewAccountBtn = new Button();
            loginBox = new GroupBox();
            AttemptsNumber = new Label();
            AttemptsLeft = new Label();
            rememberMeCheckBox = new CheckBox();
            showPasswordCheckBox = new CheckBox();
            outputLbl = new Label();
            statusLbl = new Label();
            loginBox.SuspendLayout();
            SuspendLayout();
            // 
            // userNameLbl
            // 
            userNameLbl.AutoSize = true;
            userNameLbl.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            userNameLbl.ForeColor = Color.WhiteSmoke;
            userNameLbl.Location = new Point(6, 45);
            userNameLbl.Name = "userNameLbl";
            userNameLbl.Size = new Size(107, 25);
            userNameLbl.TabIndex = 0;
            userNameLbl.Text = "Username";
            // 
            // userNameTxt
            // 
            userNameTxt.BackColor = Color.Black;
            userNameTxt.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            userNameTxt.ForeColor = Color.Gold;
            userNameTxt.Location = new Point(6, 73);
            userNameTxt.Name = "userNameTxt";
            userNameTxt.Size = new Size(438, 33);
            userNameTxt.TabIndex = 1;
            // 
            // passWordLbl
            // 
            passWordLbl.AutoSize = true;
            passWordLbl.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            passWordLbl.ForeColor = Color.WhiteSmoke;
            passWordLbl.Location = new Point(6, 109);
            passWordLbl.Name = "passWordLbl";
            passWordLbl.Size = new Size(103, 25);
            passWordLbl.TabIndex = 2;
            passWordLbl.Text = "Password";
            // 
            // passTxt
            // 
            passTxt.BackColor = Color.Black;
            passTxt.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            passTxt.ForeColor = Color.Gold;
            passTxt.Location = new Point(6, 139);
            passTxt.Name = "passTxt";
            passTxt.Size = new Size(438, 33);
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
            logInBtn.Location = new Point(6, 178);
            logInBtn.Name = "logInBtn";
            logInBtn.Size = new Size(438, 44);
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
            createNewAccountBtn.Location = new Point(6, 228);
            createNewAccountBtn.Name = "createNewAccountBtn";
            createNewAccountBtn.Size = new Size(438, 44);
            createNewAccountBtn.TabIndex = 5;
            createNewAccountBtn.Text = "&Create New Account";
            createNewAccountBtn.UseVisualStyleBackColor = false;
            createNewAccountBtn.Click += createNewAccountBtn_Click;
            // 
            // loginBox
            // 
            loginBox.BackColor = SystemColors.ControlDarkDark;
            loginBox.Controls.Add(AttemptsNumber);
            loginBox.Controls.Add(AttemptsLeft);
            loginBox.Controls.Add(rememberMeCheckBox);
            loginBox.Controls.Add(showPasswordCheckBox);
            loginBox.Controls.Add(outputLbl);
            loginBox.Controls.Add(userNameTxt);
            loginBox.Controls.Add(statusLbl);
            loginBox.Controls.Add(userNameLbl);
            loginBox.Controls.Add(createNewAccountBtn);
            loginBox.Controls.Add(logInBtn);
            loginBox.Controls.Add(passWordLbl);
            loginBox.Controls.Add(passTxt);
            loginBox.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            loginBox.ForeColor = Color.WhiteSmoke;
            loginBox.Location = new Point(12, 12);
            loginBox.Name = "loginBox";
            loginBox.Size = new Size(450, 386);
            loginBox.TabIndex = 6;
            loginBox.TabStop = false;
            loginBox.Text = "Login";
            // 
            // AttemptsNumber
            // 
            AttemptsNumber.AutoSize = true;
            AttemptsNumber.Location = new Point(421, 342);
            AttemptsNumber.Name = "AttemptsNumber";
            AttemptsNumber.Size = new Size(23, 25);
            AttemptsNumber.TabIndex = 15;
            AttemptsNumber.Text = "3";
            // 
            // AttemptsLeft
            // 
            AttemptsLeft.AutoSize = true;
            AttemptsLeft.Location = new Point(253, 342);
            AttemptsLeft.Name = "AttemptsLeft";
            AttemptsLeft.Size = new Size(162, 25);
            AttemptsLeft.TabIndex = 14;
            AttemptsLeft.Text = "Attempts Left ::";
            // 
            // rememberMeCheckBox
            // 
            rememberMeCheckBox.AutoSize = true;
            rememberMeCheckBox.Location = new Point(268, 278);
            rememberMeCheckBox.Name = "rememberMeCheckBox";
            rememberMeCheckBox.Size = new Size(176, 29);
            rememberMeCheckBox.TabIndex = 13;
            rememberMeCheckBox.Text = "Remember Me";
            rememberMeCheckBox.UseVisualStyleBackColor = true;
            // 
            // showPasswordCheckBox
            // 
            showPasswordCheckBox.AutoSize = true;
            showPasswordCheckBox.Location = new Point(6, 278);
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
            outputLbl.Location = new Point(102, 342);
            outputLbl.Name = "outputLbl";
            outputLbl.Size = new Size(67, 25);
            outputLbl.TabIndex = 10;
            outputLbl.Text = "Idle...";
            // 
            // statusLbl
            // 
            statusLbl.AutoSize = true;
            statusLbl.Location = new Point(6, 342);
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
            ClientSize = new Size(474, 412);
            Controls.Add(loginBox);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Login";
            Text = "Login";
            Load += Login_Load;
            loginBox.ResumeLayout(false);
            loginBox.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label userNameLbl;
        private TextBox userNameTxt;
        private Label passWordLbl;
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
    }
}