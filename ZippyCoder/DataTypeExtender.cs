namespace System.Data
{
   public static class DataTypeExtender
    {
       public static string ToPSQLString(this System.Data.SqlDbType type)
       {
           switch (type)
           {
               case System.Data.SqlDbType.NVarChar:
                   return"VarChar";
               case System.Data.SqlDbType.NText:
                   return "Text";
               case System.Data.SqlDbType.NChar:
                   return "Char";
               case  SqlDbType.DateTime:
                   return "TIMESTAMP";
               default:
                   return type.ToString();
           }

       }
    }
}
