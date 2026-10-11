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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNhaCungCap));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.colID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHoTen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDiaChi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDienThoai = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.toolHuy = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolLoc = new System.Windows.Forms.ToolStripButton();
            this.toolTimKiem = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolTimHoTen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTimDiaChi = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTimNhaCungCap = new System.Windows.Forms.ToolStripTextBox();
            this.toolThoat = new System.Windows.Forms.ToolStripButton();
            this.pnlLoc = new System.Windows.Forms.Panel();
            this.lblLocTen = new System.Windows.Forms.Label();
            this.txtLocTen = new System.Windows.Forms.TextBox();
            this.lblLocDiaChi = new System.Windows.Forms.Label();
            this.txtLocDiaChi = new System.Windows.Forms.TextBox();
            this.lblLocDienThoai = new System.Windows.Forms.Label();
            this.txtLocDienThoai = new System.Windows.Forms.TextBox();
            this.btnLoc = new System.Windows.Forms.Button();
            this.btnBoLoc = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblTongSo = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator)).BeginInit();
            this.bindingNavigator.SuspendLayout();
            this.pnlLoc.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(248)))), ((int)(((byte)(245)))));
            this.dataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colID,
            this.colHoTen,
            this.colDiaChi,
            this.colDienThoai});
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 86);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersWidth = 28;
            this.dataGridView.RowTemplate.Height = 28;
            this.dataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.dataGridView.Size = new System.Drawing.Size(860, 386);
            this.dataGridView.TabIndex = 3;
            this.dataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView_CellValidating);
            this.dataGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView_EditingControlShowing);
            // 
            // colID
            // 
            this.colID.DataPropertyName = "ID";
            this.colID.HeaderText = "Mã số";
            this.colID.MinimumWidth = 70;
            this.colID.Name = "colID";
            this.colID.ReadOnly = true;
            this.colID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colID.Width = 90;
            // 
            // colHoTen
            // 
            this.colHoTen.DataPropertyName = "HO_TEN";
            this.colHoTen.HeaderText = "Nhà cung cấp";
            this.colHoTen.MinimumWidth = 140;
            this.colHoTen.Name = "colHoTen";
            this.colHoTen.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colHoTen.Width = 240;
            // 
            // colDiaChi
            // 
            this.colDiaChi.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDiaChi.DataPropertyName = "DIA_CHI";
            this.colDiaChi.HeaderText = "Địa chỉ";
            this.colDiaChi.MinimumWidth = 220;
            this.colDiaChi.Name = "colDiaChi";
            this.colDiaChi.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colDienThoai
            // 
            this.colDienThoai.DataPropertyName = "DIEN_THOAI";
            this.colDienThoai.HeaderText = "Điện thoại";
            this.colDienThoai.MinimumWidth = 110;
            this.colDienThoai.Name = "colDienThoai";
            this.colDienThoai.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colDienThoai.Width = 140;
            // 
            // bindingNavigator
            // 
            this.bindingNavigator.AddNewItem = null;
            this.bindingNavigator.CountItem = this.bindingNavigatorCountItem;
            this.bindingNavigator.CountItemFormat = "/ {0}";
            this.bindingNavigator.DeleteItem = null;
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
            this.bindingNavigatorAddNewItem,
            this.toolSua,
            this.bindingNavigatorDeleteItem,
            this.toolLuu,
            this.toolHuy,
            this.toolStripSeparator1,
            this.toolLoc,
            this.toolTimKiem,
            this.toolTimNhaCungCap,
            this.toolThoat});
            this.bindingNavigator.Location = new System.Drawing.Point(0, 0);
            this.bindingNavigator.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bindingNavigator.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bindingNavigator.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bindingNavigator.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bindingNavigator.Name = "bindingNavigator";
            this.bindingNavigator.PositionItem = this.bindingNavigatorPositionItem;
            this.bindingNavigator.ShowItemToolTips = true;
            this.bindingNavigator.Size = new System.Drawing.Size(860, 46);
            this.bindingNavigator.TabIndex = 2;
            this.bindingNavigator.Text = "bindingNavigator1";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(35, 43);
            this.bindingNavigatorCountItem.Text = "/ {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Tổng số dòng đang hiển thị";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 43);
            this.bindingNavigatorMoveFirstItem.Text = "Về dòng đầu";
            this.bindingNavigatorMoveFirstItem.ToolTipText = "Về dòng đầu";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 43);
            this.bindingNavigatorMovePreviousItem.Text = "Dòng trước";
            this.bindingNavigatorMovePreviousItem.ToolTipText = "Dòng trước";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 46);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 23);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Vị trí dòng hiện tại";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 46);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 43);
            this.bindingNavigatorMoveNextItem.Text = "Dòng sau";
            this.bindingNavigatorMoveNextItem.ToolTipText = "Dòng sau";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 43);
            this.bindingNavigatorMoveLastItem.Text = "Về dòng cuối";
            this.bindingNavigatorMoveLastItem.ToolTipText = "Về dòng cuối";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 46);
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.Image = global::CuahangNongduoc.Properties.Resources.add;
            this.bindingNavigatorAddNewItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(42, 43);
            this.bindingNavigatorAddNewItem.Text = "Thêm";
            this.bindingNavigatorAddNewItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bindingNavigatorAddNewItem.ToolTipText = "Thêm nhà cung cấp mới (nhập thông tin rồi bấm Lưu)";
            this.bindingNavigatorAddNewItem.Click += new System.EventHandler(this.bindingNavigatorAddNewItem_Click);
            // 
            // toolSua
            // 
            this.toolSua.Image = global::CuahangNongduoc.Properties.Resources.edit;
            this.toolSua.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolSua.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolSua.Name = "toolSua";
            this.toolSua.Size = new System.Drawing.Size(36, 43);
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
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(43, 43);
            this.bindingNavigatorDeleteItem.Text = "Xóa";
            this.bindingNavigatorDeleteItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bindingNavigatorDeleteItem.ToolTipText = "Xóa nhà cung cấp đang chọn";
            this.bindingNavigatorDeleteItem.Click += new System.EventHandler(this.bindingNavigatorDeleteItem_Click);
            // 
            // toolLuu
            // 
            this.toolLuu.Image = global::CuahangNongduoc.Properties.Resources.save;
            this.toolLuu.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolLuu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolLuu.Name = "toolLuu";
            this.toolLuu.Size = new System.Drawing.Size(43, 43);
            this.toolLuu.Text = "Lưu";
            this.toolLuu.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolLuu.ToolTipText = "Lưu các thay đổi vào cơ sở dữ liệu";
            this.toolLuu.Click += new System.EventHandler(this.toolLuu_Click);
            // 
            // toolHuy
            // 
            this.toolHuy.Image = global::CuahangNongduoc.Properties.Resources.undo;
            this.toolHuy.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolHuy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolHuy.Name = "toolHuy";
            this.toolHuy.Size = new System.Drawing.Size(36, 43);
            this.toolHuy.Text = "Hủy";
            this.toolHuy.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolHuy.ToolTipText = "Hủy các thay đổi chưa lưu";
            this.toolHuy.Click += new System.EventHandler(this.toolHuy_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 46);
            // 
            // toolLoc
            // 
            this.toolLoc.CheckOnClick = true;
            this.toolLoc.Image = global::CuahangNongduoc.Properties.Resources.down;
            this.toolLoc.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolLoc.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolLoc.Name = "toolLoc";
            this.toolLoc.Size = new System.Drawing.Size(36, 43);
            this.toolLoc.Text = "Lọc";
            this.toolLoc.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolLoc.ToolTipText = "Hiện / ẩn bộ lọc nâng cao (theo tên, địa chỉ, điện thoại)";
            this.toolLoc.Click += new System.EventHandler(this.toolLoc_Click);
            // 
            // toolTimKiem
            // 
            this.toolTimKiem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolTimKiem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolTimHoTen,
            this.toolTimDiaChi});
            this.toolTimKiem.Image = global::CuahangNongduoc.Properties.Resources.find;
            this.toolTimKiem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolTimKiem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolTimKiem.Name = "toolTimKiem";
            this.toolTimKiem.Size = new System.Drawing.Size(37, 43);
            this.toolTimKiem.Text = "Tìm kiếm";
            this.toolTimKiem.ToolTipText = "Chọn tiêu chí tìm kiếm (Nhà cung cấp / Địa chỉ)";
            // 
            // toolTimHoTen
            // 
            this.toolTimHoTen.Checked = true;
            this.toolTimHoTen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolTimHoTen.Name = "toolTimHoTen";
            this.toolTimHoTen.Size = new System.Drawing.Size(152, 22);
            this.toolTimHoTen.Text = "Nhà cung cấp";
            this.toolTimHoTen.Click += new System.EventHandler(this.toolTimHoTen_Click);
            // 
            // toolTimDiaChi
            // 
            this.toolTimDiaChi.Name = "toolTimDiaChi";
            this.toolTimDiaChi.Size = new System.Drawing.Size(152, 22);
            this.toolTimDiaChi.Text = "Địa chỉ";
            this.toolTimDiaChi.Click += new System.EventHandler(this.toolTimDiaChi_Click);
            // 
            // toolTimNhaCungCap
            // 
            this.toolTimNhaCungCap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toolTimNhaCungCap.Enabled = true;
            this.toolTimNhaCungCap.ForeColor = System.Drawing.SystemColors.GrayText;
            this.toolTimNhaCungCap.Name = "toolTimNhaCungCap";
            this.toolTimNhaCungCap.Size = new System.Drawing.Size(170, 46);
            this.toolTimNhaCungCap.Text = "Tìm theo Nhà cung cấp";
            this.toolTimNhaCungCap.ToolTipText = "Nhập từ khóa rồi nhấn Enter để tìm (để trống rồi Enter để hiện lại tất cả)";
            this.toolTimNhaCungCap.Leave += new System.EventHandler(this.toolTimNhaCungCap_Leave);
            this.toolTimNhaCungCap.Enter += new System.EventHandler(this.toolTimNhaCungCap_Enter);
            this.toolTimNhaCungCap.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.toolTimNhaCungCap_KeyPress);
            // 
            // toolThoat
            // 
            this.toolThoat.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolThoat.Image = global::CuahangNongduoc.Properties.Resources.stop;
            this.toolThoat.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolThoat.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolThoat.Name = "toolThoat";
            this.toolThoat.Size = new System.Drawing.Size(45, 43);
            this.toolThoat.Text = "Thoát";
            this.toolThoat.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolThoat.ToolTipText = "Đóng cửa sổ (sẽ hỏi nếu còn thay đổi chưa lưu)";
            this.toolThoat.Click += new System.EventHandler(this.toolThoat_Click);
            // 
            // pnlLoc
            // 
            this.pnlLoc.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pnlLoc.Controls.Add(this.lblLocTen);
            this.pnlLoc.Controls.Add(this.txtLocTen);
            this.pnlLoc.Controls.Add(this.lblLocDiaChi);
            this.pnlLoc.Controls.Add(this.txtLocDiaChi);
            this.pnlLoc.Controls.Add(this.lblLocDienThoai);
            this.pnlLoc.Controls.Add(this.txtLocDienThoai);
            this.pnlLoc.Controls.Add(this.btnLoc);
            this.pnlLoc.Controls.Add(this.btnBoLoc);
            this.pnlLoc.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLoc.Location = new System.Drawing.Point(0, 46);
            this.pnlLoc.Name = "pnlLoc";
            this.pnlLoc.Size = new System.Drawing.Size(860, 40);
            this.pnlLoc.TabIndex = 4;
            this.pnlLoc.Visible = false;
            // 
            // lblLocTen
            // 
            this.lblLocTen.AutoSize = true;
            this.lblLocTen.Location = new System.Drawing.Point(10, 14);
            this.lblLocTen.Name = "lblLocTen";
            this.lblLocTen.Size = new System.Drawing.Size(75, 13);
            this.lblLocTen.TabIndex = 0;
            this.lblLocTen.Text = "Nhà cung cấp:";
            // 
            // txtLocTen
            // 
            this.txtLocTen.Location = new System.Drawing.Point(92, 10);
            this.txtLocTen.Name = "txtLocTen";
            this.txtLocTen.Size = new System.Drawing.Size(140, 20);
            this.txtLocTen.TabIndex = 1;
            this.txtLocTen.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtLoc_KeyPress);
            // 
            // lblLocDiaChi
            // 
            this.lblLocDiaChi.AutoSize = true;
            this.lblLocDiaChi.Location = new System.Drawing.Point(246, 14);
            this.lblLocDiaChi.Name = "lblLocDiaChi";
            this.lblLocDiaChi.Size = new System.Drawing.Size(43, 13);
            this.lblLocDiaChi.TabIndex = 2;
            this.lblLocDiaChi.Text = "Địa chỉ:";
            // 
            // txtLocDiaChi
            // 
            this.txtLocDiaChi.Location = new System.Drawing.Point(296, 10);
            this.txtLocDiaChi.Name = "txtLocDiaChi";
            this.txtLocDiaChi.Size = new System.Drawing.Size(150, 20);
            this.txtLocDiaChi.TabIndex = 3;
            this.txtLocDiaChi.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtLoc_KeyPress);
            // 
            // lblLocDienThoai
            // 
            this.lblLocDienThoai.AutoSize = true;
            this.lblLocDienThoai.Location = new System.Drawing.Point(460, 14);
            this.lblLocDienThoai.Name = "lblLocDienThoai";
            this.lblLocDienThoai.Size = new System.Drawing.Size(60, 13);
            this.lblLocDienThoai.TabIndex = 4;
            this.lblLocDienThoai.Text = "Điện thoại:";
            // 
            // txtLocDienThoai
            // 
            this.txtLocDienThoai.Location = new System.Drawing.Point(526, 10);
            this.txtLocDienThoai.Name = "txtLocDienThoai";
            this.txtLocDienThoai.Size = new System.Drawing.Size(110, 20);
            this.txtLocDienThoai.TabIndex = 5;
            this.txtLocDienThoai.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtLoc_KeyPress);
            // 
            // btnLoc
            // 
            this.btnLoc.Location = new System.Drawing.Point(652, 8);
            this.btnLoc.Name = "btnLoc";
            this.btnLoc.Size = new System.Drawing.Size(70, 24);
            this.btnLoc.TabIndex = 6;
            this.btnLoc.Text = "Lọc";
            this.btnLoc.UseVisualStyleBackColor = true;
            this.btnLoc.Click += new System.EventHandler(this.btnLoc_Click);
            // 
            // btnBoLoc
            // 
            this.btnBoLoc.Location = new System.Drawing.Point(728, 8);
            this.btnBoLoc.Name = "btnBoLoc";
            this.btnBoLoc.Size = new System.Drawing.Size(80, 24);
            this.btnBoLoc.TabIndex = 7;
            this.btnBoLoc.Text = "Bỏ lọc";
            this.btnBoLoc.UseVisualStyleBackColor = true;
            this.btnBoLoc.Click += new System.EventHandler(this.btnBoLoc_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblTongSo});
            this.statusStrip.Location = new System.Drawing.Point(0, 472);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(860, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 5;
            // 
            // lblTongSo
            // 
            this.lblTongSo.Name = "lblTongSo";
            this.lblTongSo.Size = new System.Drawing.Size(140, 17);
            this.lblTongSo.Text = "Tổng số nhà cung cấp: 0";
            // 
            // frmNhaCungCap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 494);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.pnlLoc);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.bindingNavigator);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(780, 400);
            this.Name = "frmNhaCungCap";
            this.Text = "Nha Cung Cap";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmNhaCungCap_FormClosing);
            this.Load += new System.EventHandler(this.frmNhaCungCap_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator)).EndInit();
            this.bindingNavigator.ResumeLayout(false);
            this.bindingNavigator.PerformLayout();
            this.pnlLoc.ResumeLayout(false);
            this.pnlLoc.PerformLayout();
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
        private System.Windows.Forms.ToolStripButton toolSua;
        private System.Windows.Forms.ToolStripButton toolLuu;
        private System.Windows.Forms.ToolStripButton toolHuy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolLoc;
        private System.Windows.Forms.ToolStripButton toolThoat;
        private System.Windows.Forms.ToolStripDropDownButton toolTimKiem;
        private System.Windows.Forms.ToolStripMenuItem toolTimHoTen;
        private System.Windows.Forms.ToolStripMenuItem toolTimDiaChi;
        private System.Windows.Forms.ToolStripTextBox toolTimNhaCungCap;
        private System.Windows.Forms.Panel pnlLoc;
        private System.Windows.Forms.Label lblLocTen;
        private System.Windows.Forms.TextBox txtLocTen;
        private System.Windows.Forms.Label lblLocDiaChi;
        private System.Windows.Forms.TextBox txtLocDiaChi;
        private System.Windows.Forms.Label lblLocDienThoai;
        private System.Windows.Forms.TextBox txtLocDienThoai;
        private System.Windows.Forms.Button btnLoc;
        private System.Windows.Forms.Button btnBoLoc;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblTongSo;
    }
}