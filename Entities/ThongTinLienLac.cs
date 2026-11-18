using System;
using System.Text.RegularExpressions;

namespace CuahangNongduoc.BusinessObject
{
    // REFACTOR #19 Replace Data Value with Object.
    // Số điện thoại, email, mã số thuế trước đây chỉ là chuỗi, còn luật kiểm tra nằm rải rác
    // (KhachHangController và NhaCungCapController mỗi nơi một bản Regex giống nhau).
    // Nay mỗi loại là một đối tượng giá trị: chỉ tạo được khi hợp lệ và tự giữ luật của mình.

    /// <summary>Số điện thoại Việt Nam: 0xxxxxxxx (9–11 số) hoặc +84xxxxxxxx; cho phép khoảng trắng . - ( ).</summary>
    public sealed class SoDienThoai
    {
        private readonly String m_GiaTri;

        private SoDienThoai(String giaTri) { m_GiaTri = giaTri; }

        /// <summary>Dạng đã bỏ khoảng trắng và dấu phân cách, ví dụ 0901234567.</summary>
        public String GiaTri { get { return m_GiaTri; } }

        public static String RutGon(String chuoi)
        {
            return chuoi == null ? "" : Regex.Replace(chuoi.Trim(), @"[\s\.\-\(\)]", "");
        }

        public static bool HopLe(String chuoi)
        {
            return chuoi != null && Regex.IsMatch(RutGon(chuoi), @"^(\+84|0)\d{8,10}$");
        }

        public static SoDienThoai Tao(String chuoi)
        {
            if (!HopLe(chuoi))
                throw new ArgumentException("Số điện thoại \"" + chuoi + "\" không hợp lệ.");
            return new SoDienThoai(RutGon(chuoi));
        }

        public override String ToString() { return m_GiaTri; }
        public override bool Equals(object obj) { SoDienThoai k = obj as SoDienThoai; return k != null && k.m_GiaTri == m_GiaTri; }
        public override int GetHashCode() { return m_GiaTri.GetHashCode(); }
    }

    /// <summary>Email dạng ten@mien.xx.</summary>
    public sealed class Email
    {
        private readonly String m_GiaTri;

        private Email(String giaTri) { m_GiaTri = giaTri; }

        public String GiaTri { get { return m_GiaTri; } }

        public static bool HopLe(String chuoi)
        {
            return chuoi != null && Regex.IsMatch(chuoi.Trim(),
                @"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9\-]+(\.[A-Za-z0-9\-]+)*\.[A-Za-z]{2,}$");
        }

        public static Email Tao(String chuoi)
        {
            if (!HopLe(chuoi))
                throw new ArgumentException("Email \"" + chuoi + "\" không hợp lệ.");
            return new Email(chuoi.Trim().ToLowerInvariant());
        }

        public override String ToString() { return m_GiaTri; }
        public override bool Equals(object obj) { Email k = obj as Email; return k != null && k.m_GiaTri == m_GiaTri; }
        public override int GetHashCode() { return m_GiaTri.GetHashCode(); }
    }

    /// <summary>Mã số thuế: 10 số, hoặc 13 số cho đơn vị phụ thuộc (0312345678-001).</summary>
    public sealed class MaSoThue
    {
        private readonly String m_GiaTri;

        private MaSoThue(String giaTri) { m_GiaTri = giaTri; }

        public String GiaTri { get { return m_GiaTri; } }

        public static bool HopLe(String chuoi)
        {
            return chuoi != null && Regex.IsMatch(chuoi.Trim(), @"^\d{10}(-?\d{3})?$");
        }

        public static MaSoThue Tao(String chuoi)
        {
            if (!HopLe(chuoi))
                throw new ArgumentException("Mã số thuế \"" + chuoi + "\" không hợp lệ.");
            return new MaSoThue(chuoi.Trim());
        }

        public override String ToString() { return m_GiaTri; }
        public override bool Equals(object obj) { MaSoThue k = obj as MaSoThue; return k != null && k.m_GiaTri == m_GiaTri; }
        public override int GetHashCode() { return m_GiaTri.GetHashCode(); }
    }
}
