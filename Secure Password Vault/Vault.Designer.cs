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
            Description = new DataGridViewTextBoxColumn();
            Email = new DataGridViewTextBoxColumn();
            Username = new DataGridViewTextBoxColumn();
            Password = new DataGridViewTextBoxColumn();
            addRowBtn = new Button();
            deleteRowBtn = new Button();
            saveVaultBtn = new Button();
            vaultBox = new GroupBox();
            outputLbl = new Label();
            statusLbl = new Label();
            FileEncryptDecryptBox = new GroupBox();
            FileSizeNumLbl = new Label();
            FileSizeLbl = new Label();
            FileOutputLbl = new Label();
            FileStatusLbl = new Label();
            DecryptBtn = new Button();
            EncryptBtn = new Button();
            ExportFileBtn = new Button();
            ImportFileBtn = new Button();
            UserWelcomeLbl = new Label();
            hashbox = new GroupBox();
            filenamelbl = new Label();
            calculatehashbtn = new Button();
            hashoutputlbl = new Label();
            hashoutputtxt = new TextBox();
            hashimportfile = new Button();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox4 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)PassVault).BeginInit();
            vaultBox.SuspendLayout();
            FileEncryptDecryptBox.SuspendLayout();
            hashbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
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
            addRowBtn.MouseHover += addRowBtn_MouseHover;
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
            deleteRowBtn.MouseHover += deleteRowBtn_MouseHover;
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
            saveVaultBtn.MouseHover += saveVaultBtn_MouseHover;
            // 
            // vaultBox
            // 
            vaultBox.Controls.Add(pictureBox4);
            vaultBox.Controls.Add(outputLbl);
            vaultBox.Controls.Add(statusLbl);
            vaultBox.Controls.Add(PassVault);
            vaultBox.Controls.Add(saveVaultBtn);
            vaultBox.Controls.Add(addRowBtn);
            vaultBox.Controls.Add(deleteRowBtn);
            vaultBox.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
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
            // FileEncryptDecryptBox
            // 
            FileEncryptDecryptBox.Controls.Add(pictureBox3);
            FileEncryptDecryptBox.Controls.Add(FileSizeNumLbl);
            FileEncryptDecryptBox.Controls.Add(FileSizeLbl);
            FileEncryptDecryptBox.Controls.Add(FileOutputLbl);
            FileEncryptDecryptBox.Controls.Add(FileStatusLbl);
            FileEncryptDecryptBox.Controls.Add(DecryptBtn);
            FileEncryptDecryptBox.Controls.Add(EncryptBtn);
            FileEncryptDecryptBox.Controls.Add(ExportFileBtn);
            FileEncryptDecryptBox.Controls.Add(ImportFileBtn);
            FileEncryptDecryptBox.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            FileEncryptDecryptBox.ForeColor = Color.WhiteSmoke;
            FileEncryptDecryptBox.Location = new Point(12, 491);
            FileEncryptDecryptBox.Name = "FileEncryptDecryptBox";
            FileEncryptDecryptBox.Size = new Size(930, 249);
            FileEncryptDecryptBox.TabIndex = 10;
            FileEncryptDecryptBox.TabStop = false;
            FileEncryptDecryptBox.Text = "File Encryptor / Decryptor";
            // 
            // FileSizeNumLbl
            // 
            FileSizeNumLbl.AutoSize = true;
            FileSizeNumLbl.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            FileSizeNumLbl.Location = new Point(124, 204);
            FileSizeNumLbl.Name = "FileSizeNumLbl";
            FileSizeNumLbl.Size = new Size(23, 25);
            FileSizeNumLbl.TabIndex = 16;
            FileSizeNumLbl.Text = "0";
            // 
            // FileSizeLbl
            // 
            FileSizeLbl.AutoSize = true;
            FileSizeLbl.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            FileSizeLbl.Location = new Point(7, 204);
            FileSizeLbl.Name = "FileSizeLbl";
            FileSizeLbl.Size = new Size(111, 25);
            FileSizeLbl.TabIndex = 15;
            FileSizeLbl.Text = "File Size ::";
            // 
            // FileOutputLbl
            // 
            FileOutputLbl.AutoSize = true;
            FileOutputLbl.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            FileOutputLbl.Location = new Point(103, 179);
            FileOutputLbl.Name = "FileOutputLbl";
            FileOutputLbl.Size = new Size(67, 25);
            FileOutputLbl.TabIndex = 14;
            FileOutputLbl.Text = "Idle...";
            // 
            // FileStatusLbl
            // 
            FileStatusLbl.AutoSize = true;
            FileStatusLbl.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            FileStatusLbl.Location = new Point(7, 179);
            FileStatusLbl.Name = "FileStatusLbl";
            FileStatusLbl.Size = new Size(90, 25);
            FileStatusLbl.TabIndex = 13;
            FileStatusLbl.Text = "Status ::";
            // 
            // DecryptBtn
            // 
            DecryptBtn.BackColor = SystemColors.ControlDarkDark;
            DecryptBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            DecryptBtn.FlatAppearance.BorderSize = 3;
            DecryptBtn.FlatStyle = FlatStyle.Flat;
            DecryptBtn.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            DecryptBtn.ForeColor = Color.WhiteSmoke;
            DecryptBtn.Location = new Point(467, 82);
            DecryptBtn.Name = "DecryptBtn";
            DecryptBtn.Size = new Size(454, 44);
            DecryptBtn.TabIndex = 8;
            DecryptBtn.Text = "&Decrypt";
            DecryptBtn.UseVisualStyleBackColor = false;
            DecryptBtn.Click += DecryptBtn_Click;
            DecryptBtn.MouseHover += DecryptBtn_MouseHover;
            // 
            // EncryptBtn
            // 
            EncryptBtn.BackColor = SystemColors.ControlDarkDark;
            EncryptBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            EncryptBtn.FlatAppearance.BorderSize = 3;
            EncryptBtn.FlatStyle = FlatStyle.Flat;
            EncryptBtn.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            EncryptBtn.ForeColor = Color.WhiteSmoke;
            EncryptBtn.Location = new Point(467, 32);
            EncryptBtn.Name = "EncryptBtn";
            EncryptBtn.Size = new Size(454, 44);
            EncryptBtn.TabIndex = 7;
            EncryptBtn.Text = "&Encrypt";
            EncryptBtn.UseVisualStyleBackColor = false;
            EncryptBtn.Click += EncryptBtn_Click;
            EncryptBtn.MouseHover += EncryptBtn_MouseHover;
            // 
            // ExportFileBtn
            // 
            ExportFileBtn.BackColor = SystemColors.ControlDarkDark;
            ExportFileBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            ExportFileBtn.FlatAppearance.BorderSize = 3;
            ExportFileBtn.FlatStyle = FlatStyle.Flat;
            ExportFileBtn.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ExportFileBtn.ForeColor = Color.WhiteSmoke;
            ExportFileBtn.Location = new Point(6, 82);
            ExportFileBtn.Name = "ExportFileBtn";
            ExportFileBtn.Size = new Size(455, 44);
            ExportFileBtn.TabIndex = 6;
            ExportFileBtn.Text = "&Export File";
            ExportFileBtn.UseVisualStyleBackColor = false;
            ExportFileBtn.Click += ExportFileBtn_Click;
            ExportFileBtn.MouseHover += ExportFileBtn_MouseHover;
            // 
            // ImportFileBtn
            // 
            ImportFileBtn.BackColor = SystemColors.ControlDarkDark;
            ImportFileBtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            ImportFileBtn.FlatAppearance.BorderSize = 3;
            ImportFileBtn.FlatStyle = FlatStyle.Flat;
            ImportFileBtn.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ImportFileBtn.ForeColor = Color.WhiteSmoke;
            ImportFileBtn.Location = new Point(7, 32);
            ImportFileBtn.Name = "ImportFileBtn";
            ImportFileBtn.Size = new Size(454, 44);
            ImportFileBtn.TabIndex = 5;
            ImportFileBtn.Text = "&Import File";
            ImportFileBtn.UseVisualStyleBackColor = false;
            ImportFileBtn.Click += ImportFileBtn_Click;
            ImportFileBtn.MouseHover += ImportFileBtn_MouseHover;
            // 
            // UserWelcomeLbl
            // 
            UserWelcomeLbl.AutoSize = true;
            UserWelcomeLbl.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            UserWelcomeLbl.ForeColor = Color.WhiteSmoke;
            UserWelcomeLbl.Location = new Point(12, 1056);
            UserWelcomeLbl.Name = "UserWelcomeLbl";
            UserWelcomeLbl.Size = new Size(146, 25);
            UserWelcomeLbl.TabIndex = 17;
            UserWelcomeLbl.Text = "Welcome, null";
            // 
            // hashbox
            // 
            hashbox.Controls.Add(pictureBox2);
            hashbox.Controls.Add(filenamelbl);
            hashbox.Controls.Add(calculatehashbtn);
            hashbox.Controls.Add(hashoutputlbl);
            hashbox.Controls.Add(hashoutputtxt);
            hashbox.Controls.Add(hashimportfile);
            hashbox.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            hashbox.ForeColor = Color.WhiteSmoke;
            hashbox.Location = new Point(12, 746);
            hashbox.Name = "hashbox";
            hashbox.Size = new Size(930, 282);
            hashbox.TabIndex = 11;
            hashbox.TabStop = false;
            hashbox.Text = "File Hash Calculator";
            // 
            // filenamelbl
            // 
            filenamelbl.AutoSize = true;
            filenamelbl.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            filenamelbl.ForeColor = Color.WhiteSmoke;
            filenamelbl.Location = new Point(7, 241);
            filenamelbl.Name = "filenamelbl";
            filenamelbl.Size = new Size(159, 25);
            filenamelbl.TabIndex = 20;
            filenamelbl.Text = "File Name: N/A";
            // 
            // calculatehashbtn
            // 
            calculatehashbtn.BackColor = SystemColors.ControlDarkDark;
            calculatehashbtn.FlatAppearance.BorderColor = Color.WhiteSmoke;
            calculatehashbtn.FlatAppearance.BorderSize = 3;
            calculatehashbtn.FlatStyle = FlatStyle.Flat;
            calculatehashbtn.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            calculatehashbtn.ForeColor = Color.WhiteSmoke;
            calculatehashbtn.Location = new Point(7, 116);
            calculatehashbtn.Name = "calculatehashbtn";
            calculatehashbtn.Size = new Size(917, 44);
            calculatehashbtn.TabIndex = 19;
            calculatehashbtn.Text = "&Calculate Hash";
            calculatehashbtn.UseVisualStyleBackColor = false;
            calculatehashbtn.Click += calculatehashbtn_Click;
            calculatehashbtn.MouseHover += calculatehashbtn_MouseHover;
            // 
            // hashoutputlbl
            // 
            hashoutputlbl.AutoSize = true;
            hashoutputlbl.Location = new Point(6, 40);
            hashoutputlbl.Name = "hashoutputlbl";
            hashoutputlbl.Size = new Size(134, 25);
            hashoutputlbl.TabIndex = 18;
            hashoutputlbl.Text = "Hash Output";
            // 
            // hashoutputtxt
            // 
            hashoutputtxt.BackColor = Color.Black;
            hashoutputtxt.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            hashoutputtxt.ForeColor = Color.Gold;
            hashoutputtxt.Location = new Point(10, 77);
            hashoutputtxt.Name = "hashoutputtxt";
            hashoutputtxt.ReadOnly = true;
            hashoutputtxt.Size = new Size(914, 33);
            hashoutputtxt.TabIndex = 6;
            // 
            // hashimportfile
            // 
            hashimportfile.BackColor = SystemColors.ControlDarkDark;
            hashimportfile.FlatAppearance.BorderColor = Color.WhiteSmoke;
            hashimportfile.FlatAppearance.BorderSize = 3;
            hashimportfile.FlatStyle = FlatStyle.Flat;
            hashimportfile.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            hashimportfile.ForeColor = Color.WhiteSmoke;
            hashimportfile.Location = new Point(6, 165);
            hashimportfile.Name = "hashimportfile";
            hashimportfile.Size = new Size(918, 44);
            hashimportfile.TabIndex = 5;
            hashimportfile.Text = "&Import File";
            hashimportfile.UseVisualStyleBackColor = false;
            hashimportfile.Click += hashimportfile_Click;
            hashimportfile.MouseHover += hashimportfile_MouseHover;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.cryptography;
            pictureBox2.Location = new Point(855, 211);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(69, 65);
            pictureBox2.TabIndex = 21;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = Properties.Resources.cyber;
            pictureBox3.Location = new Point(854, 168);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(67, 75);
            pictureBox3.TabIndex = 17;
            pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = Properties.Resources.safe_box;
            pictureBox4.Location = new Point(862, 398);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(62, 69);
            pictureBox4.TabIndex = 13;
            pictureBox4.TabStop = false;
            // 
            // Vault
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(954, 1097);
            Controls.Add(UserWelcomeLbl);
            Controls.Add(hashbox);
            Controls.Add(FileEncryptDecryptBox);
            Controls.Add(vaultBox);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Vault";
            Text = "Vault";
            Load += Vault_Load;
            ((System.ComponentModel.ISupportInitialize)PassVault).EndInit();
            vaultBox.ResumeLayout(false);
            vaultBox.PerformLayout();
            FileEncryptDecryptBox.ResumeLayout(false);
            FileEncryptDecryptBox.PerformLayout();
            hashbox.ResumeLayout(false);
            hashbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
        private GroupBox FileEncryptDecryptBox;
        private Button ImportFileBtn;
        private Button EncryptBtn;
        private Button ExportFileBtn;
        private Button DecryptBtn;
        private Label FileOutputLbl;
        private Label FileStatusLbl;
        private Label FileSizeNumLbl;
        private Label FileSizeLbl;
        private Label UserWelcomeLbl;
        private GroupBox hashbox;
        private Button hashimportfile;
        private Label hashoutputlbl;
        private TextBox hashoutputtxt;
        private Label filenamelbl;
        private Button calculatehashbtn;
        private PictureBox pictureBox2;
        private PictureBox pictureBox4;
        private PictureBox pictureBox3;
    }
}