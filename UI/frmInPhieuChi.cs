using System;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc
{
    /// <summary>
    /// PM-29: In phiếu chi bằng PhieuIn (không ReportViewer).
    /// Entity: PhieuChi.Id, NgayChi, TongTien, LyDoChi, GhiChu — LyDoChi.LyDo
    /// </summary>
    public partial class frmInPhieuChi : Form
    {
        PhieuChi m_PhieuChi;

        public frmInPhieuChi(PhieuChi ph)
        {
            InitializeComponent();
            m_PhieuChi = ph;
        }

        private void frmInPhieuChi_Load(object sender, EventArgs e)
        {
            this.Text = "In phiếu chi";

            if (m_PhieuChi == null)
            {
                MessageBox.Show("Không có dữ liệu phiếu chi để in.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            string ma = m_PhieuChi.Id ?? "";
            if (ma.Trim().Length == 0)
            {
                MessageBox.Show("Phiếu chi chưa có mã. Hãy lưu phiếu trước khi in.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;
            }

            string lyDo = "";
            if (m_PhieuChi.LyDoChi != null)
                lyDo = m_PhieuChi.LyDoChi.LyDo ?? "";

            string ngay = m_PhieuChi.NgayChi.ToString("dd/MM/yyyy");
            string soTien = m_PhieuChi.TongTien.ToString("N0");
            string ghiChu = m_PhieuChi.GhiChu ?? "";

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
                string tieuDe = "PHIẾU CHI";
                if (lyDo.Length > 0)
                    tieuDe = "PHIẾU CHI — " + lyDo.ToUpper();

                PhieuIn.Mo(tieuDe, cuaHang, new string[] {
                    "Mã phiếu|" + ma,
                    "Lý do chi|" + lyDo,
                    "Ngày chi|" + ngay,
                    "Số tiền|" + soTien + " đ",
                    "Ghi chú|" + ghiChu
                }, this.Owner ?? this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không in được:\n" + ex.Message, "In phiếu chi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            this.Close();
        }
    }
}
