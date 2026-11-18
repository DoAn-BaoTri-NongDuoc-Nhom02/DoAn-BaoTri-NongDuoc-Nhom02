using System;

namespace CuahangNongduoc.BLL.Service
{
    /// <summary>
    /// Lỗi nghiệp vụ có câu thông báo dành cho người dùng (dữ liệu thiếu, trùng tên đăng nhập...).
    /// REFACTOR #53 Replace Error Code with Exception – thay cho việc trả chuỗi "OK" / chuỗi lỗi.
    /// </summary>
    public class NghiepVuException : Exception
    {
        public NghiepVuException(String thongBao) : base(thongBao) { }
    }
}
