using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

//# currently supported version: Microsoft Edge: 97.0.1072.69 (Official build) (64-bit)
//# MS-Edge driver url - https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/
//# Note: to check your ms edge browser version:
//# 1. Open your MS Edge
//# 2. Navigate to url input textbox
//# 3. type: chrome:version
//# 4. You should see something similar to this: Microsoft Edge: 97.0.1072.69 (Official build) (64-bit)

//# Fields for Fill_Me.txt file    
//# location = '<address location>'
//# gpn_number = '<GPN Number Mandatory>'
//# mobile_number = '<cp #>'
//# url = '<target site url'
//# delay_per_seconds = '2'

namespace SGV_Health_Tracker
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd,
                                    IntPtr hWndInsertAfter,
                                    int X,
                                    int Y,
                                    int cx,
                                    int cy,
                                    uint uFlags);
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        const uint SWP_NOSIZE = 0x0001, SWP_NOMOVE = 0x0002, SWP_SHOWWINDOW = 0x0040;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();


        static void Main(string[] args)
        {
            // Keep the console viewable
            IntPtr handle = GetConsoleWindow();
            SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

            string user_location, user_gpn_number, user_mobile_num, site_url;
            int delay_per_sec = 0;
            var isValidValues = true;
            EdgeDriver driver = null;

            try
            {

                var appdata_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                if (appdata_path.ToLower().Contains("appdata"))
                    appdata_path = appdata_path.Substring(0, appdata_path.ToLower().IndexOf("appdata", 0) + 7);

                var edge_profile_path = appdata_path + "\\local\\microsoft\\edge\\user data";

                var texts = File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + "\\Fill_Me.txt");

                if (texts.Length < 4)
                    throw new Exception("Incomplete Fields. Please consult the owner or fix the necessary fields.");
                else
                {
                    user_location = texts[0].Substring(texts[0].IndexOf("'")).Replace("'", "");     //# Mandatory.
                    user_gpn_number = texts[1].Substring(texts[1].IndexOf("'")).Replace("'", "");   //# this starts with PH (see GT&E under your profile to know your GPN #) Mandatory.
                    user_mobile_num = texts[2].Substring(texts[2].IndexOf("'")).Replace("'", "");
                    site_url = texts[3].Substring(texts[3].IndexOf("'")).Replace("'", "");          //# Target site to fill the health tracker Mandatory.
                    delay_per_sec = int.Parse(texts[4].Substring(texts[4].IndexOf("'")).Replace("'", ""));

                    if (user_gpn_number.Trim().Length == 0 || user_location.Trim().Length == 0)
                        isValidValues = false;

                }

                if (!isValidValues)
                    throw new Exception("fields must not be empty! Please consult the owner or fix the necessary fields.");

                if (user_mobile_num.Trim().Length == 0)
                    user_mobile_num = "same";
                if (delay_per_sec > 1)
                    delay_per_sec = delay_per_sec * 1000;
                else
                {
                    delay_per_sec = 2000; // set a default delay second of 2
                }

                var edge_driver_path = System.IO.Directory.GetCurrentDirectory() + @"\edgedriver_win64"; // \msedgedriver.exe

                var options = new EdgeOptions();
                options.AddArgument("--user-data-dir=" + edge_profile_path);
                EdgeDriverService service = EdgeDriverService.CreateDefaultService(edge_driver_path, @"msedgedriver.exe");

                driver = new EdgeDriver(service, options) { Url = site_url };

                Thread.Sleep(12000);

                var text_gpn = driver.FindElement(By.XPath("//*[@id=" + @"""form-container""" + "]/div/div/div[1]/div/div[1]/div[2]/div[2]/div[2]/div/div[2]/div/div/input"), 10);
                text_gpn.Click();
                text_gpn.Clear();
                text_gpn.SendKeys(user_gpn_number);

                Thread.Sleep(delay_per_sec);

                var rad_consent = driver.FindElement(By.XPath("//*[@id=" + @"""form-container""" + "]/div/div/div[1]/div/div[1]/div[2]/div[2]/div[3]/div/div[2]/div/div[1]/div/label/span"), 10);

                var label_description = rad_consent.Text;

                if (label_description.ToLower() == "yes, i agree and give consent. proceed to answer the form" ||
                    label_description.ToLower().Contains("yes, i agree"))
                    rad_consent.Click();
                else
                {
                    rad_consent = driver.FindElement(By.XPath("//*[@id=" + @"""form-container""" + "]/div/div/div[1]/div/div[1]/div[2]/div[2]/div[3]/div/div[2]/div/div[2]/div/label/span"), 10);
                    rad_consent.Click();
                }

                Thread.Sleep(delay_per_sec);

                var rad_feeling_well = driver.FindElement(By.XPath("//*[@id=" + @"""form-container""" + "]/div/div/div[1]/div/div[1]/div[2]/div[2]/div[4]/div/div[2]/div/div[1]/div/label/span"), 10);
                rad_feeling_well.Click();

                Thread.Sleep(delay_per_sec);

                var rad_work_from = driver.FindElement(By.XPath("//*[@id=" + @"""form-container""" + "]/div/div/div[1]/div/div[1]/div[2]/div[2]/div[5]/div/div[2]/div/div[1]/div/label/span"), 10);
                rad_work_from.Click();

                Thread.Sleep(delay_per_sec);

                var text_location = driver.FindElement(By.XPath("//*[@id=" + @"""form-container""" + "]/div/div/div[1]/div/div[1]/div[2]/div[2]/div[6]/div/div[2]/div/div/input"), 10);
                text_location.Click();
                text_location.Clear();
                text_location.SendKeys(user_location);

                Thread.Sleep(delay_per_sec);

                var rad_living_covid_diagnosed = driver.FindElement(By.XPath("//*[@id=" + @"""form-container""" + "]/div/div/div[1]/div/div[1]/div[2]/div[2]/div[7]/div/div[2]/div/div[2]/div/label/span"), 10);
                rad_living_covid_diagnosed.Click();

                Thread.Sleep(delay_per_sec);

                var rad_closed_contact = driver.FindElement(By.XPath("//*[@id=" + @"""form-container""" + "]/div/div/div[1]/div/div[1]/div[2]/div[2]/div[8]/div/div[2]/div/div[2]/div/label/span"), 10);
                rad_closed_contact.Click();

                Thread.Sleep(delay_per_sec);

                var rad_traveled_outside = driver.FindElement(By.XPath("//*[@id=" + @"""form-container""" + "]/div/div/div[1]/div/div[1]/div[2]/div[2]/div[9]/div/div[2]/div/div[2]/div/label/span"), 10);
                rad_traveled_outside.Click();

                Thread.Sleep(delay_per_sec);

                var rad_traveled_any_NCR = driver.FindElement(By.XPath("//*[@id=" + @"""form-container""" + "]/div/div/div[1]/div/div[1]/div[2]/div[2]/div[10]/div/div[2]/div/div[2]/div/label/span"), 10);
                rad_traveled_any_NCR.Click();

                Thread.Sleep(delay_per_sec);

                var text_mobile_number = driver.FindElement(By.XPath("//*[@id=" + @"""form-container""" + "]/div/div/div[1]/div/div[1]/div[2]/div[2]/div[12]/div/div[2]/div/div/input"), 10);
                text_mobile_number.Click();
                text_mobile_number.Clear();
                text_mobile_number.SendKeys(user_mobile_num);

                Thread.Sleep(delay_per_sec);

                var rad_i_accept = driver.FindElement(By.XPath("//*[@id=" + @"""form-container""" + "]/div/div/div[1]/div/div[1]/div[2]/div[2]/div[13]/div/div[2]/div/div/div/label/span"), 10);
                rad_i_accept.Click();

                Thread.Sleep(delay_per_sec);

                var rad_send_me_receipt = driver.FindElement(By.XPath("//*[@id=" + @"""form-container""" + "]/div/div/div[1]/div/div[1]/div[2]/div[3]/div/div/label/span"), 10);
                rad_send_me_receipt.Click();

                Thread.Sleep(delay_per_sec);

                var button_submit = driver.FindElement(By.XPath("//*[@id=" + @"""form-container""" + "]/div/div/div[1]/div/div[1]/div[2]/div[4]/div[1]/button/div"), 10);
                button_submit.Click();

                // End of the process below

                Console.WriteLine("\r\n\r\nPress any key to close.");
                Console.ReadLine();
            }
            catch (Exception)
            {
                Console.WriteLine("\r\nSomething unexpected happens closing the browser. Please wait for few seconds.");
                throw;
            }
            finally
            {
                driver.Close();
                Console.WriteLine("\r\nClosing the browser. Please wait for few seconds.");
                driver.Quit();
                driver.Dispose();
            }
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
