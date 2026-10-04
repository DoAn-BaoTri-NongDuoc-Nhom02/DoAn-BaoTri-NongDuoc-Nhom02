using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.Controller;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc
{
    public partial class frmBanLe: Form
    {
        SanPhamController ctrlSanPham = new SanPhamController();
        KhachHangController ctrlKhachHang = new KhachHangController();
        MaSanPhamController ctrlMaSanPham = new MaSanPhamController();
        PhieuBanController ctrlPhieuBan = new PhieuBanController();
        ChiTietPhieuBanController ctrlChiTiet = new ChiTietPhieuBanController();
        XuatKhoController ctrlXuatKho = new XuatKhoController();
        IList<MaSanPham> deleted = new List<MaSanPham>();
        Controll status = Controll.Normal;

        public frmBanLe()
        {
            InitializeComponent();
            status = Controll.AddNew;
        }

     
        public frmBanLe(PhieuBanController ctrlPB)
            : this()
        {
            this.ctrlPhieuBan = ctrlPB;
            status = Controll.Normal;
        }

        private void frmNhapHang_Load(object sender, EventArgs e)
        {

            // Cấu hình xuất kho: FIFO thì hệ thống tự phân lô, chỉ định thì cho chọn lô.
            cmbMaSanPham.Enabled = (CauHinhKho.PhuongPhapXuatKho == PhuongPhapXuatKho.ChiDinh);

            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            ctrlMaSanPham.HienThiDataGridViewComboBox(colMaSanPham);

            cmbSanPham.SelectedIndexChanged += new EventHandler(cmbSanPham_SelectedIndexChanged);

            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, false);

            ctrlPhieuBan.HienthiPhieuBan(bindingNavigator,cmbKhachHang, txtMaPhieu, dtNgayLapPhieu, numTongTien, numDaTra, numConNo);

            bindingNavigator.BindingSource.CurrentChanged -= new EventHandler(BindingSource_CurrentChanged);
            bindingNavigator.BindingSource.CurrentChanged += new EventHandler(BindingSource_CurrentChanged);
            
            ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);


            if (status == Controll.AddNew)
            {
                txtMaPhieu.Text = ThamSo.LayMaPhieuBan().ToString();
            }
            else
            {
                this.Allow(false);
            }


        }

        void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (status == Controll.Normal)
            {
                ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);
            }
        }


        void cmbSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSanPham.SelectedValue != null)
            {
                MaSanPhamController ctrlMSP = new MaSanPhamController();

                cmbMaSanPham.SelectedIndexChanged -= new EventHandler(cmbMaSanPham_SelectedIndexChanged);
                ctrlMSP.HienThiAutoComboBox(cmbSanPham.SelectedValue.ToString(), cmbMaSanPham);
                cmbMaSanPham.SelectedIndexChanged += new EventHandler(cmbMaSanPham_SelectedIndexChanged);

                // Lô đầu danh sách (hạn dùng gần nhất) được chọn sẵn: cập nhật giá/thông tin ngay.
                cmbMaSanPham_SelectedIndexChanged(cmbMaSanPham, EventArgs.Empty);
            }
        }

        void cmbMaSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMaSanPham.SelectedValue == null)
            {
                // Sản phẩm hết hàng: không còn lô nào để hiển thị.
                numDonGia.Value = 0;
                txtGiaNhap.Text = "";
                txtGiaXuat.Text = "";
                return;
            }

            MaSanPhamController ctrl = new MaSanPhamController();
            MaSanPham masp = ctrl.LayMaSanPham(cmbMaSanPham.SelectedValue.ToString());
            numDonGia.Value = masp.SanPham.GiaBanLe;
            txtGiaNhap.Text = masp.GiaNhap.ToString("#,###0");
            txtGiaBanSi.Text = masp.SanPham.GiaBanSi.ToString("#,###0");
            txtGiaBanLe.Text = masp.SanPham.GiaBanLe.ToString("#,###0");
            txtGiaBQGQ.Text = masp.SanPham.DonGiaNhap.ToString("#,###0");
            txtGiaXuat.Text = ctrlXuatKho.TinhGiaXuat(masp).ToString("#,###0");

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            bool chiDinh = (CauHinhKho.PhuongPhapXuatKho == PhuongPhapXuatKho.ChiDinh);

            if (cmbSanPham.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Sản phẩm !", "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (chiDinh && cmbMaSanPham.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Mã sản phẩm !", "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (numSoLuong.Value <= 0)
            {
                MessageBox.Show("Vui lòng nhập Số lượng !", "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (numDonGia.Value * numSoLuong.Value != numThanhTien.Value)
            {
                MessageBox.Show("Thành tiền sai!", "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    ThemDongChiTiet(chiDinh);
                }
                catch (TonKhoException ex)
                {
                    MessageBox.Show(ex.Message, "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // Thêm hàng vào phiếu: FIFO thì tách theo từng lô (hạn dùng gần nhất trước), chỉ định thì đúng lô đã chọn.
        // Số lượng các dòng đã có trong phiếu (chưa lưu) được tính vào để không bán vượt tồn.
        void ThemDongChiTiet(bool chiDinh)
        {
            int soLuong = (int)numSoLuong.Value;
            IDictionary<String, int> daChon = ctrlChiTiet.SoLuongDangChon();

            IList<PhanBoLo> phanBo;
            if (chiDinh)
                phanBo = ctrlXuatKho.PhanLoChiDinh(cmbMaSanPham.SelectedValue.ToString(), soLuong, daChon);
            else
                phanBo = ctrlXuatKho.PhanLoFIFO(cmbSanPham.SelectedValue.ToString(), soLuong, daChon);

            // Khóa chính chi tiết là (phiếu, lô): không thêm trùng vào dòng đã lưu trước đó.
            foreach (PhanBoLo pb in phanBo)
            {
                if (ctrlChiTiet.DaCoDongDaLuu(pb.Lo.Id))
                {
                    throw new TonKhoException("Lô '" + pb.Lo.Id + "' đã có trong phiếu đã lưu. " +
                        "Hãy xóa dòng đó rồi thêm lại với số lượng mới.");
                }
            }

            foreach (PhanBoLo pb in phanBo)
            {
                DataRow dong = ctrlChiTiet.TimDongChuaLuu(pb.Lo.Id);
                if (dong != null)
                {
                    // Cùng lô đã có dòng chưa lưu: gộp số lượng.
                    int sl = Convert.ToInt32(dong["SO_LUONG"]) + pb.SoLuong;
                    dong["SO_LUONG"] = sl;
                    dong["THANH_TIEN"] = numDonGia.Value * sl;
                }
                else
                {
                    DataRow row = ctrlChiTiet.NewRow();
                    row["ID_MA_SAN_PHAM"] = pb.Lo.Id;
                    row["ID_PHIEU_BAN"] = txtMaPhieu.Text;
                    row["DON_GIA"] = numDonGia.Value;
                    row["SO_LUONG"] = pb.SoLuong;
                    row["THANH_TIEN"] = numDonGia.Value * pb.SoLuong;
                    ctrlChiTiet.Add(row);
                }
            }
            numTongTien.Value += numThanhTien.Value;
        }

        // Dòng vừa thêm (chưa lưu) chưa trừ kho nên khi xóa không được hoàn kho.
        void GhiNhanXoaDong(DataRowView row)
        {
            if (row.Row.RowState != DataRowState.Added)
            {
                deleted.Add(new MaSanPham(Convert.ToString(row["ID_MA_SAN_PHAM"]), Convert.ToInt32(row["SO_LUONG"])));
            }
        }

        private void numDonGia_ValueChanged(object sender, EventArgs e)
        {
            numThanhTien.Value = numDonGia.Value * numSoLuong.Value;
        }

        private void numTongTien_ValueChanged(object sender, EventArgs e)
        {
            numConNo.Value = numTongTien.Value - numDaTra.Value;
        }

        private void toolLuu_Click(object sender, EventArgs e)
        {
            bindingNavigatorPositionItem.Focus();
            if (!this.Luu())
                return;
            status = Controll.Normal;
            this.Allow(false);
        }

        // Trả về false nếu chưa lưu được (thiếu hàng, trùng mã phiếu...): phiếu vẫn ở trạng thái đang soạn.
        bool Luu()
        {
            if (!KiemTraTruocKhiLuu())
                return false;

            try
            {
                if (status == Controll.AddNew)
                {
                    return ThemMoi();
                }
                CapNhat();
                return true;
            }
            catch (TonKhoException ex)
            {
                MessageBox.Show(ex.Message, "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        // Kiểm tra tồn kho cho các dòng mới TRƯỚC khi ghi bất cứ thứ gì (phiếu, chi tiết, kho).
        bool KiemTraTruocKhiLuu()
        {
            IDictionary<String, int> hoanTra = new Dictionary<String, int>();
            foreach (MaSanPham masp in deleted)
            {
                int cu;
                hoanTra.TryGetValue(masp.Id, out cu);
                hoanTra[masp.Id] = cu + masp.SoLuong;
            }

            try
            {
                ctrlChiTiet.KiemTraTonKho(hoanTra);
                return true;
            }
            catch (TonKhoException ex)
            {
                MessageBox.Show(ex.Message, "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        void CapNhat()
        {
            foreach (MaSanPham masp in deleted)
            {
                CuahangNongduoc.DataLayer.MaSanPhanFactory.CapNhatSoLuong(masp.Id, masp.SoLuong);
            }
            deleted.Clear();

            ctrlChiTiet.Save();

            ctrlPhieuBan.Update();
        }

        bool ThemMoi()
        {
            PhieuBanController ctrl = new PhieuBanController();

            if (ctrl.LayPhieuBan(txtMaPhieu.Text) != null)
            {
                MessageBox.Show("Mã Phiếu bán này đã tồn tại !", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            DataRow row = ctrlPhieuBan.NewRow();
            row["ID"] = txtMaPhieu.Text;
            row["ID_KHACH_HANG"] = cmbKhachHang.SelectedValue;
            row["NGAY_BAN"] = dtNgayLapPhieu.Value.Date;
            row["TONG_TIEN"] = numTongTien.Value;
            row["DA_TRA"] = numDaTra.Value;
            row["CON_NO"] = numConNo.Value;
            ctrlPhieuBan.Add(row);

            if (ThamSo.LaSoNguyen(txtMaPhieu.Text))
            {
                long so = Convert.ToInt64(txtMaPhieu.Text);
                if (so >= ThamSo.LayMaPhieuBan())
                {
                    ThamSo.GanMaPhieuBan(so + 1);
                }
            }

            ctrlPhieuBan.Save();

            ctrlChiTiet.Save();
            return true;
        }

        private void toolLuu_Them_Click(object sender, EventArgs e)
        {
            ctrlPhieuBan = new PhieuBanController();
            status = Controll.AddNew;
            txtMaPhieu.Text = ThamSo.LayMaPhieuBan().ToString();
            numTongTien.Value = 0;
            ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);
            this.Allow(true);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phieu Ban Le", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                BindingSource bs = ((BindingSource)dgvDanhsachSP.DataSource);
                DataRowView row = (DataRowView)bs.Current;
                numTongTien.Value -= Convert.ToInt64(row["THANH_TIEN"]);
                GhiNhanXoaDong(row);
                bs.RemoveCurrent();
            }
           
        }

        private void dgvDanhsachSP_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phieu Ban Le", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                BindingSource bs = ((BindingSource)dgvDanhsachSP.DataSource);
                DataRowView row = (DataRowView)bs.Current;
                numTongTien.Value -= Convert.ToInt64(row["THANH_TIEN"]);
                GhiNhanXoaDong(row);

            }
        }

        private void toolLuuIn_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                MessageBox.Show("Vui lòng lưu lại Phiếu bán hiện tại!", "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                String ma_phieu = txtMaPhieu.Text;

                PhieuBanController ctrlPB = new PhieuBanController();

                CuahangNongduoc.BusinessObject.PhieuBan ph = ctrlPB.LayPhieuBan(ma_phieu);

                frmInPhieuBan InPhieuBan = new frmInPhieuBan(ph);

                InPhieuBan.Show();

            }
        }

        private void dgvDanhsachSP_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;

        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {

        }

        private void toolChinhSua_Click(object sender, EventArgs e)
        {
            status = Controll.Edit;
            this.Allow(true);
        }

        void Allow(bool val)
        {
            txtMaPhieu.Enabled = val;
            dtNgayLapPhieu.Enabled = val;
            numTongTien.Enabled = val;
            btnAdd.Enabled = val;
            btnRemove.Enabled = val;
            dgvDanhsachSP.Enabled = val;
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                if (MessageBox.Show("Bạn có muốn lưu lại Phiếu bán này không?", "Phieu Ban Le", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (!this.Luu())
                        return;   // chưa lưu được: ở lại để người dùng sửa
                }
            }
            this.Close();
        }

        private void toolXoa_Click(object sender, EventArgs e)
        {
            DataRowView view =  (DataRowView)bindingNavigator.BindingSource.Current;
            if (view != null)
            {
                if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Phieu Ban Le", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ChiTietPhieuBanController ctrl = new ChiTietPhieuBanController();
                    IList<ChiTietPhieuBan> ds = ctrl.ChiTietPhieuBan(view["ID"].ToString());
                    foreach (ChiTietPhieuBan ct in ds)
                    {
                        CuahangNongduoc.DataLayer.MaSanPhanFactory.CapNhatSoLuong(ct.MaSanPham.Id, ct.SoLuong);
                    }
                    bindingNavigator.BindingSource.RemoveCurrent();
                    ctrlPhieuBan.Save();
                }
            }
        }

        private void btnThemDaiLy_Click(object sender, EventArgs e)
        {
            frmKhachHang KhachHang = new frmKhachHang();
            KhachHang.ShowDialog();
            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, false);
        }

        private void btnThemSanPham_Click(object sender, EventArgs e)
        {
            frmSanPham SanPham = new frmSanPham();
            SanPham.ShowDialog();
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
        }


     }
}
