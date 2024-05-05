namespace CIS.Application.Shared.Services
{
    public interface IExecuteImportFromShopify<TModel>
        where TModel : class
    {
        Task ExecuteShopifyImport(CancellationToken cancellationToken = default);
    }
}
