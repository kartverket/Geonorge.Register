using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using WebGrease.Css.Extensions;

namespace Kartverket.Register.Models.Api
{
    [XmlRoot("Monitoring")]
    public class InspireDatasetRegistery
    {
        [XmlAttributeAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string schemaLocation = "http://inspire.jrc.ec.europa.eu/monitoringreporting/monitoring http://dd.eionet.europa.eu/schemas/inspire-monitoring/monitoring.xsd";

        public Date DocumentYear { get; set; }
        public string Memberstate { get; set; }
        public MonitoringMD MonitoringMD { get; set; }
        public Indicators Indicators{ get; set; }

        [XmlArray("RowData")]
        [XmlArrayItem("SpatialDataSet")]
        public List<InspireDataset> InspireDatasets { get; set; }

        public InspireDatasetRegistery()
        {
            InspireDatasets = new List<InspireDataset>();
        }

        public InspireDatasetRegistery(Models.Api.Register register)
        {
            DocumentYear = new Date();
            Memberstate = "NO";
            MonitoringMD = new MonitoringMD();
            Indicators = new Indicators();
            InspireDatasets = GetInspireDatasets(register.containeditems);
        }

        private List<InspireDataset> GetInspireDatasets(ICollection<Registeritem> registerItems)
        {
            var inspireDatasets = new List<InspireDataset>();

            foreach (var inspireDataset in registerItems)
            {
                inspireDatasets.Add(new InspireDataset(inspireDataset));
            }

            return inspireDatasets;
        }
    }

    public class MonitoringMD
    {
        public string OrganizationName { get; set; }
        public string Email { get; set; }
        public Date MonitoringDate { get; set; }
        public string Language { get; set; }

        public MonitoringMD()
        {
            OrganizationName = "Kartverket";
            Email = "post@norgedigitalt.no";
            MonitoringDate = new Date();
            Language = "nor";
        }
    }

    public class Indicators
    {
        public NnConformityIndicators NnConformityIndicators { get; set; }
        public GeoCoverageIndicators GeoCoverageIndicators { get; set; }
        public UseNNindicators UseNNindicators { get; set; }
        public MetadataExistenceIndicators MetadataExistenceIndicators { get; set; }
        public DiscoveryMetadataIndicators DiscoveryMetadataIndicators { get; set; }
        public ViewDownloadAccessibilityIndicators ViewDownloadAccessibilityIndicators { get; set; }
        public SpatialDataAndService SpatialDataAndService { get; set; }
        public MetadataConformityIndicators MetadataConformityIndicators { get; set; }
        public SdsConformantIndicators SdsConformantIndicators { get; set; }

        public Indicators()
        {
            NnConformityIndicators = new NnConformityIndicators();
            GeoCoverageIndicators = new GeoCoverageIndicators();
            UseNNindicators = new UseNNindicators();
            MetadataExistenceIndicators = new MetadataExistenceIndicators();
            DiscoveryMetadataIndicators = new DiscoveryMetadataIndicators();
            ViewDownloadAccessibilityIndicators = new ViewDownloadAccessibilityIndicators();
            SpatialDataAndService = new SpatialDataAndService();
            MetadataConformityIndicators = new MetadataConformityIndicators();
            SdsConformantIndicators = new SdsConformantIndicators();
        }
    }

    public class SdsConformantIndicators
    {
        public string DSi21 { get; set; }
        public string DSi22 { get; set; }
        public string DSi23 { get; set; }
        public string DSi2 { get; set; }
        public SdsConformant SdsConformant { get; set; }

        public SdsConformantIndicators()
        {
            DSi21 = "";
            DSi22 = "";
            DSi23 = "";
            DSi2 = "";
            SdsConformant = new SdsConformant();
        }
    }

    public class SdsConformant
    {
        public string DSv21 { get; set; }
        public string DSv22 { get; set; }
        public string DSv23 { get; set; }
        public string DSv2 { get; set; }

        public SdsConformant()
        {
            DSv21 = "";
            DSv22 = "";
            DSv23 = "";
            DSv2 = "";
        }
    }

    public class MetadataConformityIndicators
    {
        public string MDi21 { get; set; }
        public string MDi22 { get; set; }
        public string MDi23 { get; set; }
        public string MDi24 { get; set; }
        public string MDi2 { get; set; }
        public MetadataConformity MetadataConformity { get; set; }

        public MetadataConformityIndicators()
        {
            MDi21 = "";
            MDi22 = "";
            MDi23 = "";
            MDi24 = "";
            MDi2 = "";
            MetadataConformity = new MetadataConformity();
        }
    }

