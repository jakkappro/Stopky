using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Transactions;
using System.Windows.Input;
using Stopky.Annotations;
using Xamarin.Forms;

namespace Stopky.Models
{
    class MainModel : INotifyPropertyChanged
    {
        private int _seconds;
        private string _currentMode;

        public ObservableCollection<int> Laps { get; }
        
        public int Seconds
        {
            get => _seconds;
            
            private set
            {
                _seconds = value;
                OnPropertyChanged();
            }
        }

        public string CurrentMode
        {
            get => _currentMode;
            
            private set
            {
                _currentMode = value;
                OnPropertyChanged();
            } 
        }

        public ICommand StartCommand { get; }
        public ICommand RestartCommand { get; }
        public ICommand LapCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Thread countingThread;

        public MainModel()
        {
            _seconds = 0;
            _currentMode = "Start";
            Laps = new ObservableCollection<int>();
            
            StartCommand = new Command(Start);
            RestartCommand = new Command(Restart);
            LapCommand = new Command(Lap);

            var starter = new ThreadStart(Tick);
            countingThread = new Thread(starter);
            countingThread.Start();

        }

        private void Tick()
        {
            while (true)
            {
                if (CurrentMode != "Stop") 
                    continue;
                
                Seconds++;
                Thread.Sleep(1000);
            }
        }

        private void Start()
        {
            CurrentMode = CurrentMode == "Start" ? "Stop" : "Start";
        }

        private void Restart()
        {
            CurrentMode = "Start";
            Seconds = 0;
            Laps.Clear();
        }

        private void Lap()
        {
            Laps.Add(Seconds);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
