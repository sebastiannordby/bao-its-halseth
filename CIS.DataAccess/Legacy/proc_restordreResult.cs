﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CIS.DataAccess.Legacy
{
    public partial class proc_restordreResult
    {
        public DateTime? Dato { get; set; }
        public int Butikknr { get; set; }
        public string Butikknavn { get; set; }
        public string Ordreref { get; set; }
        public int? OrdrAnt { get; set; }
        public int? LevAnt { get; set; }
        public int? RestAnt { get; set; }
        public decimal? Restverdi { get; set; }
        public int? AntVarenr { get; set; }
    }
}
