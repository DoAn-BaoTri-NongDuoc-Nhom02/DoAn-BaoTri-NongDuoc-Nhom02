using System;

namespace CuahangNongduoc.Controller
{
    /// <summary>
    /// Một lựa chọn lọc phiếu theo công nợ: tên hiển thị + biểu thức lọc + cách kiểm tra từng dòng.
    /// REFACTOR #22 Replace Array with Object – trước đây là mảng chuỗi
    /// { "Tất cả", "Đã thanh toán", "Còn nợ" } rồi so sánh chuỗi đã chọn ở 2 nơi trong mỗi form.
    /// </summary>
    public sealed class BoLocCongNo
    {
        public static readonly BoLocCongNo TatCa = new BoLocCongNo("Tất cả", "", 0);
        public static readonly BoLocCongNo DaThanhToan = new BoLocCongNo("Đã thanh toán", "CON_NO <= 0 OR CON_NO IS NULL", 1);
        public static readonly BoLocCongNo ConNo = new BoLocCongNo("Còn nợ", "CON_NO > 0", 2);

        /// <summary>Các lựa chọn theo thứ tự hiển thị trong ComboBox.</summary>
        public static BoLocCongNo[] DanhSach()
        {
            return new BoLocCongNo[] { TatCa, DaThanhToan, ConNo };
        }

        private readonly String m_Ten;
        private readonly String m_BieuThucLoc;
        private readonly int m_Kieu;

        private BoLocCongNo(String ten, String bieuThucLoc, int kieu)
        {
            m_Ten = ten;
            m_BieuThucLoc = bieuThucLoc;
            m_Kieu = kieu;
        }

        public String Ten { get { return m_Ten; } }

        /// <summary>Biểu thức dùng cho BindingSource.Filter.</summary>
        public String BieuThucLoc { get { return m_BieuThucLoc; } }

        /// <summary>Dòng có số còn nợ này có được hiện không.</summary>
        public bool Khop(long conNo)
        {
            if (m_Kieu == 1) return conNo <= 0;
            if (m_Kieu == 2) return conNo > 0;
            return true;
        }

        public override String ToString() { return m_Ten; }
    }
}
