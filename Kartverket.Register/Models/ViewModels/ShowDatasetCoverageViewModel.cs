namespace Kartverket.Register.Models.ViewModels
{
    public class ShowDatasetCoverageViewModel
    {
        public ShowDatasetCoverageViewModel()
        {
            StateBoundingBox = new BoundingBoxViewModel();
        }

        public string StateName { get; set; }
        public BoundingBoxViewModel StateBoundingBox { get; set; }
        public string DatasetName { get; set; }
        public string DatasetWmsIdentifier { get; set; }
    }
}