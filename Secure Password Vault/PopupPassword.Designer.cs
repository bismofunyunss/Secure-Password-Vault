namespace Secure_Password_Vault
{
    partial class PopupPassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupPassword));
            passLbl = new Label();
            passTxt = new TextBox();
            confirmPassBtn = new Button();
            groupBox1 = new GroupBox();
            outputLbl = new Label();
            statusLbl = new Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // passLbl
            // 
            passLbl.AutoSize = true;
            passLbl.Font = new Font("Segoe Script", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            passLbl.ForeColor = Color.WhiteSmoke;
            passLbl.Location = new Point(6, 40);
            passLbl.Name = "passLbl";
            passLbl.Size = new Size(126, 37);
            passLbl.TabIndex = 3;
            passLbl.Text = "Password";
            // 
            // passTxt
            // 
            passTxt.BackColor = SystemColors.ButtonShadow;
            passTxt.Font = new Font("Segoe Script", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            passTxt.ForeColor = Color.Gold;
            passTxt.Location = new Point(6, 90);
            passTxt.Name = "passTxt";
            passTxt.Size = new Size(456, 36);
            passTxt.TabIndex = 4;
            passTxt.UseSystemPasswordChar = true;
            // 
            // confirmPassBtn
            // 
            confirmPassBtn.BackColor = SystemColors.ButtonShadow;
            confirmPassBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            confirmPassBtn.FlatAppearance.BorderSize = 3;
            confirmPassBtn.FlatStyle = FlatStyle.Flat;
            confirmPassBtn.Location = new Point(6, 132);
            confirmPassBtn.Name = "confirmPassBtn";
            confirmPassBtn.Size = new Size(456, 44);
            confirmPassBtn.TabIndex = 6;
            confirmPassBtn.Text = "&Confirm Password";
            confirmPassBtn.UseVisualStyleBackColor = false;
            confirmPassBtn.Click += confirmPassBtn_ClickAsync;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.ButtonShadow;
            groupBox1.Controls.Add(outputLbl);
            groupBox1.Controls.Add(statusLbl);
            groupBox1.Controls.Add(passLbl);
            groupBox1.Controls.Add(passTxt);
            groupBox1.Controls.Add(confirmPassBtn);
            groupBox1.Font = new Font("Segoe Script", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBox1.ForeColor = Color.WhiteSmoke;
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(468, 269);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Password Confirmation";
            // 
            // outputLbl
            // 
            outputLbl.AutoSize = true;
            outputLbl.Location = new Point(122, 224);
            outputLbl.Name = "outputLbl";
            outputLbl.Size = new Size(78, 35);
            outputLbl.TabIndex = 11;
            outputLbl.Text = "Idle...";
            // 
            // statusLbl
            // 
            statusLbl.AutoSize = true;
            statusLbl.Location = new Point(6, 224);
            statusLbl.Name = "statusLbl";
            statusLbl.Size = new Size(110, 35);
            statusLbl.TabIndex = 10;
            statusLbl.Text = "Status ::";
            // 
            // PopupPassword
            // 
            AcceptButton = confirmPassBtn;
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonShadow;
            ClientSize = new Size(492, 295);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "PopupPassword";
            Text = "Confirm Password";
            FormClosing += PopupPassword_FormClosing;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label passLbl;
        private Button confirmPassBtn;
        public TextBox passTxt;
        private GroupBox groupBox1;
        private Label outputLbl;
        private Label statusLbl;
    }
}