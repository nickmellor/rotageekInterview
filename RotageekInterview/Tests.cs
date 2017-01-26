using System;
using System.Configuration;
using System.Linq;
using System.Runtime.ExceptionServices;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

// PLEASE NOTE:
// as requested, this is implemented as a test, when in fact it's not really an automated test,
// more a tiny scraping application.

// There is, strictly speaking, no need for Asserts or to express this as an NUnit test

// I've put a couple of Asserts in so the test will notice if, for example, RotaGeek adds to
// (or takes away from) its subheadings

// The test description is ambiguous as to whether the link text or the href should be returned
// for the subheadings. I've returned the link text

namespace RotageekInterview
{
    [TestFixture]
    public class Tests
    {
        IWebDriver driver;

        [SetUp]
        public void StartEachTestWith()
        {
            driver = new FirefoxDriver();
        }

        [Test]
        public void RotageekTest()
        {
            driver.Url = "http://www.google.com";

            Assert.True(driver.Title == "Google",
                "Something seems to be wrong with the Selenium binding. I should be at the Google search page (and I'm not)");
            var searchBox = driver.FindElement(By.Id("lst-ib"));
            searchBox.SendKeys("RotaGeek" + Keys.Enter);

            // wait for SERP page to be rendered
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(7));
            wait.Until(d => d.Title.StartsWith("RotaGeek", StringComparison.OrdinalIgnoreCase));
            Assert.True(driver.Title.StartsWith("RotaGeek"),
                "Something went wrong with the Google search results. I should be on the SERP for 'RotaGeek' (I'm not)");

            // NB be careful of XPATH indices in IE <= 9-- they're zero-based by default
            // See http://www.w3schools.com/xml/xpath_syntax.asp
            // ASSUMPTION: Google only shows subheadings for the first search result
            // so list of all subheading patterns found on the SERP is the same as the subheadings of the first SERP
            var allSubHeadings = driver.FindElements(By.XPath("//h3[@class='r'][1]/a[@class='l']"));
            var firstSerpSubHeadings = allSubHeadings;

            // subheading text saved to a list
            var subHeadings = firstSerpSubHeadings.Select(subHeading => subHeading.Text).ToList();

            // "more results from" heading added
            var moreSubHeadingElement = driver.FindElements(By.XPath("//div[@class='mrf']/a"));
            var moreSubHeading = moreSubHeadingElement[0].Text;
            subHeadings.Add(moreSubHeading);

            foreach (var subHeading in subHeadings)
            {
                Console.WriteLine(subHeading);
            }
            Assert.True(subHeadings.Count == 7,
                $"I was expecting 7 subheadings under first search result including a 'more results from'. Found {subHeadings.Count}.");
        }

        [TearDown]
        public void EndEachTestWith()
        {
            driver.Close();
        }
    }
}