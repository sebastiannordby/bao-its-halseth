using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Orders.Contracts
{
    public class SalesOrderView
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public string? AlternateNumber { get; set; }
        public DateOnly OrderDate { get; set; }
        public string? Reference { get; set; }

        public DateOnly DeliveredDate { get; set; }

        public int StoreNumber { get; set; }
        public required string StoreName { get; set; }

        public int CustomerNumber { get; set; }
        public required string CustomerName { get; set; }

        public bool IsDeleted { get; set; }
    }
}
