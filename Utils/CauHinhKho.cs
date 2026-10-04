using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text;

namespace CuahangNongduoc
{
    /// <summary>Phương pháp chọn lô khi xuất kho (bán hàng).</summary>
    public enum PhuongPhapXuatKho
    {
        /// <summary>Tự phân lô theo hạn dùng gần nhất (FIFO/FEFO).</summary>
        FIFO = 0,
        /// <summary>Người bán tự chọn lô.</summary>
        ChiDinh = 1
    }

    /// <summary>Phương pháp tính giá xuất (giá vốn) của hàng bán ra.</summary>
    public enum PhuongPhapGiaXuat
    {
        /// <summary>Bình quân gia quyền: lấy SAN_PHAM.DON_GIA_NHAP.</summary>
        BinhQuanGiaQuyen = 0,
        /// <summary>FIFO: lấy giá nhập của chính lô được xuất.</summary>
        FIFO = 1
    }

    /// <summary>
    /// Cấu hình kho, lưu trong bảng CAU_HINH (KHOA, GIA_TRI).
    /// Form "Thông tin cửa hàng" (STT 3) chỉ cần đọc/ghi hai thuộc tính bên dưới.
    /// Nếu DB chưa có bảng CAU_HINH thì tự tạo ở lần đọc/ghi đầu tiên.
    /// Mặc định: xuất kho FIFO, giá xuất bình quân gia quyền.
    /// </summary>
    public static class CauHinhKho
    {
        const string KHOA_XUAT_KHO = "PHUONG_PHAP_XUAT_KHO";
        const string KHOA_GIA_XUAT = "PHUONG_PHAP_GIA_XUAT";

        static bool m_DaKiemTraBang = false;

        public static PhuongPhapXuatKho PhuongPhapXuatKho
        {
            get
            {
                string v = Lay(KHOA_XUAT_KHO);
                if (v == PhuongPhapXuatKho.ChiDinh.ToString())
                    return PhuongPhapXuatKho.ChiDinh;
                return PhuongPhapXuatKho.FIFO;
            }
            set { Gan(KHOA_XUAT_KHO, value.ToString()); }
        }

        public static PhuongPhapGiaXuat PhuongPhapGiaXuat
        {
            get
            {
                string v = Lay(KHOA_GIA_XUAT);
                if (v == PhuongPhapGiaXuat.FIFO.ToString())
                    return PhuongPhapGiaXuat.FIFO;
                return PhuongPhapGiaXuat.BinhQuanGiaQuyen;
            }
            set { Gan(KHOA_GIA_XUAT, value.ToString()); }
        }

        static void DamBaoBang()
        {
            if (m_DaKiemTraBang)
                return;

            DataService ds = new DataService();
            try
            {
                ds.ExecuteScalar(new OleDbCommand("SELECT COUNT(*) FROM CAU_HINH"));
            }
            catch
            {
                // Chưa có bảng -> tạo mới (cú pháp dùng được cho cả Access/Jet và SQL Server).
                ds.ExecuteNoneQuery(new OleDbCommand(
                    "CREATE TABLE CAU_HINH (KHOA VARCHAR(50) NOT NULL, GIA_TRI VARCHAR(50), " +
                    "CONSTRAINT PK_CAU_HINH PRIMARY KEY (KHOA))"));
            }
            m_DaKiemTraBang = true;
        }

        static string Lay(string khoa)
        {
            DamBaoBang();
            DataService ds = new DataService();
            OleDbCommand cmd = new OleDbCommand("SELECT GIA_TRI FROM CAU_HINH WHERE KHOA = @khoa");
            cmd.Parameters.Add("khoa", OleDbType.VarChar, 50).Value = khoa;
            object obj = ds.ExecuteScalar(cmd);
            return obj == null ? null : Convert.ToString(obj);
        }

        static void Gan(string khoa, string giaTri)
        {
            DamBaoBang();
            DataService ds = new DataService();

            OleDbCommand upd = new OleDbCommand("UPDATE CAU_HINH SET GIA_TRI = @giatri WHERE KHOA = @khoa");
            upd.Parameters.Add("giatri", OleDbType.VarChar, 50).Value = giaTri;
            upd.Parameters.Add("khoa", OleDbType.VarChar, 50).Value = khoa;

            if (ds.ExecuteNoneQuery(upd) == 0)
            {
                OleDbCommand ins = new OleDbCommand("INSERT INTO CAU_HINH (KHOA, GIA_TRI) VALUES (@khoa, @giatri)");
                ins.Parameters.Add("khoa", OleDbType.VarChar, 50).Value = khoa;
                ins.Parameters.Add("giatri", OleDbType.VarChar, 50).Value = giaTri;
                ds.ExecuteNoneQuery(ins);
            }
        }
    }
}
