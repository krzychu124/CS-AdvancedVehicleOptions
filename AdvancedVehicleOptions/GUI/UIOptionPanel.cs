using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework.Threading;

using UIUtils = SamsamTS.UIUtils;

using AdvancedVehicleOptionsUID.Compatibility;

namespace AdvancedVehicleOptionsUID.GUI
{
    public class UIOptionPanel : UIPanel
    {
        private const float maxSpeedToKmhConversionFactor = 6.25f;
        private const float mphFactor = 1.609344f;

        private UITextField m_maxSpeed;
        private UITextField m_acceleration;
        private UITextField m_braking;
        private UITextField m_turning;
        private UITextField m_springs;
        private UITextField m_dampers;
        private UITextField m_leanMultiplier;
        private UITextField m_nodMultiplier;
        private UICheckBox m_useColors;
        private UIColorField m_color0;
        private UIColorField m_color1;
        private UIColorField m_color2;
        private UIColorField m_color3;
        private UITextField m_color0_hex;
        private UITextField m_color1_hex;
        private UITextField m_color2_hex;
        private UITextField m_color3_hex;
        private UICheckBox m_enabled;
        private UICheckBox m_addBackEngine;
        private UITextField m_capacity;
        private UITextField m_specialcapacity;
        private UIButton m_restore;
        private UIButton m_lineoverview;
        private UIButton m_userguidespawn;
        private UILabel m_removeLabel;
        private UIButton m_clearVehicles;
        private UIButton m_clearParked;
        private PublicTransportDetailPanel _publicTransportDetailPanel;
        private UILabel capacityLabel;
        private UILabel specialcapacityLabel;
        private UIButton m_userguidespecialcapacity;
        private UILabel bustrailerLabel;
        private UICheckBox m_isLargeVehicle;
        private UILabel m_useColorsLabel;
        private UILabel kmhLabel;

        public VehicleOptions m_options = null;

        private bool m_initialized = false;
        private int LineOverviewType = -1;
        private Color32 OldColorTextField;
        private Color32 OldColorText;
        private string OldCapacityTooltip;
        private string OldMaxSpeedTooltip;
        private string OldSpecialCapacityTooltip;

        public event PropertyChangedEventHandler<bool> eventEnableCheckChanged;

        public override void Start()
        {
            base.Start();
            canFocus = true;
            isInteractive = true;
            width = 390;
            height = 370;

            SetupControls();
            OldCapacityTooltip = m_capacity.tooltip;
            OldMaxSpeedTooltip = m_maxSpeed.tooltip;
            OldSpecialCapacityTooltip = m_specialcapacity.tooltip;
            OldColorTextField = m_capacity.color;
            OldColorText = m_capacity.textColor;

            m_options = new VehicleOptions();
        }

