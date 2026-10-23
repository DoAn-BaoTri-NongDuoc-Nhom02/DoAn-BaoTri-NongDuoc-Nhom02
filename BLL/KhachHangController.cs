using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;


namespace CuahangNongduoc.Controller
{
    public class KhachHangController
    {
        KhachHangFactory factory = new KhachHangFactory();

        // ---- Các nhãn dùng chung cho cả form Đại lý và form Khách hàng ----
        public static readonly string[] NhanCapDaiLy = new string[] { "Đại lý cấp 1", "Đại lý cấp 2" };   // CAP_DAI_LY = 1, 2
        public static readonly string[] NhanNhomKhach = new string[] { "Khách lẻ", "Khách sỉ" };          // NHOM_KH = 1, 2
        public static readonly string[] NhanKyHan = new string[] { "Theo từng phiếu", "Hàng tuần", "Hàng tháng" }; // KY_HAN = 1, 2, 3
        public const string NhanDangHoatDong = "Đang hoạt động";
        public const string NhanNgungHopTac = "Ngừng hợp tác";

        public void HienthiAutoComboBox(System.Windows.Forms.ComboBox cmb, bool loai)
        {
            cmb.DataSource = factory.DanhsachKhachHang(loai);
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
        }
        public void HienthiChungAutoComboBox(System.Windows.Forms.ComboBox cmb)
        {
            cmb.DataSource = factory.DanhsachKhachHang();
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
        }

        public void HienthiKhachHangDataGridview(
     System.Windows.Forms.DataGridView dg,
     System.Windows.Forms.BindingNavigator bn)
        {
            System.Windows.Forms.BindingSource bs = new System.Windows.Forms.BindingSource();
            DataTable tbl = factory.DanhsachKhachHang(false);

            if (tbl == null)
            {
                tbl = new DataTable();
            }
            else if (tbl.Columns.Contains("LOAI_KH"))
            {
                tbl.Columns["LOAI_KH"].DefaultValue = false;
            }

            bs.DataSource = tbl;
            bn.BindingSource = bs;
            dg.DataSource = bs;
        }

        public void HienthiKhachHangChungDataGridviewComboBox(System.Windows.Forms.DataGridViewComboBoxColumn cmb)
        {

            cmb.DataSource = factory.DanhsachKhachHang();
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_KHACH_HANG";
            cmb.HeaderText = "Khách hàng";

        }

        public void HienthiKhachHangDataGridviewComboBox(System.Windows.Forms.DataGridViewComboBoxColumn cmb)
        {

            cmb.DataSource = factory.DanhsachKhachHang(false);
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_KHACH_HANG";
            cmb.HeaderText = "Khách hàng";

        }
        public void HienthiDaiLyDataGridviewComboBox(System.Windows.Forms.DataGridViewComboBoxColumn cmb)
        {

            cmb.DataSource = factory.DanhsachKhachHang(true);
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_KHACH_HANG";
            cmb.HeaderText = "Đại lý";

        }
        public void HienthiDaiLyDataGridview(System.Windows.Forms.DataGridView dg, System.Windows.Forms.BindingNavigator bn)
        {
            System.Windows.Forms.BindingSource bs = new System.Windows.Forms.BindingSource();
            DataTable tbl = factory.DanhsachKhachHang(true);

            if (tbl == null)
            {
                tbl = new DataTable();
            }
            else if (tbl.Columns.Contains("LOAI_KH"))
            {
                tbl.Columns["LOAI_KH"].DefaultValue = true;
            }

            bs.DataSource = tbl;
            bn.BindingSource = bs;
            dg.DataSource = bs;
        }

        public void TimHoTen(String hoten, bool loai)
        {
            factory.TimHoTen(hoten, loai);
        }
        public void TimDiaChi(String diachi, bool loai)
        {
            factory.TimDiaChi(diachi, loai);
        }

        public KhachHang LayKhachHang(String id)
        {
            DataTable tbl = factory.LayKhachHang(id);
            KhachHang kh = new KhachHang();
            if (tbl.Rows.Count > 0)
            {
                kh.Id = Convert.ToString(tbl.Rows[0]["ID"]);
                kh.HoTen = Convert.ToString(tbl.Rows[0]["HO_TEN"]);
                kh.DienThoai = Convert.ToString(tbl.Rows[0]["DIEN_THOAI"]);
                kh.DiaChi = Convert.ToString(tbl.Rows[0]["DIA_CHI"]);
                kh.LoaiKH = Convert.ToBoolean(tbl.Rows[0]["LOAI_KH"]);
            }
            return kh;
        }

