﻿using ICities;
using UnityEngine;

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ColossalFramework;
using ColossalFramework.Threading;
using ColossalFramework.UI;

using AdvancedVehicleOptionsUID.Compatibility;

namespace AdvancedVehicleOptionsUID
{
    public class ModInfo : IUserMod
    {
        public ModInfo()
        {
            try
            {
                // Creating setting file
                GameSettings.AddSettingsFile(new SettingsFile[] { new SettingsFile() { fileName = AdvancedVehicleOptionsUID.settingsFileName } });
            }
            catch (Exception e)
            {
                DebugUtils.Log("Couldn't load/create the setting file.");
                DebugUtils.LogException(e);
            }
        }

        public string Name
        {
            get { return "Advanced Vehicle Options " + version; }
        }

        public string Description
        {
            get { return "Customize your vehicles (supports Cities Skylines Sunset Harbor 1.13.0-f8)."; }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            try
            {
                UICheckBox checkBox;	
				UITextField TextField;
				UIButton Button;

// Section for General Settings

                UIHelperBase group_general = helper.AddGroup("General Settings                                                   " + Name);

                checkBox = (UICheckBox)group_general.AddCheckbox("Disable debug messages logging", DebugUtils.hideDebugMessages.value, (b) =>
                {
                    DebugUtils.hideDebugMessages.value = b;
                });
                checkBox.tooltip = "If checked, debug messages will not be logged.";

                group_general.AddSpace(10);

                checkBox = (UICheckBox)group_general.AddCheckbox("Hide the user interface", AdvancedVehicleOptionsUID.hideGUI.value, (b) =>
                {
                    AdvancedVehicleOptionsUID.hideGUI.value = b;
                    AdvancedVehicleOptionsUID.UpdateGUI();

                });
                checkBox.tooltip = "Hide the UI completely if you feel like you are done with it and want to save\n" +
                                   "the little bit of memory it takes. Everything else will still be functional.";

                checkBox = (UICheckBox)group_general.AddCheckbox("Disable warning for no available services at map loading", !AdvancedVehicleOptionsUID.onLoadCheck.value, (b) =>
                {
                    AdvancedVehicleOptionsUID.onLoadCheck.value = !b;
                });
                checkBox.tooltip = "Disable the check for missing service vehicles assigned in any category when loading a map.";

// Section for Game Balancing

                UIHelperBase group_balance = helper.AddGroup("Gameplay & Balancing");

// Checkbox for SpeedUnitOption kmh vs mph	

                checkBox = (UICheckBox)group_balance.AddCheckbox("Display Miles per Hour (mph) instead of Kilometer per Hour (km/h)", AdvancedVehicleOptionsUID.SpeedUnitOption.value, (b) =>
                {
                    AdvancedVehicleOptionsUID.SpeedUnitOption.value = b;
                });
                checkBox.tooltip = "Changes display of unit of speed from mph to km/h.";

// Checkbox for Game Balancing	

                checkBox = (UICheckBox)group_balance.AddCheckbox("Enable various values for non-cargo and non-passenger vehicles", AdvancedVehicleOptionsUID.GameBalanceOptions.value, (b) =>
                {
                    AdvancedVehicleOptionsUID.GameBalanceOptions.value = b;
                });
                checkBox.tooltip = "Allows changes the Firefighting Rate and Capacity for Fire Safety, the Crime Rate Capacity\n" +
                                   "for Police Vehicles and the Maintenance and Pumping Rate for Maintenance Vehicles.\n\n" +
                                   "Can de-balance the intended gameplay. Some values are not documented.";

// Section for Compatibility

                UIHelperBase group_compatibility = helper.AddGroup("Compatibility");

// Checkbox for Overriding Incompability Warnings

                checkBox = (UICheckBox)group_compatibility.AddCheckbox("Display Compatibility Warnings for Mods", AdvancedVehicleOptionsUID.OverrideCompatibilityWarnings.value, (b) =>
                {
                    AdvancedVehicleOptionsUID.OverrideCompatibilityWarnings.value = b;
                });

                checkBox.tooltip = "If enabled, settings which can be modified in Improved Public Transport\n" +
                                   "(by BloodyPenguin) and Transport Lines Manager (by Klyte) will be shown\n" +
                                   "with warning color. Values should be edited in these mods only.\n\n" +
                                   "If disabled, the coloring will not shown.";
                //True, if AVO shall shall color shared mod setting values in red.

// Checkbox for Vehicle Color Expander

                checkBox = (UICheckBox) group_compatibility.AddCheckbox("Vehicle Color Expander: Priority over AVO vehicle coloring", AdvancedVehicleOptionsUID.OverrideVCX.value, (b) =>
                {
                    AdvancedVehicleOptionsUID.OverrideVCX.value = b;
                });
				
                checkBox.tooltip = "Permanent setting, if Vehicle Color Expander (by Klyte) is active.\n" +
                                   "The color management is controlled by Vehicle Color Expander.\n\n" +
                                   "Values will be configured in Vehicle Color Expander.";	
				
				//True, if AVO shall not override Vehicle Color Expander settings. As there is not Settings for Vehicle Color Expander. AVO will show the option, but user cannot change anything as long readOnly is True.
				checkBox.readOnly = true;
				checkBox.label.textColor = Color.gray;		
				
				if (!VCXCompatibilityPatch.IsVCXActive())
				{
				    checkBox.enabled  = false;	//Do not show the option Checkbox, if Vehicle Color Expander is not active.
				}
		
// Checkbox for No Big Trucks

                checkBox = (UICheckBox)group_compatibility.AddCheckbox("No Big Trucks: Classify Generic Industry vehicles as Large Vehicle", AdvancedVehicleOptionsUID.ControlTruckDelivery.value, (b) =>
                {
                    AdvancedVehicleOptionsUID.ControlTruckDelivery.value = b;
                });

                checkBox.tooltip = "If enabled, Delivery Trucks can be tagged as Large Vehicles.\n" +
                                   "Dispatch will be blocked by No Big Trucks (by MacSergey).\n\n" +
                                   "Warning: Experimental feature and may have impact on the simulation.";
                //True, if AVO shall be enabled to classify Generic Industry vehicles as Large Vehicles, so No Big Trucks can suppress the dispatch to small buildings.

                if (!NoBigTruckCompatibilityPatch.IsNBTActive())
                {
                    checkBox.enabled = false;   //Do not show the option Checkbox, if No Big Trucks is not active.
                }

// Add a Spacer
                group_compatibility.AddSpace(20);

// Add Trailer Compatibility Reference

                TextField = (UITextField) group_compatibility.AddTextfield("Vehicle Trailer compatibility references last updated:", TrailerRef.Revision, (value) => Debug.Log(""), (value) => Debug.Log(""));
				TextField.tooltip = "This field shows the vehicle list revision date for the Bus, Trolley Bus, Fire and Police\n" +
                                    "trailers, which are in real life industry trailers, but have been re-categorized by AVO.";
				TextField.readOnly = true;

 // Support Section with Wiki and Output-Log	

                UIHelperBase group_support = helper.AddGroup("Support");
				
				Button = (UIButton) group_support.AddButton("Open the Advanced Vehicle Options Wiki", () =>
                {
                    SimulationManager.instance.SimulationPaused = true;
                    Application.OpenURL("https://github.com/CityGecko/CS-AdvancedVehicleOptions/wiki");
                });
				Button.textScale = 0.8f;
				
				Button = (UIButton) group_support.AddButton("Open Cities:Skylines log folder (output_log.txt)", () =>
                {
                    Utils.OpenInFileBrowser(Application.dataPath);
                });
				Button.textScale = 0.8f;
			}	
			
            catch (Exception e)
            {
                DebugUtils.Log("OnSettingsUI failed");
                DebugUtils.LogException(e);
            }
        }

