using ICities;
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
            get { return "Customize your vehicles (for Cities Skylines Sunset Harbor 1.13.0-f7)."; }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            try
            {
                UICheckBox checkBox;	
				UIHelperBase group_general = helper.AddGroup("General Settings                                                " + Name);

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
                checkBox.tooltip = "Hide the UI completely if you feel like you are done with it and want to\nsave the little bit of memory it takes. Everything else will still be functional.";

                checkBox = (UICheckBox)group_general.AddCheckbox("Disable warning for no available services at map loading", !AdvancedVehicleOptionsUID.onLoadCheck.value, (b) =>
                {
                    AdvancedVehicleOptionsUID.onLoadCheck.value = !b;
                });
                checkBox.tooltip = "Disable the check for missing service vehicles assigned in any category when loading a map.";

                UIHelperBase group_balance = helper.AddGroup("Game Balancing");
				
                checkBox = (UICheckBox)group_balance.AddCheckbox("Enable capacity values for non-cargo and non-passenger vehicles", AdvancedVehicleOptionsUID.GameBalanceOptions.value, (b) =>
                {
                    AdvancedVehicleOptionsUID.GameBalanceOptions.value = b;
                });
                checkBox.tooltip = "Allows changes the Firefighting Rate for Fire Engines, the Crime Rate for Police Cars\nand the Maintenance Rate for Maintenance Vehicles. Can de-balance the intended gameplay.";
				
				
				UIHelperBase group_compatibility= helper.AddGroup("Compatibility");
				
                checkBox = (UICheckBox) group_compatibility.AddCheckbox("Hide the Spawn Control for game-controlled Public Transport vehicles", AdvancedVehicleOptionsUID.SpawnControl.value, (b) =>
                {
                    AdvancedVehicleOptionsUID.SpawnControl.value = b;
                });
				
                checkBox.tooltip = "Disable the Spawn Control for Bus, Trolley Bus, Tram, Monorail and Metro\nas Cities Skylines will control the spawning in the game's line manager.\n\nAVO is unable to control the vehicle spawning for these since DLC Campus.";				

                checkBox = (UICheckBox) group_compatibility.AddCheckbox("Vehicle Color Expander: Ignores AVO Vehicle Coloring", AdvancedVehicleOptionsUID.OverrideVCX.value, (b) =>
                {
                    AdvancedVehicleOptionsUID.OverrideVCX.value = b;
                });
				
                checkBox.tooltip = "Permanent setting, if Vehicle Color Expander (by Klyte) is active.\nThe color management is controlled by Vehicle Color Expander.\n\nAVO will not allow user to edit any vehicle colors.";	
				checkBox.readOnly = true;
				checkBox.label.textColor = Color.gray;
				
				if (!VCXCompatibilityPatch.IsVCXActive())
				{
 			        checkBox.label.text = "Vehicle Color Expander: Mod is not active";
				}
				
                checkBox = (UICheckBox) group_compatibility.AddCheckbox("Transport Lines Manager: Ignores AVO Spawn Control", AdvancedVehicleOptionsUID.OverrideVCX.value, (b) =>
                {
                    AdvancedVehicleOptionsUID.OverrideTLM.value = b;
                });
				
			checkBox.tooltip = "Permanent setting, if Transport Lines Manager (by Klyte) is active.\nIf user wants to change these values, change must be done in Transport Lines\nManager.Spawning of Public Transport Vehicles is permanently disabled.\n\nAVO will not allow user to edit Capacity values for Public Transport vehicles.";				
				checkBox.readOnly = true;
				checkBox.label.textColor = Color.gray;
				
				if (!TLMCompatibilityPatch.IsTLMActive())
				{
				    checkBox.label.text = "Transport Line Manager: Mod is not active";	
				}
				
                checkBox = (UICheckBox) group_compatibility.AddCheckbox("Improved Public Transport: Overwrites AVO vehicle parameters", AdvancedVehicleOptionsUID.OverrideIPT.value, (b) =>
                {
                    AdvancedVehicleOptionsUID.OverrideIPT.value = b;
                });
				
                checkBox.tooltip = "If enabled, Improved Public Transport (by BloodyPenguin) will manage\nCapacity and Maximum Speed values for Public Transport Vehicles.\nSpawn Control for Public Transport Vehicles is permanently disabled.\nIf user wants to change these values, change must be done in Improved\nPublic Transport.\n\nAVO will not allow user to edit Capacity and Maximum Speed values\nfor Public Transport vehicles.";
				
				if (!IPTCompatibilityPatch.IsIPTActive())
				{
        			checkBox.readOnly  = !IPTCompatibilityPatch.IsIPTActive();
				    checkBox.label.text = "Improved Public Transport: Mod is not active";
					checkBox.label.textColor = Color.gray;
			    }
		
			}	
            catch (Exception e)
            {
                DebugUtils.Log("OnSettingsUI failed");
                DebugUtils.LogException(e);
            }
        }

        public const string version = "1.9.0a";
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
		public static SavedBool SpawnControl = new SavedBool("SpawnControl", settingsFileName, true, true);
		public static SavedBool OverrideVCX = new SavedBool("OverrideVCX", settingsFileName, true, true);
		public static SavedBool OverrideTLM = new SavedBool("OverrideTLM", settingsFileName, true, true);
		public static SavedBool OverrideIPT = new SavedBool("OverrideIPT", settingsFileName, true, true);

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
            if(!isGameLoaded) return;
            
            if(!hideGUI && m_mainPanel == null)
            {
                // Creating GUI
                m_mainPanel = UIView.GetAView().AddUIComponent(typeof(GUI.UIMainPanel)) as GUI.UIMainPanel;
            }
            else if (hideGUI && m_mainPanel != null)
            {
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

            if(config.data != null)
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
            new EnumerableActionThread(VehicleOptions.UpdateCapacityUnits);
            new EnumerableActionThread(VehicleOptions.UpdateBackEngines);

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
            new EnumerableActionThread(VehicleOptions.UpdateCapacityUnits);
            new EnumerableActionThread(VehicleOptions.UpdateBackEngines);
            
            DebugUtils.Log("Configuration imported");
            LogVehicleListSteamID();
        }

        /// <summary>
        /// Export the configuration file
        /// </summary>
        public static void ExportConfig()
        {
            config.Serialize(m_fileName);
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
                    new EnumerableActionThread(ActionRemoveParkedAll);
                    return;
                }
                
                m_removeParkedInfo = options.prefab;
                new EnumerableActionThread(ActionRemoveParked);
            }
            else
            {
                if (options == null)
                {
                    new EnumerableActionThread(ActionRemoveExistingAll);
                    return;
                }

                m_removeInfo = options.prefab;
                new EnumerableActionThread(ActionRemoveExisting);
            }
        }

        public static IEnumerator ActionRemoveExisting(ThreadBase t)
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

                if (i % 256 == 255) yield return i;
            }
        }

        public static IEnumerator ActionRemoveParked(ThreadBase t)
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

                if (i % 256 == 255) yield return i;
            }
        }

        public static IEnumerator ActionRemoveExistingAll(ThreadBase t)
        {
            for (ushort i = 0; i < VehicleManager.instance.m_vehicles.m_buffer.Length; i++)
            {
                VehicleManager.instance.ReleaseVehicle(i);
                if (i % 256 == 255) yield return i;
            }
        }

        public static IEnumerator ActionRemoveParkedAll(ThreadBase t)
        {
            for (ushort i = 0; i < VehicleManager.instance.m_parkedVehicles.m_buffer.Length; i++)
            {
                VehicleManager.instance.ReleaseParkedVehicle(i);
                if (i % 256 == 255) yield return i;
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

        /// <summary>
        /// Check if there are new vehicles and add them to the options list
        /// </summary>
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
