using Microsoft.AspNet.Identity;
using SecondLife.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SecondLife.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // ===== DANH SÁCH SẢN PHẨM (có tìm kiếm + lọc danh mục) =====
        public ActionResult Index(string tuKhoa, int? danhMuc)
        {
            var ds = db.SanPhams
                       .Include("DanhMuc")
                       .Include("DanhSachAnh")
                       .Where(p => p.TrangThai == TrangThaiSanPham.DaDuyet);

            if (!string.IsNullOrWhiteSpace(tuKhoa))
                ds = ds.Where(p => p.TieuDe.Contains(tuKhoa));

            if (danhMuc != null)
                ds = ds.Where(p => p.MaDanhMuc == danhMuc);

            ViewBag.DanhMucs = db.DanhMucs.ToList();
            ViewBag.TuKhoa = tuKhoa;
            ViewBag.DanhMucChon = danhMuc;

            return View(ds.OrderByDescending(p => p.NgayDang).ToList());
        }

        // ===== CHI TIẾT SẢN PHẨM =====
        public ActionResult ChiTiet(int id)
        {
            var sp = db.SanPhams
                       .Include(p => p.DanhMuc)
                       .Include(p => p.DanhSachAnh)
                       .Include(p => p.NguoiBan)
                       .FirstOrDefault(p => p.MaSanPham == id);

            if (sp == null)
                return HttpNotFound();

            return View(sp);
        }

        // ===== ĐĂNG TIN (GET) =====
        [Authorize]
        public ActionResult DangTin()
        {
            if (User.IsInRole("Admin"))
            {
                TempData["Loi"] = "Tài khoản quản trị không thể đăng tin bán.";
                return RedirectToAction("Index", "Home");
            }
            string userId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null || user.TinhTrangXacThuc != TrangThaiXacThuc.DaDuyet)
            {
                TempData["Loi"] = "Bạn cần xác thực CCCD để được đăng tin.";
                return RedirectToAction("XacThuc", "Profile");
            }

            ViewBag.DanhMucs = db.DanhMucs.ToList();
            return View();
        }

        // ===== ĐĂNG TIN (POST) =====
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangTin(string TieuDe, string MoTa, decimal Gia,
                                    int TinhTrangPhanTram, int MaDanhMuc,
                                    IEnumerable<HttpPostedFileBase> DanhSachAnh)
        {
            if (string.IsNullOrWhiteSpace(TieuDe) || string.IsNullOrWhiteSpace(MoTa))
            {
                TempData["Loi"] = "Vui lòng nhập đầy đủ tiêu đề và mô tả.";
                ViewBag.DanhMucs = db.DanhMucs.ToList();
                return View();
            }

            string id = User.Identity.GetUserId();
            // Chặn nếu chưa xác thực
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null || user.TinhTrangXacThuc != TrangThaiXacThuc.DaDuyet)
            {
                TempData["Loi"] = "Bạn cần xác thực CCCD để được đăng tin.";
                return RedirectToAction("XacThuc", "Profile");
            }

            var sp = new Product();
            sp.TieuDe = TieuDe;
            sp.MoTa = MoTa;
            sp.Gia = Gia;
            sp.TinhTrangPhanTram = TinhTrangPhanTram;
            sp.MaDanhMuc = MaDanhMuc;
            sp.MaNguoiBan = id;
            sp.NgayDang = DateTime.Now;
            sp.TrangThai = TrangThaiSanPham.ChoDuyet;

            db.SanPhams.Add(sp);
            db.SaveChanges();

            // Lưu ảnh
            if (DanhSachAnh != null)
            {
                string thuMuc = Server.MapPath("~/Uploads/SanPham");
                if (!Directory.Exists(thuMuc))
                    Directory.CreateDirectory(thuMuc);

                int stt = 1;
                foreach (var anh in DanhSachAnh)
                {
                    if (anh != null && anh.ContentLength > 0)
                    {
                        string ten = sp.MaSanPham + "_" + stt + Path.GetExtension(anh.FileName);
                        anh.SaveAs(Path.Combine(thuMuc, ten));

                        var img = new ProductImage();
                        img.MaSanPham = sp.MaSanPham;
                        img.DuongDanAnh = "~/Uploads/SanPham/" + ten;
                        db.AnhSanPhams.Add(img);
                        stt++;
                    }
                }
                db.SaveChanges();
            }

            TempData["ThongBao"] = "Đăng tin thành công! Tin đang chờ admin duyệt.";
            return RedirectToAction("Index", "Product");
        }

        // ===== XÓA TIN CỦA MÌNH =====
        [Authorize]
        [HttpPost]
        public ActionResult XoaTin(int id)
        {
            string userId = User.Identity.GetUserId();
            var sp = db.SanPhams.FirstOrDefault(p => p.MaSanPham == id && p.MaNguoiBan == userId);

            if (sp != null)
            {
                sp.TrangThai = TrangThaiSanPham.DaGo;
                db.SaveChanges();
                TempData["ThongBao"] = "Đã gỡ tin.";
            }
            return RedirectToAction("TinDaDang", "Profile");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}