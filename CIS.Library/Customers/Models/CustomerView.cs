namespace CIS.Library.Customers.Models
{
    public record CustomerView(
        int Number,
        string Name,
        string? ContactPersonName,
        string? ContactPersonEmailAddress,
        string? ContactPersonPhoneNumber,
        int? CustomerGroupNumber,
        string? CustomerGroupName,
        bool IsActive
    );
}
