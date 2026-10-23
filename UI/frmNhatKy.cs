using System;
using System.Data;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    /// <summary>
    /// Xem nhật ký thay đổi khách hàng / đại lý (bảng NHAT_KY_KHACH_HANG)
    /// hoặc phiếu nhập (bảng NHAT_KY_PHIEU_NHAP).
    /// Mở từ nút "Nhật ký" của form Đại lý, Khách hàng và Nhập hàng.
    /// </summary>
    public partial class frmNhatKy : Form
    {
        public const string NGUON_KHACH_HANG = "KHACH_HANG";
        public const string NGUON_PHIEU_NHAP = "PHIEU_NHAP";

        private readonly CuahangNongduoc.Controller.KhachHangController ctrl = new CuahangNongduoc.Controller.KhachHangController();
        private readonly string m_Id;
        private readonly string m_Ten;
        private readonly string m_Nguon;

        // Mở độc lập (ví dụ từ menu): xem nhật ký của tất cả khách hàng / đại lý
        public frmNhatKy() : this("", "")
        {
        }

        public frmNhatKy(string id, string ten) : this(id, ten, NGUON_KHACH_HANG)
        {
        }

        public frmNhatKy(string id, string ten, string nguon)
        {
            InitializeComponent();
            m_Id = id == null ? "" : id;
            m_Ten = ten == null ? "" : ten;
            m_Nguon = (nguon == NGUON_PHIEU_NHAP) ? NGUON_PHIEU_NHAP : NGUON_KHACH_HANG;
        }

        private bool LaPhieuNhap
        {
            get { return m_Nguon == NGUON_PHIEU_NHAP; }
        }

        private void frmNhatKy_Load(object sender, EventArgs e)
        {
            UiTheme.Apply(this);
            dataGridView.AutoGenerateColumns = false;

            if (LaPhieuNhap)
            {
                this.Text = "Nhật ký thay đổi phiếu nhập";
                colIdKhach.HeaderText = "Mã phiếu";
                colTenKhach.HeaderText = "Nhà cung cấp";
                chkTatCa.Text = "Xem nhật ký của tất cả phiếu";
            }

            // Không có dòng nào được chọn thì xem nhật ký của tất cả
            if (m_Id.Length == 0)
            {
                chkTatCa.Checked = true;
                chkTatCa.Enabled = false;
            }
            TaiNhatKy();
        }

        private void chkTatCa_CheckedChanged(object sender, EventArgs e)
        {
            TaiNhatKy();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            TaiNhatKy();
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TaiNhatKy()
        {
            bool tatCa = chkTatCa.Checked || m_Id.Length == 0;
            string id = tatCa ? "" : m_Id;

            if (tatCa)
                lblTieuDe.Text = "Nhật ký thay đổi mới nhất (tối đa 500 dòng)";
            else if (LaPhieuNhap)
                lblTieuDe.Text = "Nhật ký của phiếu nhập " + m_Id + (m_Ten.Length > 0 ? " (" + m_Ten + ")" : "");
            else
                lblTieuDe.Text = "Nhật ký của: " + m_Ten + " (mã " + m_Id + ")";

            try
            {
                DataTable tbl = LaPhieuNhap
                    ? new CuahangNongduoc.Controller.PhieuNhapController().LayNhatKy(id)
                    : ctrl.LayNhatKy(id);
                dataGridView.DataSource = tbl;
                lblSoDong.Text = tbl.Rows.Count.ToString() + " dòng";
            }
            catch (Exception ex)
            {
                dataGridView.DataSource = null;
                lblSoDong.Text = "";
                string script = LaPhieuNhap ? "CapNhat_CSDL_PhieuNhap.sql" : "CapNhat_CSDL.sql";
                string bang = LaPhieuNhap ? "NHAT_KY_PHIEU_NHAP" : "NHAT_KY_KHACH_HANG";
                MessageBox.Show("Không đọc được nhật ký (đã chạy " + script + " để tạo bảng " + bang + " chưa?):\n" + ex.Message,
                    "Nhật ký thay đổi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}