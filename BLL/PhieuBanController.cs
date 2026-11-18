using System;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    public class PhieuBanController
    {
        PhieuBanFactory factory = new PhieuBanFactory();
        BindingSource bs = new BindingSource();

        public PhieuBanController()
        {
            bs.DataSource = factory.LayPhieuBan("-1");
        }

        public DataRow NewRow()
        {
            return factory.BangDangSoan.NewRow();
        }

        public void Add(DataRow row)
        {
            factory.BangDangSoan.Rows.Add(row);
        }

        // Form Bán lẻ / Bán sỉ gọi khi Lưu (đang sửa)
        public void Update()
        {
            try { bs.EndEdit(); } catch { }
            try { bs.MoveNext(); } catch { }
            factory.Save();
        }

        /// <summary>
        /// Lưu các thay đổi của bảng phiếu bán.
        /// REFACTOR #42 Add Parameter – thêm tham số nguoiLap để ghi ai lập phiếu vào cột ID_NGUOI_DUNG
        /// (phục vụ thống kê hóa đơn theo nhân viên). Truyền null nếu không xác định.
        /// </summary>
        public void Save(CuahangNongduoc.Entities.NguoiDung nguoiLap)
        {
            GhiNguoiLap(nguoiLap);

            // Sửa lỗi: bản cũ còn giới hạn "bản dùng thử" – từ phiếu thứ 51 không lưu (và cũng không xóa)
            // được phiếu nào nhưng chi tiết vẫn bị ghi, gây lỗi khóa ngoại. Mỗi lần lưu còn hiện thêm hộp thoại.
            factory.Save();
        }

        private void GhiNguoiLap(CuahangNongduoc.Entities.NguoiDung nguoiLap)
        {
            DataTable bang = factory.BangDangSoan;
            if (nguoiLap == null || !nguoiLap.LaNguoiDungThat || !bang.Columns.Contains("ID_NGUOI_DUNG"))
                return;
            foreach (DataRow dong in bang.Rows)
            {
                if (dong.RowState == DataRowState.Added)
                    dong["ID_NGUOI_DUNG"] = nguoiLap.ID;
            }
        }

        public void HienthiPhieuBanLe(BindingNavigator bn, DataGridView dg)
        {
            bs.DataSource = factory.DanhsachPhieuBanLe();
            bn.BindingSource = bs;
            dg.AutoGenerateColumns = false;
            dg.DataSource = bs;
        }

        public void HienthiPhieuBanSi(BindingNavigator bn, DataGridView dg)
        {
            bs.DataSource = factory.DanhsachPhieuBanSi();
            bn.BindingSource = bs;
            dg.AutoGenerateColumns = false;
            dg.DataSource = bs;
        }

        /// <summary>
        /// Hiển thị phiếu bán lên form chi tiết.
        /// isBanSi = true: danh sách bán sỉ; false: bán lẻ.
        /// Nếu bs đang trống (mở form mới) sẽ nạp lại từ CSDL.
        /// </summary>
        public void HienthiPhieuBan(BindingNavigator bn, ComboBox cmb, TextBox txt, DateTimePicker dt,
            NumericUpDown numTongTien, NumericUpDown numDatra, NumericUpDown numConNo)
        {
            HienthiPhieuBan(bn, cmb, txt, dt, numTongTien, numDatra, numConNo, false);
        }

        public void HienthiPhieuBan(BindingNavigator bn, ComboBox cmb, TextBox txt, DateTimePicker dt,
            NumericUpDown numTongTien, NumericUpDown numDatra, NumericUpDown numConNo, bool isBanSi)
        {
            // Luôn nạp danh sách phù hợp để BindingNavigator không bị 0 of 0
            try
            {
                if (isBanSi)
                    bs.DataSource = factory.DanhsachPhieuBanSi();
                else
                    bs.DataSource = factory.DanhsachPhieuBanLe();
            }
            catch { }

            if (bn != null)
                bn.BindingSource = bs;

            if (txt != null)
            {
                txt.DataBindings.Clear();
                txt.DataBindings.Add("Text", bs, "ID");
            }
            if (cmb != null)
            {
                cmb.DataBindings.Clear();
                cmb.DataBindings.Add("SelectedValue", bs, "ID_KHACH_HANG");
            }
            if (dt != null)
            {
                dt.DataBindings.Clear();
                dt.DataBindings.Add("Value", bs, "NGAY_BAN");
            }
            if (numTongTien != null)
            {
                numTongTien.DataBindings.Clear();
                numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN");
            }
            if (numDatra != null)
            {
                numDatra.DataBindings.Clear();
                numDatra.DataBindings.Add("Value", bs, "DA_TRA");
            }
            if (numConNo != null)
            {
                numConNo.DataBindings.Clear();
                numConNo.DataBindings.Add("Value", bs, "CON_NO");
            }
        }

        public PhieuBan LayPhieuBan(String id)
        {
            DataTable tbl = factory.LayPhieuBan(id);
            PhieuBan ph = null;
            if (tbl.Rows.Count > 0)
            {
                ph = new PhieuBan(Convert.ToString(tbl.Rows[0]["ID"]));
                ph.NgayBan = Convert.ToDateTime(tbl.Rows[0]["NGAY_BAN"]);
                ph.TongTien = Convert.ToInt64(tbl.Rows[0]["TONG_TIEN"]);
                ph.DaTra = Convert.ToInt64(tbl.Rows[0]["DA_TRA"]);
                ph.ConNo = Convert.ToInt64(tbl.Rows[0]["CON_NO"]);
                KhachHangController ctrlKH = new KhachHangController();
                ph.KhachHang = ctrlKH.LayKhachHang(Convert.ToString(tbl.Rows[0]["ID_KHACH_HANG"]));
                ChiTietPhieuBanController ctrl = new ChiTietPhieuBanController();
                ph.GanChiTiet(ctrl.ChiTietPhieuBan(ph.Id));
            }
            return ph;
        }

        // Tìm + gán lại lưới (sửa lỗi 0 of 0)
        public int TimPhieuBan(String maKH, DateTime dt)
        {
            DataTable tbl = factory.TimPhieuBan(maKH, dt);
            bs.DataSource = tbl;
            return tbl != null ? tbl.Rows.Count : 0;
        }

        public int TimPhieuBanTheoKhach(String maKH)
        {
            DataTable tbl = factory.TimPhieuBanTheoKhach(maKH);
            bs.DataSource = tbl;
            return tbl != null ? tbl.Rows.Count : 0;
        }

        public int SoDongHienTai()
        {
            try { return bs != null ? bs.Count : 0; }
            catch { return 0; }
        }
    
        // ===== PM-36 Doanh thu =====
        public DataTable ThongKeDoanhThu(DateTime tuNgay, DateTime denNgay, string idKhachHang, bool? banSi)
        {
            return factory.ThongKeDoanhThu(tuNgay, denNgay, idKhachHang, banSi);
        }

        public DataTable DoanhThuTheoNgay(DateTime tuNgay, DateTime denNgay)
        {
            return factory.DoanhThuTheoNgay(tuNgay, denNgay);
        }

        public int TimPhieuBanKhoang(String maKH, DateTime tu, DateTime den)
        {
            DataTable tbl = factory.TimPhieuBanKhoang(maKH, tu, den);
            bs.DataSource = tbl;
            return tbl != null ? tbl.Rows.Count : 0;
        }

    }
}
