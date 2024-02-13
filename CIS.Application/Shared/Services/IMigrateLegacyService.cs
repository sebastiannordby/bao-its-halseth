using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Shared.Services
{
    public interface IMigrateLegacyService<TModel>
        where TModel : class
    {
        Task Migrate(Func<string, Task> log);
    }
}
