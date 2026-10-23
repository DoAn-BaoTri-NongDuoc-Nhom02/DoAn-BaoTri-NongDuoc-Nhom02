using System;
using System.Collections.Generic;
using System.Text;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.BusinessObject;
using System.Windows.Forms;
using System.Data;

namespace CuahangNongduoc.Controller
{

    public class PhieuNhapController
    {
        PhieuNhapFactory factory = new PhieuNhapFactory();
        BindingSource bs = new BindingSource();

        public PhieuNhapController()
        {
            // Lỗi kết nối không được làm hỏng việc mở form: form sẽ tải lại và báo lỗi rõ ràng sau đó
            try
            {
                bs.DataSource = factory.LayPhieuNhap("-1");
            }
            catch (Exception)
            {
            }
        }

        public DataRow NewRow()
        {
            return factory.NewRow();
        }
        public void Add(DataRow row)
        {
            factory.Add(row);
        }

        public void Update()
        {
            bs.MoveNext();
            factory.Save();
        }
        public void Save()
        {
            factory.Save();
        }


        public PhieuNhap LayPhieuNhap(String id)
        {
            // Dùng factory riêng để việc tra cứu không làm mất danh sách phiếu đang hiển thị
            PhieuNhapFactory tra = new PhieuNhapFactory();
            DataTable tbl = tra.LayPhieuNhap(id);
            PhieuNhap ph = null;
            NhaCungCapController ctrlNCC = new NhaCungCapController();
            if (tbl.Rows.Count > 0)
            {

                ph = new PhieuNhap();
                ph.Id = Convert.ToString(tbl.Rows[0]["ID"]);
                ph.NgayNhap = Convert.ToDateTime(tbl.Rows[0]["NGAY_NHAP"]);
                ph.TongTien = SoNguyen(tbl.Rows[0]["TONG_TIEN"]);
                ph.DaTra = SoNguyen(tbl.Rows[0]["DA_TRA"]);
                ph.ConNo = SoNguyen(tbl.Rows[0]["CON_NO"]);
                ph.NhaCungCap = ctrlNCC.LayNCC(Convert.ToString(tbl.Rows[0]["ID_NHA_CUNG_CAP"]));
                MaSanPhamController ctrl = new MaSanPhamController();
                ph.ChiTiet = ctrl.ChiTietPhieuNhap(ph.Id);
            }
            return ph;
        }

        private static long SoNguyen(object o)
        {
            return (o == null || o == DBNull.Value) ? 0 : Convert.ToInt64(o);
        }

        public void HienthiPhieuNhap(BindingNavigator bn, DataGridView dg)
        {

            bs.DataSource = factory.DanhsachPhieuNhap();
            bn.BindingSource = bs;
            dg.DataSource = bs;
        }

        public void HienthiPhieuNhap(BindingNavigator bn, TextBox txt, ComboBox cmb,
    DateTimePicker dt, NumericUpDown numTongTien, NumericUpDown numDaTra, NumericUpDown numConNo)
        {
            // 1. Load data TRƯỚC
            bs.DataSource = factory.DanhsachPhieuNhap();
            bn.BindingSource = bs;

            // 2. Mới bind control
            txt.DataBindings.Clear();
            txt.DataBindings.Add("Text", bs, "ID");

            cmb.DataBindings.Clear();
            cmb.DataBindings.Add("SelectedValue", bs, "ID_NHA_CUNG_CAP");

            dt.DataBindings.Clear();
            dt.DataBindings.Add("Value", bs, "NGAY_NHAP");

            numTongTien.DataBindings.Clear();
            numTongTien.DataBindings.Add("Value", bs, "TONG_TIEN");

            numDaTra.DataBindings.Clear();
            numDaTra.DataBindings.Add("Value", bs, "DA_TRA");

            numConNo.DataBindings.Clear();
            numConNo.DataBindings.Add("Value", bs, "CON_NO");
        }

        public void TimPhieuNhap(String maNCC, DateTime dt)
        {
            DataTable tbl = factory.TimPhieuNhap(maNCC, dt);
            bs.DataSource = tbl;
        }

        /// <summary>PM-15: Tim theo nhieu tieu chi. Tra ve so dong.</summary>
        public int TimPhieuNhapNangCao(string maPhieu, string idNcc, DateTime? ngayNhap,
            decimal? tongTu, decimal? tongDen, string nguoiLap, string khoNhap)
        {
            DataTable tbl = factory.TimPhieuNhapNangCao(maPhieu, idNcc, ngayNhap, tongTu, tongDen, nguoiLap, khoNhap);
            bs.DataSource = tbl;
            return tbl != null ? tbl.Rows.Count : 0;
        }

        // ---------------------------------------------------------------
        // Dùng cho form Nhập hàng: tải danh sách, đứng ở một phiếu, sửa, xóa
        // (form tự điền các ô từ dòng hiện tại, không gắn DataBindings nên không
        //  vô tình ghi đè dữ liệu của phiếu đang xem)
        // ---------------------------------------------------------------

