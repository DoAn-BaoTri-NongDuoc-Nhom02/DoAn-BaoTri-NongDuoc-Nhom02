using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    public partial class frmNhaCungCap : Form
    {
        private const string TieuDe = "Nhà cung cấp";
        private const string GoiYTimTen = "Tìm theo Nhà cung cấp";
        private const string GoiYTimDiaChi = "Tìm theo Địa chỉ";

        NhaCungCapController ctrl = new NhaCungCapController();

        // Normal: chỉ xem (lưới chỉ đọc). AddNew / Edit: lưới cho phép nhập liệu cho tới khi Lưu hoặc Hủy.
        Controll cheDo = Controll.Normal;
        int tongTrongCSDL = 0;      // tổng số NCC trong CSDL tại lần tải / lưu gần nhất
        bool dangTai = false;       // true khi đang nạp lại dữ liệu (tránh cập nhật trạng thái liên tục)
        Label lblTrong;             // thông báo khi không có dòng nào

        public frmNhaCungCap()
        {
            InitializeComponent();
            KhoiTaoThongBaoTrong();
        }

        private void KhoiTaoThongBaoTrong()
        {
            lblTrong = new Label();
            lblTrong.Dock = DockStyle.Fill;
            lblTrong.TextAlign = ContentAlignment.MiddleCenter;
            lblTrong.BackColor = Color.Transparent;
            lblTrong.ForeColor = SystemColors.GrayText;
            lblTrong.Font = new Font(this.Font.FontFamily, 12F, FontStyle.Italic);
            lblTrong.Visible = false;
            dataGridView.Controls.Add(lblTrong);
        }

        private BindingSource Nguon
        {
            get { return bindingNavigator.BindingSource; }
        }

        // ------------------------------------------------------------------
        // Tải form
        // ------------------------------------------------------------------

        private void frmNhaCungCap_Load(object sender, EventArgs e)
        {
            dangTai = true;
            try
            {
                ctrl.HienthiDataGridview(dataGridView, bindingNavigator);
            }
            finally
            {
                dangTai = false;
            }
            Nguon.ListChanged += new ListChangedEventHandler(bindingSource_ListChanged);

            tongTrongCSDL = ctrl.DemTong();
            DatGoiYTimKiem();
            DatCheDo(Controll.Normal);
        }

        private void bindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            CapNhatTrangThai();
        }

        // ------------------------------------------------------------------
        // Trạng thái: chế độ, nút, tổng số, thông báo "Chưa có dữ liệu"
        // ------------------------------------------------------------------

        private void DatCheDo(Controll c)
        {
            cheDo = c;
            dataGridView.ReadOnly = (c == Controll.Normal);
            colID.ReadOnly = true;      // Mã số do hệ thống cấp, không cho sửa
            CapNhatTrangThai();
        }

        private void CapNhatTrangThai()
        {
            if (dangTai) return;
            BindingSource bs = Nguon;
            if (bs == null) return;

            int hienThi = bs.Count;
            int them, sua, xoa;
            ctrl.DemThayDoi(out them, out sua, out xoa);
            int chuaLuu = them + sua + xoa;

            int tong = tongTrongCSDL + them - xoa;
            if (tong < 0) tong = 0;

            StringBuilder sb = new StringBuilder();
            sb.Append("Tổng số nhà cung cấp: ").Append(tong);
            if (hienThi != tong)
                sb.Append("   |   Đang hiển thị: ").Append(hienThi);
            if (chuaLuu > 0)
                sb.Append("   |   Chưa lưu: ").Append(chuaLuu).Append(" thay đổi");
            lblTongSo.Text = sb.ToString();

            lblTrong.Text = (tong == 0) ? "Chưa có dữ liệu" : "Không tìm thấy nhà cung cấp phù hợp";
            lblTrong.Visible = (hienThi == 0);

            toolSua.Enabled = hienThi > 0;
            bindingNavigatorDeleteItem.Enabled = hienThi > 0;
            toolLuu.Enabled = (cheDo != Controll.Normal) || chuaLuu > 0;
            toolHuy.Enabled = toolLuu.Enabled;
        }

        private int DemChuaLuu()
        {
            int them, sua, xoa;
            ctrl.DemThayDoi(out them, out sua, out xoa);
            return them + sua + xoa;
        }

        // Chốt các ô / dòng đang nhập dở để đếm thay đổi chính xác.
        private void ChotNhap()
        {
            dataGridView.EndEdit();
            if (Nguon != null) Nguon.EndEdit();
        }

        // ------------------------------------------------------------------
        // Thêm / Sửa / Xóa
        // ------------------------------------------------------------------

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            BindingSource bs = Nguon;
            if (bs == null) return;

            BoLoc();    // dòng mới còn trống nên sẽ bị bộ lọc ẩn đi nếu không bỏ lọc

            long maso = ThamSo.NhaCungCap;
            ThamSo.NhaCungCap = maso + 1;

            if (cheDo == Controll.Normal) DatCheDo(Controll.AddNew);

            DataRowView row = (DataRowView)bs.AddNew();
            row["ID"] = maso;
            bs.EndEdit();

            BatDauNhap(colHoTen);
        }

        private void toolSua_Click(object sender, EventArgs e)
        {
            BindingSource bs = Nguon;
            if (bs == null || bs.Position < 0)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp cần sửa.", TieuDe, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (cheDo == Controll.Normal) DatCheDo(Controll.Edit);
            BatDauNhap(colHoTen);
        }

        // Đưa con trỏ vào ô của dòng hiện tại và bắt đầu nhập.
        private void BatDauNhap(DataGridViewColumn cot)
        {
            BindingSource bs = Nguon;
            if (bs == null || bs.Position < 0 || bs.Position >= dataGridView.Rows.Count) return;
            dataGridView.CurrentCell = dataGridView.Rows[bs.Position].Cells[cot.Index];
            dataGridView.Focus();
            dataGridView.BeginEdit(true);
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            BindingSource bs = Nguon;
            DataRowView drv = (bs == null) ? null : bs.Current as DataRowView;
            if (drv == null)
            {
                MessageBox.Show("Không có nhà cung cấp nào để xóa.", TieuDe, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string ten = Convert.ToString(drv["HO_TEN"]);
            string ma = Convert.ToString(drv["ID"]);
            bool chuaLuuVaoCSDL = drv.IsNew || drv.Row.RowState == DataRowState.Added;

            string hoi = "Bạn có chắc chắn xóa nhà cung cấp "
                + (ten.Length > 0 ? "\"" + ten + "\"" : "mã số " + ma) + " không?";
            if (!chuaLuuVaoCSDL)
                hoi += "\r\n(Việc xóa chỉ được ghi vào cơ sở dữ liệu khi bạn bấm Lưu.)";

            if (MessageBox.Show(hoi, TieuDe, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                dataGridView.CancelEdit();
                bs.RemoveCurrent();
            }
        }

        // ------------------------------------------------------------------
        // Lưu / Hủy / Thoát
        // ------------------------------------------------------------------

        private void toolLuu_Click(object sender, EventArgs e)
        {
            LuuThayDoi(true);
        }

        /// <summary>
        /// Xác thực, hỏi xác nhận (nếu hoiXacNhan), rồi lưu. Trả về true nếu không còn gì chờ lưu.
        /// </summary>
        private bool LuuThayDoi(bool hoiXacNhan)
        {
            ChotNhap();

            int them, sua, xoa;
            ctrl.DemThayDoi(out them, out sua, out xoa);
            if (them + sua + xoa == 0)
            {
                if (hoiXacNhan)
                    MessageBox.Show("Không có thay đổi nào để lưu.", TieuDe, MessageBoxButtons.OK, MessageBoxIcon.Information);
                DatCheDo(Controll.Normal);
                return true;
            }

            string idLoi, cotLoi;
            string loi = ctrl.KiemTraTruocKhiLuu(out idLoi, out cotLoi);
            if (loi != null)
            {
                HienThiLoi(idLoi, cotLoi, loi);
                return false;
            }

            string tomTat = "Thêm mới: " + them + "\r\nCập nhật: " + sua + "\r\nXóa: " + xoa;
            if (hoiXacNhan)
            {
                if (MessageBox.Show("Bạn có chắc chắn lưu các thay đổi sau?\r\n\r\n" + tomTat,
                    TieuDe, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return false;
            }

            if (!ctrl.Save())
            {
                MessageBox.Show("Không thể lưu dữ liệu.\r\nCó thể nhà cung cấp đang được dùng trong phiếu nhập (không xóa được) "
                    + "hoặc mất kết nối cơ sở dữ liệu. Các thay đổi vẫn được giữ lại trên màn hình.",
                    TieuDe, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            tongTrongCSDL = ctrl.DemTong();
            XoaLoiHienThi();
            DatCheDo(Controll.Normal);

            if (hoiXacNhan)
                MessageBox.Show("Đã lưu thành công.\r\n\r\n" + tomTat, TieuDe, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        private void HienThiLoi(string id, string cot, string loi)
        {
            DataGridViewColumn c = colHoTen;
            if (cot == NhaCungCapValidator.CotDiaChi) c = colDiaChi;
            else if (cot == NhaCungCapValidator.CotDienThoai) c = colDienThoai;

            DataGridViewRow dong = TimDong(id);
            if (dong == null)
            {
                BoLoc();                // dòng lỗi có thể đang bị bộ lọc ẩn
                dong = TimDong(id);
            }
            if (dong != null)
            {
                DataGridViewCell cell = dong.Cells[c.Index];
                cell.ErrorText = loi;
                dataGridView.CurrentCell = cell;
            }

            MessageBox.Show(loi, TieuDe, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (!dataGridView.ReadOnly && dataGridView.CurrentCell != null)
                dataGridView.BeginEdit(true);
        }

        private DataGridViewRow TimDong(string id)
        {
            foreach (DataGridViewRow r in dataGridView.Rows)
            {
                if (Convert.ToString(r.Cells[colID.Index].Value) == id) return r;
            }
            return null;
        }

        private void XoaLoiHienThi()
        {
            foreach (DataGridViewRow r in dataGridView.Rows)
            {
                r.ErrorText = "";
                foreach (DataGridViewCell c in r.Cells) c.ErrorText = "";
            }
        }

        private void toolHuy_Click(object sender, EventArgs e)
        {
            dataGridView.CancelEdit();
            if (DemChuaLuu() > 0)
            {
                if (MessageBox.Show("Hủy toàn bộ thay đổi chưa lưu?", TieuDe,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
            }
            ctrl.HuyThayDoi();
            XoaLoiHienThi();
            DatCheDo(Controll.Normal);
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            this.Close();     // việc hỏi lưu nằm ở FormClosing để cũng áp dụng khi bấm nút X
        }

        private void frmNhaCungCap_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Nguon == null) return;
            ChotNhap();
            if (DemChuaLuu() == 0) return;

            DialogResult kq = MessageBox.Show(
                "Còn thay đổi chưa lưu. Bạn có muốn lưu trước khi thoát không?\r\n\r\n"
                + "Có: lưu rồi thoát\r\nKhông: bỏ thay đổi và thoát\r\nHủy: ở lại chỉnh sửa tiếp",
                TieuDe, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (kq == DialogResult.Cancel)
                e.Cancel = true;
            else if (kq == DialogResult.Yes && !LuuThayDoi(false))
                e.Cancel = true;
        }

        // ------------------------------------------------------------------
        // Xác thực khi nhập (báo ngay trên ô; việc chặn cứng nằm ở nút Lưu)
        // ------------------------------------------------------------------

        private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string gt = Convert.ToString(e.FormattedValue);
            string loi = null;

            if (e.ColumnIndex == colHoTen.Index) loi = NhaCungCapValidator.KiemTraHoTen(gt);
            else if (e.ColumnIndex == colDiaChi.Index) loi = NhaCungCapValidator.KiemTraDiaChi(gt);
            else if (e.ColumnIndex == colDienThoai.Index) loi = NhaCungCapValidator.KiemTraDienThoai(gt);
            else return;

            dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = (loi == null) ? "" : loi;
        }

        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox tb = e.Control as TextBox;
            if (tb == null || dataGridView.CurrentCell == null) return;

            tb.KeyPress -= new KeyPressEventHandler(DienThoai_KeyPress);

            int c = dataGridView.CurrentCell.ColumnIndex;
            if (c == colHoTen.Index)
            {
                tb.MaxLength = NhaCungCapValidator.DoDaiToiDaHoTen;
            }
            else if (c == colDiaChi.Index)
            {
                tb.MaxLength = NhaCungCapValidator.DoDaiToiDaDiaChi;
            }
            else if (c == colDienThoai.Index)
            {
                tb.MaxLength = NhaCungCapValidator.DoDaiToiDaDienThoaiNhap;
                tb.KeyPress += new KeyPressEventHandler(DienThoai_KeyPress);
            }
        }

        // Ô điện thoại chỉ nhận chữ số và + khoảng-trắng . - ( )
        private void DienThoai_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (char.IsControl(ch) || (ch >= '0' && ch <= '9') || "+ .-()".IndexOf(ch) >= 0) return;
            e.Handled = true;
        }

        // ------------------------------------------------------------------
        // Tìm kiếm nhanh (thanh công cụ)
        // ------------------------------------------------------------------

        private bool DangHienGoiY()
        {
            string t = toolTimNhaCungCap.Text;
            return t == GoiYTimTen || t == GoiYTimDiaChi;
        }

        // Chữ gợi ý màu xám đậm (đọc được), chữ người dùng gõ màu đen.
        private void DatGoiYTimKiem()
        {
            toolTimNhaCungCap.Text = toolTimHoTen.Checked ? GoiYTimTen : GoiYTimDiaChi;
            toolTimNhaCungCap.ForeColor = SystemColors.GrayText;
        }

        private void toolTimNhaCungCap_Enter(object sender, EventArgs e)
        {
            if (DangHienGoiY())
                toolTimNhaCungCap.Text = "";
            toolTimNhaCungCap.ForeColor = SystemColors.WindowText;
        }

        private void toolTimNhaCungCap_Leave(object sender, EventArgs e)
        {
            if (toolTimNhaCungCap.Text.Trim().Length == 0)
                DatGoiYTimKiem();
        }

        private void toolTimNhaCungCap_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                e.Handled = true;
                TimKiem();
            }
        }

        private void TimKiem()
        {
            if (Nguon == null) return;

            ChotNhap();
            if (DemChuaLuu() > 0)
            {
                if (MessageBox.Show("Tìm kiếm sẽ tải lại danh sách và hủy các thay đổi chưa lưu.\r\nBạn có muốn tiếp tục không?",
                    TieuDe, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
            }

            string tuKhoa = DangHienGoiY() ? "" : NhaCungCapValidator.ChuanHoaChuoi(toolTimNhaCungCap.Text);

            dangTai = true;
            try
            {
                if (tuKhoa.Length == 0) ctrl.TaiLai();
                else if (toolTimHoTen.Checked) ctrl.TimHoTen(tuKhoa);
                else ctrl.TimDiaChi(tuKhoa);
            }
            finally
            {
                dangTai = false;
            }

            tongTrongCSDL = ctrl.DemTong();
            XoaLoiHienThi();
            DatCheDo(Controll.Normal);
        }

        private void toolTimHoTen_Click(object sender, EventArgs e)
        {
            toolTimHoTen.Checked = true;
            toolTimDiaChi.Checked = false;
            DatGoiYTimKiem();
            bindingNavigator.Focus();
        }

        private void toolTimDiaChi_Click(object sender, EventArgs e)
        {
            toolTimHoTen.Checked = false;
            toolTimDiaChi.Checked = true;
            DatGoiYTimKiem();
            bindingNavigator.Focus();
        }

        // ------------------------------------------------------------------
        // Lọc nâng cao
        // ------------------------------------------------------------------

        private void toolLoc_Click(object sender, EventArgs e)
        {
            pnlLoc.Visible = toolLoc.Checked;
            if (toolLoc.Checked) txtLocTen.Focus();
            else BoLoc();
        }

        private void btnLoc_Click(object sender, EventArgs e)
        {
            ApDungLoc();
        }

        private void btnBoLoc_Click(object sender, EventArgs e)
        {
            BoLoc();
        }

        private void txtLoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                e.Handled = true;
                ApDungLoc();
            }
        }

        private void ApDungLoc()
        {
            if (Nguon == null) return;
            ChotNhap();
            Nguon.Filter = NhaCungCapController.TaoBieuThucLoc(txtLocTen.Text, txtLocDiaChi.Text, txtLocDienThoai.Text);
        }

        private void BoLoc()
        {
            txtLocTen.Text = "";
            txtLocDiaChi.Text = "";
            txtLocDienThoai.Text = "";
            if (Nguon != null && Nguon.Filter != null)
                Nguon.Filter = null;
        }
    }
}