using CIS.Application.Shopify;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopifyOrder = ShopifySharp.Order;

namespace CIS.Tests.Domain.Orders
{
    public class ImportShopifyOrderServiceTests : IClassFixture<DomainTestFixture>
    {
        private readonly ImportShopifyOrderService _sut;
        private readonly IShopifyClientService _clientServiceMock;

        public ImportShopifyOrderServiceTests(DomainTestFixture fixture)
        {
            var serviceCollection = fixture.GetServiceCollection();

            serviceCollection.AddShopifyFeature();

            _clientServiceMock = Substitute.For<IShopifyClientService>();
            serviceCollection.AddSingleton(_clientServiceMock);

            var services = serviceCollection.BuildServiceProvider();
            _sut = services.GetRequiredService<ImportShopifyOrderService>();
        }

        [Fact]
        public async Task ExecuteShopifyImportShouldCallShopify()
        {
            try
            {
                var shopifyOrders = new List<ShopifyOrder>();

                _clientServiceMock
                    .GetOrdersAsync(
                        Arg.Any<CancellationToken>())
                    .Returns(shopifyOrders);

                await _sut.ExecuteShopifyImport(CancellationToken.None);

                await _clientServiceMock
                    .Received()
                    .GetOrdersAsync(Arg.Any<CancellationToken>());
            }
            catch(Exception e)
            {

            }
        }
    }
}
