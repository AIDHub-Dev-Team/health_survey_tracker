using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
// using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Remote;

using Microsoft.Edge.SeleniumTools;

namespace SGV_Health_Tracker
{
    class Program
    {
        static void Main(string[] args)
        {
            string user_address, user_location, user_gpn_number, user_mobile_num, site_url;

            var appdata_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            if (appdata_path.ToLower().Contains("appdata"))
                appdata_path = appdata_path.Substring(0, appdata_path.ToLower().IndexOf("appdata", 0) + 7);

            var edge_profile_path = appdata_path + "\\local\\microsoft\\edge\\user data";

            var texts = File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + "\\Fill_Me.txt");

            if (texts.Length < 4)
                throw new Exception("Incomplete Fields. Please consult the owner or fix the necessary fields");
            else
            {
                user_address = texts[0].Substring(texts[0].IndexOf("'")).Replace("'","");
                user_location = texts[1].Substring(texts[1].IndexOf("'")).Replace("'", "");
                user_gpn_number = texts[2].Substring(texts[2].IndexOf("'")).Replace("'", "");
                user_mobile_num = texts[3].Substring(texts[3].IndexOf("'")).Replace("'", "");
                site_url = texts[4].Substring(texts[4].IndexOf("'")).Replace("'", "");

            }
             
            var edge_driver_path = System.IO.Directory.GetCurrentDirectory() + @"\edgedriver_win64"; // \msedgedriver.exe

            var a = new OpenQA.Selenium.Chrome.ChromeOptions();
            a.AddArgument("--user-data-dir=" + edge_profile_path);
            a.AddArgument("--profile-directory=Profile 1");
            a.AddArgument("--start-maximized");

            var options = new EdgeOptions();
            // options.UseChromium = true;
            options.AddArgument("--user-data-dir=" + edge_profile_path);
            options.AddArgument("--disable-extensions");
            options.AddArgument("--profile-directory=Profile 1");
            options.AddArgument("--start-maximized");
            EdgeDriverService service = EdgeDriverService.CreateDefaultService(edge_driver_path, @"msedgedriver.exe");

            //var driver = new EdgeDriver(options);

            var driver = new EdgeDriver(service, options) {Url = site_url};

            Thread.Sleep(5000);

