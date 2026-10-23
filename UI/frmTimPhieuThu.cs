using System;
using System.Drawing;
using System.Windows.Forms;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    /// <summary>
    /// PM-28: Tim phieu thu — them tieu chi, Xoa trang, nhan ro, giong design Nhap hang.
    /// Giữ cmbKhachHang, dtNgayThu public cho frmThanhToan.
    /// </summary>
    public partial class frmTimPhieuThu : Form
    {
        public TextBox txtMaPhieu;
        public TextBox txtNguoiLap;
        public TextBox txtSoTienTu;
        public TextBox txtSoTienDen;
        public ComboBox cboHinhThuc;
        public CheckBox chkTheoNgay;

        Button btnXoaTrang;
        Label lblHuongDan;

        public frmTimPhieuThu()
        {
            InitializeComponent();
        }

        private void frmTimPhieuThu_Load(object sender, EventArgs e)
        {
            this.Text = "Tìm kiếm / Lọc phiếu thu";
            this.BackColor = Color.FromArgb(192, 255, 192);

            try { label1.Text = "Khách / Đại lý"; } catch { }
            try { label2.Text = "Ngày thu"; } catch { }

            try
            {
                btnOK.Text = "Tìm kiếm";
                btnOK.Size = new Size(90, 32);
                btnCancel.Text = "Hủy";
                btnCancel.Size = new Size(90, 32);
            }
            catch { }

            BoSungTieuChiVaNut();

            try
            {
                KhachHangController ctrl = new KhachHangController();
                ctrl.HienthiChungAutoComboBox(cmbKhachHang);
            }
            catch { }
            try
            {
                cmbKhachHang.DropDownStyle = ComboBoxStyle.DropDown;
            }
            catch { }
        }

        void BoSungTieuChiVaNut()
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            int y = 78;
            int leftL = 12;
            int leftC = 110;
            int w = 180;

            this.Controls.Add(new Label { Text = "Mã phiếu thu", Left = leftL, Top = y + 3, AutoSize = true });
            txtMaPhieu = new TextBox { Left = leftC, Top = y, Width = w };
            this.Controls.Add(txtMaPhieu);
            y += 28;

            this.Controls.Add(new Label { Text = "Người lập", Left = leftL, Top = y + 3, AutoSize = true });
            txtNguoiLap = new TextBox { Left = leftC, Top = y, Width = w };
            this.Controls.Add(txtNguoiLap);
            y += 28;

            this.Controls.Add(new Label { Text = "Số tiền từ", Left = leftL, Top = y + 3, AutoSize = true });
            txtSoTienTu = new TextBox { Left = leftC, Top = y, Width = 80 };
            this.Controls.Add(txtSoTienTu);
            this.Controls.Add(new Label { Text = "đến", Left = leftC + 85, Top = y + 3, AutoSize = true });
            txtSoTienDen = new TextBox { Left = leftC + 110, Top = y, Width = 70 };
            this.Controls.Add(txtSoTienDen);
            y += 28;

            this.Controls.Add(new Label { Text = "Hình thức TT", Left = leftL, Top = y + 3, AutoSize = true });
            cboHinhThuc = new ComboBox
            {
                Left = leftC,
                Top = y,
                Width = w,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboHinhThuc.Items.AddRange(new object[] { "(Tất cả)", "Tiền mặt", "Chuyển khoản" });
            cboHinhThuc.SelectedIndex = 0;
            this.Controls.Add(cboHinhThuc);
            y += 28;

            chkTheoNgay = new CheckBox
            {
                Text = "Lọc theo ngày thu",
                Left = leftL,
                Top = y,
                AutoSize = true,
                Checked = true
            };
            this.Controls.Add(chkTheoNgay);
            y += 26;

            lblHuongDan = new Label
            {
                Text = "Để trống tiêu chí = không lọc theo mục đó.",
                Left = leftL,
                Top = y,
                AutoSize = true,
                ForeColor = Color.DimGray
            };
            this.Controls.Add(lblHuongDan);
            y += 28;

            btnOK.Location = new Point(20, y);
            btnOK.Size = new Size(90, 32);

            btnXoaTrang = new Button();
            btnXoaTrang.Text = "Xóa trắng";
            btnXoaTrang.Size = new Size(90, 32);
            btnXoaTrang.Location = new Point(115, y);
            btnXoaTrang.Click += btnXoaTrang_Click;
            this.Controls.Add(btnXoaTrang);

            btnCancel.Location = new Point(210, y);
            btnCancel.Size = new Size(90, 32);

            this.ClientSize = new Size(320, y + 50);
        }

        void btnXoaTrang_Click(object sender, EventArgs e)
        {
            try
            {
                cmbKhachHang.SelectedIndex = -1;
                cmbKhachHang.Text = "";
                dtNgayThu.Value = DateTime.Today;
                if (chkTheoNgay != null) chkTheoNgay.Checked = false;
                if (txtMaPhieu != null) txtMaPhieu.Text = "";
                if (txtNguoiLap != null) txtNguoiLap.Text = "";
                if (txtSoTienTu != null) txtSoTienTu.Text = "";
                if (txtSoTienDen != null) txtSoTienDen.Text = "";
                if (cboHinhThuc != null) cboHinhThuc.SelectedIndex = 0;
            }
            catch { }
        }
    }
}
