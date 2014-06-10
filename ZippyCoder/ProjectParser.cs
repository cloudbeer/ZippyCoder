using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZippyCoder.Entity;
using System.Xml;

namespace ZippyCoder
{
    public class ProjectParser
    {
        #region Load 3 Load(TreeView tvProject, XmlNode projectNode, XmlDocument doc)
        public static Project LoadOldProject(string file)
        {
            Project project = new Project();
            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            XmlNode projectNode = doc.ChildNodes[1];
            project.Namespace = projectNode.Attributes["NameSpace"].Value;
            project.Title = projectNode.Attributes["Title"].Value;
            project.Remark = projectNode.Attributes["Remark"].Value;


            foreach (XmlNode tableNode in projectNode.ChildNodes)
            {
                Table table = new Table();
                table.Name = tableNode.Attributes["Name"].Value;
                table.Remark = tableNode.Attributes["Remark"].Value;
                if (tableNode.Attributes["Title"] != null)
                    table.Title = tableNode.Attributes["Title"].Value;
                else
                    table.Title = table.Name;
                project.Tables.Add(table);
                foreach (XmlNode colNode in tableNode.ChildNodes)
                {
                    Col col = new Col();
                    col.Name = colNode.Attributes["Name"].Value;
                    col.AutoIncrease = ProjectParser.CBool(colNode.Attributes["AutoIncrease"].Value);
                    col.DataType = ProjectParser.CDataTypes(colNode.Attributes["DataType"].Value);
                    col.IsNull = ProjectParser.CBool(colNode.Attributes["IsNull"].Value);
                    col.IsPK = ProjectParser.CBool(colNode.Attributes["IsPK"].Value);
                    col.Unique = ProjectParser.CBool(colNode.Attributes["Unique"].Value);
                    col.Default = colNode.Attributes["Default"].Value;
                    col.Title = colNode.Attributes["Title"].Value;
                    col.Remark = colNode.Attributes["Remark"].Value;
                    bool renderHtml = ProjectParser.CBool(colNode.Attributes["RenderAsHtml"].Value);
                    if (renderHtml) col.RenderType = RenderTypes.Html;
                    else
                    {
                        col.RenderType = RenderTypes.TextBox;
                        col.CssClass = "textBox";
                        col.CssClassLength = "w10";
                    }
                    try
                    {
                        col.RefTable = colNode.Attributes["RefTable"].Value;
                    }
                    catch { }
                    try
                    {
                        col.RefCol = colNode.Attributes["RefCol"].Value;
                    }
                    catch { }
                    if (col.Title == null || col.Title == "")
                    {
                        col.Title = col.Name;
                    }
                    UIColTypes uiColType = 0;
                    if (col.DataType != System.Data.SqlDbType.Text)
                        uiColType = uiColType | UIColTypes.Listable;
                    if (!col.IsPK)
                    {
                        uiColType = uiColType | UIColTypes.Editable;
                        if (!string.IsNullOrEmpty(col.RefTable))
                            uiColType = uiColType | UIColTypes.Queryable;
                    }
                    uiColType = uiColType | UIColTypes.Detailable;

                    col.UIColType = uiColType;
                    string sLength = colNode.Attributes["MaxLength"].Value;
                    if (col.DataType == System.Data.SqlDbType.Decimal)
                    {
                        col.Length = "18,2";
                    }
                    else if (CInt(sLength) > 0 && col.DataType != System.Data.SqlDbType.Text && col.DataType != System.Data.SqlDbType.Int && col.DataType != System.Data.SqlDbType.Date &&
                        col.DataType != System.Data.SqlDbType.DateTime && col.DataType != System.Data.SqlDbType.DateTime2)
                    {
                        col.Length = sLength;
                    }
                    else
                    {
                        col.Length = "";
                    }
                    table.Cols.Add(col);
                }
            }

            return project;
        }
        #endregion

        public static string CString(object obj)
        {
            if (obj == null)
                return string.Empty;
            return obj.ToString();
        }
        public static int CInt(object obj)
        {
            if (obj == null)
                return 0;
            return Convert.ToInt32(obj);
        }
        public static bool CBool(object obj)
        {
            if (obj == null)
                return false;
            return Convert.ToBoolean(obj);
        }
        //public static System.Data.SqlDbType CDataTypes(object obj)
        //{
        //    if (obj == null)
        //        return System.Data.SqlDbType.VarChar;
        //    return (System.Data.SqlDbType)Convert.ChangeType(obj, typeof(System.Data.SqlDbType));
        //}

        public static System.Data.SqlDbType CDataTypes(string dbType)
        {
            switch (dbType)
            {
                case "Varchar":
                    return System.Data.SqlDbType.VarChar;
                case "Int":
                    return System.Data.SqlDbType.Int;
                case "Text":
                    return System.Data.SqlDbType.Text;
                case "Char":
                    return System.Data.SqlDbType.Char;
                case "Datetime":
                    return System.Data.SqlDbType.DateTime;
                case "Smalldatetime":
                    return System.Data.SqlDbType.DateTime;
                case "Float":
                    return System.Data.SqlDbType.Float;
                case "Money":
                    return System.Data.SqlDbType.Money;
                case "Smallmoney":
                    return System.Data.SqlDbType.SmallMoney;
                case "Numeric":
                    return System.Data.SqlDbType.Decimal;
                case "Decimal":
                    return System.Data.SqlDbType.Decimal;
                case "Smallint":
                    return System.Data.SqlDbType.SmallInt;
                case "Tinyint":
                    return System.Data.SqlDbType.TinyInt;
                case "Bit":
                    return System.Data.SqlDbType.Bit;
                case "NChar":
                    return System.Data.SqlDbType.NChar;
                case "NVarchar":
                    return System.Data.SqlDbType.NVarChar;
                case "NText":
                    return System.Data.SqlDbType.NText;
                default:
                    return System.Data.SqlDbType.VarChar;
            }
        }

    }
}
