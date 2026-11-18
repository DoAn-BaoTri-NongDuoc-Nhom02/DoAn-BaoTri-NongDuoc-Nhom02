using System;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    /// <summary>
    /// Điều phối các ô số trên form lập hóa đơn (Bán lẻ / Bán sỉ).
    ///
    /// PATTERN #17 Mediator.
    /// Trước đây mỗi ô tự gắn handler và tự sửa ô khác (đơn giá sửa thành tiền, tổng tiền / đã trả / còn nợ
    /// sửa lẫn nhau – kể cả ô Còn nợ gọi lại chính handler của nó). Nay các ô chỉ báo "tôi vừa đổi" cho
    /// mediator; mediator là nơi DUY NHẤT biết ô nào phải tính lại theo ô nào. Hai form dùng chung một lớp.
    /// </summary>
    public class HoaDonMediator
    {
        private readonly NumericUpDown m_DonGia;
        private readonly NumericUpDown m_SoLuong;
        private readonly NumericUpDown m_ThanhTien;
        private readonly NumericUpDown m_TongTien;
        private readonly NumericUpDown m_DaTra;
        private readonly NumericUpDown m_ConNo;
        private bool m_DangCapNhat;

        public HoaDonMediator(NumericUpDown donGia, NumericUpDown soLuong, NumericUpDown thanhTien,
            NumericUpDown tongTien, NumericUpDown daTra, NumericUpDown conNo)
        {
            m_DonGia = donGia;
            m_SoLuong = soLuong;
            m_ThanhTien = thanhTien;
            m_TongTien = tongTien;
            m_DaTra = daTra;
            m_ConNo = conNo;

            EventHandler baoDoi = new EventHandler(ThanhPhanDaDoi);
            m_DonGia.ValueChanged += baoDoi;
            m_SoLuong.ValueChanged += baoDoi;
            m_TongTien.ValueChanged += baoDoi;
            m_DaTra.ValueChanged += baoDoi;
        }

        /// <summary>Một ô vừa đổi giá trị: quyết định ô nào phải tính lại.</summary>
        private void ThanhPhanDaDoi(object sender, EventArgs e)
        {
            if (m_DangCapNhat) return;    // tránh vòng lặp khi chính mediator đang gán giá trị
            m_DangCapNhat = true;
            try
            {
                if (sender == m_DonGia || sender == m_SoLuong)
                    Gan(m_ThanhTien, m_DonGia.Value * m_SoLuong.Value);
                else if (sender == m_TongTien || sender == m_DaTra)
                    Gan(m_ConNo, m_TongTien.Value - m_DaTra.Value);
            }
            finally
            {
                m_DangCapNhat = false;
            }
        }

        private static void Gan(NumericUpDown o, decimal giaTri)
        {
            o.Value = Math.Max(o.Minimum, Math.Min(o.Maximum, giaTri));
        }
    }
}
