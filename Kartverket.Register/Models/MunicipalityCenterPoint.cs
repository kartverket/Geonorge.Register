namespace Kartverket.Register.Models
{
    public class MunicipalityCenterPoint
    {

        public MunicipalityCenterPoint(string coordinateX, string coordinateY)
        {
            CoordinateX = coordinateX;
            CoordinateY = coordinateY;
        }

        public string CoordinateX { get; set; }
        public string CoordinateY { get; set; }
    }
}