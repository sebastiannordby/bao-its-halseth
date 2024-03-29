﻿using CIS.Application.Shared.Services;
using CIS.Application.Shopify.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Shopify
{
    public static class ShopifyFeatureExtensions
    {
        public static IServiceCollection AddShopifyFeature(this IServiceCollection services)
        {
            IShopifyClientService.RegisterShopifyClientService(services);

            services.AddScoped<IExecuteImportFromShopify<Order>, ImportShopifyOrderService>();

            return services;
        }
    }
}