    public class MetadataConformity
    {
        public string MDi21 { get; set; }
        public string MDi22 { get; set; }
        public string MDi23 { get; set; }
        public string MDv2_DS { get; set; }
        public string MDv24 { get; set; }

        public MetadataConformity()
        {
            MDi21 = "";
            MDi22 = "";
            MDi23 = "";
            MDv2_DS = "";
            MDv24 = "";
        }
    }

    public class SpatialDataAndService  
    {
        public string DSv_Num1 { get; set; }
        public string DSv_Num2 { get; set; }
        public string DSv_Num3 { get; set; }
        public string DSv_Num { get; set; }
        public string SDSv_Num { get; set; }
        public string NSv_NumDiscServ { get; set; }
        public string NSv_NumDownServ { get; set; }
        public string NSv_NumInvkServ { get; set; }
        public string NSv_NumAllServ { get; set; }
        public string NSv_NumTransfServ { get; set; }

        public SpatialDataAndService()
        {
            DSv_Num1 = "";
            DSv_Num2 = "";
            DSv_Num3 = "";
            DSv_Num = "";
            SDSv_Num = "";
            NSv_NumDiscServ = "";
            NSv_NumDownServ = "";
            NSv_NumInvkServ = "";
            NSv_NumAllServ = "";
            NSv_NumTransfServ = "";
        }
    }

    public class ViewDownloadAccessibilityIndicators
    {
        public string NSi21 { get; set; }
        public string NSi22 { get; set; }
        public string NSi2 { get; set; }
        public ViewDownloadAccessibility ViewDownloadAccessibility { get; set; }

        public ViewDownloadAccessibilityIndicators()
        {
            NSi21 = "";
            NSi22 = "";
            NSi2 = "";
            ViewDownloadAccessibility = new ViewDownloadAccessibility();
        }
    }

    public class ViewDownloadAccessibility
    {
        public string NSv21 { get; set; }
        public string NSv22 { get; set; }
        public string NSv23 { get; set; }

        public ViewDownloadAccessibility()
        {
            NSv21 = "";
            NSv22 = "";
            NSv23 = "";
        }
    }

    public class DiscoveryMetadataIndicators
    {
        public string NSi11 { get; set; }
        public string NSi12 { get; set; }
        public string NSi1 { get; set; }
        public DiscoveryMetadata DiscoveryMetadata { get; set; }

        public DiscoveryMetadataIndicators()
        {
            NSi11 = "";
            NSi12 = "";
            NSi1 = "";
            DiscoveryMetadata = new DiscoveryMetadata();
        }
    }

    public class DiscoveryMetadata
    {
        public string NSv11 { get; set; }
        public string NSv12 { get; set; }

        public DiscoveryMetadata()
        {
            NSv11 = "";
            NSv12 = "";
        }
    }

    public class MetadataExistenceIndicators
    {
        public string MDi11 { get; set; }
        public string MDi12 { get; set; }
        public string MDi13 { get; set; }
        public string MDi14 { get; set; }
        public string MDi1 { get; set; }
        public MetadataExistence MetadataExistence { get; set; }

        public MetadataExistenceIndicators()
        {
            MDi11 = "";
            MDi12 = "";
            MDi13 = "";
            MDi14 = "";
            MDi1 = "";
            MetadataExistence = new MetadataExistence();
        }
    }

    public class MetadataExistence
    {
        public string MDv11 { get; set; }
        public string MDv12 { get; set; }
        public string MDv13 { get; set; }
        public string MDv1_DS { get; set; }
        public string MDv14 { get; set; }

        public MetadataExistence()
        {
            MDv11 = "";
            MDv12 = "";
            MDv13 = "";
            MDv1_DS = "";
            MDv14 = "";
        }
    }

    public class UseNNindicators
    {
        public string NSi31 { get; set; }
        public string NSi32 { get; set; }
        public string NSi33 { get; set; }
        public string NSi34 { get; set; }
        public string NSi35 { get; set; }
        public string NSi3 { get; set; }
        public UseNN UseNN { get; set; }

        public UseNNindicators()
        {
            NSi31 = "0";
            NSi32 = "0";
            NSi33 = "0";
            NSi34 = "0";
            NSi35 = "0";
            NSi3 = "0";
            UseNN = new UseNN();
        }
    }

    public class UseNN
    {
        public string NSv31 { get; set; }
        public string NSv32 { get; set; }
        public string NSv33 { get; set; }
        public string NSv34 { get; set; }
        public string NSv35 { get; set; }
        public string NSv3 { get; set; }

        public UseNN()
        {
            NSv31 = "";
            NSv32 = "";
            NSv33 = "";
            NSv34 = "";
            NSv35 = "";
            NSv3 = "";
        }
    }

