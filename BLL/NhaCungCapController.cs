using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;


namespace CuahangNongduoc.Controller
{
    public class NhaCungCapController
    {
        NhaCungCapFactory factory = new  NhaCungCapFactory();

        public void HienthiAutoComboBox(System.Windows.Forms.ComboBox cmb)
        {
            cmb.DataSource = factory.DanhsachNCC();
            cmb.DisplayMember = "HO_TEN";
            cmb.ValueMember = "ID";
            
        }
        public void HienthiAllComboBox(System.Windows.Forms.ComboBox cmb)
        {
            IList<NhaCungCap> ds = this.LayDanhSachNCC();
            ds.Add(new NhaCungCap("ALL","Tất cả"));
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

        // ------------------------------------------------------------------
        // Quản lý dữ liệu đang hiển thị
        // ------------------------------------------------------------------

        /// <summary>Tải lại toàn bộ danh sách (hủy các thay đổi chưa lưu).</summary>
        public void TaiLai()
        {
            factory.DanhsachNCC();
        }

        /// <summary>Tổng số nhà cung cấp trong CSDL.</summary>
        public int DemTong()
        {
            return factory.DemTong();
        }

        /// <summary>Đếm số dòng thêm mới / sửa / xóa đang chờ lưu.</summary>
        public void DemThayDoi(out int them, out int sua, out int xoa)
        {
            them = 0; sua = 0; xoa = 0;
            foreach (DataRow row in factory.Bang.Rows)
            {
                switch (row.RowState)
                {
                    case DataRowState.Added: them++; break;
                    case DataRowState.Modified: sua++; break;
                    case DataRowState.Deleted: xoa++; break;
                }
            }
        }

        /// <summary>Bỏ toàn bộ thay đổi chưa lưu.</summary>
        public void HuyThayDoi()
        {
            factory.Bang.RejectChanges();
        }

        // ------------------------------------------------------------------
        // Lọc nâng cao (biểu thức RowFilter cho BindingSource)
        // ------------------------------------------------------------------

        /// <summary>
        /// Tạo biểu thức lọc "chứa" theo tên / địa chỉ / điện thoại.
        /// Trả về null nếu không nhập điều kiện nào.
        /// </summary>
        public static string TaoBieuThucLoc(string hoTen, string diaChi, string dienThoai)
        {
            List<string> dk = new List<string>();
            ThemDieuKienLike(dk, NhaCungCapValidator.CotHoTen, NhaCungCapValidator.ChuanHoaChuoi(hoTen));
            ThemDieuKienLike(dk, NhaCungCapValidator.CotDiaChi, NhaCungCapValidator.ChuanHoaChuoi(diaChi));
            ThemDieuKienLike(dk, NhaCungCapValidator.CotDienThoai, NhaCungCapValidator.ChuanHoaDienThoai(dienThoai));
            if (dk.Count == 0) return null;
            return string.Join(" AND ", dk.ToArray());
        }

        private static void ThemDieuKienLike(List<string> dk, string cot, string giaTri)
        {
            if (giaTri.Length == 0) return;
            dk.Add(cot + " LIKE '%" + ThoatLike(giaTri) + "%'");
        }

        // Thoát ký tự đặc biệt của DataColumn.Expression: ' [ ] * %
        private static string ThoatLike(string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                switch (c)
                {
                    case '\'': sb.Append("''"); break;
                    case '[': sb.Append("[[]"); break;
                    case ']': sb.Append("[]]"); break;
                    case '*': sb.Append("[*]"); break;
                    case '%': sb.Append("[%]"); break;
                    default: sb.Append(c); break;
                }
            }
            return sb.ToString();
        }

        // ------------------------------------------------------------------
        // Xác thực trước khi lưu
        // ------------------------------------------------------------------

