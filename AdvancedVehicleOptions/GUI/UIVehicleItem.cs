﻿using UnityEngine;
using ColossalFramework.UI;

namespace AdvancedVehicleOptions.GUI
{
    public class UIVehicleItem : UIPanel
    {
        private UISprite m_icon;
        private UISprite m_disabled;
        private UILabel m_name;
        private UIPanel m_background;

        private VehicleOptions m_options;

        public VehicleOptions options
        {
            get { return m_options; }
            set { m_options = value; }
        }

        public UIPanel background
        {
            get
            {
                if (m_background == null)
                {
                    m_background = AddUIComponent<UIPanel>();
                    m_background.width = width;
                    m_background.height = 40;
                    m_background.relativePosition = Vector2.zero;

                    m_background.zOrder = 0;
                }

                return m_background;
            }
        }
        
        public override void Start()
        {
            base.Start();

            isVisible = true;
            canFocus = true;
            isInteractive = true;
            width = parent.width;
            height = 40;

            m_icon = AddUIComponent<UISprite>();
            m_icon.spriteName = UIMainPanel.vehicleIconList[(int)m_options.category];
            m_icon.size = m_icon.spriteInfo.pixelSize;
            UIUtils.ResizeIcon(m_icon, new Vector2(32,32));
            m_icon.relativePosition = new Vector3(10, Mathf.Floor((height - m_icon.height) / 2));

            m_disabled = AddUIComponent<UISprite>();
            m_disabled.spriteName = "Niet";
            m_disabled.size = m_disabled.spriteInfo.pixelSize;
            UIUtils.ResizeIcon(m_disabled, new Vector2(32, 32));
            m_disabled.relativePosition = new Vector3(10, Mathf.Floor((height - m_disabled.height) / 2));

            m_name = AddUIComponent<UILabel>();
            m_name.textScale = 0.9f;
            m_name.text = m_options.localizedName;
            m_name.relativePosition = new Vector3(55, 13);

            Refresh();
        }

        public void Refresh()
        {
            m_disabled.isVisible = !options.enabled;
            m_name.textColor = options.enabled ? new Color32(255, 255, 255, 255) : new Color32(128, 128, 128, 255);
        }

        protected override void OnClick(UIMouseEventParameter p)
        {
            base.OnClick(p);

            m_name.textColor = new Color32(255, 255, 255, 255);
        }

    }
}