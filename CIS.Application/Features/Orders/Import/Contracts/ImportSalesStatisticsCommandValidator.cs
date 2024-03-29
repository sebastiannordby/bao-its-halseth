﻿using CIS.Application.Features.Orders.Contracts;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Orders.Import.Contracts
{
    public sealed class ImportSalesStatisticsCommandValidator : AbstractValidator<ImportSalesStatisticsCommand>
    {
        public ImportSalesStatisticsCommandValidator(
            IValidator<ImportSalesStatisticsDefinition> definitionValidator)
        {
            RuleFor(x => x.Definitions)
                .NotEmpty();
            RuleForEach(x => x.Definitions)
                .SetValidator(definitionValidator);
        }
    }
}
