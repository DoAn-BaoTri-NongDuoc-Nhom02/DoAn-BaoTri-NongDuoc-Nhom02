using System;
using System.Windows.Forms;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc.Controller
{
    /// <summary>Gắn một danh mục bất kỳ (IDanhMucFactory) vào ComboBox.</summary>
    public static class DanhMucHelper
    {
        public static void GanComboBox(ComboBox cmb, IDanhMucFactory danhMuc)
        {
            cmb.DataSource = danhMuc.DanhSach();
            cmb.DisplayMember = danhMuc.CotHienThi;
            cmb.ValueMember = danhMuc.CotMa;
        }
    }
}
