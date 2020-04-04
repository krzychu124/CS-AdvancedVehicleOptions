using System.Collections.Generic;
using System.Linq;
using ColossalFramework.Plugins;

namespace AdvancedVehicleOptionsUID.Compatibility
{
    public class IPTCompatibilityPatch
    {
        public static bool IsIPTActive()
        {
            return PluginManager.instance.GetPluginsInfo().Where(x => x.isEnabled)
                .Any(plugin => plugin.publishedFileID.AsUInt64 == 928128676);
        }
        
        public static List<string> AVOFields = new List<string>
        {
            "m_maxSpeed",
			"m_enabled",
			"m_capacity"
            
            
        };
    }
}