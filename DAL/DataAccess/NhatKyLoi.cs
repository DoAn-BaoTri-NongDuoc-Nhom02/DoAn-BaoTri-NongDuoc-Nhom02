using System;
using System.IO;
using System.Text;

namespace CuahangNongduoc
{
    /// <summary>
    /// Ghi lỗi truy cập CSDL ra file loi_csdl.log (cạnh file .exe).
    /// REFACTOR #12 Extract Class – tách ra từ DataService để DataService chỉ lo việc truy vấn.
    /// </summary>
    public static class NhatKyLoi
    {
        public const String TEN_FILE = "loi_csdl.log";

        public static String DuongDan
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TEN_FILE); }
        }

        /// <summary>Ghi một dòng lỗi. Không bao giờ ném ngoại lệ (ghi log không được làm hỏng nghiệp vụ).</summary>
        public static void Ghi(String noiXayRa, String sql, String loi)
        {
            try
            {
                String dong = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [" + noiXayRa + "] " + loi
                    + (String.IsNullOrEmpty(sql) ? "" : "\r\n    SQL: " + sql) + "\r\n";
                File.AppendAllText(DuongDan, dong, Encoding.UTF8);
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }
}
