﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace V2RayGCon.Services.Caches
{
    public class CoreCache
    {
        object writeLock;
        Dictionary<string, string> data;
        Services.Settings setting;

        public CoreCache()
        {

            writeLock = new object();
        }

        #region public method
        public void Run(Services.Settings setting)
        {
            this.setting = setting;
            data = LoadDecodeCache();
        }

        public void Clear()
        {
            lock (writeLock)
            {
                data = new Dictionary<string, string>();
                SaveDecodeCache();
            }
        }

        public string this[string configString]
        {
            get
            {
                if (!data.ContainsKey(configString))
                {
                    throw new KeyNotFoundException(
                        "Core decode cache do not contain this config.");
                }

                return data[configString];
            }
            set
            {
                UpdateValue(configString, value);
            }
        }


        #endregion

        #region private method
        void UpdateValue(string configString, string decodedString)
        {
            try
            {
                JObject.Parse(decodedString);
            }
            catch
            {
                return;
            }

            lock (writeLock)
            {
                TrimDownCache();
                data[configString] = decodedString;
                SaveDecodeCache();
            }
        }

        void TrimDownCache()
        {
            var cacheSize = VgcApis.Models.Consts.Import.DecodeCacheSize;
            var count = data.Keys.Count;
            if (count < cacheSize)
            {
                return;
            }

            var keys = VgcApis.Misc.Utils.Shuffle(data.Keys).Take(count / 2).ToList();
            for (var i = 0; i < keys.Count; i++)
            {
                if (data.ContainsKey(keys[i]))
                {
                    data.Remove(keys[i]);
                }
            }
        }

        Dictionary<string, string> LoadDecodeCache()
        {
            var result = new Dictionary<string, string>();
            try
            {
                var temp = setting.decodeCache;
                foreach (var item in temp)
                {
                    try
                    {
                        JObject.Parse(item.Value);
                        result[item.Key] = item.Value;
                    }
                    catch
                    {
                        // continue
                    }
                }
            }
            catch { }
            return result;
        }

        void SaveDecodeCache()
        {
            setting.decodeCache = data;
        }
        #endregion
    }
}
