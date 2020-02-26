﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xappy.Domain.Global;

namespace Xappy.Content
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public AppModel AppModel { get; set; } = DependencyService.Resolve<AppModel>();

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public ICommand SkeletonCommand { get; set; }

        public ICommand BackCommand { get; set; }

        public BaseViewModel()
        {
            SkeletonCommand = new Command(async (x) =>
            {
                IsBusy = true;
                await Task.Delay(4000);
                IsBusy = false;
            });

            BackCommand = new Command((x) =>
            {
                Shell.Current.SendBackButtonPressed();
            });
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}