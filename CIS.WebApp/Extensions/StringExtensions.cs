namespace CIS.WebApp.Extensions
{
    public static class StringExtensions
    {
        public static decimal? StringDecimalToDecimal(this string value)
        {
            if (value is null)
                return null;

            if (value == "NULL")
                return null;

            return Convert.ToDecimal(value);
        }
    }
}
