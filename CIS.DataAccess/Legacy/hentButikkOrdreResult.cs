﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CIS.DataAccess.Legacy
{
    public partial class hentButikkOrdreResult
    {
        [Column("ID", TypeName = "decimal(18,0)")]
        public decimal ID { get; set; }
        public DateTime? dato { get; set; }
        public int? butikknr { get; set; }
        public string varenr_lev { get; set; }
        public string Beskrivelse { get; set; }
        public string ordreref { get; set; }
        public int? antall { get; set; }
    }
}
