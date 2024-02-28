using Bogus;
using CIS.Application.Legacy;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Tests.Integration
{
    public static class SWNDistroSeedData
    {
        //public static void SeedWithIntegrationTestData(string connectionString)
        //{
        //    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);

        //    string server = builder.DataSource;
        //    string username = builder.UserID;
        //    string password = builder.Password;

        //    Server sqlServer = new Server(new ServerConnection(server, username, password));
        //    Restore restore = new Restore();
        //    restore.Database = "swn_distro";
        //    restore.Action = RestoreActionType.Database;
        //    restore.Devices.AddDevice(@"./swn_distro_sanitized.bak", DeviceType.File);
        //    restore.ReplaceDatabase = true;
        //    restore.SqlRestore(sqlServer);
        //}

        public static void SeedWithIntegrationTestData(ServiceProvider services)
        {
            var dbContext = services
                .GetRequiredService<SWNDistroContext>();

            var faker = new Faker();

            var orders = new List<Ordre>();
            var customers = new List<Butikkliste>();
            var products = new List<Vareinfo>();

            for(var i = 1; i <= 1000; i++) 
            {
                var customer = new Butikkliste()
                {
                    Aktiv = true,
                    Butikknavn = faker.Person.FirstName,
                    Butikknr = i,
                    Epost = faker.Person.Email,
                    Kundenr = faker.Random.Number(20000),
                    Gateadresse = faker.Person.Address.Street,
                    Postnr = faker.Random.Number(9999),
                    Poststed = faker.Person.Address.City,
                    Telefon = faker.Random.Number(99999999),
                };

                var ourPrice = faker.Random.Number(10000);
                var innpris = faker.Random.Number(0, ourPrice);
                var utPris = faker.Random.Number(innpris, 10000);

                var product = new Vareinfo()
                {
                    Id = i,
                    VarenrSwn = faker.Random.Number(20000),
                    Innpris = faker.Random.Number(),
                    OurPrice = ourPrice,
                    Utpris = utPris,
                    Varebeskrivelse2 = faker.Random.AlphaNumeric(10),
                    Ean = faker.Random.AlphaNumeric(10),
                    VarenrLev = faker.Random.AlphaNumeric(5),
                    VaretekstAlternativ = faker.Random.AlphaNumeric(20),
                    Aktiv = true,
                    DatoInserted = DateTime.Now,
                    MarginKr = 0,
                    MarginKr2 = 0,
                    MarginP = 0,
                    MarginP2 = 0,
                    OurMarginKr = 0,
                    OurMarginP = 0,
                    Utpris2 = 0,
                };

                var ordre = new Ordre()
                {
                    Id = i,
                    Antall = faker.Random.Number(100),
                    AntallLevert = faker.Random.Number(100),
                    Butikknr = customer.Butikknr,
                    NettOrdreRef = faker.Random.AlphaNumeric(5),
                    LevertDato = faker.Date.RecentOffset(1000).DateTime,
                    Innpris = faker.Random.Decimal(20000),
                    OurPrice = faker.Random.Decimal(20000),
                    VarenrLev = faker.Random.AlphaNumeric(5),
                    VarenrSwn = (int) product.Id
                };

                orders.Add(ordre);
                customers.Add(customer);
                products.Add(product);
            }

            try
            {
                dbContext.Database.BeginTransaction();
                dbContext.Database.EnsureCreated();

                dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Ordre ON");
                dbContext.Ordres.AddRange(orders);
                dbContext.SaveChanges();
                dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Ordre OFF");

                dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Vareinfo ON");
                dbContext.Vareinfos.AddRange(products);
                dbContext.SaveChanges();
                dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Vareinfo OFF");

                dbContext.Butikklistes.AddRange(customers);
                dbContext.SaveChanges();
                //dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Butikkliste OFF");
                dbContext.Database.CommitTransaction();
            }
            catch (Exception e)
            {
                var db = e;

                dbContext.Database.RollbackTransaction();
            }
        }
    }
}
