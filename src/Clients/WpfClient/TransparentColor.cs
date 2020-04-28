using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Triton.WpfClient
{
    public class TransparentColor : DependencyObject
    {
        public static readonly DependencyProperty BaseColorProperty =
            DependencyProperty.Register(nameof(BaseColor), typeof(Color?), typeof(TransparentColor), new PropertyMetadata(null,OnBaseColorChanged,CoerceProperColor));

        private static object CoerceProperColor(DependencyObject d, object baseValue)
        {
            throw new NotImplementedException();
        }

        private static void OnBaseColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public static readonly DependencyProperty TransparencyProperty = DependencyProperty.Register(nameof(Transparency), typeof(float), typeof(TransparentColor), new PropertyMetadata(1f));

        public Color BaseColor
        {
            get => (Color)GetValue(BaseColorProperty);
            set => SetValue(BaseColorProperty, value);
        }

        public float Transparency
        {
            get { return (float)GetValue(TransparencyProperty); }
            set { SetValue(TransparencyProperty, value); }
        }

        public static implicit operator System.Windows.Media.Color(TransparentColor color)
        {
            var bc = color.BaseColor;
            
            return new System.Windows.Media.Color()
            {
                ScA = color.Transparency,
                ScR = bc.ScR,
                ScG = bc.ScG,
                ScB = bc.ScB,
            };
        }
    }

    public class TransparentColorAdapter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TransparentColor c)
            {
                if (targetType == typeof(System.Windows.Media.Color)) return (System.Windows.Media.Color)c;
            }

            return targetType.Default();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
