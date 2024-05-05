namespace CIS.Application.Features
{
    public interface INotifyClientService
    {
        Task SendPlainText(string text);
    }
}
