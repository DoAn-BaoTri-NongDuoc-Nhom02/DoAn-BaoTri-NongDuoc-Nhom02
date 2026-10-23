using System;
using System.Drawing;
using System.Windows.Forms;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    /// <summary>
    /// PM-27: Form tìm phiếu chi — tiêu đề rõ, Ngày chi, thêm tiêu chí, Xóa trắng.
    /// Giữ control công khai cmbLyDo, dtNgayChi để frmPhieuChi dùng như cũ.
    /// </summary>
    public partial class frmTimPhieuChi : Form
    {
        // Tieu chi bo sung (public de frmPhieuChi doc neu can)
        public TextBox txtMaPhieu;
        public TextBox txtNguoiLap;
        public TextBox txtSoTienTu;
        public TextBox txtSoTienDen;
        public TextBox txtNguoiNhan;
        public CheckBox chkTheoNgay;

        Button btnXoaTrang;
        Label lblHuongDan;

        public frmTimPhieuChi()
        {
            InitializeComponent();
        }

        private void frmTimPhieuChi_Load(object sender, EventArgs e)
        {
            // 1. Tieu de
            this.Text = "Tìm kiếm / Lọc phiếu chi";
            this.BackColor = Color.FromArgb(192, 255, 192);

            // 4. Ngay mua -> Ngay chi
            try { label2.Text = "Ngày chi"; } catch { }

            // 6. Nut Tim kiem (san co), dong bo kich thuoc
            try
            {
                btnOK.Text = "Tìm kiếm";
                btnOK.Size = new Size(90, 32);
                btnCancel.Text = "Hủy";
                btnCancel.Size = new Size(90, 32);
            }
            catch { }

            BoSungTieuChiVaNut();

            // 5. Load ly do + dong "Tat ca"
            try
            {
                LyDoChiController ctrl = new LyDoChiController();
                ctrl.HienthiAutoComboBox(cmbLyDo);
            }
            catch { }
            try
            {
                // Cho phep de trong = khong loc theo ly do
                cmbLyDo.DropDownStyle = ComboBoxStyle.DropDown;
                if (string.IsNullOrEmpty(cmbLyDo.Text))
                    cmbLyDo.Text = "";
            }
            catch { }
        }

        void BoSungTieuChiVaNut()
        {
            // Mo rong form mot chut
            this.ClientSize = new Size(320, 280);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            int y = 78;
            int leftL = 18;
            int leftC = 100;
            int w = 190;

            // Ma phieu
            this.Controls.Add(new Label { Text = "Mã phiếu", Left = leftL, Top = y + 3, AutoSize = true });
            txtMaPhieu = new TextBox { Left = leftC, Top = y, Width = w };
            this.Controls.Add(txtMaPhieu);
            y += 28;

            // Nguoi lap
            this.Controls.Add(new Label { Text = "Người lập", Left = leftL, Top = y + 3, AutoSize = true });
            txtNguoiLap = new TextBox { Left = leftC, Top = y, Width = w };
            this.Controls.Add(txtNguoiLap);
            y += 28;

            // So tien tu - den
            this.Controls.Add(new Label { Text = "Số tiền từ", Left = leftL, Top = y + 3, AutoSize = true });
            txtSoTienTu = new TextBox { Left = leftC, Top = y, Width = 85 };
            this.Controls.Add(txtSoTienTu);
            this.Controls.Add(new Label { Text = "đến", Left = leftC + 90, Top = y + 3, AutoSize = true });
            txtSoTienDen = new TextBox { Left = leftC + 115, Top = y, Width = 75 };
            this.Controls.Add(txtSoTienDen);
            y += 28;

            // Nguoi nhan
            this.Controls.Add(new Label { Text = "Người nhận", Left = leftL, Top = y + 3, AutoSize = true });
            txtNguoiNhan = new TextBox { Left = leftC, Top = y, Width = w };
            this.Controls.Add(txtNguoiNhan);
            y += 28;

            // Loc theo ngay (tuy chon)
            chkTheoNgay = new CheckBox
            {
                Text = "Lọc theo ngày chi",
                Left = leftL,
                Top = y,
                AutoSize = true,
                Checked = true
            };
            this.Controls.Add(chkTheoNgay);
            y += 26;

            // Huong dan
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

            // Nut: Tim kiem | Xoa trang | Huy
            int by = y;
            btnOK.Location = new Point(20, by);
            btnOK.Size = new Size(90, 32);

            btnXoaTrang = new Button();
            btnXoaTrang.Text = "Xóa trắng";
            btnXoaTrang.Size = new Size(90, 32);
            btnXoaTrang.Location = new Point(115, by);
            btnXoaTrang.Click += btnXoaTrang_Click;
            this.Controls.Add(btnXoaTrang);

            btnCancel.Location = new Point(210, by);
            btnCancel.Size = new Size(90, 32);

            this.ClientSize = new Size(320, by + 50);
        }

        void btnXoaTrang_Click(object sender, EventArgs e)
        {
            try
            {
                cmbLyDo.SelectedIndex = -1;
                cmbLyDo.Text = "";
                dtNgayChi.Value = DateTime.Today;
                if (chkTheoNgay != null) chkTheoNgay.Checked = false;
                if (txtMaPhieu != null) txtMaPhieu.Text = "";
                if (txtNguoiLap != null) txtNguoiLap.Text = "";
                if (txtSoTienTu != null) txtSoTienTu.Text = "";
                if (txtSoTienDen != null) txtSoTienDen.Text = "";
                if (txtNguoiNhan != null) txtNguoiNhan.Text = "";
            }
            catch { }
        }
    }
}
