namespace Secure_Password_Vault
{
    partial class RegisterAccount
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegisterAccount));
            userLbl = new Label();
            userTxt = new TextBox();
            passLbl = new Label();
            passTxt = new TextBox();
            confirmPassLbl = new Label();
            confirmPassTxt = new TextBox();
            CreateAccountBtn = new Button();
            statusLbl = new Label();
            outputLbl = new Label();
            CancelBtn = new Button();
            showPasswordCheckBox = new CheckBox();
            RegisterBox = new GroupBox();
            RegisterBox.SuspendLayout();
            SuspendLayout();
            // 
            // userLbl
            // 
            userLbl.AutoSize = true;
            userLbl.Location = new Point(6, 41);
            userLbl.Name = "userLbl";
            userLbl.Size = new Size(107, 25);
            userLbl.TabIndex = 0;
            userLbl.Text = "Username";
            // 
            // userTxt
            // 
            userTxt.BackColor = Color.Black;
            userTxt.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            userTxt.ForeColor = Color.Gold;
            userTxt.Location = new Point(6, 69);
            userTxt.Name = "userTxt";
            userTxt.Size = new Size(438, 33);
            userTxt.TabIndex = 1;
            // 
            // passLbl
            // 
            passLbl.AutoSize = true;
            passLbl.Location = new Point(6, 105);
            passLbl.Name = "passLbl";
            passLbl.Size = new Size(103, 25);
            passLbl.TabIndex = 2;
            passLbl.Text = "Password";
            // 
            // passTxt
            // 
            passTxt.BackColor = Color.Black;
            passTxt.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            passTxt.ForeColor = Color.Gold;
            passTxt.Location = new Point(6, 133);
            passTxt.Name = "passTxt";
            passTxt.Size = new Size(438, 33);
            passTxt.TabIndex = 3;
            passTxt.UseSystemPasswordChar = true;
            // 
            // confirmPassLbl
            // 
            confirmPassLbl.AutoSize = true;
            confirmPassLbl.Location = new Point(6, 169);
            confirmPassLbl.Name = "confirmPassLbl";
            confirmPassLbl.Size = new Size(188, 25);
            confirmPassLbl.TabIndex = 4;
            confirmPassLbl.Text = "Confirm Password";
            // 
            // confirmPassTxt
            // 
            confirmPassTxt.BackColor = Color.Black;
            confirmPassTxt.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            confirmPassTxt.ForeColor = Color.Gold;
            confirmPassTxt.Location = new Point(6, 197);
            confirmPassTxt.Name = "confirmPassTxt";
            confirmPassTxt.Size = new Size(438, 33);
            confirmPassTxt.TabIndex = 5;
            confirmPassTxt.UseSystemPasswordChar = true;
            // 
            // CreateAccountBtn
            // 
            CreateAccountBtn.BackColor = SystemColors.ControlDarkDark;
            CreateAccountBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            CreateAccountBtn.FlatAppearance.BorderSize = 3;
            CreateAccountBtn.FlatStyle = FlatStyle.Flat;
            CreateAccountBtn.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            CreateAccountBtn.ForeColor = Color.WhiteSmoke;
            CreateAccountBtn.Location = new Point(6, 239);
            CreateAccountBtn.Name = "CreateAccountBtn";
            CreateAccountBtn.Size = new Size(438, 44);
            CreateAccountBtn.TabIndex = 6;
            CreateAccountBtn.Text = "&Create Account";
            CreateAccountBtn.UseVisualStyleBackColor = false;
            CreateAccountBtn.Click += CreateAccountBtn_Click;
            // 
            // statusLbl
            // 
            statusLbl.AutoSize = true;
            statusLbl.Location = new Point(6, 387);
            statusLbl.Name = "statusLbl";
            statusLbl.Size = new Size(90, 25);
            statusLbl.TabIndex = 8;
            statusLbl.Text = "Status ::";
            // 
            // outputLbl
            // 
            outputLbl.AutoSize = true;
            outputLbl.Location = new Point(102, 387);
            outputLbl.Name = "outputLbl";
            outputLbl.Size = new Size(67, 25);
            outputLbl.TabIndex = 9;
            outputLbl.Text = "Idle...";
            // 
            // CancelBtn
            // 
            CancelBtn.BackColor = SystemColors.ControlDarkDark;
            CancelBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            CancelBtn.FlatAppearance.BorderSize = 3;
            CancelBtn.FlatStyle = FlatStyle.Flat;
            CancelBtn.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            CancelBtn.ForeColor = Color.WhiteSmoke;
            CancelBtn.Location = new Point(6, 289);
            CancelBtn.Name = "CancelBtn";
            CancelBtn.Size = new Size(438, 44);
            CancelBtn.TabIndex = 10;
            CancelBtn.Text = "&Cancel";
            CancelBtn.UseVisualStyleBackColor = false;
            CancelBtn.Click += CancelBtn_Click;
            // 
            // showPasswordCheckBox
            // 
            showPasswordCheckBox.AutoSize = true;
            showPasswordCheckBox.Location = new Point(258, 339);
            showPasswordCheckBox.Name = "showPasswordCheckBox";
            showPasswordCheckBox.Size = new Size(186, 29);
            showPasswordCheckBox.TabIndex = 11;
            showPasswordCheckBox.Text = "Show Password";
            showPasswordCheckBox.UseVisualStyleBackColor = true;
            showPasswordCheckBox.CheckedChanged += ShowPasswordCheckBox_CheckedChanged;
            // 
            // RegisterBox
            // 
            RegisterBox.BackColor = SystemColors.ControlDarkDark;
            RegisterBox.Controls.Add(showPasswordCheckBox);
            RegisterBox.Controls.Add(CancelBtn);
            RegisterBox.Controls.Add(outputLbl);
            RegisterBox.Controls.Add(statusLbl);
            RegisterBox.Controls.Add(CreateAccountBtn);
            RegisterBox.Controls.Add(confirmPassTxt);
            RegisterBox.Controls.Add(confirmPassLbl);
            RegisterBox.Controls.Add(passTxt);
            RegisterBox.Controls.Add(passLbl);
            RegisterBox.Controls.Add(userTxt);
            RegisterBox.Controls.Add(userLbl);
            RegisterBox.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            RegisterBox.ForeColor = Color.WhiteSmoke;
            RegisterBox.Location = new Point(12, 12);
            RegisterBox.Name = "RegisterBox";
            RegisterBox.Size = new Size(450, 434);
            RegisterBox.TabIndex = 0;
            RegisterBox.TabStop = false;
            RegisterBox.Text = "Register Account";
            // 
            // RegisterAccount
            // 
            AcceptButton = CreateAccountBtn;
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(474, 458);
            Controls.Add(RegisterBox);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "RegisterAccount";
            Text = "Register Account";
            RegisterBox.ResumeLayout(false);
            RegisterBox.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label userLbl;
        private TextBox userTxt;
        private Label passLbl;
        private TextBox passTxt;
        private Label confirmPassLbl;
        private TextBox confirmPassTxt;
        private Button CreateAccountBtn;
        private Label statusLbl;
        private Label outputLbl;
        private Button CancelBtn;
        private CheckBox showPasswordCheckBox;
        private GroupBox RegisterBox;
    }
}