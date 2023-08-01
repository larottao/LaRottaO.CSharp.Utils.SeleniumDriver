//Selenium Driver
//A wrapper that makes Selenium even more intuitive to use
//By Luis Felipe La Rotta O.
//Includes code from Stack Overflow user: Arran

using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;

namespace LaRottaO.CSharp.Utils.SeleniumDriver
{
    public class SeleniumFirefox
    {
        private WebDriver driver { get; set; }

        public WebDriver newInstance()
        {
            foreach (var process in Process.GetProcessesByName("geckodriver"))
            {
                process.Kill();
            }

            var allProfiles = new FirefoxProfileManager();

            System.Collections.ObjectModel.ReadOnlyCollection<String> profiles = allProfiles.ExistingProfiles;

            foreach (String profileEncontrado in profiles)
            {
                Console.WriteLine(profileEncontrado);
            }

            FirefoxOptions options = new FirefoxOptions();

            /*
            //FirefoxProfile profile = allProfiles.GetProfile("default");
            //profile = webdriver.FirefoxProfile();
            profile.SetPreference("browser.cache.disk.enable", false);
            profile.SetPreference("browser.cache.memory.enable", false);
            profile.SetPreference("network.http.use-cache", false);
            profile.SetPreference("browser.cache.offline.enable", false);
            */

            this.driver = new FirefoxDriver(options);

            return this.driver;
        }

        public static string getElementInnerHtml(By elementType, WebDriver driver, Boolean mustBeVisible = false, Boolean mustBeEnabled = false, int maxRetries = 5, int msBetweenRetries = 1000)
        {
            Tuple<Boolean, IWebElement> element = awaitForElement(elementType, driver, mustBeVisible, mustBeEnabled, maxRetries, msBetweenRetries);

            if (!element.Item1)
            {
                return "";
            }

            return element.Item2.GetAttribute("innerHTML");
        }

        public static void goToPreviousPage(WebDriver driver)
        {
            driver.Navigate().Back();
        }

