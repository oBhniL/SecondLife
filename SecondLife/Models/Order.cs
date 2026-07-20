using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace SecondLife.Models
{
    public enum TrangThaiDonHang
    {
        ChoXacNhan,
        DangGiao,
        HoanTat,
        DaHuy
    }

    public class Order
    {
        [Key]
        public int MaDonHang { get; set; }
        public string MaNguoiMua { get; set; }
        public virtual ApplicationUser NguoiMua { get; set; }

        public TrangThaiDonHang TrangThai { get; set; } = TrangThaiDonHang.ChoXacNhan;

        [Column(TypeName = "decimal")]
        public decimal TongTien { get; set; }

        public string DiaChiGiaoHang { get; set; }
        public string SoDienThoaiLienHe { get; set; }

        public DateTime NgayDatHang { get; set; } = DateTime.Now;

        public virtual ICollection<OrderDetail> ChiTietDonHang { get; set; }
    }

    public class OrderDetail
    {
        [Key]
        public int MaChiTietDonHang { get; set; }

        public int MaDonHang { get; set; }
        public virtual Order DonHang { get; set; }

        public int MaSanPham { get; set; }
        public virtual Product SanPham { get; set; }

        [Column(TypeName = "decimal")]
        public decimal GiaLucMua { get; set; }
    }
}