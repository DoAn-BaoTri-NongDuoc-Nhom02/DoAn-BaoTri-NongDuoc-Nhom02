using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;


namespace CuahangNongduoc.Controller
{
    public class NhaCungCapController
    {
        NhaCungCapFactory factory = new NhaCungCapFactory();

        public void HienthiAutoComboBox(System.Windows.Forms.ComboBox cmb)
        {
            cmb.DataSource = factory.DanhsachNCC();
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";

        }
        public void HienthiAllComboBox(System.Windows.Forms.ComboBox cmb)
        {
            IList<NhaCungCap> ds = this.LayDanhSachNCC();
            ds.Add(new NhaCungCap("ALL", "Tất cả"));
            cmb.DataSource = ds;
            cmb.DisplayMember = "HoTen";
            cmb.ValueMember = "Id";

        }

        public void HienthiDataGridview(System.Windows.Forms.DataGridView dg, System.Windows.Forms.BindingNavigator bn)
        {
            System.Windows.Forms.BindingSource bs = new System.Windows.Forms.BindingSource();
            DataTable tbl = factory.DanhsachNCC();
            bs.DataSource = tbl;
            bn.BindingSource = bs;
            dg.DataSource = bs;

        }

        public void HienthiDataGridviewComboBox(System.Windows.Forms.DataGridViewComboBoxColumn cmb)
        {

            cmb.DataSource = factory.DanhsachNCC();
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_NHA_CUNG_CAP";
            cmb.HeaderText = "Nhà cung cấp";

        }

        public NhaCungCap LayNCC(String id)
        {
            DataTable tbl = factory.LayNCC(id);
            NhaCungCap ncc = new NhaCungCap();
            if (tbl.Rows.Count > 0)
            {
                ncc.Id = Convert.ToString(tbl.Rows[0]["ID"]);
                ncc.HoTen = Convert.ToString(tbl.Rows[0]["HO_TEN"]);
                ncc.DienThoai = Convert.ToString(tbl.Rows[0]["DIEN_THOAI"]);
                ncc.DiaChi = Convert.ToString(tbl.Rows[0]["DIA_CHI"]);
            }
            return ncc;
        }

        public IList<NhaCungCap> LayDanhSachNCC()
        {
            DataTable tbl = factory.DanhsachNCC();
            IList<NhaCungCap> ds = new List<NhaCungCap>();

            foreach (DataRow row in tbl.Rows)
            {
                NhaCungCap kh = new NhaCungCap();
                kh.Id = Convert.ToString(row["ID"]);
                kh.HoTen = Convert.ToString(row["HO_TEN"]);
                kh.DienThoai = Convert.ToString(row["DIEN_THOAI"]);
                kh.DiaChi = Convert.ToString(row["DIA_CHI"]);
                ds.Add(kh);
            }
            return ds;
        }

