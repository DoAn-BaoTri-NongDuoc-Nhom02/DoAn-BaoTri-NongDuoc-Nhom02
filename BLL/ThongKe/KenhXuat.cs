using System;
using System.Data;
using System.Windows.Forms;

namespace CuahangNongduoc.ThongKe
{
    /// <summary>
    /// Nơi đưa kết quả thống kê ra (phía "cài đặt" của Bridge).
    ///
    /// PATTERN #7 Bridge.
    /// Có hai chiều thay đổi độc lập: LOẠI thống kê (tồn kho, chi phí, giảm giá, theo nhân viên...) và CÁCH
    /// xuất (hiện lên lưới, ghi file CSV mở bằng Excel...). Nếu kế thừa thẳng sẽ phải có 4 x 2 = 8 lớp.
    /// Bridge tách hai chiều: lớp thống kê (ThongKeTheoKhoangNgay) giữ một tham chiếu IKenhXuat và gọi
    /// Xuat(); thêm loại thống kê hay thêm kênh xuất mới đều không phải sửa chiều còn lại.
    /// </summary>
    public interface IKenhXuat
    {
        void Xuat(String tieuDe, DataTable bang);
    }

    /// <summary>Hiện kết quả lên một DataGridView (ẩn các cột kỹ thuật, định dạng số tiền).</summary>
    public class XuatRaLuoi : IKenhXuat
    {
        private readonly DataGridView m_Luoi;
        private readonly Label m_TieuDe;

        public XuatRaLuoi(DataGridView luoi, Label tieuDe)
        {
            if (luoi == null) throw new ArgumentNullException("luoi");
            m_Luoi = luoi;
            m_TieuDe = tieuDe;
        }

        public void Xuat(String tieuDe, DataTable bang)
        {
            if (m_TieuDe != null) m_TieuDe.Text = tieuDe;
            m_Luoi.DataSource = bang;
            foreach (DataGridViewColumn c in m_Luoi.Columns)
            {
                if (c.Name.StartsWith("ID_") || c.Name == ThongKeTheoKhoangNgay.COT_LA_DONG_TONG)
                    c.Visible = false;
                Type t = bang.Columns.Contains(c.DataPropertyName) ? bang.Columns[c.DataPropertyName].DataType : typeof(String);
                if (t == typeof(long) || t == typeof(decimal) || t == typeof(int))
                {
                    c.DefaultCellStyle.Format = "#,##0";
                    c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (t == typeof(DateTime))
                    c.DefaultCellStyle.Format = "dd/MM/yyyy";
            }
            if (bang.Rows.Count > 0 && bang.Columns.Contains(ThongKeTheoKhoangNgay.COT_LA_DONG_TONG)
                && m_Luoi.Rows.Count > 0)
            {
                DataGridViewRow dongCuoi = m_Luoi.Rows[m_Luoi.Rows.Count - 1];
                dongCuoi.DefaultCellStyle.Font = new System.Drawing.Font(m_Luoi.Font, System.Drawing.FontStyle.Bold);
            }
        }
    }

    /// <summary>Ghi kết quả ra file CSV (mở bằng Excel), dùng lớp BaoCaoCsv có sẵn của dự án.</summary>
    public class XuatRaCsv : IKenhXuat
    {
        private readonly IWin32Window m_ChuSoHuu;

        public XuatRaCsv(IWin32Window chuSoHuu)
        {
            m_ChuSoHuu = chuSoHuu;
        }

        public void Xuat(String tieuDe, DataTable bang)
        {
            DataTable banIn = bang.Copy();
            if (banIn.Columns.Contains(ThongKeTheoKhoangNgay.COT_LA_DONG_TONG))
                banIn.Columns.Remove(ThongKeTheoKhoangNgay.COT_LA_DONG_TONG);
            for (int i = banIn.Columns.Count - 1; i >= 0; i--)
                if (banIn.Columns[i].ColumnName.StartsWith("ID_")) banIn.Columns.RemoveAt(i);
            BaoCaoCsv.Xuat(banIn, tieuDe.Replace(' ', '_') + ".csv", m_ChuSoHuu);
        }
    }
}
