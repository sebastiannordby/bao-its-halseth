﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Models
{
    public class StoreMostBoughtView
    {
        public string StoreName { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalBoughtFor { get; set; }
    }
}
