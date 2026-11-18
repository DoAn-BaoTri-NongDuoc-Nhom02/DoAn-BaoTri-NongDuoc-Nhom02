using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using CuahangNongduoc.DataLayer;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.Controller
{
    public class MaSanPhamController
    {
        MaSanPhamFactory factory = new MaSanPhamFactory();

        public DataRow NewRow()
        {
            return factory.NewRow();
        }
        public void Add(DataRow row)
        {
            factory.Add(row);
        }
        public bool Save()
        {
            return factory.Save();
        }

        /// <summary>Toan bo bang MA_SAN_PHAM de form tu loc theo phieu nhap va sua tren man hinh.</summary>
        public DataTable TaiBangChiTiet()
        {
            return factory.TaiBangChiTiet();
        }

        public String LastError
        {
            get { return factory.LastError; }
        }

        public bool CoThayDoi()
        {
            return factory.CoThayDoi();
        }

        public SanPham LaySanPham(String idMaSanPham)
        {
            MaSanPhamFactory f = new MaSanPhamFactory();
            DataTable tbl = f.LaySanPham(idMaSanPham);
            SanPham sp = null;
            DonViTinhController ctrlDVT = new DonViTinhController();
            if (tbl.Rows.Count > 0)
            {
                sp = new SanPham();
                sp.Id = Convert.ToString(tbl.Rows[0]["ID"]);
                sp.TenSanPham = Convert.ToString(tbl.Rows[0]["TEN_SAN_PHAM"]);
                sp.SoLuong = Convert.ToInt32(tbl.Rows[0]["SO_LUONG"]);
                sp.DonGiaNhap = Convert.ToInt64(tbl.Rows[0]["DON_GIA_NHAP"]);
                sp.GiaBanLe = Convert.ToInt64(tbl.Rows[0]["GIA_BAN_LE"]);
                sp.GiaBanSi = Convert.ToInt64(tbl.Rows[0]["GIA_BAN_SI"]);
                // PATTERN #11 Flyweight (nơi dùng) – đơn vị tính lấy từ kho dùng chung, không tạo đối tượng mới mỗi dòng.
                sp.DonViTinh = ctrlDVT.LayDVT(Convert.ToInt32(tbl.Rows[0]["ID_DON_VI_TINH"]));
            }
            return sp;

        }


        public MaSanPham LayMaSanPham(String idMaSanPham)
        {
            MaSanPhamFactory f = new MaSanPhamFactory();
            DataTable tbl = f.LayMaSanPham(idMaSanPham);
            MaSanPham sp = null;
            SanPhamController ctrlSanPham = new SanPhamController();
            if (tbl.Rows.Count > 0)
            {
                sp = MaSanPham.TuDongDuLieu(tbl.Rows[0]);
                sp.SanPham = ctrlSanPham.LaySanPham(tbl.Rows[0]["ID_SAN_PHAM"].ToString());

            }
            return sp;

        }

        public static IList<MaSanPham> LayMaSanPhamHetHan(DateTime dt)
        {
            IList<MaSanPham> ds = new List<MaSanPham>();
            MaSanPhamFactory f = new MaSanPhamFactory();
            DataTable tbl = f.DanhsachMaSanPhamHetHan(dt);

            MaSanPham sp = null;
            SanPhamController ctrlSanPham = new SanPhamController();
            foreach (DataRow row in tbl.Rows)
            {
                sp = MaSanPham.TuDongDuLieu(row);
                sp.SanPham = ctrlSanPham.LaySanPham(row["ID_SAN_PHAM"].ToString());
                if (sp.SanPham != null)
                    sp.TenSanPham = sp.SanPham.TenSanPham ?? "";
                else
                    sp.TenSanPham = "";
                ds.Add(sp);
            }
            return ds;

        }

        public void HienThiAutoComboBox(String sp, ComboBox cmb)
        {
            cmb.DataSource = factory.DanhsachMaSanPham(sp);
            cmb.DisplayMember = "ID";
            cmb.ValueMember = "ID";
        }

        public void HienThiDataGridViewComboBox(DataGridViewComboBoxColumn cmb)
        {
            // Lấy cả lô đã hết hàng để các phiếu bán cũ vẫn hiện đúng mã lô.
            cmb.DataSource = factory.DanhsachTatCaMaSanPham();
            cmb.DisplayMember = "ID";
            cmb.ValueMember = "ID";
            cmb.DataPropertyName = "ID_MA_SAN_PHAM";
            cmb.HeaderText = "Mã sản phẩm";
        }

        public void HienThiChiTietPhieuNhap(String id, DataGridView dg)
        {

            dg.DataSource = factory.DanhsachChiTiet(id);
        }
        public IList<MaSanPham> ChiTietPhieuNhap(String id)
        {
            SanPhamController ctrlSanPham = new SanPhamController();
            IList<MaSanPham> ds = new List<MaSanPham>();
            DataTable tbl = factory.DanhsachChiTiet(id);
            foreach (DataRow row in tbl.Rows)
            {
                MaSanPham sp = MaSanPham.TuDongDuLieu(row);
                sp.ThanhTien = sp.SoLuong * sp.GiaNhap;
                sp.SanPham = ctrlSanPham.LaySanPham(row["ID_SAN_PHAM"].ToString());
                if (sp.SanPham != null)
                    sp.TenSanPham = sp.SanPham.TenSanPham ?? "";
                else
                    sp.TenSanPham = "";
                ds.Add(sp);
            }
            return ds;
        }

    }
}