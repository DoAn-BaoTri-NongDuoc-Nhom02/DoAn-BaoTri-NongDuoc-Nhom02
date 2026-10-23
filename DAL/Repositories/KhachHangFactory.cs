using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace CuahangNongduoc.DataLayer
{
    /// <summary>Một dòng nhật ký thay đổi (ghi vào bảng NHAT_KY_KHACH_HANG).</summary>
    public class NhatKyKhachHang
    {
        public string HanhDong;     // Thêm / Sửa / Xóa
        public string IdKhach;
        public string TenKhach;
        public string NoiDung;
    }

    public class KhachHangFactory
    {
        DataService m_Ds = new DataService();

        /// <summary>
        /// loai = true: đại lý; false: khách hàng. CSDL là SQL Server nên KHÔNG viết True/False vào câu SQL
        /// (T-SQL không có hai từ này) mà truyền tham số Boolean; dòng chưa gán loại (NULL) tính là khách hàng.
        /// </summary>
        public DataTable DanhsachKhachHang(bool loai)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM KHACH_HANG WHERE ISNULL(LOAI_KH, 0) = ?");
            cmd.Parameters.Add("loai", OleDbType.Boolean).Value = loai;
            m_Ds.Load(cmd);
            return m_Ds;
        }

        public DataTable TimHoTen(String hoten, bool loai)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM KHACH_HANG WHERE HO_TEN LIKE '%' + ? + '%' AND ISNULL(LOAI_KH, 0) = ?");
            cmd.Parameters.Add("hoten", OleDbType.VarWChar).Value = hoten;
            cmd.Parameters.Add("loai", OleDbType.Boolean).Value = loai;
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable TimDiaChi(String diachi, bool loai)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM KHACH_HANG WHERE DIA_CHI LIKE '%' + ? + '%' AND ISNULL(LOAI_KH, 0) = ?");
            cmd.Parameters.Add("diachi", OleDbType.VarWChar).Value = diachi;
            cmd.Parameters.Add("loai", OleDbType.Boolean).Value = loai;
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable DanhsachKhachHang()
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM KHACH_HANG");
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable LayKhachHang(String id)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM KHACH_HANG WHERE ID = ?");
            cmd.Parameters.Add("id", OleDbType.VarWChar, 50).Value = id;
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
        public bool Save()
        {
            return m_Ds.ExecuteNoneQuery() > 0;
        }

        /// <summary>
        /// Đếm phiếu bán và phiếu thanh toán của một khách hàng.
        /// Dùng tham số vị trí (?) vì OleDb không nhận tham số đặt tên @id trên SQL Server.
        /// </summary>
        public void DemGiaoDich(String id, out int soPhieuBan, out int soPhieuThanhToan)
        {
            DataService ds = new DataService();

            OleDbCommand c1 = new OleDbCommand("SELECT COUNT(*) FROM PHIEU_BAN WHERE ID_KHACH_HANG = ?");
            c1.Parameters.Add("id", OleDbType.VarWChar, 50).Value = id;
            soPhieuBan = DemSo(ds.ExecuteScalar(c1));

            OleDbCommand c2 = new OleDbCommand("SELECT COUNT(*) FROM PHIEU_THANH_TOAN WHERE ID_KHACH_HANG = ?");
            c2.Parameters.Add("id", OleDbType.VarWChar, 50).Value = id;
            soPhieuThanhToan = DemSo(ds.ExecuteScalar(c2));
        }

        private static int DemSo(object o)
        {
            return (o == null || o == DBNull.Value) ? 0 : Convert.ToInt32(o);
        }

        private static long DemSo64(object o)
        {
            return (o == null || o == DBNull.Value) ? 0 : Convert.ToInt64(o);
        }

        /// <summary>
        /// Gom theo khách hàng: công nợ hiện tại = tổng CON_NO các phiếu bán - tổng đã thanh toán;
        /// tổng mua = tổng TONG_TIEN các phiếu bán. Mỗi thống kê dùng một DataService riêng.
        /// </summary>
        public void LayThongKe(out Dictionary<string, long> congNo, out Dictionary<string, long> tongMua)
        {
            congNo = new Dictionary<string, long>();
            tongMua = new Dictionary<string, long>();

            DataService ban = new DataService();
            ban.Load(new OleDbCommand("SELECT ID_KHACH_HANG, SUM(CON_NO) AS TONG_NO, SUM(TONG_TIEN) AS TONG_MUA FROM PHIEU_BAN GROUP BY ID_KHACH_HANG"));
            foreach (DataRow r in ban.Rows)
            {
                string kh = Convert.ToString(r["ID_KHACH_HANG"]);
                congNo[kh] = ThanhSoNguyen(r["TONG_NO"]);
                tongMua[kh] = ThanhSoNguyen(r["TONG_MUA"]);
            }

            DataService tt = new DataService();
            tt.Load(new OleDbCommand("SELECT ID_KHACH_HANG, SUM(TONG_TIEN) AS DA_TRA FROM PHIEU_THANH_TOAN GROUP BY ID_KHACH_HANG"));
            foreach (DataRow r in tt.Rows)
            {
                string kh = Convert.ToString(r["ID_KHACH_HANG"]);
                long no;
                if (!congNo.TryGetValue(kh, out no)) no = 0;
                congNo[kh] = no - ThanhSoNguyen(r["DA_TRA"]);
            }
        }

        private static long ThanhSoNguyen(object o)
        {
            return (o == null || o == DBNull.Value) ? 0 : Convert.ToInt64(o);
        }

        // ---------------------------------------------------------------
        // Nhật ký thay đổi (bảng NHAT_KY_KHACH_HANG, tạo bằng CapNhat_CSDL.sql)
        // ---------------------------------------------------------------
        private static string CatChuoi(string s, int toiDa)
        {
            if (s == null) return "";
            return s.Length <= toiDa ? s : s.Substring(0, toiDa);
        }

        public void GhiNhatKy(IList<NhatKyKhachHang> ds, string nguoiDung)
        {
            if (ds == null || ds.Count == 0) return;

            // Mã nhật ký tự tính (không dùng cột identity để bộ cập nhật của DataService chạy ổn định)
            DataService doMax = new DataService();
            long tiep = DemSo64(doMax.ExecuteScalar(new OleDbCommand("SELECT ISNULL(MAX(ID), 0) FROM NHAT_KY_KHACH_HANG"))) + 1;

            DataService nk = new DataService();
            nk.Load(new OleDbCommand("SELECT * FROM NHAT_KY_KHACH_HANG WHERE 1 = 0"));

            DateTime bayGio = DateTime.Now;
            foreach (NhatKyKhachHang n in ds)
            {
                DataRow r = nk.NewRow();
                r["ID"] = tiep;
                r["THOI_GIAN"] = bayGio;
                r["NGUOI_DUNG"] = CatChuoi(nguoiDung, 100);
                r["HANH_DONG"] = CatChuoi(n.HanhDong, 20);
                r["ID_KHACH_HANG"] = CatChuoi(n.IdKhach, 50);
                r["TEN_KHACH_HANG"] = CatChuoi(n.TenKhach, 100);
                r["NOI_DUNG"] = n.NoiDung == null ? "" : n.NoiDung;
                nk.Rows.Add(r);
                tiep++;
            }
            nk.ExecuteNoneQuery();
        }

        /// <summary>Lấy nhật ký mới nhất (tối đa 500 dòng); id rỗng thì lấy của tất cả khách hàng.</summary>
        public DataTable LayNhatKy(string id)
        {
            DataService ds = new DataService();
            OleDbCommand cmd;
            if (string.IsNullOrEmpty(id))
            {
                cmd = new OleDbCommand("SELECT TOP 500 * FROM NHAT_KY_KHACH_HANG ORDER BY THOI_GIAN DESC, ID DESC");
            }
            else
            {
                cmd = new OleDbCommand("SELECT TOP 500 * FROM NHAT_KY_KHACH_HANG WHERE ID_KHACH_HANG = ? ORDER BY THOI_GIAN DESC, ID DESC");
                cmd.Parameters.Add("id", OleDbType.VarWChar, 50).Value = id;
            }
            ds.Load(cmd);
            return ds;
        }
    }
}