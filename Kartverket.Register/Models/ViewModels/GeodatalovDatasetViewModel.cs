using Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Kartverket.Register.Models.ViewModels
{
    public class GeodatalovDatasetViewModel : DatasetViewModel
    {
        public string InspireTheme { get; set; }
        public bool Dok { get; set; }
        public bool NationalDataset { get; set; }
        public bool Plan { get; set; }
        public bool Geodatalov { get; set; }

        [Display(Name = "Metadata:")]
        public string MetadataStatus { get; set; }
        public string MetadataNote { get; set; }
        public bool MetadataAutoUpdate { get; set; }

        [Display(Name = "Produktspesifikasjon:")]
        public string ProductSpesificationStatus { get; set; }
        public string ProduktspesifikasjonNote { get; set; }
        public bool ProduktspesifikasjonAutoUpdate { get; set; }

        [Display(Name = "SOSI data:")]
        public string SosiDataStatus { get; set; }
        public string SosiDataNote { get; set; }
        public bool SosiDataAutoUpdate { get; set; }

        [Display(Name = "GML data")]
        public string GmlDataStatus { get; set; }
        public string GmlDataNote { get; set; }
        public bool GmlDataAutoUpdate { get; set; }

        [Display(Name = "WMS")]
        public string WmsStatus { get; set; }
        public string WmsNote { get; set; }
        public bool WmsAutoUpdate { get; set; }

        [Display(Name = "WFS")]
        public string WfsStatus { get; set; }
        public string WfsNote { get; set; }
        public bool WfsAutoUpdate { get; set; }

        [Display(Name = "Atom feed")]
        public string AtomFeedStatus { get; set; }
        public string AtomFeedNote { get; set; }
        public bool AtomFeedAutoUpdate { get; set; }

        [Display(Name = "Felles Status")]
        public string CommonStatus { get; set; }
        public string CommonNote { get; set; }
        public bool CommonAutoUpdate { get; set; }

        public GeodatalovDatasetViewModel(GeodatalovDataset item)
        {
            Update(item);
        }

        public GeodatalovDatasetViewModel()
        {
        }

        public void Update(GeodatalovDataset geodatalovDataset)
        {
            if (geodatalovDataset != null)
            {
                InspireTheme = geodatalovDataset.InspireTheme;
                Dok = geodatalovDataset.Dok;
                NationalDataset = geodatalovDataset.NationalDataset;
                Plan = geodatalovDataset.Plan;
                Geodatalov = geodatalovDataset.Geodatalov;

                if (geodatalovDataset.MetadataStatus != null)
                {
                    MetadataStatus = geodatalovDataset.MetadataStatus.StatusId;
                    MetadataNote = geodatalovDataset.MetadataStatus.Note;
                    MetadataAutoUpdate = geodatalovDataset.MetadataStatus.AutoUpdate;
                }
                if (geodatalovDataset.ProductSpesificationStatus != null)
                {
                    ProductSpesificationStatus = geodatalovDataset.ProductSpesificationStatus.StatusId;
                    ProduktspesifikasjonNote = geodatalovDataset.ProductSpesificationStatus.Note;
                    ProduktspesifikasjonAutoUpdate = geodatalovDataset.ProductSpesificationStatus.AutoUpdate;
                }
                if (geodatalovDataset.SosiDataStatus != null)
                {
                    SosiDataStatus = geodatalovDataset.SosiDataStatus.StatusId;
                    SosiDataNote = geodatalovDataset.SosiDataStatus.Note;
                    SosiDataAutoUpdate = geodatalovDataset.SosiDataStatus.AutoUpdate;
                }
                if (geodatalovDataset.GmlDataStatus != null)
                {
                    GmlDataStatus = geodatalovDataset.GmlDataStatus.StatusId;
                    GmlDataNote = geodatalovDataset.GmlDataStatus.Note;
                    GmlDataAutoUpdate = geodatalovDataset.GmlDataStatus.AutoUpdate;
                }
                if (geodatalovDataset.WmsStatus != null)
                {
                    WmsStatus = geodatalovDataset.WmsStatus.StatusId;
                    WmsNote = geodatalovDataset.WmsStatus.Note;
                    WmsAutoUpdate = geodatalovDataset.WmsStatus.AutoUpdate;
                }
                if (geodatalovDataset.WfsStatus != null)
                {
                    WfsStatus = geodatalovDataset.WfsStatus.StatusId;
                    WfsNote = geodatalovDataset.WfsStatus.Note;
                    WfsAutoUpdate = geodatalovDataset.WfsStatus.AutoUpdate;
                }
                if (geodatalovDataset.AtomFeedStatus != null)
                {
                    AtomFeedStatus = geodatalovDataset.AtomFeedStatus.StatusId;
                    AtomFeedNote = geodatalovDataset.AtomFeedStatus.Note;
                    AtomFeedAutoUpdate = geodatalovDataset.AtomFeedStatus.AutoUpdate;
                }
                if (geodatalovDataset.CommonStatus != null)
                {
                    CommonStatus = geodatalovDataset.CommonStatus.StatusId;
                    CommonNote = geodatalovDataset.CommonStatus.Note;
                    CommonAutoUpdate = geodatalovDataset.CommonStatus.AutoUpdate;
                }

                UpdateDataset(geodatalovDataset);
            }
        }

        public string GetGeodatalovDatasetEditUrl()
        {
            if (Register.parentRegister == null)
            {
                return "/geodatalov/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/rediger";
            }
            return "/geodatalov/" + Register.parentRegister.seoname + "/" + Register.owner.seoname + "/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/rediger";
        }

        public string GetGeodatalovDatasetDeleteUrl()
        {
            if (Register.parentRegister == null)
            {
                return "/geodatalov/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett";
            }
            return "/geodatalov/" + Register.parentRegister.seoname + "/" + Register.owner.seoname + "/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett";
        }
    }
}