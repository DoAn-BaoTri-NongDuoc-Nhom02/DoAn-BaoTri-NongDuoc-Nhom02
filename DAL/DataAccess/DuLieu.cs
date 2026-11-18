using System;
using System.Data;

namespace CuahangNongduoc
{
    /// <summary>
    /// Hàm đọc giá trị an toàn từ DataRow / kết quả ExecuteScalar.
    /// REFACTOR #16 Introduce Foreign Method – DataRow là lớp của .NET (không sửa được) và
    /// .NET 2.0 không có extension method, nên các hàm "lẽ ra thuộc DataRow" được gom ở đây
    /// thay cho các biểu thức "x == DBNull.Value ? ... : Convert..." lặp khắp nơi.
    /// </summary>
    public static class DuLieu
    {
        public static bool LaRong(object giaTri)
        {
            return giaTri == null || giaTri == DBNull.Value;
        }

        public static int LaySoNguyen(object giaTri)
        {
            return LaRong(giaTri) ? 0 : Convert.ToInt32(giaTri);
        }

        public static long LaySoLon(object giaTri)
        {
            return LaRong(giaTri) ? 0 : Convert.ToInt64(giaTri);
        }

        public static String LayChuoi(object giaTri)
        {
            return LaRong(giaTri) ? "" : Convert.ToString(giaTri);
        }

        public static DateTime LayNgay(object giaTri, DateTime macDinh)
        {
            return LaRong(giaTri) ? macDinh : Convert.ToDateTime(giaTri);
        }

        public static long LaySoLon(DataRow dong, String cot)
        {
            return LaySoLon(dong[cot]);
        }

        public static DateTime LayNgay(DataRow dong, String cot, DateTime macDinh)
        {
            return LayNgay(dong[cot], macDinh);
        }
    }
}
