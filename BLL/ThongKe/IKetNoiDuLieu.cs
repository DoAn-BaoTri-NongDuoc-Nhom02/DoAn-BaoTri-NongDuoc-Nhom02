using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace CuahangNongduoc.ThongKe
{
    /// <summary>Cách các lớp thống kê đọc dữ liệu: một câu SELECT + tham số theo tên -> bảng; lỗi thì ném ngoại lệ.</summary>
    public interface IKetNoiDuLieu
    {
        DataTable TruyVan(String sql, IDictionary<String, object> thamSo);
    }

    /// <summary>
    /// PATTERN #6 Adapter.
    /// Lớp cũ DataService có giao diện khác hẳn: tự tạo OleDbCommand, gọi Load(), đọc kết quả ở .Bang, và
    /// lỗi không ném ra mà chỉ ghi vào LastError. Các lớp thống kê mới chỉ làm việc với IKetNoiDuLieu;
    /// DataServiceAdapter "chuyển đổi" lời gọi TruyVan(sql, thamSo) sang cách dùng của DataService và đổi
    /// LastError thành ngoại lệ. Sau này đổi sang SqlClient chỉ cần viết adapter khác, không sửa thống kê.
    /// </summary>
    public class DataServiceAdapter : IKetNoiDuLieu
    {
        public DataTable TruyVan(String sql, IDictionary<String, object> thamSo)
        {
            OleDbCommand cmd = new OleDbCommand(sql);
            if (thamSo != null)
            {
                foreach (KeyValuePair<String, object> ts in thamSo)
                {
                    OleDbParameter p = cmd.Parameters.Add(ts.Key, KieuOleDb(ts.Value));
                    p.Value = ts.Value ?? DBNull.Value;
                }
            }

            DataService ds = new DataService();
            ds.Load(cmd);
            if (ds.LastError.Length > 0)
                throw new Exception("Lỗi truy vấn thống kê: " + ds.LastError
                    + "\r\n(Nếu báo thiếu cột/bảng: hãy chạy Database/CapNhat_YeuCau4.sql)");
            return ds.Bang.Copy();
        }

        private static OleDbType KieuOleDb(object giaTri)
        {
            if (giaTri is DateTime) return OleDbType.Date;
            if (giaTri is int) return OleDbType.Integer;
            if (giaTri is long) return OleDbType.BigInt;
            if (giaTri is decimal) return OleDbType.Decimal;
            return OleDbType.VarWChar;
        }
    }
}
