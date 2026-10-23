using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    /// <summary>
    /// PM-15: Form Tim Phieu Nhap – tieu de ro, nhieu tieu chi, nut Tim kiem / Xoa trang / Huy.
    /// </summary>
    public partial class frmTimPhieuNhap : Form
    {
        NhaCungCapController ctrlNCC = new NhaCungCapController();

        // Giu tuong thich code cu
        public ComboBox cmbNCC;
        public DateTimePicker dtNgayNhap;

        // Tieu chi moi
        private TextBox txtMaPhieu;
        private TextBox txtNguoiLap;
        private TextBox txtKhoNhap;
        private NumericUpDown numTongTu;
        private NumericUpDown numTongDen;
        private CheckBox chkBoQuaNgay;
        private CheckBox chkLocTongTien;
        private Label lblHuongDan;
        private Button btnOK;
        private Button btnXoaTrang;
        private Button btnCancel;

        /// <summary>True neu nguoi dung khong loc theo ngay.</summary>
        public bool BoQuaNgay
        {
            get { return chkBoQuaNgay != null && chkBoQuaNgay.Checked; }
        }

        public string MaPhieu
        {
            get { return txtMaPhieu != null ? txtMaPhieu.Text.Trim() : ""; }
        }

        public string NguoiLap
        {
            get { return txtNguoiLap != null ? txtNguoiLap.Text.Trim() : ""; }
        }

        public string KhoNhap
        {
            get { return txtKhoNhap != null ? txtKhoNhap.Text.Trim() : ""; }
        }

        public bool LocTongTien
        {
            get { return chkLocTongTien != null && chkLocTongTien.Checked; }
        }

        public decimal TongTienTu
        {
            get { return numTongTu != null ? numTongTu.Value : 0; }
        }

        public decimal TongTienDen
        {
            get { return numTongDen != null ? numTongDen.Value : 0; }
        }

        public string IdNhaCungCap
        {
            get
            {
                try
                {
                    if (cmbNCC != null && cmbNCC.SelectedValue != null)
                        return Convert.ToString(cmbNCC.SelectedValue);
                }
                catch { }
                return "";
            }
        }

        public frmTimPhieuNhap()
        {
            // Neu co Designer cu, van goi InitializeComponent neu partial co
            try { InitializeComponent(); } catch { }
            XayGiaoDienMoi();
        }

        private void XayGiaoDienMoi()
        {
            this.SuspendLayout();
            this.Controls.Clear();

            this.Text = "Tìm Phiếu Nhập – theo NCC, ngày, mã phiếu, tổng tiền…";
            this.ClientSize = new Size(420, 320);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9f);

            int xLbl = 16, xCtrl = 130, wCtrl = 250, y = 16, dy = 32;

            // Ma phieu
            AddLabel("Mã phiếu nhập:", xLbl, y);
            txtMaPhieu = AddTextBox(xCtrl, y - 2, wCtrl);
            y += dy;

            // NCC
            AddLabel("Nhà cung cấp:", xLbl, y);
            cmbNCC = new ComboBox
            {
                Location = new Point(xCtrl, y - 2),
                Width = wCtrl,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cmbNCC);
            y += dy;

            // Ngay
            AddLabel("Ngày nhập:", xLbl, y);
            dtNgayNhap = new DateTimePicker
            {
                Location = new Point(xCtrl, y - 2),
                Width = 140,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Value = DateTime.Now.Date
            };
            this.Controls.Add(dtNgayNhap);
            chkBoQuaNgay = new CheckBox
            {
                Text = "Bỏ qua ngày",
                AutoSize = true,
                Location = new Point(xCtrl + 150, y),
                Checked = false
            };
            chkBoQuaNgay.CheckedChanged += (s, e) => { dtNgayNhap.Enabled = !chkBoQuaNgay.Checked; };
            this.Controls.Add(chkBoQuaNgay);
            y += dy;

            // Nguoi lap
            AddLabel("Người lập:", xLbl, y);
            txtNguoiLap = AddTextBox(xCtrl, y - 2, wCtrl);
            y += dy;

            // Tong tien
            AddLabel("Tổng tiền:", xLbl, y);
            chkLocTongTien = new CheckBox
            {
                Text = "Lọc",
                AutoSize = true,
                Location = new Point(xCtrl, y),
                Checked = false
            };
            numTongTu = new NumericUpDown
            {
                Location = new Point(xCtrl + 50, y - 2),
                Width = 90,
                Maximum = 999999999999,
                ThousandsSeparator = true,
                Enabled = false
            };
            Label lDen = new Label { Text = "→", AutoSize = true, Location = new Point(xCtrl + 145, y) };
            numTongDen = new NumericUpDown
            {
                Location = new Point(xCtrl + 165, y - 2),
                Width = 90,
                Maximum = 999999999999,
                ThousandsSeparator = true,
                Value = 999999999,
                Enabled = false
            };
            chkLocTongTien.CheckedChanged += (s, e) =>
            {
                numTongTu.Enabled = chkLocTongTien.Checked;
                numTongDen.Enabled = chkLocTongTien.Checked;
            };
            this.Controls.Add(chkLocTongTien);
            this.Controls.Add(numTongTu);
            this.Controls.Add(lDen);
            this.Controls.Add(numTongDen);
            y += dy;

            // Kho nhap
            AddLabel("Kho nhập:", xLbl, y);
            txtKhoNhap = AddTextBox(xCtrl, y - 2, wCtrl);
            y += dy + 4;

            // Huong dan
            lblHuongDan = new Label
            {
                Text = "Hướng dẫn: chọn tiêu chí tìm kiếm rồi bấm Tìm kiếm. Có thể để trống các ô không cần lọc.",
                AutoSize = false,
                Size = new Size(380, 36),
                Location = new Point(16, y),
                ForeColor = Color.FromArgb(66, 66, 66),
                Font = new Font("Segoe UI", 8.25f, FontStyle.Italic)
            };
            this.Controls.Add(lblHuongDan);
            y += 42;

            // Buttons – gọn
            int btnY = y;
            btnOK = new Button
            {
                Text = "Tìm kiếm",
                Size = new Size(90, 28),
                Location = new Point(100, btnY),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.None
            };
            btnOK.Click += btnOK_Click;

            btnXoaTrang = new Button
            {
                Text = "Xóa trắng",
                Size = new Size(90, 28),
                Location = new Point(200, btnY)
            };
            btnXoaTrang.Click += btnXoaTrang_Click;

            btnCancel = new Button
            {
                Text = "Hủy bỏ",
                Size = new Size(90, 28),
                Location = new Point(300, btnY),
                DialogResult = DialogResult.Cancel
            };

            this.Controls.Add(btnOK);
            this.Controls.Add(btnXoaTrang);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
            this.ClientSize = new Size(420, btnY + 50);

            this.Load += frmTimPhieuNhap_Load;
            this.ResumeLayout(true);
        }

        private void AddLabel(string text, int x, int y)
        {
            Label lb = new Label
            {
                Text = text,
                AutoSize = true,
                Location = new Point(x, y),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lb);
        }

        private TextBox AddTextBox(int x, int y, int w)
        {
            TextBox t = new TextBox { Location = new Point(x, y), Width = w };
            this.Controls.Add(t);
            return t;
        }

        private void frmTimPhieuNhap_Load(object sender, EventArgs e)
        {
            try
            {
                ctrlNCC.HienthiAutoComboBox(cmbNCC);
                // Dong "tat ca"
                try
                {
                    if (cmbNCC.DataSource is DataTable dt)
                    {
                        DataTable copy = dt.Copy();
                        DataRow r = copy.NewRow();
                        // doan chuan: cot ID + ten
                        if (copy.Columns.Contains("ID"))
                            r["ID"] = "";
                        foreach (DataColumn c in copy.Columns)
                        {
                            if (c.ColumnName != "ID" && c.DataType == typeof(string))
                            {
                                r[c.ColumnName] = "-- Tất cả NCC --";
                                break;
                            }
                        }
                        copy.Rows.InsertAt(r, 0);
                        string disp = cmbNCC.DisplayMember;
                        string val = cmbNCC.ValueMember;
                        cmbNCC.DataSource = copy;
                        cmbNCC.DisplayMember = disp;
                        cmbNCC.ValueMember = val;
                        cmbNCC.SelectedIndex = 0;
                    }
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không nạp được danh sách nhà cung cấp:\n" + ex.Message,
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (LocTongTien && TongTienTu > TongTienDen)
            {
                MessageBox.Show("Tổng tiền từ phải ≤ tổng tiền đến.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnXoaTrang_Click(object sender, EventArgs e)
        {
            if (txtMaPhieu != null) txtMaPhieu.Text = "";
            if (txtNguoiLap != null) txtNguoiLap.Text = "";
            if (txtKhoNhap != null) txtKhoNhap.Text = "";
            if (chkBoQuaNgay != null) chkBoQuaNgay.Checked = true;
            if (dtNgayNhap != null) { dtNgayNhap.Value = DateTime.Now.Date; dtNgayNhap.Enabled = false; }
            if (chkLocTongTien != null) chkLocTongTien.Checked = false;
            if (numTongTu != null) numTongTu.Value = 0;
            if (numTongDen != null) numTongDen.Value = 999999999;
            try { if (cmbNCC != null && cmbNCC.Items.Count > 0) cmbNCC.SelectedIndex = 0; } catch { }
            if (txtMaPhieu != null) txtMaPhieu.Focus();
        }
    }
}
