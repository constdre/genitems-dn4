namespace ASProject.Migrations_SystemContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPetBreed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pet", "Breed", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Pet", "Breed");
        }
    }
}