        public void Show(VehicleOptions options)
        {
            m_initialized = false;

            m_options = options;

            if (m_color0 == null) return;

            m_color0.relativePosition = new Vector3(35, 135);
            m_color1.relativePosition = new Vector3(35, 160);
            m_color2.relativePosition = new Vector3(182, 135);
            m_color3.relativePosition = new Vector3(182, 160);

            if (!AdvancedVehicleOptionsUID.SpeedUnitOption)
            {
                m_maxSpeed.text = Mathf.RoundToInt(options.maxSpeed * maxSpeedToKmhConversionFactor).ToString();
                kmhLabel.text = "km/h";
            }
            else
            {
                m_maxSpeed.text = Mathf.RoundToInt((options.maxSpeed / mphFactor) * maxSpeedToKmhConversionFactor).ToString();
                kmhLabel.text = "mph";
            }

            m_acceleration.text = options.acceleration.ToString();//m_capacity
            m_braking.text = options.braking.ToString();
            m_turning.text = options.turning.ToString();
            m_springs.text = options.springs.ToString();
            m_dampers.text = options.dampers.ToString();
            m_leanMultiplier.text = options.leanMultiplier.ToString();
            m_nodMultiplier.text = options.nodMultiplier.ToString();
            m_useColors.isChecked = options.useColorVariations;
            m_color0.selectedColor = options.color0;
            m_color1.selectedColor = options.color1;
            m_color2.selectedColor = options.color2;
            m_color3.selectedColor = options.color3;
            m_color0_hex.text = options.color0.ToString();
            m_color1_hex.text = options.color1.ToString();
            m_color2_hex.text = options.color2.ToString();
            m_color3_hex.text = options.color3.ToString();
            m_isLargeVehicle.isChecked = options.isLargeVehicle;

            m_enabled.isChecked = options.enabled;
            m_enabled.Show();
            m_enabled.isVisible = !options.isTrailer;

            m_addBackEngine.isChecked = options.addBackEngine;
            m_addBackEngine.isVisible = options.isTrain;
            m_maxSpeed.isInteractive = true;

            m_lineoverview.Hide();
            m_isLargeVehicle.Hide();
            m_useColorsLabel.Hide();

            m_capacity.parent.isVisible = true;
            m_capacity.isInteractive = true;
            m_capacity.text = options.capacity.ToString();
            m_capacity.isVisible = options.hasCapacity;
            capacityLabel.isVisible = options.hasCapacity;
            bustrailerLabel.isVisible = false;

            m_specialcapacity.text = options.specialcapacity.ToString();
            m_specialcapacity.isVisible = options.hasSpecialCapacity;
            specialcapacityLabel.isVisible = options.hasSpecialCapacity;
            m_userguidespecialcapacity.isVisible = options.hasSpecialCapacity;

            m_capacity.color = OldColorTextField;
            m_capacity.textColor = OldColorText;
            m_capacity.tooltip = OldCapacityTooltip;
            m_specialcapacity.tooltip = OldSpecialCapacityTooltip;
            m_maxSpeed.color = OldColorTextField;
            m_maxSpeed.textColor = OldColorText;
            m_maxSpeed.tooltip = OldMaxSpeedTooltip;

            //DebugUtils.Log("GameBalanceOptionsptions " + AdvancedVehicleOptionsUID.GameBalanceOptions);		
            //DebugUtils.Log("NonPaxCargo - Do Not Show Capacity " + options.isNonPaxCargo);		
            //DebugUtils.Log("Capacity - Do Not Show Capacity " + options.hasCapacity);	

            //Only display Cargo Capacity or Passenger Capacity - not any other values
            if (options.isNonPaxCargo == true && options.hasCapacity == true && AdvancedVehicleOptionsUID.GameBalanceOptions == true)
            {
                m_capacity.parent.isVisible = true;
            }
            else
                if (options.isNonPaxCargo == true && options.hasCapacity == true && AdvancedVehicleOptionsUID.GameBalanceOptions == false)
            {
                m_capacity.parent.isVisible = false;
            }

            //Compatibility Patch for Vehicle Color Expander - hide all controls, if mod is active. Not relating to PublicTransport only, but all vehicles
            if (VCXCompatibilityPatch.IsVCXActive())
            {
                m_useColors.enabled = false;
                m_color0.Hide();
                m_color0_hex.Hide();
                m_color1.Hide();
                m_color1_hex.Hide();
                m_color2.Hide();
                m_color2_hex.Hide();
                m_color3.Hide();
                m_color3_hex.Hide();
                m_useColorsLabel.Show();
            }

            //DebugUtils.Log("IsIPTActive " + IPTCompatibilityPatch.IsIPTActive());	
            //DebugUtils.Log("OverrideIPT " + AdvancedVehicleOptionsUID.OverrideIPT);	
            //DebugUtils.Log("IsPublicTransport " + options.isPublicTransport);	

            // Compatibility Patch for IPT, TLM and Cities Skylines Vehicle Spawning, Vehicle values. For Spawning Vehicles the Line Overview Window will be shown.

            if ((options.isPublicTransportGame == true) && AdvancedVehicleOptionsUID.SpawnControl == true)
            {
                m_enabled.Hide();
                m_lineoverview.Show();
                m_userguidespawn.Show();
                LineOverviewType = options.ReturnLineOverviewType;
            }
            else
            {
                if (options.ValidateIsBusTrailer)
                {
                    m_enabled.Hide();
                    bustrailerLabel.isVisible = true;
                }
                else
                {
                    m_enabled.Show();
                    bustrailerLabel.isVisible = false;
                }
                m_lineoverview.Hide();
                m_userguidespawn.Hide();
            }

            if (AdvancedVehicleOptionsUID.OverrideCompatibilityWarnings == true && options.isPublicTransport == true && !options.isNotPublicTransportMod == true)
            {
                if (IPTCompatibilityPatch.IsIPTActive() == true)
                {
                    m_maxSpeed.color = new Color32(240, 130, 130, 255);
                    m_capacity.color = new Color32(240, 130, 130, 255);
                    m_specialcapacity.color = new Color32(240, 130, 130, 255);
                    m_maxSpeed.textColor = new Color32(255, 230, 130, 255);
                    m_capacity.textColor = new Color32(255, 230, 130, 255);
                    m_specialcapacity.textColor = new Color32(255, 230, 130, 255);
                    m_maxSpeed.tooltip = m_maxSpeed.tooltip + "\n\nWarning: Improved Public Transport is active\nand may override this setting.";
                    m_capacity.tooltip = m_capacity.tooltip + "\n\nWarning: Improved Public Transport is active\nand may override this setting.";
                    m_specialcapacity.tooltip = m_specialcapacity.tooltip + "\n\nWarning: Improved Public Transport is active\nand may override this setting.";
                }
                if (TLMCompatibilityPatch.IsTLMActive() == true)
                {
                    m_capacity.color = new Color32(240, 130, 130, 255);
                    m_capacity.textColor = new Color32(255, 230, 130, 255);
                    m_capacity.tooltip = m_capacity.tooltip + "\n\nWarning: Transport Lines Manager is active\nand may override this setting.";
                }
            }

            if (NoBigTruckCompatibilityPatch.IsNBTActive() == true && AdvancedVehicleOptionsUID.ControlTruckDelivery == true && options.isDelivery == true && options.hasTrailer == true)
            {
                m_isLargeVehicle.Show();
                //DebugUtils.Log("AVO IsLargeVehicle");	
            }

            // Compatibility Patch section ends

            string name = options.localizedName;
            if (name.Length > 40) name = name.Substring(0, 38) + "...";
            m_removeLabel.text = "Actions for: " + name;

            (parent as UIMainPanel).ChangePreviewColor(m_color0.selectedColor);

            capacityLabel.text = options.CapacityString;
            specialcapacityLabel.text = options.SpecialCapacityString;

            m_initialized = true;
        }

