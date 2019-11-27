using System;

namespace FinancialEntries.Extensions
{
    public static class AbsoluteDateTime
    {
         public static DateTime AbsoluteStart(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        public static DateTime AbsoluteEnd(this DateTime dateTime)
        {
            return AbsoluteStart(dateTime).AddDays(1).AddTicks(-1);
        }
    }
}