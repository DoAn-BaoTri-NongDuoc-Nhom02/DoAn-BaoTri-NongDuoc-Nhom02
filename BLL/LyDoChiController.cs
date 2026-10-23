using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;


namespace CuahangNongduoc.Controller
{
    public class LyDoChiController
    {
        LyDoChiFactory factory = new LyDoChiFactory();

        public void HienthiAutoComboBox(System.Windows.Forms.ComboBox cmb)
        {
            cmb.DataSource = factory.DanhsachLyDo();
            cmb.DisplayMember = "LY_DO";
            cmb.ValueMember = "ID";
        }

        public void HienthiDataGridview(System.Windows.Forms.DataGridView dg, System.Windows.Forms.BindingNavigator bn)
        {
            System.Windows.Forms.BindingSource bs = new System.Windows.Forms.BindingSource();
            DataTable tbl = factory.DanhsachLyDo();
            bs.DataSource = tbl;
            bn.BindingSource = bs;
            dg.DataSource = bs;

        }

        public void HienthiDataGridviewComboBox(System.Windows.Forms.DataGridViewComboBoxColumn cmb)
        {

            cmb.DataSource = factory.DanhsachLyDo();
            cmb.DisplayMember = "LY_DO";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_LY_DO_CHI";
            cmb.HeaderText = "Lý do chi";

        }

        public LyDoChi LayLyDoChi(long id)
        {
            DataTable tbl = factory.LayLyDoChi(id);
            LyDoChi lydo = new LyDoChi();
            if (tbl.Rows.Count > 0)
            {
                lydo.Id = Convert.ToInt64(tbl.Rows[0]["ID"]);
                lydo.LyDo = Convert.ToString(tbl.Rows[0]["LY_DO"]);
            }
            return lydo;
        }

        public bool Save()
        {
            return factory.Save();
        }

        /// <summary>Độ dài tối đa của một lý do chi (khớp MaxInputLength của cột trên form).</summary>
        public const int DoDaiToiDa = 100;

        private static readonly string[] LY_DO_MAU = new string[] {
            "Tiền điện",
            "Tiền nước",
            "Tiền thuê mặt bằng",
            "Lương nhân viên",
            "Chi phí vận chuyển",
            "Mua văn phòng phẩm",
            "Sửa chữa, bảo trì",
            "Chi phí khác"
        };

        /// <summary>
        /// Nếu bảng LY_DO_CHI đang trống thì nạp sẵn một số lý do chi mẫu.
        /// Trả về true khi đã nạp.
        /// </summary>
        public bool KhoiTaoDuLieuMau()
        {
            DataTable tbl = factory.DanhsachLyDo();
            if (tbl.Rows.Count > 0) return false;

            foreach (string ten in LY_DO_MAU)
            {
                DataRow r = tbl.NewRow();
                r["LY_DO"] = ten;
                tbl.Rows.Add(r);
            }
            return factory.Save();
        }

        /// <summary>Lý do chi này đã có phiếu chi sử dụng chưa (nếu có thì không được xóa).</summary>
        public bool LyDoDaSuDung(long id)
        {
            return factory.DaSuDung(id);
        }

        /// <summary>
        /// Kiểm tra dữ liệu trước khi lưu: không để trống, không quá dài, không trùng tên.
        /// Đồng thời cắt khoảng trắng thừa. Trả về false và dòng lỗi đầu tiên nếu không hợp lệ.
        /// </summary>
        public bool KiemTraHopLe(DataTable tbl, out DataRow dongLoi, out string thongBao)
        {
            dongLoi = null;
            thongBao = null;

            // Đếm số lần xuất hiện của từng tên (không phân biệt hoa thường) trên các dòng còn lại
            Dictionary<string, int> soLan = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
            foreach (DataRow r in tbl.Rows)
            {
                if (r.RowState == DataRowState.Deleted || r.RowState == DataRowState.Detached) continue;
                string ten = Convert.ToString(r["LY_DO"]).Trim();
                if (ten.Length == 0) continue;
                if (soLan.ContainsKey(ten)) soLan[ten]++; else soLan.Add(ten, 1);
            }

            foreach (DataRow r in tbl.Rows)
            {
                if (r.RowState == DataRowState.Deleted || r.RowState == DataRowState.Detached) continue;

                string ten = Convert.ToString(r["LY_DO"]).Trim();
                if (ten.Length == 0)
                {
                    dongLoi = r;
                    thongBao = "Vui lòng nhập \"Lý do\" cho dòng đang chọn, hoặc xóa dòng đó đi.";
                    return false;
                }
                if (ten.Length > DoDaiToiDa)
                {
                    dongLoi = r;
                    thongBao = "\"Lý do\" tối đa " + DoDaiToiDa.ToString() + " ký tự.";
                    return false;
                }
                // Chỉ chặn trùng với dòng mới thêm / vừa sửa, tránh kẹt vì dữ liệu cũ
                if ((r.RowState == DataRowState.Added || r.RowState == DataRowState.Modified) && soLan[ten] > 1)
                {
                    dongLoi = r;
                    thongBao = "Lý do chi \"" + ten + "\" đã tồn tại. Vui lòng nhập tên khác.";
                    return false;
                }
            }

            // Hợp lệ: chuẩn hóa khoảng trắng thừa
            foreach (DataRow r in tbl.Rows)
            {
                if (r.RowState == DataRowState.Deleted || r.RowState == DataRowState.Detached) continue;
                string goc = Convert.ToString(r["LY_DO"]);
                string gon = goc.Trim();
                if (gon != goc) r["LY_DO"] = gon;
            }
            return true;
        }
    }
}