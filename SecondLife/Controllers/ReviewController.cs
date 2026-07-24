using Microsoft.AspNet.Identity;
using SecondLife.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SecondLife.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // ===== FORM ĐÁNH GIÁ (GET) =====
        public ActionResult DanhGia(int id)   // id = mã đơn hàng
        {
            string userId = User.Identity.GetUserId();

            var dh = db.DonHangs.FirstOrDefault(d => d.MaDonHang == id && d.MaNguoiMua == userId);

            if (dh == null)
            {
                TempData["Loi"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("DonHang", "Profile");
            }

            if (dh.TrangThai != TrangThaiDonHang.HoanTat)
            {
                TempData["Loi"] = "Chỉ đánh giá được khi đơn hàng đã hoàn tất.";
                return RedirectToAction("DonHang", "Profile");
            }

            // Đã đánh giá rồi thì không cho đánh giá lại
            bool daDanhGia = db.DanhGias.Any(r => r.MaDonHang == id && r.MaNguoiDanhGia == userId);
            if (daDanhGia)
            {
                TempData["Loi"] = "Bạn đã đánh giá đơn hàng này rồi.";
                return RedirectToAction("DonHang", "Profile");
            }

            // Lấy người bán từ chi tiết đơn
            var ct = db.ChiTietDonHangs
                       .Include(c => c.SanPham)
                       .Include("SanPham.NguoiBan")
                       .FirstOrDefault(c => c.MaDonHang == id);

            if (ct == null)
            {
                TempData["Loi"] = "Đơn hàng không có sản phẩm.";
                return RedirectToAction("DonHang", "Profile");
            }

            ViewBag.MaDonHang = id;
            ViewBag.NguoiBan = ct.SanPham.NguoiBan;
            ViewBag.TenSanPham = ct.SanPham.TieuDe;

            return View();
        }

        // ===== LƯU ĐÁNH GIÁ (POST) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DanhGia(int MaDonHang, int SoSao, string BinhLuan)
        {
            string userId = User.Identity.GetUserId();

            var dh = db.DonHangs.FirstOrDefault(d => d.MaDonHang == MaDonHang && d.MaNguoiMua == userId);

            if (dh == null || dh.TrangThai != TrangThaiDonHang.HoanTat)
            {
                TempData["Loi"] = "Không thể đánh giá đơn hàng này.";
                return RedirectToAction("DonHang", "Profile");
            }

            bool daDanhGia = db.DanhGias.Any(r => r.MaDonHang == MaDonHang && r.MaNguoiDanhGia == userId);
            if (daDanhGia)
            {
                TempData["Loi"] = "Bạn đã đánh giá đơn hàng này rồi.";
                return RedirectToAction("DonHang", "Profile");
            }

            if (SoSao < 1 || SoSao > 5)
            {
                TempData["Loi"] = "Vui lòng chọn số sao từ 1 đến 5.";
                return RedirectToAction("DanhGia", new { id = MaDonHang });
            }

            // Tìm người bán
            var ct = db.ChiTietDonHangs
                       .Include(c => c.SanPham)
                       .FirstOrDefault(c => c.MaDonHang == MaDonHang);

            if (ct == null)
            {
                TempData["Loi"] = "Đơn hàng không có sản phẩm.";
                return RedirectToAction("DonHang", "Profile");
            }

            var dg = new Review();
            dg.MaDonHang = MaDonHang;
            dg.MaNguoiDanhGia = userId;
            dg.MaNguoiDuocDanhGia = ct.SanPham.MaNguoiBan;
            dg.SoSao = SoSao;
            dg.BinhLuan = BinhLuan;
            dg.NgayDanhGia = DateTime.Now;

            db.DanhGias.Add(dg);
            db.SaveChanges();

            TempData["ThongBao"] = "Cảm ơn bạn đã đánh giá!";
            return RedirectToAction("DonHang", "Profile");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}