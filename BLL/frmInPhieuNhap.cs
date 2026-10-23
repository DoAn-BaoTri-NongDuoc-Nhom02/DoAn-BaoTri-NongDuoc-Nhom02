using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    /// <summary>
    /// In phieu nhap – cung kieu form in Phieu Ban; truyen ten_ncc + chi tiet TenSanPham de tranh #Error.
    /// </summary>
    public partial class frmInPhieuNhap : Form
    {
        CuahangNongduoc.BusinessObject.PhieuNhap m_PhieuNhap = null;

        public frmInPhieuNhap(CuahangNongduoc.BusinessObject.PhieuNhap ph)
        {
            m_PhieuNhap = ph;
            InitializeComponent();

            reportViewer.LocalReport.ExecuteReportInCurrentAppDomain(
                System.Reflection.Assembly.GetExecutingAssembly().Evidence);
            this.reportViewer.LocalReport.SubreportProcessing +=
                new Microsoft.Reporting.WinForms.SubreportProcessingEventHandler(LocalReport_SubreportProcessing);
        }

        void LocalReport_SubreportProcessing(object sender, Microsoft.Reporting.WinForms.SubreportProcessingEventArgs e)
        {
            e.DataSources.Clear();
            object chiTiet = (m_PhieuNhap != null && m_PhieuNhap.ChiTiet != null)
                ? (object)m_PhieuNhap.ChiTiet
                : new List<CuahangNongduoc.BusinessObject.MaSanPham>();
            e.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource(
                "CuahangNongduoc_BusinessObject_MaSanPham", chiTiet));
        }

        private void frmInPhieuNhap_Load(object sender, EventArgs e)
        {
            this.Text = "In Phiếu Nhập Hàng";

            if (m_PhieuNhap == null)
            {
                MessageBox.Show("Không có dữ liệu phiếu nhập để in.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            // Dam bao ten NCC
            string tenNcc = "";
            try
            {
                if (m_PhieuNhap.NhaCungCap != null)
                    tenNcc = m_PhieuNhap.NhaCungCap.HoTen ?? "";
            }
            catch { }
            if (string.IsNullOrEmpty(tenNcc))
            {
                try
                {
                    // thu load lai
                    CuahangNongduoc.Controller.PhieuNhapController c = new CuahangNongduoc.Controller.PhieuNhapController();
                    var lai = c.LayPhieuNhap(m_PhieuNhap.Id);
                    if (lai != null)
                    {
                        m_PhieuNhap = lai;
                        if (m_PhieuNhap.NhaCungCap != null)
                            tenNcc = m_PhieuNhap.NhaCungCap.HoTen ?? "";
                    }
                }
                catch { }
            }

            // Dam bao chi tiet co TenSanPham
            try
            {
                if (m_PhieuNhap.ChiTiet != null)
                {
                    foreach (var line in m_PhieuNhap.ChiTiet)
                    {
                        if (line == null) continue;
                        if (string.IsNullOrEmpty(line.TenSanPham) && line.SanPham != null)
                            line.TenSanPham = line.SanPham.TenSanPham ?? "";
                        if (line.TenSanPham == null)
                            line.TenSanPham = "";
                    }
                }
            }
            catch { }

            Num2Str num = new Num2Str();
            IList<Microsoft.Reporting.WinForms.ReportParameter> param =
                new List<Microsoft.Reporting.WinForms.ReportParameter>();

            CuahangNongduoc.BusinessObject.CuaHang ch = null;
            try { ch = ThamSo.LayCuaHang(); } catch { }
            string tenCH = (ch != null && ch.TenCuaHang != null) ? ch.TenCuaHang : "";
            string diaChi = (ch != null && ch.DiaChi != null) ? ch.DiaChi : "";
            string dienThoai = (ch != null && ch.DienThoai != null) ? ch.DienThoai : "";

            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("ten_cua_hang", tenCH));
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("dia_chi", diaChi));
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("dien_thoai", dienThoai));
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("ten_ncc", tenNcc ?? ""));

            string bangChu = "0";
            try { bangChu = num.NumberToString(m_PhieuNhap.TongTien.ToString()); }
            catch { bangChu = m_PhieuNhap.TongTien.ToString(); }
            param.Add(new Microsoft.Reporting.WinForms.ReportParameter("bang_chu", bangChu));

            this.reportViewer.LocalReport.SetParameters(param);
            this.PhieuNhapBindingSource.DataSource = m_PhieuNhap;
            this.reportViewer.RefreshReport();
        }
    }
}
