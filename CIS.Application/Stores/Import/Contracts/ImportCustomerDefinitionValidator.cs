using CIS.Application.Stores.Models.Import;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Stores.Import.Contracts
{
    public sealed class ImportCustomerDefinitionValidator : AbstractValidator<ImportCustomerDefinition>
    {
        public ImportCustomerDefinitionValidator()
        {

        }
    }
}