        /// <summary>Số phiếu đang có trong danh sách.</summary>
        public int SoPhieu
        {
            get { return bs.Count; }
        }

        /// <summary>Vị trí phiếu hiện tại trong danh sách (-1 nếu chưa có).</summary>
        public int ViTri
        {
            get { return bs.Position; }
        }

        /// <summary>Dòng phiếu hiện tại, null nếu danh sách rỗng.</summary>
        public DataRowView HienTai
        {
            get { return bs.Current as DataRowView; }
        }

        /// <summary>Mã phiếu hiện tại, rỗng nếu chưa có phiếu nào.</summary>
        public string IdHienTai
        {
            get
            {
                DataRowView v = bs.Current as DataRowView;
                return (v == null) ? "" : Convert.ToString(v["ID"]);
            }
        }

        public void DatViTri(int vitri)
        {
            if (vitri >= 0 && vitri < bs.Count) bs.Position = vitri;
        }

        /// <summary>Đứng ở phiếu có mã này; trả về false nếu không tìm thấy.</summary>
        public bool ChonPhieu(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            for (int i = 0; i < bs.Count; i++)
            {
                DataRowView v = bs[i] as DataRowView;
                if (v != null && Convert.ToString(v["ID"]) == id)
                {
                    bs.Position = i;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Tải toàn bộ phiếu nhập và gắn vào thanh điều hướng. Đứng ở phiếu idChon;
        /// không có (hoặc không tìm thấy) thì đứng ở phiếu mới nhất.
        /// </summary>
        public void TaiDanhSach(BindingNavigator bn, string idChon)
        {
            bs.DataSource = factory.DanhsachPhieuNhap();
            bn.BindingSource = bs;

            if (!ChonPhieu(idChon) && bs.Count > 0)
                bs.MoveLast();
        }

        private DataRow TimDong(string id)
        {
            DataTable tbl = bs.DataSource as DataTable;
            if (tbl == null) return null;
            foreach (DataRow r in tbl.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                if (Convert.ToString(r["ID"]) == id) return r;
            }
            return null;
        }

        private static void Dat(DataRow r, string cot, object giaTri)
        {
            object moi = giaTri;
            try
            {
                moi = Convert.ChangeType(giaTri, r.Table.Columns[cot].DataType);
            }
            catch (Exception)
            {
            }
            object cu = r[cot];
            if (cu != DBNull.Value && cu.Equals(moi)) return;
            r[cot] = moi;
        }

        /// <summary>Ghi nhà cung cấp, tổng tiền, đã trả, còn nợ của phiếu xuống CSDL.</summary>
        public void CapNhatPhieu(string id, string idNhaCungCap, decimal tongTien, decimal daTra, decimal conNo)
        {
            DataRow r = TimDong(id);
            if (r == null)
                throw new InvalidOperationException("Không tìm thấy phiếu nhập " + id + " để cập nhật.");

            Dat(r, "ID_NHA_CUNG_CAP", idNhaCungCap);
            Dat(r, "TONG_TIEN", tongTien);
            Dat(r, "DA_TRA", daTra);
            Dat(r, "CON_NO", conNo);

            if (r.RowState != DataRowState.Unchanged)
                factory.Save();
        }

        /// <summary>Xóa phiếu khỏi CSDL (các dòng chi tiết phải được xóa trước).</summary>
        public void XoaPhieu(string id)
        {
            DataRow r = TimDong(id);
            if (r == null)
                throw new InvalidOperationException("Không tìm thấy phiếu nhập " + id + " để xóa.");
            r.Delete();
            factory.Save();
        }

        // ---------------------------------------------------------------
        // Nhật ký thay đổi
        // ---------------------------------------------------------------
        public void GhiNhatKy(string hanhDong, string idPhieu, string tenNcc, string noiDung, string nguoiDung)
        {
            factory.GhiNhatKy(hanhDong, idPhieu, tenNcc, noiDung, nguoiDung);
        }

        public DataTable LayNhatKy(string idPhieu)
        {
            return factory.LayNhatKy(idPhieu);
        }

    
        public DataTable DanhsachPhieuNhapLoc(string idNcc, DateTime? tuNgay, DateTime? denNgay, int? locCongNo)
        {
            DataTable tbl = factory.DanhsachPhieuNhapLoc(idNcc, tuNgay, denNgay, locCongNo);
            bs.DataSource = tbl;
            return tbl;
        }

        public void HienthiPhieuNhapLoc(BindingNavigator bn, DataGridView dg,
            string idNcc, DateTime? tuNgay, DateTime? denNgay, int? locCongNo)
        {
            bs.DataSource = factory.DanhsachPhieuNhapLoc(idNcc, tuNgay, denNgay, locCongNo);
            if (bn != null) bn.BindingSource = bs;
            if (dg != null) dg.DataSource = bs;
        }

        public static void DamBaoCotMoRong()
        {
            PhieuNhapFactory.DamBaoCotMoRong();
        }
    }
}
