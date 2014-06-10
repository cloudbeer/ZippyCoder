using System;

namespace ZippyCoder
{
    public class TypeConverter
    {

        public static System.Data.DbType ToDbType(System.Data.SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                case System.Data.SqlDbType.BigInt:
                    return System.Data.DbType.Int64;
                case System.Data.SqlDbType.Binary:
                    return System.Data.DbType.Binary;
                case System.Data.SqlDbType.Bit:
                    return System.Data.DbType.Boolean;
                case System.Data.SqlDbType.Char:
                    return System.Data.DbType.String;
                case System.Data.SqlDbType.Date:
                    return System.Data.DbType.Date;
                case System.Data.SqlDbType.DateTime:
                    return System.Data.DbType.DateTime;
                case System.Data.SqlDbType.DateTime2:
                    return System.Data.DbType.DateTime2;
                case System.Data.SqlDbType.DateTimeOffset:
                    return System.Data.DbType.DateTimeOffset;
                case System.Data.SqlDbType.Decimal:
                    return System.Data.DbType.Decimal;
                case System.Data.SqlDbType.Float:
                    return System.Data.DbType.VarNumeric;
                case System.Data.SqlDbType.Image:
                    return System.Data.DbType.Object;
                case System.Data.SqlDbType.Int:
                    return System.Data.DbType.Int32;
                case System.Data.SqlDbType.Money:
                    return System.Data.DbType.Currency;
                case System.Data.SqlDbType.NChar:
                    return System.Data.DbType.String;
                case System.Data.SqlDbType.NText:
                    return System.Data.DbType.String;
                case System.Data.SqlDbType.NVarChar:
                    return System.Data.DbType.String;
                case System.Data.SqlDbType.Real:
                    return System.Data.DbType.VarNumeric;
                case System.Data.SqlDbType.SmallDateTime:
                    return System.Data.DbType.DateTime;
                case System.Data.SqlDbType.SmallInt:
                    return System.Data.DbType.Int16;
                case System.Data.SqlDbType.SmallMoney:
                    return System.Data.DbType.Currency;
                case System.Data.SqlDbType.Structured:
                    return System.Data.DbType.Object;
                case System.Data.SqlDbType.Text:
                    return System.Data.DbType.String;
                case System.Data.SqlDbType.Time:
                    return System.Data.DbType.Time;
                case System.Data.SqlDbType.Timestamp:
                    return System.Data.DbType.DateTimeOffset;
                case System.Data.SqlDbType.TinyInt:
                    return System.Data.DbType.Byte;
                case System.Data.SqlDbType.Udt:
                    return System.Data.DbType.Object;
                case System.Data.SqlDbType.UniqueIdentifier:
                    return System.Data.DbType.Guid;
                case System.Data.SqlDbType.VarBinary:
                    return System.Data.DbType.Binary;
                case System.Data.SqlDbType.VarChar:
                    return System.Data.DbType.String;
                case System.Data.SqlDbType.Variant:
                    return System.Data.DbType.Object;
                case System.Data.SqlDbType.Xml:
                    return System.Data.DbType.Xml;
                default:
                    return System.Data.DbType.String;
            }
        }

