namespace ASProject.Migrations_ApplicationDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserLocation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Country = c.String(),
                        State = c.String(),
                        City = c.String(),
                        ZipCode = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Locations", "Id", "dbo.AspNetUsers");
            DropIndex("dbo.Locations", new[] { "Id" });
            DropTable("dbo.Locations");
        }
    }
}
