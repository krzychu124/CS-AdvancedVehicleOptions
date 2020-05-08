using System.Collections.Generic;
using System.Linq;
using ColossalFramework.Plugins;

namespace AdvancedVehicleOptionsUID.Compatibility
{
    public class NoBigTruckCompatibilityPatch
    {
        public static bool IsNBTActive()
        {
            return PluginManager.instance.GetPluginsInfo().Where(x => x.isEnabled)
                .Any(plugin => plugin.publishedFileID.AsUInt64 == 2069057130);
        }

        public static List<string> AVOFields = new List<string>
        {
            "m_isLargeVehicle",
        };
    }
}