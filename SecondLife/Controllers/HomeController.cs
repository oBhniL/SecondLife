using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SecondLife.Models;
using System.Threading.Tasks;
using System.Data.Entity;

namespace SecondLife.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var db = new ApplicationDbContext();

            if (Request.IsAuthenticated)
            {
                var id = User.Identity.GetUserId();
                var nd = db.Users.FirstOrDefault(u => u.Id == id);
                ViewBag.HoTen = nd?.HoTen;
            }

            ViewBag.DanhMucs = db.DanhMucs.ToList();

            var sanPhamMoi = db.SanPhams
                               .Include("DanhMuc")
                               .Include("DanhSachAnh")
                               .Where(p => p.TrangThai == TrangThaiSanPham.DaDuyet)
                               .OrderByDescending(p => p.NgayDang)
                               .Take(10)
                               .ToList();

            ViewBag.TongSanPham = db.SanPhams.Count(p => p.TrangThai == TrangThaiSanPham.DaDuyet);
            ViewBag.TongNguoiDung = db.Users.Count();
            ViewBag.TongDonHang = db.DonHangs.Count();

            return View(sanPhamMoi);
        }

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        // GET: Đăng ký
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Đăng ký
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(DangKyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    HoTen = model.HoTen,
                    PhoneNumber = model.SoDienThoai,
                    NgayTao = System.DateTime.Now
                };

                var result = await UserManager.CreateAsync(user, model.MatKhau);

                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "User");
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    TempData["ThongBao"] = "Đăng ký thành công! Chào mừng bạn đến với SecondLife.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            return View(model);
        }

        // GET: Đăng nhập
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Đăng nhập
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(DangNhapViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(
                model.Email, model.MatKhau, model.GhiNho, shouldLockout: true);

            switch (result)
            {
                case SignInStatus.Success:
                    var nd = UserManager.FindByEmail(model.Email);
                    if (nd != null && UserManager.IsInRole(nd.Id, "Admin"))
                    {
                        TempData["ThongBao"] = "Chào Admin!";
                        return RedirectToAction("Index", "Admin");
                    }
                    TempData["ThongBao"] = "Đăng nhập thành công!";
                    return RedirectToAction("Index", "Home");
                case SignInStatus.LockedOut:
                    ModelState.AddModelError("", "Tài khoản đã bị khóa do đăng nhập sai quá nhiều lần. Vui lòng thử lại sau.");
                    return View(model);

                default:
                    ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                    return View(model);
            }
        }

        // Đăng xuất
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            AuthenticationManager.SignOut(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}