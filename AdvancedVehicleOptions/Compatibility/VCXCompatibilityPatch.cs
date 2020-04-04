using System.Collections.Generic;
using System.Linq;
using ColossalFramework.Plugins;

namespace AdvancedVehicleOptionsUID.Compatibility
{
    public class VCXCompatibilityPatch
    {
        public static bool IsVCXActive()
        {
            return PluginManager.instance.GetPluginsInfo().Where(x => x.isEnabled)
                .Any(plugin => plugin.publishedFileID.AsUInt64 == 1818462177);
        }
        
        public static List<string> AVOFields = new List<string>
        {
            "m_useColorVariations",
            
            
        };
    }
}