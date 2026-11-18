using System;

namespace CuahangNongduoc.Entities
{
    /// <summary>
    /// Loại vai trò người dùng (ứng với VAI_TRO.ID trong CSDL).
    /// REFACTOR #29 Replace Type Code with Class – trước đây vai trò chỉ là số int (1, 2, 3, 4)
    /// rải khắp nơi; truyền nhầm một số bất kỳ vẫn biên dịch được. Nay chỉ có 4 đối tượng cố định.
    /// </summary>
    public sealed class LoaiVaiTro
    {
        // Mã trong bảng VAI_TRO (dùng được trong switch/case)
        public const int MA_ADMIN = 1;
        public const int MA_BAN_HANG = 2;
        public const int MA_KHO = 3;
        public const int MA_KE_TOAN = 4;

        public static readonly LoaiVaiTro Admin = new LoaiVaiTro(MA_ADMIN, "Admin");
        public static readonly LoaiVaiTro BanHang = new LoaiVaiTro(MA_BAN_HANG, "Nhân viên Bán hàng");
        public static readonly LoaiVaiTro Kho = new LoaiVaiTro(MA_KHO, "Nhân viên Kho");
        public static readonly LoaiVaiTro KeToan = new LoaiVaiTro(MA_KE_TOAN, "Kế toán");

        private static readonly LoaiVaiTro[] s_TatCa = new LoaiVaiTro[] { Admin, BanHang, Kho, KeToan };

        private readonly int m_Ma;
        private readonly String m_Ten;

        private LoaiVaiTro(int ma, String ten)
        {
            m_Ma = ma;
            m_Ten = ten;
        }

        public int Ma { get { return m_Ma; } }
        public String Ten { get { return m_Ten; } }

        /// <summary>Đổi mã trong CSDL sang loại vai trò; mã lạ thì trả về null.</summary>
        public static LoaiVaiTro TuMa(int ma)
        {
            foreach (LoaiVaiTro v in s_TatCa)
                if (v.m_Ma == ma) return v;
            return null;
        }

        public override String ToString() { return m_Ten; }
    }
}
