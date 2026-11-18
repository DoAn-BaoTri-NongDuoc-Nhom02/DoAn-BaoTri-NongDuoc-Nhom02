using System;
using System.Security.Cryptography;
using System.Text;

namespace CuahangNongduoc
{
    /// <summary>
    /// Băm mật khẩu bằng SHA-256 trước khi lưu (sửa lỗi 3: trước đây mật khẩu lưu nguyên văn).
    /// Dạng lưu: "SHA256:" + 64 ký tự hex. Mật khẩu cũ chưa băm vẫn đăng nhập được và được băm lại
    /// ngay ở lần đăng nhập thành công đầu tiên.
    /// </summary>
    public static class MatKhau
    {
        public const String TIEN_TO = "SHA256:";

        public static bool DaBam(String daLuu)
        {
            return daLuu != null && daLuu.StartsWith(TIEN_TO);
        }

        /// <summary>Băm mật khẩu; chuỗi đã băm thì giữ nguyên (tránh băm hai lần khi sửa tài khoản).</summary>
        public static String Bam(String matKhau)
        {
            if (matKhau == null) matKhau = "";
            if (DaBam(matKhau)) return matKhau;
            byte[] bam;
            using (SHA256 sha = SHA256.Create())
            {
                bam = sha.ComputeHash(Encoding.UTF8.GetBytes(matKhau));
            }
            StringBuilder sb = new StringBuilder(TIEN_TO);
            foreach (byte b in bam) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>Mật khẩu người dùng nhập có khớp với giá trị đang lưu (đã băm hoặc chưa) không.</summary>
        public static bool Khop(String matKhauNhap, String daLuu)
        {
            if (daLuu == null) return false;
            if (DaBam(daLuu)) return Bam(matKhauNhap) == daLuu;
            return matKhauNhap == daLuu;
        }
    }
}
