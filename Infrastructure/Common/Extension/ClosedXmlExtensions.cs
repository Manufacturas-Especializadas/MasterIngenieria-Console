using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Extension
{
    public static class ClosedXmlExtensions
    {
        public static int? GetValueOrNullInt(this IXLCell cell)
        {
            return cell.IsEmpty() || !cell.TryGetValue(out int result) ? null : result;
        }

        public static decimal? GetValueOrNullDecimal(this IXLCell cell)
        {
            return cell.IsEmpty() || !cell.TryGetValue(out decimal result) ? null : result; 
        }

        public static string GetValueOrEmptyString(this IXLCell cell)
        {
            return cell.IsEmpty() ? string.Empty : cell.GetValue<string>().Trim();
        }

    }
}