        private void SetupControls()
        {
            UIPanel panel = AddUIComponent<UIPanel>();
            panel.gameObject.AddComponent<UICustomControl>();

            panel.backgroundSprite = "UnlockingPanel";
            panel.width = width - 10;
            panel.height = height - 75;
            panel.relativePosition = new Vector3(5, 0);

            // Max Speed
            UILabel maxSpeedLabel = panel.AddUIComponent<UILabel>();
            maxSpeedLabel.text = "Maximum speed:";
            maxSpeedLabel.textScale = 0.8f;
            maxSpeedLabel.relativePosition = new Vector3(15, 14);

            m_maxSpeed = UIUtils.CreateTextField(panel);
            m_maxSpeed.numericalOnly = true;
            m_maxSpeed.width = 75;
            m_maxSpeed.tooltip = "Change the maximum speed of the vehicle\nPlease note that vehicles do not go beyond speed limits";
            m_maxSpeed.relativePosition = new Vector3(15, 33);

            kmhLabel = panel.AddUIComponent<UILabel>();
            kmhLabel.text = "km/h";
            kmhLabel.textScale = 0.8f;
            kmhLabel.relativePosition = new Vector3(95, 38);

            // Acceleration
            UILabel accelerationLabel = panel.AddUIComponent<UILabel>();
            accelerationLabel.text = "Acceleration/Brake/Turning:";
            accelerationLabel.textScale = 0.8f;
            accelerationLabel.relativePosition = new Vector3(160, 13);

            m_acceleration = UIUtils.CreateTextField(panel);
            m_acceleration.numericalOnly = true;
            m_acceleration.allowFloats = true;
            m_acceleration.width = 60;
            m_acceleration.tooltip = "Change the vehicle acceleration factor";
            m_acceleration.relativePosition = new Vector3(160, 33);

            // Braking
            m_braking = UIUtils.CreateTextField(panel);
            m_braking.numericalOnly = true;
            m_braking.allowFloats = true;
            m_braking.width = 60;
            m_braking.tooltip = "Change the vehicle braking factor";
            m_braking.relativePosition = new Vector3(230, 33);

            // Turning
            m_turning = UIUtils.CreateTextField(panel);
            m_turning.numericalOnly = true;
            m_turning.allowFloats = true;
            m_turning.width = 60;
            m_turning.tooltip = "Change the vehicle turning factor;\nDefines how well the car corners";
            m_turning.relativePosition = new Vector3(300, 33);

            // Springs
            UILabel springsLabel = panel.AddUIComponent<UILabel>();
            springsLabel.text = "Springs/Dampers:";
            springsLabel.textScale = 0.8f;
            springsLabel.relativePosition = new Vector3(15, 66);

            m_springs = UIUtils.CreateTextField(panel);
            m_springs.numericalOnly = true;
            m_springs.allowFloats = true;
            m_springs.width = 60;
            m_springs.tooltip = "Change the vehicle spring factor;\nDefines how much the suspension moves";
            m_springs.relativePosition = new Vector3(15, 85);

            // Dampers
            m_dampers = UIUtils.CreateTextField(panel);
            m_dampers.numericalOnly = true;
            m_dampers.allowFloats = true;
            m_dampers.width = 60;
            m_dampers.tooltip = "Change the vehicle damper factor;\nDefines how quickly the suspension returns to the default state";
            m_dampers.relativePosition = new Vector3(85, 85);

            // LeanMultiplier
            UILabel leanMultiplierLabel = panel.AddUIComponent<UILabel>();
            leanMultiplierLabel.text = "Lean/Nod Multiplier:";
            leanMultiplierLabel.textScale = 0.8f;
            leanMultiplierLabel.relativePosition = new Vector3(160, 66);

            m_leanMultiplier = UIUtils.CreateTextField(panel);
            m_leanMultiplier.numericalOnly = true;
            m_leanMultiplier.allowFloats = true;
            m_leanMultiplier.width = 60;
            m_leanMultiplier.tooltip = "Change the vehicle lean multiplication factor;\nDefines how much the vehicle leans to the sides when turning";
            m_leanMultiplier.relativePosition = new Vector3(160, 85);

            // NodMultiplier
            m_nodMultiplier = UIUtils.CreateTextField(panel);
            m_nodMultiplier.numericalOnly = true;
            m_nodMultiplier.allowFloats = true;
            m_nodMultiplier.width = 60;
            m_nodMultiplier.tooltip = "Change the vehicle nod multiplication factor;\nDefines how much the vehicle nods forward/backward when braking or accelerating";
            m_nodMultiplier.relativePosition = new Vector3(230, 85);

            // Colors
            m_useColors = UIUtils.CreateCheckBox(panel);
            m_useColors.text = "Enable Color variations:";
            m_useColors.isChecked = true;
            m_useColors.width = width - 40;
            m_useColors.tooltip = "Enable color variations\nA random color is chosen between the four following colors";
            m_useColors.relativePosition = new Vector3(15, 116);

            m_color0 = UIUtils.CreateColorField(panel);
            m_color0.name = "AVO-color0";
            m_color0.popupTopmostRoot = false;
            m_color0.relativePosition = new Vector3(35, 135);
            m_color0_hex = UIUtils.CreateTextField(panel);
            m_color0_hex.maxLength = 6;
            m_color0_hex.relativePosition = new Vector3(80, 137);

            m_color1 = UIUtils.CreateColorField(panel);
            m_color1.name = "AVO-color1";
            m_color1.popupTopmostRoot = false;
            m_color1.relativePosition = new Vector3(35, 160);
            m_color1_hex = UIUtils.CreateTextField(panel);
            m_color1_hex.maxLength = 6;
            m_color1_hex.relativePosition = new Vector3(80, 162);

            m_color2 = UIUtils.CreateColorField(panel);
            m_color2.name = "AVO-color2";
            m_color2.popupTopmostRoot = false;
            m_color2.relativePosition = new Vector3(182, 135);
            m_color2_hex = UIUtils.CreateTextField(panel);
            m_color2_hex.maxLength = 6;
            m_color2_hex.relativePosition = new Vector3(225, 137);

            m_color3 = UIUtils.CreateColorField(panel);
            m_color3.name = "AVO-color3";
            m_color3.popupTopmostRoot = false;
            m_color3.relativePosition = new Vector3(182, 160);
            m_color3_hex = UIUtils.CreateTextField(panel);
            m_color3_hex.maxLength = 6;
            m_color3_hex.relativePosition = new Vector3(225, 162);

            m_useColorsLabel = panel.AddUIComponent<UILabel>();
            m_useColorsLabel.Hide();
            m_useColorsLabel.textScale = 0.8f;
            m_useColorsLabel.text = "Colors are managed by Vehicle Color Expander";
            m_useColorsLabel.relativePosition = new Vector3(15, 116);

            // Enable & BackEngine
            m_enabled = UIUtils.CreateCheckBox(panel);
            m_enabled.text = "Allow this vehicle to spawn";
            m_enabled.isChecked = true;
            m_enabled.width = width - 40;
            m_enabled.tooltip = "Make sure you have at least one vehicle allowed to spawn for that category";
            m_enabled.relativePosition = new Vector3(15, 195); ;

            m_addBackEngine = UIUtils.CreateCheckBox(panel);
            m_addBackEngine.text = "Replace last car with engine";
            m_addBackEngine.isChecked = false;
            m_addBackEngine.width = width - 40;
            m_addBackEngine.tooltip = "Make the last car of this train be an engine";
            m_addBackEngine.relativePosition = new Vector3(15, 215);

            // LargeVehicle Setting for NoBigTruck Delivery Mod
            m_isLargeVehicle = UIUtils.CreateCheckBox(panel);
            m_isLargeVehicle.text = "Flag as Large Vehicle";
            m_isLargeVehicle.width = width - 40;
            m_isLargeVehicle.tooltip = "Prevent vehicles with trailer to deliver to small buildings\n\nNeeds No Big Trucks mod to work";
            m_isLargeVehicle.relativePosition = new Vector3(15, 215);

            // Capacity
            UIPanel capacityPanel = panel.AddUIComponent<UIPanel>();
            capacityPanel.size = Vector2.zero;
            capacityPanel.relativePosition = new Vector3(15, 240);

            capacityLabel = capacityPanel.AddUIComponent<UILabel>();
            capacityLabel.text = "Capacity: ";
            capacityLabel.textScale = 0.8f;
            capacityLabel.relativePosition = new Vector3(0, 2);

            m_capacity = UIUtils.CreateTextField(capacityPanel);
            m_capacity.numericalOnly = true;
            m_capacity.maxLength = 8;
            m_capacity.width = 100;
            m_capacity.tooltip = "Change the capacity of the vehicle";
            m_capacity.relativePosition = new Vector3(0, 21);

            // Special Capacity			
            specialcapacityLabel = capacityPanel.AddUIComponent<UILabel>();
            specialcapacityLabel.Hide();
            specialcapacityLabel.text = "Special Capacity: ";
            specialcapacityLabel.textScale = 0.8f;
            specialcapacityLabel.relativePosition = new Vector3(160, 2);

            m_specialcapacity = UIUtils.CreateTextField(capacityPanel);
            m_specialcapacity.Hide();
            m_specialcapacity.numericalOnly = true;
            m_specialcapacity.maxLength = 8;
            m_specialcapacity.width = 100;
            m_specialcapacity.tooltip = "Change special parameters of the vehicle";
            m_specialcapacity.relativePosition = new Vector3(160, 21);

            // Userguide Special Capacity Button
            m_userguidespecialcapacity = UIUtils.CreateButton(capacityPanel);
            m_userguidespecialcapacity.Hide();
            m_userguidespecialcapacity.normalBgSprite = "EconomyMoreInfo";
            m_userguidespecialcapacity.hoveredBgSprite = "EconomyMoreInfoHovered";
            m_userguidespecialcapacity.size = new Vector2(14f, 14f);
            m_userguidespecialcapacity.tooltip = "If you do not know, what this value is:\nDo not touch it!\n\nClick for User Guide: Special Capacity";
            m_userguidespecialcapacity.relativePosition = new Vector3(265, 24);

            // Transport Line Overview Button	
            m_lineoverview = UIUtils.CreateButton(panel);
            m_lineoverview.Hide();
            m_lineoverview.textScale = 0.8f;
            m_lineoverview.height = 18;
            m_lineoverview.textVerticalAlignment = UIVerticalAlignment.Bottom;
            m_lineoverview.text = "Manage Spawning in Transport Line Overview";
            m_lineoverview.width = 335;
            m_lineoverview.tooltip = "Open the Transport Line Overview and manage the vehicle spawning";
            m_lineoverview.relativePosition = new Vector3(15, 194);

            // Userguide Spawn Button
            m_userguidespawn = UIUtils.CreateButton(panel);
            m_userguidespawn.Hide();
            m_userguidespawn.normalBgSprite = "EconomyMoreInfo";
            m_userguidespawn.hoveredBgSprite = "EconomyMoreInfoHovered";
            m_userguidespawn.size = new Vector2(14f, 14f);
            m_userguidespawn.tooltip = "Click for User Guide: Spawn Control";
            m_userguidespawn.relativePosition = new Vector3(355, 195);

            // Buslabel		
            bustrailerLabel = panel.AddUIComponent<UILabel>();
            bustrailerLabel.textScale = 0.8f;
            bustrailerLabel.text = "Bus trailers spawning is controlled by the main\n" +
                                   "Bus vehicle. Settings must be configured from\n" +
                                   "the Transport Line Overview panel.";
            bustrailerLabel.relativePosition = new Vector3(15, 194);

            // Restore default
            m_restore = UIUtils.CreateButton(panel);
            m_restore.text = "Default Values";
            m_restore.width = 120;
            m_restore.tooltip = "Restore all values to default";
            m_restore.relativePosition = new Vector3(250, height - 45);

            // Remove Vehicles
            m_removeLabel = this.AddUIComponent<UILabel>();
            m_removeLabel.text = "Global Actions for: ";
            m_removeLabel.textScale = 0.8f;
            m_removeLabel.relativePosition = new Vector3(10, height - 65);

            m_clearVehicles = UIUtils.CreateButton(this);
            m_clearVehicles.text = "Remove Driving";
            m_clearVehicles.width = 120;
            m_clearVehicles.tooltip = "Remove all driving vehicles of that type\nHold the SHIFT key to remove all types";
            m_clearVehicles.relativePosition = new Vector3(5, height - 45);

            m_clearParked = UIUtils.CreateButton(this);
            m_clearParked.text = "Remove Parked";
            m_clearParked.width = 120;
            m_clearParked.tooltip = "Remove all parked vehicles of that type\nHold the SHIFT key to remove all types";
            m_clearParked.relativePosition = new Vector3(130, height - 45);

            panel.BringToFront();

            // Event handlers
            m_maxSpeed.eventTextSubmitted += OnMaxSpeedSubmitted;
            m_acceleration.eventTextSubmitted += OnAccelerationSubmitted;
            m_braking.eventTextSubmitted += OnBrakingSubmitted;
            m_turning.eventTextSubmitted += OnTurningSubmitted;
            m_springs.eventTextSubmitted += OnSpringsSubmitted;
            m_dampers.eventTextSubmitted += OnDampersSubmitted;
            m_leanMultiplier.eventTextSubmitted += OnleanMultiplierSubmitted;
            m_nodMultiplier.eventTextSubmitted += OnnodMultiplierSubmitted;

            m_useColors.eventCheckChanged += OnCheckChanged;

            MouseEventHandler mousehandler = (c, p) => { if (m_initialized) (parent as UIMainPanel).ChangePreviewColor((c as UIColorField).selectedColor); };

            m_color0.eventMouseEnter += mousehandler;
            m_color1.eventMouseEnter += mousehandler;
            m_color2.eventMouseEnter += mousehandler;
            m_color3.eventMouseEnter += mousehandler;

            m_color0_hex.eventMouseEnter += (c, p) => { if (m_initialized) (parent as UIMainPanel).ChangePreviewColor(m_color0.selectedColor); };
            m_color1_hex.eventMouseEnter += (c, p) => { if (m_initialized) (parent as UIMainPanel).ChangePreviewColor(m_color1.selectedColor); };
            m_color2_hex.eventMouseEnter += (c, p) => { if (m_initialized) (parent as UIMainPanel).ChangePreviewColor(m_color2.selectedColor); };
            m_color3_hex.eventMouseEnter += (c, p) => { if (m_initialized) (parent as UIMainPanel).ChangePreviewColor(m_color3.selectedColor); };

            m_color0.eventSelectedColorChanged += OnColorChanged;
            m_color1.eventSelectedColorChanged += OnColorChanged;
            m_color2.eventSelectedColorChanged += OnColorChanged;
            m_color3.eventSelectedColorChanged += OnColorChanged;

            m_color0_hex.eventTextSubmitted += OnColorHexSubmitted;
            m_color1_hex.eventTextSubmitted += OnColorHexSubmitted;
            m_color2_hex.eventTextSubmitted += OnColorHexSubmitted;
            m_color3_hex.eventTextSubmitted += OnColorHexSubmitted;

            m_enabled.eventCheckChanged += OnCheckChanged;
            m_addBackEngine.eventCheckChanged += OnCheckChanged;
            m_isLargeVehicle.eventCheckChanged += OnCheckChanged;

            m_capacity.eventTextSubmitted += OnCapacitySubmitted;
            m_specialcapacity.eventTextSubmitted += OnSpecialCapacitySubmitted;

            m_restore.eventClick += (c, p) =>
            {
                m_initialized = false;
                bool isEnabled = m_options.enabled;
                DefaultOptions.Restore(m_options.prefab);
                VehicleOptions.UpdateTransfertVehicles();

                VehicleOptions.prefabUpdateEngine = m_options.prefab;
                VehicleOptions.prefabUpdateUnits = m_options.prefab;
                SimulationManager.instance.AddAction(VehicleOptions.UpdateBackEngines);
                SimulationManager.instance.AddAction(VehicleOptions.UpdateCapacityUnits);

                Show(m_options);

                if (m_options.enabled != isEnabled)
                    eventEnableCheckChanged(this, m_options.enabled);
            };

            m_clearVehicles.eventClick += OnClearVehicleClicked;
            m_clearParked.eventClick += OnClearVehicleClicked;
            m_lineoverview.eventClick += OnlineoverviewClicked;
            m_userguidespawn.eventClick += OnUserGuideSpawnClicked;
            m_userguidespecialcapacity.eventClick += OnUserGuideSpecialCapacityClicked;
        }

