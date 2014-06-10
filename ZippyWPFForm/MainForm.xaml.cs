using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace ZippyWPFForm
{
    /// <summary>
    /// MainForm.xaml 的交互逻辑
    /// </summary>
    public partial class MainForm : Window
    {
        #region 私有变量
        private Nodes _CurrentNode = Nodes.None;
        private TreeViewItem _CurrentTreeViewItem = null;
        private ZippyCoder.Entity.Project _Project = null;
        public ZippyCoder.Entity.Project Project
        {
            get { return _Project; }
            set { _Project = value; }
        }
        private PPProject _uiProject = null;
        private PPTable _uiTable = null;
        private PPCol _uiCol = null;
        private PPT4Setting _uiT4Setting = null;
        private PPCodeCreateResult _uiTips = null;
        private string projectPath = string.Empty;
        private bool _Modified = false; //是否已经改变

        private ZippyCoder.Entity.Col _ClipboardCol = null;
        private ZippyCoder.Entity.Table _ClipboardTable = null;

        private System.IO.StreamWriter swSqlWriter = null;
        private System.IO.StreamWriter swOracleSqlWriter = null;

        private ChangeTypes operation = ChangeTypes.None;

        public ChangeTypes Operation
        {
            get { return operation; }
            set { operation = value; }
        }
        #endregion

        #region Init
        public MainForm()
        {
            InitializeComponent();

            InitPlugins();
            InitT4s();

            InitPages();

        }

        private void InitPages()
        {
            _uiProject = new PPProject(this);
            _uiTable = new PPTable(this);
            _uiCol = new PPCol(this);
            _uiT4Setting = new PPT4Setting(this);
            _uiTips = new PPCodeCreateResult();

        }

        #region T4 Init
        private void InitT4s()
        {
            InitT4Tables();
            InitT4Projects();

        }

        private void RecursionDirectory(System.IO.DirectoryInfo di, MenuItem rootMenu, bool isTableSource)
        {
            //System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(rootDir);
            System.IO.FileInfo[] files = di.GetFiles("*.tt");

            foreach (System.IO.FileInfo file in files)
            {
                MenuItem mi = new MenuItem();
                mi.Header = file.Name;
                mi.Tag = file.FullName;
                rootMenu.Items.Add(mi);
                if (isTableSource)
                    mi.Click += new RoutedEventHandler(miT4Table_Click);
                else
                    mi.Click += new RoutedEventHandler(miT4Project_Click);
            }

            System.IO.DirectoryInfo[] dirs = di.GetDirectories();
            foreach (System.IO.DirectoryInfo diChild in dirs)
            {
                MenuItem miDirChild = new MenuItem();
                miDirChild.Header = diChild.Name;
                rootMenu.Items.Add(miDirChild);
                RecursionDirectory(diChild, miDirChild, isTableSource);
            }
        }

        private void InitT4Tables()
        {
            string pluginPath = ".\\T4Template\\Table";
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(pluginPath);
            RecursionDirectory(di, mT4TableCoder, true);
        }
        private void InitT4Projects()
        {
            string pluginPath = ".\\T4Template\\Project";
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(pluginPath);
            RecursionDirectory(di, mT4ProjectCoder, false);
        }

        void miT4Project_Click(object sender, RoutedEventArgs e)
        {
            _uiT4Setting.Visibility = Visibility.Collapsed;
            MenuItem xsender = sender as MenuItem;
            _uiT4Setting.Tag = _Project;
            _uiT4Setting.CodeMode = CodeModes.Project;
            _uiT4Setting.TTFile = xsender.Tag.ToString();
            //_uiT4Setting.LoadSettings();
            Navigate(_uiT4Setting);
        }
        void miT4Table_Click(object sender, RoutedEventArgs e)
        {
            _uiT4Setting.Visibility = Visibility.Collapsed;
            MenuItem xsender = sender as MenuItem;
            _uiT4Setting.Tag = _Project;
            _uiT4Setting.CodeMode = CodeModes.Table;
            _uiT4Setting.TTFile = xsender.Tag.ToString();
            //_uiT4Setting.LoadSettings();
            Navigate(_uiT4Setting);
        }


        #endregion

        #region Plugins Init
        private void InitPlugins()
        {
            string pluginPath = ".\\Plugins\\";
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(pluginPath);
            System.IO.FileInfo[] files = di.GetFiles("*.dll");

            foreach (System.IO.FileInfo file in files)
            {
                InitOneAssembly(file.FullName);
            }
        }
        private void InitOneAssembly(string extensionPath)
        {
            System.Reflection.Assembly assExtention = System.Reflection.Assembly.LoadFrom(extensionPath);
            string menuTitle = "Menu";
            if (assExtention != null)
            {
                object[] customAttributes = assExtention.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if ((customAttributes != null) && (customAttributes.Length > 0))
                    menuTitle = ((AssemblyTitleAttribute)customAttributes[0]).Title;
                else
                    menuTitle = assExtention.GetName().Name;
            }

            MenuItem mi = new MenuItem();
            mi.Header = menuTitle;
            mCoder.Items.Add(mi);

            Type[] types = assExtention.GetTypes();
            IEnumerable<Type> atypes = types.OrderBy(s => s.Name);

            foreach (Type type in atypes)
            {
                if (type.IsSubclassOf(typeof(ZippyCoder.AbstractCoder)))
                {
                    MenuItem miCoder = new MenuItem();
                    object[] ziAttrs = type.GetCustomAttributes(typeof(ZippyCoder.PluginIndicatorAttribute), false);
                    if ((ziAttrs != null) && (ziAttrs.Length > 0))
                        miCoder.Header = ((ZippyCoder.PluginIndicatorAttribute)ziAttrs[0]).Title;
                    else
                        miCoder.Header = type.Name;
                    miCoder.Tag = type;
                    miCoder.Click += new RoutedEventHandler(miCoder_Click);
                    mi.Items.Add(miCoder);
                }

            }
        }

        void miCoder_Click(object sender, RoutedEventArgs e)
        {
            MenuItem xsender = sender as MenuItem;
            Type mTP = (Type)xsender.Tag;
            ZippyCoder.AbstractCoder coder = (ZippyCoder.AbstractCoder)Activator.CreateInstance(mTP);
            if (_Project != null)
            {
                coder.DataSourse = _Project;
                try
                {
                    coder.Create();
                    ShowInfo("恭喜你，代码生成成功！");
                }
                catch (Exception ex)
                {
                    ShowInfo("代码生成失败：\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                }
            }
            else
            {
                ShowInfo("请首先创建一个项目");
            }
        }
        #endregion

        #endregion

        #region 通用事件 CommandBinding

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

            if (e.Command.Equals(ApplicationCommands.Save))
            {
                if (_Project == null)
                {
                    e.CanExecute = false;
                }
                else
                    e.CanExecute = true;
            }
            else if (e.Command.Equals(ApplicationCommands.New))
            {
                if (_CurrentNode == Nodes.Col)
                    e.CanExecute = false;
                else
                    e.CanExecute = true;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command.Equals(ApplicationCommands.Open))
            {
                LoadProject();
            }
            else if (e.Command.Equals(ApplicationCommands.Save))
            {
                if (Validate())
                    SaveProject();
            }
            else if (e.Command.Equals(ApplicationCommands.Copy))
            {
                CopyNode();
            }
            else if (e.Command.Equals(ApplicationCommands.Paste))
            {
                PasteNode();
            }
            else if (e.Command.Equals(ApplicationCommands.New))
            {
                NewNode();
            }
            else if (e.Command.Equals(ApplicationCommands.Close))
            {
                this.Close();
            }

        }
        private void ZippyCommands_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

            if (e.Command.Equals(ZippyCommands.CopyNode))
            {
                if (_CurrentNode == Nodes.Project || _CurrentNode == Nodes.None)
                    e.CanExecute = false;
                else
                    e.CanExecute = true;
            }
            else if (e.Command.Equals(ZippyCommands.PasteNode))
            {
                InitClipBoard();
                if (((_CurrentNode == Nodes.Project) && (_ClipboardTable != null)) || ((_CurrentNode == Nodes.Table) && (_ClipboardCol != null)))
                    e.CanExecute = true;
                else
                    e.CanExecute = false;

            }
            else if (e.Command.Equals(ZippyCommands.DeleteNode))
            {
                if (_CurrentNode == Nodes.Project || _CurrentNode == Nodes.None)
                    e.CanExecute = false;
                else
                    e.CanExecute = true;
            }
            else if (e.Command.Equals(ZippyCommands.CreateCols))
            {
                if (_CurrentNode == Nodes.Table)
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
            else if (e.Command.Equals(ZippyCommands.NodeDown))
            {
                TreeViewItem myParent = _CurrentTreeViewItem.Parent as TreeViewItem;
                if (myParent == null)
                {
                    e.CanExecute = false;

                    return;
                }
                if (myParent.Items[myParent.Items.Count - 1] == _CurrentTreeViewItem)
                {
                    e.CanExecute = false;
                    return;
                }
                e.CanExecute = true;
            }
            else if (e.Command.Equals(ZippyCommands.NodeUp))
            {
                TreeViewItem myParent = _CurrentTreeViewItem.Parent as TreeViewItem;
                if (myParent == null)
                {
                    e.CanExecute = false;
                    return;
                }
                if (myParent.Items[0] == _CurrentTreeViewItem)
                {
                    e.CanExecute = false;
                    return;
                }
                e.CanExecute = true;
            }


        }

        private void ZippyCommands_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command.Equals(ZippyCommands.CopyNode))
            {
                CopyNode();
            }
            else if (e.Command.Equals(ZippyCommands.PasteNode))
            {
                PasteNode();
            }
            else if (e.Command.Equals(ZippyCommands.DeleteNode))
            {
                DeleteNode();
            }
            else if (e.Command.Equals(ZippyCommands.CreateCols))
            {
                Helper.CreateLazyCols(_CurrentTreeViewItem, (ZippyCoder.Entity.Table)_CurrentTreeViewItem.Tag);
            }
            else if (e.Command.Equals(ZippyCommands.NodeDown))
            {
                TreeViewItem myParent = _CurrentTreeViewItem.Parent as TreeViewItem;

                int index = 0;
                for (; index < myParent.Items.Count; index++)
                {
                    if (_CurrentTreeViewItem == myParent.Items[index])
                    {
                        break;
                    }
                }
                myParent.Items.Remove(_CurrentTreeViewItem);
                myParent.Items.Insert(index + 1, _CurrentTreeViewItem);

                if (myParent.Tag is ZippyCoder.Entity.Project) //表移动
                {
                    _Project.Tables.Remove(_CurrentTreeViewItem.Tag as ZippyCoder.Entity.Table);
                    _Project.Tables.Insert(index + 1, _CurrentTreeViewItem.Tag as ZippyCoder.Entity.Table);

                }
                else
                {
                    ZippyCoder.Entity.Table table = myParent.Tag as ZippyCoder.Entity.Table;

                    table.Cols.Remove(_CurrentTreeViewItem.Tag as ZippyCoder.Entity.Col);
                    table.Cols.Insert(index + 1, _CurrentTreeViewItem.Tag as ZippyCoder.Entity.Col);
                }
                _CurrentTreeViewItem.IsSelected = true;
                ShowInfo("下移成功。");

            }
            else if (e.Command.Equals(ZippyCommands.NodeUp))
            {
                TreeViewItem myParent = _CurrentTreeViewItem.Parent as TreeViewItem;

                int index = 0;
                for (; index < myParent.Items.Count; index++)
                {
                    if (_CurrentTreeViewItem == myParent.Items[index])
                    {
                        break;
                    }
                }
                myParent.Items.Remove(_CurrentTreeViewItem);
                myParent.Items.Insert(index - 1 >= 0 ? index - 1 : 0, _CurrentTreeViewItem);

                if (myParent.Tag is ZippyCoder.Entity.Project) //表移动
                {
                    _Project.Tables.Remove(_CurrentTreeViewItem.Tag as ZippyCoder.Entity.Table);
                    _Project.Tables.Insert(index - 1 >= 0 ? index - 1 : 0, _CurrentTreeViewItem.Tag as ZippyCoder.Entity.Table);

                }
                else
                {
                    ZippyCoder.Entity.Table table = myParent.Tag as ZippyCoder.Entity.Table;

                    table.Cols.Remove(_CurrentTreeViewItem.Tag as ZippyCoder.Entity.Col);
                    table.Cols.Insert(index - 1 >= 0 ? index - 1 : 0, _CurrentTreeViewItem.Tag as ZippyCoder.Entity.Col);
                }
                _CurrentTreeViewItem.IsSelected = true;
                ShowInfo("上移成功。");
            }

        }





        #endregion

        #region 点击树
        private void treeProject_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TreeView myTV = (TreeView)sender;
            if (myTV.SelectedItem == null) return;
            _CurrentTreeViewItem = myTV.SelectedItem as TreeViewItem;


            if (_CurrentTreeViewItem.Tag is ZippyCoder.Entity.Project)
            {
                operation = ChangeTypes.AlterProject;
                _CurrentNode = Nodes.Project;
                _uiProject.Project = _Project;
                Navigate(_uiProject);
            }
            else if (_CurrentTreeViewItem.Tag is ZippyCoder.Entity.Table)
            {
                operation = ChangeTypes.AlterTable;
                _CurrentNode = Nodes.Table;
                _uiTable.Table = (ZippyCoder.Entity.Table)_CurrentTreeViewItem.Tag;
                Navigate(_uiTable);

            }
            else if (_CurrentTreeViewItem.Tag is ZippyCoder.Entity.Col)
            {
                operation = ChangeTypes.AlterCol;

                _CurrentNode = Nodes.Col;
                _uiCol.Project = _Project;
                _uiCol.Col = (ZippyCoder.Entity.Col)_CurrentTreeViewItem.Tag;
                Navigate(_uiCol);

            }
        }
        #endregion

        #region 菜单事件
        /// <summary>
        /// 强行保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SaveAnyway_Click(object sender, RoutedEventArgs e)
        {
            SaveProject();
        }
        private void mOptionSqlMonitor_Click(object sender, RoutedEventArgs e)
        {
            if (swSqlWriter == null)
            {
                swSqlWriter = SqlMonitor.OpenSqlFile();
            }
            else
            {
                swSqlWriter.Close();
                swSqlWriter.Dispose();
                swSqlWriter = null;
                MessageBox.Show("已停止监视。", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void ImportOldProject_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Xml文件|*.xml";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                //projectPath = dlg.FileName;
                try
                {
                    _Project = ZippyCoder.ProjectParser.LoadOldProject(dlg.FileName);  //ZippyCoder.Entity.Project.Load(projectPath);
                    projectPath = string.Empty;
                    UpdateUI();
                }
                catch (Exception ex)
                {
                    ShowInfo("文件打开错误，请核对项目文件的格式。" + ex.Message);
                    //MessageBox.Show("文件打开错误，请核对项目文件的格式。\r\n" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        #endregion

        #region 窗口事件
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_Modified)
            {
                MessageBoxResult res = MessageBox.Show("当前项目被改变，是否在退出前保存？\r\n点击“是”则保存项目，点击“否”则放弃保存。", "请注意", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    SaveProject();
                }
                else if (res == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (swSqlWriter != null)
                swSqlWriter.Close();
            if (swOracleSqlWriter != null)
                swOracleSqlWriter.Close();
        }
        #endregion

        #region Create Node
        private void NewNode()
        {
            operation = ChangeTypes.None;
            switch (_CurrentNode)
            {
                case Nodes.None:
                    operation = ChangeTypes.CreateProject;
                    CreateProject();
                    break;
                case Nodes.Project:
                    operation = ChangeTypes.AddTable;
                    CreateTable();
                    break;
                case Nodes.Table:
                    operation = ChangeTypes.AddCol;
                    CreateCol();
                    break;
                case Nodes.Col:
                    MessageBox.Show("下面不能继续创建了。");
                    break;
            }
        }

        private void CreateProject()
        {
            _Project = new ZippyCoder.Entity.Project();
            _uiProject.Project = _Project;
            Navigate(_uiProject);
        }
        private void CreateTable()
        {
            ZippyCoder.Entity.Table table = new ZippyCoder.Entity.Table();
            _uiTable.Table = table;
            Navigate(_uiTable);
        }
        private void CreateCol()
        {
            ZippyCoder.Entity.Col col = new ZippyCoder.Entity.Col();
            _uiCol.Project = _Project;
            _uiCol.Col = col;
            Navigate(_uiCol);
        }
        #endregion

        #region update UI ,组织数据

        public void UpdateUI()
        {
            _uiProject.Visibility = Visibility.Hidden;
            _uiTable.Visibility = Visibility.Hidden;
            _uiCol.Visibility = Visibility.Hidden;
            treeProject.Items.Clear();
            TreeViewItem tviProject = new TreeViewItem();
            tviProject.Tag = _Project;
            tviProject.Header = _Project.Title + "[" + _Project.Namespace + "]";
            tviProject.IsExpanded = true;
            treeProject.Items.Add(tviProject);

            foreach (ZippyCoder.Entity.Table table in _Project.Tables)
            {
                TreeViewItem tviTable = new TreeViewItem();
                tviProject.Items.Add(tviTable);
                //tviProject.IsExpanded = true;
                tviTable.Tag = table;
                tviTable.Header = table.Title + "[" + table.Name + "]";

                foreach (ZippyCoder.Entity.Col col in table.Cols)
                {
                    //Helper.BatchTenantID(col);
                    TreeViewItem tviCol = new TreeViewItem();
                    tviTable.Items.Add(tviCol);
                    //tviTable.IsExpanded = true;
                    col.Name = col.Name.ToLower();
                    tviCol.Tag = col;
                    tviCol.Header = col.Title + "[" + col.Name + "]";
                }
            }

            tviProject.IsSelected = true;
            _CurrentNode = Nodes.Project;
            _CurrentTreeViewItem = tviProject;
        }

        public void UpdateProjectNode(ZippyCoder.Entity.Project project)
        {
            _Modified = true;
            ChangeTitle(false);
            if (_CurrentTreeViewItem == null) //当前选中的item项目不存在，表示新创
            {
                _CurrentTreeViewItem = new TreeViewItem();
                treeProject.Items.Add(_CurrentTreeViewItem);
                _CurrentTreeViewItem.Focus();

                _CurrentNode = Nodes.Project;
            }
            _CurrentTreeViewItem.Tag = project;
            _CurrentTreeViewItem.Header = project.Title + "[" + project.Namespace + "]";

            this.Title = project.Title + "[" + project.Namespace + "]" + " - Zippy 代码生成器";

        }

        public void UpdateTableNode(ZippyCoder.Entity.Table table)
        {
            _Modified = true;
            ChangeTitle(false);
            if (_CurrentTreeViewItem.Tag is ZippyCoder.Entity.Table) //表示修改
            {
                _CurrentTreeViewItem.Tag = table;
                _CurrentTreeViewItem.Header = table.Title + "[" + table.Name + "]";
            }
            else //新建的时候加入默认列
            {
                _Project.Tables.Add(table);

                TreeViewItem tviTable = new TreeViewItem();
                _CurrentTreeViewItem.Items.Add(tviTable);
                _CurrentTreeViewItem.IsExpanded = true;

                tviTable.Tag = table;
                tviTable.Header = table.Title + "[" + table.Name + "]";

                //CreateDefaultCol(tviTable, table);

            }
        }
        public void UpdateColNode(ZippyCoder.Entity.Col col)
        {
            _Modified = true;
            ChangeTitle(false);
            if (_CurrentTreeViewItem.Tag is ZippyCoder.Entity.Col)
            {
                _CurrentTreeViewItem.Tag = col;
                _CurrentTreeViewItem.Header = col.Title + "[" + col.Name + "]";
            }
            else if (_CurrentTreeViewItem.Tag is ZippyCoder.Entity.Table)
            {
                ZippyCoder.Entity.Table table = _CurrentTreeViewItem.Tag as ZippyCoder.Entity.Table;
                //int insPos = table.Cols.Count;
                //if (insPos >= 9)   //懒人列
                //{
                //    insPos = insPos - 8;
                //}
                //table.Cols.Insert(insPos, col);
                table.Cols.Add(col);

                TreeViewItem tviCol = new TreeViewItem();
                _CurrentTreeViewItem.Items.Add(tviCol);
                //_CurrentTreeViewItem.Items.Insert(insPos, tviCol);

                _CurrentTreeViewItem.IsExpanded = true;
                tviCol.Tag = col;
                tviCol.Header = col.Title + "[" + col.Name + "]";
            }
        }

        #endregion

        #region 公用方法
        public void WriteSqlLog(ZippyCoder.Entity.Col col, string oldColName)
        {
            if (_CurrentTreeViewItem.Tag is ZippyCoder.Entity.Table)
            {
                SqlMonitor.WriteSqlServerChangeLog(swSqlWriter, _CurrentTreeViewItem.Tag as ZippyCoder.Entity.Table, col, operation, oldColName);
            }
            else if (_CurrentTreeViewItem.Tag is ZippyCoder.Entity.Col)
            {
                TreeViewItem parentTVI = _CurrentTreeViewItem.Parent as TreeViewItem;
                if (parentTVI.Tag is ZippyCoder.Entity.Table)
                {
                    SqlMonitor.WriteSqlServerChangeLog(swSqlWriter, parentTVI.Tag as ZippyCoder.Entity.Table, col, operation, oldColName);
                }
            }
        }

        private bool Validate()
        {
            bool rtn = true;
            List<string> errors = new List<string>();

            foreach (ZippyCoder.Entity.Table table in _Project.Tables)
            {
                IEnumerable<ZippyCoder.Entity.Table> queryTable = _Project.Tables.Where(x => x.Name == table.Name);
                if (queryTable.Count() > 1)
                {
                    errors.Add("表 [" + table.Name + "] 发生重复。");
                    rtn = false;
                }
                var pkCol = from xcol in table.Cols
                            where xcol.IsPK == true
                            select xcol;
                if (pkCol.Count() == 0)
                {
                    errors.Add("表 [" + table.Name + "] 没有主键");
                    rtn = false;
                }
                if (pkCol.Count() > 1)
                {
                    errors.Add("表 [" + table.Name + "] 有多个主键，目前此版本的设计不支持联合主键。");
                    rtn = false;
                }

                foreach (ZippyCoder.Entity.Col col in table.Cols)
                {
                    IEnumerable<ZippyCoder.Entity.Col> queryCol = table.Cols.Where(x => x.Name == col.Name);
                    if (queryCol.Count() > 1)
                    {
                        errors.Add("列 [" + table.Name + "." + col.Name + "] 发生重复。");
                        rtn = false;
                    }

                    if (!string.IsNullOrEmpty(col.RefTable) && !string.IsNullOrEmpty(col.RefCol))
                    {
                        IEnumerable<ZippyCoder.Entity.Table> queryRefTable = _Project.Tables.Where(x => x.Name == col.RefTable);
                        if (queryRefTable.Count() < 1)
                        {
                            errors.Add("列 [" + table.Name + "." + col.Name + "] 的外键映射的表 [" + col.RefTable + "] 没有找到。");
                            rtn = false;
                        }
                        else if (!queryRefTable.First().Cols.Exists(x => x.Name == col.RefCol))
                        {
                            errors.Add("列 [" + table.Name + "." + col.Name + "] 的外键映射的表中的列 [" + queryRefTable.First().Name + "." + col.RefCol + "] 没有找到。");
                            rtn = false;
                        }
                    }
                }
            }
            if (errors.Count() > 0)
            {
                PPError _uiError = new PPError();
                _uiError.Errors = errors;
                pnlContainer.Navigate(_uiError);
            }

            return rtn;

        }
        private void SaveProject()
        {
            //_Project = treeProject.Tag as ZippyCoder.Entity.Project;
            if (_Project == null)
            {
                //MessageBox.Show("", "错误");
                ShowInfo("没有项目文件，请新建或者载入");
                return;
            }

            if (string.IsNullOrEmpty(projectPath))
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = _Project.Namespace + ".xml";
                dlg.Filter = "Xml文件|*.xml";

                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    projectPath = dlg.FileName;
                }
                else
                {
                    return;
                }
            }

            try
            {
                _Project.SaveProject(projectPath);
                _Modified = false;
                ChangeTitle(true);

                ShowInfo("项目保存成功。");
            }
            catch (Exception ex)
            {
                ShowInfo("文件保存失败：\r\n\r\n\t" + ex.Message);
            }

        }
        private void LoadProject()
        {
            if (_Project != null)
            {
                if (MessageBox.Show("载入新项目将覆盖目前的项目，是否继续？", "询问", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Xml文件|*.xml";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                projectPath = dlg.FileName;
                _Project = ZippyCoder.Entity.Project.Load(dlg.FileName);
                if (!string.IsNullOrEmpty(_Project.Namespace))
                {
                    UpdateUI();
                    projectPath = dlg.FileName;
                    Title = _Project.Namespace;
                    ShowInfo("项目载入成功。");
                }
                else
                {
                    MessageBox.Show("项目载入失败，请检查该文件的格式项目是否是正确。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    ShowInfo("项目载入失败，请检查该文件的格式项目是否是正确。");
                }
            }
        }


        private void Navigate(Page page)
        {
            page.Visibility = Visibility.Visible;
            pnlContainer.Navigate(page);
        }

        private void ChangeTitle(bool saved)
        {

            if (Title.StartsWith("*") && saved)
            {
                Title = Title.Substring(1, Title.Length - 1);
            }
            else if (!Title.StartsWith("*") && !saved)
            {
                Title = "*" + Title;
            }
        }

        public void ShowInfo(string info)
        {
            _uiTips.Result = info;
            Navigate(_uiTips);
        }

        public void InitClipBoard()
        {
            IDataObject iobj = Clipboard.GetDataObject();
            object colObj = iobj.GetData(typeof(ZippyCoder.Entity.Col));
            object tableObj = iobj.GetData(typeof(ZippyCoder.Entity.Table));

            if (colObj != null)
            {
                _ClipboardCol = colObj as ZippyCoder.Entity.Col;
            }
            if (tableObj != null)
            {
                _ClipboardTable = tableObj as ZippyCoder.Entity.Table;
            }
        }
        #endregion

        #region node 的增删改
        private void DeleteNode()
        {
            if (MessageBox.Show("是否删除当前节点？", "请确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }
            _Modified = true;
            ChangeTitle(false);
            if (_CurrentNode == Nodes.Table)
            {

                ZippyCoder.Entity.Table table = _CurrentTreeViewItem.Tag as ZippyCoder.Entity.Table;

                operation = ChangeTypes.DropTable;
                WriteSqlLog(null, null); //没有col，但有当前 table，通过 ChangeTypes.DropTable 删除表

                TreeViewItem parentTV = _CurrentTreeViewItem.Parent as TreeViewItem;
                parentTV.Items.Remove(_CurrentTreeViewItem);
                _Project.Tables.Remove(table);

                ShowInfo("删除成功!");
            }
            else if (_CurrentNode == Nodes.Col)
            {

                ZippyCoder.Entity.Col col = _CurrentTreeViewItem.Tag as ZippyCoder.Entity.Col;

                operation = ChangeTypes.DropCol;
                WriteSqlLog(col, null);  //删除列

                TreeViewItem parentTV = _CurrentTreeViewItem.Parent as TreeViewItem;
                parentTV.Items.Remove(_CurrentTreeViewItem);
                ZippyCoder.Entity.Table table = parentTV.Tag as ZippyCoder.Entity.Table;
                table.Cols.Remove(col);

                ShowInfo("删除成功!");

            }
        }
        private void CopyNode()
        {
            if (_CurrentNode == Nodes.Table)
            {
                _ClipboardTable = _CurrentTreeViewItem.Tag as ZippyCoder.Entity.Table;
                //_ClipboardTable = table;
                DataObject dataobj = new DataObject(typeof(ZippyCoder.Entity.Table), _ClipboardTable);
                Clipboard.SetDataObject(dataobj);
            }
            else if (_CurrentNode == Nodes.Col)
            {
                _ClipboardCol = _CurrentTreeViewItem.Tag as ZippyCoder.Entity.Col;
                //_ClipboardCol = col;
                DataObject dataobj = new DataObject(typeof(ZippyCoder.Entity.Col), _ClipboardCol);
                Clipboard.SetDataObject(dataobj);
            }
        }
        private void PasteNode()
        {
            _Modified = true;
            ChangeTitle(false);

            if (_CurrentNode == Nodes.Project && (_ClipboardTable != null)) //table 直接加入，因为带有下面的列数据
            {
                operation = ChangeTypes.None;
                _uiProject.Visibility = Visibility.Hidden;
                _uiTable.Visibility = Visibility.Hidden;
                _uiCol.Visibility = Visibility.Hidden;
                ZippyCoder.Entity.Table table = _ClipboardTable;
                table.Name = table.Name;
                TreeViewItem tviTable = new TreeViewItem();
                _CurrentTreeViewItem.Items.Add(tviTable);
                tviTable.Tag = table;
                tviTable.Header = table.Title + "[" + table.Name + "]";

                foreach (ZippyCoder.Entity.Col col in table.Cols)
                {
                    TreeViewItem tviCol = new TreeViewItem();
                    tviTable.Items.Add(tviCol);
                    tviTable.IsExpanded = true;
                    tviCol.Tag = col;
                    tviCol.Header = col.Title + "[" + col.Name + "]";
                }

                _Project.Tables.Add(table);

            }
            else if (_CurrentNode == Nodes.Table && (_ClipboardCol != null))  //列加入的时候，确认一下
            {
                operation = ChangeTypes.AddCol;
                ZippyCoder.Entity.Col col = _ClipboardCol;
                col.Name = col.Name;
                _uiCol.Project = _Project;
                _uiCol.Col = col;
                Navigate(_uiCol);
            }

        }
        #endregion

        #region 各event触发的方法

        private void UpdateStatusBar()
        {
        }

        #endregion

        private void mFileOpenDBSqlServer_Click(object sender, RoutedEventArgs e)
        {
            PPOpenSqlServer openSql = new PPOpenSqlServer(this);
            Navigate(openSql);
        }
        private void mFileOpenDBMySql_Click(object sender, RoutedEventArgs e)
        {
            PPOpenMySql openSql = new PPOpenMySql(this);
            Navigate(openSql);
        }

        

        #region 修改懒人列数据类型
        private void ChangeLayCols(System.Data.SqlDbType dType, string defaultVal)
        {
            foreach (var t in _Project.Tables)
            {
                foreach (var c in t.Cols)
                {
                    if (c.Name.ToLower() == "creator" || c.Name.ToLower() == "updater")
                    {
                        c.DataType = dType;
                        c.Default = defaultVal;
                    }

                    if (c.Name == "TenantID" && !c.IsPK)
                    {
                        c.DataType = dType;
                        c.Default = defaultVal;
                        c.RenderType = ZippyCoder.Entity.RenderTypes.UrlQuery;
                        c.CssClass = "";
                        c.CssClassLength = "";
                    }

                    if (c.DataType == System.Data.SqlDbType.VarChar)
                    {
                        c.DataType = System.Data.SqlDbType.NVarChar;
                    }
                    if (c.Name == "creator")
                    {
                        c.UIColType = ZippyCoder.Entity.UIColTypes.Detailable;
                    }
                    if (c.Name == "create_date")
                    {
                        c.UIColType = ZippyCoder.Entity.UIColTypes.Listable | ZippyCoder.Entity.UIColTypes.Detailable;
                    }
                    if (c.Name == "updater")
                    {
                        c.UIColType = ZippyCoder.Entity.UIColTypes.Detailable;
                    }
                    if (c.Name == "update_date")
                    {
                        c.UIColType = ZippyCoder.Entity.UIColTypes.Detailable;
                    }
                    if (c.Name == "tenant_id")
                    {
                        c.UIColType = ZippyCoder.Entity.UIColTypes.Detailable;
                    }
                    if (c.Name == "remark")
                    {
                        c.UIColType = ZippyCoder.Entity.UIColTypes.Detailable;
                    }
                    if (c.Name == "extend_data")
                    {
                        c.UIColType = ZippyCoder.Entity.UIColTypes.Detailable;
                    }
                    
                    
                    
                }
            }

        }
        private void mLazy001_Click(object sender, RoutedEventArgs e)
        {
            ChangeLayCols(System.Data.SqlDbType.BigInt, "0");
        }

        private void mLazy002_Click(object sender, RoutedEventArgs e)
        {
            ChangeLayCols(System.Data.SqlDbType.Int, "0");
        }
        private void mLazy003_Click(object sender, RoutedEventArgs e)
        {
            ChangeLayCols(System.Data.SqlDbType.UniqueIdentifier, "");
        }
        #endregion



    }
}
