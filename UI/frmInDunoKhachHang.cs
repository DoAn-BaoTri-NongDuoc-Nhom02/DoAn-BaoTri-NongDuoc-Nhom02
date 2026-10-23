using System;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc
{
    /// <summary>
    /// PM-32: In dư nợ KH bằng PhieuIn — không ReportViewer, có cảnh báo.
    /// </summary>
    public partial class frmInDunoKhachHang : Form
    {
        DuNoKhachHang m_DuNo;

        public frmInDunoKhachHang(DuNoKhachHang dn)
        {
            m_DuNo = dn;
            InitializeComponent();
        }

        private void frmInDunoKhachHang_Load(object sender, EventArgs e)
        {
            this.Text = "In dư nợ khách hàng";

            if (m_DuNo == null)
            {
                MessageBox.Show("Không có dữ liệu để hiển thị.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            string maKH = "";
            string tenKH = "";
            string diaChi = "";
            string dienThoai = "";
            try
            {
                if (m_DuNo.KhachHang != null)
                {
                    maKH = m_DuNo.KhachHang.Id ?? "";
                    tenKH = m_DuNo.KhachHang.HoTen ?? "";
                    diaChi = m_DuNo.KhachHang.DiaChi ?? "";
                    try { dienThoai = m_DuNo.KhachHang.DienThoai ?? ""; } catch { }
                }
            }
            catch { }

            if (string.IsNullOrEmpty(tenKH) && string.IsNullOrEmpty(maKH))
            {
                MessageBox.Show("Không có dữ liệu để hiển thị.\nChọn một dòng dư nợ có khách hàng rồi in lại.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;
            }

            string cuaHang = "CỬA HÀNG NÔNG DƯỢC";
            try
            {
                CuaHang ch = ThamSo.LayCuaHang();
                if (ch != null && !string.IsNullOrEmpty(ch.TenCuaHang) && ch.TenCuaHang.IndexOf('?') < 0)
                    cuaHang = ch.TenCuaHang.ToUpper();
            }
            catch { }

            string thang = m_DuNo.Thang.ToString() + "/" + m_DuNo.Nam.ToString();
            string tieuDe = "DƯ NỢ KHÁCH HÀNG";
            if (tenKH.Length > 0)
                tieuDe = "DƯ NỢ — " + tenKH;

            try
            {
                PhieuIn.Mo(tieuDe, cuaHang, new string[] {
                    "Mã khách hàng|" + maKH,
                    "Khách hàng|" + tenKH,
                    "Địa chỉ|" + diaChi,
                    "Điện thoại|" + dienThoai,
                    "Tháng / Năm|" + thang,
                    "Đầu kỳ|" + SafeN0(m_DuNo.DauKy) + " đ",
                    "Phát sinh|" + SafeN0(m_DuNo.PhatSinh) + " đ",
                    "Đã trả|" + SafeN0(m_DuNo.DaTra) + " đ",
                    "Cuối kỳ|" + SafeN0(m_DuNo.CuoiKy) + " đ"
                }, this.Owner ?? this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không in được:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            this.Close();
        }

        static string SafeN0(object v)
        {
            try { return Convert.ToDecimal(v).ToString("N0"); }
            catch { return "0"; }
        }
    }
}
