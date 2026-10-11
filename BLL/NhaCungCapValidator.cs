using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CuahangNongduoc.Controller
{
    /// <summary>
    /// Kiểm tra và chuẩn hóa dữ liệu Nhà cung cấp.
    /// Lớp thuần logic (không phụ thuộc WinForms / CSDL) nên dễ kiểm thử.
    /// Các hàm KiemTra* trả về thông báo lỗi, hoặc null nếu hợp lệ.
    /// </summary>
    public static class NhaCungCapValidator
    {
        // Khớp độ dài cột trong bảng NHA_CUNG_CAP (HO_TEN NVARCHAR(100), DIA_CHI NVARCHAR(200), DIEN_THOAI VARCHAR(20))
        public const int DoDaiToiDaHoTen = 100;
        public const int DoDaiToiDaDiaChi = 200;
        public const int DoDaiToiDaDienThoaiNhap = 20;

        public const string CotHoTen = "HO_TEN";
        public const string CotDiaChi = "DIA_CHI";
        public const string CotDienThoai = "DIEN_THOAI";

        private static readonly Regex KhoangTrang = new Regex(@"\s+");

        // Sau khi chuẩn hóa: bắt đầu bằng 0, tổng 10-11 chữ số
        // (di động 10 số, cố định 10-11 số tùy mã vùng).
        private static readonly Regex DinhDangDienThoai = new Regex(@"^0[0-9]{9,10}$");

        /// <summary>Cắt khoảng trắng đầu/cuối và gộp nhiều khoảng trắng liên tiếp thành một.</summary>
        public static string ChuanHoaChuoi(string s)
        {
            if (s == null) return "";
            return KhoangTrang.Replace(s.Trim(), " ");
        }

        /// <summary>Bỏ khoảng trắng, dấu chấm, gạch ngang, ngoặc; đổi đầu +84 thành 0.</summary>
        public static string ChuanHoaDienThoai(string s)
        {
            if (s == null) return "";
            StringBuilder sb = new StringBuilder();
            foreach (char c in s.Trim())
            {
                if (c == ' ' || c == '.' || c == '-' || c == '(' || c == ')') continue;
                sb.Append(c);
            }
            string kq = sb.ToString();
            if (kq.StartsWith("+84", StringComparison.Ordinal))
                kq = "0" + kq.Substring(3);
            return kq;
        }

        public static string KiemTraHoTen(string giaTri)
        {
            string ten = ChuanHoaChuoi(giaTri);
            if (ten.Length == 0)
                return "Tên nhà cung cấp không được để trống.";
            if (ten.Length > DoDaiToiDaHoTen)
                return "Tên nhà cung cấp tối đa " + DoDaiToiDaHoTen + " ký tự.";

            bool coChuCai = false;
            foreach (char c in ten)
            {
                if (char.IsLetter(c)) { coChuCai = true; break; }
            }
            if (!coChuCai)
                return "Tên nhà cung cấp phải có ít nhất một chữ cái.";
            return null;
        }

        public static string KiemTraDiaChi(string giaTri)
        {
            string diaChi = ChuanHoaChuoi(giaTri);
            if (diaChi.Length == 0)
                return "Địa chỉ không được để trống.";
            if (diaChi.Length > DoDaiToiDaDiaChi)
                return "Địa chỉ tối đa " + DoDaiToiDaDiaChi + " ký tự.";
            return null;
        }

        public static string KiemTraDienThoai(string giaTri)
        {
            string dt = ChuanHoaDienThoai(giaTri);
            if (dt.Length == 0)
                return "Số điện thoại không được để trống.";
            if (!DinhDangDienThoai.IsMatch(dt))
                return "Số điện thoại không hợp lệ. Nhập 10-11 chữ số, bắt đầu bằng 0 hoặc +84 (ví dụ: 0901234567, 0292 3812345).";
            return null;
        }

        /// <summary>
        /// Kiểm tra cả ba trường theo thứ tự Tên, Địa chỉ, Điện thoại.
        /// Trả về lỗi đầu tiên và tên cột bị lỗi qua cotLoi.
        /// </summary>
        public static string KiemTra(string hoTen, string diaChi, string dienThoai, out string cotLoi)
        {
            string loi = KiemTraHoTen(hoTen);
            if (loi != null) { cotLoi = CotHoTen; return loi; }

            loi = KiemTraDiaChi(diaChi);
            if (loi != null) { cotLoi = CotDiaChi; return loi; }

            loi = KiemTraDienThoai(dienThoai);
            if (loi != null) { cotLoi = CotDienThoai; return loi; }

            cotLoi = null;
            return null;
        }
    }
}
