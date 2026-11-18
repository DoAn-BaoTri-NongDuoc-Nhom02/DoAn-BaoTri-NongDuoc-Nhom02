using System;
using System.Data;

namespace CuahangNongduoc.DataLayer
{
    /// <summary>
    /// Giao diện chung của các factory danh mục đơn giản (đơn vị tính, lý do chi, nhà cung cấp).
    /// REFACTOR #62 Extract Interface – ba factory này có cùng bộ thao tác "lấy danh sách / lưu",
    /// nên phía controller chỉ cần làm việc với giao diện này.
    /// </summary>
    public interface IDanhMucFactory
    {
        /// <summary>Toàn bộ danh mục.</summary>
        DataTable DanhSach();

        /// <summary>Tên cột dùng để hiển thị (DisplayMember).</summary>
        String CotHienThi { get; }

        /// <summary>Tên cột khóa (ValueMember).</summary>
        String CotMa { get; }

        bool Save();
    }
}
