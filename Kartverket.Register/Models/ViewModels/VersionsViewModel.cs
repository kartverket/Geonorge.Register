using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ImageResizer.Util;
using Resources;

namespace Kartverket.Register.Models.ViewModels
{
    public class VersionsViewModel
    {
        public RegisterItemV2ViewModel CurrentVersion { get; set; }
        public ICollection<RegisterItemV2ViewModel> HistoricalVersions { get; set; }
        public ICollection<RegisterItemV2ViewModel> SuggestedVersions { get; set; }

        public VersionsViewModel(VersionsItem versionsItem)
        {
            HistoricalVersions = new List<RegisterItemV2ViewModel>();
            SuggestedVersions = new List<RegisterItemV2ViewModel>();

            if (versionsItem.currentVersion is Document document)
            {
                CurrentVersion = new DocumentViewModel(document);
                foreach (Document historicalversion in versionsItem.historical)
                {
                    HistoricalVersions.Add(new DocumentViewModel(historicalversion));
                }
                foreach (Document suggestedVersion in versionsItem.suggestions)
                {
                    SuggestedVersions.Add(new DocumentViewModel(suggestedVersion));
                }
            }
            else
            {
                CurrentVersion = new RegisterItemV2ViewModel(versionsItem.currentVersion);
                foreach (var historicalversion in versionsItem.historical)
                {
                    HistoricalVersions.Add(new RegisterItemV2ViewModel(historicalversion));
                }
                foreach (var suggestedVersion in versionsItem.suggestions)
                {
                    SuggestedVersions.Add(new RegisterItemV2ViewModel(suggestedVersion));
                }
            }
        }
    }
}