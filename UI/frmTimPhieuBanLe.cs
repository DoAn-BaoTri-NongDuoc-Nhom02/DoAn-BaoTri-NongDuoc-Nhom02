using System;
using System.Windows.Forms;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    public partial class frmTimPhieuBanLe : Form
    {
        private bool m_LoaiDaiLy; // true = đại lý (bán sỉ), false = khách lẻ

        public frmTimPhieuBanLe()
        {
            InitializeComponent();
            m_LoaiDaiLy = false;
        }

        public frmTimPhieuBanLe(bool loai)
            : this()
        {
            m_LoaiDaiLy = loai;
            NapKhachHang();
            if (loai)
            {
                this.Text = "Tìm phiếu bán sỉ";
                label1.Text = "Đại lý";
                lblHuongDan.Text = "Hướng dẫn: có thể để trống đại lý để tìm theo khoảng ngày. Bấm «Xóa ĐK» để làm mới.";
            }
            else
            {
                this.Text = "Tìm phiếu bán lẻ";
                label1.Text = "Khách hàng";
            }
        }

        private void frmTimPhieuBanLe_Load(object sender, EventArgs e)
        {
            if (this.Text == null || this.Text.Length == 0)
                this.Text = "Tìm phiếu bán lẻ";

            // Mặc định khoảng 1 tháng gần đây
            try
            {
                dtTuNgay.Value = DateTime.Today.AddMonths(-1);
                dtNgayNhap.Value = DateTime.Today;
            }
            catch { }
        }

        void NapKhachHang()
        {
            try
            {
                KhachHangController ctrlKH = new KhachHangController();
                ctrlKH.HienthiAutoComboBox(cmbNCC, m_LoaiDaiLy);
                cmbNCC.SelectedIndex = -1; // cho phép trống
            }
            catch { }
        }

        private void btnXoaDK_Click(object sender, EventArgs e)
        {
            try
            {
                cmbNCC.SelectedIndex = -1;
                cmbNCC.Text = "";
                dtTuNgay.Value = DateTime.Today.AddMonths(-1);
                dtNgayNhap.Value = DateTime.Today;
            }
            catch { }
        }

        private void btnThemKH_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_LoaiDaiLy)
                {
                    frmDaiLy f = new frmDaiLy();
                    f.ShowDialog(this);
                }
                else
                {
                    // Form khách hàng nếu có; fallback Đại lý form không đúng → thử frmDaiLy không
                    try
                    {
                        Form f = (Form)Activator.CreateInstance(Type.GetType("CuahangNongduoc.frmKhachHang")
                            ?? Type.GetType("CuahangNongduoc.frmDaiLy"));
                        if (f != null) f.ShowDialog(this);
                    }
                    catch
                    {
                        frmDaiLy f = new frmDaiLy();
                        f.ShowDialog(this);
                    }
                }
                NapKhachHang();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không mở được form thêm:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>Ngày bắt đầu khoảng tìm.</summary>
        public DateTime TuNgay
        {
            get { return dtTuNgay.Value.Date; }
        }

        /// <summary>Ngày kết thúc (cũng là dtNgayNhap — tương thích code cũ).</summary>
        public DateTime DenNgay
        {
            get { return dtNgayNhap.Value.Date; }
        }
    }
}
