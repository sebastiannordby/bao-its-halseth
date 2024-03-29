﻿using CIS.Application.Features.Products.Models.Import;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Features.Products.Import.Contracts
{
    public sealed class ImportProductCommandValidator : AbstractValidator<ImportProductCommand>
    {
        public ImportProductCommandValidator(
            IValidator<ImportProductDefinition> definitionValidator)
        {
            RuleFor(x => x.Definitions)
                .NotEmpty();
            RuleForEach(x => x.Definitions)
                .SetValidator(definitionValidator);
        }
    }
}
