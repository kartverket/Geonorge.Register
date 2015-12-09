namespace Kartverket.Register.Models.ViewModels
{
    public class BoundingBoxViewModel
    {
        public BoundingBoxViewModel() : this("57", "2", "72", "33")
        {
            
        }
            
        public BoundingBoxViewModel(string south, string west, string north, string east)
        {
            South = south;
            West = west;
            North = north;
            East = east;
        }

        public string South { get; set; }
        public string West { get; set; }
        public string North { get; set; }
        public string East { get; set; }
    }
}