        /// <summary>
        /// Kiểm tra các dòng thêm mới / đã sửa: bắt buộc nhập, đúng định dạng, không trùng.
        /// Đồng thời ghi lại giá trị đã chuẩn hóa (cắt khoảng trắng, số điện thoại dạng 0xxxxxxxxx).
        /// Trả về thông báo lỗi đầu tiên (null nếu hợp lệ); idLoi/cotLoi cho biết dòng và cột lỗi.
        /// </summary>
        public string KiemTraTruocKhiLuu(out string idLoi, out string cotLoi)
        {
            idLoi = null;
            cotLoi = null;

            foreach (DataRow row in factory.Bang.Rows)
            {
                if (row.RowState != DataRowState.Added && row.RowState != DataRowState.Modified)
                    continue;

                string hoTen = NhaCungCapValidator.ChuanHoaChuoi(Convert.ToString(row["HO_TEN"]));
                string diaChi = NhaCungCapValidator.ChuanHoaChuoi(Convert.ToString(row["DIA_CHI"]));
                string dienThoai = NhaCungCapValidator.ChuanHoaDienThoai(Convert.ToString(row["DIEN_THOAI"]));

                string loi = NhaCungCapValidator.KiemTra(hoTen, diaChi, dienThoai, out cotLoi);
                if (loi == null)
                    loi = TimTrung(row, hoTen, dienThoai, out cotLoi);

                if (loi != null)
                {
                    idLoi = Convert.ToString(row["ID"]);
                    return loi;
                }

                GanNeuKhac(row, "HO_TEN", hoTen);
                GanNeuKhac(row, "DIA_CHI", diaChi);
                GanNeuKhac(row, "DIEN_THOAI", dienThoai);
            }
            return null;
        }

        private static void GanNeuKhac(DataRow row, string cot, string giaTri)
        {
            if (Convert.ToString(row[cot]) != giaTri)
                row[cot] = giaTri;
        }

        /// <summary>
        /// Tìm nhà cung cấp trùng (cùng tên hoặc cùng số điện thoại) với dòng đang kiểm tra.
        /// Bước 1: so với các dòng đang có trên lưới (kể cả dòng chưa lưu).
        /// Bước 2: so với CSDL, nhưng bỏ qua các dòng đã có trên lưới (đã so ở bước 1 theo giá trị hiện tại).
        /// </summary>
        private string TimTrung(DataRow row, string hoTen, string dienThoai, out string cotLoi)
        {
            cotLoi = null;
            string id = Convert.ToString(row["ID"]);

            foreach (DataRow r in factory.Bang.Rows)
            {
                if (object.ReferenceEquals(r, row) || r.RowState == DataRowState.Deleted)
                    continue;

                string tenKhac = NhaCungCapValidator.ChuanHoaChuoi(Convert.ToString(r["HO_TEN"]));
                if (string.Equals(tenKhac, hoTen, StringComparison.OrdinalIgnoreCase))
                {
                    cotLoi = NhaCungCapValidator.CotHoTen;
                    return "Nhà cung cấp \"" + hoTen + "\" đã tồn tại (Mã số " + Convert.ToString(r["ID"]) + ").";
                }

                string dtKhac = NhaCungCapValidator.ChuanHoaDienThoai(Convert.ToString(r["DIEN_THOAI"]));
                if (dtKhac.Length > 0 && dtKhac == dienThoai)
                {
                    cotLoi = NhaCungCapValidator.CotDienThoai;
                    return "Số điện thoại " + dienThoai + " đã thuộc nhà cung cấp \"" + tenKhac + "\" (Mã số " + Convert.ToString(r["ID"]) + ").";
                }
            }

            foreach (DataRow d in factory.TimTrungHoTen(hoTen, id).Rows)
            {
                if (TimDongTheoId(Convert.ToString(d["ID"])) != null) continue;
                cotLoi = NhaCungCapValidator.CotHoTen;
                return "Nhà cung cấp \"" + hoTen + "\" đã tồn tại (Mã số " + Convert.ToString(d["ID"]) + ").";
            }

            foreach (DataRow d in factory.TimTrungDienThoai(dienThoai, id).Rows)
            {
                if (TimDongTheoId(Convert.ToString(d["ID"])) != null) continue;
                cotLoi = NhaCungCapValidator.CotDienThoai;
                return "Số điện thoại " + dienThoai + " đã thuộc nhà cung cấp \"" + Convert.ToString(d["HO_TEN"]) + "\" (Mã số " + Convert.ToString(d["ID"]) + ").";
            }

            return null;
        }

        private DataRow TimDongTheoId(string id)
        {
            foreach (DataRow r in factory.Bang.Rows)
            {
                string rid = (r.RowState == DataRowState.Deleted)
                    ? Convert.ToString(r["ID", DataRowVersion.Original])
                    : Convert.ToString(r["ID"]);
                if (rid == id) return r;
            }
            return null;
        }
    }
}
