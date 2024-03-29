﻿using CIS.Application.Features.Orders.Contracts;
using CIS.Application.Features.Orders.Infrastructure;
using CIS.Application.Features.Orders.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Runtime.CompilerServices;

namespace CIS.Application.Shared.Infrastructure
{
    internal class SalesQueries : ISalesQueries
    {
        private readonly CISDbContext _dbContext;

        public SalesQueries(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SeasonalityAnalysisResult> AnalyzeSeasonality(int year)
        {
            // Initialize the result object
            var seasonalityResult = new SeasonalityAnalysisResult();

            // Query sales data grouped by month for the specified year
            var monthlySalesData = await _dbContext.SalesStatistics
                .AsNoTracking()
                .Where(sale => sale.Date.Year == year)  // Filter by the specified year
                .GroupBy(sale => sale.Date.Month)       // Group by month
                .Select(group => new
                {
                    Month = group.Key,
                    TotalQuantity = group.Sum(sale => sale.Quantity)
                })
                .ToListAsync();

            // Add monthly sales data to the result object
            foreach (var monthlyData in monthlySalesData)
            {
                seasonalityResult.MonthlySales.Add(new MonthlySalesData
                {
                    Month = monthlyData.Month,
                    TotalQuantity = monthlyData.TotalQuantity
                });
            }

            if (!seasonalityResult.MonthlySales.Any())
                return seasonalityResult;

            // Calculate the average monthly sales
            var averageMonthlySales = seasonalityResult.MonthlySales.Average(data => data.TotalQuantity);

            // Calculate the percentage deviation from the average for each month
            foreach (var monthlyData in seasonalityResult.MonthlySales)
            {
                monthlyData.DeviationFromAverage = (monthlyData.TotalQuantity - averageMonthlySales) / averageMonthlySales * 100;
            }

            // Identify peak and off-peak months based on deviation from average
            foreach (var monthlyData in seasonalityResult.MonthlySales)
            {
                if (monthlyData.DeviationFromAverage >= 20)  // Threshold for peak months
                {
                    seasonalityResult.PeakMonths.Add(monthlyData.Month);
                }
                else if (monthlyData.DeviationFromAverage <= -20)  // Threshold for off-peak months
                {
                    seasonalityResult.OffPeakMonths.Add(monthlyData.Month);
                }
            }

            return seasonalityResult;
        }


        public async Task<IReadOnlyCollection<StoreMostBoughtView>> GetMostBoughtViews(int count)
        {
            var query = (
                from sale in _dbContext.SalesStatistics
                    .AsNoTracking()
                join store in _dbContext.Stores
                    on sale.StoreNumber equals store.Number
                group new { sale, store } by
                    new { sale.StoreNumber, store.Name, sale.Quantity }
                    into productGroup
                orderby productGroup.Sum(x => x.sale.PurchasePrice) descending
                select new StoreMostBoughtView
                {
                    StoreName = productGroup.Key.Name,
                    TotalBoughtFor = productGroup.Sum(x => x.sale.PurchasePrice),
                    TotalCost = productGroup.Sum(x => x.sale.CostPrice),
                }
            ).Take(count);

            var list = await query.ToListAsync();

            return list;
        }

        public async Task<IReadOnlyCollection<MostSoldProductView>> GetMostSoldProduct(int count)
        {
            var query = (
                from sale in _dbContext.SalesStatistics
                    .AsNoTracking()
                join product in _dbContext.Products
                    on sale.ProductNumber equals product.Number
                group new { sale, product } by
                    new { sale.ProductNumber, product.Name }
                    into productGroup
                orderby productGroup.Sum(x => x.sale.Quantity) descending
                select new MostSoldProductView
                {
                    ProductNumber = productGroup.Key.ProductNumber,
                    ProductName = productGroup.Key.Name,
                    TotalQuantity = productGroup.Sum(x => x.sale.Quantity),
                    TotalSoldFor = productGroup.Sum(x => x.sale.PurchasePrice),
                    TotalCostPrice = productGroup.Sum(x => x.sale.CostPrice)
                }
            ).Take(count);

            var list = await query.ToListAsync();

            return list;
        }

        public async Task<IReadOnlyCollection<SalesOrderView>> List(int pageSize, int pageIndex)
        {
            var orderList = await _dbContext.SalesOrders
                .AsViewQuery()
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            return orderList.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<SalesOrderView>> List(
            int customerNumber, int pageSize, int pageIndex)
        {
            var orderList = await _dbContext.SalesOrders
                .Where(x => x.CustomerNumber == customerNumber)
                .AsViewQuery()
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            return orderList.AsReadOnly();
        }

        public IQueryable<SalesOrderView> Query()
        {
            var orderQuery =
                from order in _dbContext.SalesOrders
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted)

                select new SalesOrderView()
                {
                    Id = order.Id,
                    Number = order.Number,
                    AlternateNumber = order.AlternateNumber,
                    OrderDate = order.OrderDate,
                    Reference = order.Reference,
                    DeliveredDate = order.DeliveredDate,
                    StoreNumber = order.StoreNumber,
                    StoreName = order.StoreName,
                    CustomerNumber = order.CustomerNumber,
                    CustomerName = order.CustomerName,
                    IsDeleted = order.IsDeleted,
                }
            ;

            return orderQuery;
        }
    }

    internal static class Extensions
    {
        internal static IQueryable<SalesOrderView> AsViewQuery(this IQueryable<SalesOrderDao> orders)
        {
            var orderViewsQuery = (
                from order in orders.AsNoTracking()

                select new SalesOrderView()
                {
                    Id = order.Id,
                    Number = order.Number,
                    AlternateNumber = order.AlternateNumber,
                    OrderDate = order.OrderDate,
                    Reference = order.Reference,
                    DeliveredDate = order.DeliveredDate,
                    StoreNumber = order.StoreNumber,
                    StoreName = order.StoreName,
                    CustomerNumber = order.CustomerNumber,
                    CustomerName = order.CustomerName,
                    IsDeleted = order.IsDeleted,
                }
            );

            return orderViewsQuery;
        }
    }
}
