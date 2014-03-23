using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Powers_Of_Five
{
    public class NumberToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object
            parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is int)
                {
                    return ((int)value).ToString(parameter as string);
                }
                else if (value is long)
                {
                    return ((long)value).ToString(parameter as string);
                }
                return value.ToString();
            }
            catch (Exception ex)
            {
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
