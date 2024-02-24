using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Shared
{
    public sealed class ApplicationUserView : ApplicationUser
    {
        public int? CustomerNumber { get; set; }
        public string? CustomerName { get; set; }
    }
}
