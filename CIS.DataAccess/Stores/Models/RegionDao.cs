using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.DataAccess.Stores.Models
{
    public sealed class RegionDao
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public required string Name { get; set; }
    }
}
