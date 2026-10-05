using CuahangNongduoc.DAL.DataAccess;
using CuahangNongduoc.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CuahangNongduoc.DAL.Repositories
{
    public class NguoiDungRepository
    {
        public NguoiDung DangNhap(string tenDangNhap, string matKhau)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT nd.ID, nd.TEN_DANG_NHAP, nd.HO_TEN, nd.ID_VAI_TRO, 
                                      vt.TEN_VAI_TRO, nd.TRANG_THAI
                               FROM NGUOI_DUNG nd
                               JOIN VAI_TRO vt ON nd.ID_VAI_TRO = vt.ID
WHERE nd.TEN_DANG_NHAP = @user AND nd.MAT_KHAU = @pass 
                                     AND nd.TRANG_THAI = 1";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@user", tenDangNhap);
                    cmd.Parameters.AddWithValue("@pass", matKhau);

                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            return new NguoiDung
                            {
                                ID = Convert.ToInt32(rd["ID"]),
                                TenDangNhap = rd["TEN_DANG_NHAP"].ToString(),
                                HoTen = rd["HO_TEN"].ToString(),
                                IDVaiTro = Convert.ToInt32(rd["ID_VAI_TRO"]),
                                TenVaiTro = rd["TEN_VAI_TRO"].ToString(),
                                TrangThai = Convert.ToBoolean(rd["TRANG_THAI"])
                            };
                        }
                    }
                }
            }
            return null; // đăng nhập thất bại
        }
        public List<NguoiDung> GetAll()
        {
            var list = new List<NguoiDung>();
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT nd.ID, nd.TEN_DANG_NHAP, nd.MAT_KHAU, nd.HO_TEN, 
                              nd.ID_VAI_TRO, vt.TEN_VAI_TRO, nd.TRANG_THAI
                       FROM NGUOI_DUNG nd
                       JOIN VAI_TRO vt ON nd.ID_VAI_TRO = vt.ID
                       ORDER BY nd.ID";
                using (var cmd = new SqlCommand(sql, conn))
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new NguoiDung
                        {
                            ID = Convert.ToInt32(rd["ID"]),
                            TenDangNhap = rd["TEN_DANG_NHAP"].ToString(),
                            MatKhau = rd["MAT_KHAU"].ToString(),
                            HoTen = rd["HO_TEN"].ToString(),
                            IDVaiTro = Convert.ToInt32(rd["ID_VAI_TRO"]),
                            TenVaiTro = rd["TEN_VAI_TRO"].ToString(),
                            TrangThai = Convert.ToBoolean(rd["TRANG_THAI"])
                        });
                    }
                }
            }
            return list;
        }

        public List<VaiTro> GetAllVaiTro()
        {
            var list = new List<VaiTro>();
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT ID, TEN_VAI_TRO, MO_TA FROM VAI_TRO ORDER BY ID";
                using (var cmd = new SqlCommand(sql, conn))
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new VaiTro
                        {
                            ID = Convert.ToInt32(rd["ID"]),
                            TenVaiTro = rd["TEN_VAI_TRO"].ToString(),
                            MoTa = rd["MO_TA"] == DBNull.Value ? "" : rd["MO_TA"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public bool Them(NguoiDung nd)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = @"INSERT INTO NGUOI_DUNG (TEN_DANG_NHAP, MAT_KHAU, HO_TEN, ID_VAI_TRO, TRANG_THAI)
                       VALUES (@user, @pass, @hoten, @idvt, @tt)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@user", nd.TenDangNhap);
                    cmd.Parameters.AddWithValue("@pass", nd.MatKhau);
                    cmd.Parameters.AddWithValue("@hoten", nd.HoTen);
                    cmd.Parameters.AddWithValue("@idvt", nd.IDVaiTro);
                    cmd.Parameters.AddWithValue("@tt", nd.TrangThai);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Sua(NguoiDung nd)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = @"UPDATE NGUOI_DUNG 
                       SET TEN_DANG_NHAP=@user, MAT_KHAU=@pass, HO_TEN=@hoten, 
                           ID_VAI_TRO=@idvt, TRANG_THAI=@tt
                       WHERE ID=@id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", nd.ID);
                    cmd.Parameters.AddWithValue("@user", nd.TenDangNhap);
                    cmd.Parameters.AddWithValue("@pass", nd.MatKhau);
                    cmd.Parameters.AddWithValue("@hoten", nd.HoTen);
                    cmd.Parameters.AddWithValue("@idvt", nd.IDVaiTro);
                    cmd.Parameters.AddWithValue("@tt", nd.TrangThai);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Xoa(int id)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                // Không cho xóa tài khoản Admin gốc (ID = 1)
                string sql = "DELETE FROM NGUOI_DUNG WHERE ID = @id AND ID <> 1";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>Mật khẩu hiện tại của tài khoản có đúng là "matKhau" không (cùng cách so sánh với DangNhap).</summary>
        public bool KiemTraMatKhau(int id, string matKhau)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM NGUOI_DUNG WHERE ID = @id AND MAT_KHAU = @pass AND TRANG_THAI = 1";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@pass", matKhau);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        public bool DoiMatKhau(int id, string matKhauMoi)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE NGUOI_DUNG SET MAT_KHAU = @pass WHERE ID = @id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@pass", matKhauMoi);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool KiemTraTenDangNhapTonTai(string tenDangNhap, int idBoQua = 0)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM NGUOI_DUNG WHERE TEN_DANG_NHAP = @user AND ID <> @id";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@user", tenDangNhap);
                    cmd.Parameters.AddWithValue("@id", idBoQua);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }   
    }
}