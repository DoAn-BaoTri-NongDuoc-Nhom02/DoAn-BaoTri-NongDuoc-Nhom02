// Kiểm thử phần nghiệp vụ mới (không cần SQL Server): Decorator, Strategy, Builder, Factory Method,
// State, Template Method (tồn kho), Proxy + Singleton + Null Object, băm mật khẩu, KhoangNgay.
// Dữ liệu thống kê lấy từ KetNoiGia (một IKetNoiDuLieu giả - cũng là ví dụ lợi ích của Adapter).
// File này KHÔNG nằm trong CuahangNongduoc.csproj. Cách chạy: tạo Console App (.NET Framework), thêm file này cùng thư mục Entities, BLL, DAL (trừ file *_Snippet.cs) và Utils (PhienDangNhap, BaoCaoCsv, MatKhau, ThamSo, CauHinhKho), rồi chạy.
// các file Entities/*, BLL/BanHang/*, BLL/ThongKe/*, ... (xem danh sách trong MINH_CHUNG.md), rồi chạy.
// Kết quả mong đợi: TRUOT: 0.
using System;
using System.Collections.Generic;
using System.Data;
using CuahangNongduoc;
using CuahangNongduoc.BanHang;
using CuahangNongduoc.BLL.Service;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Entities;
using CuahangNongduoc.ThongKe;

class KetNoiGia : IKetNoiDuLieu
{
    public Dictionary<string, DataTable> Bang = new Dictionary<string, DataTable>();
    public DataTable TruyVan(string sql, IDictionary<string, object> ts)
    {
        foreach (KeyValuePair<string, DataTable> kv in Bang) if (sql.Contains(kv.Key)) return kv.Value.Copy();
        throw new Exception("khong co du lieu gia cho: " + sql);
    }
}

static class KiemThu
{
    static int dat = 0, truot = 0;
    static void Kiem(bool dk, string ten) { if (dk) dat++; else { truot++; Console.WriteLine("TRUOT: " + ten); } }

    static long CongThucCu(long tien, long ck, long phi, decimal vat)
    {
        decimal b = tien - ck + phi; if (b < 0) b = 0;
        return (long)Math.Round(b * (1 + vat / 100m), 0);
    }

