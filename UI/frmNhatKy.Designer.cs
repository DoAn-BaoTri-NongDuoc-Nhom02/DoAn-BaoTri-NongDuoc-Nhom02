namespace CuahangNongduoc
{
    partial class frmNhatKy
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNhatKy));
            this.panelTren = new System.Windows.Forms.Panel();
            this.chkTatCa = new System.Windows.Forms.CheckBox();
            this.lblTieuDe = new System.Windows.Forms.Label();
            this.panelDuoi = new System.Windows.Forms.Panel();
            this.lblSoDong = new System.Windows.Forms.Label();
            this.btnLamMoi = new System.Windows.Forms.Button();
            this.btnDong = new System.Windows.Forms.Button();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.colThoiGian = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNguoiDung = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHanhDong = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIdKhach = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTenKhach = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNoiDung = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelTren.SuspendLayout();
            this.panelDuoi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTren
            // 
            this.panelTren.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panelTren.Controls.Add(this.chkTatCa);
            this.panelTren.Controls.Add(this.lblTieuDe);
            this.panelTren.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTren.Location = new System.Drawing.Point(0, 0);
            this.panelTren.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelTren.Name = "panelTren";
            this.panelTren.Size = new System.Drawing.Size(748, 42);
            this.panelTren.TabIndex = 0;
            // 
            // chkTatCa
            // 
            this.chkTatCa.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkTatCa.AutoSize = true;
            this.chkTatCa.Location = new System.Drawing.Point(574, 11);
            this.chkTatCa.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkTatCa.Name = "chkTatCa";
            this.chkTatCa.Size = new System.Drawing.Size(161, 20);
            this.chkTatCa.TabIndex = 1;
            this.chkTatCa.Text = "Xem nhật ký của tất cả";
            this.chkTatCa.UseVisualStyleBackColor = true;
            this.chkTatCa.CheckedChanged += new System.EventHandler(this.chkTatCa_CheckedChanged);
            // 
            // lblTieuDe
            // 
            this.lblTieuDe.AutoSize = true;
            this.lblTieuDe.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblTieuDe.Location = new System.Drawing.Point(13, 12);
            this.lblTieuDe.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTieuDe.Name = "lblTieuDe";
            this.lblTieuDe.Size = new System.Drawing.Size(129, 18);
            this.lblTieuDe.TabIndex = 0;
            this.lblTieuDe.Text = "Nhật ký thay đổi";
            // 
            // panelDuoi
            // 
            this.panelDuoi.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panelDuoi.Controls.Add(this.lblSoDong);
            this.panelDuoi.Controls.Add(this.btnLamMoi);
            this.panelDuoi.Controls.Add(this.btnDong);
            this.panelDuoi.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelDuoi.Location = new System.Drawing.Point(0, 358);
            this.panelDuoi.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelDuoi.Name = "panelDuoi";
            this.panelDuoi.Size = new System.Drawing.Size(748, 49);
            this.panelDuoi.TabIndex = 2;
            // 
            // lblSoDong
            // 
            this.lblSoDong.AutoSize = true;
            this.lblSoDong.Location = new System.Drawing.Point(13, 17);
            this.lblSoDong.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSoDong.Name = "lblSoDong";
            this.lblSoDong.Size = new System.Drawing.Size(0, 16);
            this.lblSoDong.TabIndex = 0;
            // 
            // btnLamMoi
            // 
            this.btnLamMoi.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLamMoi.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLamMoi.Location = new System.Drawing.Point(494, 10);
            this.btnLamMoi.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLamMoi.Name = "btnLamMoi";
            this.btnLamMoi.Size = new System.Drawing.Size(107, 31);
            this.btnLamMoi.TabIndex = 1;
            this.btnLamMoi.Text = "Làm mới";
            this.btnLamMoi.UseVisualStyleBackColor = true;
            this.btnLamMoi.Click += new System.EventHandler(this.btnLamMoi_Click);
            // 
            // btnDong
            // 
            this.btnDong.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDong.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDong.Location = new System.Drawing.Point(621, 10);
            this.btnDong.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDong.Name = "btnDong";
            this.btnDong.Size = new System.Drawing.Size(107, 31);
            this.btnDong.TabIndex = 2;
            this.btnDong.Text = "Đóng";
            this.btnDong.UseVisualStyleBackColor = true;
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colThoiGian,
            this.colNguoiDung,
            this.colHanhDong,
            this.colIdKhach,
            this.colTenKhach,
            this.colNoiDung});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 42);
            this.dataGridView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowHeadersWidth = 51;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(748, 316);
            this.dataGridView.TabIndex = 1;
            // 
            // colThoiGian
            // 
            this.colThoiGian.DataPropertyName = "THOI_GIAN";
            dataGridViewCellStyle1.Format = "dd/MM/yyyy HH:mm:ss";
            this.colThoiGian.DefaultCellStyle = dataGridViewCellStyle1;
            this.colThoiGian.HeaderText = "Thời gian";
            this.colThoiGian.MinimumWidth = 6;
            this.colThoiGian.Name = "colThoiGian";
            this.colThoiGian.ReadOnly = true;
            this.colThoiGian.Width = 130;
            // 
            // colNguoiDung
            // 
            this.colNguoiDung.DataPropertyName = "NGUOI_DUNG";
            this.colNguoiDung.HeaderText = "Người thực hiện";
            this.colNguoiDung.MinimumWidth = 6;
            this.colNguoiDung.Name = "colNguoiDung";
            this.colNguoiDung.ReadOnly = true;
            this.colNguoiDung.Width = 110;
            // 
            // colHanhDong
            // 
            this.colHanhDong.DataPropertyName = "HANH_DONG";
            this.colHanhDong.HeaderText = "Hành động";
            this.colHanhDong.MinimumWidth = 6;
            this.colHanhDong.Name = "colHanhDong";
            this.colHanhDong.ReadOnly = true;
            this.colHanhDong.Width = 75;
            // 
            // colIdKhach
            // 
            this.colIdKhach.DataPropertyName = "ID_KHACH_HANG";
            this.colIdKhach.HeaderText = "Mã số";
            this.colIdKhach.MinimumWidth = 6;
            this.colIdKhach.Name = "colIdKhach";
            this.colIdKhach.ReadOnly = true;
            this.colIdKhach.Width = 70;
            // 
            // colTenKhach
            // 
            this.colTenKhach.DataPropertyName = "TEN_KHACH_HANG";
            this.colTenKhach.HeaderText = "Tên";
            this.colTenKhach.MinimumWidth = 6;
            this.colTenKhach.Name = "colTenKhach";
            this.colTenKhach.ReadOnly = true;
            this.colTenKhach.Width = 150;
            // 
            // colNoiDung
            // 
            this.colNoiDung.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colNoiDung.DataPropertyName = "NOI_DUNG";
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colNoiDung.DefaultCellStyle = dataGridViewCellStyle2;
            this.colNoiDung.HeaderText = "Nội dung thay đổi";
            this.colNoiDung.MinimumWidth = 6;
            this.colNoiDung.Name = "colNoiDung";
            this.colNoiDung.ReadOnly = true;
            // 
            // frmNhatKy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(748, 407);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.panelDuoi);
            this.Controls.Add(this.panelTren);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimizeBox = false;
            this.Name = "frmNhatKy";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nhật ký thay đổi";
            this.Load += new System.EventHandler(this.frmNhatKy_Load);
            this.panelTren.ResumeLayout(false);
            this.panelTren.PerformLayout();
            this.panelDuoi.ResumeLayout(false);
            this.panelDuoi.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTren;
        private System.Windows.Forms.Label lblTieuDe;
        private System.Windows.Forms.CheckBox chkTatCa;
        private System.Windows.Forms.Panel panelDuoi;
        private System.Windows.Forms.Label lblSoDong;
        private System.Windows.Forms.Button btnLamMoi;
        private System.Windows.Forms.Button btnDong;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn colThoiGian;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNguoiDung;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHanhDong;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIdKhach;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTenKhach;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNoiDung;
    }
}