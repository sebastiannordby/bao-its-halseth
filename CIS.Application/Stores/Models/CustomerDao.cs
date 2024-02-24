using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Stores.Models
{
    public sealed class CustomerDao
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public required string Name { get; set; }
        public string? ContactPersonName { get; set; }
        public string? ContactPersonEmailAddress { get; set; }
        public string? ContactPersonPhoneNumber { get; set; }
        public Guid? CustomerGroupId { get; set; }
        public bool IsActive { get; set; }
    }
}
