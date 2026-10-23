using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace CuahangNongduoc.DataLayer
{
    public class PhieuBanFactory
    {
        DataService m_Ds = new DataService();

        public DataTable TimPhieuBan(String idKh, DateTime dt)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_BAN WHERE NGAY_BAN = @ngay AND ID_KHACH_HANG=@kh");
            cmd.Parameters.Add("ngay", OleDbType.Date).Value = dt;
            cmd.Parameters.Add("kh", OleDbType.VarChar).Value = idKh;

            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable TimPhieuBanTheoKhach(String idKh)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_BAN WHERE ID_KHACH_HANG=@kh ORDER BY NGAY_BAN DESC");
            cmd.Parameters.Add("kh", OleDbType.VarChar).Value = idKh;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable DanhsachPhieuBanLe()
        {
            OleDbCommand cmd = new OleDbCommand("SELECT PB.* FROM PHIEU_BAN PB INNER JOIN KHACH_HANG KH ON PB.ID_KHACH_HANG=KH.ID WHERE KH.LOAI_KH=FALSE");
            m_Ds.Load(cmd);

            return m_Ds;
        }
        public DataTable DanhsachPhieuBanSi()
        {
            OleDbCommand cmd = new OleDbCommand("SELECT PB.* FROM PHIEU_BAN PB INNER JOIN KHACH_HANG KH ON PB.ID_KHACH_HANG=KH.ID WHERE KH.LOAI_KH=TRUE");
            m_Ds.Load(cmd);

            return m_Ds;
        }


        public DataTable LayPhieuBan(String id)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_BAN WHERE ID = @id");
            cmd.Parameters.Add("id", OleDbType.VarChar,50).Value = id;
            m_Ds.Load(cmd);
            return m_Ds;
        }


        public DataTable LayChiTietPhieuBan(String idPhieuBan)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM CHI_TIET_PHIEU_BAN WHERE ID_PHIEU_BAN = @id");
            cmd.Parameters.Add("id", OleDbType.VarChar,50).Value = idPhieuBan;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public static long LayConNo(String kh, int thang, int nam)
        {
            DataService ds = new DataService();
            OleDbCommand cmd = new OleDbCommand("SELECT SUM(CON_NO) FROM PHIEU_BAN WHERE ID_KHACH_HANG = @kh AND MONTH(NGAY_BAN)=@thang AND YEAR(NGAY_BAN)= @nam");
            cmd.Parameters.Add("kh", OleDbType.VarChar, 50).Value = kh;
            cmd.Parameters.Add("thang", OleDbType.Integer).Value = thang;
            cmd.Parameters.Add("nam", OleDbType.Integer).Value = nam;

            object obj = ds.ExecuteScalar(cmd);
            if (obj == null)
                return 0;
            else
                return Convert.ToInt64(obj);
        }

        public static int LaySoPhieu()
        {
            DataService ds = new DataService();
            OleDbCommand cmd = new OleDbCommand("SELECT COUNT(*) FROM PHIEU_BAN");
            
            object obj = ds.ExecuteScalar(cmd);
            if (obj == null)
                return 0;
            else
                return Convert.ToInt32(obj);
        }
        
        public DataRow NewRow()
        {
            return m_Ds.NewRow();
        }
        public void Add(DataRow row)
        {
            m_Ds.Rows.Add(row);
        }
       public bool Save()
        {
           
            return m_Ds.ExecuteNoneQuery() > 0;
        }
    
        public DataTable ThongKeDoanhThu(DateTime tuNgay, DateTime denNgay, string idKhachHang, bool? banSi)
        {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.Append(
                "SELECT PB.ID AS MaPhieu, PB.NGAY_BAN AS NgayBan, " +
                "KH.HO_TEN AS TenKhachHang, " +
                "IIF(KH.LOAI_KH=TRUE OR KH.LOAI_KH=-1 OR KH.LOAI_KH=1, 'Ban si', 'Ban le') AS Kenh, " +
                "PB.TONG_TIEN AS TongTien, PB.DA_TRA AS DaTra, PB.CON_NO AS ConNo " +
                "FROM PHIEU_BAN PB LEFT JOIN KHACH_HANG KH ON PB.ID_KHACH_HANG = KH.ID " +
                "WHERE PB.NGAY_BAN >= ? AND PB.NGAY_BAN <= ? ");
            if (!string.IsNullOrEmpty(idKhachHang))
                sql.Append("AND PB.ID_KHACH_HANG = ? ");
            if (banSi.HasValue)
            {
                if (banSi.Value)
                    sql.Append("AND (KH.LOAI_KH = TRUE OR KH.LOAI_KH = -1 OR KH.LOAI_KH = 1) ");
                else
                    sql.Append("AND (KH.LOAI_KH = FALSE OR KH.LOAI_KH = 0 OR KH.LOAI_KH IS NULL) ");
            }
            sql.Append("ORDER BY PB.NGAY_BAN, PB.ID");

            OleDbCommand cmd = new OleDbCommand(sql.ToString());
            cmd.Parameters.Add("tu", OleDbType.Date).Value = tuNgay.Date;
            cmd.Parameters.Add("den", OleDbType.Date).Value = denNgay.Date.AddDays(1).AddSeconds(-1);
            if (!string.IsNullOrEmpty(idKhachHang))
                cmd.Parameters.Add("kh", OleDbType.VarChar).Value = idKhachHang;

            DataService ds = new DataService();
            ds.Load(cmd);
            return ds;
        }

        public DataTable DoanhThuTheoNgay(DateTime tuNgay, DateTime denNgay)
        {
            string sql =
                "SELECT PB.NGAY_BAN AS NgayBan, SUM(PB.TONG_TIEN) AS TongTien " +
                "FROM PHIEU_BAN PB WHERE PB.NGAY_BAN >= ? AND PB.NGAY_BAN <= ? " +
                "GROUP BY PB.NGAY_BAN ORDER BY PB.NGAY_BAN";
            OleDbCommand cmd = new OleDbCommand(sql);
            cmd.Parameters.Add("tu", OleDbType.Date).Value = tuNgay.Date;
            cmd.Parameters.Add("den", OleDbType.Date).Value = denNgay.Date.AddDays(1).AddSeconds(-1);
            DataService ds = new DataService();
            ds.Load(cmd);
            return ds;
        }

        public DataTable TimPhieuBanKhoang(String idKh, DateTime tu, DateTime den)
        {
            System.Data.OleDb.OleDbCommand cmd;
            bool coKH = idKh != null && idKh.Length > 0;
            if (coKH)
            {
                cmd = new System.Data.OleDb.OleDbCommand(
                    "SELECT * FROM PHIEU_BAN WHERE ID_KHACH_HANG=@kh AND NGAY_BAN >= @tu AND NGAY_BAN <= @den ORDER BY NGAY_BAN");
                cmd.Parameters.Add("@kh", System.Data.OleDb.OleDbType.VarWChar).Value = idKh;
            }
            else
            {
                cmd = new System.Data.OleDb.OleDbCommand(
                    "SELECT * FROM PHIEU_BAN WHERE NGAY_BAN >= @tu AND NGAY_BAN <= @den ORDER BY NGAY_BAN");
            }
            cmd.Parameters.Add("@tu", System.Data.OleDb.OleDbType.Date).Value = tu.Date;
            cmd.Parameters.Add("@den", System.Data.OleDb.OleDbType.Date).Value = den.Date;
            DataService ds = new DataService();
            ds.Load(cmd);
            return ds;
        }

    }
}
