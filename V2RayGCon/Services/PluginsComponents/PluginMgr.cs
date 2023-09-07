using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using V2RayGCon.Libs.Lua;

namespace V2RayGCon.Services.PluginsComponents
{
    internal class PluginMgr : IDisposable
    {
        private readonly Settings settings;
        private readonly Apis apis;

        readonly object locker = new object();
        Dictionary<string, VgcApis.Interfaces.IPlugin> cache =
            new Dictionary<string, VgcApis.Interfaces.IPlugin>();
        List<string> internalPluginNames = new List<string>();

        public PluginMgr(Settings settings, Libs.Lua.Apis apis)
        {
            this.settings = settings;
            this.apis = apis;
        }

        #region properties

        #endregion

        #region public methods
        public List<Models.Datas.PluginInfoItem> GatherAllPluginInfos()
        {
            var enabled = GetEnabledPluginFileNames();

            var r = new List<Models.Datas.PluginInfoItem>();
            foreach (var name in internalPluginNames)
            {
                var info = ToPluginInfoItem(cache[name], name, enabled.Contains(name));
                r.Add(info);
            }

            var extPlugins = settings.isLoad3rdPartyPlugins
                ? LoadAllPluginFromDir()
                : new Dictionary<string, VgcApis.Interfaces.IPlugin>();

            foreach (var kv in extPlugins)
            {
                var name = kv.Key;
                var p = kv.Value;
                var info = ToPluginInfoItem(p, name, enabled.Contains(name));
                r.Add(info);
            }

            return r;
        }

        public List<ToolStripMenuItem> GetEnabledPluginMenus()
        {
            var menu = new List<ToolStripMenuItem>();
            var enabledList = GetEnabledPluginFileNames();
            foreach (var fileName in enabledList)
            {
                if (cache.TryGetValue(fileName, out var plugin) && plugin != null)
                {
                    var mi = plugin.GetToolStripMenu();
                    mi.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                    mi.ToolTipText = plugin.Description;
                    menu.Add(mi);
                }
            }
            return menu;
        }

        public void RestartAllPlugins()
        {
            var enabledList = GetEnabledPluginFileNames();
            var disabled = cache.Keys.Where(k => !enabledList.Contains(k)).ToList();

            foreach (var filename in disabled)
            {
                if (cache.TryGetValue(filename, out var plugin) && plugin != null)
                {
                    plugin.Stop();
                }
            }

            LoadPlugins(enabledList);

            foreach (var filename in enabledList)
            {
                if (cache.TryGetValue(filename, out var plugin) && plugin != null)
                {
                    plugin.Run(apis);
                }
            }
        }

        public ReadOnlyCollection<VgcApis.Interfaces.IPlugin> GetAllEnabledPlugins()
        {
            var result = new List<VgcApis.Interfaces.IPlugin>();
            var enabledList = GetEnabledPluginFileNames();
            lock (locker)
            {
                foreach (var filename in enabledList)
                {
                    var p = GetPlugin(filename);
                    if (p != null)
                    {
                        result.Add(p);
                    }
                }
            }
            return result.AsReadOnly();
        }

        public void Init()
        {
            var ps = new List<VgcApis.Interfaces.IPlugin>()
            {
                new NeoLuna.NeoLuna(),
                new Pacman.Pacman(),
                new ProxySetter.ProxySetter(),
            };

            internalPluginNames = ps.Select(p => p.Name).ToList();
            cache = ps.ToDictionary(p => p.Name);

            if (settings.isLoad3rdPartyPlugins)
            {
                var filenames = GetEnabledPluginFileNames()
                    .Where(fn => !internalPluginNames.Contains(fn))
                    .ToList();

                LoadPlugins(filenames);
            }
        }
        #endregion

        #region private methods
        void LoadPlugins(IEnumerable<string> filenames)
        {
            lock (locker)
            {
                foreach (var filename in filenames)
                {
                    GetPlugin(filename);
                }
            }
        }

        string CompatibleWithLuna(string filename)
        {
            if (filename == "Luna")
            {
                // ��Ҫ����libsĿ¼��Luna.dll���Ǹ��汾������
                var file = Path.Combine(VgcApis.Models.Consts.Files.PluginsDir, "Luna.dll");
                if (File.Exists(file))
                {
                    return file;
                }
            }
            return filename;
        }

        VgcApis.Interfaces.IPlugin GetPlugin(string filename)
        {
            if (cache.TryGetValue(filename, out var plugin) && plugin != null)
            {
                return plugin;
            }

            var dll = CompatibleWithLuna(filename);
            var p = LoadPluginFromFile(dll);
            if (p != null)
            {
                cache.Add(filename, p);
            }
            return p;
        }

        VgcApis.Interfaces.IPlugin LoadPluginFromFile(string dllFile)
        {
            if (!File.Exists(dllFile))
            {
                return null;
            }

            var iName = nameof(VgcApis.Interfaces.IPlugin);
            try
            {
                // ��Ҫ��File.ReadAllBytes()Ȼ��Assembly.Load(bytes)
                // ��μ�����ͬ���ʱ���������ƺ������"_2"
                var assembly = Assembly.LoadFrom(dllFile);
                foreach (var ty in assembly.GetExportedTypes())
                {
                    if (ty.GetInterface(iName, false) != null)
                    {
                        return Activator.CreateInstance(ty) as VgcApis.Interfaces.IPlugin;
                    }
                }
            }
            catch { }
            return null;
        }

        Dictionary<string, VgcApis.Interfaces.IPlugin> LoadAllPluginFromDir()
        {
            var dir = VgcApis.Models.Consts.Files.PluginsDir;
            var r = new Dictionary<string, VgcApis.Interfaces.IPlugin>();
            try
            {
                foreach (
                    string file in Directory.GetFiles(dir, @"*.dll", SearchOption.AllDirectories)
                )
                {
                    var p = LoadPluginFromFile(file);
                    if (p != null)
                    {
                        r.Add(file, p);
                    }
                }
            }
            catch { }
            return r;
        }

        List<string> GetEnabledPluginFileNames()
        {
            return settings
                .GetPluginInfoItems()
                .Where(pi => pi.isUse)
                .Select(pi => pi.filename)
                .ToList();
        }

        void RemovePlugins(List<string> filenames)
        {
            List<VgcApis.Interfaces.IPlugin> ps = null;

            lock (locker)
            {
                ps = cache.Where(kv => filenames.Contains(kv.Key)).Select(kv => kv.Value).ToList();

                foreach (var name in filenames)
                {
                    cache.Remove(name);
                }
            }

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                foreach (var p in ps)
                {
                    p.Dispose();
                }
            });
        }

        Models.Datas.PluginInfoItem ToPluginInfoItem(
            VgcApis.Interfaces.IPlugin plugin,
            string filename,
            bool isEanbled
        )
        {
            if (plugin == null)
            {
                return null;
            }

            return new Models.Datas.PluginInfoItem()
            {
                name = plugin.Name,
                filename = filename,
                isUse = isEanbled,
                version = plugin.Version,
                description = plugin.Description,
            };
        }

        void Cleanup()
        {
            foreach (var kv in cache)
            {
                kv.Value?.Dispose();
            }
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
        // ~PluginCache()
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
