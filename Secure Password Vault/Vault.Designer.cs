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
            PassVault = new DataGridView();
            Description = new DataGridViewTextBoxColumn();
            Email = new DataGridViewTextBoxColumn();
            Username = new DataGridViewTextBoxColumn();
            Password = new DataGridViewTextBoxColumn();
            addRowBtn = new Button();
            deleteRowBtn = new Button();
            saveVaultBtn = new Button();
            loadVaultBtn = new Button();
            ((System.ComponentModel.ISupportInitialize)PassVault).BeginInit();
            SuspendLayout();
            // 
            // PassVault
            // 
            PassVault.AllowUserToAddRows = false;
            PassVault.AllowUserToDeleteRows = false;
            PassVault.AllowUserToResizeColumns = false;
            PassVault.AllowUserToResizeRows = false;
            PassVault.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            PassVault.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            PassVault.BackgroundColor = Color.Black;
            PassVault.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.Black;
            dataGridViewCellStyle1.Font = new Font("Segoe Script", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = Color.DarkRed;
            dataGridViewCellStyle1.SelectionBackColor = Color.Black;
            dataGridViewCellStyle1.SelectionForeColor = Color.DarkRed;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            PassVault.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            PassVault.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            PassVault.Columns.AddRange(new DataGridViewColumn[] { Description, Email, Username, Password });
            PassVault.GridColor = Color.DarkRed;
            PassVault.Location = new Point(12, 12);
            PassVault.MultiSelect = false;
            PassVault.Name = "PassVault";
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = Color.Black;
            dataGridViewCellStyle6.Font = new Font("Segoe Script", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle6.ForeColor = Color.DarkRed;
            dataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.True;
            PassVault.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            PassVault.RowHeadersWidth = 62;
            PassVault.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            PassVault.SelectionMode = DataGridViewSelectionMode.CellSelect;
            PassVault.ShowCellErrors = false;
            PassVault.ShowCellToolTips = false;
            PassVault.ShowEditingIcon = false;
            PassVault.ShowRowErrors = false;
            PassVault.Size = new Size(664, 221);
            PassVault.TabIndex = 0;
            PassVault.CellPainting += PassVault_CellPainting;
            // 
            // Description
            // 
            dataGridViewCellStyle2.BackColor = Color.Black;
            dataGridViewCellStyle2.Font = new Font("Segoe Script", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = Color.DarkRed;
            dataGridViewCellStyle2.SelectionBackColor = Color.Black;
            dataGridViewCellStyle2.SelectionForeColor = Color.DarkRed;
            Description.DefaultCellStyle = dataGridViewCellStyle2;
            Description.HeaderText = "Description";
            Description.MinimumWidth = 8;
            Description.Name = "Description";
            // 
            // Email
            // 
            dataGridViewCellStyle3.BackColor = Color.Black;
            dataGridViewCellStyle3.Font = new Font("Segoe Script", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = Color.DarkRed;
            dataGridViewCellStyle3.SelectionBackColor = Color.Black;
            dataGridViewCellStyle3.SelectionForeColor = Color.DarkRed;
            Email.DefaultCellStyle = dataGridViewCellStyle3;
            Email.HeaderText = "Email";
            Email.MinimumWidth = 8;
            Email.Name = "Email";
            // 
            // Username
            // 
            dataGridViewCellStyle4.BackColor = Color.Black;
            dataGridViewCellStyle4.Font = new Font("Segoe Script", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle4.ForeColor = Color.DarkRed;
            dataGridViewCellStyle4.SelectionBackColor = Color.Black;
            dataGridViewCellStyle4.SelectionForeColor = Color.DarkRed;
            Username.DefaultCellStyle = dataGridViewCellStyle4;
            Username.HeaderText = "Username";
            Username.MinimumWidth = 8;
            Username.Name = "Username";
            // 
            // Password
            // 
            dataGridViewCellStyle5.BackColor = Color.Black;
            dataGridViewCellStyle5.Font = new Font("Segoe Script", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle5.ForeColor = Color.DarkRed;
            dataGridViewCellStyle5.SelectionBackColor = Color.Black;
            dataGridViewCellStyle5.SelectionForeColor = Color.DarkRed;
            Password.DefaultCellStyle = dataGridViewCellStyle5;
            Password.HeaderText = "Paassword";
            Password.MinimumWidth = 8;
            Password.Name = "Password";
            // 
            // addRowBtn
            // 
            addRowBtn.BackColor = Color.WhiteSmoke;
            addRowBtn.FlatAppearance.BorderColor = Color.Black;
            addRowBtn.FlatAppearance.BorderSize = 3;
            addRowBtn.FlatStyle = FlatStyle.Flat;
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
            deleteRowBtn.BackColor = Color.WhiteSmoke;
            deleteRowBtn.FlatAppearance.BorderColor = Color.Black;
            deleteRowBtn.FlatAppearance.BorderSize = 3;
            deleteRowBtn.FlatStyle = FlatStyle.Flat;
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
            saveVaultBtn.BackColor = Color.WhiteSmoke;
            saveVaultBtn.FlatAppearance.BorderColor = Color.Black;
            saveVaultBtn.FlatAppearance.BorderSize = 3;
            saveVaultBtn.FlatStyle = FlatStyle.Flat;
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
            loadVaultBtn.BackColor = Color.WhiteSmoke;
            loadVaultBtn.FlatAppearance.BorderColor = Color.Black;
            loadVaultBtn.FlatAppearance.BorderSize = 3;
            loadVaultBtn.FlatStyle = FlatStyle.Flat;
            loadVaultBtn.Location = new Point(12, 389);
            loadVaultBtn.Name = "loadVaultBtn";
            loadVaultBtn.Size = new Size(664, 44);
            loadVaultBtn.TabIndex = 8;
            loadVaultBtn.Text = "&Load Vault";
            loadVaultBtn.UseVisualStyleBackColor = false;
            loadVaultBtn.Click += loadVaultBtn_Click;
            // 
            // Vault
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(688, 449);
            Controls.Add(loadVaultBtn);
            Controls.Add(saveVaultBtn);
            Controls.Add(deleteRowBtn);
            Controls.Add(addRowBtn);
            Controls.Add(PassVault);
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