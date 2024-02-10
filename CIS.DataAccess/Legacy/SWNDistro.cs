﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CIS.Application.Legacy;

public partial class SWNDistro : DbContext
{
    public SWNDistro(DbContextOptions<SWNDistro> options)
        : base(options)
    {
    }

    public virtual DbSet<Bestillingsforslag> Bestillingsforslags { get; set; }

    public virtual DbSet<ButikkTemp> ButikkTemps { get; set; }

    public virtual DbSet<Butikkliste> Butikklistes { get; set; }

    public virtual DbSet<Konsept> Konsepts { get; set; }

    public virtual DbSet<KonseptAntall> KonseptAntalls { get; set; }

    public virtual DbSet<KonseptAntallTemp> KonseptAntallTemps { get; set; }

    public virtual DbSet<Kundenr> Kundenrs { get; set; }

    public virtual DbSet<Kundenrdato> Kundenrdatos { get; set; }

    public virtual DbSet<Lager> Lagers { get; set; }

    public virtual DbSet<Ordre> Ordres { get; set; }

    public virtual DbSet<OrdreFraNett> OrdreFraNetts { get; set; }

    public virtual DbSet<Pakkseddel> Pakkseddels { get; set; }

    public virtual DbSet<Reklamasjon> Reklamasjons { get; set; }

    public virtual DbSet<Salg> Salgs { get; set; }

    public virtual DbSet<SwsgTooltip> SwsgTooltips { get; set; }

    public virtual DbSet<TblCheckAutomate> TblCheckAutomates { get; set; }

    public virtual DbSet<TblLoginLog> TblLoginLogs { get; set; }

    public virtual DbSet<TblPostnr> TblPostnrs { get; set; }

    public virtual DbSet<TblVaregr1> TblVaregr1s { get; set; }

    public virtual DbSet<TblVaregr2> TblVaregr2s { get; set; }

    public virtual DbSet<TempBestilling> TempBestillings { get; set; }

    public virtual DbSet<TempImport1> TempImport1s { get; set; }

    public virtual DbSet<Varegruppe> Varegruppes { get; set; }

    public virtual DbSet<Vareinfo> Vareinfos { get; set; }

    public virtual DbSet<VareinfoBackup> VareinfoBackups { get; set; }

    public virtual DbSet<VareinfoTemp> VareinfoTemps { get; set; }

    public virtual DbSet<VareinfoTemp2> VareinfoTemp2s { get; set; }

