# CS-AdvancedVehicleOptions 1.9.0

This version is 1.9.0, tested in a limited environment only. Last changes include a new logo (yeah) und the change of the namespace for the mod. This should stop annyoing messages when user are still subscribed to the original one. 

26/03 Still error messages on load; should be resolved if moving to Harmony 2.0 / have no idea, how to do that :/
26/03 New vehicles to be added for new DLC Sunset Harbor

# CS-AdvancedVehicleOptions

This version is 1.8.7, incorporating airatin's updates and additional changes intended to make this mod compatible with More Vechicles by @dymanoid, most importantly using ```Array16<T>::m_buffer.Length``` instead of ```Array16<T>::m_size``` to iterate over the filled length of ```VehicleManager.instance.m_vehicles``` and ```VehicleManager.instance.m_parkedVehicles``` to circumvent the use of preset vehicle limits.

# Testing

I've seen a normal frequency of functional cargo trains in multiple cities of different sizes while using this version locally with More Vehicles and many other mods (to be updated), but please test further before incorporating into the workshop version.