        public static Boolean changeToWindow(int windowHandle, WebDriver driver)
        {
            try
            {
                ReadOnlyCollection<String> handles = driver.WindowHandles;

                driver.SwitchTo().Window(handles[windowHandle]);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Boolean closeDriver(WebDriver driver)
        {
            try
            {
                if (driver == null)
                {
                    return true;
                }
                driver.Quit();
                driver.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool executeJavaScript(string script, WebDriver driver)
        {
            try
            {
                ((IJavaScriptExecutor)driver).ExecuteScript(script, Array.Empty<object>());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Tuple<Boolean, IWebElement> awaitForElement(By elementType, WebDriver driver, Boolean mustBeVisible = false, Boolean mustBeEnabled = false, int maxRetries = 5, int msBetweenTries = 1000)
        {
            try
            {
                int attemptNumber = 1;

                while (true)

                {
                    Tuple<Boolean, IWebElement> element = getElementOnPage(elementType, driver, mustBeVisible, mustBeEnabled);

                    if (element.Item1)
                    {
                        return new Tuple<Boolean, IWebElement>(true, element.Item2);
                    }
                    else

                    {
                        if (attemptNumber <= maxRetries)
                        {
                            Thread.Sleep(msBetweenTries);
                            attemptNumber++;
                        }
                        else
                        {
                            return new Tuple<Boolean, IWebElement>(false, null);
                        }
                    }
                }
            }
            catch
            {
                return new Tuple<Boolean, IWebElement>(false, null);
            }
        }

        public static Tuple<bool, IWebElement> getElementOnPage(By elementType, WebDriver driver, Boolean mustBeVisible = false, Boolean mustBeEnabled = false, int maxRetries = 5, int msBetweenRetries = 1000)
        {
            IWebElement element = null;

            try
            {
                element = driver.FindElement(elementType);

                if (!mustBeVisible && !element.Displayed)
                {
                    return new Tuple<bool, IWebElement>(false, element);
                }

                if (mustBeEnabled && element.Enabled)
                {
                    return new Tuple<bool, IWebElement>(false, element);
                }

                return new Tuple<bool, IWebElement>(true, element);
            }
            catch
            {
                return new Tuple<bool, IWebElement>(false, element);
            }
        }

        public static Tuple<int, IWebElement> awaitEitherOfTwoElements(By elementAType, By elementBType, WebDriver driver, Boolean mustBeVisible = false, Boolean mustBeEnabled = false, int maxRetries = 5, int msBetweenRetries = 1000)
        {
            int attemptNumber = 1;

            while (true)

            {
                try
                {
                    Tuple<Boolean, IWebElement> elementA = getElementOnPage(elementAType, driver, mustBeVisible, mustBeEnabled);
                    Tuple<Boolean, IWebElement> elementB = getElementOnPage(elementBType, driver, mustBeVisible, mustBeEnabled);

                    if (elementA.Item1 && elementB.Item1)
                    {
                        Debug.WriteLine("ERROR: Two elements found at the same time");
                        return new Tuple<int, IWebElement>(3, null);
                    }

                    if (elementA.Item1)
                    {
                        return new Tuple<int, IWebElement>(1, elementA.Item2);
                    }

                    if (elementB.Item1)
                    {
                        return new Tuple<int, IWebElement>(2, elementB.Item2);
                    }

                    //Ninguno de los dos

                    if (attemptNumber < maxRetries)
                    {
                        attemptNumber++;
                        Thread.Sleep(msBetweenRetries);
                    }
                    else
                    {
                        return new Tuple<int, IWebElement>(0, null);
                    }
                }
                catch
                {
                    return new Tuple<int, IWebElement>(0, null);
                }
            }
        }

        public static void clickOnElementByHref(String href, WebDriver driver)
        {
            SeleniumFirefox.clickOnElement(By.CssSelector("[href*='" + href + "']"), driver, false, false);
        }

        public static bool clickOnElement(By elementType, WebDriver driver, Boolean mustBeVisible = false, Boolean mustBeEnabled = false, int maxRetries = 5, int msBetweenTries = 1000)
        {
            try
            {
                Tuple<Boolean, IWebElement> elemento = awaitForElement(elementType, driver, mustBeVisible, mustBeEnabled, maxRetries, msBetweenTries);

                if (!elemento.Item1) { return false; }

                elemento.Item2.Click();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool submit(By elementType, WebDriver driver, Boolean mustBeVisible = false, Boolean mustBeEnabled = false, int maxRetries = 5, int msBetweenRetries = 1000)
        {
            try
            {
                Tuple<Boolean, IWebElement> element = awaitForElement(elementType, driver, mustBeVisible, mustBeEnabled, maxRetries, msBetweenRetries);
                if (!element.Item1) { return false; }

                element.Item2.Submit();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool typeIntroKeyIntoElement(By elementType, WebDriver driver, Boolean mustBeVisible = false, Boolean mustBeEnabled = false, int maxRetries = 5, int msBetweenTries = 1000)
        {
            try
            {
                Tuple<Boolean, IWebElement> elemento = awaitForElement(elementType, driver, mustBeVisible, mustBeEnabled, maxRetries, msBetweenTries);
                if (!elemento.Item1) { return false; }

                elemento.Item2.SendKeys(Keys.Return);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool typeTextIntoElement(By elementType, WebDriver driver, String desiredText, Boolean mustBeVisible = false, Boolean mustBeEnabled = false, int maxRetries = 5, int msBetweenTries = 1000)
        {
            try
            {
                Tuple<Boolean, IWebElement> elemento = awaitForElement(elementType, driver, mustBeVisible, mustBeEnabled, maxRetries, msBetweenTries);
                if (!elemento.Item1) { return false; }

                elemento.Item2.SendKeys(desiredText);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool clearInputText(By elementType, WebDriver driver, Boolean mustBeVisible = false, Boolean mustBeEnabled = false, int maxRetries = 5, int msBetweenTries = 1000)
        {
            try
            {
                Tuple<Boolean, IWebElement> elemento = awaitForElement(elementType, driver, mustBeVisible, mustBeEnabled, maxRetries, msBetweenTries);
                if (!elemento.Item1) { return false; }

                elemento.Item2.Clear();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Boolean maximizeBrowserWindow(WebDriver driver)
        {
            try
            {
                driver.Manage().Window.Maximize();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Boolean showWindowHandles(WebDriver driver)
        {
            try
            {
                foreach (string str in driver.WindowHandles)
                {
                    Debug.WriteLine("Window handle: " + str);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string getElementAttribute(string elementId, string attribute, WebDriver driver)
        {
            try
            {
                return driver.FindElement(By.Id(elementId)).GetAttribute(attribute);
            }
            catch
            {
                return "";
            }
        }

        public static string getElementOuterXml(By elementType, WebDriver driver, Boolean mustBeVisible = false, Boolean mustBeEnabled = false, int maxRetries = 5, int msBetweenTries = 1000)
        {
            Tuple<Boolean, IWebElement> elemento = awaitForElement(elementType, driver, mustBeVisible, mustBeEnabled, maxRetries, msBetweenTries);
            if (!elemento.Item1)
            {
                return "";
            }

            return elemento.Item2.GetAttribute("outerHTML");
        }

        public static string getFrameSource(string idFrame, WebDriver driver)
        {
            try
            {
                IWebElement frameElement = driver.FindElement(By.Id(idFrame));
                driver.SwitchTo().Frame(frameElement);
                driver.SwitchTo().DefaultContent();
                return driver.PageSource;
            }
            catch
            {
                return "";
            }
        }

        public static string getPageSource(WebDriver driver)
        {
            try
            {
                return driver.PageSource;
            }
            catch
            {
                return "";
            }
        }

        public static string getElementText(By elementType, WebDriver driver, Boolean mustBeVisible = false, Boolean mustBeEnabled = false, int maxRetries = 5, int msBetweenTries = 1000)
        {
            try
            {
                Tuple<Boolean, IWebElement> elemento = awaitForElement(elementType, driver, mustBeVisible, mustBeEnabled, maxRetries, msBetweenTries);
                if (!elemento.Item1)
                {
                    return "";
                }

                return elemento.Item2.Text;
            }
            catch
            {
                return "";
            }
        }

        public static bool selectDropDownByText(By elementType, WebDriver driver, String textoEnDropDown, Boolean mustBeVisible = false, Boolean mustBeEnabled = false, int maxRetries = 5, int msBetweenTries = 1000)
        {
            try
            {
                Tuple<Boolean, IWebElement> elemento = awaitForElement(elementType, driver, mustBeVisible, mustBeEnabled, maxRetries, msBetweenTries);
                if (!elemento.Item1) { return false; }

                new SelectElement(elemento.Item2).SelectByText(textoEnDropDown, false);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool selectDropDownByValue(By elementType, WebDriver driver, String requiredValue, Boolean mustBeVisible = false, Boolean mustBeEnabled = false, int maxRetries = 5, int msBetweenTries = 1000)
        {
            try
            {
                Tuple<Boolean, IWebElement> elemento = awaitForElement(elementType, driver, mustBeVisible, mustBeEnabled, maxRetries, msBetweenTries);
                if (!elemento.Item1) { return false; }

                new SelectElement(elemento.Item2).SelectByValue(requiredValue);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Boolean goToUrl(string url, WebDriver driver)
        {
            try
            {
                if (!url.StartsWith("https://") && !url.StartsWith("http://"))
                {
                    url = "http://" + url;
                }

                driver.Url = url;
                driver.Navigate();
                return true;
            }
            catch
            {
                Console.WriteLine("Impossible to visit URL: " + url);
                return false;
            }
        }
    }
}