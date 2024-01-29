using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Library.Stores.Models
{
    public record StoreView(
        int Number,
        string Name,
        bool IsActive,
        int CustomerNumber,
        string CustomerName,
        string? CustomerContactPersonName,
        string? CustomerContactPersonEmailAddress,
        string? CustomerContactPersonPhoneNumber,
        int? CustomerCustomerGroupNumber,
        string? CustomerGroupName
    );
}
