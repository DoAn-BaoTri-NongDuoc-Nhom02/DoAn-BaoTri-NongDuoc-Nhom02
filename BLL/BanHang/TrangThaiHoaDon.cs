using System;
using CuahangNongduoc.BLL.Service;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BanHang
{
    /// <summary>
    /// Trạng thái của hóa đơn bán: Nháp → Đã lưu → Đã thanh toán, hoặc Đã hủy.
    ///
    /// PATTERN #20 State.
    /// Mỗi trạng thái là một lớp tự quyết định hóa đơn có được sửa / xóa / in không và chuyển sang trạng thái
    /// nào khi Lưu, Thanh toán, Hủy. Form không còn phải so sánh chuỗi "Nháp", "Đã lưu"... rải rác;
    /// chuyển trạng thái sai (ví dụ hủy hóa đơn đã thanh toán) sẽ báo lỗi nghiệp vụ.
    /// Tên trạng thái được lưu ở cột PHIEU_BAN.TRANG_THAI.
    /// </summary>
    public abstract class TrangThaiHoaDon
    {
        public const String NHAP = "Nháp";
        public const String DA_LUU = "Đã lưu";
        public const String DA_THANH_TOAN = "Đã thanh toán";
        public const String DA_HUY = "Đã hủy";

        public abstract String Ten { get; }

        public virtual bool ChoSua { get { return false; } }
        public virtual bool ChoXoa { get { return false; } }
        public virtual bool ChoIn { get { return true; } }

        public virtual TrangThaiHoaDon Luu(PhieuBan phieu)
        {
            throw new NghiepVuException("Không thể lưu hóa đơn đang ở trạng thái \"" + Ten + "\".");
        }

        public virtual TrangThaiHoaDon ThanhToan(PhieuBan phieu)
        {
            throw new NghiepVuException("Không thể thanh toán hóa đơn đang ở trạng thái \"" + Ten + "\".");
        }

        public virtual TrangThaiHoaDon Huy(PhieuBan phieu)
        {
            throw new NghiepVuException("Không thể hủy hóa đơn đang ở trạng thái \"" + Ten + "\".");
        }

        /// <summary>Sau khi lưu: hết nợ là "Đã thanh toán", còn nợ là "Đã lưu".</summary>
        protected static TrangThaiHoaDon TheoCongNo(PhieuBan phieu)
        {
            if (phieu.ConNo <= 0) return new DaThanhToan();
            return new DaLuu();
        }

        /// <summary>Trạng thái ứng với chuỗi lưu trong CSDL (phiếu cũ chưa có trạng thái coi như "Đã lưu").</summary>
        public static TrangThaiHoaDon TuTen(String ten)
        {
            switch ((ten ?? "").Trim())
            {
                case NHAP: return new Nhap();
                case DA_THANH_TOAN: return new DaThanhToan();
                case DA_HUY:
                case "Hủy": return new DaHuy();
                default: return new DaLuu();
            }
        }

        public override String ToString() { return Ten; }
    }

    public class Nhap : TrangThaiHoaDon
    {
        public override String Ten { get { return NHAP; } }
        public override bool ChoSua { get { return true; } }
        public override bool ChoXoa { get { return true; } }
        public override bool ChoIn { get { return false; } }

        public override TrangThaiHoaDon Luu(PhieuBan phieu) { return TheoCongNo(phieu); }
        public override TrangThaiHoaDon Huy(PhieuBan phieu) { return new DaHuy(); }
    }

    public class DaLuu : TrangThaiHoaDon
    {
        public override String Ten { get { return DA_LUU; } }
        public override bool ChoSua { get { return true; } }
        public override bool ChoXoa { get { return true; } }

        public override TrangThaiHoaDon Luu(PhieuBan phieu) { return TheoCongNo(phieu); }

        public override TrangThaiHoaDon ThanhToan(PhieuBan phieu)
        {
            if (phieu.ConNo > 0)
                throw new NghiepVuException("Hóa đơn còn nợ " + phieu.ConNo.ToString("#,##0") + " đ, chưa thể chuyển sang Đã thanh toán.");
            return new DaThanhToan();
        }

        public override TrangThaiHoaDon Huy(PhieuBan phieu) { return new DaHuy(); }
    }

    public class DaThanhToan : TrangThaiHoaDon
    {
        public override String Ten { get { return DA_THANH_TOAN; } }
        // Hóa đơn đã thanh toán không được sửa / xóa / hủy, chỉ được in lại.
    }

    public class DaHuy : TrangThaiHoaDon
    {
        public override String Ten { get { return DA_HUY; } }
        public override bool ChoIn { get { return false; } }
    }
}
