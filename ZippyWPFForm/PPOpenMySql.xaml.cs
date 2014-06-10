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
using MySql.Data.MySqlClient;

namespace ZippyWPFForm
{
    /// <summary>
    /// PPOpenSqlServer.xaml 的交互逻辑
    /// </summary>
    public partial class PPOpenMySql : Page
    {
        private MainForm _Owner;
        private ZippyCoder.Entity.Project project;

        public PPOpenMySql()
        {
            InitializeComponent();
        }
        public PPOpenMySql(MainForm owner)
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
            string conStr1 = "Server={0};Port={1};Uid={2};Pwd={3};";
            //string conStr2 = "Persist Security Info=False;Integrated Security=true;Server={0};Initial Catalog=master";
            string conStr = "";
            if (string.IsNullOrEmpty(tbxUserName.Text.Trim()))
            {
                MessageBox.Show("所有项目都要填写。");
                return;
            }
            conStr = string.Format(conStr1, tbxServer.Text, tbxPort.Text, tbxUserName.Text, tbxPassword.Text);

            MySqlConnection con = new MySqlConnection(conStr);
            try
            {

                con.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            MySqlCommand cmd = new MySqlCommand("show databases", con);
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

            string conStr1 = "Server={0};Port={1};Uid={2};Pwd={3};Database={4}";

            string conStr = string.Format(conStr1, tbxServer.Text, tbxPort.Text, tbxUserName.Text, tbxPassword.Text, ddlDatabase.Text);

            MessageBox.Show(conStr);

            MySqlConnection con = new MySqlConnection(conStr);
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
            MySqlCommand cmd = new MySqlCommand("show tables", con);
            IDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tableNames.Add(reader.GetString(0));
            }
            reader.Close();

            #region 查询列结构
            foreach (string strTableName in tableNames)
            {
                ZippyCoder.Entity.Table table = new ZippyCoder.Entity.Table();
                table.Name = strTableName;
                table.Title = strTableName;
                project.Tables.Add(table);


                string sql = "SHOW COLUMNS FROM " + strTableName;
                MySqlCommand cmdColProperty = new MySqlCommand(sql, con);
                IDataReader readerCol = cmdColProperty.ExecuteReader();

                List<ZippyCoder.Entity.Col> fkCols = new List<ZippyCoder.Entity.Col>();

                while (readerCol.Read())
                {
                    object _colName = readerCol.GetValue(0);
                    object _mysqlType = readerCol.GetValue(1);
                    object _canNull = readerCol.GetValue(2);
                    object _isKey = readerCol.GetValue(3);
                    object _defVal = readerCol.GetValue(4);
                    object _extra = readerCol.GetValue(5);
                    string colName = _colName == null ? "" : _colName.ToString();
                    string mysqlType = _mysqlType == null ? "" : _mysqlType.ToString();
                    string canNull = _canNull == null ? "" : _canNull.ToString();
                    string isKey = _isKey == null ? "" : _isKey.ToString();
                    string defVal = _defVal == null ? "" : _defVal.ToString();
                    string extra = _extra == null ? "" : _extra.ToString();
                    string dataType = string.Empty, dataLen = string.Empty;
                    var match = System.Text.RegularExpressions.Regex.Match(mysqlType, @"([\w]+).*?([\d\,]+).*");
                    var matchCount = match.Groups.Count;
                    if (matchCount > 1)
                        dataType = match.Groups[1].Value;
                    if (matchCount > 2)
                        dataLen = match.Groups[2].Value;

                    if (string.IsNullOrEmpty(dataType))
                        dataType = mysqlType;



                    ZippyCoder.Entity.Col col = table.Exists(colName);
                    if (col == null)
                    {
                        col = new ZippyCoder.Entity.Col();
                        table.Cols.Add(col);
                        col.Parent = table;
                    }
                    col.Name = colName;
                    col.Title = colName;
                    col.DataType = ZippyCoder.TypeConverter.ToSqlDbType(dataType);
                    col.Default = (defVal == "NULL" ? "" : defVal);
                    col.Length = dataLen;
                    col.IsPK = (isKey != null && isKey.ToLower() == "pri");
                    col.IsNull = (canNull != null && canNull.ToLower() == "yes");
                    col.AutoIncrease = (extra != null && extra.ToLower() == "auto_increment");
                }
                readerCol.Close();

            }

            #endregion

            con.Close();

            _Owner.Project = project;
            _Owner.UpdateUI();
        }
    }
}
