using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NineSliceEditor
{
    public class NSModel : INotifyPropertyChanged
    {
        int top;
        int bottom;
        int left;
        int right;
        BitmapImage image = new();

        public int Top
        {
            get => top;
            set
            {
                if (Set(ref top, value, nameof(Top)))
                {
                    OnChanged(nameof(Center));
                }
            }
        }
        public int Bottom
        {
            get { return bottom; }
            set
            {
                if (Set(ref bottom, value, nameof(Bottom)))
                {
                    OnChanged(nameof(Center));
                }
            }
        }
        public int Left
        {
            get { return left; }
            set
            {
                if (Set(ref left, value, nameof(Left)))
                {
                    OnChanged(nameof(Center));
                }
            }
        }
        public int Right
        {
            get { return right; }
            set
            {
                if (Set(ref right, value, nameof(Right)))
                {
                    OnChanged(nameof(Center));
                }
            }
        }

        public Rectangle Center
        {
            get { return new(left, top, right - left, bottom - top); }
            set
            {
                if (Center != value)
                {
                    Set(ref top, value.Top, nameof(Top));
                    Set(ref bottom, value.Bottom, nameof(Bottom));
                    Set(ref left, value.Left, nameof(Left));
                    Set(ref right, value.Right, nameof(Right));

                    OnChanged(nameof(Center));
                }
            }
        }

        public BitmapImage Image { get => image; set { image = value; OnChanged(nameof(Image)); } }

        public event PropertyChangedEventHandler? PropertyChanged;

        void OnChanged(string name)
        {
            PropertyChanged?.Invoke(this, new(name));
        }

        bool Set(ref int prop, int value, string name)
        {
            if (prop != value)
            {
                prop = value;
                OnChanged(name);
                return true;
            }
            return false;
        }
    }
}
