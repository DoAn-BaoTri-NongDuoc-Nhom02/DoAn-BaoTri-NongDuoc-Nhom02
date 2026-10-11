using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace CuahangNongduoc.DataLayer
{
    public class NhaCungCapFactory
    {
        DataService m_Ds = new DataService();

        public DataTable DanhsachNCC()
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM NHA_CUNG_CAP");
            m_Ds.Load(cmd);

            return m_Ds;
        }
        public DataTable TimDiaChi(String diachi)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM NHA_CUNG_CAP WHERE DIA_CHI LIKE '%' + @diachi + '%' ");
            cmd.Parameters.Add("diachi", OleDbType.VarWChar).Value = diachi;
            m_Ds.Load(cmd);

            return m_Ds;
        }
        public DataTable TimHoTen(String hoten)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM NHA_CUNG_CAP WHERE HO_TEN LIKE '%' + @hoten + '%' ");
            cmd.Parameters.Add("hoten", OleDbType.VarWChar).Value = hoten;
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable LayNCC(String id)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM NHA_CUNG_CAP WHERE ID = @id");
            cmd.Parameters.Add("id", OleDbType.VarChar,50).Value = id;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        /// <summary>Bảng đang được hiển thị / chỉnh sửa (kết quả của lần Load gần nhất).</summary>
        public DataTable Bang
        {
            get { return m_Ds; }
        }

        /// <summary>Tổng số nhà cung cấp trong CSDL (không phụ thuộc bộ lọc/tìm kiếm đang áp dụng).</summary>
        public int DemTong()
        {
            DataService ds = new DataService();
            ds.Load(new OleDbCommand("SELECT COUNT(*) FROM NHA_CUNG_CAP"));
            if (ds.Rows.Count > 0 && ds.Rows[0][0] != DBNull.Value)
                return Convert.ToInt32(ds.Rows[0][0]);
            return 0;
        }

        /// <summary>Các nhà cung cấp khác (ID khác) trong CSDL có cùng họ tên.</summary>
        public DataTable TimTrungHoTen(String hoten, String id)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT ID, HO_TEN FROM NHA_CUNG_CAP WHERE HO_TEN = @hoten AND ID <> @id");
            cmd.Parameters.Add("hoten", OleDbType.VarWChar).Value = hoten;
            cmd.Parameters.Add("id", OleDbType.VarChar, 50).Value = id;
            DataService ds = new DataService();
            ds.Load(cmd);
            return ds;
        }

        /// <summary>Các nhà cung cấp khác (ID khác) trong CSDL có cùng số điện thoại.</summary>
        public DataTable TimTrungDienThoai(String dienthoai, String id)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT ID, HO_TEN FROM NHA_CUNG_CAP WHERE DIEN_THOAI = @dienthoai AND ID <> @id");
            cmd.Parameters.Add("dienthoai", OleDbType.VarChar, 20).Value = dienthoai;
            cmd.Parameters.Add("id", OleDbType.VarChar, 50).Value = id;
            DataService ds = new DataService();
            ds.Load(cmd);
            return ds;
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
    }
}
