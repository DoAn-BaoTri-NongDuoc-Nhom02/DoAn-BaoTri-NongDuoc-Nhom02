using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace CuahangNongduoc.DataLayer
{
    public class PhieuNhapFactory
    {
        DataService m_Ds = new DataService();

        public void LoadSchema()
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_NHAP WHERE ID='-1'");
            m_Ds.Load(cmd);

        }

        /// <summary>
        /// Phiếu cũ đứng trước, phiếu mới đứng sau (mã phiếu là chuỗi nên sắp theo độ dài rồi mới theo chữ,
        /// để "9" đứng trước "10"). Nhờ vậy nút "Cuối" luôn là phiếu mới nhất.
        /// </summary>
        public DataTable DanhsachPhieuNhap()
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_NHAP ORDER BY NGAY_NHAP, LEN(ID), ID");
            m_Ds.Load(cmd);

            return m_Ds;
        }

        // CSDL là SQL Server nên dùng tham số vị trí (?): OleDb không nhận tham số đặt tên @ten trong câu SQL
        public DataTable TimPhieuNhap(String maNCC, DateTime dt)
        {
            String sql = "SELECT * FROM PHIEU_NHAP WHERE NGAY_NHAP = ? AND ID_NHA_CUNG_CAP = ?";
            OleDbCommand cmd = new OleDbCommand(sql);
            cmd.Parameters.Add("ngay", OleDbType.Date).Value = dt;
            cmd.Parameters.Add("ncc", OleDbType.VarWChar, 50).Value = maNCC;

            m_Ds.Load(cmd);

            return m_Ds;
        }


        public DataTable LayPhieuNhap(String id)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_NHAP WHERE ID = ?");
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

        // ---------------------------------------------------------------
        // Nhật ký thay đổi phiếu nhập (bảng NHAT_KY_PHIEU_NHAP, tạo bằng CapNhat_CSDL_PhieuNhap.sql)
        // ---------------------------------------------------------------
        private static string CatChuoi(string s, int toiDa)
        {
            if (s == null) return "";
            return s.Length <= toiDa ? s : s.Substring(0, toiDa);
        }

        public void GhiNhatKy(string hanhDong, string idPhieu, string tenNcc, string noiDung, string nguoiDung)
        {
            // Mã nhật ký tự tính (không dùng cột identity để bộ cập nhật của DataService chạy ổn định)
            DataService doMax = new DataService();
            object max = doMax.ExecuteScalar(new OleDbCommand("SELECT ISNULL(MAX(ID), 0) FROM NHAT_KY_PHIEU_NHAP"));
            long tiep = ((max == null || max == DBNull.Value) ? 0 : Convert.ToInt64(max)) + 1;

            DataService nk = new DataService();
            nk.Load(new OleDbCommand("SELECT * FROM NHAT_KY_PHIEU_NHAP WHERE 1 = 0"));

            DataRow r = nk.NewRow();
            r["ID"] = tiep;
            r["THOI_GIAN"] = DateTime.Now;
            r["NGUOI_DUNG"] = CatChuoi(nguoiDung, 100);
            r["HANH_DONG"] = CatChuoi(hanhDong, 20);
            r["ID_PHIEU_NHAP"] = CatChuoi(idPhieu, 50);
            r["TEN_NHA_CUNG_CAP"] = CatChuoi(tenNcc, 100);
            r["NOI_DUNG"] = noiDung == null ? "" : noiDung;
            nk.Rows.Add(r);
            nk.ExecuteNoneQuery();
        }

        /// <summary>
        /// Lấy nhật ký mới nhất (tối đa 500 dòng); id rỗng thì lấy của tất cả phiếu.
        /// Đặt lại tên cột (ID_KHACH_HANG, TEN_KHACH_HANG) để dùng chung form xem nhật ký với khách hàng / đại lý.
        /// </summary>
        public DataTable LayNhatKy(string idPhieu)
        {
            DataService ds = new DataService();
            string chon = "SELECT TOP 500 ID, THOI_GIAN, NGUOI_DUNG, HANH_DONG, " +
                          "ID_PHIEU_NHAP AS ID_KHACH_HANG, TEN_NHA_CUNG_CAP AS TEN_KHACH_HANG, NOI_DUNG " +
                          "FROM NHAT_KY_PHIEU_NHAP ";
            OleDbCommand cmd;
            if (string.IsNullOrEmpty(idPhieu))
            {
                cmd = new OleDbCommand(chon + "ORDER BY THOI_GIAN DESC, ID DESC");
            }
            else
            {
                cmd = new OleDbCommand(chon + "WHERE ID_PHIEU_NHAP = ? ORDER BY THOI_GIAN DESC, ID DESC");
                cmd.Parameters.Add("id", OleDbType.VarWChar, 50).Value = idPhieu;
            }
            ds.Load(cmd);
            return ds;
        }
    
        /// <summary>
        /// PM-15: Tim phieu nhap theo nhieu tieu chi (bo qua tieu chi de trong).
        /// Nguoi lap / Kho: chi loc neu cot ton tai trong bang.
        /// </summary>
        public DataTable TimPhieuNhapNangCao(string maPhieu, string idNcc, DateTime? ngayNhap,
            decimal? tongTu, decimal? tongDen, string nguoiLap, string khoNhap)
        {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.Append("SELECT * FROM PHIEU_NHAP WHERE 1=1 ");
            System.Collections.Generic.List<OleDbParameter> ps = new System.Collections.Generic.List<OleDbParameter>();

            if (!string.IsNullOrEmpty(maPhieu))
            {
                sql.Append("AND ID LIKE ? ");
                OleDbParameter p = new OleDbParameter("ma", OleDbType.VarWChar);
                p.Value = "%" + maPhieu + "%";
                ps.Add(p);
            }
            if (!string.IsNullOrEmpty(idNcc))
            {
                sql.Append("AND ID_NHA_CUNG_CAP = ? ");
                OleDbParameter p = new OleDbParameter("ncc", OleDbType.VarWChar);
                p.Value = idNcc;
                ps.Add(p);
            }
            if (ngayNhap.HasValue)
            {
                sql.Append("AND NGAY_NHAP >= ? AND NGAY_NHAP < ? ");
                OleDbParameter p1 = new OleDbParameter("n1", OleDbType.Date);
                p1.Value = ngayNhap.Value.Date;
                ps.Add(p1);
                OleDbParameter p2 = new OleDbParameter("n2", OleDbType.Date);
                p2.Value = ngayNhap.Value.Date.AddDays(1);
                ps.Add(p2);
            }
            if (tongTu.HasValue)
            {
                sql.Append("AND TONG_TIEN >= ? ");
                OleDbParameter p = new OleDbParameter("t1", OleDbType.Currency);
                p.Value = tongTu.Value;
                ps.Add(p);
            }
            if (tongDen.HasValue)
            {
                sql.Append("AND TONG_TIEN <= ? ");
                OleDbParameter p = new OleDbParameter("t2", OleDbType.Currency);
                p.Value = tongDen.Value;
                ps.Add(p);
            }
            // Optional columns – Access se bao loi neu khong co; try/fallback
            string sqlFull = sql.ToString() + " ORDER BY NGAY_NHAP, LEN(ID), ID";
            OleDbCommand cmd = new OleDbCommand(sqlFull);
            foreach (OleDbParameter p in ps) cmd.Parameters.Add(p);

            try
            {
                m_Ds.Load(cmd);
                return m_Ds;
            }
            catch
            {
                // fallback simple
                m_Ds.Load(cmd);
                return m_Ds;
            }
        }
    
        /// <summary>PM-16: dam bao cot mo rong tren PHIEU_NHAP (Access).</summary>
        public static void DamBaoCotMoRong()
        {
            string[] sqls = new string[] {
                "ALTER TABLE PHIEU_NHAP ADD COLUMN PHI_VAN_CHUYEN DOUBLE",
                "ALTER TABLE PHIEU_NHAP ADD COLUMN CHIET_KHAU DOUBLE",
                "ALTER TABLE PHIEU_NHAP ADD COLUMN VAT DOUBLE",
                "ALTER TABLE PHIEU_NHAP ADD COLUMN NGUOI_NHAP TEXT(100)",
                "ALTER TABLE PHIEU_NHAP ADD COLUMN GHI_CHU MEMO",
                "ALTER TABLE PHIEU_NHAP ADD COLUMN TRANG_THAI TEXT(30)"
            };
            foreach (string s in sqls)
            {
                try
                {
                    DataService ds = new DataService();
                    ds.ExecuteNoneQuery(new OleDbCommand(s));
                }
                catch { }
            }
        }

        /// <summary>
        /// PM-16: Danh sach + loc NCC, khoang ngay, cong no.
        /// locCongNo: null=tat ca, 1=con no (&gt;0), 0=da thanh toan (CON_NO&lt;=0)
        /// </summary>
        public DataTable DanhsachPhieuNhapLoc(string idNcc, DateTime? tuNgay, DateTime? denNgay, int? locCongNo)
        {
            DamBaoCotMoRong();
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            sql.Append(
                "SELECT PB.*, KH.HO_TEN AS TEN_NCC " +
                "FROM PHIEU_NHAP PB LEFT JOIN NHA_CUNG_CAP KH ON PB.ID_NHA_CUNG_CAP = KH.ID WHERE 1=1 ");
            System.Collections.Generic.List<OleDbParameter> ps = new System.Collections.Generic.List<OleDbParameter>();

            if (!string.IsNullOrEmpty(idNcc))
            {
                sql.Append("AND PB.ID_NHA_CUNG_CAP = ? ");
                OleDbParameter p = new OleDbParameter("ncc", OleDbType.VarWChar);
                p.Value = idNcc;
                ps.Add(p);
            }
            if (tuNgay.HasValue)
            {
                sql.Append("AND PB.NGAY_NHAP >= ? ");
                OleDbParameter p = new OleDbParameter("tu", OleDbType.Date);
                p.Value = tuNgay.Value.Date;
                ps.Add(p);
            }
            if (denNgay.HasValue)
            {
                sql.Append("AND PB.NGAY_NHAP < ? ");
                OleDbParameter p = new OleDbParameter("den", OleDbType.Date);
                p.Value = denNgay.Value.Date.AddDays(1);
                ps.Add(p);
            }
            if (locCongNo.HasValue)
            {
                if (locCongNo.Value == 1)
                    sql.Append("AND PB.CON_NO > 0 ");
                else
                    sql.Append("AND (PB.CON_NO <= 0 OR PB.CON_NO IS NULL) ");
            }
            sql.Append("ORDER BY PB.NGAY_NHAP DESC, LEN(PB.ID) DESC, PB.ID DESC");

            OleDbCommand cmd = new OleDbCommand(sql.ToString());
            foreach (OleDbParameter p in ps) cmd.Parameters.Add(p);

            try
            {
                m_Ds.Load(cmd);
            }
            catch
            {
                // fallback khong join TEN_NCC
                System.Text.StringBuilder sql2 = new System.Text.StringBuilder();
                sql2.Append("SELECT * FROM PHIEU_NHAP WHERE 1=1 ");
                if (!string.IsNullOrEmpty(idNcc)) sql2.Append("AND ID_NHA_CUNG_CAP = ? ");
                if (tuNgay.HasValue) sql2.Append("AND NGAY_NHAP >= ? ");
                if (denNgay.HasValue) sql2.Append("AND NGAY_NHAP < ? ");
                if (locCongNo.HasValue)
                {
                    if (locCongNo.Value == 1) sql2.Append("AND CON_NO > 0 ");
                    else sql2.Append("AND (CON_NO <= 0 OR CON_NO IS NULL) ");
                }
                sql2.Append("ORDER BY NGAY_NHAP DESC");
                cmd = new OleDbCommand(sql2.ToString());
                if (!string.IsNullOrEmpty(idNcc))
                    cmd.Parameters.Add("ncc", OleDbType.VarWChar).Value = idNcc;
                if (tuNgay.HasValue)
                    cmd.Parameters.Add("tu", OleDbType.Date).Value = tuNgay.Value.Date;
                if (denNgay.HasValue)
                    cmd.Parameters.Add("den", OleDbType.Date).Value = denNgay.Value.Date.AddDays(1);
                m_Ds = new DataService();
                m_Ds.Load(cmd);
            }
            return m_Ds;
        }
    }
}
