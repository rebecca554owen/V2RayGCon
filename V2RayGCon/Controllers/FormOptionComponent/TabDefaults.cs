﻿using System;
using System.Windows.Forms;

namespace V2RayGCon.Controllers.OptionComponent
{
    class TabDefaults : OptionComponentController
    {
        readonly Services.Settings setting;
        readonly ComboBox cboxDefImportMode = null,
            cboxDefSpeedtestUrl = null,
            cboxDefSpeedtestExpectedSize = null;
        readonly CheckBox chkSetSpeedtestIsUse = null,
            chkImportSsShareLink = null,
            chkImportTrojanShareLink = null,
            chkDefVmessDecodeTemplateEnabled = null;
        readonly TextBox tboxDefImportAddr = null,
            tboxSetSpeedtestCycles = null,
            tboxSetSpeedtestTimeout = null,
            tboxDefVmessDecodeTemplateUrl = null;
        readonly RichTextBox exRTBoxDefCustomInbounds = null;

        public TabDefaults(
            ComboBox cboxDefImportMode,
            TextBox tboxDefImportAddr,
            CheckBox chkImportSsShareLink,
            CheckBox chkImportTrojanShareLink,
            CheckBox chkSetSpeedtestIsUse,
            ComboBox cboxDefSpeedtestUrl,
            TextBox tboxSetSpeedtestCycles,
            ComboBox cboxDefSpeedtestExpectedSize,
            TextBox tboxSetSpeedtestTimeout,
            TextBox tboxDefVmessDecodeTemplateUrl,
            CheckBox chkDefVmessDecodeTemplateEnabled,
            RichTextBox exRTBoxDefCustomInbounds
        )
        {
            this.setting = Services.Settings.Instance;

            this.tboxDefVmessDecodeTemplateUrl = tboxDefVmessDecodeTemplateUrl;
            this.chkDefVmessDecodeTemplateEnabled = chkDefVmessDecodeTemplateEnabled;

            this.exRTBoxDefCustomInbounds = exRTBoxDefCustomInbounds;

            // Do not put these lines of code into InitElement.
            this.cboxDefImportMode = cboxDefImportMode;
            this.tboxDefImportAddr = tboxDefImportAddr;
            this.chkImportSsShareLink = chkImportSsShareLink;
            this.chkImportTrojanShareLink = chkImportTrojanShareLink;
            this.chkSetSpeedtestIsUse = chkSetSpeedtestIsUse;
            this.cboxDefSpeedtestUrl = cboxDefSpeedtestUrl;
            this.tboxSetSpeedtestCycles = tboxSetSpeedtestCycles;
            this.cboxDefSpeedtestExpectedSize = cboxDefSpeedtestExpectedSize;
            this.tboxSetSpeedtestTimeout = tboxSetSpeedtestTimeout;

            InitElement();
        }

        private void InitElement()
        {
            tboxDefVmessDecodeTemplateUrl.Text = setting.CustomVmessDecodeTemplateUrl;
            chkDefVmessDecodeTemplateEnabled.Checked = setting.CustomVmessDecodeTemplateEnabled;
            exRTBoxDefCustomInbounds.Text = setting.CustomDefInbounds;

            chkImportSsShareLink.Checked = setting.CustomDefImportSsShareLink;
            chkImportTrojanShareLink.Checked = setting.CustomDefImportTrojanShareLink;

            cboxDefImportMode.SelectedIndex = setting.CustomDefImportMode;
            tboxDefImportAddr.TextChanged += OnTboxImportAddrTextChanged;
            tboxDefImportAddr.Text = string.Format(
                @"{0}:{1}",
                setting.CustomDefImportIp,
                setting.CustomDefImportPort
            );

            // speedtest
            chkSetSpeedtestIsUse.Checked = setting.isUseCustomSpeedtestSettings;
            tboxSetSpeedtestCycles.Text = setting.CustomSpeedtestCycles.ToString();
            cboxDefSpeedtestUrl.Text = setting.CustomSpeedtestUrl;
            cboxDefSpeedtestExpectedSize.Text = setting.CustomSpeedtestExpectedSizeInKib.ToString();
            tboxSetSpeedtestTimeout.Text = setting.CustomSpeedtestTimeout.ToString();
        }

