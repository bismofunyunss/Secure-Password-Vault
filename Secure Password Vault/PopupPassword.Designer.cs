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
            passLbl = new Label();
            passTxt = new TextBox();
            confirmPassBtn = new Button();
            cancelBtn = new Button();
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
            passLbl.ForeColor = Color.DarkRed;
            passLbl.Location = new Point(6, 38);
            passLbl.Name = "passLbl";
            passLbl.Size = new Size(126, 37);
            passLbl.TabIndex = 3;
            passLbl.Text = "Password";
            // 
            // passTxt
            // 
            passTxt.BackColor = Color.Black;
            passTxt.Font = new Font("Segoe Script", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            passTxt.Location = new Point(6, 78);
            passTxt.Name = "passTxt";
            passTxt.Size = new Size(394, 36);
            passTxt.TabIndex = 4;
            passTxt.UseSystemPasswordChar = true;
            // 
            // confirmPassBtn
            // 
            confirmPassBtn.BackColor = Color.Black;
            confirmPassBtn.FlatAppearance.BorderColor = Color.DarkRed;
            confirmPassBtn.FlatAppearance.BorderSize = 3;
            confirmPassBtn.FlatStyle = FlatStyle.Flat;
            confirmPassBtn.Location = new Point(6, 120);
            confirmPassBtn.Name = "confirmPassBtn";
            confirmPassBtn.Size = new Size(394, 44);
            confirmPassBtn.TabIndex = 6;
            confirmPassBtn.Text = "&Confirm Password";
            confirmPassBtn.UseVisualStyleBackColor = false;
            confirmPassBtn.Click += confirmPassBtn_ClickAsync;
            // 
            // cancelBtn
            // 
            cancelBtn.BackColor = Color.Black;
            cancelBtn.FlatAppearance.BorderColor = Color.DarkRed;
            cancelBtn.FlatAppearance.BorderSize = 3;
            cancelBtn.FlatStyle = FlatStyle.Flat;
            cancelBtn.Location = new Point(6, 170);
            cancelBtn.Name = "cancelBtn";
            cancelBtn.Size = new Size(394, 44);
            cancelBtn.TabIndex = 7;
            cancelBtn.Text = "&Cancel";
            cancelBtn.UseVisualStyleBackColor = false;
            cancelBtn.Click += cancelBtn_Click;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.Black;
            groupBox1.Controls.Add(outputLbl);
            groupBox1.Controls.Add(statusLbl);
            groupBox1.Controls.Add(passLbl);
            groupBox1.Controls.Add(cancelBtn);
            groupBox1.Controls.Add(passTxt);
            groupBox1.Controls.Add(confirmPassBtn);
            groupBox1.Font = new Font("Segoe Script", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBox1.ForeColor = Color.DarkRed;
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(406, 297);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Password Confirmation";
            // 
            // outputLbl
            // 
            outputLbl.AutoSize = true;
            outputLbl.Location = new Point(122, 245);
            outputLbl.Name = "outputLbl";
            outputLbl.Size = new Size(78, 35);
            outputLbl.TabIndex = 11;
            outputLbl.Text = "Idle...";
            // 
            // statusLbl
            // 
            statusLbl.AutoSize = true;
            statusLbl.Location = new Point(6, 245);
            statusLbl.Name = "statusLbl";
            statusLbl.Size = new Size(110, 35);
            statusLbl.TabIndex = 10;
            statusLbl.Text = "Status ::";
            // 
            // PopupPassword
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(430, 317);
            Controls.Add(groupBox1);
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
        private Button cancelBtn;
        private GroupBox groupBox1;
        private Label outputLbl;
        private Label statusLbl;
    }
}