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
using System.Windows.Shapes;
using System.Threading;

namespace ZippyWPFForm
{
    /// <summary>
    /// PPT4Setting.xaml 的交互逻辑
    /// </summary>
    public partial class PPT4Setting : Page
    {
        public CodeModes CodeMode { get; set; }
        private ZippyCoder.Entity.Project _Tag;
        public new ZippyCoder.Entity.Project Tag
        {
            get
            {
                return _Tag;
            }
            set
            {
                _Tag = value;

                lvTable.ItemsSource = _Tag.Tables;
            }
        }
        public string TTFile { get; set; }

        private TTSetting mySettings;

        private MainForm _Owner;

        public void LoadSettings()
        {
            if (System.IO.File.Exists(TTFile + ".config"))
            {
                mySettings = TTSetting.Load(TTFile + ".config");
            }
            else
            {
                mySettings = new TTSetting();
            }
            tbxPath.Text = mySettings.SaveingPath;
            tbxNamePattern.Text = mySettings.FileNamePattern;
            tbxFixDel.Text = mySettings.FixDelete;
            cbxSepDir.IsChecked = mySettings.MultiDir;
        }

        private void SaveSettings()
        {
            if (mySettings != null)
            {
                mySettings.SaveingPath = tbxPath.Text;
                mySettings.FileNamePattern = tbxNamePattern.Text;
                mySettings.FixDelete = tbxFixDel.Text;
                mySettings.MultiDir = cbxSepDir.IsChecked.Value;

                mySettings.Save(TTFile + ".config");
            }
        }

        public PPT4Setting(MainForm owner)
        {
            InitializeComponent();
            _Owner = owner;
            this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(PPT4Setting_IsVisibleChanged);
        }

        void PPT4Setting_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            spProgress.Visibility = Visibility.Hidden;
            if (this.Visibility == Visibility.Visible)
            {
                if (!string.IsNullOrEmpty(TTFile))
                    LoadSettings();
                if (CodeMode == CodeModes.Project)
                    cbxSepDir.IsEnabled = false;
            }
        }
        public PPT4Setting()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void btnChooseFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "选择路径";
            dlg.Filter = "*.txt|路径";

            if (dlg.ShowDialog().Equals(true))
            {
                tbxPath.Text = System.IO.Path.GetDirectoryName(dlg.FileName);
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            string outputPath = tbxPath.Text;
            if (System.IO.Directory.Exists(outputPath))
            {
                if (MessageBox.Show("生成的代码将覆盖此目录的同名文件，是否继续？", "询问", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                {
                    return;
                }

                ZippyCoder.Entity.Project project = Tag;
                if (CodeMode == CodeModes.Project)
                {
                    T4Engine.T4Creator.CreateCode(project, null, TTFile, project.Namespace, tbxNamePattern.Text, tbxFixDel.Text, outputPath, cbxSepDir.IsChecked.Value);
                }
                else if (CodeMode == CodeModes.Table)
                {
                    tipCreate.Text = "正在载入 T4 模板...";
                    spProgress.Visibility = Visibility.Visible;

                    pbCreate.Maximum = (project.Tables.Where(s => s.IsCoding)).Count();
                    pbCreate.Value = 0;
                    double value = 0;

                    foreach (ZippyCoder.Entity.Table table in project.Tables)
                    {
                        if (table.IsCoding)
                        {
                            T4Engine.T4Creator.CreateCode(project, table, TTFile, table.Name, tbxNamePattern.Text, tbxFixDel.Text, outputPath, cbxSepDir.IsChecked.Value);
                            pbCreate.Dispatcher.Invoke(new Action<System.Windows.DependencyProperty, object>(pbCreate.SetValue), System.Windows.Threading.DispatcherPriority.Background, ProgressBar.ValueProperty, value);
                            tipCreate.Dispatcher.Invoke(new Action<System.Windows.DependencyProperty, object>(tipCreate.SetValue), System.Windows.Threading.DispatcherPriority.Background, TextBlock.TextProperty, "正在执行生成：" + table.Title + "[" + table.Name + "]...");
                            value++;
                        }
                    }
                }

                if (T4Engine.T4Creator.T4Errors != null && T4Engine.T4Creator.T4Errors.Count > 0)
                {
                    string res = string.Empty;
                    foreach (System.CodeDom.Compiler.CompilerError error in T4Engine.T4Creator.T4Errors)
                    {
                        res += "\r\n" + "行：" + error.Line + " 列：" + error.Column + " ... " + error.ErrorText;
                    }
                    _Owner.ShowInfo(res);
                }
                else
                {
                    _Owner.ShowInfo("生成成功。");
                    this.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                MessageBox.Show("此目录不存在。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        ZippyCoder.Entity.Table currentTable = null;
        private void lvTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lbi = e.Source as ListBox;
            currentTable = lbi.SelectedItem as ZippyCoder.Entity.Table;
            if (currentTable == null) return;
            lvCol.ItemsSource = currentTable.Cols;

        }

        private void btnChooseAllTable_Click(object sender, RoutedEventArgs e)
        {
            //lvTable.SelectAll();
            foreach (ZippyCoder.Entity.Table table in _Tag.Tables)
            {
                table.IsCoding = true;
            }
            lvTable.ItemsSource = null;
            lvCol.ItemsSource = null;
            lvTable.ItemsSource = _Tag.Tables;
        }

        private void btnDeChooseAllTable_Click(object sender, RoutedEventArgs e)
        {

            //lvTable.UnselectAll();
            foreach (ZippyCoder.Entity.Table table in _Tag.Tables)
            {
                table.IsCoding = false;
            }
            lvTable.ItemsSource = null;
            lvCol.ItemsSource = null;
            lvTable.ItemsSource = _Tag.Tables;
        }

        private void btnChooseAllCol_Click(object sender, RoutedEventArgs e)
        {
            if (currentTable == null) return;
            foreach (ZippyCoder.Entity.Col col in currentTable.Cols)
            {
                col.IsCoding = true;
            }
            lvCol.ItemsSource = null;
            lvCol.ItemsSource = currentTable.Cols;
        }

        private void btnDeChooseAllCol_Click(object sender, RoutedEventArgs e)
        {
            if (currentTable == null) return;
            foreach (ZippyCoder.Entity.Col col in currentTable.Cols)
            {
                col.IsCoding = false;
            }
            lvCol.ItemsSource = null;
            lvCol.ItemsSource = currentTable.Cols;
        }
    }

    public enum CodeModes
    {
        Project,
        Table
    }
}
