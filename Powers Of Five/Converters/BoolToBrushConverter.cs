using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Powers_Of_Five
{
    public class BoolToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object
            parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var v = (bool) value;
                Brush[] b = ParameterToBrushes(parameter);

                if (v)
                {
                    return b[0];
                }
                else
                {
                    return b[1];
                }
            }
            catch (Exception ex)
            {
                return Brushes.Black;
            }
        }

        private Brush[] ParameterToBrushes(object parameter)
        {
            var sp = parameter as string;
            bool returnDefault = false;

            Brush trueBrush = Brushes.Black;
            Brush falseBrush = Brushes.Red;

            if (!string.IsNullOrWhiteSpace(sp))
            {
                var ss = sp.Split(';');

                if (ss.Length > 0)
                {
                    trueBrush = GetBrush(ss[0], trueBrush);
                    if (ss.Length > 1)
                    {
                        falseBrush = GetBrush(ss[1], falseBrush);
                    }
                }
            }

            return new []
            {
                trueBrush,
                falseBrush,
            };
        }

        private Brush GetBrush(string s, Brush defaultBrush)
        {
            try
            {
                return new SolidColorBrush((Color) ColorConverter.ConvertFromString(s));
            }
            catch (Exception)
            {
                return defaultBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
