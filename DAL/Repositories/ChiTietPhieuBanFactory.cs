using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace CuahangNongduoc.DataLayer
{
    public class ChiTietPhieuBanFactory
    {
        DataService m_Ds = new DataService();

      

        public DataTable LayChiTietPhieuBan(String idPhieuBan)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM CHI_TIET_PHIEU_BAN WHERE ID_PHIEU_BAN = @id");
            cmd.Parameters.Add("id", OleDbType.VarChar , 50).Value = idPhieuBan;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable LayChiTietPhieuBan(DateTime dtNgayBan)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT CT.* FROM CHI_TIET_PHIEU_BAN CT INNER JOIN PHIEU_BAN PB ON CT.ID_PHIEU_BAN = PB.ID " +
                    " WHERE PB.NGAY_BAN = @ngayban");
            cmd.Parameters.Add("ngayban", OleDbType.Date).Value = dtNgayBan;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable LayChiTietPhieuBan(int thang, int nam)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT CT.* FROM CHI_TIET_PHIEU_BAN CT INNER JOIN PHIEU_BAN PB ON CT.ID_PHIEU_BAN = PB.ID " +
                    " WHERE MONTH(PB.NGAY_BAN) = @thang AND YEAR(PB.NGAY_BAN)= @nam");
            cmd.Parameters.Add("thang", OleDbType.Integer).Value = thang;
            cmd.Parameters.Add("nam", OleDbType.Integer).Value = nam;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        
        
        public DataRow NewRow()
        {
            return m_Ds.NewRow();
        }
        public void Add(DataRow row)
        {
            m_Ds.Rows.Add(row);
        }

        /// <summary>Tổng số lượng theo từng lô của các dòng mới thêm vào phiếu nhưng chưa lưu.</summary>
        public IDictionary<String, int> SoLuongChuaLuu()
        {
            Dictionary<String, int> ds = new Dictionary<String, int>();
            foreach (DataRow row in m_Ds.Rows)
            {
                if (row.RowState == DataRowState.Added)
                {
                    String lo = Convert.ToString(row["ID_MA_SAN_PHAM"]);
                    int sl = Convert.ToInt32(row["SO_LUONG"]);
                    int cu;
                    ds.TryGetValue(lo, out cu);
                    ds[lo] = cu + sl;
                }
            }
            return ds;
        }

        /// <summary>Dòng mới (chưa lưu) của lô này trong phiếu, null nếu chưa có.</summary>
        public DataRow TimDongChuaLuu(String idLo)
        {
            foreach (DataRow row in m_Ds.Rows)
            {
                if (row.RowState == DataRowState.Added && Convert.ToString(row["ID_MA_SAN_PHAM"]) == idLo)
                    return row;
            }
            return null;
        }

        /// <summary>Phiếu đã có dòng được lưu trước đó cho lô này chưa (khóa chính là phiếu + lô).</summary>
        public bool DaCoDongDaLuu(String idLo)
        {
            foreach (DataRow row in m_Ds.Rows)
            {
                if (row.RowState != DataRowState.Added && row.RowState != DataRowState.Deleted
                    && row.RowState != DataRowState.Detached && Convert.ToString(row["ID_MA_SAN_PHAM"]) == idLo)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Kiểm tra các dòng mới không làm tồn kho âm. hoanTra: số lượng sắp được hoàn kho theo từng lô
        /// (các dòng bị xóa khỏi phiếu khi sửa), có thể null. Ném TonKhoException nếu thiếu hàng.
        /// </summary>
        public void KiemTraTonKho(IDictionary<String, int> hoanTra)
        {
            foreach (KeyValuePair<String, int> kv in SoLuongChuaLuu())
            {
                int ton = CuahangNongduoc.DataLayer.MaSanPhanFactory.LaySoLuong(kv.Key);
                int hoan = 0;
                if (hoanTra != null)
                    hoanTra.TryGetValue(kv.Key, out hoan);
                if (ton + hoan < kv.Value)
                {
                    throw new TonKhoException("Lô '" + kv.Key + "' không đủ hàng: cần " + kv.Value +
                        ", chỉ còn " + (ton + hoan) + ".");
                }
            }
        }

        public bool Save()
        {
            // Kiểm tra trước toàn bộ để không trừ dở dang một phần.
            KiemTraTonKho(null);

            List<KeyValuePair<String, int>> daTru = new List<KeyValuePair<String, int>>();
            try
            {
                foreach (KeyValuePair<String, int> kv in SoLuongChuaLuu())
                {
                    CuahangNongduoc.DataLayer.MaSanPhanFactory.CapNhatSoLuong(kv.Key, -kv.Value);
                    daTru.Add(kv);
                }
            }
            catch
            {
                HoanKho(daTru);
                throw;
            }

            bool ok;
            try
            {
                ok = m_Ds.ExecuteNoneQuery() > 0;
            }
            catch
            {
                HoanKho(daTru);
                throw;
            }

            if (!ok && daTru.Count > 0)
            {
                HoanKho(daTru);
                throw new TonKhoException("Không lưu được chi tiết phiếu bán. Số lượng kho đã được hoàn lại.");
            }
            return ok;
        }

        static void HoanKho(List<KeyValuePair<String, int>> daTru)
        {
            foreach (KeyValuePair<String, int> kv in daTru)
            {
                try
                {
                    CuahangNongduoc.DataLayer.MaSanPhanFactory.CapNhatSoLuong(kv.Key, kv.Value);
                }
                catch
                {
                    // Hoàn kho là cố gắng tối đa; lỗi gốc mới là lỗi cần báo.
                }
            }
        }
    }
}
