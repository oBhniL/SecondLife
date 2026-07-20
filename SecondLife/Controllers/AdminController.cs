using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SecondLife.Models;

namespace SecondLife.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // ===== DASHBOARD =====
        public ActionResult Index()
        {
            DemViecCho();
            ViewBag.TongNguoiDung = db.Users.Count();
            ViewBag.TongSanPham = db.SanPhams.Count();
            ViewBag.TongDonHang = db.DonHangs.Count();
            ViewBag.TongDanhMuc = db.DanhMucs.Count();

            ViewBag.ChoDuyetTin = db.SanPhams.Count(p => p.TrangThai == TrangThaiSanPham.ChoDuyet);
            ViewBag.ChoDuyetXacThuc = db.Users.Count(u => u.TinhTrangXacThuc == TrangThaiXacThuc.DangCho);
            ViewBag.DoanhThu = db.DonHangs
                                 .Where(o => o.TrangThai == TrangThaiDonHang.HoanTat)
                                 .Select(o => (decimal?)o.TongTien).Sum() ?? 0;

            ViewBag.TinMoiNhat = db.SanPhams
                                   .Include("NguoiBan")
                                   .OrderByDescending(p => p.NgayDang)
                                   .Take(5).ToList();

            return View();
        }

        // ===== DUYỆT TIN =====
        public ActionResult DuyetTin(string loc = "ChoDuyet")
        {
            DemViecCho();
            var q = db.SanPhams.Include("NguoiBan").Include("DanhMuc").Include("DanhSachAnh").AsQueryable();

            if (loc == "ChoDuyet") q = q.Where(p => p.TrangThai == TrangThaiSanPham.ChoDuyet);
            else if (loc == "DaDuyet") q = q.Where(p => p.TrangThai == TrangThaiSanPham.DaDuyet);
            else if (loc == "TuChoi") q = q.Where(p => p.TrangThai == TrangThaiSanPham.TuChoi);

            ViewBag.Loc = loc;
            return View(q.OrderByDescending(p => p.NgayDang).ToList());
        }

        [HttpPost]
        public ActionResult DuyetSanPham(int id)
        {
            var sp = db.SanPhams.Find(id);
            if (sp != null)
            {
                sp.TrangThai = TrangThaiSanPham.DaDuyet;
                db.SaveChanges();
                TempData["ThongBao"] = "Đã duyệt tin: " + sp.TieuDe;
            }
            return RedirectToAction("DuyetTin");
        }

        [HttpPost]
        public ActionResult TuChoiSanPham(int id)
        {
            var sp = db.SanPhams.Find(id);
            if (sp != null)
            {
                sp.TrangThai = TrangThaiSanPham.TuChoi;
                db.SaveChanges();
                TempData["ThongBao"] = "Đã từ chối tin: " + sp.TieuDe;
            }
            return RedirectToAction("DuyetTin");
        }

        // ===== DUYỆT XÁC THỰC CCCD =====
        public ActionResult DuyetXacThuc(string loc = "DangCho")
        {
            DemViecCho();
            var q = db.Users.AsQueryable();

            if (loc == "DangCho") q = q.Where(u => u.TinhTrangXacThuc == TrangThaiXacThuc.DangCho);
            else if (loc == "DaDuyet") q = q.Where(u => u.TinhTrangXacThuc == TrangThaiXacThuc.DaDuyet);
            else if (loc == "TuChoi") q = q.Where(u => u.TinhTrangXacThuc == TrangThaiXacThuc.TuChoi);

            ViewBag.Loc = loc;
            return View(q.OrderByDescending(u => u.NgayNopXacThuc).ToList());
        }

        [HttpPost]
        public ActionResult DuyetCCCD(string id)
        {
            var nd = db.Users.FirstOrDefault(u => u.Id == id);
            if (nd != null)
            {
                nd.TinhTrangXacThuc = TrangThaiXacThuc.DaDuyet;
                nd.LaNguoiBanDaXacThuc = true;
                nd.LyDoTuChoi = null;
                db.SaveChanges();
                TempData["ThongBao"] = "Đã duyệt xác thực cho " + nd.HoTen;
            }
            return RedirectToAction("DuyetXacThuc");
        }

        [HttpPost]
        public ActionResult TuChoiCCCD(string id, string lyDo)
        {
            var nd = db.Users.FirstOrDefault(u => u.Id == id);
            if (nd != null)
            {
                nd.TinhTrangXacThuc = TrangThaiXacThuc.TuChoi;
                nd.LaNguoiBanDaXacThuc = false;
                nd.LyDoTuChoi = string.IsNullOrWhiteSpace(lyDo) ? "Hồ sơ không hợp lệ" : lyDo;
                db.SaveChanges();
                TempData["ThongBao"] = "Đã từ chối hồ sơ của " + nd.HoTen;
            }
            return RedirectToAction("DuyetXacThuc");
        }

        // ===== QUẢN LÝ DANH MỤC =====
        public ActionResult DanhMuc()
        {
            DemViecCho();

            var ds = db.DanhMucs.OrderBy(c => c.TenDanhMuc).ToList();

            ViewBag.DemSP = db.SanPhams
                              .GroupBy(p => p.MaDanhMuc)
                              .ToDictionary(g => g.Key, g => g.Count());

            return View(ds);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ThemDanhMuc(string TenDanhMuc)
        {
            if (string.IsNullOrWhiteSpace(TenDanhMuc))
            {
                TempData["Loi"] = "Tên danh mục không được để trống.";
                return RedirectToAction("DanhMuc");
            }

            if (db.DanhMucs.Any(c => c.TenDanhMuc == TenDanhMuc))
            {
                TempData["Loi"] = "Danh mục này đã tồn tại.";
                return RedirectToAction("DanhMuc");
            }

            db.DanhMucs.Add(new Category { TenDanhMuc = TenDanhMuc });
            db.SaveChanges();
            TempData["ThongBao"] = "Đã thêm danh mục.";
            return RedirectToAction("DanhMuc");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SuaDanhMuc(int MaDanhMuc, string TenDanhMuc)
        {
            var dm = db.DanhMucs.Find(MaDanhMuc);
            if (dm != null && !string.IsNullOrWhiteSpace(TenDanhMuc))
            {
                dm.TenDanhMuc = TenDanhMuc;
                db.SaveChanges();
                TempData["ThongBao"] = "Đã cập nhật danh mục.";
            }
            return RedirectToAction("DanhMuc");
        }

        [HttpPost]
        public ActionResult XoaDanhMuc(int id)
        {
            var dm = db.DanhMucs.Find(id);
            if (dm == null) return RedirectToAction("DanhMuc");

            if (db.SanPhams.Any(p => p.MaDanhMuc == id))
            {
                TempData["Loi"] = "Không thể xóa: danh mục đang có sản phẩm.";
                return RedirectToAction("DanhMuc");
            }

            db.DanhMucs.Remove(dm);
            db.SaveChanges();
            TempData["ThongBao"] = "Đã xóa danh mục.";
            return RedirectToAction("DanhMuc");
        }

        // ===== QUẢN LÝ NGƯỜI DÙNG =====
        public ActionResult NguoiDung(string tuKhoa)
        {
            DemViecCho();
            var q = db.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
                q = q.Where(u => u.HoTen.Contains(tuKhoa) || u.Email.Contains(tuKhoa));

            ViewBag.TuKhoa = tuKhoa;
            return View(q.OrderByDescending(u => u.NgayTao).ToList());
        }

        // ===== QUẢN LÝ ĐƠN HÀNG =====
        public ActionResult DonHang()
        {
            DemViecCho();
            var ds = db.DonHangs
                       .Include("NguoiMua")
                       .OrderByDescending(o => o.NgayDatHang)
                       .ToList();
            return View(ds);
        }
        private void DemViecCho()
        {
            ViewBag.SoTinCho = db.SanPhams.Count(p => p.TrangThai == TrangThaiSanPham.ChoDuyet);
            ViewBag.SoXacThucCho = db.Users.Count(u => u.TinhTrangXacThuc == TrangThaiXacThuc.DangCho);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}