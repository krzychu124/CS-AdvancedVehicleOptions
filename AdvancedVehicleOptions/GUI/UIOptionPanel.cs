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
        private UIButton m_restore;
        private UILabel m_removeLabel;
        private UIButton m_clearVehicles;
        private UIButton m_clearParked;

        public VehicleOptions m_options = null;

        private bool m_initialized = false;

        public event PropertyChangedEventHandler<bool> eventEnableCheckChanged;

        public override void Start()
        {
            base.Start();
            canFocus = true;
            isInteractive = true;
            width = 390;
            height = 370;

            SetupControls();

            m_options = new VehicleOptions();
        }

        public void Show(VehicleOptions options)
        {
            m_initialized = false;

            m_options = options;

            if (m_color0 == null) return;

            m_color0.relativePosition = new Vector3(13, 140 - 2);
            m_color1.relativePosition = new Vector3(13, 165 - 2);
            m_color2.relativePosition = new Vector3(158, 140 - 2);
            m_color3.relativePosition = new Vector3(158, 165 - 2);
			
            m_maxSpeed.text = Mathf.RoundToInt(options.maxSpeed * maxSpeedToKmhConversionFactor).ToString();
            m_acceleration.text = options.acceleration.ToString();
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
            m_enabled.isChecked = options.enabled;
            //m_enabled.isVisible = !options.isTrailer;
            m_addBackEngine.isChecked = options.addBackEngine;
            m_addBackEngine.isVisible = options.isTrain;

            m_capacity.text = options.capacity.ToString();
            m_capacity.parent.isVisible = options.hasCapacity;
			
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
				m_useColors.isInteractive = false;
				m_useColors.isEnabled = false;
				m_useColors.text = "Colors are managed by Vehicle Color Expander";
				m_useColors.tooltip = "Vehicle Colors are managed by Vehicle Color Expander.";
				m_color0.isEnabled = false;	
				m_color0.isInteractive = false;
				m_color0_hex.isInteractive = false;
				m_color0_hex.isVisible = false;
				m_color1.isInteractive = false;
				m_color1.isEnabled = false;					
				m_color1_hex.isInteractive = false;		
				m_color1_hex.isVisible = false;				
				m_color2.isInteractive = false;			
				m_color2.isEnabled = false;
				m_color2_hex.isInteractive = false;
				m_color2_hex.isVisible = false;				
				m_color3.isInteractive = false;	
				m_color3.isEnabled = false;
				m_color3_hex.isInteractive = false;	
				m_color3_hex.isVisible = false;			
			}				
						
			//DebugUtils.Log("IsIPTActive " + IPTCompatibilityPatch.IsIPTActive());	
			//DebugUtils.Log("OverrideIPT " + AdvancedVehicleOptionsUID.OverrideIPT);	
			//DebugUtils.Log("IsPublicTransport " + options.isPublicTransport);	
			
			// Compatibility Patch for IPT, TLM and Cities Skylines Vehicle Spawning, Vehicle values. Only affecting Public Transport, except Intercity Bus
			m_maxSpeed.isInteractive = true;	
			m_enabled.isInteractive = true;
			m_enabled.isEnabled = true;
			m_capacity.isInteractive = true;
			m_enabled.text = "Allow this vehicle to spawn";
			m_enabled.tooltip = "Make sure you have at least one vehicle allowed to spawn for that category";
			
			if ((options.isPublicTransportGame == true) && AdvancedVehicleOptionsUID.SpawnControl == true && options.isUncontrolledPublicTransport == false)
			{
				m_enabled.isInteractive = false;
			    m_enabled.isEnabled = false;
                
				if (IPTCompatibilityPatch.IsIPTActive() == true)
				{
				m_enabled.text = "Spawning is managed by IPT";
				m_enabled.tooltip = "Improved Public Transport will take care for public transport spawning vehicles.";		
				}
				else
				if (TLMCompatibilityPatch.IsTLMActive() == true)
				{
				m_enabled.text = "Spawning is managed by TLM";
				m_enabled.tooltip = "Transport Lines Manager will take care for public transport spawning vehicles.";		
				}
				else
				{
				m_enabled.text = "Spawning is managed by Cities:Skylines";	
			    m_enabled.tooltip = "The game will take care for spawning public transport vehicles.";
				}
			}
			
			if (IPTCompatibilityPatch.IsIPTActive() == true && AdvancedVehicleOptionsUID.OverrideIPT == true && options.isPublicTransport == true && options.isUncontrolledPublicTransport == false)
			{
				m_maxSpeed.isInteractive = false;
				m_maxSpeed.text = "IPT";	
				m_capacity.isInteractive = false;
				m_capacity.text = "IPT";				
			}
			
			if (TLMCompatibilityPatch.IsTLMActive() == true && AdvancedVehicleOptionsUID.OverrideTLM == true && options.isPublicTransport == true && options.isUncontrolledPublicTransport == false)
			{
				m_capacity.isInteractive = false;
				m_capacity.text = "TLM";				
			}
	
			// Compatibility Patch section ends
			
            string name = options.localizedName;
            if (name.Length > 20) name = name.Substring(0, 20) + "...";
            m_removeLabel.text = "Remove vehicles (" + name + "):";

            (parent as UIMainPanel).ChangePreviewColor(m_color0.selectedColor);

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
            maxSpeedLabel.relativePosition = new Vector3(15, 15);

            m_maxSpeed = UIUtils.CreateTextField(panel);
            m_maxSpeed.numericalOnly = true;
            m_maxSpeed.width = 75;
            m_maxSpeed.tooltip = "Change the maximum speed of the vehicle\nPlease note that vehicles do not go beyond speed limits";
            m_maxSpeed.relativePosition = new Vector3(15, 35);

            UILabel kmh = panel.AddUIComponent<UILabel>();
            kmh.text = "km/h";
            kmh.textScale = 0.8f;
            kmh.relativePosition = new Vector3(95, 40);

            // Acceleration
            UILabel accelerationLabel = panel.AddUIComponent<UILabel>();
            accelerationLabel.text = "Acceleration/Brake/Turning:";
            accelerationLabel.textScale = 0.8f;
            accelerationLabel.relativePosition = new Vector3(160, 15);

            m_acceleration = UIUtils.CreateTextField(panel);
            m_acceleration.numericalOnly = true;
            m_acceleration.allowFloats = true;
            m_acceleration.width = 60;
            m_acceleration.tooltip = "Change the vehicle acceleration factor";
            m_acceleration.relativePosition = new Vector3(160, 35);

            // Braking
            m_braking = UIUtils.CreateTextField(panel);
            m_braking.numericalOnly = true;
            m_braking.allowFloats = true;
            m_braking.width = 60;
            m_braking.tooltip = "Change the vehicle braking factor";
            m_braking.relativePosition = new Vector3(230, 35);
            
            // Turning
            m_turning = UIUtils.CreateTextField(panel);
            m_turning.numericalOnly = true;
            m_turning.allowFloats = true;
            m_turning.width = 60;
            m_turning.tooltip = "Change the vehicle turning factor;\nDefines how well the car corners";
            m_turning.relativePosition = new Vector3(300, 35);

            // Springs
            UILabel springsLabel = panel.AddUIComponent<UILabel>();
            springsLabel.text = "Springs/Dampers:";
            springsLabel.textScale = 0.8f;
            springsLabel.relativePosition = new Vector3(15, 70);

            m_springs = UIUtils.CreateTextField(panel);
            m_springs.numericalOnly = true;
            m_springs.allowFloats = true;
            m_springs.width = 60;
            m_springs.tooltip = "Change the vehicle spring factor;\nDefines how much the suspension moves";
            m_springs.relativePosition = new Vector3(15, 90);

            // Dampers
            m_dampers = UIUtils.CreateTextField(panel);
            m_dampers.numericalOnly = true;
            m_dampers.allowFloats = true;
            m_dampers.width = 60;
            m_dampers.tooltip = "Change the vehicle damper factor;\nDefines how quickly the suspension returns to the default state";
            m_dampers.relativePosition = new Vector3(85, 90);

            // LeanMultiplier
            UILabel leanMultiplierLabel = panel.AddUIComponent<UILabel>();
            leanMultiplierLabel.text = "Lean/Nod Multiplier:";
            leanMultiplierLabel.textScale = 0.8f;
            leanMultiplierLabel.relativePosition = new Vector3(160, 70);

            m_leanMultiplier = UIUtils.CreateTextField(panel);
            m_leanMultiplier.numericalOnly = true;
            m_leanMultiplier.allowFloats = true;
            m_leanMultiplier.width = 60;
            m_leanMultiplier.tooltip = "Change the vehicle lean multiplication factor;\nDefines how much the vehicle leans to the sides when turning";
            m_leanMultiplier.relativePosition = new Vector3(160, 90);

            // NodMultiplier
            m_nodMultiplier = UIUtils.CreateTextField(panel);
            m_nodMultiplier.numericalOnly = true;
            m_nodMultiplier.allowFloats = true;
            m_nodMultiplier.width = 60;
            m_nodMultiplier.tooltip = "Change the vehicle nod multiplication factor;\nDefines how much the vehicle nods forward/backward when braking or accelerating";
            m_nodMultiplier.relativePosition = new Vector3(230, 90);

            // Colors
            m_useColors = UIUtils.CreateCheckBox(panel);
            m_useColors.text = "Color variations:";
            m_useColors.isChecked = true;
            m_useColors.width = width - 40;
            m_useColors.tooltip = "Enable color variations\nA random color is chosen between the four following colors";
            m_useColors.relativePosition = new Vector3(15, 120);

            m_color0 = UIUtils.CreateColorField(panel);
            m_color0.name = "AVO-color0";
            m_color0.popupTopmostRoot = false;
            m_color0.relativePosition = new Vector3(13 , 140 - 2);
            m_color0_hex = UIUtils.CreateTextField(panel);
            m_color0_hex.maxLength = 6;
            m_color0_hex.relativePosition = new Vector3(55, 140);

            m_color1 = UIUtils.CreateColorField(panel);
            m_color1.name = "AVO-color1";
            m_color1.popupTopmostRoot = false;
            m_color1.relativePosition = new Vector3(13, 165 - 2);
            m_color1_hex = UIUtils.CreateTextField(panel);
            m_color1_hex.maxLength = 6;
            m_color1_hex.relativePosition = new Vector3(55, 165);

            m_color2 = UIUtils.CreateColorField(panel);
            m_color2.name = "AVO-color2";
            m_color2.popupTopmostRoot = false;
            m_color2.relativePosition = new Vector3(158, 140 - 2);
            m_color2_hex = UIUtils.CreateTextField(panel);
            m_color2_hex.maxLength = 6;
            m_color2_hex.relativePosition = new Vector3(200, 140);

            m_color3 = UIUtils.CreateColorField(panel);
            m_color3.name = "AVO-color3";
            m_color3.popupTopmostRoot = false;
            m_color3.relativePosition = new Vector3(158, 165 - 2);
            m_color3_hex = UIUtils.CreateTextField(panel);
            m_color3_hex.maxLength = 6;
            m_color3_hex.relativePosition = new Vector3(200, 165);
			
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

            // Capacity
            UIPanel capacityPanel = panel.AddUIComponent<UIPanel>();
            capacityPanel.size = Vector2.zero;
            capacityPanel.relativePosition = new Vector3(15, 240);

            UILabel capacityLabel = capacityPanel.AddUIComponent<UILabel>();
            capacityLabel.text = "Capacity:";
            capacityLabel.textScale = 0.8f;
            capacityLabel.relativePosition = Vector3.zero;

            m_capacity = UIUtils.CreateTextField(capacityPanel);
            m_capacity.numericalOnly = true;
            m_capacity.width = 110;
            m_capacity.tooltip = "Change the capacity of the vehicle";
            m_capacity.relativePosition = new Vector3(0, 20);
			
            // Restore default
            m_restore = UIUtils.CreateButton(panel);
            m_restore.text = "Restore default";
            m_restore.width = 130;
            m_restore.tooltip = "Restore all values to default";
            m_restore.relativePosition = new Vector3(160, 255);

            // Remove Vehicles
            m_removeLabel = this.AddUIComponent<UILabel>();
            m_removeLabel.text = "Remove vehicles:";
            m_removeLabel.textScale = 0.8f;
            m_removeLabel.relativePosition = new Vector3(10, height - 65);

            m_clearVehicles = UIUtils.CreateButton(this);
            m_clearVehicles.text = "Driving";
            m_clearVehicles.width = 90f;
            m_clearVehicles.tooltip = "Remove all driving vehicles of that type\nHold the SHIFT key to remove all types";
            m_clearVehicles.relativePosition = new Vector3(10, height - 45);

            m_clearParked = UIUtils.CreateButton(this);
            m_clearParked.text = "Parked";
            m_clearParked.width = 90f;
            m_clearParked.tooltip = "Remove all parked vehicles of that type\nHold the SHIFT key to remove all types";
            m_clearParked.relativePosition = new Vector3(105, height - 45);

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

            m_capacity.eventTextSubmitted += OnCapacitySubmitted;

            m_restore.eventClick += (c, p) =>
            {
                m_initialized = false;
                bool isEnabled = m_options.enabled;
                DefaultOptions.Restore(m_options.prefab);
                VehicleOptions.UpdateTransfertVehicles();

                VehicleOptions.prefabUpdateEngine = m_options.prefab;
                VehicleOptions.prefabUpdateUnits = m_options.prefab;
                new EnumerableActionThread(VehicleOptions.UpdateBackEngines);
                new EnumerableActionThread(VehicleOptions.UpdateCapacityUnits);

                Show(m_options);

                if (m_options.enabled != isEnabled)
                    eventEnableCheckChanged(this, m_options.enabled);
            };

            m_clearVehicles.eventClick += OnClearVehicleClicked;
            m_clearParked.eventClick += OnClearVehicleClicked;
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
                    new EnumerableActionThread(VehicleOptions.UpdateBackEngines);
                }
            }
            else if (component == m_useColors && m_options.useColorVariations != state)
            {
                m_options.useColorVariations = state;
                (parent as UIMainPanel).ChangePreviewColor(m_color0.selectedColor);
            }

            m_initialized = true;
        }

        protected void OnMaxSpeedSubmitted(UIComponent component, string text)
        {
            if (!m_initialized || m_options == null) return;
            m_initialized = false;

            m_options.maxSpeed = float.Parse(text) / maxSpeedToKmhConversionFactor;

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
            new EnumerableActionThread(VehicleOptions.UpdateCapacityUnits);

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
    }

}
