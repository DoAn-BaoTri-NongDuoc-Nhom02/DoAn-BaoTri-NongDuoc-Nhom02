using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    /// <summary>
    /// PM-31: Du no KH — hien thi, loc, sap xep, ro nut — giu design thong nhat.
    /// </summary>
    public partial class frmDunoKhachhang : Form
    {
        DuNoKhachHangController ctrl = new DuNoKhachHangController();
        KhachHangController ctrlKH = new KhachHangController();
        ToolStripTextBox txtTimKH;
        ToolStripLabel lblThongBao;

        public frmDunoKhachhang()
        {
            InitializeComponent();
        }

        private void frmDunoKhachhang_Load(object sender, EventArgs e)
        {
            this.Text = "Dư nợ khách hàng";
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.MinimumSize = new Size(800, 450);
            this.BackColor = Color.FromArgb(192, 255, 192);

            try { if (dataGridView != null) dataGridView.Dock = DockStyle.Fill; } catch { }
            try { if (bindingNavigator != null) bindingNavigator.BackColor = Color.FromArgb(192, 255, 192); } catch { }
            try { if (panel2 != null) panel2.BackColor = Color.FromArgb(192, 255, 192); } catch { }

            // "of {0}" -> "/ {0} bản ghi"
            try { bindingNavigator.CountItemFormat = "/ {0} bản ghi"; } catch { }

            // Nut tong hop ro nghia
            try
            {
                toolTongHop.Text = "Tổng hợp dư nợ";
                toolTongHop.ToolTipText = "Tính lại dư nợ theo tháng / năm đang chọn";
                toolLuu.ToolTipText = "Lưu bảng dư nợ đã tổng hợp vào CSDL";
                toolIn.ToolTipText = "In phiếu dư nợ của dòng đang chọn";
                toolThoat.ToolTipText = "Đóng form";
            }
            catch { }

            BoSungTimVaCot();
            SapXepNutChuan();

            this.toolThang.SelectedIndex = DateTime.Now.Month - 1;
            this.toolNam.Text = DateTime.Now.Year.ToString();
            try { ctrlKH.HienthiKhachHangChungDataGridviewComboBox(colKhachHang); } catch { }

            dataGridView.DataError -= dataGridView_DataError;
            dataGridView.DataError += dataGridView_DataError;
            dataGridView.DataBindingComplete -= dataGridView_DataBindingComplete;
            dataGridView.DataBindingComplete += dataGridView_DataBindingComplete;

            try
            {
                if (toolThang.SelectedIndex < 0) toolThang.SelectedIndex = DateTime.Now.Month - 1;
                if (string.IsNullOrEmpty(toolNam.Text) || toolNam.Text.Trim().Length == 0)
                    toolNam.Text = DateTime.Now.Year.ToString();
                toolTongHop_Click(null, EventArgs.Empty);
            }
            catch { }
        }

        void BoSungTimVaCot()
        {
            try
            {
                // Cot Ma KH (text) — doi chieu
                if (!dataGridView.Columns.Contains("colMaKH"))
                {
                    DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                    col.Name = "colMaKH";
                    col.HeaderText = "Mã KH";
                    col.DataPropertyName = "ID_KHACH_HANG";
                    col.Width = 90;
                    col.ReadOnly = true;
                    dataGridView.Columns.Insert(0, col);
                }
            }
            catch { }

            try
            {
                if (bindingNavigator.Items["txtTimKH"] != null) return;
                bindingNavigator.Items.Add(new ToolStripSeparator());
                txtTimKH = new ToolStripTextBox();
                txtTimKH.Name = "txtTimKH";
                txtTimKH.AutoSize = false;
                txtTimKH.Width = 160;
                txtTimKH.Text = "Lọc theo mã / tên KH...";
                txtTimKH.ForeColor = Color.Gray;
                txtTimKH.Enter += delegate
                {
                    if (txtTimKH.ForeColor == Color.Gray)
                    {
                        txtTimKH.Text = "";
                        txtTimKH.ForeColor = Color.Black;
                    }
                };
                txtTimKH.Leave += delegate
                {
                    if (string.IsNullOrEmpty(txtTimKH.Text.Trim()))
                    {
                        txtTimKH.Text = "Lọc theo mã / tên KH...";
                        txtTimKH.ForeColor = Color.Gray;
                    }
                };
                txtTimKH.TextChanged += txtTimKH_TextChanged;
                bindingNavigator.Items.Add(txtTimKH);

                ToolStripButton xoaLoc = new ToolStripButton();
                xoaLoc.Text = "Xóa lọc";
                xoaLoc.DisplayStyle = ToolStripItemDisplayStyle.Text;
                xoaLoc.Click += delegate
                {
                    if (txtTimKH != null)
                    {
                        txtTimKH.Text = "Lọc theo mã / tên KH...";
                        txtTimKH.ForeColor = Color.Gray;
                    }
                    try
                    {
                        if (bindingNavigator.BindingSource != null)
                            bindingNavigator.BindingSource.Filter = "";
                    }
                    catch { }
                    CapNhatThongBao();
                };
                bindingNavigator.Items.Add(xoaLoc);

                lblThongBao = new ToolStripLabel(" ");
                lblThongBao.ForeColor = Color.DimGray;
                bindingNavigator.Items.Add(lblThongBao);
            }
            catch { }
        }

        void txtTimKH_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (bindingNavigator.BindingSource == null) return;
                if (txtTimKH == null || txtTimKH.ForeColor == Color.Gray) return;
                string k = (txtTimKH.Text ?? "").Trim().Replace("'", "''");
                if (k.Length == 0)
                {
                    bindingNavigator.BindingSource.Filter = "";
                }
                else
                {
                    bindingNavigator.BindingSource.Filter =
                        "Convert(ID_KHACH_HANG, 'System.String') LIKE '%" + k + "%'";
                }
                CapNhatThongBao();
            }
            catch { }
        }

        void CapNhatThongBao()
        {
            try
            {
                int n = 0;
                if (bindingNavigator.BindingSource != null)
                    n = bindingNavigator.BindingSource.Count;
                if (lblThongBao != null)
                    lblThongBao.Text = n == 0 ? "Không có dữ liệu." : (" " + n + " dòng");
            }
            catch { }
        }

        void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            try { e.Cancel = true; } catch { }
        }

        void dataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ToMauDuNoLon();
            CapNhatThongBao();
        }

        /// <summary>PM-31: tô màu cuối kỳ lớn.</summary>
        void ToMauDuNoLon()
        {
            try
            {
                if (dataGridView == null || !dataGridView.Columns.Contains("colConNo")) return;
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.IsNewRow) continue;
                    long ck = 0;
                    try
                    {
                        object v = row.Cells["colConNo"].Value;
                        if (v != null && v != DBNull.Value)
                            ck = Convert.ToInt64(v);
                    }
                    catch { }
                    if (ck > 5000000)
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220); // do nhat
                    else if (ck > 1000000)
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 245, 200); // vang nhat
                    else if (ck < 0)
                        row.DefaultCellStyle.BackColor = Color.FromArgb(220, 240, 255); // xanh nhat
                    else
                        row.DefaultCellStyle.BackColor = Color.White;
                }
            }
            catch { }
        }


        /// <summary>Bo nut chuan giong Phieu thu/chi: Them, Xoa, Sua, Huy, Nhat ky.</summary>
        void SapXepNutChuan()
        {
            try
            {
                if (bindingNavigator == null) return;
                bindingNavigator.BackColor = Color.FromArgb(192, 255, 192);
                bindingNavigator.ImageScalingSize = new Size(24, 24);

                // Cho phep sua luoi (chinh du tam)
                dataGridView.ReadOnly = false;
                dataGridView.AllowUserToAddRows = false;
                dataGridView.AllowUserToDeleteRows = false;

                if (bindingNavigator.Items["toolThem"] != null) return;

                ToolStripButton them = TaoNut("toolThem", "Thêm", toolThem_Click);
                ToolStripButton xoa = TaoNut("toolXoa", "Xóa", toolXoa_Click);
                ToolStripButton sua = TaoNut("toolSua", "Chỉnh sửa", toolSua_Click);
                ToolStripButton huy = TaoNut("toolHuy", "Hủy", toolHuy_Click);
                ToolStripButton nhatKy = TaoNut("toolNhatKy", "Nhật ký", toolNhatKy_Click);

                // Chen truoc toolLuu neu co
                int idx = 0;
                for (int i = 0; i < bindingNavigator.Items.Count; i++)
                {
                    if (bindingNavigator.Items[i] == toolLuu) { idx = i; break; }
                }
                bindingNavigator.Items.Insert(idx, them);
                bindingNavigator.Items.Insert(idx + 1, xoa);
                // toolLuu da co
                int idxLuu = bindingNavigator.Items.IndexOf(toolLuu);
                bindingNavigator.Items.Insert(idxLuu + 1, sua);
                bindingNavigator.Items.Insert(idxLuu + 2, huy);
                bindingNavigator.Items.Insert(idxLuu + 3, nhatKy);
            }
            catch { }
        }

        ToolStripButton TaoNut(string name, string text, EventHandler click)
        {
            ToolStripButton b = new ToolStripButton();
            b.Name = name;
            b.Text = text;
            b.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            b.TextImageRelation = TextImageRelation.ImageAboveText;
            b.ImageScaling = ToolStripItemImageScaling.None;
            try
            {
                if (name.Contains("Them"))
                    b.Image = global::CuahangNongduoc.Properties.Resources.add;
                else if (name.Contains("Xoa"))
                    b.Image = global::CuahangNongduoc.Properties.Resources.remove;
                else if (name.Contains("Sua"))
                    b.Image = global::CuahangNongduoc.Properties.Resources.edit;
                else if (name.Contains("Huy"))
                    b.Image = global::CuahangNongduoc.Properties.Resources.undo;
                else if (name.Contains("NhatKy"))
                    b.Image = global::CuahangNongduoc.Properties.Resources.info;
            }
            catch { }
            b.Click += click;
            return b;
        }

        void toolThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (bindingNavigator.BindingSource == null)
                {
                    MessageBox.Show("Hãy bấm Tổng hợp dư nợ trước.", this.Text,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DataTable tbl = null;
                if (bindingNavigator.BindingSource.DataSource is DataTable)
                    tbl = (DataTable)bindingNavigator.BindingSource.DataSource;
                else if (bindingNavigator.BindingSource.DataSource is DataView)
                    tbl = ((DataView)bindingNavigator.BindingSource.DataSource).Table;
                if (tbl == null)
                {
                    MessageBox.Show("Chưa có bảng dữ liệu. Bấm Tổng hợp dư nợ trước.", this.Text,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DataRow r = tbl.NewRow();
                if (tbl.Columns.Contains("THANG")) r["THANG"] = toolThang.SelectedIndex + 1;
                if (tbl.Columns.Contains("NAM")) r["NAM"] = Convert.ToInt32(toolNam.Text);
                if (tbl.Columns.Contains("DAU_KY")) r["DAU_KY"] = 0;
                if (tbl.Columns.Contains("PHAT_SINH")) r["PHAT_SINH"] = 0;
                if (tbl.Columns.Contains("DA_TRA")) r["DA_TRA"] = 0;
                if (tbl.Columns.Contains("CUOI_KY")) r["CUOI_KY"] = 0;
                tbl.Rows.Add(r);
                bindingNavigator.BindingSource.MoveLast();
                dataGridView.ReadOnly = false;
                MessageBox.Show("Đã thêm dòng trống. Chọn khách hàng, nhập số liệu rồi bấm Lưu.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thêm lỗi:\n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        void toolXoa_Click(object sender, EventArgs e)
        {
            if (bindingNavigator.BindingSource == null || bindingNavigator.BindingSource.Current == null)
            {
                MessageBox.Show("Chọn dòng cần xóa.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Xóa dòng dư nợ đang chọn?", this.Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            try
            {
                bindingNavigator.BindingSource.RemoveCurrent();
                CapNhatThongBao();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xóa lỗi:\n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        void toolSua_Click(object sender, EventArgs e)
        {
            if (bindingNavigator.BindingSource == null || bindingNavigator.BindingSource.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu để sửa. Bấm Tổng hợp dư nợ trước.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            dataGridView.ReadOnly = false;
            dataGridView.Focus();
            MessageBox.Show("Đang chế độ chỉnh sửa. Sửa trên lưới rồi bấm Lưu.\nBấm Hủy để tải lại từ tổng hợp.", this.Text,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void toolHuy_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Hủy thay đổi và tổng hợp lại theo tháng/năm đang chọn?", this.Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            toolTongHop_Click(sender, e);
        }

        void toolNhatKy_Click(object sender, EventArgs e)
        {
            try
            {
                string ma = "";
                try
                {
                    if (bindingNavigator.BindingSource != null && bindingNavigator.BindingSource.Current is DataRowView)
                    {
                        DataRowView rv = (DataRowView)bindingNavigator.BindingSource.Current;
                        ma = Convert.ToString(rv["ID_KHACH_HANG"]);
                    }
                }
                catch { }
                using (frmNhatKyPhieu f = new frmNhatKyPhieu("DuNo", "dư nợ KH", ma, toolThang.Text + "/" + toolNam.Text))
                    f.ShowDialog(this);
            }
            catch
            {
                MessageBox.Show(
                    "Nhật ký dư nợ tháng " + (toolThang.SelectedIndex + 1) + "/" + toolNam.Text +
                    ".\nDùng Tổng hợp / Lưu để ghi nhận thay đổi.",
                    "Nhật ký", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnTongHop_Click(object sender, EventArgs e)
        {
            toolTongHop_Click(sender, e);
        }

        private void toolNam_Validating(object sender, CancelEventArgs e)
        {
            bool ok = true;
            try
            {
                long nam = Convert.ToInt32(toolNam.Text);
                if (nam < 2000 || nam > 9999) ok = false;
            }
            catch { ok = false; }
            if (!ok)
            {
                MessageBox.Show("Thông tin năm không hợp lệ!", "Tổng hợp dư nợ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void toolTongHop_Click(object sender, EventArgs e)
        {
            try
            {
                int thang = toolThang.SelectedIndex + 1;
                int nam = Convert.ToInt32(toolNam.Text);
                toolProgress.Visible = true;
                ctrl.Tonghop(thang, nam, toolProgress, dataGridView, bindingNavigator);
                toolProgress.Visible = false;

                try { ctrlKH.HienthiKhachHangChungDataGridviewComboBox(colKhachHang); } catch { }
                ToMauDuNoLon();
                CapNhatThongBao();

                int n = 0;
                try
                {
                    if (bindingNavigator.BindingSource != null)
                        n = bindingNavigator.BindingSource.Count;
                }
                catch { }

                if (n == 0)
                {
                    MessageBox.Show(
                        "Không có dữ liệu dư nợ tháng " + thang + "/" + nam + ".\n" +
                        "Có thể chưa có khách hàng hoặc chưa phát sinh bán / thu.",
                        "Tổng hợp dư nợ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                try { toolProgress.Visible = false; } catch { }
                MessageBox.Show("Tổng hợp lỗi:\n" + ex.Message, "Tổng hợp dư nợ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void toolLuu_Click(object sender, EventArgs e)
        {
            try
            {
                toolThang.Focus();
                ctrl.Save();
                MessageBox.Show("Đã lưu bảng dư nợ.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lưu lỗi:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolIn_Click(object sender, EventArgs e)
        {
            if (bindingNavigator.BindingSource == null || bindingNavigator.BindingSource.Current == null)
            {
                MessageBox.Show("Chọn một dòng dư nợ trước khi in.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                DataRowView row = (DataRowView)bindingNavigator.BindingSource.Current;
                if (row == null || row.Row.RowState == DataRowState.Deleted)
                {
                    MessageBox.Show("Dòng đã bị xóa hoặc không hợp lệ.", this.Text,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DuNoKhachHang dn = new DuNoKhachHang();
                dn.Thang = Convert.ToInt32(row["THANG"]);
                dn.Nam = Convert.ToInt32(row["NAM"]);
                dn.DauKy = Convert.ToInt64(row["DAU_KY"]);
                dn.PhatSinh = Convert.ToInt64(row["PHAT_SINH"]);
                dn.DaTra = Convert.ToInt64(row["DA_TRA"]);
                dn.CuoiKy = Convert.ToInt64(row["CUOI_KY"]);
                dn.KhachHang = ctrlKH.LayKhachHang(Convert.ToString(row["ID_KHACH_HANG"]));

                frmInDunoKhachHang InDuNo = new frmInDunoKhachHang(dn);
                InDuNo.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không in được:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