    public virtual DbSet<Varetelling> Varetellings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bestillingsforslag>(entity =>
        {
            entity.ToTable("bestillingsforslag");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.But).HasColumnName("But#");
            entity.Property(e => e.Butikknavn)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Dato).HasColumnType("smalldatetime");
            entity.Property(e => e.Dul).HasColumnName("DUL");
            entity.Property(e => e.VareId).HasColumnName("VareID");
            entity.Property(e => e.Varebeskrivelse)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VarenrLev)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ButikkTemp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Butikk_temp");

            entity.Property(e => e.Butikknr).HasColumnName("butikknr");
            entity.Property(e => e.Konsept).HasColumnName("konsept");
        });

        modelBuilder.Entity<Butikkliste>(entity =>
        {
            entity.HasKey(e => e.Butikknr);

            entity.ToTable("Butikkliste");

            entity.Property(e => e.Butikknr).ValueGeneratedNever();
            entity.Property(e => e.Aktiv).HasDefaultValue(false);
            entity.Property(e => e.Butikknavn)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.DatoSisteordre)
                .HasColumnType("smalldatetime")
                .HasColumnName("dato_sisteordre");
            entity.Property(e => e.Epost)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Gateadresse)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.KredittSperre).HasDefaultValue(false);
            entity.Property(e => e.Kundenr).HasColumnName("kundenr");
            entity.Property(e => e.Lokasjon)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Poststed)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RegionNavn)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefon).HasColumnName("telefon");
        });

        modelBuilder.Entity<Konsept>(entity =>
        {
            entity.HasKey(e => e.Konseptnr);

            entity.ToTable("Konsept");

            entity.Property(e => e.Konseptnr).ValueGeneratedNever();
            entity.Property(e => e.Konseptnavn)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<KonseptAntall>(entity =>
        {
            entity.ToTable("KonseptAntall");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.VareId).HasColumnName("VareID");
        });

        modelBuilder.Entity<KonseptAntallTemp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("KonseptAntall_temp");
        });

        modelBuilder.Entity<Kundenr>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Kundenr");

            entity.Property(e => e.Butikknr).HasColumnName("butikknr");
            entity.Property(e => e.Kundenr1).HasColumnName("kundenr");
            entity.Property(e => e.Levid).HasColumnName("levid");
        });

        modelBuilder.Entity<Kundenrdato>(entity =>
        {
            entity.ToTable("kundenrdato");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.Aktiv)
                .HasComputedColumnSql("(case when getdate()>=[datofra] AND getdate()<=[datotil] then (1) else (0) end)", false)
                .HasColumnName("aktiv");
            entity.Property(e => e.Butikknr).HasColumnName("butikknr");
            entity.Property(e => e.Datofra).HasColumnName("datofra");
            entity.Property(e => e.Datotil).HasColumnName("datotil");
            entity.Property(e => e.Kundenr).HasColumnName("kundenr");
            entity.Property(e => e.Levid).HasColumnName("levid");
        });

        modelBuilder.Entity<Lager>(entity =>
        {
            entity.ToTable("Lager");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.Dato).HasColumnType("smalldatetime");
            entity.Property(e => e.Stkverdi).HasColumnType("money");
        });

        modelBuilder.Entity<Ordre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ordre");

            entity.ToTable("Ordre");

            entity.HasIndex(e => new { e.SendtLev, e.Dato, e.Ordreref }, "idx_ordre_20181126_1");

            entity.HasIndex(e => new { e.Butikknr, e.SendtLev, e.Dato, e.Ordreref }, "idx_ordre_20181126_2");

            entity.HasIndex(e => new { e.SendtLev, e.Ordretype }, "idx_ordre_20181126_3");

            entity.HasIndex(e => new { e.Dato, e.Antall }, "idx_ordre_dato_antall_20171011");

            entity.HasIndex(e => new { e.VareId, e.Dato }, "idx_ordre_vareid_dato_20171011");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.Antall).HasColumnName("antall");
            entity.Property(e => e.AntallLevert)
                .HasDefaultValue(0)
                .HasColumnName("antallLevert");
            entity.Property(e => e.Bedriftspakke)
                .HasDefaultValue(0)
                .HasColumnName("bedriftspakke");
            entity.Property(e => e.Butikknr).HasColumnName("butikknr");
            entity.Property(e => e.Dato)
                .HasColumnType("smalldatetime")
                .HasColumnName("dato");
            entity.Property(e => e.Ean)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("ean");
            entity.Property(e => e.Innpris)
                .HasColumnType("money")
                .HasColumnName("innpris");
            entity.Property(e => e.Inserted)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("inserted");
            entity.Property(e => e.LevId).HasColumnName("LevID");
            entity.Property(e => e.LevertDato)
                .HasColumnType("smalldatetime")
                .HasColumnName("levertDato");
            entity.Property(e => e.NettOrdreRef)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("nettOrdreRef");
            entity.Property(e => e.Ordreref)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ordreref");
            entity.Property(e => e.Ordretype)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Ordre");
            entity.Property(e => e.OurPrice)
                .HasColumnType("money")
                .HasColumnName("our_price");
            entity.Property(e => e.SendtLev)
                .HasDefaultValue((byte)0)
                .HasColumnName("sendtLev");
            entity.Property(e => e.VareId).HasColumnName("vareID");
            entity.Property(e => e.VarenrLev)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("varenr_lev");
            entity.Property(e => e.VarenrSwn).HasColumnName("varenr_swn");
        });

        modelBuilder.Entity<OrdreFraNett>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ordreFra__3213E83FE431941F");

            entity.ToTable("ordreFraNett");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("id");
            entity.Property(e => e.Antall)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("antall");
            entity.Property(e => e.Butikknr).HasColumnName("butikknr");
            entity.Property(e => e.DatoInserted)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("dato_inserted");
            entity.Property(e => e.DatoSendtLev)
                .HasColumnType("datetime")
                .HasColumnName("dato_sendtLev");
            entity.Property(e => e.DatoSlettet)
                .HasColumnType("datetime")
                .HasColumnName("dato_slettet");
            entity.Property(e => e.Frakt)
                .HasColumnType("money")
                .HasColumnName("frakt");
            entity.Property(e => e.OrdrenrNett).HasColumnName("ordrenrNett");
            entity.Property(e => e.SendtLev)
                .HasDefaultValue(false)
                .HasColumnName("sendtLev");
            entity.Property(e => e.Slettet)
                .HasDefaultValue(false)
                .HasColumnName("slettet");
            entity.Property(e => e.VarenrLev)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("varenrLev");
        });

        modelBuilder.Entity<Pakkseddel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_pakkseddel");

            entity.ToTable("Pakkseddel");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.Antall).HasColumnName("antall");
            entity.Property(e => e.Butikknr).HasColumnName("butikknr");
            entity.Property(e => e.Dato)
                .HasColumnType("smalldatetime")
                .HasColumnName("dato");
            entity.Property(e => e.Ordreref)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ordreref");
            entity.Property(e => e.PakkseddelId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("pakkseddelID");
            entity.Property(e => e.Sendingsnr)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sendingsnr");
            entity.Property(e => e.Stkpris)
                .HasColumnType("money")
                .HasColumnName("stkpris");
            entity.Property(e => e.Varenr).HasColumnName("varenr");
        });

        modelBuilder.Entity<Reklamasjon>(entity =>
        {
            entity.ToTable("Reklamasjon");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.Dato).HasColumnType("smalldatetime");
            entity.Property(e => e.Feilbeskrivelse)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.GodkjentReturDato).HasColumnType("smalldatetime");
            entity.Property(e => e.KreditertDato).HasColumnType("smalldatetime");
        });

        modelBuilder.Entity<Salg>(entity =>
        {
            entity.ToTable("Salg");

            entity.HasIndex(e => new { e.Butikknr, e.Dato }, "idx_salg_butikk_dato_20171011");

            entity.HasIndex(e => new { e.Dato, e.Antall }, "idx_salg_dato_antall_20171011");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.Dato).HasColumnType("smalldatetime");
            entity.Property(e => e.Innpris)
                .HasColumnType("money")
                .HasColumnName("innpris");
            entity.Property(e => e.Kundenr).HasColumnName("kundenr");
            entity.Property(e => e.OurPrice)
                .HasColumnType("money")
                .HasColumnName("our_price");
            entity.Property(e => e.Secbut).HasColumnName("secbut");
            entity.Property(e => e.SendFakt).HasDefaultValue(false);
            entity.Property(e => e.Utpris)
                .HasColumnType("money")
                .HasColumnName("utpris");
            entity.Property(e => e.VareId).HasColumnName("VareID");
            entity.Property(e => e.VarenrLev)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("varenrLev");
            entity.Property(e => e.VarenrSwsg).HasColumnName("varenr_swsg");
        });

        modelBuilder.Entity<SwsgTooltip>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("swsg_tooltips");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.Modulnavn)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("modulnavn");
            entity.Property(e => e.Tooltiptekst)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("tooltiptekst");
        });

        modelBuilder.Entity<TblCheckAutomate>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tbl_check_automate");

            entity.Property(e => e.AmStatus).HasColumnName("amStatus");
            entity.Property(e => e.SendtMail).HasColumnName("sendtMail");
            entity.Property(e => e.Updated).HasColumnName("updated");
        });

        modelBuilder.Entity<TblLoginLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbl_logi__3214EC27BBB72FEE");

            entity.ToTable("tbl_login_log");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.DateLogin).HasColumnName("date_login");
            entity.Property(e => e.ModulId).HasColumnName("modulID");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<TblPostnr>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tbl_postnr");

            entity.Property(e => e.Kategori)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("kategori");
            entity.Property(e => e.Kommunenavn)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("kommunenavn");
            entity.Property(e => e.Kommunenr).HasColumnName("kommunenr");
            entity.Property(e => e.Postnr)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("postnr");
            entity.Property(e => e.Poststed)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("poststed");
        });

        modelBuilder.Entity<TblVaregr1>(entity =>
        {
            entity.HasKey(e => e.IdVgr1).HasName("PK__tbl_vare__5519100EE2C22E0E");

            entity.ToTable("tbl_varegr1");

            entity.Property(e => e.IdVgr1).HasColumnName("id_vgr1");
            entity.Property(e => e.Varegruppe1)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblVaregr2>(entity =>
        {
            entity.HasKey(e => e.IdVgr2).HasName("PK__tbl_vare__5519100FD809418B");

            entity.ToTable("tbl_varegr2");

            entity.Property(e => e.IdVgr2).HasColumnName("id_vgr2");
            entity.Property(e => e.Varegruppe2)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TempBestilling>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("temp_bestilling");

            entity.Property(e => e.Antall).HasColumnName("antall");
            entity.Property(e => e.Butikknr).HasColumnName("butikknr");
            entity.Property(e => e.Varenrlev)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("varenrlev");
        });

        modelBuilder.Entity<TempImport1>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("temp_import_1");

            entity.Property(e => e.Andel).HasColumnType("money");
            entity.Property(e => e.Kostpris).HasColumnType("money");
            entity.Property(e => e.LevVarenr).HasColumnName("levVarenr");
            entity.Property(e => e.NyNetto).HasColumnType("money");
        });

        modelBuilder.Entity<Varegruppe>(entity =>
        {
            entity.HasKey(e => e.Varegruppenr);

            entity.ToTable("Varegruppe");

            entity.Property(e => e.Varegruppenr).ValueGeneratedNever();
            entity.Property(e => e.Varegruppebeskrivelse)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Vareinfo>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.VarenrSwn });

            entity.ToTable("Vareinfo");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.VarenrSwn).HasColumnName("Varenr_SWN");
            entity.Property(e => e.DatoInserted)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("dato_inserted");
            entity.Property(e => e.Ean)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("EAN");
            entity.Property(e => e.Innpris).HasColumnType("decimal(18, 8)");
            entity.Property(e => e.MarginKr)
                .HasColumnType("decimal(18, 8)")
                .HasColumnName("marginKr");
            entity.Property(e => e.MarginKr2)
                .HasColumnType("decimal(18, 8)")
                .HasColumnName("marginKr2");
            entity.Property(e => e.MarginP)
                .HasColumnType("decimal(18, 8)")
                .HasColumnName("marginP");
            entity.Property(e => e.MarginP2)
                .HasColumnType("decimal(18, 8)")
                .HasColumnName("marginP2");
            entity.Property(e => e.OurMarginKr)
                .HasColumnType("decimal(18, 8)")
                .HasColumnName("our_marginKr");
            entity.Property(e => e.OurMarginP)
                .HasColumnType("decimal(18, 8)")
                .HasColumnName("our_marginP");
            entity.Property(e => e.OurPrice)
                .HasColumnType("decimal(18, 8)")
                .HasColumnName("our_price");
            entity.Property(e => e.SistEndret).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Utpris).HasColumnType("decimal(18, 8)");
            entity.Property(e => e.Utpris2).HasColumnType("decimal(18, 8)");
            entity.Property(e => e.Varebeskrivelse2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Varebeskrivelse_2");
            entity.Property(e => e.VarenrLev)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Varenr_Lev");
            entity.Property(e => e.VaretekstAlternativ)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("varetekstAlternativ");
        });

        modelBuilder.Entity<VareinfoBackup>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Vareinfo_backup");

            entity.Property(e => e.BongLengde).HasComputedColumnSql("(len([Bongtekst]))", false);
            entity.Property(e => e.Bongtekst)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.DistrKostpris)
                .HasColumnType("money")
                .HasColumnName("Distr. Kostpris");
            entity.Property(e => e.Ean)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("EAN");
            entity.Property(e => e.EstimertAndel)
                .HasColumnType("money")
                .HasColumnName("Estimert Andel");
            entity.Property(e => e.ExportRc).HasColumnName("ExportRC");
            entity.Property(e => e.Fl).HasColumnName("FL");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.Innpris).HasColumnType("money");
            entity.Property(e => e.Kj).HasColumnName("KJ");
            entity.Property(e => e.La).HasColumnName("LA");
            entity.Property(e => e.LinkBildeBank)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.LinkForpakning)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Lo).HasColumnName("LO");
            entity.Property(e => e.MarginKr)
                .HasComputedColumnSql("([Utpris]*(0.8)-[Innpris])", false)
                .HasColumnType("numeric(22, 5)")
                .HasColumnName("marginKr");
            entity.Property(e => e.MarginP)
                .HasComputedColumnSql("(case when isnull([Utpris],(0))=(0) then (0) else ([Utpris]*(0.8)-[Innpris])/([Utpris]*(0.8)) end)", false)
                .HasColumnType("numeric(38, 16)")
                .HasColumnName("marginP");
            entity.Property(e => e.NyNetto).HasColumnType("money");
            entity.Property(e => e.OurMarginKr)
                .HasComputedColumnSql("(isnull([Innpris]-[our_price],(0)))", false)
                .HasColumnType("money")
                .HasColumnName("our_marginKr");
            entity.Property(e => e.OurMarginP)
                .HasComputedColumnSql("(case when isnull([Innpris],(0))=(0) then (0) else isnull([Innpris]-[our_price],(0))/[Innpris] end)", false)
                .HasColumnType("money")
                .HasColumnName("our_marginP");
            entity.Property(e => e.OurPrice)
                .HasColumnType("money")
                .HasColumnName("our_price");
            entity.Property(e => e.ProductFit)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Se).HasColumnName("SE");
            entity.Property(e => e.SortKode)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Sy).HasColumnName("SY");
            entity.Property(e => e.Tb).HasColumnName("TB");
            entity.Property(e => e.To).HasColumnName("TO");
            entity.Property(e => e.Utpris).HasColumnType("money");
            entity.Property(e => e.Varebeskrivelse)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Varebeskrivelse2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Varebeskrivelse_2");
            entity.Property(e => e.VarenrLev)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Varenr_Lev");
            entity.Property(e => e.VarenrProd)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Varenr_Prod");
            entity.Property(e => e.VarenrRc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Varenr_RC");
            entity.Property(e => e.VarenrSwn).HasColumnName("Varenr_SWN");
            entity.Property(e => e.VaretekstAlternativ)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("varetekstAlternativ");
        });

        modelBuilder.Entity<VareinfoTemp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Vareinfo_Temp");

            entity.Property(e => e.Bongtekst)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Fl)
                .HasDefaultValue(false)
                .HasColumnName("FL");
            entity.Property(e => e.Kj)
                .HasDefaultValue(false)
                .HasColumnName("KJ");
            entity.Property(e => e.La)
                .HasDefaultValue(false)
                .HasColumnName("LA");
            entity.Property(e => e.LinkBildeBank).IsUnicode(false);
            entity.Property(e => e.LinkForpakning).IsUnicode(false);
            entity.Property(e => e.Lo)
                .HasDefaultValue(false)
                .HasColumnName("LO");
            entity.Property(e => e.ProductFit)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Prøve).HasDefaultValue(false);
            entity.Property(e => e.Se)
                .HasDefaultValue(false)
                .HasColumnName("SE");
            entity.Property(e => e.Sy)
                .HasDefaultValue(false)
                .HasColumnName("SY");
            entity.Property(e => e.Tb)
                .HasDefaultValue(false)
                .HasColumnName("TB");
            entity.Property(e => e.To)
                .HasDefaultValue(false)
                .HasColumnName("TO");
            entity.Property(e => e.VarenrLev)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("Varenr_Lev");
        });

        modelBuilder.Entity<VareinfoTemp2>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Vareinfo_temp2");

            entity.Property(e => e.VarenrLev)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("varenrLev");
            entity.Property(e => e.Varetekst)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Varetelling>(entity =>
        {
            entity.ToTable("varetelling");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.Antall).HasColumnName("antall");
            entity.Property(e => e.Butikknr).HasColumnName("butikknr");
            entity.Property(e => e.Ean)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ean");
            entity.Property(e => e.PeriodeRc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("periodeRC");
            entity.Property(e => e.Telledato)
                .HasColumnType("smalldatetime")
                .HasColumnName("telledato");
            entity.Property(e => e.VareId).HasColumnName("vareID");
            entity.Property(e => e.VarenrLev)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("varenrLev");
        });

        OnModelCreatingGeneratedProcedures(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}