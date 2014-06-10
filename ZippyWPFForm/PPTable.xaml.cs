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

namespace ZippyWPFForm
{
    /// <summary>
    /// PPTable.xaml 的交互逻辑
    /// </summary>
    public partial class PPTable : Page
    {
        private MainForm _Owner;

        private ZippyCoder.Entity.Table _Table;
        public ZippyCoder.Entity.Table Table
        {
            get
            {
                if (_Table == null) _Table = new ZippyCoder.Entity.Table();
                _Table.Name = tbxName.Text;
                _Table.Title = tbxTitle.Text;
                _Table.Remark = tbxRemark.Text;
                return _Table;
            }
            set
            {
                tbxName.Text = value.Name;
                tbxTitle.Text = value.Title;
                tbxRemark.Text = value.Remark;
                _Table = value;
            }
        }
        public PPTable()
        {
            InitializeComponent();
        }
        public PPTable(MainForm owner)
        {
            InitializeComponent();
            _Owner = owner;
            this.Visibility = Visibility.Hidden;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            _Owner.UpdateTableNode(Table);
            this.Visibility = Visibility.Hidden;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

            this.Visibility = Visibility.Hidden;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            if (_Owner.Operation == ChangeTypes.AddTable)
            {
                xTitle.Content = "新建表/实体";
            }
            else
            {
                xTitle.Content = "修改表/实体";
            }
        }
    }
}
