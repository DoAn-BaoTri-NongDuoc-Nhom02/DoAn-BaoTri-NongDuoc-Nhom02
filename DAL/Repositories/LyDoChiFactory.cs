using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;


namespace CuahangNongduoc.DataLayer
{
    public class LyDoChiFactory
    {
        DataService m_Ds = new DataService();

        public DataTable DanhsachLyDo()
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM LY_DO_CHI");
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable LayLyDoChi(long id)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM LY_DO_CHI WHERE ID = " + id);
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public bool Save()
        {
            return m_Ds.ExecuteNoneQuery() > 0;
        }

        /// <summary>
        /// Có phiếu chi nào đang dùng lý do này không. Dùng DataService riêng
        /// để không ghi đè bảng m_Ds đang gắn với lưới.
        /// </summary>
        public bool DaSuDung(long id)
        {
            DataService ds = new DataService();
            OleDbCommand cmd = new OleDbCommand("SELECT TOP 1 ID FROM PHIEU_CHI WHERE ID_LY_DO_CHI = ?");
            cmd.Parameters.Add("@id", OleDbType.Integer).Value = (int)id;
            ds.Load(cmd);
            return ds.Rows.Count > 0;
        }
    }
}