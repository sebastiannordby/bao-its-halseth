﻿using CIS.Library.Products.Import;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Products.Import.Contracts
{
    public sealed class ImportProductDefinitionValidator : AbstractValidator<ImportProductDefinition>
    {
        public ImportProductDefinitionValidator()
        {

        }
    }
}