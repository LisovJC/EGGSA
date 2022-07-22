using EGGSA.Models;
using EGGSA.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace EGGSA.ViewModels
{
    internal class MainViewModels : Observer
    {
        #region Properties
        private readonly string _pathToHistory = $"{Environment.CurrentDirectory}\\History.json";
        private readonly string _pathToCountAllEgg = $"{Environment.CurrentDirectory}\\CountAllEgg.txt";
        private readonly string _pathToSoldAllEgg = $"{Environment.CurrentDirectory}\\CountSoldEgg.txt";
        private readonly string _pathToCountEggAfterSold = $"{Environment.CurrentDirectory}\\CountEggAfterSold.txt";             

        private bool _acceptCollectCorrect = false;

        public bool AcceptCollectCorrect
        {
            get => _acceptCollectCorrect;
            set { _acceptCollectCorrect  = value; OnPropertyChanged(); }
        }

        private bool _acceptSoldCorrect = false;

        public bool AcceptSoldCorrect
        {
            get => _acceptSoldCorrect;
            set { _acceptSoldCorrect = value; OnPropertyChanged(); }
        }


        private int _countAllEggs;

        public int CountAllEggs
        {
            get => _countAllEggs;
            set { _countAllEggs = value; OnPropertyChanged(); }
        }

        private string _collectEggs;

        public string CollectEggs
        {
            get => _collectEggs;
            set { _collectEggs = value; AcceptCollectCorrect = Regex.IsMatch(CollectEggs.Trim(), "^\\d+$") & !CollectEggs.StartsWith("0"); OnPropertyChanged(); }
        }

        private string _soldEggs;

        public string SoldEggs
        {
            get => _soldEggs;
            set { _soldEggs = value; AcceptSoldCorrect = Regex.IsMatch(SoldEggs.Trim(), "^\\d+$") & !SoldEggs.StartsWith("0"); OnPropertyChanged(); }
        }

        private int _allCountSold;

        public int AllCountSold
        {
            get => _allCountSold;
            set { _allCountSold = value; OnPropertyChanged(); }
        }

        private int _countEggAfterSold;

        public int CountEggAfterSold
        {
            get => _countEggAfterSold;
            set { _countEggAfterSold = value; OnPropertyChanged(); }
        }

        private int _selectedItem;

        public int SelectedIndex
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }




        #endregion

        public ObservableCollectionEX<Item> Items { get; set; }

        #region Commands
        public RelayCommand AcceptCollectedEggsCommand { get; set; }
        public RelayCommand AcceptSoldEggsCommand { get; set; }
        public RelayCommand RemoveAllCommand { get; set; }
        public RelayCommand WarningRemoveAllCommand { get; set; }
        public RelayCommand OpenCalcWindowCommand { get; set; }
        public RelayCommand RemoveElementCommand { get; set; }
        #endregion

        public MainViewModels()
        {            
           
            Items = new ObservableCollectionEX<Item>();                             
            LoadJsonITEMS(_pathToHistory);                                                 
            CountAllEggs = LoadTXT(_pathToCountAllEgg);
            AllCountSold = LoadTXT(_pathToSoldAllEgg);
            CountEggAfterSold = LoadTXT(_pathToCountEggAfterSold);
          
            AcceptCollectedEggsCommand = new RelayCommand(o =>
            {
                Items.Add(new Item() { Date = DateTime.Now, History = $"{CollectEggs} шт. Собрано", Collected = int.Parse(CollectEggs)});                
                SaveChangePlus(_pathToCountAllEgg, CollectEggs);
                SaveChangePlus(_pathToCountEggAfterSold, CollectEggs);
                CountAllEggs = LoadTXT(_pathToCountAllEgg);
                CountEggAfterSold = LoadTXT(_pathToCountEggAfterSold);
                CollectEggs = String.Empty;
            });

            AcceptSoldEggsCommand = new RelayCommand(o =>
            {
                Items.Add(new Item() { Date = DateTime.Now, History = $"{SoldEggs} шт. Продано", Sold = int.Parse(SoldEggs) });
                SaveChangePlus(_pathToSoldAllEgg, SoldEggs);
                SaveChangeMinus(_pathToCountEggAfterSold, SoldEggs);
                AllCountSold = LoadTXT(_pathToSoldAllEgg);
                CountEggAfterSold = LoadTXT(_pathToCountEggAfterSold);
                SoldEggs = String.Empty;
            });

            RemoveAllCommand = new RelayCommand(o =>
            {
                File.Delete(_pathToHistory);
                File.WriteAllText(_pathToCountAllEgg, "0");
                File.WriteAllText(_pathToSoldAllEgg, "0");
                File.WriteAllText(_pathToCountEggAfterSold, "0");
                CountAllEggs = 0;
                CollectEggs = String.Empty;
                SoldEggs = String.Empty;
                AllCountSold = 0;
                CountEggAfterSold = 0;
                Items.Clear();
            });

            RemoveElementCommand = new RelayCommand(o =>
            {
                try
                {
                    RemoveElement(_pathToCountAllEgg, _pathToSoldAllEgg, _pathToCountEggAfterSold, Items[SelectedIndex].Collected, Items[SelectedIndex].Sold);
                    Items.Remove(Items[SelectedIndex]);
                    CountAllEggs = LoadTXT(_pathToCountAllEgg);
                    AllCountSold = LoadTXT(_pathToSoldAllEgg);
                    CountEggAfterSold = LoadTXT(_pathToCountEggAfterSold);
                }
                catch (Exception ex)
                {

                    Debug.WriteLine(ex.Message);
                }
            });

            WarningRemoveAllCommand = new RelayCommand(o =>
            {
                WarningWindow warning = new WarningWindow();
                warning.DataContext = Application.Current.MainWindow.DataContext;
                warning.Owner = Application.Current.MainWindow;
                warning.Show();
            });

            OpenCalcWindowCommand = new RelayCommand(o =>
             {
                 CalculationsWindow calculations = new CalculationsWindow();                
                 calculations.Owner = Application.Current.MainWindow;
                 calculations.Show();
             });           
        }
        #region Methods
        public static bool IsValidJson(string stringValue)
        {
            if (File.Exists(stringValue))
            {
                var value = File.ReadAllText(stringValue);
                value = value.Trim();
                if ((value.StartsWith("{") && value.EndsWith("}")) ||
                    (value.StartsWith("[") && value.EndsWith("]")))
                {
                    try
                    {
                        JToken obj = JToken.Parse(value);
                        return true;
                    }
                    catch (JsonReaderException)
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        public void LoadJsonITEMS(string path)
        {
            if (IsValidJson(path))
            {
                string json = File.ReadAllText(path);
                Items = JsonConvert.DeserializeObject<ObservableCollectionEX<Item>>(json);
            }
            Items.JsonPath = path;                             
        }      

        public int LoadTXT(string path)
        {
            int value;
            if(File.Exists(path))
            {
                value = int.Parse(File.ReadAllText(path));
                return value;
            }
            else
            {
                File.Create(path).Close();
                File.WriteAllText(path, "0");
                return 0;
            }
        }

        public void SaveChangePlus(string path, string newValue)
        {
            try
            {
                int oldValue = int.Parse(File.ReadAllText(path).Trim());
                int result = oldValue + Convert.ToInt32(newValue);
                File.WriteAllText(path, result.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            } 
        }

        public void SaveChangeMinus(string path, string newValue)
        {
            try
            {
                int oldValue = int.Parse(File.ReadAllText(path).Trim());
                int result = oldValue - Convert.ToInt32(newValue);
                File.WriteAllText(path, result.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void RemoveElement(string path_1, string path_2, string path_3, int value_1, int value_2)
        {
            int oldValue_1 = int.Parse(File.ReadAllText(path_1).Trim()), oldValue_2 = int.Parse(File.ReadAllText(path_2).Trim()), result_1, result_2, temp;
            
            result_1 = oldValue_1 - value_1;
            temp = int.Parse(File.ReadAllText(path_3));
            temp = temp - value_1;
            File.WriteAllText(path_1, result_1.ToString());
            File.WriteAllText(path_3, temp.ToString());
            temp = 0;
                        
            result_2 = oldValue_2 - value_2;
            temp = int.Parse(File.ReadAllText(path_3));
            temp = temp + value_2;
            File.WriteAllText(path_2, result_2.ToString());
            File.WriteAllText(path_3, temp.ToString());
            temp = 0;
        }
        #endregion
    }
}
