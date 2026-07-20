using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SecondLife.Models
{
    public enum TrangThaiXacThuc
    {
        ChuaNop,
        DangCho,
        DaDuyet,
        TuChoi
    }

    public class ApplicationUser : IdentityUser
    {
        [Required, StringLength(100)]
        public string HoTen { get; set; }

        public bool DaXacThucSDT { get; set; } = false;

        // Xác thực người bán
        public bool LaNguoiBanDaXacThuc { get; set; } = false;
        public TrangThaiXacThuc TinhTrangXacThuc { get; set; } = TrangThaiXacThuc.ChuaNop;
        public string SoCCCD { get; set; }
        public string AnhCCCDMatTruoc { get; set; }
        public string AnhCCCDMatSau { get; set; }
        public DateTime? NgayNopXacThuc { get; set; }
        public string LyDoTuChoi { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Product> SanPhamDaDang { get; set; }
        public virtual ICollection<Order> DonHangDaMua { get; set; }
        public virtual ICollection<Review> DanhGiaNhanDuoc { get; set; }
        public virtual ICollection<Review> DanhGiaDaViet { get; set; }

        public ClaimsIdentity GenerateUserIdentity(UserManager<ApplicationUser> manager)
        {
            var userIdentity = manager.CreateIdentity(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
        public DateTime? NgaySinh { get; set; }
        public string AnhDaiDien { get; set; }
    }
}