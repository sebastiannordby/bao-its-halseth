using CIS.Library.Products.Models;
using Microsoft.AspNetCore.Components;

namespace CIS.Components
{
    public partial class ProductDetailDialog
    {
        [Parameter]
        public ProductView Product { get; set; }
    }
}
