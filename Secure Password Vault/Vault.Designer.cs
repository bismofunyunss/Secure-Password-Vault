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
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Vault));
            PassVault = new DataGridView();
            addRowBtn = new Button();
            deleteRowBtn = new Button();
            saveVaultBtn = new Button();
            loadVaultBtn = new Button();
            Description = new DataGridViewTextBoxColumn();
            Email = new DataGridViewTextBoxColumn();
            Username = new DataGridViewTextBoxColumn();
            Password = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)PassVault).BeginInit();
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
            dataGridViewCellStyle1.Font = new Font("Segoe Script", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = Color.WhiteSmoke;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.ControlDarkDark;
            dataGridViewCellStyle1.SelectionForeColor = Color.WhiteSmoke;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            PassVault.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            PassVault.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            PassVault.Columns.AddRange(new DataGridViewColumn[] { Description, Email, Username, Password });
            PassVault.GridColor = Color.WhiteSmoke;
            PassVault.Location = new Point(12, 12);
            PassVault.MultiSelect = false;
            PassVault.Name = "PassVault";
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = SystemColors.ControlDarkDark;
            dataGridViewCellStyle6.Font = new Font("Segoe Script", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle6.ForeColor = Color.WhiteSmoke;
            dataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.True;
            PassVault.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            PassVault.RowHeadersWidth = 62;
            PassVault.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            PassVault.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            PassVault.ShowCellErrors = false;
            PassVault.ShowCellToolTips = false;
            PassVault.ShowEditingIcon = false;
            PassVault.ShowRowErrors = false;
            PassVault.Size = new Size(664, 221);
            PassVault.TabIndex = 0;
            // 
            // addRowBtn
            // 
            addRowBtn.BackColor = SystemColors.ButtonShadow;
            addRowBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            addRowBtn.FlatAppearance.BorderSize = 3;
            addRowBtn.FlatStyle = FlatStyle.Flat;
            addRowBtn.Font = new Font("Segoe Script", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            addRowBtn.ForeColor = Color.WhiteSmoke;
            addRowBtn.Location = new Point(12, 239);
            addRowBtn.Name = "addRowBtn";
            addRowBtn.Size = new Size(664, 44);
            addRowBtn.TabIndex = 5;
            addRowBtn.Text = "&Add New Row";
            addRowBtn.UseVisualStyleBackColor = false;
            addRowBtn.Click += addRowBtn_Click;
            // 
            // deleteRowBtn
            // 
            deleteRowBtn.BackColor = SystemColors.ButtonShadow;
            deleteRowBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            deleteRowBtn.FlatAppearance.BorderSize = 3;
            deleteRowBtn.FlatStyle = FlatStyle.Flat;
            deleteRowBtn.Font = new Font("Segoe Script", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            deleteRowBtn.ForeColor = Color.WhiteSmoke;
            deleteRowBtn.Location = new Point(12, 289);
            deleteRowBtn.Name = "deleteRowBtn";
            deleteRowBtn.Size = new Size(664, 44);
            deleteRowBtn.TabIndex = 6;
            deleteRowBtn.Text = "&Delete Row";
            deleteRowBtn.UseVisualStyleBackColor = false;
            deleteRowBtn.Click += deleteRowBtn_Click;
            // 
            // saveVaultBtn
            // 
            saveVaultBtn.BackColor = SystemColors.ButtonShadow;
            saveVaultBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            saveVaultBtn.FlatAppearance.BorderSize = 3;
            saveVaultBtn.FlatStyle = FlatStyle.Flat;
            saveVaultBtn.Font = new Font("Segoe Script", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            saveVaultBtn.ForeColor = Color.WhiteSmoke;
            saveVaultBtn.Location = new Point(12, 339);
            saveVaultBtn.Name = "saveVaultBtn";
            saveVaultBtn.Size = new Size(664, 44);
            saveVaultBtn.TabIndex = 7;
            saveVaultBtn.Text = "&Save Vault";
            saveVaultBtn.UseVisualStyleBackColor = false;
            saveVaultBtn.Click += saveVaultBtn_Click;
            // 
            // loadVaultBtn
            // 
            loadVaultBtn.BackColor = SystemColors.ButtonShadow;
            loadVaultBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            loadVaultBtn.FlatAppearance.BorderSize = 3;
            loadVaultBtn.FlatStyle = FlatStyle.Flat;
            loadVaultBtn.Font = new Font("Segoe Script", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            loadVaultBtn.ForeColor = Color.WhiteSmoke;
            loadVaultBtn.Location = new Point(12, 389);
            loadVaultBtn.Name = "loadVaultBtn";
            loadVaultBtn.Size = new Size(664, 44);
            loadVaultBtn.TabIndex = 8;
            loadVaultBtn.Text = "&Load Vault";
            loadVaultBtn.UseVisualStyleBackColor = false;
            loadVaultBtn.Click += loadVaultBtn_Click;
            // 
            // Description
            // 
            dataGridViewCellStyle2.BackColor = SystemColors.ControlDark;
            dataGridViewCellStyle2.Font = new Font("Segoe Script", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = Color.Gold;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.ButtonShadow;
            dataGridViewCellStyle2.SelectionForeColor = Color.WhiteSmoke;
            Description.DefaultCellStyle = dataGridViewCellStyle2;
            Description.HeaderText = "Description";
            Description.MinimumWidth = 8;
            Description.Name = "Description";
            // 
            // Email
            // 
            dataGridViewCellStyle3.BackColor = SystemColors.ButtonShadow;
            dataGridViewCellStyle3.Font = new Font("Segoe Script", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = Color.Gold;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.ButtonShadow;
            dataGridViewCellStyle3.SelectionForeColor = Color.WhiteSmoke;
            Email.DefaultCellStyle = dataGridViewCellStyle3;
            Email.HeaderText = "Email";
            Email.MinimumWidth = 8;
            Email.Name = "Email";
            // 
            // Username
            // 
            dataGridViewCellStyle4.BackColor = SystemColors.ButtonShadow;
            dataGridViewCellStyle4.Font = new Font("Segoe Script", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle4.ForeColor = Color.Gold;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.ButtonShadow;
            dataGridViewCellStyle4.SelectionForeColor = Color.WhiteSmoke;
            Username.DefaultCellStyle = dataGridViewCellStyle4;
            Username.HeaderText = "Username";
            Username.MinimumWidth = 8;
            Username.Name = "Username";
            // 
            // Password
            // 
            dataGridViewCellStyle5.BackColor = SystemColors.ButtonShadow;
            dataGridViewCellStyle5.Font = new Font("Segoe Script", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle5.ForeColor = Color.Gold;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.ControlDark;
            dataGridViewCellStyle5.SelectionForeColor = Color.WhiteSmoke;
            Password.DefaultCellStyle = dataGridViewCellStyle5;
            Password.HeaderText = "Password";
            Password.MinimumWidth = 8;
            Password.Name = "Password";
            // 
            // Vault
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonShadow;
            ClientSize = new Size(688, 449);
            Controls.Add(loadVaultBtn);
            Controls.Add(saveVaultBtn);
            Controls.Add(deleteRowBtn);
            Controls.Add(addRowBtn);
            Controls.Add(PassVault);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Vault";
            Text = "Vault";
            ((System.ComponentModel.ISupportInitialize)PassVault).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView PassVault;
        private Button addRowBtn;
        private Button deleteRowBtn;
        private Button saveVaultBtn;
        private Button loadVaultBtn;
        private DataGridViewTextBoxColumn Description;
        private DataGridViewTextBoxColumn Email;
        private DataGridViewTextBoxColumn Username;
        private DataGridViewTextBoxColumn Password;
    }
}