        public void TimDiaChi(String diachi)
        {
            factory.TimDiaChi(diachi);
        }
        public void TimHoTen(String hoten)
        {
            factory.TimHoTen(hoten);
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

        // Độ dài tối đa của từng trường (khớp MaxInputLength của cột trên form)
        public const int TenToiDa = 100;
        public const int DiaChiToiDa = 200;
        public const int DienThoaiToiDa = 20;

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

        /// <summary>Nhà cung cấp này đã có phiếu nhập hàng chưa (nếu có thì không được xóa).</summary>
        public bool NhaCungCapDaSuDung(string id)
        {
            return factory.DaSuDung(id);
        }

        /// <summary>
        /// Kiểm tra dữ liệu trước khi lưu: tên bắt buộc, độ dài, số điện thoại đúng định dạng
        /// (nếu có nhập), mã số không trống/không trùng, nhà cung cấp không trùng (tên + địa chỉ).
        /// Đồng thời gọn khoảng trắng thừa. Trả về dòng lỗi đầu tiên và cột cần sửa.
        /// </summary>
        public bool KiemTraHopLe(DataTable tbl, out DataRow dongLoi, out string cotLoi, out string thongBao)
        {
            dongLoi = null;
            cotLoi = null;
            thongBao = null;

            Dictionary<string, int> soLanKhoa = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
            Dictionary<string, int> soLanMa = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
            foreach (DataRow r in tbl.Rows)
            {
                if (r.RowState == DataRowState.Deleted || r.RowState == DataRowState.Detached) continue;
                string khoa = GonChuoi(Convert.ToString(r["HO_TEN"])) + "|" + GonChuoi(Convert.ToString(r["DIA_CHI"]));
                if (soLanKhoa.ContainsKey(khoa)) soLanKhoa[khoa]++; else soLanKhoa.Add(khoa, 1);
                string ma = Convert.ToString(r["ID"]).Trim();
                if (soLanMa.ContainsKey(ma)) soLanMa[ma]++; else soLanMa.Add(ma, 1);
            }

            foreach (DataRow r in tbl.Rows)
            {
                if (r.RowState == DataRowState.Deleted || r.RowState == DataRowState.Detached) continue;

                string ma = Convert.ToString(r["ID"]).Trim();
                string ten = GonChuoi(Convert.ToString(r["HO_TEN"]));
                string diaChi = GonChuoi(Convert.ToString(r["DIA_CHI"]));
                string sdt = Convert.ToString(r["DIEN_THOAI"]).Trim();

                if (ma.Length == 0)
                    return Loi(r, "ID", "Nhà cung cấp chưa có mã số.", out dongLoi, out cotLoi, out thongBao);
                if (soLanMa[ma] > 1)
                    return Loi(r, "ID", "Mã số \"" + ma + "\" bị trùng.", out dongLoi, out cotLoi, out thongBao);

                if (ten.Length == 0)
                    return Loi(r, "HO_TEN", "Vui lòng nhập tên nhà cung cấp, hoặc xóa dòng đó đi.", out dongLoi, out cotLoi, out thongBao);
                if (ten.Length > TenToiDa)
                    return Loi(r, "HO_TEN", "Tên nhà cung cấp tối đa " + TenToiDa.ToString() + " ký tự.", out dongLoi, out cotLoi, out thongBao);
                if (diaChi.Length > DiaChiToiDa)
                    return Loi(r, "DIA_CHI", "Địa chỉ tối đa " + DiaChiToiDa.ToString() + " ký tự.", out dongLoi, out cotLoi, out thongBao);

                if (sdt.Length > 0)
                {
                    if (sdt.Length > DienThoaiToiDa)
                        return Loi(r, "DIEN_THOAI", "Số điện thoại tối đa " + DienThoaiToiDa.ToString() + " ký tự.", out dongLoi, out cotLoi, out thongBao);
                    if (!SoDienThoaiHopLe(sdt))
                        return Loi(r, "DIEN_THOAI", "Số điện thoại \"" + sdt + "\" không hợp lệ.\nVí dụ: 0296 3841 234, 0901234567 hoặc +84901234567.", out dongLoi, out cotLoi, out thongBao);
                }

                // Chỉ chặn trùng với dòng mới thêm / vừa sửa, tránh kẹt vì dữ liệu cũ
                if ((r.RowState == DataRowState.Added || r.RowState == DataRowState.Modified) && soLanKhoa[ten + "|" + diaChi] > 1)
                    return Loi(r, "HO_TEN", "Nhà cung cấp \"" + ten + "\" (cùng địa chỉ) đã tồn tại.", out dongLoi, out cotLoi, out thongBao);
            }

            // Hợp lệ: chuẩn hóa khoảng trắng thừa
            foreach (DataRow r in tbl.Rows)
            {
                if (r.RowState == DataRowState.Deleted || r.RowState == DataRowState.Detached) continue;
                CapNhatNeuKhac(r, "HO_TEN", GonChuoi(Convert.ToString(r["HO_TEN"])));
                CapNhatNeuKhac(r, "DIA_CHI", GonChuoi(Convert.ToString(r["DIA_CHI"])));
                CapNhatNeuKhac(r, "DIEN_THOAI", Convert.ToString(r["DIEN_THOAI"]).Trim());
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

        private static string GonChuoi(string s)
        {
            return Regex.Replace(s == null ? "" : s.Trim(), @"\s+", " ");
        }

        private static void CapNhatNeuKhac(DataRow r, string cot, string giaTriMoi)
        {
            if (Convert.ToString(r[cot]) != giaTriMoi) r[cot] = giaTriMoi;
        }
    }
}