﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Services
{
    public sealed class ShareLinkMgr
        : BaseClasses.SingletonService<ShareLinkMgr>,
            VgcApis.Interfaces.Services.IShareLinkMgrService
    {
        Settings setting;
        Servers servers;

        ShareLinkComponents.Codecs codecs;

        public ShareLinkMgr()
        {
            codecs = new ShareLinkComponents.Codecs();
        }

        #region properties

        #endregion

        #region IShareLinkMgrService methods

        /// <summary>
        /// return null if fail!
        /// </summary>
        public string DecodeShareLinkToConfig(string shareLink)
        {
            var linkType = VgcApis.Misc.Utils.DetectLinkType(shareLink);
            switch (linkType)
            {
                case VgcApis.Models.Datas.Enums.LinkTypes.v:
                    return codecs.Decode<ShareLinkComponents.VeeDecoder>(shareLink);
                case VgcApis.Models.Datas.Enums.LinkTypes.vmess:
                    return codecs.Decode<ShareLinkComponents.VmessDecoder>(shareLink);
                case VgcApis.Models.Datas.Enums.LinkTypes.v2cfg:
                    return codecs.Decode<ShareLinkComponents.V2cfgDecoder>(shareLink);
                case VgcApis.Models.Datas.Enums.LinkTypes.vless:
                    return codecs.Decode<ShareLinkComponents.VlessDecoder>(shareLink);
                case VgcApis.Models.Datas.Enums.LinkTypes.trojan:
                    return codecs.Decode<ShareLinkComponents.TrojanDecoder>(shareLink);
                default:
                    break;
            }
            return null;
        }

        /// <summary>
        /// return null if fail!
        /// </summary>
        public string EncodeConfigToShareLink(
            string config,
            VgcApis.Models.Datas.Enums.LinkTypes linkType
        )
        {
            switch (linkType)
            {
                case VgcApis.Models.Datas.Enums.LinkTypes.ss:
                    return codecs.Encode<ShareLinkComponents.SsDecoder>(config);
                case VgcApis.Models.Datas.Enums.LinkTypes.v:
                    return codecs.Encode<ShareLinkComponents.VeeDecoder>(config);
                case VgcApis.Models.Datas.Enums.LinkTypes.vmess:
                    return codecs.Encode<ShareLinkComponents.VmessDecoder>(config);
                case VgcApis.Models.Datas.Enums.LinkTypes.v2cfg:
                    return codecs.Encode<ShareLinkComponents.V2cfgDecoder>(config);
                case VgcApis.Models.Datas.Enums.LinkTypes.vless:
                    return codecs.Encode<ShareLinkComponents.VlessDecoder>(config);
                case VgcApis.Models.Datas.Enums.LinkTypes.trojan:
                    return codecs.Encode<ShareLinkComponents.TrojanDecoder>(config);
                default:
                    return null;
            }
        }
        #endregion

        #region public methods
        public T GetCodec<T>()
            where T : VgcApis.BaseClasses.ComponentOf<ShareLinkComponents.Codecs>
        {
            return codecs.GetChild<T>();
        }

        public int UpdateSubscriptions(int proxyPort)
        {
            var enabledSubs = setting.GetSubscriptionItems().Where(s => s.isUse).ToList();

            var links = Misc.Utils.FetchLinksFromSubcriptions(enabledSubs, proxyPort);
            var decoders = GenDecoderList(false);
            var results = ImportLinksBatchModeSync(links, decoders);
            return CountImportSuccessResult(results);
        }

        public int ImportLinksWithOutV2cfgLinksSync(string links, string mark)
        {
            if (string.IsNullOrEmpty(links))
            {
                return 0;
            }

            var pair = new string[] { links, mark ?? "" };
            var linkList = new List<string[]> { pair };
            var decoders = GenDecoderList(false);
            var results = ImportLinksBatchModeSync(linkList, decoders);

            // servers.UpdateAllServersSummary();

            return CountImportSuccessResult(results);
        }

        public void ImportLinkWithOutV2cfgLinksBatchModeSync(IEnumerable<string[]> linkList)
        {
            var decoders = GenDecoderList(false);
            var results = ImportLinksBatchModeSync(linkList, decoders);
            ShowImportResults(results);
        }

        public void ImportLinkWithOutV2cfgLinks(string text)
        {
            var pair = new string[] { text, "" };
            var linkList = new List<string[]> { pair };
            var decoders = GenDecoderList(false);
            ImportLinksBatchModeAsync(linkList, decoders, true);
        }

        public void ImportLinkWithV2cfgLinks(string text)
        {
            var pair = new string[] { text, "" };
            var linkList = new List<string[]> { pair };
            var decoders = GenDecoderList(true);

            ImportLinksBatchModeAsync(linkList, decoders, true);
        }

        public void Run(Settings setting, Servers servers, Cache cache)
        {
            this.setting = setting;
            this.servers = servers;

            codecs.Run(cache, setting);
        }

        #endregion

        #region private methods
        int CountImportSuccessResult(IEnumerable<string[]> result)
        {
            return result.Where(r => VgcApis.Misc.Utils.IsImportResultSuccess(r)).Count();
        }

        List<VgcApis.Interfaces.IShareLinkDecoder> GenDecoderList(bool isIncludeV2cfgDecoder)
        {
            var decoders = new List<VgcApis.Interfaces.IShareLinkDecoder>
            {
                codecs.GetChild<ShareLinkComponents.VlessDecoder>(),
                codecs.GetChild<ShareLinkComponents.VmessDecoder>(),
                codecs.GetChild<ShareLinkComponents.VeeDecoder>(),
            };

            if (setting.CustomDefImportTrojanShareLink)
            {
                decoders.Add(codecs.GetChild<ShareLinkComponents.TrojanDecoder>());
            }

            if (setting.CustomDefImportSsShareLink)
            {
                decoders.Add(codecs.GetChild<ShareLinkComponents.SsDecoder>());
            }

            if (isIncludeV2cfgDecoder)
            {
                decoders.Add(codecs.GetChild<ShareLinkComponents.V2cfgDecoder>());
            }

            return decoders;
        }

        void ImportLinksBatchModeAsync(
            IEnumerable<string[]> linkList,
            IEnumerable<VgcApis.Interfaces.IShareLinkDecoder> decoders,
            bool isShowImportResults
        )
        {
            VgcApis.Misc.Utils.RunInBgSlim(() =>
            {
                var results = ImportLinksBatchModeSync(linkList, decoders);
                if (isShowImportResults)
                {
                    ShowImportResults(results);
                }
            });
        }

        /// <summary>
        /// <para>linkList=List(string[]{0: text, 1: mark}>)</para>
        /// <para>decoders = List(IShareLinkDecoder)</para>
        /// </summary>
        IEnumerable<string[]> ImportLinksBatchModeSync(
            IEnumerable<string[]> linkList,
            IEnumerable<VgcApis.Interfaces.IShareLinkDecoder> decoders
        )
        {
            var jobs = new List<Tuple<string, string, VgcApis.Interfaces.IShareLinkDecoder>>();

            foreach (var link in linkList)
            {
                foreach (var decoder in decoders)
                {
                    jobs.Add(
                        new Tuple<string, string, VgcApis.Interfaces.IShareLinkDecoder>(
                            link[0],
                            link[1],
                            decoder
                        )
                    );
                }
            }

            List<string[]> worker(Tuple<string, string, VgcApis.Interfaces.IShareLinkDecoder> job)
            {
                return ImportShareLinks(job.Item1, job.Item2, job.Item3);
            }

            var results = Misc.Utils.ExecuteInParallel(jobs, worker);
            return results.SelectMany(r => r);
        }

        void ShowImportResults(IEnumerable<string[]> results)
        {
            var c = results.Count();

            if (c <= 0)
            {
                MessageBox.Show(I18N.NoLinkFound);
                return;
            }

            if (IsImportAnyServer(results))
            {
                servers.RequireFormMainReload();
            }

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                Views.WinForms.FormImportLinksResult.ShowResult(results);
            });
        }

        private List<string[]> ImportShareLinks(
            string text,
            string mark,
            VgcApis.Interfaces.IShareLinkDecoder decoder
        )
        {
            var links = decoder.ExtractLinksFromText(text);
            var results = links
                .AsParallel()
                .AsOrdered()
                .Select(link =>
                {
                    var formatedConfig = codecs.Decode(link, decoder);
                    var msg = AddLinkToServerList(mark, formatedConfig);
                    return GenImportResult(link, msg.Item1, msg.Item2, mark);
                })
                .ToList();
            return results;
        }

        bool IsImportAnyServer(IEnumerable<string[]> importResults)
        {
            return importResults.Any(r => VgcApis.Misc.Utils.IsImportResultSuccess(r));
        }

        private Tuple<bool, string> AddLinkToServerList(string mark, string formatedConfig)
        {
            if (string.IsNullOrEmpty(formatedConfig))
            {
                return new Tuple<bool, string>(false, I18N.DecodeFail);
            }
            var ok = servers.AddServerWithFormatedConfig(formatedConfig, mark, true);
            var reason = ok ? I18N.Success : I18N.DuplicateServer;
            return new Tuple<bool, string>(ok, reason);
        }

        string[] GenImportResult(string link, bool success, string reason, string mark)
        {
            var importSuccessMark = success
                ? VgcApis.Models.Consts.Import.MarkImportSuccess
                : VgcApis.Models.Consts.Import.MarkImportFail;

            return new string[]
            {
                string.Empty, // reserved for index
                link,
                mark,
                importSuccessMark, // be aware of IsImportResultSuccess()
                reason,
            };
        }
        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            VgcApis.Libs.Sys.FileLogger.Info("ShareLinkMgr.Cleanup() begin");
            codecs?.Dispose();
            VgcApis.Libs.Sys.FileLogger.Info("ShareLinkMgr.Cleanup() done");
        }
        #endregion
    }
}