    public class GeoCoverageIndicators
    {
        public string DSi11 { get; set; }
        public string DSi12 { get; set; }
        public string DSi13 { get; set; }
        public string DSi1 { get; set; }
        public GeoCoverageSDS GeoCoverageSDS { get; set; }
        

        public GeoCoverageIndicators()
        {
            DSi11 = "0";
            DSi12 = "0";
            DSi13 = "0";
            DSi1 = "0";
            GeoCoverageSDS = new GeoCoverageSDS();
        }
    }

    public class GeoCoverageSDS
    {
        public string DSv11_ActArea { get; set; }
        public string DSv12_ActArea { get; set; }
        public string DSv13_ActArea { get; set; }
        public string DSv1_ActArea { get; set; }

        public string DSv11_RelArea { get; set; }
        public string DSv12_RelArea { get; set; }
        public string DSv13_RelArea { get; set; }
        public string DSv1_RelArea { get; set; }

        public GeoCoverageSDS()
        {
            DSv11_ActArea = "0";
            DSv12_ActArea = "0";
            DSv13_ActArea = "0";
            DSv1_ActArea = "0";
            DSv11_RelArea = "0";
            DSv12_RelArea = "0";
            DSv13_RelArea = "0";
            DSv1_RelArea = "0";
        }
    }

    public class NnConformityIndicators
    {
        public int NSi41 { get; set; }
        public int NSi42 { get; set; }
        public int NSi43 { get; set; }
        public int NSi44 { get; set; }
        public int NSi45 { get; set; }
        public int NSi4 { get; set; }
        public NnConformity NnConformity { get; set; }

        public NnConformityIndicators()
        {
            NSi41 = 0;
            NSi42 = 0;
            NSi43 = 0;
            NSi44 = 0;
            NSi45 = 0;
            NSi4 = 0;
            NnConformity = new NnConformity();
        }
    }

    public class NnConformity
    {
        public int NSv41 { get; set; }
        public int NSv42 { get; set; }
        public int NSv43 { get; set; }
        public int NSv44 { get; set; }
        public int NSv45 { get; set; }
        public int NSv4 { get; set; }

        public NnConformity()
        {
            NSv41 = 0;
            NSv42 = 0;
            NSv43 = 0;
            NSv44 = 0;
            NSv45 = 0;
            NSv4 = 0;
        }
    }

    public class Date
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public Date()
        {
            Day = DateTime.Now.Day;
            Month = DateTime.Now.Month;
            Year = DateTime.Now.Year;
        }
    }

    [XmlRoot("SpatialDataSet")]
    public class InspireDataset
    {
        public InspireDataset()
        {
        }

        public InspireDataset(Registeritem registerItem)
        {
            if (registerItem != null)
            {
                Name = registerItem.label;
                RespAuthority = registerItem.owner;
                Uuid = registerItem.uuid.ToString();
                Theme = new InspireTheme(registerItem.theme);
                Coverage = new InspireCoverage();
                MdDataSetExistence = new MdDataSetExistence();
            }
        }


        public string Name { get; set; }
        public string RespAuthority { get; set; }
        public string Uuid { get; set; }
        public InspireTheme Theme { get; set; }
        public InspireCoverage Coverage { get; set; }
        public MdDataSetExistence MdDataSetExistence { get; set; }
    }

    public class InspireTheme
    {
        public string AnnexI { get; set; }

        public InspireTheme()
        {
        }

        public InspireTheme(string theme)
        {
            AnnexI = theme;
        }
    }

    public class InspireCoverage
    {
        public string RelevantArea { get; set; }
        public string ActualArea { get; set; }

        public InspireCoverage()
        {
            RelevantArea = "RelevantArea";
            ActualArea = "ActualArea";
        }

    }

    public class MdDataSetExistence
    {
        public MdAccessibility MdAccessibility { get; set; }
        public IrConformity IrConformity { get; set; }

        public MdDataSetExistence()
        {
            MdAccessibility = new MdAccessibility();
            IrConformity = new IrConformity();
        }

    }

    public class MdAccessibility
    {
        public bool Discovery { get; set; }
        public string DiscoveryUuid { get; set; }
        public bool View { get; set; }
        public string ViewUuid { get; set; }
        public bool Download { get; set; }
        public string DownloadUuid { get; set; }
        public bool ViewDownload { get; set; }

        public MdAccessibility()
        {
            Discovery = true;
            DiscoveryUuid = "DiscoveryUuid";
            View = true;
            ViewUuid = "ViewUuid";
            Download = true;
            DownloadUuid = "DownloadUuid";
            ViewDownload = true;
        }
    }

    public class IrConformity
    {
        public bool StructureCompliance { get; set; }

        public IrConformity()
        {
            StructureCompliance = true;
        }
    }
}