        protected void OnCheckChanged(UIComponent component, bool state)
        {
            if (!m_initialized || m_options == null) return;
            m_initialized = false;

            if (component == m_enabled)
            {
                if (m_options.isTrailer)
                {
                    VehicleOptions engine = m_options.engine;

                    if (engine.enabled != state)
                    {
                        engine.enabled = state;
                        VehicleOptions.UpdateTransfertVehicles();
                        eventEnableCheckChanged(this, state);
                    }
                }
                else
                {
                    if (m_options.enabled != state)
                    {
                        m_options.enabled = state;
                        VehicleOptions.UpdateTransfertVehicles();
                        eventEnableCheckChanged(this, state);
                    }
                }

                if (!state && !AdvancedVehicleOptionsUID.CheckServiceValidity(m_options.category))
                {
                    GUI.UIWarningModal.instance.message = UIMainPanel.categoryList[(int)m_options.category + 1] + " may not work correctly because no vehicles are allowed to spawn.";
                    UIView.PushModal(GUI.UIWarningModal.instance);
                    GUI.UIWarningModal.instance.Show(true);
                }
            }
            else if (component == m_addBackEngine && m_options.addBackEngine != state)
            {
                m_options.addBackEngine = state;
                if (m_options.addBackEngine == state)
                {
                    VehicleOptions.prefabUpdateEngine = m_options.prefab;
                    SimulationManager.instance.AddAction(VehicleOptions.UpdateBackEngines);
                }
            }
            else if (component == m_useColors && m_options.useColorVariations != state)
            {
                m_options.useColorVariations = state;
                (parent as UIMainPanel).ChangePreviewColor(m_color0.selectedColor);
            }
            else if (component == m_isLargeVehicle)
            {
                m_options.isLargeVehicle = state;
            }

            m_initialized = true;
        }

