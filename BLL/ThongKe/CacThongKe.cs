using System;
using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BanHang;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.ThongKe
{
    // Bốn thống kê "từ ngày đến ngày" của yêu cầu 4. Mỗi lớp chỉ viết bước TruyVan (Template Method).

    /// <summary>Tồn kho từng sản phẩm: tồn đầu kỳ, nhập trong kỳ, xuất trong kỳ, tồn cuối kỳ.</summary>
    public class ThongKeTonKho : ThongKeTheoKhoangNgay
    {
        public ThongKeTonKho(IKetNoiDuLieu ketNoi) : base(ketNoi) { }
        public ThongKeTonKho() : this(null) { }

        public override String TieuDe { get { return "Thống kê tồn kho hàng hóa"; } }

        protected override DataTable TruyVan(KhoangNgay khoang)
        {
            // Bảng lô chỉ giữ số lượng CÒN LẠI, nên số nhập ban đầu của lô = còn lại + đã bán từ lô đó.
            DataTable lo = KetNoi.TruyVan(
                "SELECT SP.ID AS MA_SP, SP.TEN_SAN_PHAM AS TEN_SP, DVT.TEN_DON_VI AS DVT, MSP.NGAY_NHAP, " +
                "MSP.SO_LUONG AS CON_LAI, " +
                "ISNULL((SELECT SUM(CT.SO_LUONG) FROM CHI_TIET_PHIEU_BAN CT WHERE CT.ID_MA_SAN_PHAM = MSP.ID), 0) AS DA_BAN " +
                "FROM MA_SAN_PHAM MSP INNER JOIN SAN_PHAM SP ON MSP.ID_SAN_PHAM = SP.ID " +
                "LEFT JOIN DON_VI_TINH DVT ON SP.ID_DON_VI_TINH = DVT.ID", null);

            Dictionary<String, object> ts = new Dictionary<String, object>();
            ts["tu"] = khoang.TuNgay;
            DataTable ban = KetNoi.TruyVan(
                "SELECT MSP.ID_SAN_PHAM AS MA_SP, PB.NGAY_BAN, CT.SO_LUONG " +
                "FROM CHI_TIET_PHIEU_BAN CT INNER JOIN PHIEU_BAN PB ON CT.ID_PHIEU_BAN = PB.ID " +
                "INNER JOIN MA_SAN_PHAM MSP ON CT.ID_MA_SAN_PHAM = MSP.ID " +
                "WHERE PB.NGAY_BAN >= @tu", ts);

            DataTable kq = new DataTable();
            kq.Columns.Add("Mã SP", typeof(String));
            kq.Columns.Add("Tên sản phẩm", typeof(String));
            kq.Columns.Add("ĐVT", typeof(String));
            kq.Columns.Add("Tồn đầu kỳ", typeof(long));
            kq.Columns.Add("Nhập trong kỳ", typeof(long));
            kq.Columns.Add("Xuất trong kỳ", typeof(long));
            kq.Columns.Add("Tồn cuối kỳ", typeof(long));

            // [0] tồn hiện tại, [1] nhập trong kỳ, [2] nhập sau kỳ, [3] xuất trong kỳ, [4] xuất sau kỳ
            Dictionary<String, long[]> so = new Dictionary<String, long[]>();
            Dictionary<String, DataRow> dong = new Dictionary<String, DataRow>();
            foreach (DataRow r in lo.Rows)
            {
                String ma = DuLieu.LayChuoi(r["MA_SP"]);
                if (!so.ContainsKey(ma))
                {
                    so[ma] = new long[5];
                    DataRow d = kq.NewRow();
                    d["Mã SP"] = ma;
                    d["Tên sản phẩm"] = DuLieu.LayChuoi(r["TEN_SP"]);
                    d["ĐVT"] = DuLieu.LayChuoi(r["DVT"]);
                    dong[ma] = d;
                }
                long conLai = DuLieu.LaySoLon(r["CON_LAI"]);
                long banDau = conLai + DuLieu.LaySoLon(r["DA_BAN"]);
                DateTime ngayNhap = DuLieu.LayNgay(r["NGAY_NHAP"], DateTime.MinValue).Date;
                so[ma][0] += conLai;
                if (khoang.Chua(ngayNhap)) so[ma][1] += banDau;
                else if (ngayNhap > khoang.DenNgay) so[ma][2] += banDau;
            }
            foreach (DataRow r in ban.Rows)
            {
                String ma = DuLieu.LayChuoi(r["MA_SP"]);
                if (!so.ContainsKey(ma)) continue;
                DateTime ngay = DuLieu.LayNgay(r["NGAY_BAN"], DateTime.MinValue);
                long sl = DuLieu.LaySoLon(r["SO_LUONG"]);
                if (khoang.Chua(ngay)) so[ma][3] += sl;
                else if (ngay.Date > khoang.DenNgay) so[ma][4] += sl;
            }

            foreach (KeyValuePair<String, long[]> kv in so)
            {
                long[] s = kv.Value;
                long tonCuoi = s[0] - s[2] + s[4];
                long tonDau = tonCuoi - s[1] + s[3];
                DataRow d = dong[kv.Key];
                d["Tồn đầu kỳ"] = tonDau;
                d["Nhập trong kỳ"] = s[1];
                d["Xuất trong kỳ"] = s[3];
                d["Tồn cuối kỳ"] = tonCuoi;
                kq.Rows.Add(d);
            }
            return kq;
        }
    }

    /// <summary>Chi phí vận chuyển và từng loại dịch vụ phụ đã thu trên hóa đơn trong kỳ.</summary>
    public class ThongKeChiPhiVanChuyenDichVu : ThongKeTheoKhoangNgay
    {
        public ThongKeChiPhiVanChuyenDichVu(IKetNoiDuLieu ketNoi) : base(ketNoi) { }
        public ThongKeChiPhiVanChuyenDichVu() : this(null) { }

        public override String TieuDe { get { return "Thống kê chi phí vận chuyển và dịch vụ phụ"; } }

        protected override DataTable TruyVan(KhoangNgay khoang)
        {
            DataTable phi = KetNoi.TruyVan(
                "SELECT COUNT(*) AS SO_HD, ISNULL(SUM(PHI_VAN_CHUYEN), 0) AS TONG FROM PHIEU_BAN " +
                "WHERE NGAY_BAN >= @tu AND NGAY_BAN < @den AND PHI_VAN_CHUYEN > 0", ThamSoKhoangNgay(khoang));
            DataTable dichVu = KetNoi.TruyVan(
                "SELECT DV.TEN_DICH_VU AS KHOAN, COUNT(DISTINCT DV.ID_PHIEU_BAN) AS SO_HD, SUM(DV.SO_TIEN) AS TONG " +
                "FROM CT_DICH_VU_PHIEU_BAN DV INNER JOIN PHIEU_BAN PB ON DV.ID_PHIEU_BAN = PB.ID " +
                "WHERE PB.NGAY_BAN >= @tu AND PB.NGAY_BAN < @den " +
                "GROUP BY DV.TEN_DICH_VU ORDER BY SUM(DV.SO_TIEN) DESC", ThamSoKhoangNgay(khoang));

            DataTable kq = new DataTable();
            kq.Columns.Add("Khoản", typeof(String));
            kq.Columns.Add("Số hóa đơn", typeof(long));
            kq.Columns.Add("Tổng tiền", typeof(long));
            if (phi.Rows.Count > 0)
                kq.Rows.Add("Phí vận chuyển", DuLieu.LaySoLon(phi.Rows[0]["SO_HD"]), DuLieu.LaySoLon(phi.Rows[0]["TONG"]));
            foreach (DataRow r in dichVu.Rows)
                kq.Rows.Add("Dịch vụ: " + DuLieu.LayChuoi(r["KHOAN"]), DuLieu.LaySoLon(r["SO_HD"]), DuLieu.LaySoLon(r["TONG"]));
            return kq;
        }
    }

    /// <summary>Các hóa đơn có chiết khấu / giảm giá / khuyến mãi trong kỳ.</summary>
    public class ThongKeGiamGiaKhuyenMai : ThongKeTheoKhoangNgay
    {
        public ThongKeGiamGiaKhuyenMai(IKetNoiDuLieu ketNoi) : base(ketNoi) { }
        public ThongKeGiamGiaKhuyenMai() : this(null) { }

        public override String TieuDe { get { return "Thống kê hóa đơn giảm giá, khuyến mãi"; } }

        protected override String[][] TenCot
        {
            get
            {
                return new String[][] {
                    new String[] { "MA_PHIEU", "Mã phiếu" }, new String[] { "NGAY", "Ngày bán" },
                    new String[] { "KHACH_HANG", "Khách hàng" }, new String[] { "TIEN_HANG", "Tiền hàng" },
                    new String[] { "CHIET_KHAU", "Chiết khấu" }, new String[] { "GIAM_GIA", "Giảm giá" },
                    new String[] { "KIEU_GIAM", "Cách giảm" }, new String[] { "TONG_TIEN", "Tổng tiền" },
                    new String[] { "NHAN_VIEN", "Nhân viên" } };
            }
        }

        protected override DataTable TruyVan(KhoangNgay khoang)
        {
            DataTable bang = KetNoi.TruyVan(
                "SELECT PB.ID AS MA_PHIEU, PB.NGAY_BAN AS NGAY, KH.HO_TEN AS KHACH_HANG, PB.TIEN_HANG, PB.CHIET_KHAU, " +
                "PB.GIAM_GIA, PB.KIEU_GIAM, PB.TONG_TIEN, ND.HO_TEN AS NHAN_VIEN, PB.ID_NGUOI_DUNG " +
                "FROM PHIEU_BAN PB LEFT JOIN KHACH_HANG KH ON PB.ID_KHACH_HANG = KH.ID " +
                "LEFT JOIN NGUOI_DUNG ND ON PB.ID_NGUOI_DUNG = ND.ID " +
                "WHERE PB.NGAY_BAN >= @tu AND PB.NGAY_BAN < @den AND (PB.CHIET_KHAU > 0 OR PB.GIAM_GIA > 0) " +
                "ORDER BY PB.NGAY_BAN", ThamSoKhoangNgay(khoang));
            foreach (DataRow r in bang.Rows)
            {
                String ma = DuLieu.LayChuoi(r["KIEU_GIAM"]);
                r["KIEU_GIAM"] = TenCachGiam(ma);
            }
            return bang;
        }

        private static String TenCachGiam(String ma)
        {
            foreach (String[] lc in ChienLuocGiamGia.DanhSachLuaChon())
                if (lc[0] == ma) return lc[1];
            return ma.Length == 0 ? "Chỉ chiết khấu" : ma;
        }
    }

    /// <summary>Hóa đơn giảm giá / khuyến mãi gom theo nhân viên lập.</summary>
    public class ThongKeGiamGiaTheoNhanVien : ThongKeTheoKhoangNgay
    {
        public ThongKeGiamGiaTheoNhanVien(IKetNoiDuLieu ketNoi) : base(ketNoi) { }
        public ThongKeGiamGiaTheoNhanVien() : this(null) { }

        public override String TieuDe { get { return "Thống kê hóa đơn giảm giá theo nhân viên"; } }

        protected override String[][] TenCot
        {
            get
            {
                return new String[][] {
                    new String[] { "NHAN_VIEN", "Nhân viên" }, new String[] { "SO_HD", "Số hóa đơn" },
                    new String[] { "TIEN_HANG", "Tiền hàng" }, new String[] { "CHIET_KHAU", "Chiết khấu" },
                    new String[] { "GIAM_GIA", "Giảm giá" }, new String[] { "TONG_TIEN", "Tổng tiền" } };
            }
        }

        protected override DataTable TruyVan(KhoangNgay khoang)
        {
            return KetNoi.TruyVan(
                "SELECT PB.ID_NGUOI_DUNG, ISNULL(ND.HO_TEN, N'(không rõ)') AS NHAN_VIEN, COUNT(*) AS SO_HD, " +
                "SUM(PB.TIEN_HANG) AS TIEN_HANG, SUM(PB.CHIET_KHAU) AS CHIET_KHAU, SUM(PB.GIAM_GIA) AS GIAM_GIA, " +
                "SUM(PB.TONG_TIEN) AS TONG_TIEN " +
                "FROM PHIEU_BAN PB LEFT JOIN NGUOI_DUNG ND ON PB.ID_NGUOI_DUNG = ND.ID " +
                "WHERE PB.NGAY_BAN >= @tu AND PB.NGAY_BAN < @den AND (PB.CHIET_KHAU > 0 OR PB.GIAM_GIA > 0) " +
                "GROUP BY PB.ID_NGUOI_DUNG, ND.HO_TEN ORDER BY SUM(PB.GIAM_GIA) + SUM(PB.CHIET_KHAU) DESC",
                ThamSoKhoangNgay(khoang));
        }
    }
}