            try
            {
                var label_signin = driver.FindElement(By.XPath("/html/body/div/form[1]/div/div/div[1]/div[2]/div[2]/div/div/div/div[1]/div/div"), 10);

                if (label_signin.Text.ToLower().Trim() == "sign in")
                {
                    driver.Navigate().GoToUrl("edge://settings/profiles");
                    Thread.Sleep(2000);

                    System.Windows.Forms.MessageBox.Show("Please sign in your work profile to pass the SSO.", "Browser Profile", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                    var button_signin = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/div/div/div[2]/div/div/div/div[2]/div/div/div/div[1]/div[3]/button[2]/span"), 10);
                    button_signin.Click();
                }
            }
            catch (Exception)
            {

                throw;
            }

            Console.WriteLine("\r\n\r\nPlease sign in your work profile to the browser before you proceed.\r\n\r\n");
            Console.ReadLine();

            var label_profile = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/div/div/div[2]/div/div/div/div[2]/div/div/div/div[1]/div[2]/p"), 10);
            if (label_profile.Text.ToLower().Trim( ) == "work")
            {
                driver.Navigate().GoToUrl(site_url);
                Thread.Sleep(2000);

                var label_signin_email = driver.FindElement(By.XPath("/html/body/div/form[1]/div/div/div[1]/div[2]/div[2]/div/div/div/div[2]/div/div/div[1]/div/div/div/div[2]/div[2]/small"), 10);
                if ( label_signin_email.Text.ToLower().Contains("@ph.ey.com") )
                    label_signin_email.Click();
                else
                {
                    label_signin_email = driver.FindElement(By.XPath("/html/body/div/form[1]/div/div/div[1]/div[2]/div[2]/div/div/div/div[2]/div/div/div[2]/div/div/div/div[2]/div[2]/small"), 10);
                    if (label_signin_email.Text.ToLower().Contains("@ph.ey.com"))
                        label_signin_email.Click();
                }
                Thread.Sleep(2000);
            }

            Thread.Sleep(2000);

            var rad_consent = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[2]/div[2]/div/div[2]/div/div[2]/div/label/span/span"), 10);

            var label_description = rad_consent.Text;

            if (label_description.ToLower() == "yes, i agree and give consent. proceed to answer the survey")
                rad_consent.Click();
            else
            {
                rad_consent = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[2]/div[2]/div/div[2]/div/div[1]/div/label/span/span"), 10);
                rad_consent.Click();
            }

            Thread.Sleep(500);

            var button_next = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[3]/div[1]/button/div"), 10);
            button_next.Click();

            Thread.Sleep(500);

            var rad_feeling_well = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[2]/div[2]/div/div[2]/div/div[1]/div/label"), 10);
            rad_feeling_well.Click();

            Thread.Sleep(500);

            var text_address = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[2]/div[3]/div/div[2]/div/div/input"), 10);
            text_address.Click();
            text_address.Clear();
            text_address.SendKeys(user_address);

            Thread.Sleep(500);

            var rad_work_from = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[2]/div[4]/div/div[2]/div/div[1]/div/label"), 10);
            rad_work_from.Click();

            Thread.Sleep(500);

            var text_location = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[2]/div[5]/div/div[2]/div/div/input"), 10);
            text_location.Click();
            text_location.Clear();
            text_location.SendKeys(user_location);

            Thread.Sleep(500);

            var text_gpn = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[2]/div[6]/div/div[2]/div/div/input"), 10);
            text_gpn.Click();
            text_gpn.Clear();
            text_gpn.SendKeys(user_gpn_number);

            Thread.Sleep(500);

            var text_mobile_number = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[2]/div[7]/div/div[2]/div/div/input"), 10);
            text_mobile_number.Click();
            text_mobile_number.Clear();
            text_mobile_number.SendKeys(user_mobile_num);

            Thread.Sleep(500);

            var rad_family_not_well = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[2]/div[8]/div/div[2]/div/div[2]/div/label"), 10);
            rad_family_not_well.Click();

            Thread.Sleep(500);

            var rad_living_covid_diagnosed = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[2]/div[9]/div/div[2]/div/div[2]/div/label"), 10);
            rad_living_covid_diagnosed.Click();

            Thread.Sleep(500);

            var rad_closed_contact = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[2]/div[10]/div/div[2]/div/div[2]/div/label"), 10);
            rad_closed_contact.Click();

            Thread.Sleep(500);

            var rad_contact_fever_throat = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[2]/div[11]/div/div[2]/div/div[2]/div/label"), 10);
            rad_contact_fever_throat.Click();

            Thread.Sleep(500);

            var rad_traveled_outside = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[2]/div[12]/div/div[2]/div/div[2]/div/label"), 10);
            rad_traveled_outside.Click();

            Thread.Sleep(500);

            button_next = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[3]/div[1]/button[2]/div"), 10);
            button_next.Click();

            Thread.Sleep(500);

            var rad_traveled_any_NCR = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[2]/div[2]/div/div[2]/div/div[2]/div/label"), 10);
            rad_traveled_any_NCR.Click();

            Thread.Sleep(500);

            var rad_i_accept = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[2]/div[3]/div/div[2]/div/div/div/label"), 10);
            rad_i_accept.Click();

            Thread.Sleep(500);

            var rad_send_me_receipt = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[3]/div/div/label/span"), 10);
            rad_send_me_receipt.Click();

            Thread.Sleep(500);

            var button_submit = driver.FindElement(By.XPath("/html/body/div/div/div/div/div/div/div[1]/div[2]/div[4]/div[1]/button[2]/div"), 10);
            button_submit.Click();

            // End of the process below

            Console.WriteLine("Press any key to close.");
            Console.ReadLine();
            driver.Close();
            Console.WriteLine("Closing the browser.");
            driver.Quit();
            driver.Dispose();
            
        }        
    }

    public static class WebDriverExtensions
    {
        /// <summary>
        /// Site Reference: 
        /// URL: https://stackoverflow.com/questions/6992993/selenium-c-sharp-webdriver-wait-until-element-is-present
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="by"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            return driver.FindElement(by);
        }
    }
}
