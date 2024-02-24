using CIS.Application.Orders.Contracts;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Import.Contracts
{
    internal class ImportSalesOrderDefinitionValidator : AbstractValidator<ImportSalesOrderDefinition>
    {
        public ImportSalesOrderDefinitionValidator()
        {
            RuleFor(x => x.CustomerNumber)
                .NotEmpty();
            RuleFor(x => x.CustomerName)
                .NotEmpty();
            RuleFor(x => x.StoreNumber)
                .NotEmpty();
            RuleFor(x => x.StoreName)
                .NotEmpty();
            RuleFor(x => x.Lines)
                .NotEmpty();
        }
    }
}
