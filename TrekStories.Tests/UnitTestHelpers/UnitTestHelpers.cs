using System.Globalization;
using System.Web.Mvc;

namespace TrekStories.Tests.UnitTestHelpers
{
    static class UnitTestHelpers
    {
        //https://books.google.ie/books?id=7ZOXKItbQC4C&pg=PA207&lpg=PA207&dq=unit+test+tryupdatemodel&source=bl&ots=GypWnJd80e&sig=5Ty5cPnYFXFIeV3sG18mShcSeo0&hl=en&sa=X&ved=2ahUKEwjlgaPE99vcAhVE26QKHQUgD2wQ6AEwB3oECAIQAQ#v=onepage&q=unit%20test%20tryupdatemodel&f=false
        public static T WithIncomingValues<T> (this T controller, FormCollection values)
            where T : Controller
        {
            controller.ControllerContext = new ControllerContext();
            controller.ValueProvider = new NameValueCollectionValueProvider(values, CultureInfo.CurrentCulture);
            return controller;
        }
    }
}
