using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application
{
    public class ApplicationUser : IdentityUser
    {
        public Guid? CustomerId { get; set; }
    }
}
