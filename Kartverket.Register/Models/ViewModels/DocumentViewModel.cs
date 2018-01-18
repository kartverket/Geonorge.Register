using System.ComponentModel.DataAnnotations;
using System.Text;
using Resources;

namespace Kartverket.Register.Models.ViewModels
{
    public class DocumentViewModel : RegisterItemV2ViewModel
    {
        public DocumentViewModel(Document document)
        {
            if (document != null)
            {
                DocumentUrl = document.documentUrl;
                ApplicationSchema = document.ApplicationSchema;
                GmlApplicationSchema = document.GMLApplicationSchema;
                CartographyFile = document.CartographyFile;
                ApprovalDocument = document.approvalDocument;
                ApprovalReference = document.approvalReference;
                Accepted = GetAcceptedStatus(document.Accepted);

                UpdateRegisterItem(document);
            }
        }

        private string GetAcceptedStatus(bool? documentAccepted)
        {
            return documentAccepted == true ? Registers.Accepted : Registers.Draft;
        }

        public string VersionUrl()
        {
            if (Register.parentRegister == null)
            {                
                return "/register/versjoner/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/" + VersionNumber + "/no";
            }
            else
            {
                return "/subregister/versjoner/" + Register.parentRegister.seoname + "/" + Register.parentRegister.owner.seoname + "/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/" + VersionNumber + "/no";

            }
        }

        public string EditVersionOfDocumentUrl()
        {
            return "/dokument/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/rediger?vnr=" + VersionNumber;
        }

        public string DeleteVersionOfDocumentUrl()
        {
            return "/dokument/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett?vnr=" + VersionNumber;
        }

        public string DocumentUrl { get; set; }

        public string ApplicationSchema { get; set; }
        public string GmlApplicationSchema { get; set; }
        public string CartographyFile { get; set; }
        public string ApprovalDocument { get; set; }
        public string ApprovalReference { get; set; }
        public string Accepted { get; set; }


        [Display(Name = "Thumbnail", ResourceType = typeof(Documents))]
        public string Thumbnail { get; set; }
    }
}