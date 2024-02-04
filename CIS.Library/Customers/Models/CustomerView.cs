namespace CIS.Library.Customers.Models
{
    public class CustomerView
    {
        
        public int Number { get; set; }
        public string Name { get; set; }
        public string? ContactPersonName { get; set; }
        public string? ContactPersonEmailAddress { get; set; }
        public string? ContactPersonPhoneNumber { get; set; }
        public int? CustomerGroupNumber { get; set; }
        public string? CustomerGroupName { get; set; }
        public bool IsActive { get; set; }
    }



    //public record CustomerView(
    //    int Number,
    //    string Name,
    //    string? ContactPersonName,
    //    string? ContactPersonEmailAddress,
    //    string? ContactPersonPhoneNumber,
    //    int? CustomerGroupNumber,
    //    string? CustomerGroupName,
    //    bool IsActive
    //);
}