        protected void OnMaxSpeedSubmitted(UIComponent component, string text)
        {
            if (!m_initialized || m_options == null) return;
            m_initialized = false;

            if (!AdvancedVehicleOptionsUID.SpeedUnitOption)
            {
                m_options.maxSpeed = float.Parse(text) / maxSpeedToKmhConversionFactor;
            }
            else
                m_options.maxSpeed = (float.Parse(text) * mphFactor) / maxSpeedToKmhConversionFactor;

            m_initialized = true;
        }

        protected void OnAccelerationSubmitted(UIComponent component, string text)
        {
            if (!m_initialized || m_options == null) return;
            m_initialized = false;

            m_options.acceleration = float.Parse(text);

            m_initialized = true;
        }

        protected void OnBrakingSubmitted(UIComponent component, string text)
        {
            if (!m_initialized || m_options == null) return;
            m_initialized = false;

            m_options.braking = float.Parse(text);

            m_initialized = true;
        }
        
        protected void OnTurningSubmitted(UIComponent component, string text)
        {
            if (!m_initialized || m_options == null) return;
            m_initialized = false;

            m_options.turning = float.Parse(text);

            m_initialized = true;
        }

        protected void OnSpringsSubmitted(UIComponent component, string text)
        {
            if (!m_initialized || m_options == null) return;
            m_initialized = false;

            m_options.springs = float.Parse(text);

            m_initialized = true;
        }

