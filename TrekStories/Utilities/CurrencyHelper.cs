using System.Globalization;

namespace TrekStories.Utilities
{
    static public class CurrencyHelper
    {
        static public string GetCurrency()
        {
            return CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
        }
    }
}