
using CIS.Application.Shared.Models;
using CIS.Application.Shared.Repositories;
using Microsoft.AspNetCore.Components;

namespace CIS.WebApp.Components.Layout
{
    public partial class NavMenu 
    {
        [CascadingParameter(Name = "ShowMigrationPage")] 
        public bool ShowMigrationPage { get; set; }
    }
}