        protected void OnDampersSubmitted(UIComponent component, string text)
        {
            if (!m_initialized || m_options == null) return;
            m_initialized = false;

            m_options.dampers = float.Parse(text);

            m_initialized = true;
        }

        protected void OnleanMultiplierSubmitted(UIComponent component, string text)
        {
            if (!m_initialized || m_options == null) return;
            m_initialized = false;

            m_options.leanMultiplier = float.Parse(text);

            m_initialized = true;
        }

        protected void OnnodMultiplierSubmitted(UIComponent component, string text)
        {
            if (!m_initialized || m_options == null) return;
            m_initialized = false;

            m_options.nodMultiplier = float.Parse(text);

            m_initialized = true;
        }

        protected void OnCapacitySubmitted(UIComponent component, string text)
        {
            if (!m_initialized || m_options == null) return;
            m_initialized = false;

            m_options.capacity = int.Parse(text);
            VehicleOptions.prefabUpdateUnits = m_options.prefab;
            SimulationManager.instance.AddAction(VehicleOptions.UpdateCapacityUnits);

            m_initialized = true;
        }

        protected void OnSpecialCapacitySubmitted(UIComponent component, string text)
        {
            if (!m_initialized || m_options == null) return;
            m_initialized = false;

            m_options.specialcapacity = int.Parse(text);
            VehicleOptions.prefabUpdateUnits = m_options.prefab;
            SimulationManager.instance.AddAction(VehicleOptions.UpdateCapacityUnits);

            m_initialized = true;
        }

