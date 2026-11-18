using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.DataLayer
{
    /// <summary>Đọc / ghi bảng CT_DICH_VU_PHIEU_BAN (dịch vụ phụ của từng hóa đơn).</summary>
    public class DichVuPhieuBanFactory
    {
        /// <summary>Ghi lại toàn bộ dịch vụ của một phiếu (xóa cũ, thêm mới).</summary>
        public void Luu(String idPhieuBan, IList<DichVuPhatSinh> dichVu)
        {
            DataService ds = new DataService();
            OleDbCommand xoa = new OleDbCommand("DELETE FROM CT_DICH_VU_PHIEU_BAN WHERE ID_PHIEU_BAN = @id");
            xoa.Parameters.Add("id", OleDbType.VarChar, 20).Value = idPhieuBan;
            ds.ExecuteNonQuery(xoa);

            foreach (DichVuPhatSinh dv in dichVu)
            {
                OleDbCommand them = new OleDbCommand(
                    "INSERT INTO CT_DICH_VU_PHIEU_BAN (ID_PHIEU_BAN, TEN_DICH_VU, SO_TIEN) VALUES (@id, @ten, @tien)");
                them.Parameters.Add("id", OleDbType.VarChar, 20).Value = idPhieuBan;
                them.Parameters.Add("ten", OleDbType.VarWChar, 100).Value = dv.Ten;
                them.Parameters.Add("tien", OleDbType.Decimal).Value = dv.SoTien;
                ds.ExecuteNonQuery(them);
            }
        }

        public IList<DichVuPhatSinh> LayTheoPhieu(String idPhieuBan)
        {
            List<DichVuPhatSinh> ds = new List<DichVuPhatSinh>();
            DataService d = new DataService();
            OleDbCommand cmd = new OleDbCommand("SELECT TEN_DICH_VU, SO_TIEN FROM CT_DICH_VU_PHIEU_BAN WHERE ID_PHIEU_BAN = @id");
            cmd.Parameters.Add("id", OleDbType.VarChar, 20).Value = idPhieuBan;
            d.Load(cmd);
            foreach (DataRow r in d.Bang.Rows)
                ds.Add(new DichVuPhatSinh(DuLieu.LayChuoi(r["TEN_DICH_VU"]), DuLieu.LaySoLon(r["SO_TIEN"])));
            return ds;
        }
    }
}
