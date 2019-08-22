namespace ASProject.Migrations_SystemContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Pet_PetImageModelUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PetImage", "IsDisplayPic", c => c.Boolean(nullable: false));
            CreateIndex("dbo.PetImage", "PetId");
            AddForeignKey("dbo.PetImage", "PetId", "dbo.Pet", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PetImage", "PetId", "dbo.Pet");
            DropIndex("dbo.PetImage", new[] { "PetId" });
            DropColumn("dbo.PetImage", "IsDisplayPic");
        }
    }
}
