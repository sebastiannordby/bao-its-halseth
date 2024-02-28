using CIS.Application.Features.Products.Models;
using Microsoft.AspNetCore.Components;

namespace CIS.WebApp.Components.Dialogs
{
    public partial class ProductDetailDialog
    {
        [Parameter]
        public ProductView Product { get; set; }
    }
}
