using System;
using System.Collections.Generic;
using System.Linq;

namespace V2RayGCon.Controllers.FormTextConfigEditorComponent
{
    internal class CompBase
        : BaseClasses.NotifyComponent,
            BaseClasses.IFormComponentController,
            IDisposable
    {
        protected FormTextConfigEditorCtrl container;

        public CompBase() { }

        #region properties

        #endregion

        #region public methods
        public void Bind(BaseClasses.FormController container)
        {
            this.container = container as FormTextConfigEditorCtrl;
        }
        #endregion

        #region IDisposable
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: �ͷ��й�״̬(�йܶ���)
                    Cleanup();
                }

                // TODO: �ͷ�δ�йܵ���Դ(δ�йܵĶ���)����д�ս���
                // TODO: �������ֶ�����Ϊ null
                disposedValue = true;
            }
        }

        // // TODO: ������Dispose(bool disposing)��ӵ�������ͷ�δ�й���Դ�Ĵ���ʱ������ս���
        // ~CompBase()
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

        #region private methods

        #endregion

        #region protected methods
        protected virtual void Cleanup() { }
        #endregion
    }
}
