namespace CIS.Extensions
{
    public static class ObjectExtensions
    {
        public static int? ToInt32(this object obj)
        {
            if(obj is string str)
            {
                if (str == null || str == "NULL")
                    return null;
            }

            return Convert.ToInt32(obj);
        }

        public static string ToString(this object obj)
        {
            return Convert.ToString(obj);
        }
    }
}
