namespace SecondLife.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThemNgaySinh : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "NgaySinh", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "AnhDaiDien", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "AnhDaiDien");
            DropColumn("dbo.AspNetUsers", "NgaySinh");
        }
    }
}
