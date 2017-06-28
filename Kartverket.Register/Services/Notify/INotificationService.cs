
namespace Kartverket.Register.Services.Notify
{
    public interface INotificationService
    {
        void SendSubmittedNotification(Models.Document document);
    }
}