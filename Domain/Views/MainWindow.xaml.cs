using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskManager.Domain.ViewModels;
using TaskManager.Domain.Views;

namespace TaskManager
    {
        /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();  

            App.MyGrid = MyGrid;
            var taskManagerView = new TaskManagerUC();
            var taskManagerViewModel = new TaskManagerUCViewModel();
            taskManagerView.DataContext = taskManagerViewModel;
            App.MyGrid.Children.Add(taskManagerView);
        }
    }
}
