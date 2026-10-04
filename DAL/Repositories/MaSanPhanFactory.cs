using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace CuahangNongduoc.DataLayer
{
    public class MaSanPhanFactory
    {
        DataService m_Ds = new DataService();

        public void LoadSchema()
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM MA_SAN_PHAM WHERE ID = '-1'");
            m_Ds.Load(cmd);
        }

        public DataTable DanhsachMaSanPham(String sp)
        {
            // Lô còn hàng, xếp theo hạn dùng gần nhất trước (lô đầu danh sách là lô sẽ xuất theo FIFO).
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM MA_SAN_PHAM WHERE ID_SAN_PHAM=@id AND SO_LUONG > 0 ORDER BY NGAY_HET_HAN, NGAY_NHAP, ID");
            cmd.Parameters.Add("id", OleDbType.VarChar, 50).Value = sp;
            m_Ds.Load(cmd);

            return m_Ds;
        }
        public DataTable DanhsachChiTiet(String sp)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM MA_SAN_PHAM WHERE ID_PHIEU_NHAP=@id");
            cmd.Parameters.Add("id", OleDbType.VarChar, 50).Value = sp;
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable LaySanPham(String idMaSanPham)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT SP.* FROM SAN_PHAM SP INNER JOIN MA_SAN_PHAM MSP ON SP.ID = MSP.ID_SAN_PHAM WHERE MSP.ID = @id");
            cmd.Parameters.Add("id", OleDbType.VarChar,50).Value = idMaSanPham;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable LayMaSanPham(String idMaSanPham)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM MA_SAN_PHAM MSP WHERE MSP.ID = @id");
            cmd.Parameters.Add("id", OleDbType.VarChar,50).Value = idMaSanPham;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable DanhsachMaSanPhamHetHan(DateTime dt)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM MA_SAN_PHAM WHERE SO_LUONG > 0 AND NGAY_HET_HAN <= @ngay");
            cmd.Parameters.Add("ngay", OleDbType.Date).Value = dt;
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable DanhsachMaSanPham()
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM MA_SAN_PHAM WHERE SO_LUONG > 0");
            m_Ds.Load(cmd);

            return m_Ds;
        }

        /// <summary>Tất cả lô (kể cả lô đã hết hàng) - dùng để hiển thị lại các phiếu bán cũ.</summary>
        public DataTable DanhsachTatCaMaSanPham()
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM MA_SAN_PHAM");
            m_Ds.Load(cmd);

            return m_Ds;
        }

        /// <summary>Số lượng tồn hiện tại của một lô (0 nếu không có lô).</summary>
        public static int LaySoLuong(String masp)
        {
            DataService ds = new DataService();
            OleDbCommand cmd = new OleDbCommand("SELECT SO_LUONG FROM MA_SAN_PHAM WHERE ID = @id");
            cmd.Parameters.Add("id", OleDbType.VarChar, 50).Value = masp;
            object obj = ds.ExecuteScalar(cmd);
            return obj == null ? 0 : Convert.ToInt32(obj);
        }

        /// <summary>
        /// Cộng/trừ số lượng của một lô (so_luong âm = xuất kho, dương = hoàn kho).
        /// Không cho tồn âm: nếu trừ quá số đang có thì ném TonKhoException và không đổi gì.
        /// Sau khi đổi lô, SAN_PHAM.SO_LUONG được tính lại bằng tổng các lô của sản phẩm đó (tự sửa lệch số liệu cũ).
        /// </summary>
        public static void CapNhatSoLuong(String masp, int so_luong)
        {
            DataService ds = new DataService();
            OleDbCommand cmd = new OleDbCommand(
                "UPDATE MA_SAN_PHAM SET SO_LUONG = SO_LUONG + @so WHERE ID = @id AND SO_LUONG + @so2 >= 0");
            cmd.Parameters.Add("so", OleDbType.Integer).Value = so_luong;
            cmd.Parameters.Add("id", OleDbType.VarChar, 50).Value = masp;
            cmd.Parameters.Add("so2", OleDbType.Integer).Value = so_luong;

            if (ds.ExecuteNoneQuery(cmd) == 0)
            {
                throw new TonKhoException("Lô '" + masp + "' không đủ số lượng tồn để xuất (hoặc không tồn tại).");
            }

            DongBoSoLuongSanPham(masp);
        }

        // Tính lại SAN_PHAM.SO_LUONG = tổng số lượng các lô của sản phẩm chứa lô "masp".
        // Tách thành các câu lệnh đơn giản để chạy được trên cả Access lẫn SQL Server.
        static void DongBoSoLuongSanPham(String masp)
        {
            DataService ds = new DataService();

            OleDbCommand cmdSp = new OleDbCommand("SELECT ID_SAN_PHAM FROM MA_SAN_PHAM WHERE ID = @id");
            cmdSp.Parameters.Add("id", OleDbType.VarChar, 50).Value = masp;
            object idSp = ds.ExecuteScalar(cmdSp);
            if (idSp == null)
                return;

            OleDbCommand cmdTong = new OleDbCommand("SELECT SUM(SO_LUONG) FROM MA_SAN_PHAM WHERE ID_SAN_PHAM = @sp");
            cmdTong.Parameters.Add("sp", OleDbType.VarChar, 50).Value = Convert.ToString(idSp);
            object tong = ds.ExecuteScalar(cmdTong);

            OleDbCommand upd = new OleDbCommand("UPDATE SAN_PHAM SET SO_LUONG = @tong WHERE ID = @sp");
            upd.Parameters.Add("tong", OleDbType.Integer).Value = (tong == null ? 0 : Convert.ToInt32(tong));
            upd.Parameters.Add("sp", OleDbType.VarChar, 50).Value = Convert.ToString(idSp);
            ds.ExecuteNoneQuery(upd);
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
