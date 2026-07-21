using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SecondLife.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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
        public ActionResult Login()
        {
            return View();
        }
        // Hàm này dùng để nhận dữ liệu khi người dùng bấm nút Đăng Nhập
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            // Kiểm tra đúng tài khoản Admin Linh giao chưa
            if (email == "admin@secondlife.vn" && password == "Admin@123")
            {
                // 1. Tạo chứng minh thư (Identity) xác nhận người này là Admin
                var identity = new System.Security.Claims.ClaimsIdentity(new[] {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, email)
        }, Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);

                // 2. Ép hệ thống cấp thẻ Cookie đăng nhập
                Request.GetOwinContext().Authentication.SignIn(identity);

                // 3. Đẩy thẳng vào trang Admin luôn cho ngầu
                return RedirectToAction("Index", "Admin");
            }

            // Nếu sai tài khoản thì bắt nhập lại
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
        public ActionResult Logout()
        {
            
            Request.GetOwinContext().Authentication.SignOut(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
    }
}