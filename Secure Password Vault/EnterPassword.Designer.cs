namespace Secure_Password_Vault
{
    partial class EnterPassword
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
            PasswordGrp = new GroupBox();
            outputLbl = new Label();
            statusLbl = new Label();
            EnterPasswordBtn = new Button();
            PassWordBox = new TextBox();
            PasswordLabel = new Label();
            showPasswordCheckBox = new CheckBox();
            PasswordGrp.SuspendLayout();
            SuspendLayout();
            // 
            // PasswordGrp
            // 
            PasswordGrp.Controls.Add(showPasswordCheckBox);
            PasswordGrp.Controls.Add(outputLbl);
            PasswordGrp.Controls.Add(statusLbl);
            PasswordGrp.Controls.Add(EnterPasswordBtn);
            PasswordGrp.Controls.Add(PassWordBox);
            PasswordGrp.Controls.Add(PasswordLabel);
            PasswordGrp.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            PasswordGrp.ForeColor = Color.WhiteSmoke;
            PasswordGrp.Location = new Point(12, 12);
            PasswordGrp.Name = "PasswordGrp";
            PasswordGrp.Size = new Size(493, 242);
            PasswordGrp.TabIndex = 0;
            PasswordGrp.TabStop = false;
            PasswordGrp.Text = "Confirm Password";
            // 
            // outputLbl
            // 
            outputLbl.AutoSize = true;
            outputLbl.Location = new Point(102, 205);
            outputLbl.Name = "outputLbl";
            outputLbl.Size = new Size(67, 25);
            outputLbl.TabIndex = 11;
            outputLbl.Text = "Idle...";
            // 
            // statusLbl
            // 
            statusLbl.AutoSize = true;
            statusLbl.Location = new Point(6, 205);
            statusLbl.Name = "statusLbl";
            statusLbl.Size = new Size(90, 25);
            statusLbl.TabIndex = 10;
            statusLbl.Text = "Status ::";
            // 
            // EnterPasswordBtn
            // 
            EnterPasswordBtn.BackColor = SystemColors.ControlDarkDark;
            EnterPasswordBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            EnterPasswordBtn.FlatAppearance.BorderSize = 3;
            EnterPasswordBtn.FlatStyle = FlatStyle.Flat;
            EnterPasswordBtn.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            EnterPasswordBtn.ForeColor = Color.WhiteSmoke;
            EnterPasswordBtn.Location = new Point(6, 107);
            EnterPasswordBtn.Name = "EnterPasswordBtn";
            EnterPasswordBtn.Size = new Size(481, 44);
            EnterPasswordBtn.TabIndex = 7;
            EnterPasswordBtn.Text = "&Enter";
            EnterPasswordBtn.UseVisualStyleBackColor = false;
            EnterPasswordBtn.Click += EnterPasswordBtn_Click;
            // 
            // PassWordBox
            // 
            PassWordBox.BackColor = Color.Black;
            PassWordBox.ForeColor = Color.Gold;
            PassWordBox.Location = new Point(6, 68);
            PassWordBox.Name = "PassWordBox";
            PassWordBox.Size = new Size(481, 33);
            PassWordBox.TabIndex = 1;
            PassWordBox.UseSystemPasswordChar = true;
            // 
            // PasswordLabel
            // 
            PasswordLabel.AutoSize = true;
            PasswordLabel.Location = new Point(6, 40);
            PasswordLabel.Name = "PasswordLabel";
            PasswordLabel.Size = new Size(103, 25);
            PasswordLabel.TabIndex = 0;
            PasswordLabel.Text = "Password";
            // 
            // showPasswordCheckBox
            // 
            showPasswordCheckBox.AutoSize = true;
            showPasswordCheckBox.Location = new Point(301, 157);
            showPasswordCheckBox.Name = "showPasswordCheckBox";
            showPasswordCheckBox.Size = new Size(186, 29);
            showPasswordCheckBox.TabIndex = 13;
            showPasswordCheckBox.Text = "Show Password";
            showPasswordCheckBox.UseVisualStyleBackColor = true;
            showPasswordCheckBox.CheckedChanged += showPasswordCheckBox_CheckedChanged;
            // 
            // EnterPassword
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(517, 266);
            Controls.Add(PasswordGrp);
            Name = "EnterPassword";
            Text = "Enter Password";
            FormClosing += EnterPassword_FormClosing;
            PasswordGrp.ResumeLayout(false);
            PasswordGrp.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox PasswordGrp;
        private Label PasswordLabel;
        private TextBox PassWordBox;
        private Button EnterPasswordBtn;
        private Label outputLbl;
        private Label statusLbl;
        private CheckBox showPasswordCheckBox;
    }
}