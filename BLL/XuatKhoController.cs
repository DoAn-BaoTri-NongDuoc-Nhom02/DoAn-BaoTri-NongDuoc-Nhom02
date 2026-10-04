using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc
{
    /// <summary>Lỗi nghiệp vụ kho: không đủ hàng, lô không tồn tại, sẽ làm tồn kho âm...</summary>
    public class TonKhoException : Exception
    {
        public TonKhoException(String message) : base(message) { }
    }
}

namespace CuahangNongduoc.Controller
{
    /// <summary>Một phần của lần xuất: lấy "SoLuong" từ lô "Lo".</summary>
    public class PhanBoLo
    {
        private MaSanPham m_Lo;
        private int m_SoLuong;

        public PhanBoLo(MaSanPham lo, int soLuong)
        {
            m_Lo = lo;
            m_SoLuong = soLuong;
        }

        public MaSanPham Lo
        {
            get { return m_Lo; }
        }

        public int SoLuong
        {
            get { return m_SoLuong; }
        }
    }

    /// <summary>
    /// Nghiệp vụ xuất kho: chọn lô theo cấu hình (FIFO / chỉ định), tính giá xuất,
    /// và chặn xuất vượt tồn.
    /// </summary>
    public class XuatKhoController
    {
        MaSanPhanFactory factory = new MaSanPhanFactory();

        /// <summary>Các lô còn hàng của sản phẩm, lô có hạn dùng gần nhất đứng đầu.</summary>
        public IList<MaSanPham> DanhSachLoConHang(String idSanPham)
        {
            List<MaSanPham> ds = new List<MaSanPham>();
            DataTable tbl = factory.DanhsachMaSanPham(idSanPham);
            foreach (DataRow row in tbl.Rows)
            {
                MaSanPham lo = new MaSanPham();
                lo.Id = Convert.ToString(row["ID"]);
                lo.SoLuong = Convert.ToInt32(row["SO_LUONG"]);
                lo.GiaNhap = row["DON_GIA_NHAP"] == DBNull.Value ? 0 : Convert.ToInt64(row["DON_GIA_NHAP"]);
                lo.NgayNhap = row["NGAY_NHAP"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["NGAY_NHAP"]);
                lo.NgaySanXuat = row["NGAY_SAN_XUAT"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["NGAY_SAN_XUAT"]);
                // Lô không có hạn dùng thì xếp cuối.
                lo.NgayHetHan = row["NGAY_HET_HAN"] == DBNull.Value ? DateTime.MaxValue : Convert.ToDateTime(row["NGAY_HET_HAN"]);
                ds.Add(lo);
            }

            ds.Sort(delegate(MaSanPham a, MaSanPham b)
            {
                int c = a.NgayHetHan.CompareTo(b.NgayHetHan);
                if (c == 0) c = a.NgayNhap.CompareTo(b.NgayNhap);
                if (c == 0) c = String.Compare(a.Id, b.Id, StringComparison.Ordinal);
                return c;
            });
            return ds;
        }

        /// <summary>
        /// Phân "soLuong" của sản phẩm vào các lô theo hạn dùng gần nhất (FIFO).
        /// daGiu: số lượng từng lô đã nằm trong phiếu đang soạn nhưng chưa lưu (khóa = mã lô), có thể null.
        /// Không đủ hàng -> ném TonKhoException, không phân bổ gì cả.
        /// </summary>
        public IList<PhanBoLo> PhanLoFIFO(String idSanPham, int soLuong, IDictionary<String, int> daGiu)
        {
            if (soLuong <= 0)
                throw new TonKhoException("Số lượng xuất phải lớn hơn 0.");

            IList<MaSanPham> los = DanhSachLoConHang(idSanPham);
            List<PhanBoLo> ketQua = new List<PhanBoLo>();
            int conThieu = soLuong;
            int tongKhaDung = 0;

            foreach (MaSanPham lo in los)
            {
                int kha = lo.SoLuong - DaGiu(daGiu, lo.Id);
                if (kha <= 0)
                    continue;
                tongKhaDung += kha;
                if (conThieu > 0)
                {
                    int lay = Math.Min(kha, conThieu);
                    ketQua.Add(new PhanBoLo(lo, lay));
                    conThieu -= lay;
                }
            }

            if (conThieu > 0)
            {
                throw new TonKhoException("Không đủ hàng trong kho: cần " + soLuong +
                    ", chỉ còn " + tongKhaDung + " (đã trừ số lượng trong phiếu đang soạn).");
            }
            return ketQua;
        }

        /// <summary>
        /// Xuất đúng lô người bán đã chọn. Lô phải đủ số lượng (sau khi trừ phần đã nằm trong phiếu đang soạn).
        /// </summary>
        public IList<PhanBoLo> PhanLoChiDinh(String idMaSanPham, int soLuong, IDictionary<String, int> daGiu)
        {
            if (soLuong <= 0)
                throw new TonKhoException("Số lượng xuất phải lớn hơn 0.");

            MaSanPhamController ctrl = new MaSanPhamController();
            MaSanPham lo = ctrl.LayMaSanPham(idMaSanPham);
            if (lo == null)
                throw new TonKhoException("Lô '" + idMaSanPham + "' không tồn tại.");

            int kha = lo.SoLuong - DaGiu(daGiu, lo.Id);
            if (kha < soLuong)
            {
                throw new TonKhoException("Lô '" + lo.Id + "' không đủ hàng: cần " + soLuong +
                    ", chỉ còn " + Math.Max(kha, 0) + ".");
            }

            List<PhanBoLo> ketQua = new List<PhanBoLo>();
            ketQua.Add(new PhanBoLo(lo, soLuong));
            return ketQua;
        }

        /// <summary>
        /// Giá xuất (giá vốn) của một đơn vị hàng lấy từ lô "lo", theo cấu hình:
        /// bình quân gia quyền = SAN_PHAM.DON_GIA_NHAP; FIFO = giá nhập của chính lô đó.
        /// </summary>
        public long TinhGiaXuat(MaSanPham lo)
        {
            if (lo == null)
                return 0;
            if (CauHinhKho.PhuongPhapGiaXuat == PhuongPhapGiaXuat.FIFO)
                return lo.GiaNhap;
            return lo.SanPham != null ? lo.SanPham.DonGiaNhap : lo.GiaNhap;
        }

        static int DaGiu(IDictionary<String, int> daGiu, String idLo)
        {
            int v;
            if (daGiu != null && daGiu.TryGetValue(idLo, out v))
                return v;
            return 0;
        }
    }
}