        #region public method
        public override bool SaveOptions()
        {
            if (!IsOptionsChanged())
            {
                return false;
            }

            setting.CustomVmessDecodeTemplateEnabled = chkDefVmessDecodeTemplateEnabled.Checked;
            setting.CustomVmessDecodeTemplateUrl = tboxDefVmessDecodeTemplateUrl.Text;

            setting.CustomDefInbounds = exRTBoxDefCustomInbounds.Text;

            // mode
            if (
                VgcApis.Misc.Utils.TryParseAddress(
                    tboxDefImportAddr.Text,
                    out string ip,
                    out int port
                )
            )
            {
                setting.CustomDefImportIp = ip;
                setting.CustomDefImportPort = port;
            }
            setting.CustomDefImportMode = cboxDefImportMode.SelectedIndex;

            setting.CustomDefImportSsShareLink = chkImportSsShareLink.Checked;
            setting.CustomDefImportTrojanShareLink = chkImportTrojanShareLink.Checked;

            // speedtest
            setting.isUseCustomSpeedtestSettings = chkSetSpeedtestIsUse.Checked;
            setting.CustomSpeedtestUrl = cboxDefSpeedtestUrl.Text;
            setting.CustomSpeedtestCycles = VgcApis.Misc.Utils.Str2Int(tboxSetSpeedtestCycles.Text);
            setting.CustomSpeedtestExpectedSizeInKib = VgcApis.Misc.Utils.Str2Int(
                cboxDefSpeedtestExpectedSize.Text
            );
            setting.CustomSpeedtestTimeout = VgcApis.Misc.Utils.Str2Int(
                tboxSetSpeedtestTimeout.Text
            );

            setting.SaveUserSettingsNow();
            return true;
        }

        public override bool IsOptionsChanged()
        {
            var success = VgcApis.Misc.Utils.TryParseAddress(
                tboxDefImportAddr.Text,
                out string ip,
                out int port
            );
            if (
                !success
                || setting.CustomVmessDecodeTemplateUrl != tboxDefVmessDecodeTemplateUrl.Text
                || setting.CustomVmessDecodeTemplateEnabled
                    != chkDefVmessDecodeTemplateEnabled.Checked
                || setting.CustomDefInbounds != exRTBoxDefCustomInbounds.Text
                || setting.CustomDefImportSsShareLink != chkImportSsShareLink.Checked
                || setting.CustomDefImportTrojanShareLink != chkImportTrojanShareLink.Checked
                || setting.CustomDefImportIp != ip
                || setting.CustomDefImportPort != port
                || setting.CustomDefImportMode != cboxDefImportMode.SelectedIndex
                || setting.isUseCustomSpeedtestSettings != chkSetSpeedtestIsUse.Checked
                || setting.CustomSpeedtestUrl != cboxDefSpeedtestUrl.Text
                || setting.CustomSpeedtestExpectedSizeInKib
                    != VgcApis.Misc.Utils.Str2Int(cboxDefSpeedtestExpectedSize.Text)
                || setting.CustomSpeedtestCycles
                    != VgcApis.Misc.Utils.Str2Int(tboxSetSpeedtestCycles.Text)
                || setting.CustomSpeedtestTimeout
                    != VgcApis.Misc.Utils.Str2Int(tboxSetSpeedtestTimeout.Text)
            )
            {
                return true;
            }
            return false;
        }
        #endregion

        #region private method
        void OnTboxImportAddrTextChanged(object sender, EventArgs e) =>
            VgcApis.Misc.UI.MarkInvalidAddressWithColorRed(tboxDefImportAddr);
        #endregion
    }
}
