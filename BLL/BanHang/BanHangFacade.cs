using System;
using System.Data;
using CuahangNongduoc.BLL.Service;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.BanHang
{
    /// <summary>
    /// Một cửa duy nhất để lưu hóa đơn bán.
    ///
    /// PATTERN #10 Facade.
    /// Lưu một hóa đơn cần nhiều bước trên nhiều lớp: chạy chuỗi kiểm tra (Chain of Responsibility), chuyển
    /// trạng thái (State), ghi dòng PHIEU_BAN qua PhieuBanController, cập nhật số phiếu trong THAM_SO,
    /// lưu chi tiết qua ChiTietPhieuBanController, lưu dịch vụ phụ, rồi báo sự kiện (Observer).
    /// Trước đây frmBanLe và frmBanSi mỗi form tự làm (và chép lại) các bước này; nay chỉ gọi LuuHoaDonMoi().
    /// </summary>
    public class BanHangFacade
    {
        private readonly PhieuBanController m_PhieuBan;
        private readonly ChiTietPhieuBanController m_ChiTiet;
        private readonly DichVuPhieuBanFactory m_DichVu = new DichVuPhieuBanFactory();
        private readonly MatXichKiemTra m_KiemTra;

        /// <param name="phieuBan">Controller đang giữ bảng phiếu bán của form (để lưới trên form thấy dòng mới).</param>
        /// <param name="chiTiet">Controller đang giữ các dòng chi tiết đã nhập trên form.</param>
        public BanHangFacade(PhieuBanController phieuBan, ChiTietPhieuBanController chiTiet)
            : this(phieuBan, chiTiet, MatXichKiemTra.ChuoiMacDinh())
        {
        }

        /// <param name="chuoiKiemTra">Chuỗi kiểm tra của kênh bán (lấy từ IKenhBanFactory).</param>
        public BanHangFacade(PhieuBanController phieuBan, ChiTietPhieuBanController chiTiet, MatXichKiemTra chuoiKiemTra)
        {
            m_KiemTra = chuoiKiemTra ?? MatXichKiemTra.ChuoiMacDinh();
            if (phieuBan == null) throw new ArgumentNullException("phieuBan");
            if (chiTiet == null) throw new ArgumentNullException("chiTiet");
            m_PhieuBan = phieuBan;
            m_ChiTiet = chiTiet;
        }

        /// <summary>Kiểm tra rồi lưu hóa đơn mới; sai luật nghiệp vụ thì ném NghiepVuException và chưa ghi gì.</summary>
        public void LuuHoaDonMoi(PhieuBan phieu)
        {
            // PATTERN #13 Chain of Responsibility (nơi dùng) – chuyển hóa đơn qua chuỗi mắt xích kiểm tra.
            m_KiemTra.KiemTra(phieu);

            // PATTERN #20 State (nơi dùng) – trạng thái hiện tại quyết định trạng thái mới khi Lưu.
            phieu.TrangThai = TrangThaiHoaDon.TuTen(phieu.TrangThai).Luu(phieu).Ten;

            DataRow dong = m_PhieuBan.NewRow();
            GhiDongPhieu(dong, phieu);
            m_PhieuBan.Add(dong);
            CapNhatSoPhieu(phieu.Id);
            m_PhieuBan.Save(phieu.NguoiLap);

            m_ChiTiet.Save();
            if (phieu.DichVu.Count > 0)
                m_DichVu.Luu(phieu.Id, phieu.DichVu);

            // PATTERN #19 Observer (nơi dùng) – báo cho mọi bên nghe.
            SuKienHoaDon.PhatHoaDonDaLuu(phieu);
        }

        private static void GhiDongPhieu(DataRow dong, PhieuBan phieu)
        {
            dong["ID"] = phieu.Id;
            dong["ID_KHACH_HANG"] = phieu.KhachHang.Id;
            dong["NGAY_BAN"] = phieu.NgayBan;
            dong["TONG_TIEN"] = phieu.TongTien;
            dong["DA_TRA"] = phieu.DaTra;
            dong["CON_NO"] = phieu.ConNo;

            // Các cột của yêu cầu 4 (có sau khi chạy Database/CapNhat_YeuCau4.sql)
            GhiNeuCo(dong, "TIEN_HANG", phieu.TienHang);
            GhiNeuCo(dong, "CHIET_KHAU", phieu.ChietKhau);
            GhiNeuCo(dong, "GIAM_GIA", phieu.GiamGia);
            GhiNeuCo(dong, "KIEU_GIAM", phieu.KieuGiam);
            GhiNeuCo(dong, "PHI_VAN_CHUYEN", phieu.PhiVanChuyen);
            GhiNeuCo(dong, "TIEN_DICH_VU", phieu.TienDichVu);
            GhiNeuCo(dong, "VAT", phieu.VAT);
            GhiNeuCo(dong, "TRANG_THAI", phieu.TrangThai);
            GhiNeuCo(dong, "GHI_CHU", phieu.GhiChu);
            GhiNeuCo(dong, "SO_HD_VAT", phieu.SoHoaDonVAT);
            GhiNeuCo(dong, "HINH_THUC_TT", phieu.HinhThucThanhToan);
            if (phieu.NguoiLap != null && phieu.NguoiLap.LaNguoiDungThat)
            {
                GhiNeuCo(dong, "NHAN_VIEN", phieu.NguoiLap.HoTen);
                GhiNeuCo(dong, "NGUOI_TAO", phieu.NguoiLap.TenDangNhap);
            }

            PhieuBanSi phieuSi = phieu as PhieuBanSi;
            if (phieuSi != null)
            {
                GhiNeuCo(dong, "DIEU_KHOAN_TT", phieuSi.DieuKhoanThanhToan);
                GhiNeuCo(dong, "HAN_MUC_NO", phieuSi.HanMucNo);
            }
        }

        private static void GhiNeuCo(DataRow dong, String cot, object giaTri)
        {
            if (dong.Table.Columns.Contains(cot))
                dong[cot] = giaTri ?? DBNull.Value;
        }

        private static void CapNhatSoPhieu(String maPhieu)
        {
            if (!ThamSo.LaSoNguyen(maPhieu)) return;
            long so = Convert.ToInt64(maPhieu);
            if (so >= ThamSo.LayMa(LoaiChungTu.PhieuBan))
                ThamSo.GanMa(LoaiChungTu.PhieuBan, so + 1);
        }
    }
}
