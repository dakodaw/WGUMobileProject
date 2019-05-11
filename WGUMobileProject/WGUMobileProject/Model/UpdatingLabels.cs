using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace WGUMobileProject.Model
{
    public class UpdatingLabels : INotifyPropertyChanged
    {

        string inLabel;
        public event PropertyChangedEventHandler PropertyChanged;

        public UpdatingLabels()
        {
            inLabel = "";
        }

        public string UpdateString(string input)
        {
            
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (input != inLabel)
                {
                    inLabel = input;
                }
                return true;
            });

            return input;
        }

        //string Property
        public string myString
        {
            set
            {
                if(inLabel != value)
                {
                    inLabel = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("myString"));
                }
            }
            get
            {
                return inLabel;
            }
        }
        
        
    }
}
