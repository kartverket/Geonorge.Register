
namespace Kartverket.Register.Services
{
    public interface RegisterIndexer
    {
        void RunIndexing();
        void RunIndexingOn(string systemID);
        void RunReIndexing();
    }
}