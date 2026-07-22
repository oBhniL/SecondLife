namespace SecondLife.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThemGioHang : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CartItems",
                c => new
                    {
                        MaGioHang = c.Int(nullable: false, identity: true),
                        MaNguoiMua = c.String(),
                        MaSanPham = c.Int(nullable: false),
                        NgayThem = c.DateTime(nullable: false),
                        NguoiMua_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.MaGioHang)
                .ForeignKey("dbo.AspNetUsers", t => t.NguoiMua_Id)
                .ForeignKey("dbo.Products", t => t.MaSanPham, cascadeDelete: true)
                .Index(t => t.MaSanPham)
                .Index(t => t.NguoiMua_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CartItems", "MaSanPham", "dbo.Products");
            DropForeignKey("dbo.CartItems", "NguoiMua_Id", "dbo.AspNetUsers");
            DropIndex("dbo.CartItems", new[] { "NguoiMua_Id" });
            DropIndex("dbo.CartItems", new[] { "MaSanPham" });
            DropTable("dbo.CartItems");
        }
    }
}
