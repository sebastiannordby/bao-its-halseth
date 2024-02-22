using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIS.Library.Stores.Models;

namespace CIS.Library.Stores.Repositories
{
    public interface IStoreService
    {
        Task<IReadOnlyCollection<StoreView>> List();
    }
}
