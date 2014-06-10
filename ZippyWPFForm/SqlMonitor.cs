using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZippyWPFForm
{

    public class SqlMonitor
    {
        public static System.IO.StreamWriter OpenSqlFile()
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Xml文件|*.xml";
            dlg.Title = "请选择选择一个文件来记录sql变动日志";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(dlg.FileName, true, System.Text.Encoding.Default);
                return sw;
            }
            else
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter("c:\\sqlchangelog.sql", true);
                sw.WriteLine("----------------------------------");
                sw.WriteLine("----开始记录" + DateTime.Now + "----");
                sw.WriteLine("----------------------------------");
                System.Windows.MessageBox.Show("修改的日志文件记录在：c:\\sqlchangelog.sql");
                return sw;
            }
        }
        public static System.IO.StreamWriter OpenOracleFile()
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Sql 文件|*.sql";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(dlg.FileName, true, System.Text.Encoding.Default);
                return sw;
            }
            else
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter("c:\\orachangelog.sql", false);
                System.Windows.MessageBox.Show("修改的日志文件记录在：c:\\orachangelog.sql");
                return sw;
            }
        }

        public static void WriteSqlServerChangeLog(System.IO.StreamWriter sw, ZippyCoder.Entity.Table table, ZippyCoder.Entity.Col col, ChangeTypes changeType, string oldColName)
        {

            if (sw == null) return;
            if (table == null) return;
            switch (changeType)
            {
                case ChangeTypes.AddCol:
                    if (col != null)
                    {
                        sw.Write("ALTER TABLE [" + table.Name + "] ");
                        sw.Write("ADD [" + col.Name + "] ");

                        if (!string.IsNullOrEmpty(col.Length))
                            sw.Write("(" + col.Length + ") ");
                        if (col.AutoIncrease)
                            sw.Write("IDENTITY(1,1) ");
                        if (col.IsPK)
                            sw.Write("PRIMARY KEY ");
                        if (col.Unique)
                            sw.Write("UNIQUE ");
                        if (col.IsNull)
                            sw.Write("NULL ");
                        else
                            sw.Write("NOT NULL ");

                        if (!string.IsNullOrEmpty(col.Default))
                            sw.Write("DEFAULT (" + col.Default + ") ");
                        if ((!string.IsNullOrEmpty(col.RefTable)) && (!string.IsNullOrEmpty(col.RefCol)))
                        {
                            sw.Write("REFERENCES [" + col.RefTable + "]([" + col.RefCol + "])");
                        }
                        sw.WriteLine();
                        sw.WriteLine("GO");
                        sw.WriteLine("----------------------------------");
                    }
                    break;
                case ChangeTypes.AlterCol:
                    if (col != null)
                    {
                        if (col.Name == oldColName)
                        {
                            sw.Write("ALTER TABLE [" + table.Name + "] ");
                            sw.Write("ALTER COLUMN [" + col.Name + "] ");
                            sw.Write(col.DataType.ToString());
                            if (!string.IsNullOrEmpty(col.Length))
                                sw.Write("(" + col.Length + ") ");
                            if (col.AutoIncrease)
                                sw.Write("IDENTITY(1,1) ");
                            if (col.IsPK)
                                sw.Write("PRIMARY KEY ");
                            if (col.Unique)
                                sw.Write("UNIQUE ");
                            if (col.IsNull)
                                sw.Write("NULL ");
                            else
                                sw.Write("NOT NULL ");

                            if (col.Default != "")
                                sw.Write("DEFAULT (" + col.Default + ") ");
                            if (col.RefTable != null && col.RefTable != "" && col.RefCol != null && col.RefCol != "")
                            {
                                sw.Write("REFERENCES [" + col.RefTable + "]([" + col.RefCol + "])");
                            }
                            sw.WriteLine();
                            sw.WriteLine("GO");
                            sw.WriteLine("----------------------------------");
                        }
                        else
                        {
                            sw.Write("ALTER TABLE [" + table.Name + "] ");
                            sw.Write("DROP COLUMN [" + oldColName + "] ");
                            sw.WriteLine();
                            sw.WriteLine("GO");
                            sw.WriteLine("----------------------------------");
                            sw.Write("ALTER TABLE [" + table.Name + "] ");
                            sw.Write("ADD [" + col.Name + "] ");
                            sw.Write(col.DataType.ToString() + " ");
                            if (!string.IsNullOrEmpty(col.Length))
                                sw.Write("(" + col.Length + ") ");
                            if (col.AutoIncrease)
                                sw.Write("IDENTITY(1,1) ");
                            if (col.IsPK)
                                sw.Write("PRIMARY KEY ");
                            if (col.Unique)
                                sw.Write("UNIQUE ");
                            if (col.IsNull)
                                sw.Write("NULL ");
                            else
                                sw.Write("NOT NULL ");

                            if (!string.IsNullOrEmpty(col.Default))
                                sw.Write("DEFAULT (" + col.Default + ") ");
                            if ((!string.IsNullOrEmpty(col.RefTable)) && (!string.IsNullOrEmpty(col.RefCol)))
                            {
                                sw.Write("REFERENCES [" + col.RefTable + "]([" + col.RefCol + "])");
                            }
                            sw.WriteLine();
                            sw.WriteLine("GO");
                            sw.WriteLine("----------------------------------");
                        }
                    }
                    break;
                case ChangeTypes.DropCol:
                    if (col != null)
                    {
                        sw.Write("ALTER TABLE [" + table.Name + "] ");
                        sw.Write("DROP COLUMN [" + col.Name + "] ");
                        sw.WriteLine();
                        sw.WriteLine("GO");
                        sw.WriteLine("----------------------------------");
                    }
                    break;
                case ChangeTypes.DropTable:
                    sw.Write("DROP TABLE [" + table.Name + "] ");
                    sw.WriteLine();
                    sw.WriteLine("GO");
                    sw.WriteLine("----------------------------------");
                    break;



            }
            sw.Write("");

        }
    }
}
