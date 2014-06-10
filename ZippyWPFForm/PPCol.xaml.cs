using System;
using System.Windows;
using System.Windows.Controls;

namespace ZippyWPFForm
{
    /// <summary>
    /// PPCol.xaml 的交互逻辑
    /// </summary>
    public partial class PPCol : Page
    {
        private MainForm _Owner;

        private string oldColName = string.Empty; //记录修改前的 col，改变列名 sql稍有变化
        private ZippyCoder.Entity.Col _Col;
        public ZippyCoder.Entity.Col Col
        {
            get
            {
                if (_Col == null)
                    _Col = new ZippyCoder.Entity.Col();

                _Col.Name = tbxName.Text;
                if (string.IsNullOrEmpty(_Col.Name))
                {
                    //tbxName.Focus();
                    throw new Exception("列名不能为空");
                }
                _Col.Title = tbxTitle.Text;
                if (string.IsNullOrEmpty(_Col.Title))
                    _Col.Title = _Col.Name;
                _Col.Remark = tbxRemark.Text;
                _Col.Default = tbxDefault.Text;
                _Col.Length = tbxLength.Text;
                _Col.IsNull = cbxIsNull.IsChecked.Value;
                _Col.IsPK = cbxPK.IsChecked.Value;
                _Col.AutoIncrease = cbxAutoIncreament.IsChecked.Value;
                _Col.Unique = cbxUnique.IsChecked.Value;
                _Col.RenderType = (ZippyCoder.Entity.RenderTypes)tbxRenderType.SelectedIndex;
                _Col.EnumType = tbxEnumType.Text;
                _Col.ResourceType = tbxResoureType.Text;
                _Col.FkDeleteCascade = cbxFKDeleteCascade.IsChecked.Value;
                _Col.FkWithNoCheck = cbxFKNoCheck.IsChecked.Value;
                _Col.DataType = (System.Data.SqlDbType)Enum.Parse(typeof(System.Data.SqlDbType), tbxDataType.SelectedValue.ToString());
                if (tbxFKTable.SelectedIndex > 0 && tbxFKCol.SelectedIndex >= 0)
                {
                    _Col.RefTable = tbxFKTable.SelectedValue.ToString();
                    _Col.RefCol = tbxFKCol.SelectedValue.ToString();
                    if (tbxFKColTitle.SelectedIndex >= 0)
                        _Col.RefColTextField = tbxFKColTitle.SelectedValue.ToString();
                    else
                        _Col.RefColTextField = tbxFKCol.SelectedValue.ToString();
                }
                else
                {
                    _Col.RefTable = string.Empty;
                    _Col.RefCol = string.Empty;
                }
                _Col.IsIndex = cbxIsIndex.IsChecked.Value;
                ZippyCoder.Entity.UIColTypes uiTypes = 0;
                if (cbxQueryable.IsChecked.Value)
                {
                    uiTypes = uiTypes | ZippyCoder.Entity.UIColTypes.Queryable;
                }
                if (cbxSortable.IsChecked.Value)
                {
                    uiTypes = uiTypes | ZippyCoder.Entity.UIColTypes.Sortable;
                }
                if (cbxListable.IsChecked.Value)
                {
                    uiTypes = uiTypes | ZippyCoder.Entity.UIColTypes.Listable;
                }
                if (cbxDetailable.IsChecked.Value)
                {
                    uiTypes = uiTypes | ZippyCoder.Entity.UIColTypes.Detailable;
                }
                if (cbxEditable.IsChecked.Value)
                {
                    uiTypes = uiTypes | ZippyCoder.Entity.UIColTypes.Editable;
                }
                _Col.UIColType = uiTypes;

                _Col.CssClass = tbxCssClass.Text;
                _Col.CssClassLength = tbxCssClassLength.Text;
                try
                {
                    _Col.WidthPx = int.Parse(tbxWidthPx.Text);
                }
                catch { }
                return _Col;
            }
            set
            {
                oldColName = value.Name;
                tbxName.Text = value.Name;
                //tbxName.Focus();
                tbxTitle.Text = value.Title;
                tbxRemark.Text = value.Remark;
                tbxDefault.Text = value.Default;
                tbxLength.Text = value.Length;
                cbxAutoIncreament.IsChecked = value.AutoIncrease;
                cbxIsNull.IsChecked = value.IsNull;
                cbxPK.IsChecked = value.IsPK;
                cbxUnique.IsChecked = value.Unique;
                tbxDataType.SelectedValue = value.DataType.ToString();
                tbxRenderType.SelectedItem = value.RenderType.ToString();
                tbxEnumType.Text = value.EnumType;
                tbxResoureType.Text = value.ResourceType;
                cbxFKNoCheck.IsChecked = value.FkWithNoCheck;
                cbxFKDeleteCascade.IsChecked = value.FkDeleteCascade;
                cbxIsIndex.IsChecked = value.IsIndex;
                tbxWidthPx.Text = value.WidthPx.ToString();
                if ((value.UIColType & ZippyCoder.Entity.UIColTypes.Editable) == ZippyCoder.Entity.UIColTypes.Editable)
                {
                    cbxEditable.IsChecked = true;
                }
                else
                {
                    cbxEditable.IsChecked = false;
                }
                if ((value.UIColType & ZippyCoder.Entity.UIColTypes.Detailable) == ZippyCoder.Entity.UIColTypes.Detailable)
                {
                    cbxDetailable.IsChecked = true;
                }
                else
                {
                    cbxDetailable.IsChecked = false;
                }
                if ((value.UIColType & ZippyCoder.Entity.UIColTypes.Listable) == ZippyCoder.Entity.UIColTypes.Listable)
                {
                    cbxListable.IsChecked = true;
                }
                else
                {
                    cbxListable.IsChecked = false;
                }
                if ((value.UIColType & ZippyCoder.Entity.UIColTypes.Queryable) == ZippyCoder.Entity.UIColTypes.Queryable)
                {
                    cbxQueryable.IsChecked = true;
                }
                else
                {
                    cbxQueryable.IsChecked = false;
                }
                if ((value.UIColType & ZippyCoder.Entity.UIColTypes.Sortable) == ZippyCoder.Entity.UIColTypes.Sortable)
                {
                    cbxSortable.IsChecked = true;
                }
                else
                {
                    cbxSortable.IsChecked = false;
                }


                BindFKTable();


                if (value.RefTable != null)
                {
                    tbxFKTable.SelectedValue = value.RefTable;
                    BindFKCol(tbxFKCol, value.RefTable, value.RefCol);
                    BindFKCol(tbxFKColTitle, value.RefTable, value.RefColTextField);
                }
                if (string.IsNullOrEmpty(value.CssClassLength) && value.RenderType == ZippyCoder.Entity.RenderTypes.TextBox) value.CssClassLength = "w100";
                tbxCssClassLength.Text = value.CssClassLength;
                if (string.IsNullOrEmpty(value.CssClass) && value.RenderType == ZippyCoder.Entity.RenderTypes.TextBox) value.CssClass = "textBox";
                tbxCssClass.Text = value.CssClass;


                _Col = value;
            }
        }

