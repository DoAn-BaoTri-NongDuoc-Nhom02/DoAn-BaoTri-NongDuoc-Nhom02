using System;

namespace CuahangNongduoc.BusinessObject
{
    /// <summary>Phiếu bán sỉ (bán cho đại lý): có thêm điều khoản thanh toán và hạn mức nợ.</summary>
    public class PhieuBanSi : PhieuBan
    {
        private String m_DieuKhoanThanhToan = "Trả ngay";
        private long m_HanMucNo;

        public PhieuBanSi(String id) : base(id) { }

        public String DieuKhoanThanhToan
        {
            get { return m_DieuKhoanThanhToan; }
            set { m_DieuKhoanThanhToan = value ?? "Trả ngay"; }
        }

        public long HanMucNo
        {
            get { return m_HanMucNo; }
            set { m_HanMucNo = Math.Max(0, value); }
        }
    }
}
