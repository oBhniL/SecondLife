using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SecondLife.Models;

namespace SecondLife
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            TaoDuLieuMacDinh();
        }

        private void TaoDuLieuMacDinh()
        {
            var db = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            // Tạo 2 role
            if (!roleManager.RoleExists("Admin")) roleManager.Create(new IdentityRole("Admin"));
            if (!roleManager.RoleExists("User")) roleManager.Create(new IdentityRole("User"));

            // Tạo tài khoản admin mặc định
            var admin = userManager.FindByEmail("admin@secondlife.vn");
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "admin@secondlife.vn",
                    Email = "admin@secondlife.vn",
                    HoTen = "Quản Trị Viên",
                    EmailConfirmed = true,
                    DaXacThucSDT = true,
                    LaNguoiBanDaXacThuc = true,
                    TinhTrangXacThuc = TrangThaiXacThuc.DaDuyet
                };

                var kq = userManager.Create(admin, "Admin@123");
                if (kq.Succeeded) userManager.AddToRole(admin.Id, "Admin");
            }
            else if (!userManager.IsInRole(admin.Id, "Admin"))
            {
                userManager.AddToRole(admin.Id, "Admin");
            }

            // Gán role User cho các tài khoản cũ chưa có role
            var chuaCoRole = db.Users.Where(u => !u.Roles.Any()).Select(u => u.Id).ToList();
            foreach (var id in chuaCoRole)
            {
                userManager.AddToRole(id, "User");
            }

            db.Dispose();
        }
    }
}