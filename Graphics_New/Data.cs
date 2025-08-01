﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Graphics_New
{
    public static class Data
    {
        private static int _currentRun = 0;
        private static int _lastloadedRun = 0;
        private static int _lastloadedRec = 0;
        private static int _currentRecord = 0;
        public static Dictionary<int, Run> dRuns = new Dictionary<int, Run>();
        
        // Define the event for property changes
        public static event EventHandler<PropertyChangedEventArgs> ?PropertyChanged;

        // Properties with change notification
        public static int CurrentRun
        {
            get => _currentRun;
            set
            {
                _currentRun = value;
                OnPropertyChanged(nameof(CurrentRun));
            }
        }

        public static int CurrentRecord
        {
            get => _currentRecord;
            set
            {
                _currentRecord = value;
                OnPropertyChanged(nameof(CurrentRecord));
            }
        }
        public static int LastLoadedRun
        {
            get => _lastloadedRun;
            set
            {
                _lastloadedRun = value;
                OnPropertyChanged(nameof(LastLoadedRun));
            }
        }

        public static int LastLoadedRecord
        {
            get => _lastloadedRec;
            set
            {
                _lastloadedRec = value;
                OnPropertyChanged(nameof(LastLoadedRecord));
            }
        }

        // Method to raise the PropertyChanged event
        private static void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        public static int GetNumberOfOpenedRuns()
        {
            return dRuns.Count;
        }

        public static List<Signal> GetSignals(int run, int record)
        {
            return dRuns[run].dRecords[record].GetSignals();
        }
        public static Signal GetSignalByName(int run, int record,string signalName)
        {
            return dRuns[run].dRecords[record].GetSignals().Find((s => s.Name == signalName));
        }
        public static List<Record> GetRecords(int run)
        {
            return dRuns[run].dRecords.Values.ToList();
        }
        public static Record GetRecord(int run, int rec)
        {
            return dRuns[run].dRecords[rec];
        }

        public static void AddNewRun(int? index = null) 
        {
            
            if (index.HasValue) // Un numéro est donné, on devine qu'on charge un dataset 
            {
                Run RuntoAdd = new Run(true);
                if (!dRuns.ContainsKey(index.Value))
                {
                    RuntoAdd.RunNumber = index.Value;
                    dRuns[index.Value] = RuntoAdd;
                    _lastloadedRun = index.Value;
                }
            }
            else // Un numéro n'est pas donné, on devine qu'on fait de l'acquisition
            {
                Run RuntoAdd = new Run();
                CurrentRun = RuntoAdd.RunNumber;
                dRuns[CurrentRun] = RuntoAdd;
            }
            Tools.LogToFile("AddNewRun");
        }


    }
}
