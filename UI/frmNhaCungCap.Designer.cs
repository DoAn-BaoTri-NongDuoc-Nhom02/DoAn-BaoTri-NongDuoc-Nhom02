namespace CuahangNongduoc
{
    partial class frmNhaCungCap
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNhaCungCap));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.colID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHoTen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDiaChi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDienThoai = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblChuaCoDuLieu = new System.Windows.Forms.Label();
            this.bindingNavigator = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.toolSua = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.toolLuu = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolTimKiem = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolTimTatCa = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTimHoTen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTimDiaChi = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTimDienThoai = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTimNhaCungCap = new System.Windows.Forms.ToolStripTextBox();
            this.toolXoaLoc = new System.Windows.Forms.ToolStripButton();
            this.toolThoat = new System.Windows.Forms.ToolStripButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblTong = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblThongBao = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.dataGridView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator)).BeginInit();
            this.bindingNavigator.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(247)))), ((int)(((byte)(252)))));
            this.dataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(8, 3, 8, 3);
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colID,
            this.colHoTen,
            this.colDiaChi,
            this.colDienThoai});
            this.dataGridView.Controls.Add(this.lblChuaCoDuLieu);
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(8, 3, 8, 3);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 51);
            this.dataGridView.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidth = 51;
            this.dataGridView.RowTemplate.Height = 28;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(1000, 424);
            this.dataGridView.TabIndex = 3;
            this.dataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellEndEdit);
            this.dataGridView.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dataGridView_UserDeletingRow);
            // 
            // colID
            // 
            this.colID.DataPropertyName = "ID";
            this.colID.HeaderText = "Mã số";
            this.colID.MinimumWidth = 6;
            this.colID.Name = "colID";
            this.colID.ReadOnly = true;
            this.colID.ToolTipText = "Mã số do hệ thống cấp, không sửa được. Bấm vào tiêu đề để sắp xếp.";
            this.colID.Width = 90;
            // 
            // colHoTen
            // 
            this.colHoTen.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colHoTen.DataPropertyName = "HO_TEN";
            this.colHoTen.FillWeight = 40F;
            this.colHoTen.HeaderText = "Nhà cung cấp";
            this.colHoTen.MaxInputLength = 100;
            this.colHoTen.MinimumWidth = 160;
            this.colHoTen.Name = "colHoTen";
            this.colHoTen.ToolTipText = "Bấm vào tiêu đề để sắp xếp";
            // 
            // colDiaChi
            // 
            this.colDiaChi.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDiaChi.DataPropertyName = "DIA_CHI";
            this.colDiaChi.FillWeight = 40F;
            this.colDiaChi.HeaderText = "Địa chỉ";
            this.colDiaChi.MaxInputLength = 200;
            this.colDiaChi.MinimumWidth = 160;
            this.colDiaChi.Name = "colDiaChi";
            this.colDiaChi.ToolTipText = "Bấm vào tiêu đề để sắp xếp";
            // 
            // colDienThoai
            // 
            this.colDienThoai.DataPropertyName = "DIEN_THOAI";
            this.colDienThoai.HeaderText = "Điện thoại";
            this.colDienThoai.MaxInputLength = 20;
            this.colDienThoai.MinimumWidth = 6;
            this.colDienThoai.Name = "colDienThoai";
            this.colDienThoai.ToolTipText = "Số điện thoại, ví dụ 0296 3841 234. Bấm vào tiêu đề để sắp xếp.";
            this.colDienThoai.Width = 140;
            // 
            // lblChuaCoDuLieu
            // 
            this.lblChuaCoDuLieu.AutoSize = true;
            this.lblChuaCoDuLieu.BackColor = System.Drawing.Color.Transparent;
            this.lblChuaCoDuLieu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.lblChuaCoDuLieu.ForeColor = System.Drawing.Color.Gray;
            this.lblChuaCoDuLieu.Location = new System.Drawing.Point(32, 64);
            this.lblChuaCoDuLieu.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblChuaCoDuLieu.Name = "lblChuaCoDuLieu";
            this.lblChuaCoDuLieu.Size = new System.Drawing.Size(0, 20);
            this.lblChuaCoDuLieu.TabIndex = 0;
            this.lblChuaCoDuLieu.Visible = false;
            // 
            // bindingNavigator
            // 
            this.bindingNavigator.AddNewItem = null;
            this.bindingNavigator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.bindingNavigator.CountItem = this.bindingNavigatorCountItem;
            this.bindingNavigator.DeleteItem = null;
            this.bindingNavigator.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.bindingNavigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.toolStripSeparator1,
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem,
            this.toolLuu,
            this.toolSua,
            this.toolStripSeparator1,
            this.toolTimKiem,
            this.toolStripSeparator1,
            this.toolThoat,
            this.toolTimNhaCungCap,
            this.toolXoaLoc
            });
            this.bindingNavigator.Location = new System.Drawing.Point(0, 0);
            this.bindingNavigator.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bindingNavigator.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bindingNavigator.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bindingNavigator.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bindingNavigator.Name = "bindingNavigator";
            this.bindingNavigator.PositionItem = this.bindingNavigatorPositionItem;
            this.bindingNavigator.Size = new System.Drawing.Size(1000, 51);
            this.bindingNavigator.TabIndex = 2;
            this.bindingNavigator.Text = "bindingNavigator1";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(45, 48);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(29, 48);
            this.bindingNavigatorMoveFirstItem.Text = "";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(29, 48);
            this.bindingNavigatorMovePreviousItem.Text = "";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 51);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(65, 27);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Current position";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 51);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(29, 48);
            this.bindingNavigatorMoveNextItem.Text = "";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(29, 48);
            this.bindingNavigatorMoveLastItem.Text = "";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 51);
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.Image = global::CuahangNongduoc.Properties.Resources.add;
            this.bindingNavigatorAddNewItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(50, 48);
            this.bindingNavigatorAddNewItem.Text = "Thêm";
            this.bindingNavigatorAddNewItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bindingNavigatorAddNewItem.ToolTipText = "Thêm nhà cung cấp mới";
            this.bindingNavigatorAddNewItem.Click += new System.EventHandler(this.bindingNavigatorAddNewItem_Click);
            // 
            // toolSua
            // 
            this.toolSua.Image = global::CuahangNongduoc.Properties.Resources.edit;
            this.toolSua.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolSua.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolSua.Name = "toolSua";
            this.toolSua.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.toolSua.Size = new System.Drawing.Size(54, 48);
            this.toolSua.Text = "Sửa";
            this.toolSua.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolSua.ToolTipText = "Sửa thông tin nhà cung cấp đang chọn";
            this.toolSua.Click += new System.EventHandler(this.toolSua_Click);
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.Image = global::CuahangNongduoc.Properties.Resources.remove;
            this.bindingNavigatorDeleteItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(55, 48);
            this.bindingNavigatorDeleteItem.Text = "Xóa";
            this.bindingNavigatorDeleteItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bindingNavigatorDeleteItem.ToolTipText = "Xóa nhà cung cấp đang chọn (có hỏi xác nhận)";
            this.bindingNavigatorDeleteItem.Click += new System.EventHandler(this.bindingNavigatorDeleteItem_Click);
            // 
            // toolLuu
            // 
            this.toolLuu.Image = global::CuahangNongduoc.Properties.Resources.save;
            this.toolLuu.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolLuu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolLuu.Name = "toolLuu";
            this.toolLuu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.toolLuu.Size = new System.Drawing.Size(53, 48);
            this.toolLuu.Text = "Lưu";
            this.toolLuu.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolLuu.ToolTipText = "Lưu mọi thay đổi vào cơ sở dữ liệu";
            this.toolLuu.Click += new System.EventHandler(this.toolLuu_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 51);
            // 
            // toolTimKiem
            // 
            this.toolTimKiem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.toolTimKiem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolTimKiem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolTimTatCa,
            this.toolTimHoTen,
            this.toolTimDiaChi,
            this.toolTimDienThoai});
            this.toolTimKiem.Image = global::CuahangNongduoc.Properties.Resources.find;
            this.toolTimKiem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolTimKiem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolTimKiem.Name = "toolTimKiem";
            this.toolTimKiem.Size = new System.Drawing.Size(38, 48);
            this.toolTimKiem.Text = "Tìm kiếm";
            this.toolTimKiem.ToolTipText = "Chọn cột cần tìm (tất cả, nhà cung cấp, địa chỉ, điện thoại)";
            // 
            // toolTimTatCa
            // 
            this.toolTimTatCa.Checked = true;
            this.toolTimTatCa.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolTimTatCa.Name = "toolTimTatCa";
            this.toolTimTatCa.Size = new System.Drawing.Size(183, 26);
            this.toolTimTatCa.Text = "Tất cả các cột";
            this.toolTimTatCa.Click += new System.EventHandler(this.toolTimTatCa_Click);
            // 
            // toolTimHoTen
            // 
            this.toolTimHoTen.Name = "toolTimHoTen";
            this.toolTimHoTen.Size = new System.Drawing.Size(183, 26);
            this.toolTimHoTen.Text = "Nhà cung cấp";
            this.toolTimHoTen.Click += new System.EventHandler(this.toolTimHoTen_Click);
            // 
            // toolTimDiaChi
            // 
            this.toolTimDiaChi.Name = "toolTimDiaChi";
            this.toolTimDiaChi.Size = new System.Drawing.Size(183, 26);
            this.toolTimDiaChi.Text = "Địa chỉ";
            this.toolTimDiaChi.Click += new System.EventHandler(this.toolTimDiaChi_Click);
            // 
            // toolTimDienThoai
            // 
            this.toolTimDienThoai.Name = "toolTimDienThoai";
            this.toolTimDienThoai.Size = new System.Drawing.Size(183, 26);
            this.toolTimDienThoai.Text = "Điện thoại";
            this.toolTimDienThoai.Click += new System.EventHandler(this.toolTimDienThoai_Click);
            // 
            // toolTimNhaCungCap
            // 
            this.toolTimNhaCungCap.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolTimNhaCungCap.ForeColor = System.Drawing.Color.Gray;
            this.toolTimNhaCungCap.MaxLength = 100;
            this.toolTimNhaCungCap.Name = "toolTimNhaCungCap";
            this.toolTimNhaCungCap.Size = new System.Drawing.Size(225, 51);
            this.toolTimNhaCungCap.Text = "Tìm theo tất cả các cột...";
            this.toolTimNhaCungCap.ToolTipText = "Gõ để lọc ngay trong danh sách; bấm Esc để xóa";
            this.toolTimNhaCungCap.Enter += new System.EventHandler(this.toolTimNhaCungCap_Enter);
            this.toolTimNhaCungCap.Leave += new System.EventHandler(this.toolTimNhaCungCap_Leave);
            this.toolTimNhaCungCap.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolTimNhaCungCap_KeyDown);
            this.toolTimNhaCungCap.TextChanged += new System.EventHandler(this.toolTimNhaCungCap_TextChanged);
            // 
            // toolXoaLoc
            // 
            this.toolXoaLoc.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolXoaLoc.Name = "toolXoaLoc";
            this.toolXoaLoc.Size = new System.Drawing.Size(63, 48);
            this.toolXoaLoc.Text = "Xóa lọc";
            this.toolXoaLoc.ToolTipText = "Bỏ bộ lọc, hiện lại toàn bộ danh sách";
            this.toolXoaLoc.Click += new System.EventHandler(this.toolXoaLoc_Click);
            // 
            // toolThoat
            // 
            this.toolThoat.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolThoat.Image = global::CuahangNongduoc.Properties.Resources.Thoat;
            this.toolThoat.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolThoat.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolThoat.Margin = new System.Windows.Forms.Padding(24, 1, 4, 2);
            this.toolThoat.Name = "toolThoat";
            this.toolThoat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.toolThoat.Size = new System.Drawing.Size(55, 48);
            this.toolThoat.Text = "Thoát";
            this.toolThoat.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolThoat.ToolTipText = "Đóng cửa sổ (sẽ hỏi nếu còn thay đổi chưa lưu)";
            this.toolThoat.Click += new System.EventHandler(this.toolThoat_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblTong,
            this.lblThongBao});
            this.statusStrip.Location = new System.Drawing.Point(0, 475);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip.Size = new System.Drawing.Size(1000, 30);
            this.statusStrip.TabIndex = 4;
            // 
            // lblTong
            // 
            this.lblTong.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.lblTong.Name = "lblTong";
            this.lblTong.Size = new System.Drawing.Size(173, 24);
            this.lblTong.Text = "Tổng số: 0 nhà cung cấp";
            // 
            // lblThongBao
            // 
            this.lblThongBao.Name = "lblThongBao";
            this.lblThongBao.Size = new System.Drawing.Size(807, 24);
            this.lblThongBao.Spring = true;
            this.lblThongBao.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmNhaCungCap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 505);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.bindingNavigator);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmNhaCungCap";
            this.Text = "Nhà cung cấp";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmNhaCungCap_FormClosing);
            this.Load += new System.EventHandler(this.frmNhaCungCap_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.dataGridView.ResumeLayout(false);
            this.dataGridView.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator)).EndInit();
            this.bindingNavigator.ResumeLayout(false);
            this.bindingNavigator.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn colID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHoTen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDiaChi;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDienThoai;
        private System.Windows.Forms.BindingNavigator bindingNavigator;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripButton toolLuu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolThoat;
        private System.Windows.Forms.ToolStripDropDownButton toolTimKiem;
        private System.Windows.Forms.ToolStripMenuItem toolTimHoTen;
        private System.Windows.Forms.ToolStripMenuItem toolTimDiaChi;
        private System.Windows.Forms.ToolStripTextBox toolTimNhaCungCap;
        private System.Windows.Forms.ToolStripButton toolSua;
        private System.Windows.Forms.ToolStripButton toolXoaLoc;
        private System.Windows.Forms.ToolStripMenuItem toolTimTatCa;
        private System.Windows.Forms.ToolStripMenuItem toolTimDienThoai;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblTong;
        private System.Windows.Forms.ToolStripStatusLabel lblThongBao;
        private System.Windows.Forms.Label lblChuaCoDuLieu;
    }
}