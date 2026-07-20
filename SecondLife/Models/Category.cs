using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecondLife.Models
{
    public class Category
    {
        [Key]
        public int MaDanhMuc { get; set; }

        [Required, StringLength(100)]
        public string TenDanhMuc { get; set; }

        public string MoTa { get; set; }
        public string DuongDanIcon { get; set; }

        public virtual ICollection<Product> DanhSachSanPham { get; set; }
    }
}