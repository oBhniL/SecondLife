using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SecondLife.Controllers
{
    [Authorize] // Bắt buộc đăng nhập mới được vào
    public class AdminController : Controller 
    {
        public ActionResult Index()
        {
            
            if (User.Identity.Name != "admin@secondlife.vn")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}