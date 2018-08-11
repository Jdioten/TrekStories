using Moq;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

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

        public static T WithAuthenticatedUser<T>(this T controller, string userId)
            where T : Controller
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var identity = new GenericIdentity(userId);
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", userId));
            var principal = new GenericPrincipal(identity, new[] { "user" });
            context.Setup(s => s.User).Returns(principal);

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            return controller;
        }
    }
}
