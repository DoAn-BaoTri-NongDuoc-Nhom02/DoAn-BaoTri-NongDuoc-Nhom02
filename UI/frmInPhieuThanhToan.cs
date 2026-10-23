using System;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc
{
    /// <summary>
    /// PM-30: In phiếu thanh toán (phiếu thu) bằng PhieuIn — không ReportViewer.
    /// Entity: Id, NgayThanhToan, TongTien, KhachHang.HoTen, GhiChu
    /// </summary>
    public partial class frmInPhieuThanhToan : Form
    {
        PhieuThanhToan m_PhieuThanhToan;

        public frmInPhieuThanhToan(PhieuThanhToan ph)
        {
            InitializeComponent();
            m_PhieuThanhToan = ph;
        }

        private void frmPhieuThanhToan_Load(object sender, EventArgs e)
        {
            this.Text = "In Phiếu Thanh Toán";

            if (m_PhieuThanhToan == null)
            {
                MessageBox.Show("Không có dữ liệu phiếu thanh toán để in.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            string ma = m_PhieuThanhToan.Id ?? "";
            if (ma.Trim().Length == 0)
            {
                MessageBox.Show("Phiếu chưa có mã. Hãy lưu phiếu trước khi in.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;
            }

            string kh = "";
            if (m_PhieuThanhToan.KhachHang != null)
                kh = m_PhieuThanhToan.KhachHang.HoTen ?? "";

            string ngay = m_PhieuThanhToan.NgayThanhToan.ToString("dd/MM/yyyy");
            string soTien = m_PhieuThanhToan.TongTien.ToString("N0");
            string ghiChu = m_PhieuThanhToan.GhiChu ?? "";

            string cuaHang = "CỬA HÀNG NÔNG DƯỢC";
            try
            {
                CuaHang ch = ThamSo.LayCuaHang();
                if (ch != null && !string.IsNullOrEmpty(ch.TenCuaHang))
                    cuaHang = ch.TenCuaHang;
            }
            catch { }

            try
            {
                foreach (Control c in this.Controls)
                    c.Visible = false;
            }
            catch { }

            this.Hide();
            try
            {
                PhieuIn.Mo("PHIẾU THANH TOÁN (PHIẾU THU)", cuaHang, new string[] {
                    "Mã phiếu|" + ma,
                    "Khách hàng / Đại lý|" + kh,
                    "Ngày thu|" + ngay,
                    "Số tiền|" + soTien + " đ",
                    "Ghi chú|" + ghiChu
                }, this.Owner ?? this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không in được:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            this.Close();
        }
    }
}
