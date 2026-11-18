using System;

namespace CuahangNongduoc.BusinessObject
{
    // REFACTOR #65 Collapse Hierarchy – lớp con KhachLe (tạo ở bước Replace Type Code with Subclasses)
    // không thêm trường hay hành vi nào so với KhachHang, nên gộp lại vào lớp cha: khách lẻ = KhachHang.

    /// <summary>Đại lý (LOAI_KH = 1): mua giá sỉ, được ghi nợ.</summary>
    public class DaiLy : KhachHang
    {
        // REFACTOR #60 Push Down Field.
        // Các cột HAN_MUC_CONG_NO, SO_NGAY_CONG_NO, CAP_DAI_LY nằm chung trong bảng KHACH_HANG
        // nhưng chỉ có nghĩa với đại lý, nên chỉ lớp DaiLy giữ chúng (khách lẻ không có).
        public const int SO_NGAY_CONG_NO_MAC_DINH = 30;

        // REFACTOR #11 Move Field – tên các cấp đại lý trước đây là trường KhachHangController.NhanCapDaiLy
        // (tầng điều khiển), nhưng là dữ liệu của đại lý nên chuyển về lớp DaiLy.
        /// <summary>Tên hiển thị của cấp đại lý (CAP_DAI_LY = 1, 2).</summary>
        public static readonly string[] NHAN_CAP = new string[] { "Đại lý cấp 1", "Đại lý cấp 2" };

        private long m_HanMucCongNo;
        private int m_SoNgayCongNo = SO_NGAY_CONG_NO_MAC_DINH;
        private int m_CapDaiLy = 1;

        public DaiLy() { }

        /// <summary>Tổng nợ tối đa được phép (0 = không giới hạn).</summary>
        public long HanMucCongNo
        {
            get { return m_HanMucCongNo; }
            set { m_HanMucCongNo = value; }
        }

        public int SoNgayCongNo
        {
            get { return m_SoNgayCongNo; }
            set { m_SoNgayCongNo = value; }
        }

        /// <summary>Cấp đại lý: 1 hoặc 2.</summary>
        public int CapDaiLy
        {
            get { return m_CapDaiLy; }
            set { m_CapDaiLy = value; }
        }

        public override bool LaDaiLy
        {
            get { return true; }
        }
    }
}