        private void BindFKTable()
        {
            tbxFKTable.Items.Clear();
            //tbxFKCol.Items.Clear();
            tbxFKTable.Items.Add("不映射");
            foreach (ZippyCoder.Entity.Table table in _Project.Tables)
            {
                tbxFKTable.Items.Add(table.Name);
            }
        }

        private void BindFKCol(ComboBox cbx, string tableName, string colName)
        {
            ZippyCoder.Entity.Table table = _Project.FindTable(tableName);
            if (table != null)
            {
                foreach (ZippyCoder.Entity.Col col in table.Cols)
                {
                    cbx.Items.Add(col.Name);
                }
                cbx.SelectedValue = colName;
            }
        }

        private ZippyCoder.Entity.Project _Project;
        public ZippyCoder.Entity.Project Project
        {
            get { return _Project; }
            set
            {
                _Project = value;
                BindFKTable();
            }
        }

        public PPCol()
        {
            InitializeComponent();
        }
        public PPCol(MainForm owner)
        {
            InitializeComponent();
            _Owner = owner;
            this.Visibility = Visibility.Hidden;

            tbxDataType.ItemsSource = Enum.GetNames(typeof(System.Data.SqlDbType));
            tbxRenderType.ItemsSource = Enum.GetNames(typeof(ZippyCoder.Entity.RenderTypes));
            tbxCssClassLength.ItemsSource = Enum.GetNames(typeof(ZippyCoder.Entity.CssClassWidth));
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _Owner.UpdateColNode(Col);
                _Owner.WriteSqlLog(_Col, oldColName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "发生错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            this.Visibility = Visibility.Hidden;
            _Owner.Operation = ChangeTypes.None;
        }

        private void tbxFKTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbxFKCol.Items.Clear();
            tbxFKColTitle.Items.Clear();
            if (sender == null) return;
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedValue == null) return;
            ZippyCoder.Entity.Table table = _Project.FindTable(cb.SelectedValue.ToString());
            if (table != null)
            {
                foreach (ZippyCoder.Entity.Col col in table.Cols)
                {
                    tbxFKCol.Items.Add(col.Name);
                    tbxFKColTitle.Items.Add(col.Name);
                }
                tbxFKCol.SelectedValue = _Col.RefCol;
            }
            //MessageBox.Show(cb.SelectedValue.ToString());
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_Owner.Operation == ChangeTypes.AddCol)
            {
                xTitle.Content = "新建字段/属性";
            }
            else
            {
                xTitle.Content = "修改字段/属性";
            }

        }


        private void tbxRenderType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string xClass = "textBox";
            if (tbxRenderType.SelectedItem != null)
            {
                if (tbxRenderType.SelectedItem.ToString() == "TextBox")
                {
                    xClass = "textBox";
                }
                else if (tbxRenderType.SelectedItem.ToString() == "TextArea")
                {
                    xClass = "textArea";
                }
                else
                {
                    xClass = "";
                }
            }
            tbxCssClass.Text = xClass;

        }

        private void tbxName_LostFocus(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(tbxTitle.Text) && tbxName.Text.ToLower() == "title")
            {
                tbxTitle.Text = "标题";
                tbxCssClass.Text = "textBox";
                tbxCssClassLength.Text = "w100";
                tbxDataType.SelectedValue = "VarChar";
                tbxLength.Text = "300";
                tbxRenderType.SelectedValue = "TextBox";
                cbxDetailable.IsChecked = true;
                cbxListable.IsChecked = true;
                cbxEditable.IsChecked = true;
                cbxQueryable.IsChecked = true;
            }
            else if (string.IsNullOrEmpty(tbxTitle.Text) && tbxName.Text.ToLower().EndsWith("id"))
            {
                tbxTitle.Text = "编号";
                tbxCssClass.Text = "textBox";
                tbxCssClassLength.Text = "w1";
                tbxDataType.SelectedValue = "BigInt";
                tbxRenderType.SelectedValue = "TextBox";
                cbxDetailable.IsChecked = true;
                cbxListable.IsChecked = true;
                cbxEditable.IsChecked = true;
                cbxQueryable.IsChecked = true;

            }
            else if ( string.IsNullOrEmpty(tbxTitle.Text) &&(tbxName.Text.ToLower().EndsWith("date") || tbxName.Text.ToLower().StartsWith("date")) )
            {
                tbxTitle.Text = "日期";
                tbxCssClass.Text = "textBox";
                tbxCssClassLength.Text = "w3";
                tbxDataType.SelectedValue = "DateTime";
                tbxRenderType.SelectedValue = "TextBox";
                cbxDetailable.IsChecked = true;
                cbxListable.IsChecked = true;
                cbxEditable.IsChecked = true;
            }
        }

        private void tbxDataType_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxDataType.Text == "UniqueIdentifier")
            {
                //tbxDefault.Text = "'00000000-0000-0000-0000-000000000000'";
            }
            else if (tbxDataType.Text == "Int" || tbxDataType.Text == "BigInt")
            {
                tbxCssClassLength.Text = "w1";
            }
        }
    }
}
