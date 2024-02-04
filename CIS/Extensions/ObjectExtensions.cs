namespace CIS.Extensions
{
    public static class ObjectExtensions
    {
        public static int ToInt32(this object obj)
        {
            return Convert.ToInt32(obj);
        }

        public static string ToString(this object obj)
        {
            return Convert.ToString(obj);
        }
    }
}
