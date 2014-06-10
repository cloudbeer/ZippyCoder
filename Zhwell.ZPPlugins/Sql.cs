using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZippyCoder;
using ZippyCoder.Entity;
using System.IO;


namespace Zhwell.ZPPlugins
{
    /// <summary>
    /// Sql 建表
    /// </summary>
    [PluginIndicator(Title = "Sql 建表")]
    public class Sql : AbstractCoder
    {
        System.Windows.Forms.FolderBrowserDialog dlgSavePath = new System.Windows.Forms.FolderBrowserDialog();
        public override void Create()
        {
            System.Windows.Forms.MessageBox.Show("请选择要输出的目录");
            if (dlgSavePath.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            string outputPath = dlgSavePath.SelectedPath;


            try
            {
                System.Text.StringBuilder sbDrop = new System.Text.StringBuilder();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                //按照外键引用排序
                List<Table> tablePKs = new List<Table>();
                foreach (Table tb in project.Tables)
                {
                    this.SortTable(tablePKs, tb);
                }

                //drop表
                for (int m = tablePKs.Count - 1; m >= 0; m--)
                {
                    Table table = tablePKs[m];
                    sbDrop.Append("drop table [" + table.Name + "];");
                    sbDrop.Append("\r\n");
                    sbDrop.Append("go");
                    sbDrop.Append("\r\n");
                }

                string savingPath = System.IO.Path.Combine(outputPath, project.Namespace + ".sql");
                using (Stream s = File.Open(savingPath, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(s, System.Text.Encoding.Default);
                    sw.WriteLine(sbDrop.ToString());
                    //生成表的时候刚好相反
                    sw.WriteLine(MakeSql(tablePKs).ToString());
                    sw.WriteLine("---------------------------------");
                    //sw.WriteLine(project.Sql);
                    //sw.WriteLine(MakeSql(tableFKs).ToString());
                    sw.Close();
                    s.Close();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private System.Text.StringBuilder MakeSql(List<Table> tables)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (Table table in tables)
            {
                sb.Append("CREATE TABLE [" + table.Name + "] (");
                sb.Append("\r\n");
                //throw new Exception(
                for (int j = 0; j < table.Cols.Count; j++)
                {
                    //sb.Remove(0, sb.Length);
                    Col col = table.Cols[j];
                    sb.Append("    [" + col.Name + "] ");
                    sb.Append(col.DataType.ToString().ToUpper() + " ");

                    if (!string.IsNullOrEmpty(col.Length))
                    {
                        if (col.DataType != System.Data.SqlDbType.Text && col.DataType != System.Data.SqlDbType.Int && col.DataType != System.Data.SqlDbType.Date &&
                           col.DataType != System.Data.SqlDbType.DateTime && col.DataType != System.Data.SqlDbType.DateTime2)
                            sb.Append("(" + col.Length + ") ");
                    }

                    if (col.AutoIncrease)
                        sb.Append("IDENTITY(1,1) ");

                    if (col.IsPK)
                        sb.Append("PRIMARY KEY ");
                    else if (col.Unique)
                        sb.Append("UNIQUE ");
                    else if (col.IsNull)
                        sb.Append("NULL ");
                    else
                        sb.Append("NOT NULL ");

                    if (col.Default != "")
                        sb.Append("DEFAULT " + col.Default + " ");
                    if (j != table.Cols.Count - 1)
                        sb.Append(",");
                    sb.AppendLine();
                }
                sb.Append(");");
                sb.AppendLine();
                sb.Append("go");
                sb.AppendLine();

                foreach (Col col in table.Cols)
                {
                    if (!string.IsNullOrEmpty(col.RefTable) && !string.IsNullOrEmpty(col.RefCol))
                    {
                        string fkName = "FK__" + table.Name + "__" + col.Name + "__" + col.RefTable + "__" + col.RefCol;
                        sb.AppendLine("----------外键约束--------------");
                        sb.AppendLine(@"ALTER TABLE [" + table.Name + @"] WITH CHECK ADD CONSTRAINT [" + fkName + "] FOREIGN KEY([" + col.Name + "]) REFERENCES [" + col.RefTable + "] ([" + col.RefCol + "])");
                        
                        if (col.FkDeleteCascade)
                        {
                            sb.AppendLine("ON DELETE CASCADE");
                        }
                        sb.AppendLine("GO");
                        if (col.FkWithNoCheck)
                        {
                            sb.AppendLine("ALTER TABLE [" + table.Name + @"] NOCHECK CONSTRAINT [" + fkName + "]");
                            sb.Append("GO");
                        }
                        sb.AppendLine();

                        //sb.Append("REFERENCES [" + col.RefTable + "]([" + col.RefCol + "])");
                    }
                }

            }
            return sb;
        }

        //private void AddRef()
        //{
        //    foreach (
        //}


        private void SortTable(List<Table> tCollection, Table tab)
        {
            if (IsExist(tCollection, tab))
                return;

            foreach (Col col in tab.Cols)
            {
                string lastErrorTable = "表：" + tab.Name + " 参考表：" + col.RefTable + "。";
                //System.Diagnostics.Debug.WriteLine(col.RefTable);
                if (col.RefTable != null && col.RefTable != "")
                {
                    Table tTB = GetTableByName(col.RefTable);

                    if (tTB == null)
                    {
                        throw new Exception("参考表不存在。\r\n" + lastErrorTable);
                    }

                    SortTable(tCollection, tTB);
                }
            }

            tCollection.Add(tab);
        }

        private bool IsExist(List<Table> tCollection, Table tab)
        {
            foreach (Table tb in tCollection)
            {
                if (tb == tab)
                {
                    return true;
                }
            }

            return false;
        }

        private Table GetTableByName(String tname)
        {
            foreach (Table tb in project.Tables)
            {
                if (tb.Name == tname)
                {
                    return tb;
                }
            }

            return null;
        }
    }
}
