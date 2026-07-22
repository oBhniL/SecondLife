using Microsoft.AspNet.Identity;
using SecondLife.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SecondLife.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // ===== TRANG ĐẶT HÀNG (GET) =====
        public ActionResult DatHang(int id)
        {
            var sp = db.SanPhams
                       .Include("DanhSachAnh")
                       .Include("NguoiBan")
                       .FirstOrDefault(p => p.MaSanPham == id
                                         && p.TrangThai == TrangThaiSanPham.DaDuyet);

            if (sp == null)
            {
                TempData["Loi"] = "Không tìm thấy sản phẩm này.";
                return RedirectToAction("Index", "Product");
            }

            return View(sp);
        }

        // ===== XÁC NHẬN ĐẶT HÀNG (POST) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DatHang(int MaSanPham, string DiaChiGiaoHang, string SoDienThoaiLienHe)
        {
            var sp = db.SanPhams.FirstOrDefault(p => p.MaSanPham == MaSanPham
                                                  && p.TrangThai == TrangThaiSanPham.DaDuyet);
            if (sp == null)
                return HttpNotFound();

            if (string.IsNullOrWhiteSpace(DiaChiGiaoHang) || string.IsNullOrWhiteSpace(SoDienThoaiLienHe))
            {
                TempData["Loi"] = "Vui lòng nhập địa chỉ và số điện thoại.";
                return RedirectToAction("DatHang", new { id = MaSanPham });
            }

            string id = User.Identity.GetUserId();

            // Tạo đơn hàng
            var dh = new Order();
            dh.MaNguoiMua = id;
            dh.TongTien = sp.Gia;
            dh.DiaChiGiaoHang = DiaChiGiaoHang;
            dh.SoDienThoaiLienHe = SoDienThoaiLienHe;
            dh.NgayDatHang = DateTime.Now;
            dh.TrangThai = TrangThaiDonHang.ChoXacNhan;
            db.DonHangs.Add(dh);
            db.SaveChanges();

            // Tạo chi tiết đơn hàng
            var ct = new OrderDetail();
            ct.MaDonHang = dh.MaDonHang;
            ct.MaSanPham = sp.MaSanPham;
            ct.GiaLucMua = sp.Gia;
            db.ChiTietDonHangs.Add(ct);

            // Đánh dấu sản phẩm đã bán
            sp.TrangThai = TrangThaiSanPham.DaBan;
            db.SaveChanges();

            TempData["ThongBao"] = "Đặt hàng thành công!";
            return RedirectToAction("DonHang", "Profile");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}