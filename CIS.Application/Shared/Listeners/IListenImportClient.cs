namespace CIS.Application.Listeners
{
    public interface IListenImportClient 
    {
        Task Finished();
        Task ReceiveMessage(string message);
    }
}