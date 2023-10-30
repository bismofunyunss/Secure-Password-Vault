namespace Secure_Password_Vault
{
    partial class Vault
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Vault));
            PassVault = new DataGridView();
            addRowBtn = new Button();
            deleteRowBtn = new Button();
            saveVaultBtn = new Button();
            vaultBox = new GroupBox();
            outputLbl = new Label();
            statusLbl = new Label();
            Description = new DataGridViewTextBoxColumn();
            Email = new DataGridViewTextBoxColumn();
            Username = new DataGridViewTextBoxColumn();
            Password = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)PassVault).BeginInit();
            vaultBox.SuspendLayout();
            SuspendLayout();
            // 
            // PassVault
            // 
            PassVault.AllowUserToAddRows = false;
            PassVault.AllowUserToResizeColumns = false;
            PassVault.AllowUserToResizeRows = false;
            PassVault.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            PassVault.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            PassVault.BackgroundColor = SystemColors.ButtonShadow;
            PassVault.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.ControlDarkDark;
            dataGridViewCellStyle1.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = Color.WhiteSmoke;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.ControlDarkDark;
            dataGridViewCellStyle1.SelectionForeColor = Color.WhiteSmoke;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            PassVault.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            PassVault.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            PassVault.Columns.AddRange(new DataGridViewColumn[] { Description, Email, Username, Password });
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = SystemColors.Window;
            dataGridViewCellStyle6.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle6.ForeColor = Color.WhiteSmoke;
            dataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            PassVault.DefaultCellStyle = dataGridViewCellStyle6;
            PassVault.EnableHeadersVisualStyles = false;
            PassVault.GridColor = Color.WhiteSmoke;
            PassVault.Location = new Point(6, 29);
            PassVault.MultiSelect = false;
            PassVault.Name = "PassVault";
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = SystemColors.ControlDarkDark;
            dataGridViewCellStyle7.Font = new Font("Segoe Script", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle7.ForeColor = Color.WhiteSmoke;
            dataGridViewCellStyle7.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = DataGridViewTriState.True;
            PassVault.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            PassVault.RowHeadersWidth = 62;
            PassVault.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            PassVault.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            PassVault.ShowCellErrors = false;
            PassVault.ShowCellToolTips = false;
            PassVault.ShowEditingIcon = false;
            PassVault.ShowRowErrors = false;
            PassVault.Size = new Size(918, 221);
            PassVault.TabIndex = 0;
            // 
            // addRowBtn
            // 
            addRowBtn.BackColor = SystemColors.ControlDarkDark;
            addRowBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            addRowBtn.FlatAppearance.BorderSize = 3;
            addRowBtn.FlatStyle = FlatStyle.Flat;
            addRowBtn.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            addRowBtn.ForeColor = Color.WhiteSmoke;
            addRowBtn.Location = new Point(6, 256);
            addRowBtn.Name = "addRowBtn";
            addRowBtn.Size = new Size(918, 44);
            addRowBtn.TabIndex = 5;
            addRowBtn.Text = "&Add New Row";
            addRowBtn.UseVisualStyleBackColor = false;
            addRowBtn.Click += addRowBtn_Click;
            // 
            // deleteRowBtn
            // 
            deleteRowBtn.BackColor = SystemColors.ControlDarkDark;
            deleteRowBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            deleteRowBtn.FlatAppearance.BorderSize = 3;
            deleteRowBtn.FlatStyle = FlatStyle.Flat;
            deleteRowBtn.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            deleteRowBtn.ForeColor = Color.WhiteSmoke;
            deleteRowBtn.Location = new Point(6, 306);
            deleteRowBtn.Name = "deleteRowBtn";
            deleteRowBtn.Size = new Size(918, 44);
            deleteRowBtn.TabIndex = 6;
            deleteRowBtn.Text = "&Delete Row";
            deleteRowBtn.UseVisualStyleBackColor = false;
            deleteRowBtn.Click += deleteRowBtn_Click;
            // 
            // saveVaultBtn
            // 
            saveVaultBtn.BackColor = SystemColors.ControlDarkDark;
            saveVaultBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            saveVaultBtn.FlatAppearance.BorderSize = 3;
            saveVaultBtn.FlatStyle = FlatStyle.Flat;
            saveVaultBtn.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            saveVaultBtn.ForeColor = Color.WhiteSmoke;
            saveVaultBtn.Location = new Point(6, 352);
            saveVaultBtn.Name = "saveVaultBtn";
            saveVaultBtn.Size = new Size(918, 44);
            saveVaultBtn.TabIndex = 7;
            saveVaultBtn.Text = "&Save Vault";
            saveVaultBtn.UseVisualStyleBackColor = false;
            saveVaultBtn.Click += saveVaultBtn_Click;
            // 
            // vaultBox
            // 
            vaultBox.Controls.Add(outputLbl);
            vaultBox.Controls.Add(statusLbl);
            vaultBox.Controls.Add(PassVault);
            vaultBox.Controls.Add(saveVaultBtn);
            vaultBox.Controls.Add(addRowBtn);
            vaultBox.Controls.Add(deleteRowBtn);
            vaultBox.Font = new Font("Segoe Script", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            vaultBox.ForeColor = Color.WhiteSmoke;
            vaultBox.Location = new Point(12, 12);
            vaultBox.Name = "vaultBox";
            vaultBox.Size = new Size(930, 473);
            vaultBox.TabIndex = 8;
            vaultBox.TabStop = false;
            vaultBox.Text = "Vault";
            // 
            // outputLbl
            // 
            outputLbl.AutoSize = true;
            outputLbl.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            outputLbl.Location = new Point(102, 431);
            outputLbl.Name = "outputLbl";
            outputLbl.Size = new Size(67, 25);
            outputLbl.TabIndex = 12;
            outputLbl.Text = "Idle...";
            // 
            // statusLbl
            // 
            statusLbl.AutoSize = true;
            statusLbl.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            statusLbl.Location = new Point(6, 431);
            statusLbl.Name = "statusLbl";
            statusLbl.Size = new Size(90, 25);
            statusLbl.TabIndex = 11;
            statusLbl.Text = "Status ::";
            // 
            // Description
            // 
            dataGridViewCellStyle2.BackColor = SystemColors.ControlDarkDark;
            dataGridViewCellStyle2.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = Color.Gold;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.ControlDarkDark;
            dataGridViewCellStyle2.SelectionForeColor = Color.Gold;
            Description.DefaultCellStyle = dataGridViewCellStyle2;
            Description.HeaderText = "Description";
            Description.MinimumWidth = 8;
            Description.Name = "Description";
            // 
            // Email
            // 
            dataGridViewCellStyle3.BackColor = SystemColors.ControlDarkDark;
            dataGridViewCellStyle3.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = Color.Gold;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.ControlDarkDark;
            dataGridViewCellStyle3.SelectionForeColor = Color.Gold;
            Email.DefaultCellStyle = dataGridViewCellStyle3;
            Email.HeaderText = "Email";
            Email.MinimumWidth = 8;
            Email.Name = "Email";
            // 
            // Username
            // 
            dataGridViewCellStyle4.BackColor = SystemColors.ControlDarkDark;
            dataGridViewCellStyle4.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle4.ForeColor = Color.Gold;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.ControlDarkDark;
            dataGridViewCellStyle4.SelectionForeColor = Color.Gold;
            Username.DefaultCellStyle = dataGridViewCellStyle4;
            Username.HeaderText = "Username";
            Username.MinimumWidth = 8;
            Username.Name = "Username";
            // 
            // Password
            // 
            dataGridViewCellStyle5.BackColor = SystemColors.ControlDarkDark;
            dataGridViewCellStyle5.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle5.ForeColor = Color.Gold;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.ControlDarkDark;
            dataGridViewCellStyle5.SelectionForeColor = Color.Gold;
            Password.DefaultCellStyle = dataGridViewCellStyle5;
            Password.HeaderText = "Password";
            Password.MinimumWidth = 8;
            Password.Name = "Password";
            // 
            // Vault
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(954, 511);
            Controls.Add(vaultBox);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Vault";
            Text = "Vault";
            ((System.ComponentModel.ISupportInitialize)PassVault).EndInit();
            vaultBox.ResumeLayout(false);
            vaultBox.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView PassVault;
        private Button addRowBtn;
        private Button deleteRowBtn;
        private Button saveVaultBtn;
        private GroupBox vaultBox;
        private Label outputLbl;
        private Label statusLbl;
        private DataGridViewTextBoxColumn Description;
        private DataGridViewTextBoxColumn Email;
        private DataGridViewTextBoxColumn Username;
        private DataGridViewTextBoxColumn Password;
    }
}