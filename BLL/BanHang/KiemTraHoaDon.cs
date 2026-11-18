using System;
using System.Collections.Generic;
using CuahangNongduoc.BLL.Service;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc.BanHang
{
    /// <summary>
    /// Một mắt xích kiểm tra hóa đơn trước khi lưu.
    ///
    /// PATTERN #13 Chain of Responsibility.
    /// Các luật kiểm tra (mã phiếu trùng, vượt hạn mức nợ của đại lý, giảm giá vượt mức cho phép...) là các
    /// mắt xích nối thành chuỗi. Hóa đơn được chuyển lần lượt qua từng mắt xích; mắt xích nào thấy sai thì
    /// dừng chuỗi bằng NghiepVuException. Thêm / bớt / đổi thứ tự luật chỉ cần sửa chỗ nối chuỗi.
    /// (Kiểm tra tồn kho theo lô vẫn làm ở form, vì cần các dòng chưa lưu trên lưới.)
    /// </summary>
    public abstract class MatXichKiemTra
    {
        private MatXichKiemTra m_Tiep;

        /// <summary>Nối mắt xích kế tiếp; trả về mắt xích vừa nối để nối tiếp được.</summary>
        public MatXichKiemTra Noi(MatXichKiemTra tiep)
        {
            m_Tiep = tiep;
            return tiep;
        }

        public void KiemTra(PhieuBan phieu)
        {
            KiemTraRieng(phieu);
            if (m_Tiep != null)
                m_Tiep.KiemTra(phieu);
        }

        /// <summary>Kiểm tra của riêng mắt xích này; sai thì ném NghiepVuException.</summary>
        protected abstract void KiemTraRieng(PhieuBan phieu);

        /// <summary>Chuỗi kiểm tra dùng khi lưu hóa đơn mới.</summary>
        public static MatXichKiemTra ChuoiMacDinh()
        {
            MatXichKiemTra dau = new KiemTraMaPhieuChuaCo();
            dau.Noi(new KiemTraHanMucNo())
               .Noi(new KiemTraMucGiamGia());
            return dau;
        }
    }

    /// <summary>Mã phiếu chưa được dùng.</summary>
    public class KiemTraMaPhieuChuaCo : MatXichKiemTra
    {
        protected override void KiemTraRieng(PhieuBan phieu)
        {
            if (new PhieuBanController().LayPhieuBan(phieu.Id) != null)
                throw new NghiepVuException("Mã phiếu bán \"" + phieu.Id + "\" đã tồn tại.");
        }
    }

    /// <summary>Đại lý có hạn mức công nợ: nợ cũ + nợ của hóa đơn này không được vượt hạn mức.</summary>
    public class KiemTraHanMucNo : MatXichKiemTra
    {
        protected override void KiemTraRieng(PhieuBan phieu)
        {
            DaiLy daiLy = phieu.KhachHang as DaiLy;
            if (daiLy == null || daiLy.HanMucCongNo <= 0 || phieu.ConNo <= 0)
                return;

            Dictionary<string, long> congNo, tongMua;
            new KhachHangController().LayThongKeKhachHang(out congNo, out tongMua);
            long noCu;
            if (!congNo.TryGetValue(daiLy.Id, out noCu)) noCu = 0;

            if (noCu + phieu.ConNo > daiLy.HanMucCongNo)
                throw new NghiepVuException("Đại lý \"" + daiLy.HoTen + "\" sẽ vượt hạn mức công nợ: nợ cũ "
                    + noCu.ToString("#,##0") + " + nợ mới " + phieu.ConNo.ToString("#,##0")
                    + " > hạn mức " + daiLy.HanMucCongNo.ToString("#,##0") + " đ.");
        }
    }

    /// <summary>Nhân viên (không phải Admin) chỉ được giảm tổng cộng tối đa 10% tiền hàng.</summary>
    public class KiemTraMucGiamGia : MatXichKiemTra
    {
        public const decimal MUC_GIAM_TOI_DA_NHAN_VIEN = 10;

        protected override void KiemTraRieng(PhieuBan phieu)
        {
            if (phieu.TienHang <= 0) return;
            long tongGiam = phieu.ChietKhau + phieu.GiamGia;
            decimal phanTram = tongGiam * 100m / phieu.TienHang;
            if (phanTram <= MUC_GIAM_TOI_DA_NHAN_VIEN) return;
            // PATTERN #5 Singleton (nơi dùng) – hỏi quyền của phiên đăng nhập duy nhất.
            if (PhienDangNhap.HienTai.Quyen.DuocPhep(ChucNang.GIAM_GIA_KHONG_GIOI_HAN)) return;

            throw new NghiepVuException("Tổng chiết khấu + giảm giá là " + phanTram.ToString("0.#")
                + "% tiền hàng, vượt mức " + MUC_GIAM_TOI_DA_NHAN_VIEN + "% nhân viên được phép. Cần Admin lập hóa đơn này.");
        }
    }
}
