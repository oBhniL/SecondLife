namespace SecondLife.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductImages",
                c => new
                    {
                        MaAnh = c.Int(nullable: false, identity: true),
                        DuongDanAnh = c.String(nullable: false),
                        MaSanPham = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MaAnh)
                .ForeignKey("dbo.Products", t => t.MaSanPham, cascadeDelete: true)
                .Index(t => t.MaSanPham);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        MaSanPham = c.Int(nullable: false, identity: true),
                        TieuDe = c.String(nullable: false, maxLength: 150),
                        MoTa = c.String(nullable: false),
                        Gia = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TinhTrangPhanTram = c.Int(nullable: false),
                        TrangThai = c.Int(nullable: false),
                        TinNoiBat = c.Boolean(nullable: false),
                        NgayDang = c.DateTime(nullable: false),
                        MaDanhMuc = c.Int(nullable: false),
                        MaNguoiBan = c.String(),
                        NguoiBan_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.MaSanPham)
                .ForeignKey("dbo.AspNetUsers", t => t.NguoiBan_Id)
                .ForeignKey("dbo.Categories", t => t.MaDanhMuc, cascadeDelete: true)
                .Index(t => t.MaDanhMuc)
                .Index(t => t.NguoiBan_Id);
            
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        MaChiTietDonHang = c.Int(nullable: false, identity: true),
                        MaDonHang = c.Int(nullable: false),
                        MaSanPham = c.Int(nullable: false),
                        GiaLucMua = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.MaChiTietDonHang)
                .ForeignKey("dbo.Orders", t => t.MaDonHang, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.MaSanPham, cascadeDelete: true)
                .Index(t => t.MaDonHang)
                .Index(t => t.MaSanPham);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        MaDonHang = c.Int(nullable: false, identity: true),
                        MaNguoiMua = c.String(),
                        TrangThai = c.Int(nullable: false),
                        TongTien = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiaChiGiaoHang = c.String(),
                        SoDienThoaiLienHe = c.String(),
                        NgayDatHang = c.DateTime(nullable: false),
                        NguoiMua_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.MaDonHang)
                .ForeignKey("dbo.AspNetUsers", t => t.NguoiMua_Id)
                .Index(t => t.NguoiMua_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        HoTen = c.String(nullable: false, maxLength: 100),
                        DaXacThucSDT = c.Boolean(nullable: false),
                        LaNguoiBanDaXacThuc = c.Boolean(nullable: false),
                        TinhTrangXacThuc = c.Int(nullable: false),
                        SoCCCD = c.String(),
                        AnhCCCDMatTruoc = c.String(),
                        AnhCCCDMatSau = c.String(),
                        NgayNopXacThuc = c.DateTime(),
                        LyDoTuChoi = c.String(),
                        NgayTao = c.DateTime(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        MaDanhGia = c.Int(nullable: false, identity: true),
                        SoSao = c.Int(nullable: false),
                        BinhLuan = c.String(),
                        NgayDanhGia = c.DateTime(nullable: false),
                        MaNguoiDanhGia = c.String(),
                        MaNguoiDuocDanhGia = c.String(),
                        MaDonHang = c.Int(nullable: false),
                        NguoiDanhGia_Id = c.String(maxLength: 128),
                        NguoiDuocDanhGia_Id = c.String(maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        ApplicationUser_Id1 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.MaDanhGia)
                .ForeignKey("dbo.AspNetUsers", t => t.NguoiDanhGia_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.NguoiDuocDanhGia_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id1)
                .Index(t => t.NguoiDanhGia_Id)
                .Index(t => t.NguoiDuocDanhGia_Id)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id1);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        MaDanhMuc = c.Int(nullable: false, identity: true),
                        TenDanhMuc = c.String(nullable: false, maxLength: 100),
                        MoTa = c.String(),
                        DuongDanIcon = c.String(),
                    })
                .PrimaryKey(t => t.MaDanhMuc);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ProductImages", "MaSanPham", "dbo.Products");
            DropForeignKey("dbo.Products", "MaDanhMuc", "dbo.Categories");
            DropForeignKey("dbo.OrderDetails", "MaSanPham", "dbo.Products");
            DropForeignKey("dbo.Products", "NguoiBan_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Orders", "NguoiMua_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reviews", "ApplicationUser_Id1", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reviews", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reviews", "NguoiDuocDanhGia_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reviews", "NguoiDanhGia_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrderDetails", "MaDonHang", "dbo.Orders");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Reviews", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.Reviews", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Reviews", new[] { "NguoiDuocDanhGia_Id" });
            DropIndex("dbo.Reviews", new[] { "NguoiDanhGia_Id" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Orders", new[] { "NguoiMua_Id" });
            DropIndex("dbo.OrderDetails", new[] { "MaSanPham" });
            DropIndex("dbo.OrderDetails", new[] { "MaDonHang" });
            DropIndex("dbo.Products", new[] { "NguoiBan_Id" });
            DropIndex("dbo.Products", new[] { "MaDanhMuc" });
            DropIndex("dbo.ProductImages", new[] { "MaSanPham" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Categories");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Reviews");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Products");
            DropTable("dbo.ProductImages");
        }
    }
}
