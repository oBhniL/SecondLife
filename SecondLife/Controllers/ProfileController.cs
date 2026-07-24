using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SecondLife.Models;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SecondLife.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            string id = User.Identity.GetUserId();
            var nguoiDung = db.Users.FirstOrDefault(u => u.Id == id);

            ViewBag.TinCuaToi = db.SanPhams
                                  .Include("DanhMuc")
                                  .Include("DanhSachAnh")
                                  .Where(p => p.MaNguoiBan == id)
                                  .OrderByDescending(p => p.NgayDang)
                                  .Take(6)
                                  .ToList();

            ViewBag.TongTin = db.SanPhams.Count(p => p.MaNguoiBan == id);
            return View(nguoiDung);
        }
        public ActionResult TinDaDang()
        {
            string id = User.Identity.GetUserId();
            var ds = db.SanPhams
                       .Include("DanhMuc")
                       .Include("DanhSachAnh")
                       .Where(p => p.MaNguoiBan == id)
                       .OrderByDescending(p => p.NgayDang)
                       .ToList();
            return View(ds);
        }

        public ActionResult DonHang()
        {
            string id = User.Identity.GetUserId();
            var ds = db.DonHangs
                       .Where(o => o.MaNguoiMua == id)
                       .OrderByDescending(o => o.NgayDatHang)
                       .ToList();
            return View(ds);
        }
        // ===== ĐƠN BÁN - các đơn chứa sản phẩm của mình =====
        public ActionResult DonBan()
        {
            string id = User.Identity.GetUserId();

            var dsChiTiet = db.ChiTietDonHangs
                              .Include(c => c.DonHang)
                              .Include(c => c.DonHang.NguoiMua)
                              .Include(c => c.SanPham)
                              .Where(c => c.SanPham.MaNguoiBan == id)
                              .OrderByDescending(c => c.DonHang.NgayDatHang)
                              .ToList();
            ViewBag.SoTinDaDang = db.SanPhams.Count(p => p.MaNguoiBan == id);

            return View(dsChiTiet);
        }

        // ===== CẬP NHẬT TRẠNG THÁI ĐƠN =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatDonBan(int maDonHang, string trangThaiMoi)
        {
            string id = User.Identity.GetUserId();

            // Kiểm tra đơn này có chứa sản phẩm của mình không
            bool laCuaMinh = db.ChiTietDonHangs
                               .Any(c => c.MaDonHang == maDonHang && c.SanPham.MaNguoiBan == id);

            if (!laCuaMinh)
            {
                TempData["Loi"] = "Bạn không có quyền cập nhật đơn này.";
                return RedirectToAction("DonBan");
            }

            var dh = db.DonHangs.FirstOrDefault(d => d.MaDonHang == maDonHang);
            if (dh != null)
            {
                if (trangThaiMoi == "DangGiao")
                    dh.TrangThai = TrangThaiDonHang.DangGiao;
                else if (trangThaiMoi == "HoanTat")
                    dh.TrangThai = TrangThaiDonHang.HoanTat;
                else if (trangThaiMoi == "DaHuy")
                    dh.TrangThai = TrangThaiDonHang.DaHuy;

                db.SaveChanges();
                TempData["ThongBao"] = "Đã cập nhật trạng thái đơn hàng.";
            }

            return RedirectToAction("DonBan");
        }
        public ActionResult XacThuc()
        {
            string id = User.Identity.GetUserId();
            var nguoiDung = db.Users.FirstOrDefault(u => u.Id == id);
            return View(nguoiDung);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NopXacThuc(string SoCCCD, HttpPostedFileBase AnhTruoc, HttpPostedFileBase AnhSau)
        {
            string id = User.Identity.GetUserId();
            var nguoiDung = db.Users.FirstOrDefault(u => u.Id == id);

            string thuMuc = Server.MapPath("~/Uploads/CCCD");
            if (!Directory.Exists(thuMuc)) Directory.CreateDirectory(thuMuc);

            if (AnhTruoc != null && AnhTruoc.ContentLength > 0)
            {
                string ten = id + "_truoc" + Path.GetExtension(AnhTruoc.FileName);
                AnhTruoc.SaveAs(Path.Combine(thuMuc, ten));
                nguoiDung.AnhCCCDMatTruoc = "~/Uploads/CCCD/" + ten;
            }

            if (AnhSau != null && AnhSau.ContentLength > 0)
            {
                string ten = id + "_sau" + Path.GetExtension(AnhSau.FileName);
                AnhSau.SaveAs(Path.Combine(thuMuc, ten));
                nguoiDung.AnhCCCDMatSau = "~/Uploads/CCCD/" + ten;
            }

            nguoiDung.SoCCCD = SoCCCD;
            nguoiDung.TinhTrangXacThuc = TrangThaiXacThuc.DangCho;
            nguoiDung.NgayNopXacThuc = DateTime.Now;
            nguoiDung.LyDoTuChoi = null;
            db.SaveChanges();

            TempData["ThongBao"] = "Đã nộp hồ sơ, chờ admin duyệt.";
            return RedirectToAction("XacThuc");
        }
        public ActionResult ChinhSua()
        {
            string id = User.Identity.GetUserId();
            var nguoiDung = db.Users.FirstOrDefault(u => u.Id == id);
            return View(nguoiDung);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChinhSua(string HoTen, string PhoneNumber, DateTime? NgaySinh,
                                     string Email, HttpPostedFileBase AnhMoi)
        {
            string id = User.Identity.GetUserId();
            var nguoiDung = db.Users.FirstOrDefault(u => u.Id == id);

            if (string.IsNullOrWhiteSpace(HoTen))
            {
                TempData["Loi"] = "Họ tên không được để trống.";
                return RedirectToAction("ChinhSua");
            }

            if (!string.IsNullOrWhiteSpace(Email) && Email != nguoiDung.Email)
            {
                bool trung = db.Users.Any(u => u.Email == Email && u.Id != id);
                if (trung)
                {
                    TempData["Loi"] = "Email này đã được sử dụng.";
                    return RedirectToAction("ChinhSua");
                }
                nguoiDung.Email = Email;
                nguoiDung.UserName = Email;
            }

            if (AnhMoi != null && AnhMoi.ContentLength > 0)
            {
                string duoi = Path.GetExtension(AnhMoi.FileName).ToLower();
                if (duoi != ".jpg" && duoi != ".jpeg" && duoi != ".png")
                {
                    TempData["Loi"] = "Chỉ chấp nhận ảnh JPG hoặc PNG.";
                    return RedirectToAction("ChinhSua");
                }

                string thuMuc = Server.MapPath("~/Uploads/Avatar");
                if (!Directory.Exists(thuMuc)) Directory.CreateDirectory(thuMuc);

                string ten = id + "_avatar" + duoi;
                AnhMoi.SaveAs(Path.Combine(thuMuc, ten));
                nguoiDung.AnhDaiDien = "~/Uploads/Avatar/" + ten;
            }

            nguoiDung.HoTen = HoTen;
            nguoiDung.PhoneNumber = PhoneNumber;
            nguoiDung.NgaySinh = NgaySinh;
            db.SaveChanges();

            TempData["ThongBao"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }


        public ActionResult DoiMatKhau()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DoiMatKhau(string MatKhauCu, string MatKhauMoi, string XacNhanMatKhau)
        {
            if (MatKhauMoi != XacNhanMatKhau)
            {
                TempData["Loi"] = "Mật khẩu xác nhận không khớp.";
                return RedirectToAction("DoiMatKhau");
            }

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var kq = userManager.ChangePassword(User.Identity.GetUserId(), MatKhauCu, MatKhauMoi);

            if (kq.Succeeded)
            {
                TempData["ThongBao"] = "Đổi mật khẩu thành công!";
                return RedirectToAction("Index");
            }

            TempData["Loi"] = string.Join(", ", kq.Errors);
            return RedirectToAction("DoiMatKhau");
        }
    }
}