        public IList<KhachHang> LayDanhSachKhachHang()
        {
            DataTable tbl = factory.DanhsachKhachHang();
            IList<KhachHang> ds = new List<KhachHang>();

            foreach (DataRow row in tbl.Rows)
            {
                KhachHang kh = new KhachHang();
                kh.Id = Convert.ToString(row["ID"]);
                kh.HoTen = Convert.ToString(row["HO_TEN"]);
                kh.DienThoai = Convert.ToString(row["DIEN_THOAI"]);
                kh.DiaChi = Convert.ToString(row["DIA_CHI"]);
                kh.LoaiKH = Convert.ToBoolean(row["LOAI_KH"]);
                ds.Add(kh);
            }
            return ds;
        }
        public DataRow NewRow()
        {
            return factory.NewRow();
        }
        public void Add(DataRow row)
        {
            factory.Add(row);
        }
        public bool Save()
        {
            return factory.Save();
        }

        // ---------------------------------------------------------------
        // Dùng cho form Đại lý / Khách hàng
        // ---------------------------------------------------------------

        /// <summary>
        /// Tải danh sách đại lý (laDaiLy = true) hoặc khách hàng (false) và đặt giá trị mặc định cho dòng mới.
        /// Bảng trả về chính là bảng mà Save() sẽ cập nhật.
        /// </summary>
        public DataTable TaiBangDoiTac(bool laDaiLy)
        {
            DataTable tbl = factory.DanhsachKhachHang(laDaiLy);
            if (tbl == null) tbl = new DataTable();

            DatMacDinhCot(tbl, "LOAI_KH", laDaiLy);
            DatMacDinhCot(tbl, "HO_TEN", "");
            DatMacDinhCot(tbl, "DIA_CHI", "");
            DatMacDinhCot(tbl, "DIEN_THOAI", "");
            DatMacDinhCot(tbl, "MST", "");
            DatMacDinhCot(tbl, "EMAIL", "");
            DatMacDinhCot(tbl, "NGUOI_LIEN_HE", "");
            DatMacDinhCot(tbl, "HAN_MUC_CONG_NO", 0);
            DatMacDinhCot(tbl, "SO_NGAY_CONG_NO", laDaiLy ? 30 : 0);
            DatMacDinhCot(tbl, "KY_HAN", 1);
            DatMacDinhCot(tbl, "TRANG_THAI", true);
            DatMacDinhCot(tbl, "CAP_DAI_LY", 1);
            DatMacDinhCot(tbl, "NHOM_KH", 1);
            return tbl;
        }

        private static void DatMacDinhCot(DataTable tbl, string cot, object giaTri)
        {
            if (!tbl.Columns.Contains(cot)) return;
            DataColumn c = tbl.Columns[cot];
            try
            {
                c.DefaultValue = Convert.ChangeType(giaTri, c.DataType);
            }
            catch (Exception)
            {
                // kiểu cột không chuyển được thì bỏ qua, dòng mới để trống cột đó
            }
        }

        // ---- Quy tắc nhập liệu ----
        public const int TenToiDa = 100;
        public const int DiaChiToiDa = 200;
        public const int DienThoaiToiDa = 20;
        public const int EmailToiDa = 100;
        public const int NguoiLienHeToiDa = 100;
        public const int SoNgayCongNoToiDa = 365;

        /// <summary>Khách có tổng giá trị mua từ mức này trở lên được tô màu VIP.</summary>
        public const long NguongVip = 20000000;

        /// <summary>
        /// Số điện thoại hợp lệ: cho phép khoảng trắng, dấu chấm, gạch ngang, ngoặc;
        /// sau khi bỏ các ký tự đó phải là 0xxxxxxxx (9-11 số) hoặc +84xxxxxxxx.
        /// </summary>
        public static bool SoDienThoaiHopLe(string sdt)
        {
            if (sdt == null) return false;
            string gon = Regex.Replace(sdt.Trim(), @"[\s\.\-\(\)]", "");
            return Regex.IsMatch(gon, @"^(\+84|0)\d{8,10}$");
        }

        /// <summary>Email hợp lệ dạng ten@mien.xx (không bắt buộc nhập).</summary>
        public static bool EmailHopLe(string email)
        {
            if (email == null) return false;
            return Regex.IsMatch(email.Trim(), @"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9\-]+(\.[A-Za-z0-9\-]+)*\.[A-Za-z]{2,}$");
        }

        /// <summary>Mã số thuế: 10 số, hoặc 13 số (mã đơn vị phụ thuộc, có thể có dấu gạch: 0312345678-001).</summary>
        public static bool MstHopLe(string mst)
        {
            if (mst == null) return false;
            return Regex.IsMatch(mst.Trim(), @"^\d{10}(-?\d{3})?$");
        }

