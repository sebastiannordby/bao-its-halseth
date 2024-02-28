using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Orders.Contracts
{
    public class SeasonalityAnalysisResult
    {
        public List<MonthlySalesData> MonthlySales { get; set; }
        public List<int> PeakMonths { get; set; }
        public List<int> OffPeakMonths { get; set; }

        public SeasonalityAnalysisResult()
        {
            MonthlySales = new List<MonthlySalesData>();
            PeakMonths = new List<int>();
            OffPeakMonths = new List<int>();
        }
    }

    public class MonthlySalesData
    {
        public int Month { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal DeviationFromAverage { get; set; }
    }
}
