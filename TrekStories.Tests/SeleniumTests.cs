using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Firefox;

namespace TrekStories.Tests
{
    [TestClass]
    public class SeleniumTests
    {
        
        [TestMethod]
        public void CanDisplayAboutPageTitle()
        {
            // Initialize the Firefox Driver
            using (var driver = new FirefoxDriver())
            {
                // 1. Maximize the browser
                driver.Manage().Window.Maximize();

                // 2. Go to TrekStories About page
                driver.Navigate().GoToUrl("https://trekstories.azurewebsites.net/Home/About");

                // 3. Check Page Title
                Assert.AreEqual("About - Trek Stories", driver.Title);
            }
        }

        [TestMethod]
        public void SearchCriterionKeptWhenSearchResultsDisplayed()
        {
            // Initialize the Firefox Driver
            using (var driver = new FirefoxDriver())
            {
                // 1. Maximize the browser
                driver.Manage().Window.Maximize();

                // 2. Go to TrekStories Search page
                driver.Navigate().GoToUrl("https://trekstories.azurewebsites.net/Trip/Search");

                // 3. Find the search textbox (by ID) for keyword
                var searchBox = driver.FindElementById("TitleKeyword");

                // 4. Enter the text (to search for) in the textbox
                searchBox.SendKeys("Zagori");

                // 5. Find the search button (by Name) on the homepage
                var searchButton = driver.FindElementById("SearchButton");

                // 6. Click "Submit" to start the search
                searchButton.Submit();

                // 7. Check that keyword is still displayed in seach form
                searchBox = driver.FindElementById("TitleKeyword");
                Assert.AreEqual("Zagori", searchBox.GetAttribute("value"));
            }
        }

        [TestMethod]
        public void CanClickOnRowToDisplayStepDetails()
        {
            // Initialize the Firefox Driver
            using (var driver = new FirefoxDriver())
            {
                // 1. Maximize the browser
                driver.Manage().Window.Maximize();

                // 2. Go to a Trip Details page
                driver.Navigate().GoToUrl("https://trekstories.azurewebsites.net/Trip/Details/1");

                // 3. Find the first cell which contains a step's destinations
                var stepCell = driver.FindElementByXPath("//td/h5[contains(text(), 'From')]");

                // 4. Click on the cell
                stepCell.Click();

                // 5. Check that the Step Details page is displayed
                Assert.AreEqual("Step Details - Trek Stories", driver.Title);
            }
        }

        [TestMethod]
        public void DoesNotDisplayReviewPicturesWhenNotLoggedOn()
        {
            // Initialize the Firefox Driver
            using (var driver = new FirefoxDriver())
            {
                // 1. Maximize the browser
                driver.Manage().Window.Maximize();

                // 2. Go to a Step Details page
                driver.Navigate().GoToUrl("https://trekstories.azurewebsites.net/Step/Details/14");

                // 3. Find the picture area
                var pictureArea = driver.FindElementById("PictureArea");

                // 4. Check that pictures are not being displayed
                Assert.IsFalse(pictureArea.Displayed);
            }
        }
    }
}
