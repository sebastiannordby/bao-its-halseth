using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Shared.Models
{
    public class MigrationTask
    {
        public Guid Id { get; set; }
        public TaskType Type { get; set; }
        public bool Executed { get; set; }

        public enum TaskType
        {
            Customers = 0,
            Products = 1,
            SalesOrders = 2,
            SalesOrderStatistics = 3
        }
    }
}
