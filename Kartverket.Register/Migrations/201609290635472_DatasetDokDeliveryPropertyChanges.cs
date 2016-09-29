namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DatasetDokDeliveryPropertyChanges : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.RegisterItems", name: "dokDeliveryDistributionAreaStatusId", newName: "dokDeliveryAtomFeedStatusId");
            RenameColumn(table: "dbo.RegisterItems", name: "dokDeliveryGeodataLawStatusId", newName: "dokDeliveryGmlRequirementsStatusId");
            RenameColumn(table: "dbo.RegisterItems", name: "dokDeliveryServiceAlertStatusId", newName: "dokDeliverySosiRequirementsStatusId");
            RenameIndex(table: "dbo.RegisterItems", name: "IX_dokDeliveryServiceAlertStatusId", newName: "IX_dokDeliverySosiRequirementsStatusId");
            RenameIndex(table: "dbo.RegisterItems", name: "IX_dokDeliveryGeodataLawStatusId", newName: "IX_dokDeliveryGmlRequirementsStatusId");
            RenameIndex(table: "dbo.RegisterItems", name: "IX_dokDeliveryDistributionAreaStatusId", newName: "IX_dokDeliveryAtomFeedStatusId");
            AddColumn("dbo.RegisterItems", "dokDeliverySosiRequirementsStatusNote", c => c.String());
            AddColumn("dbo.RegisterItems", "dokDeliveryGmlRequirementsStatusNote", c => c.String());
            AddColumn("dbo.RegisterItems", "dokDeliveryAtomFeedStatusNote", c => c.String());
            DropColumn("dbo.RegisterItems", "dokDeliveryDistributionAreaStatusNote");
            DropColumn("dbo.RegisterItems", "dokDeliveryServiceAlertStatusNote");
            DropColumn("dbo.RegisterItems", "dokDeliveryGeodataLawStatusNote");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RegisterItems", "dokDeliveryGeodataLawStatusNote", c => c.String());
            AddColumn("dbo.RegisterItems", "dokDeliveryServiceAlertStatusNote", c => c.String());
            AddColumn("dbo.RegisterItems", "dokDeliveryDistributionAreaStatusNote", c => c.String());
            DropColumn("dbo.RegisterItems", "dokDeliveryAtomFeedStatusNote");
            DropColumn("dbo.RegisterItems", "dokDeliveryGmlRequirementsStatusNote");
            DropColumn("dbo.RegisterItems", "dokDeliverySosiRequirementsStatusNote");
            RenameIndex(table: "dbo.RegisterItems", name: "IX_dokDeliveryAtomFeedStatusId", newName: "IX_dokDeliveryDistributionAreaStatusId");
            RenameIndex(table: "dbo.RegisterItems", name: "IX_dokDeliveryGmlRequirementsStatusId", newName: "IX_dokDeliveryGeodataLawStatusId");
            RenameIndex(table: "dbo.RegisterItems", name: "IX_dokDeliverySosiRequirementsStatusId", newName: "IX_dokDeliveryServiceAlertStatusId");
            RenameColumn(table: "dbo.RegisterItems", name: "dokDeliverySosiRequirementsStatusId", newName: "dokDeliveryServiceAlertStatusId");
            RenameColumn(table: "dbo.RegisterItems", name: "dokDeliveryGmlRequirementsStatusId", newName: "dokDeliveryGeodataLawStatusId");
            RenameColumn(table: "dbo.RegisterItems", name: "dokDeliveryAtomFeedStatusId", newName: "dokDeliveryDistributionAreaStatusId");
        }
    }
}
