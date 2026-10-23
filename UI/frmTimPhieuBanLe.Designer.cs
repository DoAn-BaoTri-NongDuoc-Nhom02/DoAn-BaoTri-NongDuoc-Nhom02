namespace CuahangNongduoc
{
    partial class frmTimPhieuBanLe
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dtNgayNhap = new System.Windows.Forms.DateTimePicker();
            this.dtTuNgay = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.labelTu = new System.Windows.Forms.Label();
            this.cmbNCC = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnXoaDK = new System.Windows.Forms.Button();
            this.btnThemKH = new System.Windows.Forms.Button();
            this.lblHuongDan = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1 - Khách hàng
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.Text = "Khách hàng";
            // 
            // cmbNCC (giữ tên public — PM-106 ghi chú: tương thích chỗ gọi)
            // 
            this.cmbNCC.FormattingEnabled = true;
            this.cmbNCC.Location = new System.Drawing.Point(100, 16);
            this.cmbNCC.Name = "cmbNCC";
            this.cmbNCC.Size = new System.Drawing.Size(220, 21);
            this.cmbNCC.TabIndex = 0;
            // 
            // btnThemKH
            // 
            this.btnThemKH.Location = new System.Drawing.Point(326, 14);
            this.btnThemKH.Name = "btnThemKH";
            this.btnThemKH.Size = new System.Drawing.Size(32, 24);
            this.btnThemKH.TabIndex = 1;
            this.btnThemKH.Text = "+";
            this.btnThemKH.UseVisualStyleBackColor = true;
            this.btnThemKH.Click += new System.EventHandler(this.btnThemKH_Click);
            // 
            // labelTu
            // 
            this.labelTu.AutoSize = true;
            this.labelTu.Location = new System.Drawing.Point(16, 54);
            this.labelTu.Name = "labelTu";
            this.labelTu.Size = new System.Drawing.Size(49, 13);
            this.labelTu.Text = "Từ ngày";
            // 
            // dtTuNgay
            // 
            this.dtTuNgay.CustomFormat = "dd/MM/yyyy";
            this.dtTuNgay.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTuNgay.Location = new System.Drawing.Point(100, 50);
            this.dtTuNgay.Name = "dtTuNgay";
            this.dtTuNgay.Size = new System.Drawing.Size(110, 20);
            this.dtTuNgay.TabIndex = 2;
            // 
            // label2 - Đến ngày (dtNgayNhap = đến ngày, giữ tên public)
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(220, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.Text = "Đến ngày";
            // 
            // dtNgayNhap (= Đến ngày — tương thích code cũ)
            // 
            this.dtNgayNhap.CustomFormat = "dd/MM/yyyy";
            this.dtNgayNhap.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtNgayNhap.Location = new System.Drawing.Point(282, 50);
            this.dtNgayNhap.Name = "dtNgayNhap";
            this.dtNgayNhap.Size = new System.Drawing.Size(110, 20);
            this.dtNgayNhap.TabIndex = 3;
            // 
            // lblHuongDan
            // 
            this.lblHuongDan.Location = new System.Drawing.Point(16, 82);
            this.lblHuongDan.Name = "lblHuongDan";
            this.lblHuongDan.Size = new System.Drawing.Size(340, 32);
            this.lblHuongDan.Text = "Hướng dẫn: có thể để trống khách hàng để tìm theo khoảng ngày. Bấm «Xóa ĐK» để làm mới.";
            this.lblHuongDan.ForeColor = System.Drawing.Color.DimGray;
            // 
            // btnXoaDK
            // 
            this.btnXoaDK.Location = new System.Drawing.Point(16, 122);
            this.btnXoaDK.Name = "btnXoaDK";
            this.btnXoaDK.Size = new System.Drawing.Size(90, 32);
            this.btnXoaDK.TabIndex = 4;
            this.btnXoaDK.Text = "Xóa ĐK";
            this.btnXoaDK.UseVisualStyleBackColor = true;
            this.btnXoaDK.Click += new System.EventHandler(this.btnXoaDK_Click);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(180, 122);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 32);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "Tìm kiếm";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(276, 122);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 32);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Hủy bỏ";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // frmTimPhieuBanLe
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(380, 168);
            this.Controls.Add(this.lblHuongDan);
            this.Controls.Add(this.btnXoaDK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.dtNgayNhap);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtTuNgay);
            this.Controls.Add(this.labelTu);
            this.Controls.Add(this.btnThemKH);
            this.Controls.Add(this.cmbNCC);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTimPhieuBanLe";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tìm phiếu bán lẻ";
            this.Load += new System.EventHandler(this.frmTimPhieuBanLe_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnXoaDK;
        private System.Windows.Forms.Button btnThemKH;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelTu;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblHuongDan;
        public System.Windows.Forms.DateTimePicker dtNgayNhap;
        public System.Windows.Forms.DateTimePicker dtTuNgay;
        public System.Windows.Forms.ComboBox cmbNCC;
    }
}
