using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CuahangNongduoc.BanHang;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc
{
    /// <summary>
    /// Ô nhập dịch vụ phụ (tên + số tiền) và cách giảm giá cho hộp thoại "Thông tin thêm" của
    /// form Bán lẻ / Bán sỉ (dùng chung cho cả hai form).
    /// </summary>
    public class ucDichVuGiamGia : UserControl
    {
        private readonly ComboBox m_KieuGiam = new ComboBox();
        private readonly NumericUpDown m_GiaTriGiam = new NumericUpDown();
        private readonly DataGridView m_LuoiDichVu = new DataGridView();
        private readonly List<String> m_MaKieuGiam = new List<String>();

        public ucDichVuGiamGia()
        {
            this.Size = new Size(340, 190);

            Label lblGiam = new Label();
            lblGiam.Text = "Giảm giá:";
            lblGiam.Location = new Point(0, 4);
            lblGiam.AutoSize = true;
            this.Controls.Add(lblGiam);

            m_KieuGiam.DropDownStyle = ComboBoxStyle.DropDownList;
            m_KieuGiam.Location = new Point(118, 0);
            m_KieuGiam.Width = 222;
            // PATTERN #21 Strategy (nơi dùng) – người bán chọn chiến lược giảm giá; lựa chọn "Khuyến mãi theo điều kiện"
            // dùng PATTERN #15 Interpreter (điều kiện khuyến mãi viết thành câu, xem DieuKienKhuyenMai.cs).
            foreach (String[] lc in ChienLuocGiamGia.DanhSachLuaChon())
            {
                m_MaKieuGiam.Add(lc[0]);
                m_KieuGiam.Items.Add(lc[1]);
            }
            m_KieuGiam.SelectedIndex = 0;
            m_KieuGiam.SelectedIndexChanged += delegate { CapNhatOGiaTri(); };
            this.Controls.Add(m_KieuGiam);

            Label lblGiaTri = new Label();
            lblGiaTri.Text = "Giá trị (% / đồng):";
            lblGiaTri.Location = new Point(0, 32);
            lblGiaTri.AutoSize = true;
            this.Controls.Add(lblGiaTri);

            m_GiaTriGiam.Location = new Point(118, 28);
            m_GiaTriGiam.Width = 222;
            m_GiaTriGiam.Maximum = TienUpDown.TOI_DA;
            m_GiaTriGiam.DecimalPlaces = 2;
            m_GiaTriGiam.ThousandsSeparator = true;
            this.Controls.Add(m_GiaTriGiam);

            Label lblDv = new Label();
            lblDv.Text = "Dịch vụ phụ (giao hàng, phun thuốc hộ, bốc xếp...):";
            lblDv.Location = new Point(0, 58);
            lblDv.AutoSize = true;
            this.Controls.Add(lblDv);

            // PATTERN #8 Composite (nơi dùng) – thêm cả một gói dịch vụ: gói tự trả ra các dịch vụ lẻ bên trong.
            Button btnGoi = new Button();
            btnGoi.Text = "+ Gói chăm sóc lúa";
            btnGoi.SetBounds(220, 54, 120, 22);
            btnGoi.Click += delegate
            {
                foreach (DichVuPhatSinh dv in GoiDichVu.GoiChamSocLua().CacDichVuLe())
                    m_LuoiDichVu.Rows.Add(dv.Ten, dv.SoTien);
            };
            this.Controls.Add(btnGoi);

            m_LuoiDichVu.Location = new Point(0, 76);
            m_LuoiDichVu.Size = new Size(340, 112);
            m_LuoiDichVu.AllowUserToAddRows = true;
            m_LuoiDichVu.RowHeadersVisible = false;
            m_LuoiDichVu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            m_LuoiDichVu.Columns.Add("colTenDichVu", "Tên dịch vụ");
            DataGridViewTextBoxColumn colTien = new DataGridViewTextBoxColumn();
            colTien.Name = "colSoTien";
            colTien.HeaderText = "Số tiền";
            colTien.DefaultCellStyle.Format = "#,##0";
            m_LuoiDichVu.Columns.Add(colTien);
            this.Controls.Add(m_LuoiDichVu);

            CapNhatOGiaTri();
        }

        private void CapNhatOGiaTri()
        {
            String ma = m_MaKieuGiam[Math.Max(0, m_KieuGiam.SelectedIndex)];
            m_GiaTriGiam.Enabled = (ma == ChienLuocGiamGia.MA_PHAN_TRAM || ma == ChienLuocGiamGia.MA_SO_TIEN);
            m_GiaTriGiam.Maximum = (ma == ChienLuocGiamGia.MA_PHAN_TRAM) ? 100 : TienUpDown.TOI_DA;
        }

        /// <summary>Hiện lên các giá trị đang có trong thông tin hóa đơn.</summary>
        public void NapTu(ThongTinHoaDon thongTin)
        {
            int i = m_MaKieuGiam.IndexOf(thongTin.KieuGiam);
            m_KieuGiam.SelectedIndex = i >= 0 ? i : 0;
            m_GiaTriGiam.Value = Math.Min(m_GiaTriGiam.Maximum, thongTin.GiaTriGiam);
            m_LuoiDichVu.Rows.Clear();
            foreach (DichVuPhatSinh dv in thongTin.DichVu)
                m_LuoiDichVu.Rows.Add(dv.Ten, dv.SoTien);
        }

        /// <summary>Ghi giá trị trên ô nhập vào thông tin hóa đơn. Dòng dịch vụ thiếu tên hoặc sai số tiền thì bỏ qua.</summary>
        public void GhiVao(ThongTinHoaDon thongTin)
        {
            thongTin.KieuGiam = m_MaKieuGiam[Math.Max(0, m_KieuGiam.SelectedIndex)];
            thongTin.GiaTriGiam = m_GiaTriGiam.Enabled ? m_GiaTriGiam.Value : 0;

            List<DichVuPhatSinh> ds = new List<DichVuPhatSinh>();
            foreach (DataGridViewRow r in m_LuoiDichVu.Rows)
            {
                if (r.IsNewRow) continue;
                String ten = Convert.ToString(r.Cells[0].Value);
                long tien;
                if (String.IsNullOrEmpty(ten) || !long.TryParse(Convert.ToString(r.Cells[1].Value).Replace(",", "").Replace(".", ""), out tien) || tien < 0)
                    continue;
                ds.Add(new DichVuPhatSinh(ten, tien));
            }
            thongTin.GanDichVu(ds);
        }
    }
}
