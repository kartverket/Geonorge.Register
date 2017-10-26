using System.ComponentModel.DataAnnotations;
using Resources;

namespace Kartverket.Register.Models.ViewModels
{
    public class DocumentViewModel : RegisterItemV2ViewModel
    {
        public DocumentViewModel(Document document)
        {
            DocumentUrl = document.documentUrl;
            ApplicationSchema = document.ApplicationSchema;
            GmlApplicationSchema = document.GMLApplicationSchema;
            CartographyFile = document.CartographyFile;
            ApprovalDocument = document.approvalDocument;
            ApprovalReference = document.approvalReference;

            new RegisterItemV2ViewModel(document);
        }

        public string DocumentUrl { get; set; }

        public string ApplicationSchema { get; set; }
        public string GmlApplicationSchema { get; set; }
        public string CartographyFile { get; set; }
        public string ApprovalDocument { get; set; }
        public string ApprovalReference { get; set; }


        [Display(Name = "Thumbnail", ResourceType = typeof(Documents))]
        public string Thumbnail { get; set; }
    }
}