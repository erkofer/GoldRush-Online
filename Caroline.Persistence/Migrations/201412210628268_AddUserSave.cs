namespace Caroline.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserSave : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SaveData = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "GameId", c => c.Int(nullable: false));
            CreateIndex("dbo.AspNetUsers", "GameId");
            AddForeignKey("dbo.AspNetUsers", "GameId", "dbo.Games", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "GameId", "dbo.Games");
            DropIndex("dbo.AspNetUsers", new[] { "GameId" });
            DropColumn("dbo.AspNetUsers", "GameId");
            DropTable("dbo.Games");
        }
    }
}
