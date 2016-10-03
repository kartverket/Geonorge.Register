namespace Kartverket.Register.Models
{
    /// <summary>
    /// API methods should rely on this interface to convert internal organization models to api models.
    /// </summary>
    interface IOrganizationConverter
    {
        void Convert(Organization input);
    }
}
