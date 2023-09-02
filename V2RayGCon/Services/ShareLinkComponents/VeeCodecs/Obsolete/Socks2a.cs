﻿using Newtonsoft.Json.Linq;
using System;

namespace V2RayGCon.Services.ShareLinkComponents.VeeCodecs.Obsolete
{
    internal sealed class Socks2a : VgcApis.BaseClasses.ComponentOf<VeeDecoder>, IVeeDecoder
    {
        Cache cache;

        public Socks2a(Cache cache)
        {
            this.cache = cache;
        }

        #region properties

        #endregion
        #region IVeeConfig
        public byte[] VeeConfig2Bytes(Models.Datas.VeeConfigsWithReality veeConfig)
        {
            var vee = new Models.VeeShareLinks.Obsolete.Socks2a();
            vee.CopyFromVeeConfig(veeConfig);
            return vee.ToBytes();
        }

        public Models.Datas.VeeConfigsWithReality Bytes2VeeConfig(byte[] bytes)
        {
            var vee = new Models.VeeShareLinks.Obsolete.Socks2a(bytes);
            return new Models.Datas.VeeConfigsWithReality(vee.ToVeeConfigs());
        }
        #endregion

        #region public methods
        public string GetSupportedVeeVersion() => Models.VeeShareLinks.Obsolete.Socks2a.version;

        public string GetSupportedEncodeProtocol() => @"";

        public byte[] Config2Bytes(JObject config)
        {
            var vee = Config2Vee(config);
            return vee?.ToBytes();
        }

        public Tuple<JObject, JToken> Bytes2Config(byte[] bytes)
        {
            var veeLink = new Models.VeeShareLinks.Obsolete.Socks2a(bytes);
            return VeeToConfig(veeLink);
        }

        #endregion

        #region private methods
        Models.VeeShareLinks.Obsolete.Socks2a Config2Vee(JObject config)
        {
            var bs = Comm.ExtractBasicConfig(
                config,
                @"socks",
                @"servers",
                out bool isUseV4,
                out string root
            );

            if (bs == null)
            {
                return null;
            }

            var GetStr = Misc.Utils.GetStringByPrefixAndKeyHelper(config);

            var socks = new Models.VeeShareLinks.Obsolete.Socks2a(bs);
            var userInfoPrefix = root + ".settings.servers.0.users.0";
            socks.userName = GetStr(userInfoPrefix, "user") ?? string.Empty;
            socks.userPassword = GetStr(userInfoPrefix, "pass") ?? string.Empty;
            return socks;
        }

        Tuple<JObject, JToken> VeeToConfig(Models.VeeShareLinks.Obsolete.Socks2a socks)
        {
            if (socks == null)
            {
                return null;
            }

            var outbSocks = cache.tpl.LoadTemplate("outbVeeSocks");
            var rsocks = new Models.VeeShareLinks.BasicSettingsWithReality(socks);
            outbSocks["streamSettings"] = Comm.GenStreamSetting(cache, rsocks);
            var node = outbSocks["settings"]["servers"][0];
            node["address"] = socks.address;
            node["port"] = socks.port;

            if (!string.IsNullOrEmpty(socks.userName) || !string.IsNullOrEmpty(socks.userPassword))
            {
                node["users"] = JToken.Parse(
                    $"[{{user: \"{socks.userName ?? string.Empty}\","
                        + $"pass:\"{socks.userPassword ?? string.Empty}\"}}]"
                );
            }

            var tpl = cache.tpl.LoadTemplate("tplImportVmess") as JObject;
            tpl["v2raygcon"]["alias"] = socks.alias;
            tpl["v2raygcon"]["description"] = socks.description;
            return new Tuple<JObject, JToken>(tpl, outbSocks);
        }

        #endregion

        #region protected methods

        #endregion
    }
}
