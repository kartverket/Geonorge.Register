using Kartverket.Register.Models;


namespace Kartverket.Register.Services.Search
{
    public interface ISearchService
    {
        SearchResult Search(SearchParameters parameters);
        Models.Register Search(Models.Register register, string text);
    }
}
