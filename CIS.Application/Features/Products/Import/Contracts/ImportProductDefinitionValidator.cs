using CIS.Application.Features.Products.Models.Import;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Products.Import.Contracts
{
    public sealed class ImportProductDefinitionValidator : AbstractValidator<ImportProductDefinition>
    {
        public ImportProductDefinitionValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
            RuleFor(x => x.CurrencyCode)
                .NotEmpty();
        }
    }
}
