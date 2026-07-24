using Microsoft.AspNet.Identity;
using SecondLife.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SecondLife.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // ===== XEM GIỎ HÀNG =====
        public ActionResult Index()
        {
            string id = User.Identity.GetUserId();
            var gio = db.GioHangs
                        .Include(g => g.SanPham)
                        .Include("SanPham.DanhSachAnh")
                        .Where(g => g.MaNguoiMua == id)
                        .ToList();
            return View(gio);
        }

        // ===== THÊM VÀO GIỎ =====
        [HttpPost]
        public ActionResult Them(int id)
        {
            if (User.IsInRole("Admin"))
            {
                TempData["Loi"] = "Tài khoản quản trị không thể đăng tin bán.";
                return RedirectToAction("Index", "Home");
            }
            string userId = User.Identity.GetUserId();

            // Kiểm tra sản phẩm còn bán không
            var sp = db.SanPhams.FirstOrDefault(p => p.MaSanPham == id
                                                  && p.TrangThai == TrangThaiSanPham.DaDuyet);
            if (sp == null)
            {
                TempData["Loi"] = "Sản phẩm không còn bán.";
                return RedirectToAction("Index", "Product");
            }

            // Không cho tự mua đồ của mình
            if (sp.MaNguoiBan == userId)
            {
                TempData["Loi"] = "Bạn không thể mua sản phẩm của chính mình.";
                return RedirectToAction("ChiTiet", "Product", new { id = id });
            }

            // Nếu đã có trong giỏ thì báo, chưa có thì thêm
            bool daCo = db.GioHangs.Any(g => g.MaNguoiMua == userId && g.MaSanPham == id);
            if (daCo)
            {
                TempData["Loi"] = "Sản phẩm đã có trong giỏ hàng.";
            }
            else
            {
                var item = new CartItem();
                item.MaNguoiMua = userId;
                item.MaSanPham = id;
                db.GioHangs.Add(item);
                db.SaveChanges();
                TempData["ThongBao"] = "Đã thêm vào giỏ hàng!";
            }

            return RedirectToAction("ChiTiet", "Product", new { id = id });
        }

        // ===== XÓA KHỎI GIỎ =====
        [HttpPost]
        public ActionResult Xoa(int id)
        {
            string userId = User.Identity.GetUserId();
            var item = db.GioHangs.FirstOrDefault(g => g.MaGioHang == id && g.MaNguoiMua == userId);
            if (item != null)
            {
                db.GioHangs.Remove(item);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Cart");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}