        public static string ToJavaType(System.Data.SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                case System.Data.SqlDbType.BigInt:
                    return "Long";
                case System.Data.SqlDbType.Binary:
                    return "Byte[]";
                case System.Data.SqlDbType.Bit:
                    return "Boolean";
                case System.Data.SqlDbType.Char:
                    return "Character";
                case System.Data.SqlDbType.Date:
                    return "Date";
                case System.Data.SqlDbType.DateTime:
                    return "Date";
                case System.Data.SqlDbType.DateTime2:
                    return "Date";
                case System.Data.SqlDbType.DateTimeOffset:
                    return "Object";
                case System.Data.SqlDbType.Decimal:
                    return "BigDecimal";
                case System.Data.SqlDbType.Float:
                    return "Float";
                case System.Data.SqlDbType.Image:
                    return "Byte[]";
                case System.Data.SqlDbType.Int:
                    return "Integer";
                case System.Data.SqlDbType.Money:
                    return "Float"; ;
                case System.Data.SqlDbType.NChar:
                    return "String";
                case System.Data.SqlDbType.NText:
                    return "String";
                case System.Data.SqlDbType.NVarChar:
                    return "String";
                case System.Data.SqlDbType.Real:
                    return "Double";
                case System.Data.SqlDbType.SmallDateTime:
                    return "Date";
                case System.Data.SqlDbType.SmallInt:
                    return "int";
                case System.Data.SqlDbType.SmallMoney:
                    return "Float";
                case System.Data.SqlDbType.Structured:
                    return "Object";
                case System.Data.SqlDbType.Text:
                    return "String";
                case System.Data.SqlDbType.Time:
                    return "Date";
                case System.Data.SqlDbType.Timestamp:
                    return "Date";
                case System.Data.SqlDbType.TinyInt:
                    return "int";
                case System.Data.SqlDbType.Udt:
                    return "Object";
                case System.Data.SqlDbType.UniqueIdentifier:
                    return "UUID";
                case System.Data.SqlDbType.VarBinary:
                    return "Byte[]";
                case System.Data.SqlDbType.Variant:
                    return "Object";
                case System.Data.SqlDbType.Xml:
                    return "Object";
                default:
                    return "String"; ;
            }
        }
        public static string ToGroovyType(System.Data.SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                case System.Data.SqlDbType.BigInt:
                    return "long";
                case System.Data.SqlDbType.Binary:
                    return "byte[]";
                case System.Data.SqlDbType.Bit:
                    return "boolean";
                case System.Data.SqlDbType.Char:
                    return "char";
                case System.Data.SqlDbType.Date:
                    return "Date";
                case System.Data.SqlDbType.DateTime:
                    return "Date";
                case System.Data.SqlDbType.DateTime2:
                    return "Date";
                case System.Data.SqlDbType.DateTimeOffset:
                    return "Object";
                case System.Data.SqlDbType.Decimal:
                    return "BigDecimal";
                case System.Data.SqlDbType.Float:
                    return "float";
                case System.Data.SqlDbType.Image:
                    return "byte[]";
                case System.Data.SqlDbType.Int:
                    return "int";
                case System.Data.SqlDbType.Money:
                    return "float";;
                case System.Data.SqlDbType.NChar:
                    return "String";
                case System.Data.SqlDbType.NText:
                    return "String";
                case System.Data.SqlDbType.NVarChar:
                    return "String";
                case System.Data.SqlDbType.Real:
                    return "double";
                case System.Data.SqlDbType.SmallDateTime:
                    return "Date";
                case System.Data.SqlDbType.SmallInt:
                    return "int";
                case System.Data.SqlDbType.SmallMoney:
                    return "float";
                case System.Data.SqlDbType.Structured:
                    return "Object";
                case System.Data.SqlDbType.Text:
                    return "String";
                case System.Data.SqlDbType.Time:
                    return "Date";
                case System.Data.SqlDbType.Timestamp:
                    return "Date";
                case System.Data.SqlDbType.TinyInt:
                    return "int";
                case System.Data.SqlDbType.Udt:
                    return "Object";
                case System.Data.SqlDbType.UniqueIdentifier:
                    return "UUID";
                case System.Data.SqlDbType.VarBinary:
                    return "byte[]";
                case System.Data.SqlDbType.Variant:
                    return "Object";
                case System.Data.SqlDbType.Xml:
                    return "Object";
                default:
                    return "String";;
            }
        }

        public static Type ToNetType(System.Data.SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                case System.Data.SqlDbType.BigInt:
                    return typeof(Int64);
                case System.Data.SqlDbType.Binary:
                    return typeof(Byte);
                case System.Data.SqlDbType.Bit:
                    return typeof(Boolean);
                case System.Data.SqlDbType.Char:
                    return typeof(String);
                case System.Data.SqlDbType.Date:
                    return typeof(DateTime);
                case System.Data.SqlDbType.DateTime:
                    return typeof(DateTime);
                case System.Data.SqlDbType.DateTime2:
                    return typeof(DateTime);
                case System.Data.SqlDbType.DateTimeOffset:
                    return typeof(TimeSpan);
                case System.Data.SqlDbType.Decimal:
                    return typeof(Decimal);
                case System.Data.SqlDbType.Float:
                    return typeof(Double);
                case System.Data.SqlDbType.Image:
                    return typeof(Byte);
                case System.Data.SqlDbType.Int:
                    return typeof(Int32);
                case System.Data.SqlDbType.Money:
                    return typeof(Decimal);
                case System.Data.SqlDbType.NChar:
                    return typeof(String);
                case System.Data.SqlDbType.NText:
                    return typeof(String);
                case System.Data.SqlDbType.NVarChar:
                    return typeof(String);
                case System.Data.SqlDbType.Real:
                    return typeof(Single);
                case System.Data.SqlDbType.SmallDateTime:
                    return typeof(DateTime);
                case System.Data.SqlDbType.SmallInt:
                    return typeof(Int16);
                case System.Data.SqlDbType.SmallMoney:
                    return typeof(Decimal);
                case System.Data.SqlDbType.Structured:
                    return typeof(Object);
                case System.Data.SqlDbType.Text:
                    return typeof(String);
                case System.Data.SqlDbType.Time:
                    return typeof(DateTime);
                case System.Data.SqlDbType.Timestamp:
                    return typeof(Byte);
                case System.Data.SqlDbType.TinyInt:
                    return typeof(Byte);
                case System.Data.SqlDbType.Udt:
                    return typeof(Object);
                case System.Data.SqlDbType.UniqueIdentifier:
                    return typeof(Guid);
                case System.Data.SqlDbType.VarBinary:
                    return typeof(Byte);
                case System.Data.SqlDbType.VarChar:
                    return typeof(String);
                case System.Data.SqlDbType.Variant:
                    return typeof(Object);
                case System.Data.SqlDbType.Xml:
                    return typeof(Object);
                default:
                    return typeof(String);
            }
        }

        /// <summary>
        /// 将字符串转化成 enum
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static System.Data.SqlDbType ToSqlDbType(string type)
        {
            System.Data.SqlDbType rtn = System.Data.SqlDbType.VarChar;

            if (type.ToLower().Trim() == "numeric") return System.Data.SqlDbType.Decimal;

            string[] s = Enum.GetNames(typeof(System.Data.SqlDbType));
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i].ToLower() == type.ToLower())
                {
                    rtn = (System.Data.SqlDbType)Enum.Parse(typeof(System.Data.SqlDbType), type, true);
                    break;
                }
            }
            return rtn;
        }
    }
}
