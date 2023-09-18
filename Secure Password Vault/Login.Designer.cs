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
            userNameLbl = new Label();
            userNameTxt = new TextBox();
            passWordLbl = new Label();
            passTxt = new TextBox();
            logInBtn = new Button();
            createNewAccountBtn = new Button();
            loginBox = new GroupBox();
            outputLbl = new Label();
            statusLbl = new Label();
            loginBox.SuspendLayout();
            SuspendLayout();
            // 
            // userNameLbl
            // 
            userNameLbl.AutoSize = true;
            userNameLbl.Font = new Font("Segoe Script", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            userNameLbl.ForeColor = Color.DarkRed;
            userNameLbl.Location = new Point(6, 38);
            userNameLbl.Name = "userNameLbl";
            userNameLbl.Size = new Size(128, 35);
            userNameLbl.TabIndex = 0;
            userNameLbl.Text = "Username";
            // 
            // userNameTxt
            // 
            userNameTxt.BackColor = Color.Black;
            userNameTxt.Font = new Font("Segoe Script", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            userNameTxt.ForeColor = Color.DarkRed;
            userNameTxt.Location = new Point(6, 76);
            userNameTxt.Name = "userNameTxt";
            userNameTxt.Size = new Size(391, 36);
            userNameTxt.TabIndex = 1;
            // 
            // passWordLbl
            // 
            passWordLbl.AutoSize = true;
            passWordLbl.Font = new Font("Segoe Script", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            passWordLbl.ForeColor = Color.DarkRed;
            passWordLbl.Location = new Point(6, 121);
            passWordLbl.Name = "passWordLbl";
            passWordLbl.Size = new Size(124, 35);
            passWordLbl.TabIndex = 2;
            passWordLbl.Text = "Password";
            // 
            // passTxt
            // 
            passTxt.BackColor = Color.Black;
            passTxt.Font = new Font("Segoe Script", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            passTxt.ForeColor = Color.DarkRed;
            passTxt.Location = new Point(6, 159);
            passTxt.Name = "passTxt";
            passTxt.Size = new Size(391, 36);
            passTxt.TabIndex = 3;
            passTxt.UseSystemPasswordChar = true;
            // 
            // logInBtn
            // 
            logInBtn.BackColor = Color.Black;
            logInBtn.FlatAppearance.BorderColor = Color.DarkRed;
            logInBtn.FlatAppearance.BorderSize = 3;
            logInBtn.FlatStyle = FlatStyle.Flat;
            logInBtn.Font = new Font("Segoe Script", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            logInBtn.ForeColor = Color.DarkRed;
            logInBtn.Location = new Point(6, 201);
            logInBtn.Name = "logInBtn";
            logInBtn.Size = new Size(391, 44);
            logInBtn.TabIndex = 4;
            logInBtn.Text = "&Login";
            logInBtn.UseVisualStyleBackColor = false;
            logInBtn.Click += logInBtn_Click;
            // 
            // createNewAccountBtn
            // 
            createNewAccountBtn.BackColor = Color.Black;
            createNewAccountBtn.FlatAppearance.BorderColor = Color.DarkRed;
            createNewAccountBtn.FlatAppearance.BorderSize = 3;
            createNewAccountBtn.FlatStyle = FlatStyle.Flat;
            createNewAccountBtn.Font = new Font("Segoe Script", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            createNewAccountBtn.ForeColor = Color.DarkRed;
            createNewAccountBtn.Location = new Point(6, 251);
            createNewAccountBtn.Name = "createNewAccountBtn";
            createNewAccountBtn.Size = new Size(391, 44);
            createNewAccountBtn.TabIndex = 5;
            createNewAccountBtn.Text = "&Create New Account";
            createNewAccountBtn.UseVisualStyleBackColor = false;
            createNewAccountBtn.Click += createNewAccountBtn_Click;
            // 
            // loginBox
            // 
            loginBox.BackColor = Color.Black;
            loginBox.Controls.Add(outputLbl);
            loginBox.Controls.Add(userNameTxt);
            loginBox.Controls.Add(statusLbl);
            loginBox.Controls.Add(userNameLbl);
            loginBox.Controls.Add(createNewAccountBtn);
            loginBox.Controls.Add(logInBtn);
            loginBox.Controls.Add(passWordLbl);
            loginBox.Controls.Add(passTxt);
            loginBox.Font = new Font("Segoe Script", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            loginBox.ForeColor = Color.DarkRed;
            loginBox.Location = new Point(12, 12);
            loginBox.Name = "loginBox";
            loginBox.Size = new Size(412, 374);
            loginBox.TabIndex = 6;
            loginBox.TabStop = false;
            loginBox.Text = "Login";
            // 
            // outputLbl
            // 
            outputLbl.AutoSize = true;
            outputLbl.Location = new Point(122, 327);
            outputLbl.Name = "outputLbl";
            outputLbl.Size = new Size(78, 35);
            outputLbl.TabIndex = 10;
            outputLbl.Text = "Idle...";
            // 
            // statusLbl
            // 
            statusLbl.AutoSize = true;
            statusLbl.Location = new Point(6, 327);
            statusLbl.Name = "statusLbl";
            statusLbl.Size = new Size(110, 35);
            statusLbl.TabIndex = 9;
            statusLbl.Text = "Status ::";
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(437, 398);
            Controls.Add(loginBox);
            Name = "Login";
            Text = "Login";
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
    }
}