        private static string GonChuoi(string s)
        {
            return Regex.Replace(s == null ? "" : s.Trim(), @"\s+", " ");
        }

        private static string LayChuoi(DataRow r, string cot)
        {
            if (!r.Table.Columns.Contains(cot)) return "";
            object v = r[cot];
            return (v == null || v == DBNull.Value) ? "" : Convert.ToString(v);
        }

        /// <summary>
        /// Kiểm tra một ô theo tên cột CSDL. Trả về câu báo lỗi, hoặc chuỗi rỗng nếu hợp lệ.
        /// Dùng chung cho báo lỗi ngay trên ô khi nhập và cho kiểm tra trước khi Thêm / Lưu.
        /// </summary>
        public static string KiemTraTruong(string cot, string giaTri)
        {
            string v = GonChuoi(giaTri);
            decimal so;
            switch (cot)
            {
                case "HO_TEN":
                    if (v.Length == 0) return "Vui lòng nhập họ tên.";
                    if (v.Length > TenToiDa) return "Họ tên tối đa " + TenToiDa.ToString() + " ký tự.";
                    return "";
                case "DIEN_THOAI":
                    if (v.Length == 0) return "Vui lòng nhập số điện thoại.";
                    if (v.Length > DienThoaiToiDa) return "Số điện thoại tối đa " + DienThoaiToiDa.ToString() + " ký tự.";
                    if (!SoDienThoaiHopLe(v)) return "Số điện thoại \"" + v + "\" không hợp lệ.\nVí dụ: 0296 3841 234, 0901234567 hoặc +84901234567.";
                    return "";
                case "EMAIL":
                    if (v.Length == 0) return "";
                    if (v.Length > EmailToiDa) return "Email tối đa " + EmailToiDa.ToString() + " ký tự.";
                    if (!EmailHopLe(v)) return "Email \"" + v + "\" không hợp lệ.\nVí dụ: ten@congty.com";
                    return "";
                case "MST":
                    if (v.Length == 0) return "";
                    if (!MstHopLe(v)) return "Mã số thuế \"" + v + "\" không hợp lệ.\nGồm 10 số, hoặc 13 số (ví dụ 0312345678 hoặc 0312345678-001).";
                    return "";
                case "NGUOI_LIEN_HE":
                    if (v.Length > NguoiLienHeToiDa) return "Người liên hệ tối đa " + NguoiLienHeToiDa.ToString() + " ký tự.";
                    return "";
                case "DIA_CHI":
                    if (v.Length > DiaChiToiDa) return "Địa chỉ tối đa " + DiaChiToiDa.ToString() + " ký tự.";
                    return "";
                case "HAN_MUC_CONG_NO":
                    if (v.Length == 0) return "";
                    if (!decimal.TryParse(v, NumberStyles.Number, CultureInfo.CurrentCulture, out so)) return "Hạn mức công nợ phải là một số.";
                    if (so < 0) return "Hạn mức công nợ không được âm.";
                    return "";
                case "SO_NGAY_CONG_NO":
                    if (v.Length == 0) return "";
                    if (!decimal.TryParse(v, NumberStyles.Number, CultureInfo.CurrentCulture, out so)) return "Số ngày công nợ phải là một số.";
                    if (so < 0 || so > SoNgayCongNoToiDa) return "Số ngày công nợ phải từ 0 đến " + SoNgayCongNoToiDa.ToString() + ".";
                    return "";
            }
            return "";
        }

        private static readonly string[] COT_KIEM_TRA = new string[] {
            "HO_TEN", "DIEN_THOAI", "EMAIL", "MST", "NGUOI_LIEN_HE", "DIA_CHI", "HAN_MUC_CONG_NO", "SO_NGAY_CONG_NO" };

        private static readonly string[] COT_CHUOI = new string[] {
            "HO_TEN", "DIA_CHI", "NGUOI_LIEN_HE", "DIEN_THOAI", "EMAIL", "MST" };

