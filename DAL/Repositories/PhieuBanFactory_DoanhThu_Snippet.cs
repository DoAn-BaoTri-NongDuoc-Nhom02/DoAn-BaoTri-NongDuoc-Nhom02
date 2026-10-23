// ===== THEM VAO class PhieuBanFactory (CuahangNongduoc.DataLayer) =====
// Copy 2 method duoi vao cuoi class, truoc dau } cuoi cung.

        /// <summary>
        /// PM-36: Doanh thu theo khoang ngay, loc KH / kenh (si=true, le=false, null=tat ca).
        /// </summary>
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

        /// <summary>Tong hop nhanh theo ngay trong khoang (cho bieu do cot don gian).</summary>
        public DataTable DoanhThuTheoNgay(DateTime tuNgay, DateTime denNgay)
        {
            string sql =
                "SELECT PB.NGAY_BAN AS NgayBan, SUM(PB.TONG_TIEN) AS TongTien " +
                "FROM PHIEU_BAN PB " +
                "WHERE PB.NGAY_BAN >= ? AND PB.NGAY_BAN <= ? " +
                "GROUP BY PB.NGAY_BAN ORDER BY PB.NGAY_BAN";
            OleDbCommand cmd = new OleDbCommand(sql);
            cmd.Parameters.Add("tu", OleDbType.Date).Value = tuNgay.Date;
            cmd.Parameters.Add("den", OleDbType.Date).Value = denNgay.Date.AddDays(1).AddSeconds(-1);
            DataService ds = new DataService();
            ds.Load(cmd);
            return ds;
        }
