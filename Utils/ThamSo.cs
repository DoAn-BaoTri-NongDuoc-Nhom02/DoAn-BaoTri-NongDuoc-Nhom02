using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;

namespace CuahangNongduoc
{
    public enum Controll
    {
        Normal,
        AddNew,
        Edit
    }
    /// <summary>Các loại mã được cấp số thứ tự trong bảng THAM_SO.</summary>
    public enum LoaiChungTu
    {
        PhieuNhap,
        PhieuBan,
        PhieuThanhToan,
        PhieuChi,
        SanPham,
        NhaCungCap,
        KhachHang
    }

    public class ThamSo
    {
        public static void PreMonth(ref int thangtruoc, ref int namtruoc, int thang, int nam)
        {
            thangtruoc = thang - 1;
            namtruoc = nam;
            if (thangtruoc == 0)
            {
                thangtruoc = 12;
                namtruoc= nam - 1;
            }
        }

        public static bool LaSoNguyen(String so)
        {
            try
            {
                Convert.ToInt64(so);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Số thứ tự kế tiếp của một loại chứng từ / danh mục (cột tương ứng trong bảng THAM_SO).
        /// REFACTOR #45 Parameterize Method – thay cho 7 cặp hàm/thuộc tính gần như giống hệt nhau
        /// (LayMaPhieuNhap/GanMaPhieuNhap, LayMaPhieuBan/GanMaPhieuBan, ..., SanPham, KhachHang, PhieuChi).
        /// </summary>
        public static long LayMa(LoaiChungTu loai)
        {
            DataService ds = new DataService();
            object obj = ds.ExecuteScalar(new OleDbCommand("SELECT " + TenCot(loai) + " FROM THAM_SO"));
            return DuLieu.LaySoLon(obj);
        }

        /// <summary>
        /// Ghi số thứ tự kế tiếp. Chỉ tăng, không giảm: nếu máy khác đã ghi số lớn hơn thì giữ số đó
        /// (giảm nguy cơ hai máy cấp trùng mã).
        /// </summary>
        public static void GanMa(LoaiChungTu loai, long giaTri)
        {
            String cot = TenCot(loai);
            DataService ds = new DataService();
            OleDbCommand cmd = new OleDbCommand("UPDATE THAM_SO SET " + cot + " = @giatri WHERE " + cot + " < @giatri2");
            cmd.Parameters.Add("@giatri", OleDbType.BigInt).Value = giaTri;
            cmd.Parameters.Add("@giatri2", OleDbType.BigInt).Value = giaTri;
            ds.ExecuteNonQuery(cmd);
        }

        private static String TenCot(LoaiChungTu loai)
        {
            switch (loai)
            {
                case LoaiChungTu.PhieuNhap: return "PHIEU_NHAP";
                case LoaiChungTu.PhieuBan: return "PHIEU_BAN";
                case LoaiChungTu.PhieuThanhToan: return "PHIEU_THANH_TOAN";
                case LoaiChungTu.PhieuChi: return "PHIEU_CHI";
                case LoaiChungTu.SanPham: return "SAN_PHAM";
                case LoaiChungTu.NhaCungCap: return "NHA_CUNG_CAP";
                case LoaiChungTu.KhachHang: return "KHACH_HANG";
            }
            throw new ArgumentOutOfRangeException("loai");
        }

        public static CuahangNongduoc.BusinessObject.CuaHang LayCuaHang()
        {
            CuahangNongduoc.BusinessObject.CuaHang ch = new CuahangNongduoc.BusinessObject.CuaHang();
            DataService ds = new DataService();
            ds.Load(new OleDbCommand("SELECT TEN_CUA_HANG, DIA_CHI, DIEN_THOAI FROM THAM_SO"));
            if (ds.Bang.Rows.Count > 0)
            {
                ch.TenCuaHang = Convert.ToString(ds.Bang.Rows[0]["TEN_CUA_HANG"]);
                ch.DiaChi = Convert.ToString(ds.Bang.Rows[0]["DIA_CHI"]);
                ch.DienThoai = Convert.ToString(ds.Bang.Rows[0]["DIEN_THOAI"]);
                // Neu ten bi loi font (C?a...), de trong form Thong tin cua hang go lai "Cửa hàng Nông dược An Giang" roi Luu.
            }
            return ch;
        }
        /// <summary>
        /// Lưu thông tin cửa hàng.
        /// REFACTOR #47 Preserve Whole Object – nhận cả đối tượng CuaHang thay vì tách ra 3 tham số
        /// (thêm thông tin mới như email, mã số thuế sau này không phải đổi chữ ký hàm).
        /// </summary>
        public static void GanCuaHang(CuahangNongduoc.BusinessObject.CuaHang cuaHang)
        {
            if (cuaHang == null) throw new ArgumentNullException("cuaHang");
            DataService ds = new DataService();
            OleDbCommand cmd = new OleDbCommand("UPDATE THAM_SO SET TEN_CUA_HANG = @ten_cua_hang, DIA_CHI = @dia_chi, DIEN_THOAI = @dien_thoai ");
            cmd.Parameters.Add("@ten_cua_hang", OleDbType.VarWChar).Value = cuaHang.TenCuaHang ?? "";
            cmd.Parameters.Add("@dia_chi", OleDbType.VarWChar).Value = cuaHang.DiaChi ?? "";
            cmd.Parameters.Add("@dien_thoai", OleDbType.VarWChar).Value = cuaHang.DienThoai ?? "";

            ds.ExecuteNonQuery(cmd);
        }
    }
}