        public const string version = "1.9.3";
    }

    public class AdvancedVehicleOptionsUIDLoader : LoadingExtensionBase
    {
        private static AdvancedVehicleOptionsUID instance;

        #region LoadingExtensionBase overrides
        /// <summary>
        /// Called when the level (game, map editor, asset editor) is loaded
        /// </summary>
        public override void OnLevelLoaded(LoadMode mode)
        {
            try
            {
                // Is it an actual game ?
                if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame && mode != LoadMode.NewGameFromScenario)
                {
                    DefaultOptions.Clear();
			        DebugUtils.Log("AVO Incompatible GameMode " + mode);
                    return;
                }

                AdvancedVehicleOptionsUID.isGameLoaded = true;

                if (instance != null)
                {
                    GameObject.DestroyImmediate(instance.gameObject);
                }

                instance = new GameObject("AdvancedVehicleOptionsUID").AddComponent<AdvancedVehicleOptionsUID>();

                try
                {
                    DefaultOptions.BuildVehicleInfoDictionary();
                    VehicleOptions.Clear();
                    DebugUtils.Log("UIMainPanel created");
                }
                catch
                {
                    DebugUtils.Log("Could not create UIMainPanel");

                    if (instance != null)
                        GameObject.Destroy(instance.gameObject);

                    return;
                }

                //new EnumerableActionThread(BrokenAssetsFix);
            }
            catch (Exception e)
            {
                if (instance != null)
                    GameObject.Destroy(instance.gameObject);
                DebugUtils.LogException(e);
            }
        }

