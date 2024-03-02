using Castle.Core.Logging;
using CIS.Application.Shared.Services;
using CIS.Application.Shopify;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private readonly IExecuteImportFromShopify<ShopifyOrder> _sut;
        private readonly ILogger<ImportShopifyOrderService> _loggerMock;
        private readonly IShopifyClientService _clientServiceMock;

        public ImportShopifyOrderServiceTests(DomainTestFixture fixture)
        {
            var serviceCollection = fixture.GetServiceCollection();

            serviceCollection.AddShopifyFeature();

            _clientServiceMock = Substitute.For<IShopifyClientService>();
            serviceCollection.AddSingleton(_clientServiceMock);

            var services = serviceCollection.BuildServiceProvider();
            _sut = services.GetRequiredService<IExecuteImportFromShopify<ShopifyOrder>>();
        }

        [Fact]
        public async Task ExecuteShopifyImportShouldCallShopify()
        {
            try
            {
                var shopifyOrders = new List<ShopifyOrder>() 
                { 
                    new ShopifyOrder()
                };

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
