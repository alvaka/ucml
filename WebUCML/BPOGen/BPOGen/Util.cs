using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace UCML.IDE.WebUCML
{
    class Util
    {
        public static string[] SplitLine(string text)
        {
            string[] lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 1) return lines;
            else
            {
                lines = text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > 1) return lines;
            }
            return new string[] { text };
        }

        public static String  GetDBConnecString(string serv,string database,string user,string passwd)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append("Data Source="+serv+";");
            sb.Append("Initial Catalog="+database+";");
            sb.Append("User ID="+user+";");
            sb.Append("Password="+passwd);
            return sb.ToString();
        }
    }
    public class NoCaseStringComparer : IEqualityComparer<String>
    {
        public  bool Equals(String str1,String str2)
        {
            if (str1.ToLower() == str2.ToLower()) return true;
            else return false;
        }
        public int GetHashCode(string str)
        {
            return str.GetHashCode();
        }
    }
}
