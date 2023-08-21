using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;

namespace DyFetch.Comps
{
    internal class Plumber : IDisposable
    {
        private readonly Fetcher fetcher;
        readonly AnonymousPipeClientStream pipeIn, pipeOut;
        private bool disposedValue;

        public Plumber(string pipeIn, string pipeOut, Fetcher fetcher)
        {
            this.fetcher = fetcher;
            this.pipeIn = new AnonymousPipeClientStream(PipeDirection.In, pipeIn);
            this.pipeOut = new AnonymousPipeClientStream(PipeDirection.Out, pipeOut);
        }

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: �ͷ��й�״̬(�йܶ���)
                    pipeIn?.Dispose();
                    pipeOut?.Dispose();
                }

                // TODO: �ͷ�δ�йܵ���Դ(δ�йܵĶ���)����д�ս���
                // TODO: �������ֶ�����Ϊ null
                disposedValue = true;
            }
        }

        // // TODO: ������Dispose(bool disposing)��ӵ�������ͷ�δ�й���Դ�Ĵ���ʱ������ս���
        // ~Plumber()
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

        #region properties

        #endregion

        #region public methods
        public void Work()
        {
            var reader = new StringReader(pipeIn);
            var writer = new StringWriter(pipeOut);
            string str;
            do
            {
                str = reader.Read();
                if (str != null)
                {
                    var msg = JsonConvert.DeserializeObject<Models.Message>(str);
                    Console.WriteLine($"Fetch:{msg.url}");
                    var html = fetcher.Fetch(msg.url, msg.csses, msg.timeout);
                    Console.WriteLine($"Send:{html.Length}");
                    writer.Write(html);
                }
            } while (str != null);
            Console.WriteLine("Read end.");
        }
        #endregion

        #region private methods

        #endregion

        #region protected methods

        #endregion
    }
}