using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace ZippyWPFForm
{
    public class Helper
    {
        #region 懒人

        public static void CreateLazyCols(TreeViewItem tviTable, ZippyCoder.Entity.Table table)
        {
            TreeViewItem tviCol;
            ZippyCoder.Entity.Col col;

            col = new ZippyCoder.Entity.Col();
            table.Cols.Add(col);
            col.Name = "id";
            col.IsPK = true;
            col.IsNull = false;
            col.Unique = true;
            col.AutoIncrease = true;
            col.CssClass = "textBox";
            col.CssClassLength = "w1";
            col.DataType = System.Data.SqlDbType.BigInt;
            col.Title = "编号";
            col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable | ZippyCoder.Entity.UIColTypes.Listable | ZippyCoder.Entity.UIColTypes.Sortable;
            tviCol = new TreeViewItem();
            tviCol.Header = "编号[id]";
            tviCol.Tag = col;
            tviTable.Items.Add(tviCol);


            //col = new ZippyCoder.Entity.Col();
            //table.Cols.Add(col);
            //col.Name = "TenantID";
            //col.IsPK = false;
            //col.IsNull = false;
            //col.Unique = false;
            //col.AutoIncrease = false;
            //col.IsIndex = true;
            //col.Default = "'00000000-0000-0000-0000-000000000000'";
            //col.CssClass = "";
            //col.CssClassLength = "";
            //col.DataType = System.Data.SqlDbType.UniqueIdentifier;
            //col.Title = "租户";
            //col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable;
            //tviCol = new TreeViewItem();
            //tviCol.Header = "租户[TenantID]";
            //tviCol.Tag = col;
            //tviTable.Items.Add(tviCol);


            col = new ZippyCoder.Entity.Col();
            table.Cols.Add(col);
            col.Name = table.Name.ToLower() + "_type";
            col.DataType = System.Data.SqlDbType.Int;
            col.Default = "1";
            col.Title = "类型";
            col.EnumType = table.Name + "Types";
            col.ResourceType = "Resources.X";
            col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable | ZippyCoder.Entity.UIColTypes.Queryable;
            col.RenderType = ZippyCoder.Entity.RenderTypes.CheckBoxList;
            tviCol = new TreeViewItem();
            tviCol.Header = col.Title + "[" + col.Name + "]";
            tviCol.Tag = col;
            tviTable.Items.Add(tviCol);

            col = new ZippyCoder.Entity.Col();
            table.Cols.Add(col);
            col.Name = table.Name.ToLower() + "_status";
            col.DataType = System.Data.SqlDbType.Int;
            col.Default = "1";
            col.Title = "状态";
            col.EnumType = table.Name + "Status";
            col.ResourceType = "Resources.X";
            col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable | ZippyCoder.Entity.UIColTypes.Queryable;
            col.RenderType = ZippyCoder.Entity.RenderTypes.RadioButtonList;
            tviCol = new TreeViewItem();
            tviCol.Header = col.Title + "[" + col.Name + "]";
            tviCol.Tag = col;
            tviTable.Items.Add(tviCol);

            //col = new ZippyCoder.Entity.Col();
            //table.Cols.Add(col);
            //col.Name = "display_order";
            //col.CssClass = "textBox";
            //col.CssClassLength = "w1";
            //col.DataType = System.Data.SqlDbType.Int;
            //col.Default = "0";
            //col.Title = "排列顺序";
            //col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable | ZippyCoder.Entity.UIColTypes.Editable | ZippyCoder.Entity.UIColTypes.Sortable;
            //tviCol = new TreeViewItem();
            //tviCol.Header = "排列顺序[display_order]";
            //tviCol.Tag = col;
            //tviTable.Items.Add(tviCol);

            col = new ZippyCoder.Entity.Col();
            table.Cols.Add(col);
            col.Name = "create_date";
            col.AutoIncrease = false;
            col.CssClass = "textBox";
            col.CssClassLength = "w3";
            col.DataType = System.Data.SqlDbType.DateTime;
            col.Default = "(GetDate())";
            col.Title = "创建时间";
            col.RenderType = ZippyCoder.Entity.RenderTypes.TextBox;
            col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable | ZippyCoder.Entity.UIColTypes.Listable 
                | ZippyCoder.Entity.UIColTypes.Queryable | ZippyCoder.Entity.UIColTypes.Sortable;
            tviCol = new TreeViewItem();
            tviCol.Header = "创建时间[create_date]";
            tviCol.Tag = col;
            tviTable.Items.Add(tviCol);

            col = new ZippyCoder.Entity.Col();
            table.Cols.Add(col);
            col.Name = "creator";
            col.CssClass = "textBox";
            col.CssClassLength = "w1";
            col.DataType = System.Data.SqlDbType.BigInt; // System.Data.SqlDbType.UniqueIdentifier;
            //col.Default = "0";
            col.Title = "创建人";
            col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable;
            tviCol = new TreeViewItem();
            tviCol.Header = "创建人[creator]";
            tviCol.Tag = col;
            tviTable.Items.Add(tviCol);

            col = new ZippyCoder.Entity.Col();
            table.Cols.Add(col);
            col.Name = "update_date";
            col.AutoIncrease = false;
            col.CssClass = "textBox";
            col.CssClassLength = "w3";
            col.DataType = System.Data.SqlDbType.DateTime;
            col.Title = "更新时间";
            col.RenderType = ZippyCoder.Entity.RenderTypes.TextBox;
            col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable;
            tviCol = new TreeViewItem();
            tviCol.Header = "更新时间[update_date]";
            tviCol.Tag = col;
            tviTable.Items.Add(tviCol);

            col = new ZippyCoder.Entity.Col();
            table.Cols.Add(col);
            col.Name = "updater";
            col.CssClass = "textBox";
            col.CssClassLength = "w1";
            col.DataType = System.Data.SqlDbType.BigInt; //System.Data.SqlDbType.UniqueIdentifier;
            //col.Default = "0";
            col.Title = "更新人";
            col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable;
            tviCol = new TreeViewItem();
            tviCol.Header = "更新人[updater]";
            tviCol.Tag = col;
            tviTable.Items.Add(tviCol);


        }


        public static void CreateLazyColsWin(TreeViewItem tviTable, ZippyCoder.Entity.Table table)
        {
            TreeViewItem tviCol;
            ZippyCoder.Entity.Col col;

            col = new ZippyCoder.Entity.Col();
            table.Cols.Add(col);
            col.Name = table.Name + "ID";
            col.IsPK = true;
            col.IsNull = false;
            col.Unique = true;
            col.AutoIncrease = true;
            col.CssClass = "textBox";
            col.CssClassLength = "w1";
            col.DataType = System.Data.SqlDbType.BigInt;
            col.Title = table.Title + "编号";
            col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable | ZippyCoder.Entity.UIColTypes.Listable | ZippyCoder.Entity.UIColTypes.Sortable;
            tviCol = new TreeViewItem();
            tviCol.Header = table.Title + "编号[" + table.Name + "ID]";
            tviCol.Tag = col;
            tviTable.Items.Add(tviCol);


            col = new ZippyCoder.Entity.Col();
            table.Cols.Add(col);
            col.Name = "TenantID";
            col.IsPK = false;
            col.IsNull = false;
            col.Unique = false;
            col.AutoIncrease = false;
            col.IsIndex = true;
            col.Default = "'00000000-0000-0000-0000-000000000000'";
            col.CssClass = "";
            col.CssClassLength = "";
            col.DataType = System.Data.SqlDbType.UniqueIdentifier;
            col.Title = "租户";
            col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable;
            tviCol = new TreeViewItem();
            tviCol.Header = "租户[TenantID]";
            tviCol.Tag = col;
            tviTable.Items.Add(tviCol);


            col = new ZippyCoder.Entity.Col();
            table.Cols.Add(col);
            col.Name = table.Name + "Type";
            col.DataType = System.Data.SqlDbType.Int;
            col.Default = "1";
            col.Title = table.Title + "类型";
            col.EnumType = table.Name + "Types";
            col.ResourceType = "Resources.X";
            col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable | ZippyCoder.Entity.UIColTypes.Queryable | ZippyCoder.Entity.UIColTypes.Editable | ZippyCoder.Entity.UIColTypes.Listable;
            col.RenderType = ZippyCoder.Entity.RenderTypes.CheckBoxList;
            tviCol = new TreeViewItem();
            tviCol.Header = col.Title + "[" + col.Name + "]";
            tviCol.Tag = col;
            tviTable.Items.Add(tviCol);

            col = new ZippyCoder.Entity.Col();
            table.Cols.Add(col);
            col.Name = table.Name + "Status";
            col.DataType = System.Data.SqlDbType.Int;
            col.Default = "1";
            col.Title = table.Title + "状态";
            col.EnumType = table.Name + "Status";
            col.ResourceType = "Resources.X";
            col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable | ZippyCoder.Entity.UIColTypes.Queryable | ZippyCoder.Entity.UIColTypes.Editable | ZippyCoder.Entity.UIColTypes.Listable;
            col.RenderType = ZippyCoder.Entity.RenderTypes.RadioButtonList;
            tviCol = new TreeViewItem();
            tviCol.Header = col.Title + "[" + col.Name + "]";
            tviCol.Tag = col;
            tviTable.Items.Add(tviCol);

            col = new ZippyCoder.Entity.Col();
            table.Cols.Add(col);
            col.Name = "DisplayOrder";
            col.CssClass = "textBox";
            col.CssClassLength = "w1";
            col.DataType = System.Data.SqlDbType.Int;
            col.Default = "0";
            col.Title = "排列顺序";
            col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable | ZippyCoder.Entity.UIColTypes.Editable | ZippyCoder.Entity.UIColTypes.Sortable;
            tviCol = new TreeViewItem();
            tviCol.Header = "排列顺序[DisplayOrder]";
            tviCol.Tag = col;
            tviTable.Items.Add(tviCol);

            col = new ZippyCoder.Entity.Col();
            table.Cols.Add(col);
            col.Name = "CreateDate";
            col.AutoIncrease = false;
            col.CssClass = "textBox";
            col.CssClassLength = "w3";
            col.DataType = System.Data.SqlDbType.DateTime;
            col.Default = "(GetDate())";
            col.Title = "创建时间";
            col.RenderType = ZippyCoder.Entity.RenderTypes.TextBox;
            col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable | ZippyCoder.Entity.UIColTypes.Listable | ZippyCoder.Entity.UIColTypes.Queryable | ZippyCoder.Entity.UIColTypes.Sortable;
            tviCol = new TreeViewItem();
            tviCol.Header = "创建时间[CreateDate]";
            tviCol.Tag = col;
            tviTable.Items.Add(tviCol);

            col = new ZippyCoder.Entity.Col();
            table.Cols.Add(col);
            col.Name = "Creator";
            col.CssClass = "textBox";
            col.CssClassLength = "w1";
            col.DataType = System.Data.SqlDbType.UniqueIdentifier;
            //col.Default = "0";
            col.Title = "创建人";
            col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable;
            tviCol = new TreeViewItem();
            tviCol.Header = "创建人[Creator]";
            tviCol.Tag = col;
            tviTable.Items.Add(tviCol);

            col = new ZippyCoder.Entity.Col();
            table.Cols.Add(col);
            col.Name = "UpdateDate";
            col.AutoIncrease = false;
            col.CssClass = "textBox";
            col.CssClassLength = "w3";
            col.DataType = System.Data.SqlDbType.DateTime;
            col.Title = "更新时间";
            col.RenderType = ZippyCoder.Entity.RenderTypes.TextBox;
            col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable;
            tviCol = new TreeViewItem();
            tviCol.Header = "更新时间[UpdateDate]";
            tviCol.Tag = col;
            tviTable.Items.Add(tviCol);

            col = new ZippyCoder.Entity.Col();
            table.Cols.Add(col);
            col.Name = "Updater";
            col.CssClass = "textBox";
            col.CssClassLength = "w1";
            col.DataType = System.Data.SqlDbType.UniqueIdentifier;
            //col.Default = "0";
            col.Title = "更新人";
            col.UIColType = ZippyCoder.Entity.UIColTypes.Detailable;
            tviCol = new TreeViewItem();
            tviCol.Header = "更新人[Updater]";
            tviCol.Tag = col;
            tviTable.Items.Add(tviCol);


        }
        #endregion


        public static void BatchTenantID(ZippyCoder.Entity.Col col)
        {
            if (col.Name == "TenantID" && !col.IsPK)
            {
                col.DataType = System.Data.SqlDbType.UniqueIdentifier;
                col.Default = "'00000000-0000-0000-0000-000000000000'";
                col.RenderType = ZippyCoder.Entity.RenderTypes.UrlQuery;
                col.CssClass = "";
                col.CssClassLength = "";
                //col.UIColType = col.UIColType & (~ZippyCoder.Entity.UIColTypes.Editable);
            }
            else if (col.Name == "Creator" || col.Name == "Updater")
            {
                col.DataType = System.Data.SqlDbType.UniqueIdentifier;
                col.RenderType = ZippyCoder.Entity.RenderTypes.Hidden;
                col.Default = "";
                col.CssClass = "";
                col.CssClassLength = "";
                //col.UIColType = col.UIColType & (~ZippyCoder.Entity.UIColTypes.Editable);
            }

            if (col.WidthPx == 0 && (!string.IsNullOrEmpty(col.CssClassLength)))
            {
                try
                {
                    int length = int.Parse(col.CssClassLength.Substring(1));
                    if (length <= 32)
                        col.WidthPx = length * 30;
                }
                catch { }
            }

        }

    }
}
