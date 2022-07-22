using EGGSA.Models;
using EGGSA.Services;
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace EGGSA.ViewModels
{
    internal class CalculationsViewModels : Observer
    {

        private bool _isCorrect = true;

        public bool isCorrect
        {
            get => _isCorrect;
            set { _isCorrect = value; OnPropertyChanged(); }
        }


        private readonly string _pathToHistory = $"{Environment.CurrentDirectory}\\History.json";

        private string _countBag = "1";

        public string CountBag
        {
            get => _countBag;
            set { _countBag = value; isCorrect = !CountBag.Contains(" ") & CountBag != String.Empty; OnPropertyChanged(); }
        }

        private string _priceBag = "600";

        public string PriceBag
        {
            get => _priceBag;
            set { _priceBag = value; isCorrect = Regex.IsMatch(PriceBag.Trim(), "^\\d+$"); OnPropertyChanged(); }
        }

        private string _priceTenEggs = "85";

        public string PriceTenEggs
        {
            get => _priceTenEggs;
            set { _priceTenEggs  = value; isCorrect = Regex.IsMatch(PriceTenEggs.Trim(), "^\\d+$"); OnPropertyChanged(); }
        }

        private bool _week = true;

        public bool Week
        {
            get => _week;
            set { _week = value; OnPropertyChanged(); }
        }

        private bool _month = false;

        public bool Month
        {
            get => _month;
            set { _month = value; OnPropertyChanged(); }
        }

        private bool _halfyear = false;

        public bool HalfYear
        {
            get => _halfyear;
            set { _halfyear = value; OnPropertyChanged(); }
        }

        private bool _year = false;

        public bool Year
        {
            get => _year;
            set { _year = value; OnPropertyChanged(); }
        }

        private bool _allTime = false;

        public bool AllTime
        {
            get => _allTime;
            set { _allTime = value; OnPropertyChanged(); }
        }

        private string _result = "Не рассчитано";

        public string result
        {
            get => _result;
            set { _result = value; OnPropertyChanged(); }
        }

        public ObservableCollectionEX<Item> OldItems { get; set; }
        public SeriesCollection SeriesCollection { get; set; }        
        public Func<double, string> YFormatter { get; set; }

        public RelayCommand CalculationsCommand { get; set; }       

        public CalculationsViewModels()
        {
            SeriesCollection = new SeriesCollection();
            OldItems = new ObservableCollectionEX<Item>();           
            LoadJson(_pathToHistory);
            SeriesCollection.Add(new LineSeries
            {
                Title = "Продано",
                Values = new ChartValues<double> { 0 },
                LineSmoothness = 0,
                PointForeground = Brushes.Blue,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Blue,
                PointGeometrySize = 10

            });
            SeriesCollection.Add(new LineSeries
            {
                Title = "Собрано",
                Values = new ChartValues<double> { 0 },
                LineSmoothness = 0,
                PointForeground = Brushes.Red,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Red,
                PointGeometrySize = 10
            });

            CalculationsCommand = new RelayCommand(o =>
            {                              
                double allSold = 0;
                SeriesCollection[0].Values.Clear();
                SeriesCollection[1].Values.Clear();    
                if (Week)
                {                                                           
                    DateTime timeP = DateTime.Now;
                    DateTime timeM = DateTime.Now;
                    timeP.AddDays(7);
                    timeM.AddDays(-7);
                    timeP = timeP.AddDays(7);
                    timeM = timeM.AddDays(-7);
                    for (int i = 0; i<OldItems.Count; i++)
                    {
                        if (OldItems[i].Date <= timeP & OldItems[i].Date >= timeM)
                        {                           
                            allSold += OldItems[i].Sold;                          
                            SeriesCollection[0].Values.Add(Convert.ToDouble((OldItems[i].Sold)));
                            SeriesCollection[1].Values.Add(Convert.ToDouble((OldItems[i].Collected)));
                        }
                    }
                    result = ((allSold * (Convert.ToDouble(PriceTenEggs) / 10)) - (Convert.ToDouble(PriceBag) * Convert.ToDouble(CountBag))).ToString() + " ₽";
                }
                else if(Month)
                {                   
                    DateTime timeP = DateTime.Now;
                    DateTime timeM = DateTime.Now;
                    timeP.AddDays(30);
                    timeM.AddDays(-30);
                    timeP = timeP.AddDays(30);
                    timeM = timeM.AddDays(-30);
                    for (int i = 0; i < OldItems.Count; i++)
                    {
                        if (OldItems[i].Date <= timeP & OldItems[i].Date >= timeM)
                        {
                            allSold += OldItems[i].Sold;
                            SeriesCollection[0].Values.Add(Convert.ToDouble((OldItems[i].Sold)));
                            SeriesCollection[1].Values.Add(Convert.ToDouble((OldItems[i].Collected)));
                        }
                    }
                    result = ((allSold * (Convert.ToDouble(PriceTenEggs) / 10)) - (Convert.ToDouble(PriceBag) * Convert.ToDouble(CountBag))).ToString() + " ₽";
                }
                else if(HalfYear)
                {                   
                    DateTime timeP = DateTime.Now;
                    DateTime timeM = DateTime.Now;
                    timeP.AddDays(182);
                    timeM.AddDays(-182);
                    timeP = timeP.AddDays(182);
                    timeM = timeM.AddDays(-182);
                    for (int i = 0; i < OldItems.Count; i++)
                    {
                        if (OldItems[i].Date <= timeP & OldItems[i].Date >= timeM)
                        {
                            allSold += OldItems[i].Sold;
                            SeriesCollection[0].Values.Add(Convert.ToDouble((OldItems[i].Sold)));
                            SeriesCollection[1].Values.Add(Convert.ToDouble((OldItems[i].Collected)));
                        }
                    }
                    result = ((allSold * (Convert.ToDouble(PriceTenEggs) / 10)) - (Convert.ToDouble(PriceBag) * Convert.ToDouble(CountBag))).ToString() + " ₽";
                }
                else if(Year)
                {                    
                    DateTime timeP = DateTime.Now;
                    DateTime timeM = DateTime.Now;
                    timeP.AddDays(365);
                    timeM.AddDays(-365);
                    timeP = timeP.AddDays(365);
                    timeM = timeM.AddDays(-365);
                    for (int i = 0; i < OldItems.Count; i++)
                    {
                        if (OldItems[i].Date <= timeP & OldItems[i].Date >= timeM)
                        {
                            allSold += OldItems[i].Sold;
                            SeriesCollection[0].Values.Add(Convert.ToDouble((OldItems[i].Sold)));
                            SeriesCollection[1].Values.Add(Convert.ToDouble((OldItems[i].Collected)));
                        }
                    }
                    result = ((allSold * (Convert.ToDouble(PriceTenEggs) / 10)) - (Convert.ToDouble(PriceBag) * Convert.ToDouble(CountBag))).ToString() + " ₽";
                }
                else if(AllTime)
                {                   
                    for (int i = 0; i < OldItems.Count; i++)
                    {                                             
                            allSold += OldItems[i].Sold;
                            SeriesCollection[0].Values.Add(Convert.ToDouble((OldItems[i].Sold)));
                            SeriesCollection[1].Values.Add(Convert.ToDouble((OldItems[i].Collected)));
                    }
                    result = ((allSold * (Convert.ToDouble(PriceTenEggs) / 10)) - (Convert.ToDouble(PriceBag) * Convert.ToDouble(CountBag))).ToString() + " ₽";
                }
               

            });           
        }

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

        public void LoadJson(string path)
        {
            if (IsValidJson(path))
            {
                string json = File.ReadAllText(path);
                OldItems = JsonConvert.DeserializeObject<ObservableCollectionEX<Item>>(json);
            }
        }
    }
}
