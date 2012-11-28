using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;

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

        public static bool SaveTextFile(string context, string path)
        {
            try
            {
                StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8);
                writer.Write(context);
                writer.Close();
            }
            catch (IOException e)
            {
                return false;
            }
            return true;
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
        public static String GetPropString(SqlDataReader reader, int index)
        {
            if (reader.IsDBNull(index)) return "";
            else return reader.GetString(index);
        }
        public static bool GetPropBool(SqlDataReader reader, int index)
        {
            if (reader.IsDBNull(index)) return false;
            else return reader.GetBoolean(index);
        }

        public static int GetProperInt(SqlDataReader reader, int index)
        {
            if (reader.IsDBNull(index)) return 0;
            else return reader.GetInt32(index);
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