        /// <summary>
        /// Kiểm tra các dòng vừa thêm / vừa sửa (họ tên và số điện thoại bắt buộc; số điện thoại, email, mã số thuế
        /// đúng định dạng; mã số thuế không trùng; độ dài, số không âm). Dữ liệu cũ không đụng tới nên không bị kẹt
        /// vì bản ghi cũ thiếu thông tin. Trả về dòng lỗi đầu tiên và cột cần sửa; nếu hợp lệ thì gọn khoảng trắng thừa.
        /// </summary>
        public bool KiemTraHopLe(DataTable tbl, out DataRow dongLoi, out string cotLoi, out string thongBao)
        {
            dongLoi = null;
            cotLoi = null;
            thongBao = null;

            foreach (DataRow r in tbl.Rows)
            {
                if (r.RowState != DataRowState.Added && r.RowState != DataRowState.Modified) continue;

                if (LayChuoi(r, "ID").Trim().Length == 0)
                    return Loi(r, "ID", "Chưa có mã số.", out dongLoi, out cotLoi, out thongBao);

                string ten = GonChuoi(LayChuoi(r, "HO_TEN"));
                foreach (string cot in COT_KIEM_TRA)
                {
                    if (!tbl.Columns.Contains(cot)) continue;
                    string loi = KiemTraTruong(cot, LayChuoi(r, cot));
                    if (loi.Length > 0)
                    {
                        if (ten.Length > 0 && cot != "HO_TEN") loi += "\n(Dòng: \"" + ten + "\")";
                        return Loi(r, cot, loi, out dongLoi, out cotLoi, out thongBao);
                    }
                }
            }

            // Mã số thuế không được trùng giữa các dòng trong danh sách
            if (tbl.Columns.Contains("MST"))
            {
                foreach (DataRow r in tbl.Rows)
                {
                    if (r.RowState != DataRowState.Added && r.RowState != DataRowState.Modified) continue;
                    string mst = LayChuoi(r, "MST").Trim();
                    if (mst.Length == 0) continue;

                    foreach (DataRow s in tbl.Rows)
                    {
                        if (s == r || s.RowState == DataRowState.Deleted) continue;
                        if (LayChuoi(s, "MST").Trim() == mst)
                            return Loi(r, "MST", "Mã số thuế \"" + mst + "\" đã được dùng cho \"" + GonChuoi(LayChuoi(s, "HO_TEN")) + "\".",
                                out dongLoi, out cotLoi, out thongBao);
                    }
                }
            }

            foreach (DataRow r in tbl.Rows)
            {
                if (r.RowState != DataRowState.Added && r.RowState != DataRowState.Modified) continue;
                foreach (string cot in COT_CHUOI)
                {
                    if (!tbl.Columns.Contains(cot) || tbl.Columns[cot].DataType != typeof(string)) continue;
                    string cu = LayChuoi(r, cot);
                    string moi = GonChuoi(cu);
                    if (cu != moi) r[cot] = moi;
                }
            }
            return true;
        }

        private static bool Loi(DataRow r, string cot, string noiDung, out DataRow dongLoi, out string cotLoi, out string thongBao)
        {
            dongLoi = r;
            cotLoi = cot;
            thongBao = noiDung;
            return false;
        }

        /// <summary>
        /// Khách hàng đã có phiếu bán hoặc phiếu thanh toán chưa (nếu có thì CSDL không cho xóa).
        /// Có thể ném lỗi nếu không truy vấn được CSDL.
        /// </summary>
        public bool KhachHangDaCoGiaoDich(string id, out int soPhieuBan, out int soPhieuThanhToan)
        {
            factory.DemGiaoDich(id, out soPhieuBan, out soPhieuThanhToan);
            return soPhieuBan > 0 || soPhieuThanhToan > 0;
        }

        /// <summary>
        /// Công nợ hiện tại (tổng còn nợ trên các phiếu bán trừ tổng đã thanh toán) và
        /// tổng giá trị mua của từng khách hàng, tra theo mã khách.
        /// </summary>
        public void LayThongKeKhachHang(out Dictionary<string, long> congNo, out Dictionary<string, long> tongMua)
        {
            factory.LayThongKe(out congNo, out tongMua);
        }

        // ---------------------------------------------------------------
        // Nhật ký thay đổi (audit log)
        // ---------------------------------------------------------------
        private static string TenCot(string cot)
        {
            switch (cot)
            {
                case "HO_TEN": return "Họ tên";
                case "DIA_CHI": return "Địa chỉ";
                case "DIEN_THOAI": return "Điện thoại";
                case "MST": return "Mã số thuế";
                case "EMAIL": return "Email";
                case "NGUOI_LIEN_HE": return "Người liên hệ";
                case "HAN_MUC_CONG_NO": return "Hạn mức công nợ";
                case "SO_NGAY_CONG_NO": return "Số ngày công nợ";
                case "KY_HAN": return "Kỳ hạn thanh toán";
                case "TRANG_THAI": return "Trạng thái";
                case "CAP_DAI_LY": return "Cấp đại lý";
                case "NHOM_KH": return "Nhóm khách";
            }
            return null;    // cột không ghi nhật ký (ID, LOAI_KH)
        }

