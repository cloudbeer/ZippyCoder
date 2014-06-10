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
    /// PPProject.xaml 的交互逻辑
    /// </summary>
    public partial class PPProject : Page
    {
        private MainForm _Owner;

        private ZippyCoder.Entity.Project _Project;
        public ZippyCoder.Entity.Project Project
        {
            get
            {
                if (_Project == null) _Project = new ZippyCoder.Entity.Project();
                _Project.Namespace = tbxNamespace.Text;
                if (string.IsNullOrEmpty(_Project.Namespace))
                {
                    //tbxNamespace.Focus();
                    throw new Exception("命名空间不能为空");
                }
                _Project.Title = tbxTitle.Text;
                _Project.Remark = tbxRemark.Text;
                return _Project;
            }
            set
            {
                tbxNamespace.Text = value.Namespace;
                //tbxNamespace.Focus();
                tbxTitle.Text = value.Title;
                tbxRemark.Text = value.Remark;
                _Project = value;
            }
        }

        public PPProject()
        {
            InitializeComponent();
        }
        public PPProject(MainForm owner)
        {
            InitializeComponent();
            _Owner = owner;
            this.Visibility = Visibility.Hidden;

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //_Project = Project;
            _Owner.UpdateProjectNode(Project);
            this.Visibility = Visibility.Hidden;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_Owner.Operation == ChangeTypes.CreateProject)
            {
                xTitle.Content = "新建项目";
            }
            else
            {
                xTitle.Content = "修改项目";
            }
        }
    }
}
