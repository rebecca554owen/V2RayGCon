using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DyFetch.Comps
{
    internal class Fetcher : IDisposable
    {
        ChromeDriver driver;
        private bool disposedValue;

        public Fetcher(Models.Configs configs)
        {
            var options = CreateOptions(configs);
            driver = new ChromeDriver(options);
        }

        #region properties

        #endregion

        #region public methods
        public string Fetch(string url, List<string> csses, int timeout, int wait)
        {
            timeout = timeout > 0 ? timeout : Models.Consts.DefaultFetchTimeout;
            try
            {
                var w = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeout));
                driver.Navigate().GoToUrl(url);
                if (csses != null && csses.Count > 0)
                {
                    WaitForOneOfCsses(csses, w);
                }
                if (wait > 0)
                {
                    Thread.Sleep(wait);
                }
                // driver.GetScreenshot().SaveAsFile("c:/dyfetch-debug.png");
                return driver.PageSource;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return string.Empty;
        }

        #endregion

        #region private methods
        void WaitForOneOfCsses(IEnumerable<string> csses, WebDriverWait wait)
        {
            var bys = csses.Select(css => By.CssSelector(css)).ToList();
            wait.Until(drv => WaitForOneOfBys(drv, bys));
        }

        bool WaitForOneOfBys(IWebDriver drv, IEnumerable<By> bys)
        {
            foreach (var by in bys)
            {
                try
                {
                    var el = drv.FindElement(by);
                    if (el.Displayed)
                    {
                        Console.WriteLine("Match!");
                        return true;
                    }
                }
                catch { }
            }
            Console.WriteLine("Not match!");
            return false;
        }

        ChromeOptions CreateOptions(Models.Configs configs)
        {
            // 复制粘贴网上的几个初始化参数

            var options = new ChromeOptions();

            options.AddArgument("--window-size=1920,1080");

            if (!string.IsNullOrEmpty(configs.proxy))
            {
                var proxy = new Proxy();
                proxy.Kind = ProxyKind.Manual;
                proxy.IsAutoDetect = false;
                proxy.HttpProxy = configs.proxy;
                proxy.SslProxy = configs.proxy;
                options.Proxy = proxy;
            }

            if (configs.headless)
            {
                options.AddArgument("--headless");
            }

            if (configs.ignoreCertError)
            {
                options.AddArgument("ignore-certificate-errors");
            }
            return options;
        }
        #endregion

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    driver?.Close();
                    driver?.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~Fetcher()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region protected methods

        #endregion
    }
}