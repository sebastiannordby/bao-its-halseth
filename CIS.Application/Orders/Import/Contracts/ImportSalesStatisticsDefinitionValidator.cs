using CIS.Application.Orders.Contracts.Import;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Import.Contracts
{
    public sealed class ImportSalesStatisticsDefinitionValidator : AbstractValidator<ImportSalesStatisticsDefinition>
    {
        public ImportSalesStatisticsDefinitionValidator()
        {

        }
    }
}
