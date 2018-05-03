namespace Kartverket.Register.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableInspireMonitorings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InspireMonitorings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Date = c.DateTime(nullable: false),
                        NumberOfDatasetsByAnnexI = c.Int(nullable: false),
                        NumberOfDatasetsByAnnexII = c.Int(nullable: false),
                        NumberOfDatasetsByAnnexIII = c.Int(nullable: false),
                        NumberOfDatasetsByAnnexIWithMetadata = c.Int(nullable: false),
                        NumberOfDatasetsByAnnexIIWithMetadata = c.Int(nullable: false),
                        NumberOfDatasetsByAnnexIIIWithMetadata = c.Int(nullable: false),
                        NumberOfDatasetsByAnnexIWhereMetadataStatusIsgood = c.Int(nullable: false),
                        NumberOfDatasetsByAnnexIIWhereMetadataStatusIsgood = c.Int(nullable: false),
                        NumberOfDatasetsByAnnexIIIWhereMetadataStatusIsgood = c.Int(nullable: false),
                        NumberOfServicesWhereMetadataStatusIsgood = c.Int(nullable: false),
                        NumberOfDatasetsWithMetadata = c.Int(nullable: false),
                        NumberOfServicesWithMetadata = c.Int(nullable: false),
                        NumberOfDatasetsRegisteredInADiscoveryService = c.Int(nullable: false),
                        NumberOfServicesRegisteredInADiscoveryService = c.Int(nullable: false),
                        NumberOfServicesByServiceTypeDownload = c.Int(nullable: false),
                        NumberOfServicesByServiceTypeView = c.Int(nullable: false),
                        NumberOfServicesByServiceTypeDiscovery = c.Int(nullable: false),
                        NumberOfServicesByServiceTypeInvoke = c.Int(nullable: false),
                        NumberOfServicesByServiceTypeTransformation = c.Int(nullable: false),
                        NumberOfSdS = c.Int(nullable: false),
                        NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue = c.Int(nullable: false),
                        NumberOfServicesByServiceTypeViewWhereConformityIsTrue = c.Int(nullable: false),
                        NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue = c.Int(nullable: false),
                        NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue = c.Int(nullable: false),
                        NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue = c.Int(nullable: false),
                        NumberOfCallsByServiceTypeDiscovery = c.Int(nullable: false),
                        NumberOfCallsByServiceTypeView = c.Int(nullable: false),
                        NumberOfCallsByServiceTypeDownload = c.Int(nullable: false),
                        NumberOfCallsByServiceTypeTransformation = c.Int(nullable: false),
                        NumberOfCallsByServiceTypeInvoke = c.Int(nullable: false),
                        NumberOfDatasetsAvailableThroughViewANDDownloadService = c.Int(nullable: false),
                        NumberOfDatasetsAvailableThroughDownloadService = c.Int(nullable: false),
                        NumberOfDatasetsAvailableThroughViewService = c.Int(nullable: false),
                        NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata = c.Int(nullable: false),
                        NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata = c.Int(nullable: false),
                        NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata = c.Int(nullable: false),
                        AccumulatedCurrentAreaByAnnexI = c.Double(nullable: false),
                        AccumulatedCurrentAreaByAnnexII = c.Double(nullable: false),
                        AccumulatedCurrentAreaByAnnexIII = c.Double(nullable: false),
                        AccumulatedRelevantAreaByAnnexI = c.Double(nullable: false),
                        AccumulatedRelevantAreaByAnnexII = c.Double(nullable: false),
                        AccumulatedRelevantAreaByAnnexIII = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.InspireMonitorings");
        }
    }
}
