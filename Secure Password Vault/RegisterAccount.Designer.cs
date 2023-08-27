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
            groupBox1 = new GroupBox();
            outputLbl = new Label();
            statusLbl = new Label();
            cancelBtn = new Button();
            createAccountBtn = new Button();
            confirmPassTxt = new TextBox();
            confirmPassLbl = new Label();
            passTxt = new TextBox();
            passLbl = new Label();
            userTxt = new TextBox();
            userLbl = new Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(outputLbl);
            groupBox1.Controls.Add(statusLbl);
            groupBox1.Controls.Add(cancelBtn);
            groupBox1.Controls.Add(createAccountBtn);
            groupBox1.Controls.Add(confirmPassTxt);
            groupBox1.Controls.Add(confirmPassLbl);
            groupBox1.Controls.Add(passTxt);
            groupBox1.Controls.Add(passLbl);
            groupBox1.Controls.Add(userTxt);
            groupBox1.Controls.Add(userLbl);
            groupBox1.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox1.ForeColor = Color.WhiteSmoke;
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(450, 383);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Register Account";
            // 
            // outputLbl
            // 
            outputLbl.AutoSize = true;
            outputLbl.Location = new Point(97, 344);
            outputLbl.Name = "outputLbl";
            outputLbl.Size = new Size(64, 25);
            outputLbl.TabIndex = 9;
            outputLbl.Text = "Idle...";
            // 
            // statusLbl
            // 
            statusLbl.AutoSize = true;
            statusLbl.Location = new Point(6, 344);
            statusLbl.Name = "statusLbl";
            statusLbl.Size = new Size(85, 25);
            statusLbl.TabIndex = 8;
            statusLbl.Text = "Status ::";
            // 
            // cancelBtn
            // 
            cancelBtn.BackColor = Color.WhiteSmoke;
            cancelBtn.FlatAppearance.BorderColor = Color.Black;
            cancelBtn.FlatAppearance.BorderSize = 3;
            cancelBtn.FlatStyle = FlatStyle.Flat;
            cancelBtn.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cancelBtn.ForeColor = Color.Black;
            cancelBtn.Location = new Point(6, 270);
            cancelBtn.Name = "cancelBtn";
            cancelBtn.Size = new Size(438, 44);
            cancelBtn.TabIndex = 7;
            cancelBtn.Text = "&Cancel";
            cancelBtn.UseVisualStyleBackColor = false;
            // 
            // createAccountBtn
            // 
            createAccountBtn.BackColor = Color.WhiteSmoke;
            createAccountBtn.FlatAppearance.BorderColor = Color.Black;
            createAccountBtn.FlatAppearance.BorderSize = 3;
            createAccountBtn.FlatStyle = FlatStyle.Flat;
            createAccountBtn.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            createAccountBtn.ForeColor = Color.Black;
            createAccountBtn.Location = new Point(6, 220);
            createAccountBtn.Name = "createAccountBtn";
            createAccountBtn.Size = new Size(438, 44);
            createAccountBtn.TabIndex = 6;
            createAccountBtn.Text = "&Create Account";
            createAccountBtn.UseVisualStyleBackColor = false;
            createAccountBtn.Click += createAccountBtn_Click;
            // 
            // confirmPassTxt
            // 
            confirmPassTxt.Location = new Point(6, 182);
            confirmPassTxt.Name = "confirmPassTxt";
            confirmPassTxt.Size = new Size(438, 32);
            confirmPassTxt.TabIndex = 5;
            confirmPassTxt.UseSystemPasswordChar = true;
            // 
            // confirmPassLbl
            // 
            confirmPassLbl.AutoSize = true;
            confirmPassLbl.Location = new Point(6, 154);
            confirmPassLbl.Name = "confirmPassLbl";
            confirmPassLbl.Size = new Size(175, 25);
            confirmPassLbl.TabIndex = 4;
            confirmPassLbl.Text = "Confirm Password";
            // 
            // passTxt
            // 
            passTxt.Location = new Point(6, 119);
            passTxt.Name = "passTxt";
            passTxt.Size = new Size(438, 32);
            passTxt.TabIndex = 3;
            passTxt.UseSystemPasswordChar = true;
            // 
            // passLbl
            // 
            passLbl.AutoSize = true;
            passLbl.Location = new Point(6, 91);
            passLbl.Name = "passLbl";
            passLbl.Size = new Size(98, 25);
            passLbl.TabIndex = 2;
            passLbl.Text = "Password";
            // 
            // userTxt
            // 
            userTxt.Location = new Point(6, 56);
            userTxt.Name = "userTxt";
            userTxt.Size = new Size(438, 32);
            userTxt.TabIndex = 1;
            // 
            // userLbl
            // 
            userLbl.AutoSize = true;
            userLbl.Location = new Point(6, 28);
            userLbl.Name = "userLbl";
            userLbl.Size = new Size(100, 25);
            userLbl.TabIndex = 0;
            userLbl.Text = "Username";
            // 
            // RegisterAccount
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightSeaGreen;
            ClientSize = new Size(474, 407);
            Controls.Add(groupBox1);
            Name = "RegisterAccount";
            Text = "RegisterAccount";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private TextBox confirmPassTxt;
        private Label confirmPassLbl;
        private TextBox passTxt;
        private Label passLbl;
        private TextBox userTxt;
        private Label userLbl;
        private Button createAccountBtn;
        private Label outputLbl;
        private Label statusLbl;
        private Button cancelBtn;
    }
}