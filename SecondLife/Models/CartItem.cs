using System;
using System.ComponentModel.DataAnnotations;

namespace SecondLife.Models
{
    public class CartItem
    {
        [Key]
        public int MaGioHang { get; set; }

        public string MaNguoiMua { get; set; }
        public virtual ApplicationUser NguoiMua { get; set; }

        public int MaSanPham { get; set; }
        public virtual Product SanPham { get; set; }

        public DateTime NgayThem { get; set; } = DateTime.Now;
    }
}