namespace ASProject.Migrations_SystemContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PetImageModelUpdate_AdditionalInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PetImage", "Name", c => c.String());
            AddColumn("dbo.PetImage", "Size", c => c.Int(nullable: false));
            AddColumn("dbo.PetImage", "Type", c => c.String());
            AddColumn("dbo.PetImage", "CreatedOn", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PetImage", "CreatedOn");
            DropColumn("dbo.PetImage", "Type");
            DropColumn("dbo.PetImage", "Size");
            DropColumn("dbo.PetImage", "Name");
        }
    }
}
