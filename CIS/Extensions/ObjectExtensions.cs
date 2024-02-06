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

        public static DateTime? ToDateTime(this object obj) 
        {
            if(obj is string str)
            {
                if (str == null || str == "NULL")
                    return null;
            }

            return Convert.ToDateTime(obj);
        }

        public static decimal? ToDecimal(this object obj)
        {
            if (obj is string str)
            {
                if (str == null || str == "NULL")
                    return null;

                return Convert.ToDecimal(str.Replace(".", ","));
            }

            return Convert.ToDecimal(obj);
        }

        public static string AsExcelString(this object obj)
        {
            if(obj is string str)
            {
                if(str == "" || str == "NULL")
                {
                    return null;
                }
            }

            return Convert.ToString(obj);
        }
    }
}
