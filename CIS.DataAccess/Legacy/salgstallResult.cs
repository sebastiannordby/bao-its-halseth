﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CIS.Application.Legacy
{
    public partial class salgstallResult
    {
        public int? Butikknr { get; set; }
        public string Butikknavn { get; set; }
        public string VarenrLev { get; set; }
        public int? Varenr { get; set; }
        public string Varebeskrivelse_2 { get; set; }
        public int? Antall { get; set; }
        public decimal? SnittSalgsPris { get; set; }
        public decimal? SnittKostPris { get; set; }
        public decimal? SalgTilRC { get; set; }
        public decimal? VårKost { get; set; }
        public decimal? MarginKr { get; set; }
        public decimal? MargP { get; set; }
    }
}