        protected void OnColorChanged(UIComponent component, Color color)
        {
            if (!m_initialized || m_options == null) return;
            m_initialized = false;

            (parent as UIMainPanel).ChangePreviewColor(color);

            m_options.color0 = m_color0.selectedColor;
            m_options.color1 = m_color1.selectedColor;
            m_options.color2 = m_color2.selectedColor;
            m_options.color3 = m_color3.selectedColor;

            m_color0_hex.text = m_options.color0.ToString();
            m_color1_hex.text = m_options.color1.ToString();
            m_color2_hex.text = m_options.color2.ToString();
            m_color3_hex.text = m_options.color3.ToString();

            m_initialized = true;
        }

        protected void OnColorHexSubmitted(UIComponent component, string text)
        {
            if (!m_initialized || m_options == null) return;
            m_initialized = false;

            // Is text a valid color?
            if(text != "000000" && new HexaColor(text).ToString() == "000000")
            {
                m_color0_hex.text = m_options.color0.ToString();
                m_color1_hex.text = m_options.color1.ToString();
                m_color2_hex.text = m_options.color2.ToString();
                m_color3_hex.text = m_options.color3.ToString();

                m_initialized = true;
                return;
            }

            m_options.color0 = new HexaColor(m_color0_hex.text);
            m_options.color1 = new HexaColor(m_color1_hex.text);
            m_options.color2 = new HexaColor(m_color2_hex.text);
            m_options.color3 = new HexaColor(m_color3_hex.text);

            m_color0_hex.text = m_options.color0.ToString();
            m_color1_hex.text = m_options.color1.ToString();
            m_color2_hex.text = m_options.color2.ToString();
            m_color3_hex.text = m_options.color3.ToString();

            m_color0.selectedColor = m_options.color0;
            m_color1.selectedColor = m_options.color1;
            m_color2.selectedColor = m_options.color2;
            m_color3.selectedColor = m_options.color3;

            (parent as UIMainPanel).ChangePreviewColor(color);

            m_initialized = true;
        }

