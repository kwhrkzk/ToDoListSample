using Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ToDoListSample
{
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Status status)
            {
                switch(status.status)
                {
                    case StatusEnum.Ready: return false;
                    case StatusEnum.Doing: return null ;
                    case StatusEnum.Done: return true;
                    default: throw new ArgumentException("StatusEnumの範囲外");
                }
            }
            else
                throw new ArgumentException("StatusEnumの範囲外");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool?)value;
            return b.HasValue ? b.Value ? Status.Done : Status.Ready : Status.Doing;
        }
    }
}
