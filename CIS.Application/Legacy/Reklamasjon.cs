﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace CIS.Application.Legacy;

public partial class Reklamasjon
{
    public decimal Id { get; set; }

    public int? Butikknr { get; set; }

    public DateTime? Dato { get; set; }

    public int? Varenr { get; set; }

    public int? Antall { get; set; }

    public string Feilbeskrivelse { get; set; }

    public bool? Godkjent { get; set; }

    public DateTime? GodkjentReturDato { get; set; }

    public DateTime? KreditertDato { get; set; }
}