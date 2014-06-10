using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;

namespace ZippyWPFForm
{
    /// <summary>
    /// PPOpenSqlServer.xaml 的交互逻辑
    /// </summary>
    public partial class PPOpenSqlServer : Page
    {
        private MainForm _Owner;
        private ZippyCoder.Entity.Project project;

        public PPOpenSqlServer()
        {
            InitializeComponent();
        }
        public PPOpenSqlServer(MainForm owner)
        {
            InitializeComponent();
            _Owner = owner;
            project = new ZippyCoder.Entity.Project();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }


        private void ddlDatabase_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ddlDatabase.Items.Count > 0) return;
            //ddlDatabase.Items.Clear();
            string conStr1 = "Persist Security Info=False;User ID={1};Password={2};Server={0};Initial Catalog=master";
            string conStr2 = "Persist Security Info=False;Integrated Security=true;Server={0};Initial Catalog=master";
            string conStr = "";
            if (string.IsNullOrEmpty(tbxUserName.Text.Trim()))
            {
                conStr = string.Format(conStr2, tbxServer.Text);
            }
            else
            {
                conStr = string.Format(conStr1, tbxServer.Text, tbxUserName.Text, tbxPassword.Text);
            }

            SqlConnection con = new SqlConnection(conStr);
            try
            {

                con.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            SqlCommand cmd = new SqlCommand("select name from sysdatabases", con);
            IDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ddlDatabase.Items.Add(reader.GetString(0));
            }
            reader.Close();
            con.Close();
        }

        private void ddlDatabase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnSave.IsEnabled = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(ddlDatabase.Text)) return;

            project.Namespace = ddlDatabase.Text;
            project.Title = ddlDatabase.Text;

            string conStr1 = "Persist Security Info=False;User ID={1};Password={2};Server={0};Initial Catalog={3}";
            string conStr2 = "Persist Security Info=False;Integrated Security=true;Server={0};Initial Catalog={1}";

            string conStr = "";
            if (string.IsNullOrEmpty(tbxUserName.Text.Trim()))
            {
                conStr = string.Format(conStr2, tbxServer.Text, ddlDatabase.Text);
            }
            else
            {
                conStr = string.Format(conStr1, tbxServer.Text, tbxUserName.Text, tbxPassword.Text, ddlDatabase.Text);
            }

            SqlConnection con = new SqlConnection(conStr);
            try
            {

                con.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            List<string> tableNames = new List<string>();
            SqlCommand cmd = new SqlCommand("select name from sysobjects where xtype='U' order by name", con);
            IDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tableNames.Add(reader.GetString(0));
            }
            reader.Close();

            foreach (string strTableName in tableNames)
            {

                ZippyCoder.Entity.Table table = new ZippyCoder.Entity.Table();
                table.Name = strTableName;
                table.Title = strTableName;
                project.Tables.Add(table);

                string sql = @"SELECT 
INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME, 
INFORMATION_SCHEMA.COLUMNS.DATA_TYPE,
INFORMATION_SCHEMA.COLUMNS.COLUMN_DEFAULT, 
INFORMATION_SCHEMA.COLUMNS.CHARACTER_MAXIMUM_LENGTH, 
INFORMATION_SCHEMA.COLUMNS.NUMERIC_PRECISION, 
INFORMATION_SCHEMA.COLUMNS.NUMERIC_SCALE, 
INFORMATION_SCHEMA.COLUMNS.IS_NULLABLE, 
COLUMNPROPERTY(OBJECT_ID('" + strTableName + @"'), INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME, 'IsIdentity') AS IsIdentity,
INFORMATION_SCHEMA.TABLE_CONSTRAINTS.CONSTRAINT_TYPE
FROM INFORMATION_SCHEMA.COLUMNS LEFT JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ON INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME = INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.COLUMN_NAME AND INFORMATION_SCHEMA.COLUMNS.TABLE_NAME = INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.TABLE_NAME LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS ON INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.CONSTRAINT_NAME = INFORMATION_SCHEMA.TABLE_CONSTRAINTS.CONSTRAINT_NAME AND INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.TABLE_NAME = INFORMATION_SCHEMA.TABLE_CONSTRAINTS.TABLE_NAME WHERE INFORMATION_SCHEMA.COLUMNS.TABLE_NAME = '" + strTableName + "'";


                //System.Diagnostics.Debug.WriteLine(sql);

                //return;


                SqlCommand cmdColProperty = new SqlCommand(sql, con);
                IDataReader readerCol = cmdColProperty.ExecuteReader();

                List<ZippyCoder.Entity.Col> fkCols = new List<ZippyCoder.Entity.Col>();

                while (readerCol.Read())
                {
                    string colName = readerCol.GetValue(0).ToString();
                    ZippyCoder.Entity.Col col = table.Exists(colName);
                    if (col == null)
                    {
                        col = new ZippyCoder.Entity.Col();
                        table.Cols.Add(col);
                        col.Parent = table;
                    }
                    col.Name = colName;
                    col.Title = colName;
                    col.DataType = ZippyCoder.TypeConverter.ToSqlDbType(readerCol.GetValue(1).ToString());
                    col.Default = readerCol.GetValue(2).ToString();
                    if (col.DataType == SqlDbType.VarChar || col.DataType == SqlDbType.NVarChar || col.DataType == SqlDbType.Char || col.DataType == SqlDbType.NChar)
                    {
                        col.Length = readerCol.GetValue(3).ToString();
                    }
                    else if (col.DataType == SqlDbType.Decimal)
                    {
                        col.Length = "(" + readerCol.GetValue(4).ToString() + "," + readerCol.GetValue(5).ToString() + ")";
                    }
                    if ((readerCol.GetValue(6).ToString().ToUpper() == "NO"))
                        col.IsNull = false;
                    if (readerCol.GetValue(7).ToString() == "1")
                        col.AutoIncrease = true;
                    if (readerCol.GetValue(8).ToString() == "PRIMARY KEY")
                        col.IsPK = true;
                    if (readerCol.GetValue(8).ToString() == "UNIQUE")
                        col.Unique = true;
                    if (readerCol.GetValue(8).ToString() == "FOREIGN KEY") //将有外键约束的列记录下来，待查。
                    {
                        fkCols.Add(col);
                    }
                }
                readerCol.Close();

            }

            con.Close();

            _Owner.Project = project;
            _Owner.UpdateUI();
        }
    }
}