        private static string NhanTheoSo(string[] nhan, object giaTri)
        {
            try
            {
                int so = Convert.ToInt32(giaTri);
                if (so >= 1 && so <= nhan.Length) return nhan[so - 1];
            }
            catch (Exception) { }
            return Convert.ToString(giaTri);
        }

        private static string HienGiaTri(string cot, object v)
        {
            if (v == null || v == DBNull.Value) return "(trống)";
            string s = Convert.ToString(v);
            if (s.Length == 0) return "(trống)";
            try
            {
                switch (cot)
                {
                    case "TRANG_THAI": return Convert.ToBoolean(v) ? NhanDangHoatDong : NhanNgungHopTac;
                    case "CAP_DAI_LY": return NhanTheoSo(NhanCapDaiLy, v);
                    case "NHOM_KH": return NhanTheoSo(NhanNhomKhach, v);
                    case "KY_HAN": return NhanTheoSo(NhanKyHan, v);
                    case "HAN_MUC_CONG_NO": return Convert.ToDecimal(v).ToString("N0");
                }
            }
            catch (Exception) { }
            return s;
        }

        private static string TomTat(DataRow r, DataRowVersion ban)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataColumn c in r.Table.Columns)
            {
                string ten = TenCot(c.ColumnName);
                if (ten == null) continue;
                object v = r[c, ban];
                if (v == null || v == DBNull.Value || Convert.ToString(v).Length == 0) continue;
                if (sb.Length > 0) sb.Append("; ");
                sb.Append(ten).Append(": ").Append(HienGiaTri(c.ColumnName, v));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Lập danh sách nhật ký từ các dòng thêm / sửa / xóa chưa lưu. Phải gọi TRƯỚC Save()
        /// (sau khi lưu bảng không còn biết dòng nào đã thay đổi).
        /// </summary>
        public IList<NhatKyKhachHang> TaoNhatKy(DataTable tbl)
        {
            IList<NhatKyKhachHang> ds = new List<NhatKyKhachHang>();
            foreach (DataRow r in tbl.Rows)
            {
                if (r.RowState == DataRowState.Added)
                {
                    NhatKyKhachHang n = new NhatKyKhachHang();
                    n.HanhDong = "Thêm";
                    n.IdKhach = Convert.ToString(r["ID"]);
                    n.TenKhach = LayChuoi(r, "HO_TEN");
                    n.NoiDung = TomTat(r, DataRowVersion.Current);
                    ds.Add(n);
                }
                else if (r.RowState == DataRowState.Modified)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (DataColumn c in tbl.Columns)
                    {
                        string ten = TenCot(c.ColumnName);
                        if (ten == null) continue;
                        object cu = r[c, DataRowVersion.Original];
                        object moi = r[c, DataRowVersion.Current];
                        string scu = (cu == null || cu == DBNull.Value) ? "" : Convert.ToString(cu);
                        string smoi = (moi == null || moi == DBNull.Value) ? "" : Convert.ToString(moi);
                        if (scu == smoi) continue;
                        if (sb.Length > 0) sb.Append("; ");
                        sb.Append(ten).Append(": ").Append(HienGiaTri(c.ColumnName, cu)).Append(" → ").Append(HienGiaTri(c.ColumnName, moi));
                    }
                    if (sb.Length == 0) continue;   // chỉ đổi khoảng trắng / không đổi gì thực sự

                    NhatKyKhachHang n = new NhatKyKhachHang();
                    n.HanhDong = "Sửa";
                    n.IdKhach = Convert.ToString(r["ID"]);
                    n.TenKhach = LayChuoi(r, "HO_TEN");
                    n.NoiDung = sb.ToString();
                    ds.Add(n);
                }
                else if (r.RowState == DataRowState.Deleted)
                {
                    NhatKyKhachHang n = new NhatKyKhachHang();
                    n.HanhDong = "Xóa";
                    n.IdKhach = Convert.ToString(r["ID", DataRowVersion.Original]);
                    object ten = r["HO_TEN", DataRowVersion.Original];
                    n.TenKhach = (ten == null || ten == DBNull.Value) ? "" : Convert.ToString(ten);
                    n.NoiDung = TomTat(r, DataRowVersion.Original);
                    ds.Add(n);
                }
            }
            return ds;
        }

        public void GhiNhatKy(IList<NhatKyKhachHang> ds, string nguoiDung)
        {
            factory.GhiNhatKy(ds, nguoiDung);
        }

        public DataTable LayNhatKy(string idKhach)
        {
            return factory.LayNhatKy(idKhach);
        }
    }
}