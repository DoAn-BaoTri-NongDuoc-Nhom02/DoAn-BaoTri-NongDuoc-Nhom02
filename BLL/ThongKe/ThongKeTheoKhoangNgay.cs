using System;
using System.Collections.Generic;
using System.Data;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.ThongKe
{
    /// <summary>Một loại thống kê theo khoảng ngày.</summary>
    public interface IThongKe
    {
        String TieuDe { get; }
        DataTable ThucHien(KhoangNgay khoang);

        /// <summary>Kênh xuất kết quả (Bridge).</summary>
        IKenhXuat KenhXuat { get; set; }

        /// <summary>Chạy thống kê rồi đưa kết quả ra kênh xuất hiện tại.</summary>
        void Xuat(KhoangNgay khoang);
    }

    /// <summary>
    /// Khung chung của mọi thống kê "từ ngày đến ngày".
    ///
    /// PATTERN #22 Template Method.
    /// ThucHien() cố định trình tự các bước: kiểm tra khoảng ngày -> truy vấn -> đổi tên cột hiển thị ->
    /// thêm dòng tổng cộng -> ghi chú khoảng ngày. Lớp con chỉ viết bước khác nhau là TruyVan() (và có thể
    /// đổi TenCot / CoDongTong). Bốn thống kê của yêu cầu 4 vì thế có cùng cách chạy và cùng hình thức.
    /// </summary>
    public abstract class ThongKeTheoKhoangNgay : IThongKe
    {
        /// <summary>Tên cột có giá trị true ở dòng tổng cộng (để lớp khác nhận ra).</summary>
        public const String COT_LA_DONG_TONG = "__DONG_TONG";

        private readonly IKetNoiDuLieu m_KetNoi;

        protected ThongKeTheoKhoangNgay(IKetNoiDuLieu ketNoi)
        {
            // PATTERN #6 Adapter (nơi dùng) – mặc định đọc dữ liệu qua DataServiceAdapter (bọc DataService cũ).
            m_KetNoi = ketNoi ?? new DataServiceAdapter();
        }

        protected IKetNoiDuLieu KetNoi
        {
            get { return m_KetNoi; }
        }

        public abstract String TieuDe { get; }

        // ---------------- Bridge: thống kê (abstraction) -> kênh xuất (implementor) ----------------

        private IKenhXuat m_KenhXuat;

        public IKenhXuat KenhXuat
        {
            get { return m_KenhXuat; }
            set { m_KenhXuat = value; }
        }

        public void Xuat(KhoangNgay khoang)
        {
            if (m_KenhXuat == null) throw new InvalidOperationException("Chưa chọn kênh xuất cho thống kê.");
            m_KenhXuat.Xuat(TieuDe + " (" + khoang + ")", ThucHien(khoang));
        }

        // ---------------- Khuôn mẫu ----------------

        public DataTable ThucHien(KhoangNgay khoang)
        {
            if (khoang == null) throw new ArgumentNullException("khoang");

            DataTable bang = TruyVan(khoang);
            DoiTenCot(bang);
            if (CoDongTong)
                ThemDongTong(bang);
            bang.TableName = TieuDe;
            bang.ExtendedProperties["KhoangNgay"] = khoang.ToString();
            return bang;
        }

        // ---------------- Các bước lớp con cung cấp / có thể đổi ----------------

        /// <summary>Bước bắt buộc: lấy dữ liệu của thống kê trong khoảng ngày.</summary>
        protected abstract DataTable TruyVan(KhoangNgay khoang);

        /// <summary>Cặp (tên cột trong truy vấn, tên hiển thị).</summary>
        protected virtual String[][] TenCot
        {
            get { return new String[0][]; }
        }

        protected virtual bool CoDongTong
        {
            get { return true; }
        }

        /// <summary>Tham số @tu, @den (đến hết ngày cuối) cho câu SQL.</summary>
        protected static IDictionary<String, object> ThamSoKhoangNgay(KhoangNgay khoang)
        {
            Dictionary<String, object> ts = new Dictionary<String, object>();
            ts["tu"] = khoang.TuNgay;
            ts["den"] = khoang.DenNgayKhongBaoGom;
            return ts;
        }

        private void DoiTenCot(DataTable bang)
        {
            foreach (String[] cap in TenCot)
            {
                if (bang.Columns.Contains(cap[0]))
                    bang.Columns[cap[0]].ColumnName = cap[1];
            }
        }

        /// <summary>
        /// Thêm dòng "TỔNG CỘNG" cộng các cột số (trừ cột mã "ID_..."). Dùng chung cho lớp khác
        /// (ví dụ proxy lọc bớt dòng rồi tính lại tổng).
        /// </summary>
        public static void ThemDongTong(DataTable bang)
        {
            BoDongTong(bang);
            if (!bang.Columns.Contains(COT_LA_DONG_TONG))
            {
                DataColumn c = bang.Columns.Add(COT_LA_DONG_TONG, typeof(bool));
                c.DefaultValue = false;
                foreach (DataRow r in bang.Rows) r[c] = false;
            }

            DataRow tong = bang.NewRow();
            bool daGhiNhan = false;
            foreach (DataColumn c in bang.Columns)
            {
                if (c.ColumnName == COT_LA_DONG_TONG) continue;
                if (LaCotSo(c) && !c.ColumnName.StartsWith("ID_"))
                {
                    decimal s = 0;
                    foreach (DataRow r in bang.Rows)
                        if (r[c] != DBNull.Value) s += Convert.ToDecimal(r[c]);
                    tong[c] = Convert.ChangeType(s, c.DataType);
                }
                else if (!daGhiNhan && c.DataType == typeof(String))
                {
                    tong[c] = "TỔNG CỘNG";
                    daGhiNhan = true;
                }
            }
            tong[COT_LA_DONG_TONG] = true;
            bang.Rows.Add(tong);
        }

        /// <summary>Bỏ dòng tổng cộng (nếu có).</summary>
        public static void BoDongTong(DataTable bang)
        {
            if (!bang.Columns.Contains(COT_LA_DONG_TONG)) return;
            for (int i = bang.Rows.Count - 1; i >= 0; i--)
            {
                object v = bang.Rows[i][COT_LA_DONG_TONG];
                if (v != DBNull.Value && (bool)v)
                    bang.Rows.RemoveAt(i);
            }
        }

        private static bool LaCotSo(DataColumn c)
        {
            Type t = c.DataType;
            return t == typeof(int) || t == typeof(long) || t == typeof(decimal) || t == typeof(double)
                || t == typeof(short) || t == typeof(float);
        }
    }
}
