using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CuteDev.Database
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object TryTypeConvert(this object p, Type type)
        {
            if (type == typeof(Boolean))
            {
                Boolean result = new Boolean();
                result = (p.ToString() == "True");
                return result;
            }

            if (type == typeof(Decimal?) && p.GetType() == typeof(string))
            {
                Decimal outValue;
                return Decimal.TryParse(p.ToString(), out outValue) ? (Decimal?)outValue : null;
            }

            if (type == typeof(Decimal) && p.GetType() == typeof(string))
            {
                return Decimal.Parse(p.ToString());
            }

            if (type == typeof(Int32) && p.GetType() == typeof(string))
            {
                return Int32.Parse(p.ToString());
            }

            if (type == typeof(Int32?) && p.GetType() == typeof(string))
            {
                int outValue;
                return int.TryParse(p.ToString(), out outValue) ? (int?)outValue : null;
            }

            if (type == typeof(Int64) && p.GetType() == typeof(string))
            {
                return Int64.Parse(p.ToString());
            }

            if (type == typeof(Byte) && p.GetType() == typeof(string))
            {
                return Byte.Parse(p.ToString());
            }

            if (type == typeof(DateTime?) && p.GetType() == typeof(string))
            {
                DateTime outValue;
                return DateTime.TryParse(p.ToString(), out outValue) ? (DateTime?)outValue : null;
            }

            if (type == typeof(DateTime?) && p.GetType() == typeof(DateTime))
            {
                DateTime outValue;
                return DateTime.TryParse(p.ToString(), out outValue) ? (DateTime?)outValue : null;
            }

            if (type == typeof(DateTime) && p.GetType() == typeof(string))
            {
                return DateTime.Parse(p.ToString());
            }

            return p;
        }

        /// <summary>
        /// Checks empty value
        /// </summary>
        /// <param name="val">Value</param>
        /// <returns>If value is empty then returns true</returns>
        /// <edit date="" sign=""></edit>
        public static bool IsEmpty(this string val)
        {
            return val == String.Empty || String.IsNullOrEmpty(val) || val.Replace(" ", "")
                                                                          .Replace("\t", "")
                                                                          .Replace("\r", "")
                                                                          .Replace("\n", "") == "" ? true : false;
        }

        /// <summary>
        /// gecerli bir tarihsaat nesnesi olup olmadigini dondurur. (volkansendag - 13.01.2016)
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsDateTime(this object val)
        {
            if (val.GetType() == typeof(DateTime) || val.GetType() == typeof(DateTime?))
                return true;

            if (val == null || val.ToString().IsEmpty())
                return false;

            DateTime dateResult;

            DateTime.TryParse(val.ToString(), out dateResult);

            return (dateResult != null);
        }

        /// <summary>
        /// gecerli bir tarih nesnesi olup olmadigini dondurur. (volkansendag - 13.01.2016)
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsDate(this object val)
        {
            if (!val.IsDateTime())
                return false;

            DateTime dateResult;

            DateTime.TryParse(val.ToString(), out dateResult);

            return (dateResult.StartOfDay() == dateResult);
        }

        /// <summary>
        /// Tarihteki gunun baslangic zamanini verir. (volkansendag - 13.01.2016)
        /// </summary>
        /// <param name="theDate"></param>
        /// <returns></returns>
        public static DateTime StartOfDay(this DateTime theDate)
        {
            return theDate.Date;
        }

        /// <summary>
        /// Tarihteki gunun bitis zamanini verir. (volkansendag - 13.01.2016)
        /// </summary>
        /// <param name="theDate"></param>
        /// <returns></returns>
        public static DateTime EndOfDay(this DateTime theDate)
        {
            return theDate.Date.AddDays(1).AddTicks(-1);
        }
    }
}
