using CIS.Application.Orders.Contracts;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Orders.Import.Contracts
{
    public class ImportSalesOrderCommandValidator : AbstractValidator<ImportSalesOrderCommand>
    {
        public ImportSalesOrderCommandValidator(
            IValidator<ImportSalesOrderDefinition> definitionValidator)
        {
            RuleFor(x => x.Definitions)
                .NotEmpty();
            RuleFor(x => x.Definitions.Count())
                .NotEqual(0);
            RuleForEach(x => x.Definitions)
                .SetValidator(definitionValidator);
        }
    }
}
