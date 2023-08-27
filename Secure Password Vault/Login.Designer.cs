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
            SuspendLayout();
            // 
            // userNameLbl
            // 
            userNameLbl.AutoSize = true;
            userNameLbl.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            userNameLbl.ForeColor = Color.WhiteSmoke;
            userNameLbl.Location = new Point(12, 9);
            userNameLbl.Name = "userNameLbl";
            userNameLbl.Size = new Size(100, 25);
            userNameLbl.TabIndex = 0;
            userNameLbl.Text = "Username";
            // 
            // userNameTxt
            // 
            userNameTxt.Location = new Point(12, 37);
            userNameTxt.Name = "userNameTxt";
            userNameTxt.Size = new Size(391, 31);
            userNameTxt.TabIndex = 1;
            // 
            // passWordLbl
            // 
            passWordLbl.AutoSize = true;
            passWordLbl.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            passWordLbl.ForeColor = Color.WhiteSmoke;
            passWordLbl.Location = new Point(12, 71);
            passWordLbl.Name = "passWordLbl";
            passWordLbl.Size = new Size(98, 25);
            passWordLbl.TabIndex = 2;
            passWordLbl.Text = "Password";
            // 
            // passTxt
            // 
            passTxt.Location = new Point(12, 99);
            passTxt.Name = "passTxt";
            passTxt.Size = new Size(391, 31);
            passTxt.TabIndex = 3;
            // 
            // logInBtn
            // 
            logInBtn.BackColor = Color.WhiteSmoke;
            logInBtn.FlatAppearance.BorderColor = Color.Black;
            logInBtn.FlatAppearance.BorderSize = 3;
            logInBtn.FlatStyle = FlatStyle.Flat;
            logInBtn.Location = new Point(12, 136);
            logInBtn.Name = "logInBtn";
            logInBtn.Size = new Size(391, 44);
            logInBtn.TabIndex = 4;
            logInBtn.Text = "&Login";
            logInBtn.UseVisualStyleBackColor = false;
            logInBtn.Click += logInBtn_Click;
            // 
            // createNewAccountBtn
            // 
            createNewAccountBtn.BackColor = Color.WhiteSmoke;
            createNewAccountBtn.FlatAppearance.BorderColor = Color.Black;
            createNewAccountBtn.FlatAppearance.BorderSize = 3;
            createNewAccountBtn.FlatStyle = FlatStyle.Flat;
            createNewAccountBtn.Location = new Point(12, 186);
            createNewAccountBtn.Name = "createNewAccountBtn";
            createNewAccountBtn.Size = new Size(391, 44);
            createNewAccountBtn.TabIndex = 5;
            createNewAccountBtn.Text = "&Create New Account";
            createNewAccountBtn.UseVisualStyleBackColor = false;
            createNewAccountBtn.Click += createNewAccountBtn_Click;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightSeaGreen;
            ClientSize = new Size(418, 247);
            Controls.Add(createNewAccountBtn);
            Controls.Add(logInBtn);
            Controls.Add(passTxt);
            Controls.Add(passWordLbl);
            Controls.Add(userNameTxt);
            Controls.Add(userNameLbl);
            Name = "Login";
            Text = "Login";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label userNameLbl;
        private TextBox userNameTxt;
        private Label passWordLbl;
        private TextBox passTxt;
        private Button logInBtn;
        private Button createNewAccountBtn;
    }
}