using EGGSA.Services;
using System;

namespace EGGSA.Models
{
    internal class Item : Observer
    {
        private DateTime _date;

        public DateTime Date
        {
            get => _date; 
            set { _date = value; OnPropertyChanged(); }
        }

        private string _history;

        public string History
        {
            get => _history;
            set { _history = value; OnPropertyChanged(); }
        }

        private int _collected;

        public int Collected
        {
            get => _collected;
            set { _collected = value; OnPropertyChanged(); }
        }

        private int _sold;

        public int Sold
        {
            get => _sold;
            set { _sold = value; OnPropertyChanged(); }
        }




    }
}
