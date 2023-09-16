﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace V2RayGCon.Controllers.OptionComponent
{
    class TabMultiConf : OptionComponentController
    {
        readonly FlowLayoutPanel flyPanel;
        readonly Button btnAdd;
        readonly Services.Settings setting;
        string oldOptions;

        public TabMultiConf(FlowLayoutPanel flyPanel, Button btnAdd)
        {
            this.setting = Services.Settings.Instance;

            this.flyPanel = flyPanel;
            this.btnAdd = btnAdd;

            InitPanel();
            BindEvent();
        }

        #region public method
        public override bool SaveOptions()
        {
            string curOptions = GetCurOptions();

            if (curOptions != oldOptions)
            {
                setting.SaveMultiConfItems(curOptions);
                oldOptions = curOptions;
                return true;
            }
            return false;
        }

        public override bool IsOptionsChanged()
        {
            return GetCurOptions() != oldOptions;
        }

        public void Reload(string rawSetting)
        {
            setting.SaveMultiConfItems(rawSetting);
            Misc.UI.ClearFlowLayoutPanel(this.flyPanel);
            InitPanel();
        }
        #endregion

        #region private method
        string GetCurOptions()
        {
            return JsonConvert.SerializeObject(CollectMultiConfItems());
        }

        List<Models.Datas.MultiConfItem> CollectMultiConfItems()
        {
            var itemList = new List<Models.Datas.MultiConfItem>();
            foreach (Views.UserControls.MultiConfUI item in this.flyPanel.Controls)
            {
                var v = item.GetValue();
                if (!string.IsNullOrEmpty(v.alias) || !string.IsNullOrEmpty(v.path))
                {
                    itemList.Add(v);
                }
            }
            return itemList;
        }

        void InitPanel()
        {
            var importUrlItemList = setting.GetMultiConfItems();

            this.oldOptions = JsonConvert.SerializeObject(importUrlItemList);

            if (importUrlItemList.Count <= 0)
            {
                importUrlItemList.Add(new Models.Datas.MultiConfItem());
            }

            foreach (var item in importUrlItemList)
            {
                this.flyPanel.Controls.Add(
                    new Views.UserControls.MultiConfUI(item, UpdatePanelItemsIndex)
                );
            }

            UpdatePanelItemsIndex();
        }

        void BindEventBtnAddClick()
        {
            this.btnAdd.Click += (s, a) =>
            {
                var control = new Views.UserControls.MultiConfUI(
                    new Models.Datas.MultiConfItem(),
                    UpdatePanelItemsIndex
                );
                flyPanel.Controls.Add(control);
                flyPanel.ScrollControlIntoView(control);
                UpdatePanelItemsIndex();
            };
        }

        void BindEventFlyPanelDragDrop()
        {
            this.flyPanel.DragDrop += (s, a) =>
            {
                // https://www.codeproject.com/Articles/48411/Using-the-FlowLayoutPanel-and-Reordering-with-Drag

                var data =
                    a.Data.GetData(typeof(Views.UserControls.MultiConfUI))
                    as Views.UserControls.MultiConfUI;

                var _destination = s as FlowLayoutPanel;
                Point p = _destination.PointToClient(new Point(a.X, a.Y));
                var item = _destination.GetChildAtPoint(p);
                int index = _destination.Controls.GetChildIndex(item, false);
                _destination.Controls.SetChildIndex(data, index);
                _destination.Invalidate();
            };
        }

        void BindEvent()
        {
            BindEventBtnAddClick();
            BindEventFlyPanelDragDrop();

            this.flyPanel.DragEnter += (s, a) =>
            {
                a.Effect = DragDropEffects.Move;
            };
        }

        void UpdatePanelItemsIndex()
        {
            var index = 1;
            foreach (Views.UserControls.MultiConfUI item in this.flyPanel.Controls)
            {
                item.SetIndex(index++);
            }
        }
        #endregion
    }
}
