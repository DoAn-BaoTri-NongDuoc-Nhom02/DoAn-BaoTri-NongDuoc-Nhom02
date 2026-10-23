using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.Controller;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc
{
    public partial class frmDaiLy : Form
    {
        private const bool LA_DAI_LY = true;
        private const string TIEU_DE = "Quản lý Đại lý";
        private const string TEN_DT = "đại lý";              // dùng trong câu thông báo
        private const string COT_NHOM = "CAP_DAI_LY";          // CAP_DAI_LY (đại lý) hoặc NHOM_KH (khách hàng)
        private const long HAN_MUC_THAP = 20000000;
        private const long HAN_MUC_CAO = 50000000;
        private static readonly int[] CO_TRANG = new int[] { 20, 50, 100 };
        private static readonly Color MAU_NO = Color.FromArgb(255, 226, 222);   // có công nợ
        private static readonly Color MAU_VIP = Color.FromArgb(255, 245, 200);  // khách VIP

        private string m_CheDoTim = "ALL";    // ALL, HO_TEN, DIEN_THOAI, DIA_CHI, MST, EMAIL
        private bool m_DangGoiY = true;       // ô tìm đang hiện chữ gợi ý
        private bool m_DangDatChu = false;    // đang tự đặt chữ vào ô tìm, bỏ qua TextChanged
        private bool m_DangLamMoi = false;    // đang tính lại trang, tránh gọi lồng nhau
        private bool m_SanSang = false;       // đã nạp xong, các bộ lọc mới được phép phản ứng
        private bool m_CheDoSua = false;
        private string m_LocTim = "";         // biểu thức lọc đầy đủ (từ khóa + các bộ lọc)
        private int m_Trang = 1;
        private int m_SoTrang = 1;
        private int m_CoTrang = 50;
        private int m_SoKhop = 0;             // số dòng khớp bộ lọc (trước khi chia trang)
        private Dictionary<string, long> m_CongNo = new Dictionary<string, long>();
        private Dictionary<string, long> m_TongMua = new Dictionary<string, long>();
        private Font m_FontDam;

        KhachHangController ctrl = new KhachHangController();

        public frmDaiLy()
        {
            InitializeComponent();
        }

        // ---------------------------------------------------------------
        // Tải dữ liệu, trạng thái
        // ---------------------------------------------------------------
        private void frmDaiLy_Load(object sender, EventArgs e)
        {
            UiTheme.Apply(this);
            m_FontDam = new Font(dataGridView.Font, FontStyle.Bold);
            dataGridView.AutoGenerateColumns = false;   // chỉ dùng các cột khai báo trong Designer
            KhoiTaoBoLoc();

            try
            {
                TaiLaiDuLieu();
            }
            catch (Exception ex)
            {
                // Lỗi trong Load hay bị Windows nuốt mất khiến lưới trống, nên báo ra cho biết nguyên nhân
                MessageBox.Show("Không tải được dữ liệu " + TEN_DT + ":\n" + ex.Message,
                    TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            m_SanSang = true;
            HienGoiY();
        }

        private void KhoiTaoBoLoc()
        {
            cboLocNhom.Items.Clear();
            cboLocNhom.Items.Add("Tất cả nhóm");
            string[] nhom = LA_DAI_LY ? KhachHangController.NhanCapDaiLy : KhachHangController.NhanNhomKhach;
            for (int i = 0; i < nhom.Length; i++) cboLocNhom.Items.Add(nhom[i]);
            cboLocNhom.SelectedIndex = 0;

            cboLocTrangThai.Items.Clear();
            cboLocTrangThai.Items.Add("Tất cả trạng thái");
            cboLocTrangThai.Items.Add(KhachHangController.NhanDangHoatDong);
            cboLocTrangThai.Items.Add(KhachHangController.NhanNgungHopTac);
            cboLocTrangThai.SelectedIndex = 0;

            cboLocHanMuc.Items.Clear();
            cboLocHanMuc.Items.Add("Tất cả hạn mức");
            cboLocHanMuc.Items.Add("Chưa đặt hạn mức");
            cboLocHanMuc.Items.Add("Dưới 20 triệu");
            cboLocHanMuc.Items.Add("Từ 20 đến 50 triệu");
            cboLocHanMuc.Items.Add("Trên 50 triệu");
            cboLocHanMuc.SelectedIndex = 0;

            cboKichThuocTrang.Items.Clear();
            cboKichThuocTrang.Items.Add("20 dòng");
            cboKichThuocTrang.Items.Add("50 dòng");
            cboKichThuocTrang.Items.Add("100 dòng");
            cboKichThuocTrang.SelectedIndex = 1;    // 50 dòng / trang

            DatCheDoTimHienThi();
        }

        private void TaiLaiDuLieu()
        {
            DataTable tbl = ctrl.TaiBangDoiTac(LA_DAI_LY);
            GanNguonCombo(tbl);

            BindingSource bs = new BindingSource();
            bs.DataSource = tbl;
            if (tbl.Columns.Contains("ID")) bs.Sort = "ID";
            bs.ListChanged += new ListChangedEventHandler(bs_ListChanged);
            bindingNavigator.BindingSource = bs;
            dataGridView.DataSource = bs;

            // Công nợ / tổng mua chỉ để hiển thị và tô màu: nếu truy vấn thống kê lỗi thì vẫn phải hiện danh sách
            try
            {
                ctrl.LayThongKeKhachHang(out m_CongNo, out m_TongMua);
            }
            catch (Exception ex)
            {
                m_CongNo = new Dictionary<string, long>();
                m_TongMua = new Dictionary<string, long>();
                ThongBao("Không lấy được công nợ / tổng mua: " + ex.Message);
            }

            DatCheDoSua(false);
            LamMoiHienThi();
            BaoCotThieu(tbl);
        }

        // Cột CSDL còn thiếu (chưa chạy CapNhat_CSDL.sql) thì báo rõ thay vì để ô trống không rõ lý do
        private void BaoCotThieu(DataTable tbl)
        {
            string[] can = new string[] { "MST", "EMAIL", "NGUOI_LIEN_HE", "HAN_MUC_CONG_NO", "SO_NGAY_CONG_NO", "KY_HAN", "TRANG_THAI", COT_NHOM };
            StringBuilder sb = new StringBuilder();
            foreach (string c in can)
            {
                if (!tbl.Columns.Contains(c))
                {
                    if (sb.Length > 0) sb.Append(", ");
                    sb.Append(c);
                }
            }
            if (sb.Length > 0)
                ThongBao("Bảng KHACH_HANG còn thiếu cột: " + sb.ToString() + ". Hãy chạy CapNhat_CSDL.sql.");
        }

        private static List<KeyValuePair<string, object>> TaoDsLuaChon(DataTable tbl, string cot, string[] nhan, int batDau)
        {
            List<KeyValuePair<string, object>> ds = new List<KeyValuePair<string, object>>();
            Type kieu = typeof(int);
            if (tbl.Columns.Contains(cot)) kieu = tbl.Columns[cot].DataType;
            for (int i = 0; i < nhan.Length; i++)
            {
                object v = i + batDau;
                try { v = Convert.ChangeType(i + batDau, kieu); }
                catch (Exception) { }
                ds.Add(new KeyValuePair<string, object>(nhan[i], v));
            }
            return ds;
        }

        // Các cột chọn từ danh sách (nhóm, kỳ hạn, trạng thái); kiểu giá trị lấy theo kiểu cột trong CSDL
        private void GanNguonCombo(DataTable tbl)
        {
            colNhom.DataSource = TaoDsLuaChon(tbl, COT_NHOM, LA_DAI_LY ? KhachHangController.NhanCapDaiLy : KhachHangController.NhanNhomKhach, 1);
            colNhom.DisplayMember = "Key";
            colNhom.ValueMember = "Value";

            colKyHan.DataSource = TaoDsLuaChon(tbl, "KY_HAN", KhachHangController.NhanKyHan, 1);
            colKyHan.DisplayMember = "Key";
            colKyHan.ValueMember = "Value";

            List<KeyValuePair<string, object>> tt = new List<KeyValuePair<string, object>>();
            tt.Add(new KeyValuePair<string, object>(KhachHangController.NhanDangHoatDong, true));
            tt.Add(new KeyValuePair<string, object>(KhachHangController.NhanNgungHopTac, false));
            colTrangThai.DataSource = tt;
            colTrangThai.DisplayMember = "Key";
            colTrangThai.ValueMember = "Value";
        }

        private void bs_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (!m_DangLamMoi) CapNhatTrangThai();
        }

        private DataTable LayBang()
        {
            BindingSource bs = bindingNavigator.BindingSource;
            return (bs == null) ? null : bs.DataSource as DataTable;
        }

        private bool Co(string cot)
        {
            DataTable tbl = LayBang();
            return tbl != null && tbl.Columns.Contains(cot);
        }

        private void ThongBao(string noiDung)
        {
            lblThongBao.Text = noiDung;
        }

        private static string NguoiDung()
        {
            // Nếu dự án có sẵn thông tin người đăng nhập thì thay dòng này bằng tên đó
            return Environment.UserName;
        }

        private void DatCheDoSua(bool sua)
        {
            m_CheDoSua = sua;
            dataGridView.ReadOnly = !sua;
            colID.ReadOnly = true;          // mã số do hệ thống cấp
            colCongNo.ReadOnly = true;
            colTongMua.ReadOnly = true;
            lblCheDo.Text = sua ? "Chế độ SỬA" : "Chế độ xem";
            lblCheDo.ForeColor = sua ? Color.Firebrick : SystemColors.ControlText;
        }

        // ---------------------------------------------------------------
        // Phân trang: mỗi trang là tập mã (ID) của các dòng thuộc trang đó,
        // tính theo bộ lọc và thứ tự sắp xếp hiện tại
        // ---------------------------------------------------------------
        private void LamMoiHienThi()
        {
            BindingSource bs = bindingNavigator.BindingSource;
            DataTable tbl = LayBang();
            if (bs == null || tbl == null || m_DangLamMoi) return;

            m_DangLamMoi = true;
            try
            {
                DataView dv = new DataView(tbl);
                if (m_LocTim.Length > 0) dv.RowFilter = m_LocTim;
                if (!string.IsNullOrEmpty(bs.Sort)) dv.Sort = bs.Sort;

                m_SoKhop = dv.Count;
                m_SoTrang = (m_SoKhop + m_CoTrang - 1) / m_CoTrang;
                if (m_SoTrang < 1) m_SoTrang = 1;
                if (m_Trang > m_SoTrang) m_Trang = m_SoTrang;
                if (m_Trang < 1) m_Trang = 1;

                int dau = (m_Trang - 1) * m_CoTrang;
                int cuoi = Math.Min(m_SoKhop, dau + m_CoTrang);
                StringBuilder sb = new StringBuilder();
                for (int i = dau; i < cuoi; i++)
                {
                    if (sb.Length > 0) sb.Append(',');
                    sb.Append('\'').Append(Convert.ToString(dv[i]["ID"]).Replace("'", "''")).Append('\'');
                }
                // Convert sang chuỗi để lọc đúng dù cột ID trong CSDL là số hay chuỗi
                bs.Filter = (sb.Length == 0) ? "1 = 0" : "Convert(ID, 'System.String') IN (" + sb.ToString() + ")";
            }
            finally
            {
                m_DangLamMoi = false;
            }
            CapNhatTrangThai();
        }

        private void CapNhatTrangThai()
        {
            BindingSource bs = bindingNavigator.BindingSource;
            DataTable tbl = LayBang();
            if (bs == null || tbl == null) return;

            int tong = 0;
            foreach (DataRow r in tbl.Rows)
            {
                if (r.RowState != DataRowState.Deleted) tong++;
            }

            if (m_LocTim.Length == 0)
                lblTong.Text = "Tổng số: " + tong.ToString() + " " + TEN_DT;
            else
                lblTong.Text = "Tìm thấy " + m_SoKhop.ToString() + " / " + tong.ToString() + " " + TEN_DT;

            lblTrang.Text = "Trang " + m_Trang.ToString() + " / " + m_SoTrang.ToString();
            toolTrangTruoc.Enabled = m_Trang > 1;
            toolTrangSau.Enabled = m_Trang < m_SoTrang;

            if (tong == 0)
            {
                lblChuaCoDuLieu.Text = "Chưa có dữ liệu " + TEN_DT + ".\nBấm \"Thêm\" để tạo " + TEN_DT + " mới.";
                lblChuaCoDuLieu.Visible = true;
            }
            else if (bs.Count == 0)
            {
                lblChuaCoDuLieu.Text = "Không tìm thấy " + TEN_DT + " phù hợp.";
                lblChuaCoDuLieu.Visible = true;
            }
            else
            {
                lblChuaCoDuLieu.Visible = false;
            }
        }

        private void toolTrangTruoc_Click(object sender, EventArgs e)
        {
            if (m_Trang > 1) { m_Trang--; LamMoiHienThi(); }
        }

        private void toolTrangSau_Click(object sender, EventArgs e)
        {
            if (m_Trang < m_SoTrang) { m_Trang++; LamMoiHienThi(); }
        }

        private void cboKichThuocTrang_SelectedIndexChanged(object sender, EventArgs e)
        {
            int chiSo = cboKichThuocTrang.SelectedIndex;
            if (chiSo < 0 || chiSo >= CO_TRANG.Length) return;
            m_CoTrang = CO_TRANG[chiSo];
            m_Trang = 1;
            LamMoiHienThi();
        }

        // Bấm tiêu đề cột để sắp xếp: tính lại trang theo thứ tự mới, về trang 1
        private void dataGridView_Sorted(object sender, EventArgs e)
        {
            m_Trang = 1;
            LamMoiHienThi();
        }

        // ---------------------------------------------------------------
        // Màu / định dạng: công nợ, khách VIP, ngừng hợp tác
        // ---------------------------------------------------------------
        private void LayChiSoKhach(int dong, out long congNo, out long tongMua)
        {
            string id = Convert.ToString(dataGridView.Rows[dong].Cells[colID.Index].Value);
            if (!m_CongNo.TryGetValue(id, out congNo)) congNo = 0;
            if (!m_TongMua.TryGetValue(id, out tongMua)) tongMua = 0;
        }

        private decimal LayHanMuc(int dong)
        {
            object v = dataGridView.Rows[dong].Cells[colHanMuc.Index].Value;
            if (v == null || v == DBNull.Value) return 0;
            try { return Convert.ToDecimal(v); }
            catch (Exception) { return 0; }
        }

        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            long congNo, tongMua;
            LayChiSoKhach(e.RowIndex, out congNo, out tongMua);

            if (e.ColumnIndex == colCongNo.Index) e.Value = congNo;
            else if (e.ColumnIndex == colTongMua.Index) e.Value = tongMua;

            bool vip = tongMua >= KhachHangController.NguongVip;
            if (congNo > 0)
            {
                e.CellStyle.BackColor = MAU_NO;
                if (vip) e.CellStyle.Font = m_FontDam;
            }
            else if (vip)
            {
                e.CellStyle.BackColor = MAU_VIP;
                e.CellStyle.Font = m_FontDam;
            }

            // Công nợ vượt hạn mức thì chữ đỏ đậm
            if (e.ColumnIndex == colCongNo.Index)
            {
                decimal hanMuc = LayHanMuc(e.RowIndex);
                if (hanMuc > 0 && congNo > hanMuc)
                {
                    e.CellStyle.ForeColor = Color.Firebrick;
                    e.CellStyle.Font = m_FontDam;
                }
            }

            // Ngừng hợp tác thì chữ xám
            object tt = dataGridView.Rows[e.RowIndex].Cells[colTrangThai.Index].Value;
            if (tt is bool && !(bool)tt && e.ColumnIndex != colCongNo.Index)
                e.CellStyle.ForeColor = Color.Gray;
        }

        private void dataGridView_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            long congNo, tongMua;
            LayChiSoKhach(e.RowIndex, out congNo, out tongMua);
            StringBuilder sb = new StringBuilder();
            if (congNo > 0) sb.Append("Đang nợ: ").Append(congNo.ToString("N0")).Append(" đ");
            decimal hanMuc = LayHanMuc(e.RowIndex);
            if (hanMuc > 0 && congNo > hanMuc)
            {
                if (sb.Length > 0) sb.Append("\n");
                sb.Append("Vượt hạn mức công nợ (").Append(hanMuc.ToString("N0")).Append(" đ)");
            }
            if (tongMua >= KhachHangController.NguongVip)
            {
                if (sb.Length > 0) sb.Append("\n");
                sb.Append("Khách VIP (tổng mua ").Append(tongMua.ToString("N0")).Append(" đ)");
            }
            if (sb.Length > 0) e.ToolTipText = sb.ToString();
        }

        // Giá trị không hiển thị được trong ô (ví dụ gõ chữ vào ô số): báo nhẹ, không để hộp thoại lỗi mặc định bật lên
        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            string cot = "";
            if (e.ColumnIndex >= 0 && e.ColumnIndex < dataGridView.Columns.Count)
                cot = dataGridView.Columns[e.ColumnIndex].HeaderText;
            ThongBao("Giá trị ở cột \"" + cot + "\" không hợp lệ, vui lòng nhập lại.");
        }

        // ---------------------------------------------------------------
        // Thêm / Sửa
        // ---------------------------------------------------------------
        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            DataTable tbl = LayBang();
            if (tbl == null) return;

            // Dòng vừa thêm / vừa sửa phải đủ thông tin thì mới thêm dòng mới
            dataGridView.EndEdit();
            bindingNavigator.BindingSource.EndEdit();
            DataRow dongLoi;
            string cotLoi;
            string thongBaoLoi;
            if (!ctrl.KiemTraHopLe(tbl, out dongLoi, out cotLoi, out thongBaoLoi))
            {
                MessageBox.Show("Hãy nhập đủ thông tin cho dòng đang nhập trước khi thêm dòng mới:\n" + thongBaoLoi,
                    TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                HienDong(dongLoi, cotLoi);
                return;
            }

            XoaLoc();
            bindingNavigator.BindingSource.Sort = null;     // dòng mới nằm cuối danh sách

            long maso = ThamSo.KhachHang;
            ThamSo.KhachHang = maso + 1;

            DataRow dong = tbl.NewRow();
            dong["ID"] = maso.ToString();
            if (tbl.Columns.Contains("LOAI_KH")) dong["LOAI_KH"] = LA_DAI_LY;
            tbl.Rows.Add(dong);

            DatCheDoSua(true);
            HienDong(dong, "HO_TEN");
            ThongBao("Đã thêm " + TEN_DT + " mới (mã số " + maso.ToString() + "). Nhập họ tên, số điện thoại rồi bấm Lưu.");
        }

        private void toolSua_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView.CurrentRow;
            if (row == null || row.IsNewRow)
            {
                MessageBox.Show("Chưa chọn " + TEN_DT + " nào để sửa.\nHãy chọn một dòng hoặc bấm Thêm để tạo mới.",
                    TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DatCheDoSua(true);

            int cot = colHoTen.Index;
            if (dataGridView.CurrentCell != null && !dataGridView.Columns[dataGridView.CurrentCell.ColumnIndex].ReadOnly)
                cot = dataGridView.CurrentCell.ColumnIndex;
            dataGridView.Focus();
            dataGridView.CurrentCell = row.Cells[cot];
            dataGridView.BeginEdit(true);
            ThongBao("Đang sửa. Nhập xong bấm Enter, rồi bấm Lưu để cập nhật.");
        }

        // Báo lỗi ngay trên ô khi vừa nhập xong (việc chặn thật nằm ở nút Thêm / Lưu)
        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            string cot = dataGridView.Columns[e.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(cot)) return;

            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            object v = cell.Value;
            string giaTri = (v == null || v == DBNull.Value) ? "" : Convert.ToString(v);
            cell.ErrorText = KhachHangController.KiemTraTruong(cot, giaTri);
        }

        private int ChiSoCotTheoDuLieu(string cot)
        {
            foreach (DataGridViewColumn c in dataGridView.Columns)
            {
                if (c.DataPropertyName == cot) return c.Index;
            }
            return colHoTen.Index;
        }

        // Chọn đúng dòng (kể cả khi đang ở trang khác) và đưa con trỏ vào ô cần sửa
        private void HienDong(DataRow dong, string cot)
        {
            BindingSource bs = bindingNavigator.BindingSource;
            DataTable tbl = LayBang();
            if (bs == null || tbl == null || dong == null) return;

            string id = Convert.ToString(dong["ID"]);
            XoaLoc();

            DataView dv = new DataView(tbl);
            if (!string.IsNullOrEmpty(bs.Sort)) dv.Sort = bs.Sort;
            int chiSo = -1;
            for (int i = 0; i < dv.Count; i++)
            {
                if (Convert.ToString(dv[i]["ID"]) == id) { chiSo = i; break; }
            }
            if (chiSo < 0) return;

            m_Trang = chiSo / m_CoTrang + 1;
            LamMoiHienThi();

            int viTri = TimViTri(bs, dong);
            if (viTri < 0) return;

            int chiSoCot = ChiSoCotTheoDuLieu(cot);

            bs.Position = viTri;
            dataGridView.Focus();
            dataGridView.CurrentCell = dataGridView.Rows[viTri].Cells[chiSoCot];
            if (m_CheDoSua) dataGridView.BeginEdit(true);
        }

        private static int TimViTri(BindingSource bs, DataRow dong)
        {
            for (int i = 0; i < bs.Count; i++)
            {
                DataRowView v = bs[i] as DataRowView;
                if (v != null && v.Row == dong) return i;
            }
            return -1;
        }

        // ---------------------------------------------------------------
        // Xóa: chặn đã có giao dịch (và báo rõ lý do), còn lại luôn hỏi xác nhận trước khi xóa
        // ---------------------------------------------------------------
        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            string ten = Convert.ToString(e.Row.Cells[colHoTen.Index].Value);
            if (!XacNhanXoa(Convert.ToString(e.Row.Cells[colID.Index].Value), ten))
                e.Cancel = true;
            else
                ThongBao("Đã xóa \"" + ten + "\" (chưa lưu, bấm Lưu để cập nhật cơ sở dữ liệu).");
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            if (bindingNavigator.BindingSource == null) return;
            DataRowView dong = bindingNavigator.BindingSource.Current as DataRowView;
            if (dong == null)
            {
                MessageBox.Show("Chưa chọn " + TEN_DT + " nào để xóa.", TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string ten = Convert.ToString(dong["HO_TEN"]);
            if (XacNhanXoa(Convert.ToString(dong["ID"]), ten))
            {
                bindingNavigator.BindingSource.RemoveCurrent();
                ThongBao("Đã xóa \"" + ten + "\" (chưa lưu, bấm Lưu để cập nhật cơ sở dữ liệu).");
            }
        }

        private bool XacNhanXoa(string id, string ten)
        {
            string hienTen = (ten.Trim().Length == 0) ? "(chưa nhập họ tên)" : ten;

            if (id.Length > 0)
            {
                int soBan, soThanhToan;
                bool daCoGiaoDich;
                try
                {
                    daCoGiaoDich = ctrl.KhachHangDaCoGiaoDich(id, out soBan, out soThanhToan);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không kiểm tra được giao dịch của " + TEN_DT + " này nên chưa thể xóa:\n" + ex.Message,
                        TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (daCoGiaoDich)
                {
                    MessageBox.Show("Không thể xóa " + TEN_DT + " \"" + hienTen + "\" vì đã có giao dịch:\n" +
                        "  - Phiếu bán: " + soBan.ToString() + "\n" +
                        "  - Phiếu thanh toán: " + soThanhToan.ToString() + "\n" +
                        "Bạn chỉ có thể sửa lại thông tin, hoặc chuyển trạng thái sang \"Ngừng hợp tác\".",
                        TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return MessageBox.Show("Bạn có chắc chắn xóa " + TEN_DT + " \"" + hienTen + "\" không?\n" +
                "Thao tác chỉ có hiệu lực sau khi bấm Lưu.", TIEU_DE,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        // ---------------------------------------------------------------
        // Lưu: kiểm tra, hỏi xác nhận, lưu, ghi nhật ký, báo kết quả
        // ---------------------------------------------------------------
        private void toolLuu_Click(object sender, EventArgs e)
        {
            LuuDuLieu(true);
        }

        private bool LuuDuLieu(bool hoiXacNhan)
        {
            if (bindingNavigator.BindingSource == null) return true;
            dataGridView.EndEdit();
            bindingNavigator.BindingSource.EndEdit();

            DataTable tbl = LayBang();
            if (tbl == null) return true;

            if (tbl.GetChanges() == null)
            {
                if (hoiXacNhan)
                {
                    MessageBox.Show("Không có thay đổi nào để lưu.", TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DatCheDoSua(false);
                }
                return true;
            }

            DataRow dongLoi;
            string cotLoi;
            string thongBaoLoi;
            if (!ctrl.KiemTraHopLe(tbl, out dongLoi, out cotLoi, out thongBaoLoi))
            {
                MessageBox.Show(thongBaoLoi, TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                HienDong(dongLoi, cotLoi);
                return false;
            }

            int them = 0, sua = 0, xoa = 0;
            foreach (DataRow r in tbl.Rows)
            {
                if (r.RowState == DataRowState.Added) them++;
                else if (r.RowState == DataRowState.Modified) sua++;
                else if (r.RowState == DataRowState.Deleted) xoa++;
            }

            if (hoiXacNhan)
            {
                string noiDung = "Bạn có chắc chắn muốn lưu các thay đổi sau?\n" +
                    "  - Thêm mới: " + them.ToString() + "\n" +
                    "  - Sửa: " + sua.ToString() + "\n" +
                    "  - Xóa: " + xoa.ToString();
                if (MessageBox.Show(noiDung, TIEU_DE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return false;
            }

            // Nhật ký phải lập trước khi lưu, vì sau khi lưu bảng không còn biết dòng nào đã đổi
            IList<NhatKyKhachHang> nhatKy = ctrl.TaoNhatKy(tbl);

            bool daLuu;
            try
            {
                daLuu = ctrl.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lưu không thành công:\n" + ex.Message + "\n\n" +
                    "Dữ liệu vẫn được giữ trên màn hình, vui lòng kiểm tra rồi thử lại.",
                    TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (daLuu)
            {
                string canhBao = "";
                try
                {
                    ctrl.GhiNhatKy(nhatKy, NguoiDung());
                }
                catch (Exception ex)
                {
                    // Dữ liệu đã lưu rồi, chỉ riêng việc ghi nhật ký lỗi nên chỉ cảnh báo
                    canhBao = "\n\nLưu ý: chưa ghi được nhật ký thay đổi (" + ex.Message + ").";
                }

                TaiLaiDuLieu();
                ThongBao("Đã lưu: thêm " + them.ToString() + ", sửa " + sua.ToString() + ", xóa " + xoa.ToString() + ".");
                MessageBox.Show("Đã lưu thành công." + canhBao, TIEU_DE, MessageBoxButtons.OK,
                    canhBao.Length == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                return true;
            }

            MessageBox.Show("Lưu không thành công (có thể " + TEN_DT + " bị xóa đã có giao dịch, hoặc lỗi cơ sở dữ liệu).\n" +
                "Dữ liệu vẫn được giữ trên màn hình, vui lòng kiểm tra rồi thử lại.",
                TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        private void toolNhatKy_Click(object sender, EventArgs e)
        {
            string id = "";
            string ten = "";
            DataGridViewRow row = dataGridView.CurrentRow;
            if (row != null && !row.IsNewRow)
            {
                id = Convert.ToString(row.Cells[colID.Index].Value);
                ten = Convert.ToString(row.Cells[colHoTen.Index].Value);
            }
            using (frmNhatKy f = new frmNhatKy(id, ten))
            {
                f.ShowDialog(this);
            }
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmDaiLy_FormClosing(object sender, FormClosingEventArgs e)
        {
            DataTable tbl = LayBang();
            if (tbl == null) return;

            dataGridView.EndEdit();
            bindingNavigator.BindingSource.EndEdit();
            if (tbl.GetChanges() == null) return;

            DialogResult kq = MessageBox.Show("Còn thay đổi chưa lưu. Bạn có muốn lưu trước khi đóng không?",
                TIEU_DE, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (kq == DialogResult.Cancel)
                e.Cancel = true;
            else if (kq == DialogResult.Yes)
            {
                if (!LuuDuLieu(false)) e.Cancel = true;
            }
        }

        // ---------------------------------------------------------------
        // Tìm kiếm: lọc ngay khi gõ, theo họ tên, điện thoại, địa chỉ, MST, email hoặc tất cả cột.
        // Kết hợp (AND) với các bộ lọc địa chỉ, nhóm, trạng thái, hạn mức.
        // ---------------------------------------------------------------
        private string TenCheDo()
        {
            switch (m_CheDoTim)
            {
                case "HO_TEN": return "Họ tên";
                case "DIEN_THOAI": return "Số điện thoại";
                case "DIA_CHI": return "Địa chỉ";
                case "MST": return "Mã số thuế";
                case "EMAIL": return "Email";
                default: return "tất cả các cột";
            }
        }

        private void HienGoiY()
        {
            m_DangDatChu = true;
            toolTimKhachHang.Text = "Tìm theo " + TenCheDo() + "...";
            toolTimKhachHang.ForeColor = Color.Gray;
            m_DangGoiY = true;
            m_DangDatChu = false;
        }

        private void toolTimKhachHang_Enter(object sender, EventArgs e)
        {
            if (m_DangGoiY)
            {
                m_DangDatChu = true;
                toolTimKhachHang.Text = "";
                toolTimKhachHang.ForeColor = Color.Black;
                m_DangGoiY = false;
                m_DangDatChu = false;
            }
        }

        private void toolTimKhachHang_Leave(object sender, EventArgs e)
        {
            if (toolTimKhachHang.Text.Trim().Length == 0) HienGoiY();
        }

        private void toolTimKhachHang_TextChanged(object sender, EventArgs e)
        {
            if (m_DangDatChu || !m_SanSang) return;
            ApDungLoc();
        }

        private void toolTimKhachHang_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                XoaLoc();
                e.SuppressKeyPress = true;
            }
        }

        private void BoLoc_Changed(object sender, EventArgs e)
        {
            if (!m_SanSang) return;
            ApDungLoc();
        }

        private void toolXoaLoc_Click(object sender, EventArgs e)
        {
            XoaLoc();
        }

        // Xóa từ khóa và đưa mọi bộ lọc về "tất cả"
        private void XoaLoc()
        {
            bool truoc = m_SanSang;
            m_SanSang = false;      // đặt lại các ô lọc mà không kích hoạt lọc nhiều lần
            m_DangDatChu = true;
            toolTimKhachHang.Text = "";
            m_DangDatChu = false;
            txtLocDiaChi.Text = "";
            cboLocNhom.SelectedIndex = 0;
            cboLocTrangThai.SelectedIndex = 0;
            cboLocHanMuc.SelectedIndex = 0;
            m_SanSang = truoc;

            m_LocTim = "";
            m_Trang = 1;
            LamMoiHienThi();
            if (!toolTimKhachHang.Focused) HienGoiY();
        }

        private void DatCheDoTimHienThi()
        {
            toolTimTatCa.Checked = (m_CheDoTim == "ALL");
            toolTimHoTen.Checked = (m_CheDoTim == "HO_TEN");
            toolTimDienThoai.Checked = (m_CheDoTim == "DIEN_THOAI");
            toolTimDiaChi.Checked = (m_CheDoTim == "DIA_CHI");
            toolTimMST.Checked = (m_CheDoTim == "MST");
            toolTimEmail.Checked = (m_CheDoTim == "EMAIL");
        }

        private void DatCheDoTim(string cheDo)
        {
            m_CheDoTim = cheDo;
            DatCheDoTimHienThi();

            if (m_DangGoiY) HienGoiY();
            ApDungLoc();
            dataGridView.Focus();
        }

        // Bấm vào mục nào thì chọn đúng mục đó (không đảo qua lại giữa hai mục)
        private void toolTimTatCa_Click(object sender, EventArgs e) { DatCheDoTim("ALL"); }
        private void toolTimHoTen_Click(object sender, EventArgs e) { DatCheDoTim("HO_TEN"); }
        private void toolTimDienThoai_Click(object sender, EventArgs e) { DatCheDoTim("DIEN_THOAI"); }
        private void toolTimDiaChi_Click(object sender, EventArgs e) { DatCheDoTim("DIA_CHI"); }
        private void toolTimMST_Click(object sender, EventArgs e) { DatCheDoTim("MST"); }
        private void toolTimEmail_Click(object sender, EventArgs e) { DatCheDoTim("EMAIL"); }

        private void ApDungLoc()
        {
            List<string> dk = new List<string>();

            string tuKhoa = m_DangGoiY ? "" : toolTimKhachHang.Text.Trim();
            if (tuKhoa.Length > 0) dk.Add(TaoBieuThucTim(tuKhoa));

            string diaChi = txtLocDiaChi.Text.Trim();
            if (diaChi.Length > 0 && Co("DIA_CHI"))
                dk.Add("DIA_CHI LIKE '%" + ThoatLike(diaChi) + "%'");

            if (cboLocNhom.SelectedIndex > 0 && Co(COT_NHOM))
                dk.Add(COT_NHOM + " = " + cboLocNhom.SelectedIndex.ToString());

            if (Co("TRANG_THAI"))
            {
                if (cboLocTrangThai.SelectedIndex == 1) dk.Add("TRANG_THAI = true");
                else if (cboLocTrangThai.SelectedIndex == 2) dk.Add("TRANG_THAI = false");
            }

            if (Co("HAN_MUC_CONG_NO"))
            {
                string hm = "IsNull(HAN_MUC_CONG_NO, 0)";
                switch (cboLocHanMuc.SelectedIndex)
                {
                    case 1: dk.Add(hm + " = 0"); break;
                    case 2: dk.Add(hm + " > 0 AND " + hm + " < " + HAN_MUC_THAP.ToString()); break;
                    case 3: dk.Add(hm + " >= " + HAN_MUC_THAP.ToString() + " AND " + hm + " <= " + HAN_MUC_CAO.ToString()); break;
                    case 4: dk.Add(hm + " > " + HAN_MUC_CAO.ToString()); break;
                }
            }

            m_LocTim = (dk.Count == 0) ? "" : "(" + string.Join(") AND (", dk.ToArray()) + ")";
            m_Trang = 1;
            LamMoiHienThi();
        }

        private static string ThoatLike(string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                if (c == '[' || c == ']' || c == '*' || c == '%')
                    sb.Append('[').Append(c).Append(']');
                else if (c == '\'')
                    sb.Append("''");
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private string DieuKienDienThoai(string tuKhoa)
        {
            // Số điện thoại có thể lưu có hoặc không có dấu cách: thử cả từ khóa gốc lẫn dạng chỉ còn chữ số
            StringBuilder so = new StringBuilder();
            foreach (char c in tuKhoa)
            {
                if (char.IsDigit(c) || c == '+') so.Append(c);
            }
            string dieuKien = "DIEN_THOAI LIKE '%" + ThoatLike(tuKhoa) + "%'";
            if (so.Length > 0 && so.ToString() != tuKhoa)
                dieuKien += " OR DIEN_THOAI LIKE '%" + ThoatLike(so.ToString()) + "%'";
            return dieuKien;
        }

        private string TaoBieuThucTim(string tuKhoa)
        {
            string like = " LIKE '%" + ThoatLike(tuKhoa) + "%'";
            switch (m_CheDoTim)
            {
                case "HO_TEN": return "HO_TEN" + like;
                case "DIA_CHI": return "DIA_CHI" + like;
                case "MST": return Co("MST") ? "MST" + like : "1 = 0";
                case "EMAIL": return Co("EMAIL") ? "EMAIL" + like : "1 = 0";
                case "DIEN_THOAI": return DieuKienDienThoai(tuKhoa);
                default:
                    StringBuilder sb = new StringBuilder();
                    sb.Append("HO_TEN").Append(like);
                    sb.Append(" OR DIA_CHI").Append(like);
                    sb.Append(" OR ").Append(DieuKienDienThoai(tuKhoa));
                    sb.Append(" OR Convert(ID, 'System.String')").Append(like);
                    if (Co("MST")) sb.Append(" OR MST").Append(like);
                    if (Co("EMAIL")) sb.Append(" OR EMAIL").Append(like);
                    if (Co("NGUOI_LIEN_HE")) sb.Append(" OR NGUOI_LIEN_HE").Append(like);
                    return sb.ToString();
            }
        }
    }
}