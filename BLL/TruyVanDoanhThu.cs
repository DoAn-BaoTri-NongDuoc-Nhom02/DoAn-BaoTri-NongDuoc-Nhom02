using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.Controller
{
    /// <summary>
    /// Truy vấn doanh thu theo khoảng ngày, lọc theo khách hàng / sản phẩm / kênh bán.
    /// REFACTOR #8 Replace Method with Method Object – trước đây là hàm frmDoanhThu.DocDoanhThu dài 114 dòng
    /// với nhiều biến cục bộ và câu SQL viết thẳng trong form. Nay các tham số thành trường của lớp,
    /// phần tính giá vốn và tổng chi là hàm riêng của lớp, form chỉ gọi ThucHien().
    /// </summary>
    public class TruyVanDoanhThu
    {
        public const int KENH_TAT_CA = 0;
        public const int KENH_SI = 1;
        public const int KENH_LE = 2;

        private readonly KhoangNgay m_Khoang;
        private readonly string m_IdKhachHang;
        private readonly string m_IdSanPham;
        private readonly int m_Kenh;

        public TruyVanDoanhThu(KhoangNgay khoang, string idKhachHang, string idSanPham, int kenh)
        {
            if (khoang == null) throw new ArgumentNullException("khoang");
            m_Khoang = khoang;
            m_IdKhachHang = idKhachHang ?? "";
            m_IdSanPham = idSanPham ?? "";
            m_Kenh = kenh;
        }

        /// <summary>Chạy truy vấn, trả bảng doanh thu (TongChi nằm trong ExtendedProperties).</summary>
        public DataTable ThucHien()
        {
            KhoangNgay khoang = m_Khoang;
            // 1 tham số ngày tối đa → tránh lỗi OleDb; lọc trong bộ nhớ cho KH/SP/kênh
            DataService ds = new DataService();
            string sql =
                "SELECT PB.ID, PB.NGAY_BAN, PB.TONG_TIEN, PB.DA_TRA, PB.CON_NO, " +
                "PB.ID_KHACH_HANG, KH.HO_TEN, KH.LOAI_KH " +
                "FROM PHIEU_BAN PB " +
                "LEFT JOIN KHACH_HANG KH ON PB.ID_KHACH_HANG = KH.ID " +
                "WHERE PB.NGAY_BAN >= @tu AND PB.NGAY_BAN < @den";

            // den exclusive = den+1 ngày
            OleDbCommand cmd = new OleDbCommand(sql);
            cmd.Parameters.Add("tu", OleDbType.Date).Value = khoang.TuNgay;
            cmd.Parameters.Add("den", OleDbType.Date).Value = khoang.DenNgayKhongBaoGom;

            try
            {
                ds.Load(cmd);
            }
            catch
            {
                // Fallback 1 tham số: lấy đến den rồi lọc từ ngày trong RAM
                ds = new DataService();
                OleDbCommand cmd2 = new OleDbCommand(
                    "SELECT PB.ID, PB.NGAY_BAN, PB.TONG_TIEN, PB.DA_TRA, PB.CON_NO, " +
                    "PB.ID_KHACH_HANG, KH.HO_TEN, KH.LOAI_KH " +
                    "FROM PHIEU_BAN PB LEFT JOIN KHACH_HANG KH ON PB.ID_KHACH_HANG = KH.ID " +
                    "WHERE PB.NGAY_BAN <= @den");
                cmd2.Parameters.Add("den", OleDbType.Date).Value = khoang.DenNgayKhongBaoGom.AddSeconds(-1);
                ds.Load(cmd2);
            }

            string idKh = m_IdKhachHang;
            string idSp = m_IdSanPham;
            int kenh = m_Kenh; // 0 all, 1 sỉ, 2 lẻ

            // Chi phí (phiếu chi) trong khoảng
            long tongChi = LayTongChi(khoang);

            // Cache giá vốn theo phiếu (nếu lọc SP thì chỉ tính SP đó)
            Dictionary<string, long[]> vonVaSp = TinhGiaVonVaLocSp(khoang, idSp);

            DataTable t = new DataTable();
            t.Columns.Add("MaPhieu", typeof(string));
            t.Columns.Add("NgayBan", typeof(DateTime));
            t.Columns.Add("TenKhachHang", typeof(string));
            t.Columns.Add("Kenh", typeof(string));
            t.Columns.Add("TongTien", typeof(long));
            t.Columns.Add("DaTra", typeof(long));
            t.Columns.Add("ConNo", typeof(long));
            t.Columns.Add("GiaVon", typeof(long));
            t.Columns.Add("LaiGop", typeof(long));

            foreach (DataRow r in ds.Bang.Rows)
            {
                DateTime ngay = r["NGAY_BAN"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["NGAY_BAN"]).Date;
                if (!khoang.Chua(ngay)) continue;

                string maKh = r["ID_KHACH_HANG"] == DBNull.Value ? "" : Convert.ToString(r["ID_KHACH_HANG"]);
                if (idKh.Length > 0 && maKh != idKh) continue;

                bool laDaiLy = false;
                try
                {
                    if (r["LOAI_KH"] != DBNull.Value)
                        laDaiLy = Convert.ToBoolean(r["LOAI_KH"]);
                }
                catch
                {
                    try { laDaiLy = Convert.ToInt32(r["LOAI_KH"]) != 0; } catch { }
                }
                if (kenh == 1 && !laDaiLy) continue; // bán sỉ = đại lý
                if (kenh == 2 && laDaiLy) continue;

                string maPhieu = Convert.ToString(r["ID"]);
                long giaVon = 0;
                bool coSp = true;
                if (vonVaSp.ContainsKey(maPhieu))
                {
                    giaVon = vonVaSp[maPhieu][0];
                    coSp = vonVaSp[maPhieu][1] > 0;
                }
                else if (idSp.Length > 0)
                {
                    coSp = false; // lọc SP mà phiếu không có SP đó
                }
                if (!coSp) continue;

                long tong = r["TONG_TIEN"] == DBNull.Value ? 0L : Convert.ToInt64(r["TONG_TIEN"]);
                // Nếu lọc theo SP: doanh thu = thành tiền chi tiết SP (ước lượng qua vonVaSp[2] nếu có)
                if (idSp.Length > 0 && vonVaSp.ContainsKey(maPhieu) && vonVaSp[maPhieu].Length > 2)
                    tong = vonVaSp[maPhieu][2];

                long daTra = r["DA_TRA"] == DBNull.Value ? 0L : Convert.ToInt64(r["DA_TRA"]);
                long conNo = r["CON_NO"] == DBNull.Value ? 0L : Convert.ToInt64(r["CON_NO"]);
                long lai = tong - giaVon;

                DataRow n = t.NewRow();
                n["MaPhieu"] = maPhieu;
                n["NgayBan"] = ngay;
                n["TenKhachHang"] = r["HO_TEN"] == DBNull.Value ? "" : Convert.ToString(r["HO_TEN"]);
                n["Kenh"] = laDaiLy ? "Bán sỉ" : "Bán lẻ";
                n["TongTien"] = tong;
                n["DaTra"] = daTra;
                n["ConNo"] = conNo;
                n["GiaVon"] = giaVon;
                n["LaiGop"] = lai;
                t.Rows.Add(n);
            }

            // Gắn chi phí tổng vào Tag để CapNhatTong dùng
            t.ExtendedProperties["TongChi"] = tongChi;
            return t;
        }

        /// <summary>
        /// Trả về dictionary: mã phiếu → [giaVon, coSanPhamLoc(1/0), thanhTienSp]
        /// </summary>
        private Dictionary<string, long[]> TinhGiaVonVaLocSp(KhoangNgay khoang, string idSp)
        {
            Dictionary<string, long[]> map = new Dictionary<string, long[]>();
            try
            {
                DataService ds = new DataService();
                string sql =
                    "SELECT CT.ID_PHIEU_BAN, CT.SO_LUONG, CT.DON_GIA, CT.THANH_TIEN, " +
                    "MSP.DON_GIA_NHAP, MSP.ID_SAN_PHAM, PB.NGAY_BAN " +
                    "FROM CHI_TIET_PHIEU_BAN CT " +
                    "INNER JOIN PHIEU_BAN PB ON CT.ID_PHIEU_BAN = PB.ID " +
                    "LEFT JOIN MA_SAN_PHAM MSP ON CT.ID_MA_SAN_PHAM = MSP.ID " +
                    "WHERE PB.NGAY_BAN >= @tu AND PB.NGAY_BAN < @den";
                OleDbCommand cmd = new OleDbCommand(sql);
                cmd.Parameters.Add("tu", OleDbType.Date).Value = khoang.TuNgay;
                cmd.Parameters.Add("den", OleDbType.Date).Value = khoang.DenNgayKhongBaoGom;
                try { ds.Load(cmd); }
                catch
                {
                    ds = new DataService();
                    OleDbCommand cmd2 = new OleDbCommand(
                        "SELECT CT.ID_PHIEU_BAN, CT.SO_LUONG, CT.DON_GIA, CT.THANH_TIEN, " +
                        "MSP.DON_GIA_NHAP, MSP.ID_SAN_PHAM " +
                        "FROM CHI_TIET_PHIEU_BAN CT " +
                        "LEFT JOIN MA_SAN_PHAM MSP ON CT.ID_MA_SAN_PHAM = MSP.ID");
                    ds.Load(cmd2);
                }

                foreach (DataRow r in ds.Bang.Rows)
                {
                    string ma = Convert.ToString(r["ID_PHIEU_BAN"]);
                    string sp = r.Table.Columns.Contains("ID_SAN_PHAM") && r["ID_SAN_PHAM"] != DBNull.Value
                        ? Convert.ToString(r["ID_SAN_PHAM"]) : "";
                    if (idSp.Length > 0 && sp != idSp) continue;

                    int sl = 0;
                    try { sl = Convert.ToInt32(r["SO_LUONG"]); } catch { }
                    long giaNhap = 0;
                    try
                    {
                        if (r["DON_GIA_NHAP"] != DBNull.Value) giaNhap = Convert.ToInt64(r["DON_GIA_NHAP"]);
                    }
                    catch { }
                    long thanhTien = 0;
                    try
                    {
                        if (r["THANH_TIEN"] != DBNull.Value) thanhTien = Convert.ToInt64(r["THANH_TIEN"]);
                        else if (r["DON_GIA"] != DBNull.Value) thanhTien = Convert.ToInt64(r["DON_GIA"]) * sl;
                    }
                    catch { }

                    long von = giaNhap * sl;
                    if (!map.ContainsKey(ma))
                        map[ma] = new long[] { 0, 0, 0 };
                    map[ma][0] += von;
                    map[ma][1] = 1;
                    map[ma][2] += thanhTien;
                }
            }
            catch { }
            return map;
        }

        private long LayTongChi(KhoangNgay khoang)
        {
            try
            {
                DataService ds = new DataService();
                OleDbCommand cmd = new OleDbCommand(
                    "SELECT TONG_TIEN, NGAY_CHI FROM PHIEU_CHI WHERE NGAY_CHI >= @tu AND NGAY_CHI < @den");
                cmd.Parameters.Add("tu", OleDbType.Date).Value = khoang.TuNgay;
                cmd.Parameters.Add("den", OleDbType.Date).Value = khoang.DenNgayKhongBaoGom;
                try { ds.Load(cmd); }
                catch
                {
                    ds = new DataService();
                    ds.Load(new OleDbCommand("SELECT TONG_TIEN, NGAY_CHI FROM PHIEU_CHI"));
                }
                long tong = 0;
                foreach (DataRow r in ds.Bang.Rows)
                {
                    try
                    {
                        if (r.Table.Columns.Contains("NGAY_CHI") && r["NGAY_CHI"] != DBNull.Value)
                        {
                            DateTime n = Convert.ToDateTime(r["NGAY_CHI"]).Date;
                            if (!khoang.Chua(n)) continue;
                        }
                        if (r["TONG_TIEN"] != DBNull.Value) tong += Convert.ToInt64(r["TONG_TIEN"]);
                    }
                    catch { }
                }
                return tong;
            }
            catch { return 0; }
        }
    }
}
