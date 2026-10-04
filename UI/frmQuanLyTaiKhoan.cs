using CuahangNongduoc.BLL.Service;
using CuahangNongduoc.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc.UI
{
    public partial class frmQuanLyTaiKhoan : Form
    {
        private NguoiDungService _service = new NguoiDungService();
        private int _idDangChon = 0;

        public frmQuanLyTaiKhoan()
        {
            InitializeComponent();
        }

        private void frmQuanLyTaiKhoan_Load(object sender, EventArgs e)
        {
            LoadVaiTro();
            LoadDanhSach();
            ClearForm();
        }

        private void LoadVaiTro()
        {
            cboVaiTro.DataSource = _service.GetAllVaiTro();
            cboVaiTro.DisplayMember = "TenVaiTro";
            cboVaiTro.ValueMember = "ID";
        }

        private void LoadDanhSach()
        {
            dgvTaiKhoan.DataSource = null;
            dgvTaiKhoan.DataSource = _service.GetAll();

            // Ẩn các cột không cần hiện
            if (dgvTaiKhoan.Columns["MatKhau"] != null)
                dgvTaiKhoan.Columns["MatKhau"].Visible = false;
            if (dgvTaiKhoan.Columns["IDVaiTro"] != null)
                dgvTaiKhoan.Columns["IDVaiTro"].Visible = false;
            if (dgvTaiKhoan.Columns["TrangThai"] != null)
                dgvTaiKhoan.Columns["TrangThai"].Visible = false;

            // Đổi tên cột sang tiếng Việt cho dễ nhìn
            if (dgvTaiKhoan.Columns["ID"] != null)
                dgvTaiKhoan.Columns["ID"].HeaderText = "Mã";
            if (dgvTaiKhoan.Columns["TenDangNhap"] != null)
                dgvTaiKhoan.Columns["TenDangNhap"].HeaderText = "Tên đăng nhập";
            if (dgvTaiKhoan.Columns["HoTen"] != null)
                dgvTaiKhoan.Columns["HoTen"].HeaderText = "Họ tên";
            if (dgvTaiKhoan.Columns["TenVaiTro"] != null)
                dgvTaiKhoan.Columns["TenVaiTro"].HeaderText = "Vai trò";

            // Cho cột tự co giãn vừa khung
            dgvTaiKhoan.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ClearForm()
        {
            _idDangChon = 0;
            txtTenDangNhap.Clear();
            txtMatKhau.Clear();
            txtHoTen.Clear();
            if (cboVaiTro.Items.Count > 0)
                cboVaiTro.SelectedIndex = 0;
            txtTenDangNhap.Focus();
        }

        private void dgvTaiKhoan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvTaiKhoan.Rows[e.RowIndex];
            _idDangChon = Convert.ToInt32(row.Cells["ID"].Value);
            txtTenDangNhap.Text = row.Cells["TenDangNhap"].Value.ToString();
            txtMatKhau.Text = row.Cells["MatKhau"].Value.ToString();
            txtHoTen.Text = row.Cells["HoTen"].Value.ToString();
            cboVaiTro.SelectedValue = Convert.ToInt32(row.Cells["IDVaiTro"].Value);
        }

        private NguoiDung LayDuLieuTuForm()
        {
            return new NguoiDung
            {
                ID = _idDangChon,
                TenDangNhap = txtTenDangNhap.Text.Trim(),
                MatKhau = txtMatKhau.Text,
                HoTen = txtHoTen.Text.Trim(),
                IDVaiTro = Convert.ToInt32(cboVaiTro.SelectedValue),
                TrangThai = true
            };
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            NguoiDung nd = LayDuLieuTuForm();
            string ketQua = _service.Them(nd);

            if (ketQua == "OK")
            {
                MessageBox.Show("Thêm tài khoản thành công!", "Thông báo");
                LoadDanhSach();
                ClearForm();
            }
            else
            {
                MessageBox.Show(ketQua, "Lỗi");
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (_idDangChon == 0)
            {
                MessageBox.Show("Bạn chưa chọn tài khoản nào!");
                return;
            }

            NguoiDung nd = LayDuLieuTuForm();
            string ketQua = _service.Sua(nd);

            if (ketQua == "OK")
            {
                MessageBox.Show("Cập nhật thành công!", "Thông báo");
                LoadDanhSach();
                ClearForm();
            }
            else
            {
                MessageBox.Show(ketQua, "Lỗi");
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (_idDangChon == 0)
            {
                MessageBox.Show("Bạn chưa chọn tài khoản nào!");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa tài khoản này?", "Xác nhận",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string ketQua = _service.Xoa(_idDangChon);
                if (ketQua == "OK")
                {
                    MessageBox.Show("Xóa thành công!");
                    LoadDanhSach();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(ketQua, "Lỗi");
                }
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ClearForm();
            LoadDanhSach();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
