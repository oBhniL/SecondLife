using System;
using System.ComponentModel.DataAnnotations;

namespace SecondLife.Models
{
    public class Review
    {
        [Key]  
        
        public int MaDanhGia { get; set; }

        [Range(1, 5)]
        public int SoSao { get; set; }

        public string BinhLuan { get; set; }
        public DateTime NgayDanhGia { get; set; } = DateTime.Now;

        public string MaNguoiDanhGia { get; set; }
        public virtual ApplicationUser NguoiDanhGia { get; set; }

        public string MaNguoiDuocDanhGia { get; set; }
        public virtual ApplicationUser NguoiDuocDanhGia { get; set; }

        public int MaDonHang { get; set; }
    }
}