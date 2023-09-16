﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Services
{
    public sealed class ConfigMgr
        : BaseClasses.SingletonService<ConfigMgr>,
            VgcApis.Interfaces.Services.IConfigMgrService
    {
        Settings setting;
        Cache cache;

        static readonly long TIMEOUT = VgcApis.Models.Consts.Core.SpeedtestTimeout;

        ConfigMgr() { }

        #region public methods
        public string FetchWithCustomConfig(string rawConfig, string title, string url, int timeout)
        {
            var text = string.Empty;
            var port = VgcApis.Misc.Utils.GetFreeTcpPort();
            if (port < 0)
            {
                return text;
            }
            try
            {
                var config = CreateSpeedTestConfig(rawConfig, port, false, false, false);
                var envs = Misc.Utils.GetEnvVarsFromConfig(JObject.Parse(config));
                var core = new Libs.V2Ray.Core(setting) { title = title };
                core.RestartCoreIgnoreError(config, envs);
                if (WaitUntilCoreReady(core))
                {
                    text = Misc.Utils.Fetch(url, port, timeout);
                }
                core.StopCore();
            }
            catch
            {
                return string.Empty;
            }
            return text;
        }

        public long RunCustomSpeedTest(string rawConfig, string testUrl, int testTimeout) =>
            QueuedSpeedTesting(
                rawConfig,
                "Custom speed-test",
                testUrl,
                testTimeout,
                false,
                false,
                false,
                null
            ).Item1;

        public long RunSpeedTest(string rawConfig)
        {
            var url = GetDefaultSpeedtestUrl();
            return QueuedSpeedTesting(
                rawConfig,
                "Default speed-test",
                "",
                GetDefaultTimeout(),
                false,
                false,
                false,
                null
            ).Item1;
        }

        public Tuple<long, long> RunDefaultSpeedTest(
            string rawConfig,
            string title,
            EventHandler<VgcApis.Models.Datas.StrEvent> logDeliever
        )
        {
            var url = GetDefaultSpeedtestUrl();
            return QueuedSpeedTesting(
                rawConfig,
                title,
                url,
                GetDefaultTimeout(),
                true,
                true,
                false,
                logDeliever
            );
        }

        public string InjectImportTpls(
            string config,
            bool isIncludeSpeedTest,
            bool isIncludeActivate
        )
        {
            JObject import = Misc.Utils.ImportItemList2JObject(
                setting.GetGlobalImportItems(),
                isIncludeSpeedTest,
                isIncludeActivate,
                false
            );

            Misc.Utils.MergeJson(import, JObject.Parse(config));
            return import.ToString();
        }

        public JObject DecodeConfig(
            string rawConfig,
            bool isUseCache,
            bool isInjectSpeedTestTpl,
            bool isInjectActivateTpl
        )
        {
            var coreConfig = rawConfig;
            JObject decodedConfig = null;

            try
            {
                string injectedConfig = coreConfig;
                if (isInjectActivateTpl || isInjectSpeedTestTpl)
                {
                    injectedConfig = InjectImportTpls(
                        rawConfig,
                        isInjectSpeedTestTpl,
                        isInjectActivateTpl
                    );
                }

                decodedConfig = ParseImport(injectedConfig);

                MergeCustomTlsSettings(ref decodedConfig);

                cache.core[coreConfig] = decodedConfig.ToString(Formatting.None);
            }
            catch { }

            if (decodedConfig == null)
            {
                setting.SendLog(I18N.DecodeImportFail);
                if (isUseCache)
                {
                    try
                    {
                        decodedConfig = JObject.Parse(cache.core[coreConfig]);
                    }
                    catch (KeyNotFoundException) { }
                    setting.SendLog(I18N.UsingDecodeCache);
                }
            }

            return decodedConfig;
        }

        public bool ModifyInboundWithCustomSetting(
            ref JObject config,
            int inbType,
            string ip,
            int port
        )
        {
            if (inbType == (int)Models.Datas.Enums.ProxyTypes.Config)
            {
                return true;
            }

            if (inbType == (int)Models.Datas.Enums.ProxyTypes.Custom)
            {
                try
                {
                    var inbs = JArray.Parse(setting.CustomDefInbounds);
                    config["inbounds"] = inbs;
                    return true;
                }
                catch
                {
                    setting.SendLog(I18N.ParseCustomInboundsSettingFail);
                }
                return false;
            }

            if (
                inbType != (int)Models.Datas.Enums.ProxyTypes.HTTP
                && inbType != (int)Models.Datas.Enums.ProxyTypes.SOCKS
            )
            {
                return false;
            }

            var protocol = Misc.Utils.InboundTypeNumberToName(inbType);
            var tplKey = protocol + "In";
            try
            {
                JObject o = CreateInboundSetting(inbType, ip, port, protocol, tplKey);

                ReplaceInboundSetting(ref config, o);
#if DEBUG
                var debug = config.ToString(Formatting.Indented);
#endif
                return true;
            }
            catch
            {
                setting.SendLog(I18N.CoreCantSetLocalAddr);
            }
            return false;
        }

        /// <summary>
        /// ref means config will change after the function is executed.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public void InjectSkipCnSiteSettingsIntoConfig(ref JObject config, bool useV4)
        {
            var c = JObject.Parse(@"{}");

            var dict = new Dictionary<string, string>
            {
                { "dns", "dnsCFnGoogle" },
                { "routing", GetRoutingTplName(config, useV4) },
            };

            foreach (var item in dict)
            {
                var tpl = Misc.Utils.CreateJObject(item.Key);
                var value = cache.tpl.LoadExample(item.Value);
                tpl[item.Key] = value;

                if (!Misc.Utils.Contains(config, tpl))
                {
                    c[item.Key] = value;
                }
            }

            // put dns/routing settings in front of user settings
            Misc.Utils.CombineConfigWithRoutingInFront(ref config, c);

            // put outbounds after user settings
            var hasOutbounds = Misc.Utils.GetKey(config, "outbounds") != null;
            var hasOutDtr = Misc.Utils.GetKey(config, "outboundDetour") != null;

            var outboundTag = "outboundDetour";
            if (!hasOutDtr && (hasOutbounds || useV4))
            {
                outboundTag = "outbounds";
            }

            var o = Misc.Utils.CreateJObject(outboundTag, cache.tpl.LoadExample("outDtrFreedom"));

            if (!Misc.Utils.Contains(config, o))
            {
                Misc.Utils.CombineConfigWithRoutingInFront(ref o, config);
                config = o;
            }
        }

        /*
         * exceptions
         * test<FormatException> base64 decode fail
         * test<System.Net.WebException> url not exist
         * test<Newtonsoft.Json.JsonReaderException> json decode fail
         */
        public JObject ParseImport(string configString)
        {
            var maxDepth = VgcApis.Models.Consts.Import.ParseImportDepth;

            var result = Misc.Utils.ParseImportRecursively(
                GetHtmlContentFromCache,
                JObject.Parse(configString),
                maxDepth
            );

            try
            {
                Misc.Utils.RemoveKeyFromJObject(result, "v2raygcon.import");
            }
            catch (KeyNotFoundException)
            {
                // do nothing;
            }

            return result;
        }

        public JObject GenV4ServersPackageConfig(
            List<VgcApis.Interfaces.ICoreServCtrl> servList,
            string packageName,
            VgcApis.Models.Datas.Enums.PackageTypes packageType
        )
        {
            JObject package;
            switch (packageType)
            {
                case VgcApis.Models.Datas.Enums.PackageTypes.Chain:
                    package = GenV4ChainConfig(servList, packageName);
                    break;
                case VgcApis.Models.Datas.Enums.PackageTypes.Balancer:
                default:
                    package = GenV4BalancerConfig(servList, packageName);
                    break;
            }

            try
            {
                var finalConfig = GetGlobalImportConfigForPacking();
                Misc.Utils.CombineConfigWithRoutingInTheEnd(ref finalConfig, package);
                return finalConfig;
            }
            catch
            {
                setting.SendLog(I18N.InjectPackagingImportsFail);
                return package;
            }
        }

        public void Run(Settings setting, Cache cache)
        {
            this.setting = setting;
            this.cache = cache;
        }

        #endregion

        #region private methods
        JObject GenV4ChainConfig(
            List<VgcApis.Interfaces.ICoreServCtrl> servList,
            string packageName
        )
        {
            var package = cache.tpl.LoadPackage("chainV4Tpl");
            var outbounds = package["outbounds"] as JArray;
            var description = new List<string>();

            JObject prev = null;
            for (var i = 0; i < servList.Count; i++)
            {
                var s = servList[i];
                var parts = Misc.Utils.ExtractOutboundsFromConfig(s.GetConfiger().GetFinalConfig());
                var c = 0;
                foreach (JObject p in parts)
                {
                    var tag = $"node{i}s{c++}";
                    p["tag"] = tag;
                    if (prev != null)
                    {
                        prev["proxySettings"] = JObject.Parse(@"{tag: '',transportLayer: true}");
                        prev["proxySettings"]["tag"] = tag;
                        outbounds.Add(prev);
                    }
                    prev = p;
                }
                var name = s.GetCoreStates().GetName();
                if (c == 0)
                {
                    setting.SendLog(I18N.PackageFail + ": " + name);
                }
                else
                {
                    description.Add($"{i}.[{name}]");
                    setting.SendLog(I18N.PackageSuccess + ": " + name);
                }
            }
            outbounds.Add(prev);

            package["v2raygcon"]["alias"] = string.IsNullOrEmpty(packageName)
                ? "ChainV4"
                : packageName;
            package["v2raygcon"]["description"] =
                $"[Total: {description.Count()}] " + string.Join(" ", description);

            return package;
        }

        private JObject GenV4BalancerConfig(
            List<VgcApis.Interfaces.ICoreServCtrl> servList,
            string packageName
        )
        {
            var package = cache.tpl.LoadPackage("pkgV4Tpl");
            var outbounds = package["outbounds"] as JArray;
            var description = new List<string>();

            for (var i = 0; i < servList.Count; i++)
            {
                var s = servList[i];
                var parts = Misc.Utils.ExtractOutboundsFromConfig(s.GetConfiger().GetFinalConfig());
                var c = 0;
                foreach (JObject p in parts)
                {
                    p["tag"] = $"agentout{i}s{c++}";
                    outbounds.Add(p);
                }
                var name = s.GetCoreStates().GetName();
                if (c == 0)
                {
                    setting.SendLog(I18N.PackageFail + ": " + name);
                }
                else
                {
                    description.Add($"{i}.[{name}]");
                    setting.SendLog(I18N.PackageSuccess + ": " + name);
                }
            }

            package["v2raygcon"]["alias"] = string.IsNullOrEmpty(packageName)
                ? "PackageV4"
                : packageName;
            package["v2raygcon"]["description"] =
                $"[Total: {description.Count()}] " + string.Join(" ", description);
            return package;
        }

        void MergeCustomTlsSettings(ref JObject config)
        {
            var outB =
                Misc.Utils.GetKey(config, "outbound") ?? Misc.Utils.GetKey(config, "outbounds.0");

            if (outB == null)
            {
                return;
            }

            if (!(Misc.Utils.GetKey(outB, "streamSettings") is JObject streamSettings))
            {
                return;
            }

            if (setting.isSupportSelfSignedCert)
            {
                var selfSigned = JObject.Parse(@"{tlsSettings: {allowInsecure: true}}");
                Misc.Utils.MergeJson(streamSettings, selfSigned);
            }

            if (setting.isEnableUtlsFingerprint)
            {
                var uTlsFingerprint = JObject.Parse(@"{tlsSettings: {}}");
                uTlsFingerprint["tlsSettings"]["fingerprint"] = setting.uTlsFingerprint;
                Misc.Utils.MergeJson(streamSettings, uTlsFingerprint);
            }
        }

        int GetDefaultTimeout()
        {
            var customTimeout = setting.CustomSpeedtestTimeout;
            if (customTimeout > 0)
            {
                return customTimeout;
            }
            return VgcApis.Models.Consts.Intervals.DefaultSpeedTestTimeout;
        }

        string GetDefaultSpeedtestUrl() =>
            setting.isUseCustomSpeedtestSettings
                ? setting.CustomSpeedtestUrl
                : VgcApis.Models.Consts.Webs.GoogleDotCom;

        JObject GetGlobalImportConfigForPacking()
        {
            var imports = Misc.Utils.ImportItemList2JObject(
                setting.GetGlobalImportItems(),
                false,
                false,
                true
            );
            return ParseImport(imports.ToString());
        }

        Tuple<long, long> QueuedSpeedTesting(
            string rawConfig,
            string title,
            string testUrl,
            int testTimeout,
            bool isUseCache,
            bool isInjectSpeedTestTpl,
            bool isInjectActivateTpl,
            EventHandler<VgcApis.Models.Datas.StrEvent> logDeliever
        )
        {
            Interlocked.Increment(ref setting.SpeedtestCounter);

            // setting.SpeedTestPool may change while testing
            var pool = setting.SpeedTestPool;
            pool.Wait();

            var result = new Tuple<long, long>(VgcApis.Models.Consts.Core.SpeedtestAbort, 0);
            if (!setting.isSpeedtestCancelled)
            {
                var port = VgcApis.Misc.Utils.GetFreeTcpPort();
                var cfg = CreateSpeedTestConfig(
                    rawConfig,
                    port,
                    isUseCache,
                    isInjectSpeedTestTpl,
                    isInjectActivateTpl
                );
                result = DoSpeedTesting(title, testUrl, testTimeout, port, cfg, logDeliever);
            }

            pool.Release();
            Interlocked.Decrement(ref setting.SpeedtestCounter);
            return result;
        }

        bool WaitUntilCoreReady(Libs.V2Ray.Core core)
        {
            const int jiff = 300;
            int cycle = 30 * 1000 / jiff;
            int i;
            for (i = 0; i < cycle && !core.isReady && core.isRunning; i++)
            {
                VgcApis.Misc.Utils.Sleep(jiff);
            }

            if (!core.isRunning)
            {
                return false;
            }

            if (i < cycle)
            {
                return true;
            }
            return false;
        }

        Tuple<long, long> DoSpeedTesting(
            string title,
            string testUrl,
            int testTimeout,
            int port,
            string config,
            EventHandler<VgcApis.Models.Datas.StrEvent> logDeliever
        )
        {
            void log(string content) =>
                logDeliever?.Invoke(this, new VgcApis.Models.Datas.StrEvent(content));

            log($"{I18N.SpeedtestPortNum}{port}");
            if (string.IsNullOrEmpty(config))
            {
                log(I18N.DecodeImportFail);
                return new Tuple<long, long>(TIMEOUT, 0);
            }

            var speedTester = new Libs.V2Ray.Core(setting) { title = title };
            if (logDeliever != null)
            {
                speedTester.OnLog += logDeliever;
            }

            long latency = TIMEOUT;
            long len = 0;
            try
            {
                var envs = Misc.Utils.GetEnvVarsFromConfig(JObject.Parse(config));
                speedTester.RestartCoreIgnoreError(config, envs);
                if (WaitUntilCoreReady(speedTester))
                {
                    var expectedSizeInKib = setting.isUseCustomSpeedtestSettings
                        ? setting.CustomSpeedtestExpectedSizeInKib
                        : -1;
                    var r = VgcApis.Misc.Utils.TimedDownloadTest(
                        testUrl,
                        port,
                        expectedSizeInKib,
                        testTimeout
                    );
                    latency = r.Item1;
                    len = r.Item2;
                }
                speedTester.StopCore();
            }
            catch { }
            if (logDeliever != null)
            {
                speedTester.OnLog -= logDeliever;
            }
            return new Tuple<long, long>(latency, len);
        }

        List<string> GetHtmlContentFromCache(IEnumerable<string> urls)
        {
            if (urls == null || urls.Count() <= 0)
            {
                return new List<string>();
            }
            return Misc.Utils.ExecuteInParallel(urls, (url) => cache.html[url]);
        }

        JObject CreateInboundSetting(
            int inboundType,
            string ip,
            int port,
            string protocol,
            string tplKey
        )
        {
            var o = JObject.Parse(@"{}");
            o["tag"] = "agentin";
            o["protocol"] = protocol;
            o["listen"] = ip;
            o["port"] = port;
            o["settings"] = cache.tpl.LoadTemplate(tplKey);

            if (inboundType == (int)Models.Datas.Enums.ProxyTypes.SOCKS)
            {
                o["settings"]["ip"] = ip;
            }

            return o;
        }

        string CreateSpeedTestConfig(
            string rawConfig,
            int port,
            bool isUseCache,
            bool isInjectSpeedTestTpl,
            bool isInjectActivateTpl
        )
        {
            var empty = string.Empty;
            if (port <= 0)
            {
                return empty;
            }

            var config = DecodeConfig(
                rawConfig,
                isUseCache,
                isInjectSpeedTestTpl,
                isInjectActivateTpl
            );

            if (config == null)
            {
                return empty;
            }

            // inject log config
            var nodeLog = Misc.Utils.GetKey(config, "log");
            if (nodeLog != null && nodeLog is JObject)
            {
                nodeLog["loglevel"] = "warning";
            }
            else
            {
                config["log"] = JToken.Parse("{'loglevel': 'warning'}");
            }

            if (
                !ModifyInboundWithCustomSetting(
                    ref config,
                    (int)Models.Datas.Enums.ProxyTypes.HTTP,
                    VgcApis.Models.Consts.Webs.LoopBackIP,
                    port
                )
            )
            {
                return empty;
            }

            // debug
            var configString = config.ToString(Formatting.None);

            return configString;
        }

        string GetRoutingTplName(JObject config, bool useV4)
        {
            var routingRules = Misc.Utils.GetKey(config, "routing.rules");
            var routingSettingsRules = Misc.Utils.GetKey(config, "routing.settings.rules");
            var hasRoutingV4 = routingRules != null && (routingRules is JArray);
            var hasRoutingV3 = routingSettingsRules != null && (routingSettingsRules is JArray);

            var isUseRoutingV4 = !hasRoutingV3 && (useV4 || hasRoutingV4);
            return isUseRoutingV4 ? "routeCnipV4" : "routeCNIP";
        }

        void ReplaceInboundSetting(ref JObject config, JObject o)
        {
            // Bug. Stream setting will mess things up.
            // Misc.Utils.MergeJson(ref config, o);

            var hasInbound = Misc.Utils.GetKey(config, "inbound") != null;
            var hasInbounds = Misc.Utils.GetKey(config, "inbounds.0") != null;
            var isUseV4 = !hasInbound && (hasInbounds || setting.isUseV4);

            if (isUseV4)
            {
                if (!hasInbounds)
                {
                    config["inbounds"] = JArray.Parse(@"[{}]");
                }
                config["inbounds"][0] = o;
            }
            else
            {
                config["inbound"] = o;
            }
        }

        #endregion
    }
}
