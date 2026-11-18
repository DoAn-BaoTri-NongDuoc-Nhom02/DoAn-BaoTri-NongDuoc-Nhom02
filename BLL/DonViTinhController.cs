using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;


namespace CuahangNongduoc.Controller
{
    public class DonViTinhController
    {
        DonViTinhFactory factory = new DonViTinhFactory();

        public void HienthiAutoComboBox(System.Windows.Forms.ComboBox cmb)
        {
            DanhMucHelper.GanComboBox(cmb, factory);
        }
        public System.Windows.Forms.DataGridViewComboBoxColumn HienthiDataGridViewComboBoxColumn()
        {
            System.Windows.Forms.DataGridViewComboBoxColumn cmb = new System.Windows.Forms.DataGridViewComboBoxColumn();
            // REFACTOR #4 Inline Temp – bỏ biến tạm "tbl" chỉ dùng đúng một lần, gán thẳng kết quả cho DataSource.
            cmb.DataSource = factory.DanhsachDVT();
            cmb.DisplayMember = "TEN_DON_VI";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_DON_VI_TINH";
            cmb.HeaderText = "Đơn vị tính";
            return cmb;
        }
        public void HienthiDataGridview(System.Windows.Forms.DataGridView dg, System.Windows.Forms.BindingNavigator bn)
        {
            System.Windows.Forms.BindingSource bs = new System.Windows.Forms.BindingSource();
            bs.DataSource = factory.DanhsachDVT();
            bn.BindingSource = bs;
            dg.DataSource = bs;
            
        }
        
        // PATTERN #11 Flyweight – mỗi đơn vị tính (Chai, Gói, Kg...) chỉ có MỘT đối tượng DonViTinh dùng chung
        // cho mọi sản phẩm / lô hàng. Trước đây mỗi lần nạp một dòng sản phẩm lại truy vấn CSDL và tạo
        // đối tượng mới (danh sách 1.000 lô = 1.000 truy vấn + 1.000 đối tượng giống hệt nhau).
        // DonViTinh là đối tượng giá trị bất biến (REFACTOR #21) nên dùng chung an toàn.
        private static readonly Dictionary<int, DonViTinh> s_DungChung = new Dictionary<int, DonViTinh>();

        public DonViTinh LayDVT(int id)
        {
            DonViTinh dvt;
            if (s_DungChung.TryGetValue(id, out dvt))
                return dvt;

            DataTable tbl = factory.LayDVT(id);
            if (tbl.Rows.Count == 0)
                return null;
            dvt = new DonViTinh(Convert.ToInt32(tbl.Rows[0]["ID"]), Convert.ToString(tbl.Rows[0]["TEN_DON_VI"]));
            s_DungChung[id] = dvt;
            return dvt;
        }

        /// <summary>Số đơn vị tính đang được dùng chung (để kiểm tra Flyweight).</summary>
        public static int SoDoiTuongDungChung
        {
            get { return s_DungChung.Count; }
        }

        public bool Save()
        {
            s_DungChung.Clear();   // danh mục đổi: bỏ các đối tượng dùng chung cũ
            return factory.Save();
        }
    }
}
