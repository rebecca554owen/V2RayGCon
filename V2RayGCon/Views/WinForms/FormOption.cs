﻿using System.Diagnostics;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormOption : Form
    {
        #region Sigleton
        static readonly VgcApis.BaseClasses.AuxSiWinForm<FormOption> auxSiForm =
            new VgcApis.BaseClasses.AuxSiWinForm<FormOption>();

        public static FormOption GetForm() => auxSiForm.GetForm();

        public static void ShowForm() => auxSiForm.ShowForm();
        #endregion

        Controllers.FormOptionCtrl optionCtrl;

        public FormOption()
        {
            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);
        }

        private void FormOption_Load(object sender, System.EventArgs e)
        {
            // throw new System.ArgumentException("for debug");

            this.optionCtrl = InitOptionCtrl();

            this.FormClosing += (s, a) =>
            {
                if (!this.optionCtrl.IsOptionsSaved())
                {
                    a.Cancel = !Misc.UI.Confirm(I18N.ConfirmCloseWinWithoutSave);
                    return;
                }
            };

            this.FormClosed += (s, a) =>
            {
                optionCtrl.Cleanup();
            };
        }

        #region public method

        #endregion

        #region private method

        private Controllers.FormOptionCtrl InitOptionCtrl()
        {
            var ctrl = new Controllers.FormOptionCtrl();

            ctrl.Plug(
                new Controllers.OptionComponent.TabCustomCoreSettings(
                    flyCoresSetting,
                    toolTip1,
                    btnCoresSettingAdd,
                    cboxCoreSettingDefaultCore,
                    btnCoreSettingChangeAll
                )
            );

            ctrl.Plug(
                new Controllers.OptionComponent.Subscription(
                    flySubsUrlContainer,
                    btnAddSubsUrl,
                    btnUpdateViaSubscription,
                    chkSubsIsUseProxy,
                    chkSubsIsAutoPatch,
                    btnSubsUseAll,
                    btnSubsInvertSelection
                )
            );

            ctrl.Plug(
                new Controllers.OptionComponent.TabPlugin(
                    btnRefreshPluginsPanel,
                    chkIsLoad3rdPartyPlugins,
                    flyPluginsItemsContainer
                )
            );

            ctrl.Plug(
                new Controllers.OptionComponent.TabSetting(
                    cboxCustomUserAgent,
                    chkIsUseCustomUserAgent,
                    tboxSystrayLeftClickCommand,
                    chkIsEnableSystrayLeftClickCommand,
                    cboxSettingLanguage,
                    cboxSettingPageSize,
                    chkSetServAutotrack,
                    tboxSettingsMaxCoreNum,
                    cboxSettingsRandomSelectServerLatency,
                    chkSetSysPortable,
                    chkSetUseV4,
                    chkSetSelfSignedCert,
                    cboxSettingsUtlsFingerprint,
                    chkSettingsEnableUtlsFingerprint,
                    chkSetServStatistics,
                    chkSetUpgradeUseProxy,
                    chkSetCheckVgcUpdateWhenStart,
                    chkSetCheckV2RayCoreUpdateWhenStart,
                    btnSetBrowseDebugFile,
                    tboxSetDebugFilePath,
                    chkSetEnableDebugFile
                )
            );

            ctrl.Plug(
                new Controllers.OptionComponent.TabDefaults(
                    // def import share link mode
                    cboxDefImportMode,
                    tboxDefImportAddr,
                    chkDefImportSsShareLink,
                    chkDefImportTrojanShareLink,
                    // speedtest
                    chkDefSpeedtestIsUse,
                    cboxDefSpeedTestUrl,
                    tboxDefSpeedtestCycles,
                    cboxDefSpeedTestExpectedSize,
                    tboxDefSpeedtestTimeout,
                    tboxDefImportVmessDecodeTemplateUrl,
                    chkDefImportIsUseVmessDecodeTemplate,
                    exRTBoxDefCustomInbounds
                )
            );

            return ctrl;
        }

        #endregion

        #region UI event
        private void btnSetOpenStartupFolder_Click(object sender, System.EventArgs e)
        {
            Process.Start(@"shell:startup");
        }

        private void btnOptionExit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void btnOptionSave_Click(object sender, System.EventArgs e)
        {
            this.optionCtrl.SaveAllOptions();
            MessageBox.Show(I18N.Done);
        }

        private void flySubsUrlContainer_Scroll(object sender, ScrollEventArgs e)
        {
            flySubsUrlContainer.Refresh();
        }

        private void flyPluginsItemsContainer_Scroll(object sender, ScrollEventArgs e)
        {
            flyPluginsItemsContainer.Refresh();
        }

        private void btnDefImportBrowseVemssDecodeTemplate_Click(object sender, System.EventArgs e)
        {
            var path = VgcApis.Misc.UI.ShowSelectFileDialog(VgcApis.Models.Consts.Files.JsonExt);
            if (!string.IsNullOrWhiteSpace(path))
            {
                tboxDefImportVmessDecodeTemplateUrl.Text = path;
            }
        }
        #endregion
    }
}
