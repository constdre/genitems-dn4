namespace ASProject.Migrations_SystemContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPetTable : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.PetImage", newName: "Pet");
            CreateTable(
                "dbo.Constraint",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PetId = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pet", t => t.PetId, cascadeDelete: true)
                .Index(t => t.PetId);
            
            CreateTable(
                "dbo.PetImage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PetId = c.Int(nullable: false),
                        Image = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Pet", "Breed", c => c.String());
            DropColumn("dbo.Pet", "Consts");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Pet", "Consts", c => c.String());
            DropForeignKey("dbo.Constraint", "PetId", "dbo.Pet");
            DropIndex("dbo.Constraint", new[] { "PetId" });
            DropColumn("dbo.Pet", "Breed");
            DropTable("dbo.PetImage");
            DropTable("dbo.Constraint");
            RenameTable(name: "dbo.Pet", newName: "PetImage");
        }
    }
}
