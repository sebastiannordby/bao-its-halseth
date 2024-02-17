using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Shared.Services
{
    public interface IMigrationMapper<TIn, TOut>
    {
        Task<TOut> Map(TIn input);
    }
}
