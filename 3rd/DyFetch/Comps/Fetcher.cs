using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public string Fetch(string url, List<string> csses, int ms)
        {
            ms = ms > 0 ? ms : Models.Consts.MinFetchTimeout;
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(ms));
                driver.Navigate().GoToUrl(url);
                if (csses.Count > 0)
                {
                    WaitForOneOfCsses(csses, wait);
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
            var options = new ChromeOptions();
            if (configs.port > 0)
            {
                var addr = $"127.0.0.1:{configs.port}";
                var proxy = new Proxy();
                proxy.Kind = ProxyKind.Manual;
                proxy.IsAutoDetect = false;
                proxy.HttpProxy = addr;
                proxy.SslProxy = addr;
                options.Proxy = proxy;
            }

            if (configs.headless)
            {
                options.AddArgument("--headless");
            }

            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("ignore-certificate-errors");
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
                    // TODO: �ͷ��й�״̬(�йܶ���)
                    driver?.Close();
                    driver?.Dispose();
                }

                // TODO: �ͷ�δ�йܵ���Դ(δ�йܵĶ���)����д�ս���
                // TODO: �������ֶ�����Ϊ null
                disposedValue = true;
            }
        }

        // // TODO: ������Dispose(bool disposing)��ӵ�������ͷ�δ�й���Դ�Ĵ���ʱ������ս���
        // ~Fetcher()
        // {
        //     // ��Ҫ���Ĵ˴��롣�뽫���������롰Dispose(bool disposing)��������
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // ��Ҫ���Ĵ˴��롣�뽫���������롰Dispose(bool disposing)��������
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region protected methods

        #endregion
    }
}