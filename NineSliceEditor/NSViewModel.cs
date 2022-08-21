using Microsoft.Win32;
using NineSliceEditor.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NineSliceEditor
{
    public class NSViewModel
    {
        public NSModel Model { get; set; } = new();

        public ICommand OpenFileCommand { get; set; }

        public NSViewModel()
        {
            OpenFileCommand = new RelayCommand(OnOpen);
        }

        void OnOpen(object? param)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image (.png)|*.png";

            if(dialog.ShowDialog() == true)
            {
                Model.Image = new(new Uri(dialog.FileName));
            }
        }
    }
}
