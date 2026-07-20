using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecondLife.Models
{
    public enum TrangThaiSanPham
    {
        ChoDuyet,
        DaDuyet,
        TuChoi,
        DaBan,
        DaGo
    }

    public class Product
    {
        [Key]
        public int MaSanPham { get; set; }

        [Required, StringLength(150)]
        public string TieuDe { get; set; }

        [Required]
        public string MoTa { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Gia { get; set; }

        [Range(0, 100)]
        public int TinhTrangPhanTram { get; set; }

        public TrangThaiSanPham TrangThai { get; set; } = TrangThaiSanPham.ChoDuyet;

        public bool TinNoiBat { get; set; } = false;

        public DateTime NgayDang { get; set; } = DateTime.Now;

        public int MaDanhMuc { get; set; }
        public virtual Category DanhMuc { get; set; }

        public string MaNguoiBan { get; set; }
        public virtual ApplicationUser NguoiBan { get; set; }

        public virtual ICollection<ProductImage> DanhSachAnh { get; set; }
        public virtual ICollection<OrderDetail> ChiTietDonHangLienQuan { get; set; }
    }

    public class ProductImage
    {
        [Key]
        public int MaAnh { get; set; }

        [Required]
        public string DuongDanAnh { get; set; }

        public int MaSanPham { get; set; }
        public virtual Product SanPham { get; set; }
    }
}