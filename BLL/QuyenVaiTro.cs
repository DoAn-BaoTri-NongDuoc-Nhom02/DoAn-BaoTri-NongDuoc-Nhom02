using System;
using System.Collections.Generic;
using CuahangNongduoc.Entities;

namespace CuahangNongduoc.Controller
{
    /// <summary>Tên các chức năng dùng để phân quyền.</summary>
    public static class ChucNang
    {
        // Nhóm menu
        public const String QUAN_LY = "QUAN_LY";
        public const String NGHIEP_VU = "NGHIEP_VU";
        public const String BAO_CAO = "BAO_CAO";
        public const String TUY_CHINH = "TUY_CHINH";
        // Chức năng
        public const String DON_VI_TINH = "DON_VI_TINH";
        public const String SAN_PHAM = "SAN_PHAM";
        public const String KHACH_HANG = "KHACH_HANG";
        public const String DAI_LY = "DAI_LY";
        public const String NHA_CUNG_CAP = "NHA_CUNG_CAP";
        public const String NHAP_HANG = "NHAP_HANG";
        public const String BAN_HANG = "BAN_HANG";
        public const String BAN_LE = "BAN_LE";
        public const String BAN_SI = "BAN_SI";
        public const String THANH_TOAN = "THANH_TOAN";
        public const String PHIEU_CHI = "PHIEU_CHI";
        public const String LY_DO_CHI = "LY_DO_CHI";
        public const String TONG_HOP_DU_NO = "TONG_HOP_DU_NO";
        public const String TON_KHO = "TON_KHO";
        public const String SO_LUONG_BAN = "SO_LUONG_BAN";
        public const String SAN_PHAM_HET_HAN = "SAN_PHAM_HET_HAN";
        public const String DOANH_THU = "DOANH_THU";
        public const String THONG_TIN_CUA_HANG = "THONG_TIN_CUA_HANG";
        public const String QUAN_LY_TAI_KHOAN = "QUAN_LY_TAI_KHOAN";
        // Yêu cầu 4
        /// <summary>Được giảm giá vượt mức tối đa của nhân viên.</summary>
        public const String GIAM_GIA_KHONG_GIOI_HAN = "GIAM_GIA_KHONG_GIOI_HAN";
        /// <summary>Xem các thống kê theo khoảng ngày.</summary>
        public const String THONG_KE = "THONG_KE";
        /// <summary>Xem số liệu của mọi nhân viên (không chỉ của mình).</summary>
        public const String THONG_KE_TAT_CA_NHAN_VIEN = "THONG_KE_TAT_CA_NHAN_VIEN";
    }

    /// <summary>
    /// Quyền của một vai trò.
    /// REFACTOR #38 Replace Conditional with Polymorphism – frmMain.PhanQuyenTheoVaiTro trước đây là
    /// một switch 136 dòng bật/tắt từng menu theo số vai trò. Nay mỗi vai trò là một lớp con tự biết
    /// mình được dùng chức năng nào; thêm vai trò mới chỉ cần thêm lớp, không sửa frmMain.
    /// </summary>
    public abstract class QuyenVaiTro
    {
        public abstract bool DuocPhep(String chucNang);

        /// <summary>Quyền ứng với một vai trò (vai trò lạ / chưa đăng nhập: không có quyền gì).</summary>
        public static QuyenVaiTro Cua(LoaiVaiTro vaiTro)
        {
            if (vaiTro == LoaiVaiTro.Admin) return new QuyenAdmin();
            if (vaiTro == LoaiVaiTro.BanHang) return new QuyenTheoDanhSach(
                ChucNang.QUAN_LY, ChucNang.NGHIEP_VU, ChucNang.BAO_CAO,
                ChucNang.KHACH_HANG, ChucNang.BAN_HANG, ChucNang.BAN_LE, ChucNang.BAN_SI,
                ChucNang.THANH_TOAN, ChucNang.TON_KHO, ChucNang.THONG_KE);
            if (vaiTro == LoaiVaiTro.Kho) return new QuyenTheoDanhSach(
                ChucNang.QUAN_LY, ChucNang.NGHIEP_VU, ChucNang.BAO_CAO,
                ChucNang.SAN_PHAM, ChucNang.NHAP_HANG, ChucNang.TON_KHO, ChucNang.SAN_PHAM_HET_HAN,
                ChucNang.THONG_KE);
            if (vaiTro == LoaiVaiTro.KeToan) return new QuyenTheoDanhSach(
                ChucNang.NGHIEP_VU, ChucNang.BAO_CAO,
                ChucNang.THANH_TOAN, ChucNang.PHIEU_CHI, ChucNang.LY_DO_CHI, ChucNang.TONG_HOP_DU_NO,
                ChucNang.TON_KHO, ChucNang.SO_LUONG_BAN, ChucNang.DOANH_THU,
                ChucNang.THONG_KE, ChucNang.THONG_KE_TAT_CA_NHAN_VIEN);
            return new QuyenTheoDanhSach();
        }
    }

    /// <summary>Admin: được dùng mọi chức năng (hành vi khác hẳn nên vẫn là lớp con riêng).</summary>
    public class QuyenAdmin : QuyenVaiTro
    {
        public override bool DuocPhep(String chucNang)
        {
            return true;
        }
    }

    // REFACTOR #32 Replace Subclass with Fields – bốn lớp con QuyenBanHang, QuyenKho, QuyenKeToan, QuyenKhongCo
    // chỉ khác nhau ở MỘT mảng hằng (danh sách chức năng), không khác hành vi. Gộp thành một lớp
    // QuyenTheoDanhSach giữ danh sách đó trong trường; QuyenVaiTro.Cua() truyền danh sách cho từng vai trò.
    /// <summary>Quyền là một danh sách chức năng được phép (rỗng = không có quyền gì).</summary>
    public class QuyenTheoDanhSach : QuyenVaiTro
    {
        private readonly Dictionary<String, bool> m_DuocPhep = new Dictionary<String, bool>();

        public QuyenTheoDanhSach(params String[] chucNang)
        {
            foreach (String cn in chucNang)
                m_DuocPhep[cn] = true;
        }

        public override bool DuocPhep(String chucNang)
        {
            return m_DuocPhep.ContainsKey(chucNang);
        }
    }
}
