# CS-AdvancedVehicleOptions 1.9.0a
This version is 1.9.0a, tested in a limited environment only. Adds compatibility patches to the game (vehicle spawning), ITP/TLM (vehicle spawning, vehicle parameters), Vehicle Color Expander (coloring parameters).

New Options are visible in the Mod Option panel of Cities Skylines.
- Game Balancing : Hide capacity value for vehicles without passenger/cargo capacity; can be turned off, default setting: ON
- Compatibility : Hide Spawn option for game controlled vehicles (eg bus); can be turned off, default setting: ON
- Compatibility : If Color Vehicle Expander is present and active, disable Coloring; cannot be turned off, default setting: ON
- Compatibility : If TLM is present and active, disable some vehicle parameters; cannot be turned off, default setting: ON
- Compatibility : If IPT is present and active, disable some vehicle parameters; can be turned off, default setting: ON

# CS-AdvancedVehicleOptions 1.9.0

This version is 1.9.0, tested in a limited environment only. Last changes include a new logo (yeah) und the change of the namespace for the mod. This should stop annyoing messages when user are still subscribed to the original one. 

26/03 Still error messages on load
26/03 New vehicles added for new DLC Sunset Harbor

# CS-AdvancedVehicleOptions

This version is 1.8.7, incorporating airatin's updates and additional changes intended to make this mod compatible with More Vechicles by @dymanoid, most importantly using ```Array16<T>::m_buffer.Length``` instead of ```Array16<T>::m_size``` to iterate over the filled length of ```VehicleManager.instance.m_vehicles``` and ```VehicleManager.instance.m_parkedVehicles``` to circumvent the use of preset vehicle limits.

# Testing

I've seen a normal frequency of functional cargo trains in multiple cities of different sizes while using this version locally with More Vehicles and many other mods (to be updated), but please test further before incorporating into the workshop version.
