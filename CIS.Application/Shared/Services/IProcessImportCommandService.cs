using CIS.Application.Shared.Contracts;

namespace CIS.Library.Shared.Services
{
    public interface IProcessImportCommandService<TCommand>
        where TCommand : CISImportCommand
    {
        Task<bool> Import(TCommand command, CancellationToken cancellationToken);
    }
}
