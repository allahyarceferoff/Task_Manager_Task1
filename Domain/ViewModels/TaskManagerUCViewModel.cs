using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using TaskManager.Domain.Commands;

namespace TaskManager.Domain.ViewModels
{
    public class TaskManagerUCViewModel : BaseViewModel
    {
        public RelayCommand ExitCommand { get; set; }
        public RelayCommand SelectionChangedCommand { get; set; }
        public RelayCommand CreateProcessCommand { get; set; }
        public RelayCommand AddProcessToBlackBoxCommand { get; set; }

        private ObservableCollection<Process> processes = new ObservableCollection<Process>();

        public ObservableCollection<Process> Processes
        {
            get { return processes; }
            set { processes = value; OnPropertyChanged(); }
        }

        private Process selectedProcess;

        public Process SelectedProcess
        {
            get { return selectedProcess; }
            set { selectedProcess = value; OnPropertyChanged(); }
        }

        private string newProcessName;

        public string NewProcessName
        {
            get { return newProcessName; }
            set { newProcessName = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> blackBoxProcesses = new ObservableCollection<string>();

        public ObservableCollection<string> BlackBoxProcesses
        {
            get { return blackBoxProcesses; }
            set { blackBoxProcesses = value; OnPropertyChanged(); }
        }

        private Process selectedBlackBoxProcess;

        public Process SelectedBlackBoxProcess
        {
            get { return selectedBlackBoxProcess; }
            set { selectedBlackBoxProcess = value; OnPropertyChanged(); }
        }

        private DispatcherTimer _dispatcherTimer;

        public TaskManagerUCViewModel()
        {
            UpdateProcesses(null, null);
            DispatcherTimerSetup();

            SelectionChangedCommand = new RelayCommand((s) =>
            {
                if (SelectedProcess != null)
                {
                    _dispatcherTimer.Stop();
                    string name = SelectedProcess.ProcessName;
                    var result = MessageBox.Show($"Do you really want to delete the process : {name}?", "Delete Process", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            SelectedProcess.Kill();
                            MessageBox.Show($"The process {name} was deleted.", "Success");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"{ex}", "Exception");
                        }
                    }
                    _dispatcherTimer.Start();
                }
            });

            CreateProcessCommand = new RelayCommand((c) =>
            {
                var name = string.Empty;
                name = NewProcessName;
                if (!name.EndsWith(".exe"))
                {
                    name += ".exe";
                }
                try
                {
                    Process.Start(name);
                    NewProcessName = String.Empty;
                }
                catch (Exception)
                {
                    MessageBox.Show($"The process called '{NewProcessName}' does not exist.");
                }
            });

            ExitCommand = new RelayCommand((e) =>
            {
                App.Current.MainWindow.Close();
            });

            AddProcessToBlackBoxCommand = new RelayCommand((e) =>
            {
                string UserAnswer = Microsoft.VisualBasic.Interaction.InputBox("Enter the name of the process that you want to add to the list of black box processes.", "Add New Process");
                if (UserAnswer.Trim() != string.Empty)
                {
                    BlackBoxProcesses.Add(UserAnswer.Replace(".exe", ""));
                    MessageBox.Show($"{UserAnswer} was added.", "Success");
                }
            });
        }

        public void DispatcherTimerSetup()
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += UpdateProcesses;
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            _dispatcherTimer.Start();
        }

        public void UpdateProcesses(object sender, EventArgs e)
        {
            Processes = new ObservableCollection<Process>(Process.GetProcesses().OrderBy(p => p.ProcessName)); ;
            Processes.ToList().Where(p => BlackBoxProcesses.Contains(p.ProcessName)).ToList().ForEach(pr => pr.Kill());
        }
    }
}