        /// <summary>
        /// Called when the level is unloaded
        /// </summary>
        public override void OnLevelUnloading()
        {
            try
            {
                DebugUtils.Log("Restoring default values");
                DefaultOptions.RestoreAll();
                DefaultOptions.Clear();

                if (instance != null)
                    GameObject.Destroy(instance.gameObject);

                AdvancedVehicleOptionsUID.isGameLoaded = false;
            }
            catch (Exception e)
            {
                DebugUtils.LogException(e);
            }
        }
        #endregion
    }

    public class AdvancedVehicleOptionsUID : MonoBehaviour
    {
        public const string settingsFileName = "AdvancedVehicleOptionsUID";

        public static SavedBool hideGUI = new SavedBool("hideGUI", settingsFileName, false, true);
        public static SavedBool onLoadCheck = new SavedBool("onLoadCheck", settingsFileName, true, true);
        public static SavedBool GameBalanceOptions = new SavedBool("GameBalanceOptions", settingsFileName, false, true);
        public static SavedBool SpeedUnitOption = new SavedBool("SpeedUnitOption", settingsFileName, false, true);
        public static SavedBool SpawnControl = new SavedBool("SpawnControl", settingsFileName, true, true);    // Internal use only
        public static SavedBool OverrideVCX = new SavedBool("OverrideVCX", settingsFileName, true, true);
        public static SavedBool OverrideCompatibilityWarnings = new SavedBool("OverrideCompatibilityWarnings", settingsFileName, true, true);
        public static SavedBool ControlTruckDelivery = new SavedBool("ControlTruckDeliver", settingsFileName, true, true);

        private static GUI.UIMainPanel m_mainPanel;

        private static VehicleInfo m_removeInfo;
        private static VehicleInfo m_removeParkedInfo;

        private const string m_fileName = "AdvancedVehicleOptionsUID.xml";

        public static bool isGameLoaded = false;
        public static Configuration config = new Configuration();

        public void Start()
        {
            try
            {
                // Loading config
                AdvancedVehicleOptionsUID.InitConfig();

                if (AdvancedVehicleOptionsUID.onLoadCheck)
                {
                    AdvancedVehicleOptionsUID.CheckAllServicesValidity();
                }

                m_mainPanel = GameObject.FindObjectOfType<GUI.UIMainPanel>();
                UpdateGUI();
            }
            catch (Exception e)
            {
                DebugUtils.Log("UI initialization failed.");
                DebugUtils.LogException(e);

                GameObject.Destroy(gameObject);
            }
        }

        public static void UpdateGUI()
        {
            if (!isGameLoaded) return;

            if (!hideGUI && m_mainPanel == null)
            {
                // Creating GUI
                m_mainPanel = UIView.GetAView().AddUIComponent(typeof(GUI.UIMainPanel)) as GUI.UIMainPanel;
            }
            else if (hideGUI && m_mainPanel != null)
            {
                //m_mainPanel.enabled = false;	
                GameObject.Destroy(m_mainPanel.gameObject);
                m_mainPanel = null;
            }
        }

        /// <summary>
        /// Init the configuration
        /// </summary>
        public static void InitConfig()
        {
            // Store modded values
            DefaultOptions.StoreAllModded();

            if (config.data != null)
            {
                config.DataToOptions();

                // Remove unneeded options
                List<VehicleOptions> optionsList = new List<VehicleOptions>();

                for (uint i = 0; i < config.options.Length; i++)
                {
                    if (config.options[i] != null && config.options[i].prefab != null) optionsList.Add(config.options[i]);
                }

                config.options = optionsList.ToArray();
            }
            else if (File.Exists(m_fileName))
            {
                // Import config
                ImportConfig();
                return;
            }
            else
            {
                DebugUtils.Log("No configuration found. Default values will be used.");
            }

            // Checking for new vehicles
            CompileVehiclesList();

            // Checking for conflicts
            DefaultOptions.CheckForConflicts();

            // Update existing vehicles
            SimulationManager.instance.AddAction(VehicleOptions.UpdateCapacityUnits);
            SimulationManager.instance.AddAction(VehicleOptions.UpdateBackEngines);

            DebugUtils.Log("Configuration initialized");
            LogVehicleListSteamID();
        }

        /// <summary>
        /// Import the configuration file
        /// </summary>
        public static void ImportConfig()
        {
            if (!File.Exists(m_fileName))
            {
                DebugUtils.Log("Configuration file not found.");
                return;
            }

            config.Deserialize(m_fileName);

            if (config.options == null)
            {
                DebugUtils.Log("Configuration empty. Default values will be used.");
            }
            else
            {
                // Remove unneeded options
                List<VehicleOptions> optionsList = new List<VehicleOptions>();

                for (uint i = 0; i < config.options.Length; i++)
                {
                    if (config.options[i] != null && config.options[i].prefab != null) optionsList.Add(config.options[i]);
                }

                config.options = optionsList.ToArray();
            }

            // Checking for new vehicles
            CompileVehiclesList();

            // Checking for conflicts
            DefaultOptions.CheckForConflicts();

            // Update existing vehicles
            SimulationManager.instance.AddAction(VehicleOptions.UpdateCapacityUnits);
            SimulationManager.instance.AddAction(VehicleOptions.UpdateBackEngines);

            DebugUtils.Log("Configuration imported");
            LogVehicleListSteamID();
        }

        public static void ResetConfig()
        {  
            // Checking for conflicts
            DefaultOptions.CheckForConflicts();

            // Update existing vehicles
            SimulationManager.instance.AddAction(VehicleOptions.UpdateCapacityUnits);
            SimulationManager.instance.AddAction(VehicleOptions.UpdateBackEngines);

            DebugUtils.Log("Configuration reset");
        }

        /// <summary>
        /// Export the configuration file
        /// </summary>
        public static void ExportConfig()
        {
            config.Serialize(m_fileName);
			DebugUtils.Log("Configuration exported");
			
            // display a message for the user in the panel			
            ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
            panel.SetMessage("Advanced Vehicle Options", "Your configuration has been exported. \n\nThe configuration file can be found here:\n<Steam Install Path>/steamapps/common/\nCities_Skylines/AdvancedVehicleOptionsUID.xml", false);
        }

        public static void CheckAllServicesValidity()
        {
            string warning = "";

            for (int i = 0; i < (int)VehicleOptions.Category.Natural; i++)
                if (!CheckServiceValidity((VehicleOptions.Category)i)) warning += "- " + GUI.UIMainPanel.categoryList[i + 1] + "\n";

            if(warning != "")
            {
                GUI.UIWarningModal.instance.message = "The following services may not work correctly because no vehicles are allowed to spawn :\n\n" + warning;
                UIView.PushModal(GUI.UIWarningModal.instance);
                GUI.UIWarningModal.instance.Show(true);
            }

        }

        public static bool CheckServiceValidity(VehicleOptions.Category service)
        {
            if (config == null || config.options == null) return true;

            int count = 0;

            for (int i = 0; i < config.options.Length; i++)
            {
                if (config.options[i].category == service)
                {
                    if(config.options[i].enabled) return true;
                    count++;
                }
            }

            return count == 0;
        }

        public static void ClearVehicles(VehicleOptions options, bool parked)
        {
            if (parked)
            {
                if(options == null)
                {
                    SimulationManager.instance.AddAction(ActionRemoveParkedAll);
                    return;
                }
                
                m_removeParkedInfo = options.prefab;
                SimulationManager.instance.AddAction(ActionRemoveParked);
            }
            else
            {
                if (options == null)
                {
                    SimulationManager.instance.AddAction(ActionRemoveExistingAll);
                    return;
                }

                m_removeInfo = options.prefab;
                SimulationManager.instance.AddAction(ActionRemoveExisting);
            }
        }

        public static void ActionRemoveExisting()
        {
            VehicleInfo info = m_removeInfo;
            Array16<Vehicle> vehicles = VehicleManager.instance.m_vehicles;

            for (ushort i = 0; i < vehicles.m_buffer.Length; i++)
            {
                if (vehicles.m_buffer[i].Info != null)
                {
                    if (info == vehicles.m_buffer[i].Info)
                        VehicleManager.instance.ReleaseVehicle(i);
                }
            }
        }

        public static void ActionRemoveParked()
        {
            VehicleInfo info = m_removeParkedInfo;
            Array16<VehicleParked> parkedVehicles = VehicleManager.instance.m_parkedVehicles;

            for (ushort i = 0; i < parkedVehicles.m_buffer.Length; i++)
            {
                if (parkedVehicles.m_buffer[i].Info != null)
                {
                    if (info == parkedVehicles.m_buffer[i].Info)
                        VehicleManager.instance.ReleaseParkedVehicle(i);
                }
            }
        }

        public static void ActionRemoveExistingAll()
        {
            for (ushort i = 0; i < VehicleManager.instance.m_vehicles.m_buffer.Length; i++)
            {
                VehicleManager.instance.ReleaseVehicle(i);
            }
        }

        public static void ActionRemoveParkedAll()
        {
            for (ushort i = 0; i < VehicleManager.instance.m_parkedVehicles.m_buffer.Length; i++)
            {
                VehicleManager.instance.ReleaseParkedVehicle(i);
            }
        }

        private static int ParseVersion(string version)
        {
            if (version.IsNullOrWhiteSpace()) return 0;

			int a;
            int v = 0;
            string[] t = version.Split('.');

            for (int i = 0; i < t.Length; i++)
            {
                v *= 100;
                if (int.TryParse(t[i], out a))
                    v += a;
            }

            return v;
        }

        // <summary>
        // Check if there are new vehicles and add them to the options list
        // </summary>
        private static void CompileVehiclesList()
        {
            List<VehicleOptions> optionsList = new List<VehicleOptions>();
            if (config.options != null) optionsList.AddRange(config.options);

            for (uint i = 0; i < PrefabCollection<VehicleInfo>.PrefabCount(); i++)
            {
                VehicleInfo prefab = PrefabCollection<VehicleInfo>.GetPrefab(i);

                if (prefab == null || ContainsPrefab(prefab)) continue;

                // New vehicle
                VehicleOptions options = new VehicleOptions();
                options.SetPrefab(prefab);

                optionsList.Add(options);
            }

            if (config.options != null)
                DebugUtils.Log("Found " + (optionsList.Count - config.options.Length) + " new vehicle(s)");
            else
                DebugUtils.Log("Found " + optionsList.Count + " new vehicle(s)");

            config.options = optionsList.ToArray();

        }

        private static bool ContainsPrefab(VehicleInfo prefab)
        {
            if (config.options == null) return false;
            for (int i = 0; i < config.options.Length; i++)
            {
                if (config.options[i].prefab == prefab) return true;
            }
            return false;
        }

        private static void LogVehicleListSteamID()
        {
            StringBuilder steamIDs = new StringBuilder("Vehicle Steam IDs : ");

            for (int i = 0; i < config.options.Length; i++)
            {
                if (config.options[i] != null && config.options[i].name.Contains("."))
                {
                    steamIDs.Append(config.options[i].name.Substring(0, config.options[i].name.IndexOf(".")));
                    steamIDs.Append(",");
                }
            }
            steamIDs.Length--;

            DebugUtils.Log(steamIDs.ToString());
        }

        private static bool IsAICustom(VehicleAI ai)
        {
            Type type = ai.GetType();
            return (type != typeof(AmbulanceAI) ||
                type != typeof(BusAI) ||
                type != typeof(CargoTruckAI) ||
                type != typeof(FireTruckAI) ||
                type != typeof(GarbageTruckAI) ||
                type != typeof(HearseAI) ||
                type != typeof(PassengerCarAI) ||
                type != typeof(PoliceCarAI)) ||
				type != typeof(TaxiAI) ||
				type != typeof(TramAI) ||
				type != typeof(MaintenanceTruckAI) ||
				type != typeof(WaterTruckAI) ||
				type != typeof(ParkMaintenanceVehicleAI) ||
				type != typeof(SnowTruckAI) ||
				type != typeof(CableCarAI) ||
				type != typeof(TrolleybusAI) ||
				type != typeof(PassengerFerryAI) ||
				type != typeof(PassengerBlimpAI) ||
				type != typeof(PostVanAI) ||
				type != typeof(PassengerHelicopterAI);
        }
    }
}