        protected void OnClearVehicleClicked(UIComponent component, UIMouseEventParameter p)
        {
            if (m_options == null) return;

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                AdvancedVehicleOptionsUID.ClearVehicles(null, component == m_clearParked);
            else
                AdvancedVehicleOptionsUID.ClearVehicles(m_options, component == m_clearParked);
        }

        protected void OnlineoverviewClicked(UIComponent component, UIMouseEventParameter p)
		{
            this._publicTransportDetailPanel = GameObject.Find("(Library) PublicTransportDetailPanel").GetComponent<PublicTransportDetailPanel>();
		
			if (this._publicTransportDetailPanel.component.isVisible == true) 
			{
				UIView.library.Hide("PublicTransportDetailPanel");
			}
			else
			{
				PublicTransportDetailPanel publicTransportDetailPanel = UIView.library.Show<PublicTransportDetailPanel>("PublicTransportDetailPanel", bringToFront: true, onlyWhenInvisible: true);
					if (LineOverviewType != -1)
					{
                    publicTransportDetailPanel.SetActiveTab(LineOverviewType);
					}
			}
        }

        protected void OnUserGuideSpawnClicked(UIComponent component, UIMouseEventParameter p)
        {
            SimulationManager.instance.SimulationPaused = true;
            Application.OpenURL("https://github.com/CityGecko/CS-AdvancedVehicleOptions/wiki/02.06-Vehicle-Spawning");
        }
        protected void OnUserGuideSpecialCapacityClicked(UIComponent component, UIMouseEventParameter p)
        {
            SimulationManager.instance.SimulationPaused = true;
            Application.OpenURL("https://github.com/CityGecko/CS-AdvancedVehicleOptions/wiki/02.05-Vehicle-Settings");
        }
    }

}
