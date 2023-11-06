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
            createAccountBtn = new Button();
            statusLbl = new Label();
            outputLbl = new Label();
            cancelBtn = new Button();
            showPasswordCheckBox = new CheckBox();
            groupBox1 = new GroupBox();
            groupBox1.SuspendLayout();
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
            // createAccountBtn
            // 
            createAccountBtn.BackColor = SystemColors.ControlDarkDark;
            createAccountBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            createAccountBtn.FlatAppearance.BorderSize = 3;
            createAccountBtn.FlatStyle = FlatStyle.Flat;
            createAccountBtn.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            createAccountBtn.ForeColor = Color.WhiteSmoke;
            createAccountBtn.Location = new Point(6, 239);
            createAccountBtn.Name = "createAccountBtn";
            createAccountBtn.Size = new Size(438, 44);
            createAccountBtn.TabIndex = 6;
            createAccountBtn.Text = "&Create Account";
            createAccountBtn.UseVisualStyleBackColor = false;
            createAccountBtn.Click += createAccountBtn_Click;
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
            // cancelBtn
            // 
            cancelBtn.BackColor = SystemColors.ControlDarkDark;
            cancelBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            cancelBtn.FlatAppearance.BorderSize = 3;
            cancelBtn.FlatStyle = FlatStyle.Flat;
            cancelBtn.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cancelBtn.ForeColor = Color.WhiteSmoke;
            cancelBtn.Location = new Point(6, 289);
            cancelBtn.Name = "cancelBtn";
            cancelBtn.Size = new Size(438, 44);
            cancelBtn.TabIndex = 10;
            cancelBtn.Text = "&Cancel";
            cancelBtn.UseVisualStyleBackColor = false;
            cancelBtn.Click += cancelBtn_Click;
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
            showPasswordCheckBox.CheckedChanged += showPasswordCheckBox_CheckedChanged;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.ControlDarkDark;
            groupBox1.Controls.Add(showPasswordCheckBox);
            groupBox1.Controls.Add(cancelBtn);
            groupBox1.Controls.Add(outputLbl);
            groupBox1.Controls.Add(statusLbl);
            groupBox1.Controls.Add(createAccountBtn);
            groupBox1.Controls.Add(confirmPassTxt);
            groupBox1.Controls.Add(confirmPassLbl);
            groupBox1.Controls.Add(passTxt);
            groupBox1.Controls.Add(passLbl);
            groupBox1.Controls.Add(userTxt);
            groupBox1.Controls.Add(userLbl);
            groupBox1.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox1.ForeColor = Color.WhiteSmoke;
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(450, 434);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Register Account";
            // 
            // RegisterAccount
            // 
            AcceptButton = createAccountBtn;
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(474, 458);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "RegisterAccount";
            Text = "Register Account";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label userLbl;
        private TextBox userTxt;
        private Label passLbl;
        private TextBox passTxt;
        private Label confirmPassLbl;
        private TextBox confirmPassTxt;
        private Button createAccountBtn;
        private Label statusLbl;
        private Label outputLbl;
        private Button cancelBtn;
        private CheckBox showPasswordCheckBox;
        private GroupBox groupBox1;
    }
}