    static void Main()
    {
        // 1. Decorator khớp công thức cũ
        Random r = new Random(1);
        bool khop = true;
        for (int i = 0; i < 2000; i++)
        {
            long t = r.Next(0, 5000000), ck = r.Next(0, 3000000), phi = r.Next(0, 500000); decimal vat = r.Next(0, 11);
            ThongTinHoaDon tt = new ThongTinHoaDon(ThongTinHoaDon.GIA_LE); tt.ChietKhau = ck; tt.PhiVanChuyen = phi; tt.VAT = vat;
            if (tt.TinhTongTien(t) != CongThucCu(t, ck, phi, vat)) { khop = false; Console.WriteLine(t + " " + ck + " " + phi + " " + vat); break; }
        }
        Kiem(khop, "Decorator = cong thuc cu (2000 bo so ngau nhien)");

        // 2. Strategy
        DaiLy dl = new DaiLy(); dl.Id = "DL01"; dl.HoTen = "Dai ly"; dl.CapDaiLy = 1;
        KhachHang kl = new KhachHang(); kl.Id = "KH1"; kl.HoTen = "Khach le";
        Kiem(new GiamTheoCapDaiLy().TinhTienGiam(1000000, dl) == 50000, "Giam cap dai ly 1 = 5%");
        Kiem(new GiamTheoCapDaiLy().TinhTienGiam(1000000, kl) == 0, "Khach le khong duoc giam theo cap");
        Kiem(new GiamSoTienCoDinh(2000000).TinhTienGiam(1000000, kl) == 1000000, "Giam khong vuot tien hang");
        Kiem(new GiamTheoPhanTram(10).TinhTienGiam(1234567, kl) == 123457, "Giam 10% lam tron");

        // 3. Builder + Factory Method + Decorator day du
        ThongTinHoaDon them = new ThongTinHoaDon(ThongTinHoaDon.GIA_LE);
        them.ChietKhau = 50000; them.PhiVanChuyen = 30000; them.VAT = 10;
        them.KieuGiam = ChienLuocGiamGia.MA_PHAN_TRAM; them.GiaTriGiam = 5;
        them.GanDichVu(new DichVuPhatSinh[] { new DichVuPhatSinh("Phun thuoc ho", 100000), new DichVuPhatSinh("Boc xep", 20000) });
        PhieuBan pb = new HoaDonBuilder(new BanLeService(), "105").ChoKhachHang(kl).NgayBan(new DateTime(2026, 11, 3))
            .TienHang(1000000).DaTra(500000).ThongTinThem(them).Build();
        // (1.000.000 - 50.000 - 50.000 + 30.000 + 120.000) * 1,1 = 1.155.000
        Kiem(pb.TongTien == 1155000, "Tong tien hoa don day du = 1.155.000 (thuc te " + pb.TongTien + ")");
        Kiem(pb.GiamGia == 50000 && pb.TienDichVu == 120000 && pb.ConNo == 655000, "Giam gia / dich vu / con no");
        Kiem(pb.TrangThai == TrangThaiHoaDon.NHAP, "Hoa don moi o trang thai Nhap");
        Kiem(pb.GetType() == typeof(PhieuBan), "BanLeService tao PhieuBan");
        Kiem(pb.DienGiaiTien.Count == 6, "Dien giai co 6 dong (thuc te " + pb.DienGiaiTien.Count + ")");
        ThongTinHoaDonSi thSi = new ThongTinHoaDonSi(); thSi.HanMucNo = 7000000;
        PhieuBan pbSi = new HoaDonBuilder(new BanSiService(), "106").ChoKhachHang(dl).TienHang(200000).ThongTinThem(thSi).Build();
        Kiem(pbSi is PhieuBanSi && ((PhieuBanSi)pbSi).HanMucNo == 7000000, "BanSiService tao PhieuBanSi co han muc");
        bool loi = false; try { new HoaDonBuilder(new BanLeService(), "107").ChoKhachHang(kl).Build(); } catch (NghiepVuException) { loi = true; }
        Kiem(loi, "Builder chan hoa don khong co dong hang");
        loi = false; try { new HoaDonBuilder(new BanLeService(), "108").TienHang(1000).Build(); } catch (NghiepVuException) { loi = true; }
        Kiem(loi, "Builder chan hoa don chua chon khach");

        // 4. State
        TrangThaiHoaDon st = TrangThaiHoaDon.TuTen(pb.TrangThai).Luu(pb);
        Kiem(st.Ten == TrangThaiHoaDon.DA_LUU, "Nhap + con no -> Da luu");
        pb.DaTra = pb.TongTien; pb.ConNo = 0;
        Kiem(st.ThanhToan(pb).Ten == TrangThaiHoaDon.DA_THANH_TOAN, "Da luu + het no -> Da thanh toan");
        loi = false; try { new DaThanhToan().Huy(pb); } catch (NghiepVuException) { loi = true; }
        Kiem(loi && !new DaThanhToan().ChoSua && !new DaThanhToan().ChoXoa, "Da thanh toan khong sua/xoa/huy duoc");
        Kiem(TrangThaiHoaDon.TuTen("Hủy").Ten == TrangThaiHoaDon.DA_HUY && TrangThaiHoaDon.TuTen(null).Ten == TrangThaiHoaDon.DA_LUU, "TuTen chuoi cu");

        // 5. Template Method: ton kho
        KetNoiGia kn = new KetNoiGia();
        DataTable lo = new DataTable();
        foreach (string c in new string[] { "MA_SP", "TEN_SP", "DVT" }) lo.Columns.Add(c, typeof(string));
        lo.Columns.Add("NGAY_NHAP", typeof(DateTime)); lo.Columns.Add("CON_LAI", typeof(int)); lo.Columns.Add("DA_BAN", typeof(int));
        lo.Rows.Add("SP1", "Thuoc A", "Chai", new DateTime(2026, 9, 1), 10, 40);   // nhap truoc ky 50
        lo.Rows.Add("SP1", "Thuoc A", "Chai", new DateTime(2026, 10, 10), 20, 10); // nhap trong ky 30
        lo.Rows.Add("SP1", "Thuoc A", "Chai", new DateTime(2026, 11, 5), 5, 0);    // nhap sau ky 5
        DataTable ban = new DataTable();
        ban.Columns.Add("MA_SP", typeof(string)); ban.Columns.Add("NGAY_BAN", typeof(DateTime)); ban.Columns.Add("SO_LUONG", typeof(int));
        ban.Rows.Add("SP1", new DateTime(2026, 10, 15), 25);  // trong ky
        ban.Rows.Add("SP1", new DateTime(2026, 11, 2), 15);   // sau ky (ky ket thuc 31/10)
        kn.Bang["FROM MA_SAN_PHAM MSP INNER JOIN"] = lo;
        kn.Bang["WHERE PB.NGAY_BAN >= @tu"] = ban;
        DataTable kq = new ThongKeTonKho(kn).ThucHien(new KhoangNgay(new DateTime(2026, 10, 1), new DateTime(2026, 10, 31)));
        // ton hien tai 35; ton cuoi ky = 35 - 5 (nhap sau) + 15 (xuat sau) = 45; ton dau = 45 - 30 + 25 = 40
        DataRow d = kq.Rows[0];
        Kiem(Convert.ToInt64(d["Tồn cuối kỳ"]) == 45 && Convert.ToInt64(d["Tồn đầu kỳ"]) == 40
            && Convert.ToInt64(d["Nhập trong kỳ"]) == 30 && Convert.ToInt64(d["Xuất trong kỳ"]) == 25, "Ton kho dau/nhap/xuat/cuoi ky");
        Kiem(kq.Rows.Count == 2 && Convert.ToString(kq.Rows[1]["Mã SP"]) == "TỔNG CỘNG", "Template them dong tong cong");

        // 6. Proxy: nhan vien ban hang chi thay cua minh
        DataTable nv = new DataTable();
        nv.Columns.Add("ID_NGUOI_DUNG", typeof(int)); nv.Columns.Add("NHAN_VIEN", typeof(string));
        nv.Columns.Add("SO_HD", typeof(int)); nv.Columns.Add("GIAM_GIA", typeof(decimal));
        nv.Rows.Add(5, "Lan", 3, 150000m); nv.Rows.Add(7, "Hung", 2, 90000m);
        kn.Bang.Clear();
        kn.Bang["GROUP BY PB.ID_NGUOI_DUNG"] = nv;
        NguoiDung lan = new NguoiDung(); lan.ID = 5; lan.TenDangNhap = "lan"; lan.HoTen = "Lan"; lan.IDVaiTro = LoaiVaiTro.MA_BAN_HANG;
        PhienDangNhap.BatDau(lan);
        Kiem(object.ReferenceEquals(PhienDangNhap.HienTai, PhienDangNhap.HienTai), "Singleton chi co mot phien");
        DataTable cuaLan = new ThongKeBaoVe(new ThongKeGiamGiaTheoNhanVien(kn)).ThucHien(new KhoangNgay(DateTime.Today, DateTime.Today));
        Kiem(cuaLan.Rows.Count == 2 && Convert.ToInt32(cuaLan.Rows[0]["ID_NGUOI_DUNG"]) == 5
            && Convert.ToDecimal(cuaLan.Rows[1]["Giảm giá"]) == 150000m, "Proxy: nhan vien chi thay dong cua minh, tong tinh lai");
        NguoiDung kt = new NguoiDung(); kt.ID = 9; kt.HoTen = "Ke toan"; kt.IDVaiTro = LoaiVaiTro.MA_KE_TOAN;
        PhienDangNhap.BatDau(kt);
        DataTable tatCa = new ThongKeBaoVe(new ThongKeGiamGiaTheoNhanVien(kn)).ThucHien(new KhoangNgay(DateTime.Today, DateTime.Today));
        Kiem(tatCa.Rows.Count == 3, "Proxy: ke toan thay tat ca");
        PhienDangNhap.KetThuc();
        loi = false; try { new ThongKeBaoVe(new ThongKeTonKho(kn)).ThucHien(new KhoangNgay(DateTime.Today, DateTime.Today)); } catch (NghiepVuException) { loi = true; }
        Kiem(loi && !PhienDangNhap.NguoiDungHienTai.LaNguoiDungThat, "Chua dang nhap: Null Object + Proxy chan thong ke");

        // 7. Mat khau
        string h = MatKhau.Bam("123");
        Kiem(MatKhau.DaBam(h) && MatKhau.Khop("123", h) && !MatKhau.Khop("1234", h) && MatKhau.Khop("123", "123") && MatKhau.Bam(h) == h, "Bam / kiem tra mat khau");

        // 8. KhoangNgay
        loi = false; try { new KhoangNgay(new DateTime(2026, 2, 1), new DateTime(2026, 1, 1)); } catch (ArgumentException) { loi = true; }
        Kiem(loi, "KhoangNgay chan tu > den");


        // 9. Abstract Factory
        IKenhBanFactory nmSi = new KenhBanSiFactory();
        Kiem(nmSi.TaoDichVuBan() is BanSiService && nmSi.TaoThongTinHoaDon() is ThongTinHoaDonSi, "Abstract Factory: bo doi tuong ban si khop nhau");
        Kiem(new KenhBanLeFactory().TaoThongTinHoaDon().GetType() == typeof(ThongTinHoaDon), "Abstract Factory: ban le");

        // 10. Prototype + Memento + Command
        ThongTinHoaDonSi goc = new ThongTinHoaDonSi(); goc.ChietKhau = 1000; goc.HanMucNo = 5000; goc.GanDichVu(new DichVuPhatSinh[] { new DichVuPhatSinh("Giao hang", 30000) });
        ThongTinHoaDonSi banSao = (ThongTinHoaDonSi)goc.SaoChep(); banSao.ChietKhau = 2000;
        Kiem(banSao.HanMucNo == 5000 && banSao.DichVu.Count == 1 && goc.ChietKhau == 1000, "Prototype: sao chep doc lap, giu truong lop con");
        LichSuLenh ls = new LichSuLenh();
        ls.ThucHien(new LenhCapNhatThongTin(goc, banSao));
        Kiem(goc.ChietKhau == 2000, "Command: ap thay doi");
        ls.HoanTac();
        Kiem(goc.ChietKhau == 1000 && goc.HanMucNo == 5000 && !ls.CoTheHoanTac, "Command + Memento: hoan tac ve trang thai cu");

        // 11. Composite
        GoiDichVu goi = GoiDichVu.GoiChamSocLua(); goi.Them(new GoiDichVu("Goi con", new IDichVu[] { new DichVuPhatSinh("Boc xep", 20000) }));
        int soLe = 0; foreach (DichVuPhatSinh x in goi.CacDichVuLe()) soLe++;
        Kiem(goi.SoTien == 220000 && soLe == 3, "Composite: goi long goi, tong tien va lam phang");

        // 12. Iterator
        int soNgay = 0; DateTime cuoi = DateTime.MinValue;
        foreach (DateTime ngay in new KhoangNgay(new DateTime(2026, 10, 30), new DateTime(2026, 11, 2))) { soNgay++; cuoi = ngay; }
        Kiem(soNgay == 4 && cuoi == new DateTime(2026, 11, 2), "Iterator: duyet tung ngay qua thang");

        // 13. Visitor
        ThongTinHoaDon tv = new ThongTinHoaDon(ThongTinHoaDon.GIA_LE); tv.ChietKhau = 50000; tv.PhiVanChuyen = 30000; tv.VAT = 10;
        TongHopKhoanVisitor th = new TongHopKhoanVisitor(); tv.BangTinhTien(1000000, null).ChapNhan(th);
        Kiem(th.TongGiam == 50000 && th.TongThuThem == 30000 + 98000, "Visitor: tong khoan giam / thu them (" + th.TongGiam + "/" + th.TongThuThem + ")");

        // 14. Interpreter
        DaiLy dlKM = new DaiLy(); dlKM.Id = "DL9"; dlKM.CapDaiLy = 2;
        NguCanhKhuyenMai nc = new NguCanhKhuyenMai(12000000, dlKM, new DateTime(2026, 11, 4));
        Kiem(DieuKienKhuyenMai.PhanTich("TIEN_HANG >= 10000000 VA LA_DAI_LY").DanhGia(nc), "Interpreter: VA + bien dung");
        Kiem(!DieuKienKhuyenMai.PhanTich("TIEN_HANG<5000000 HOAC KHONG LA_DAI_LY").DanhGia(nc), "Interpreter: HOAC + KHONG, khong co dau cach");
        Kiem(DieuKienKhuyenMai.PhanTich("CAP_DAI_LY <> 1 VA THANG >= 11").DanhGia(nc), "Interpreter: <> va >=");
        GiamTheoKhuyenMai km = new GiamTheoKhuyenMai(ChuongTrinhKhuyenMai.DanhSachMacDinh(), new DateTime(2026, 11, 4));
        Kiem(km.TinhTienGiam(12000000, dlKM) == 600000 && km.TinhTienGiam(12000000, new KhachHang()) == 360000 && km.TinhTienGiam(500000, null) == 0, "Interpreter: chon chuong trinh % cao nhat");
        loi = false; try { DieuKienKhuyenMai.PhanTich("TIEN_HANG >= VA"); } catch (FormatException) { loi = true; }
        Kiem(loi, "Interpreter: bao loi cau sai");

        Console.WriteLine("DAT: " + dat + "  TRUOT: " + truot);
    }
}
