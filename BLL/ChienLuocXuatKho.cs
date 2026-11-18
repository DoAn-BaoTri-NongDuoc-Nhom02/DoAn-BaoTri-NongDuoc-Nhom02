using System;
using System.Collections.Generic;

namespace CuahangNongduoc.Controller
{
    /// <summary>
    /// Cách chọn lô khi xuất hàng.
    /// REFACTOR #31 Replace Type Code with State/Strategy – trước đây form bán hàng tự so sánh
    /// CauHinhKho.PhuongPhapXuatKho == ChiDinh (4 chỗ) rồi if/else gọi PhanLoChiDinh hay PhanLoFIFO.
    /// Nay mỗi phương pháp là một lớp; form chỉ hỏi chiến lược hiện hành.
    /// </summary>
    public abstract class ChienLuocXuatKho
    {
        /// <summary>Người bán có phải tự chọn lô không.</summary>
        public abstract bool ChoChonLo { get; }

        /// <summary>Phân số lượng cần xuất vào các lô.</summary>
        public abstract IList<PhanBoLo> PhanLo(XuatKhoController xuatKho, String idSanPham, String idLoDaChon,
            int soLuong, IDictionary<String, int> daGiu);

        /// <summary>Chiến lược theo cấu hình cửa hàng (bảng CAU_HINH).</summary>
        public static ChienLuocXuatKho TheoCauHinh()
        {
            if (CauHinhKho.PhuongPhapXuatKho == PhuongPhapXuatKho.ChiDinh)
                return new XuatKhoChiDinh();
            return new XuatKhoFIFO();
        }
    }

    /// <summary>Hệ thống tự phân lô, lô có hạn dùng gần nhất xuất trước.</summary>
    public class XuatKhoFIFO : ChienLuocXuatKho
    {
        public override bool ChoChonLo
        {
            get { return false; }
        }

        public override IList<PhanBoLo> PhanLo(XuatKhoController xuatKho, String idSanPham, String idLoDaChon,
            int soLuong, IDictionary<String, int> daGiu)
        {
            return xuatKho.PhanLoFIFO(idSanPham, soLuong, daGiu);
        }
    }

    /// <summary>Xuất đúng lô người bán đã chọn.</summary>
    public class XuatKhoChiDinh : ChienLuocXuatKho
    {
        public override bool ChoChonLo
        {
            get { return true; }
        }

        public override IList<PhanBoLo> PhanLo(XuatKhoController xuatKho, String idSanPham, String idLoDaChon,
            int soLuong, IDictionary<String, int> daGiu)
        {
            return xuatKho.PhanLoChiDinh(idLoDaChon, soLuong, daGiu);
        }
    }
}
