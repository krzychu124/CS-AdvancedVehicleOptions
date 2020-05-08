# CS-AdvancedVehicleOptions 1.9.3

Vehicle Configuration:
* New - Button for Reset full configuration / all customized settings.
* Enhanced - Export now reports back, where the configuration file is located.
* Fixed - Savegames created with older versions of AVO will no longer fail with error message when loading the vehicle configuration

Vehicle Settings
* Changed - Section for Configuration now called "Actions for Vehicle Configuration".
* Changed - Section "Remove Vehicles" now called "Actions For + Vehicle Type".
* Changed - "Restore Default" is now "Default Values" for a vehicle type. Moved down to Remove Driving & Parked section.
* Changed - Capacity description label will be adapted to what kind of vehicle it is, eg. Public transport will show Passenger while Industry shows Cargo and Postal Service reflects Mail.
* New - Option for display speed units Miles per Hour or Kilometer per Hour.
* Fixed - Full support for Fishing Industry.
* Enhanced - Support for enhanced vehicle properties (eg. pumping rate, travel distance, criminal rate, fishing rate and more).
* Changed - AVO will no longer allow to disable "Bus trailers" from the trailer itself. Reason for that is that the bus trailer is a re-assigned industrial vehicle. However, disabling the main vehicle will disable also the trailer.

Vehicle Locator & Remover
* Enhanced - When pressing SHIFT on the location tag, AVO will directly zoom to the next available vehicle. If SHIFT is not pressed, it will only center to the next available vehicle.
* New - Remove the previously located vehicle.

Mod Compatibility
* Change - AVO will no longer block setting changes, if Improved Public Transport or Transport Lines Manager are present. Instead it will color shared fields in red to warn the user, that this may result in compatibility issues.
* Change - Settings for warnings (triggered by Improved Public Transport and Transport Lines Manager) activate/deactivate the red color coding in shared data fields.
* Removed - Checkbox Options for Improved Public Transport and Transport Lines Manager have been removed.

Vehicle Spawning
* New - Button for accessing directly the Cities Skylines Transport Line Overview panel instead informing the user that "Spawning is controlled by Cities Skylines, IPT or TLM". User can now directly open the Line Overview and change the desired vehicle.
* New - Small help button sending the user to a new section for Spawning Explanation.

Mod Option Panel
* Changed - Compatibility settings in the Mod Option Panel will be only displayed, if the related mod is installed and active (Vehicle Color Extender, IsBigVehicle).

Experimental Mod Support
* New - Mod support NoBigTruck created by MacSergey using the "IsLargeVehicle" flag. You need the mod NoBigTruck installed to make this work. If a vehicle is flagged as "Large Vehicle" the IsBigTruck mod should not send this vehicle to small buildings. Please contact MacSergey for any issues, AVO only sets the flag. Subscribe to the NoBigTruck mod [here](https://steamcommunity.com/sharedfiles/filedetails/?id=2069057130).

Trailer Reference List
* References updated 

# CS-AdvancedVehicleOptions 1.9.2

Change the Mod Option Panel
- Compatibility : If TLM is present and active, disable some vehicle parameters; can be turned off, default setting: ON
- Compatibility : If IPT is present and active, disable some vehicle parameters; can be turned off, default setting: ON

Note: Disabling the option will make the values for editing available, however, the Spawn Control for Bus, Trolley Bus, Metro, Tram and Monorail is still under control of the game, TLM or IPT. The vehicles must be configured in the respective transport line managers.

Lean Modifier and Nod Mulltiplier are not saved if zero or negative value. Now remembers and loads values properly (eg. for trains or helicopters).

Trolley Bus to show next to Bus and Intercity Bus

# CS-AdvancedVehicleOptions 1.9.1
This version is 1.9.1, tested in a limited environment only. Adds compatibility patches to the game (vehicle spawning), ITP/TLM (vehicle spawning, vehicle parameters), Vehicle Color Expander (coloring parameters).

Adds compatibility patches to the game (vehicle spawning), ITP/TLM (vehicle spawning, vehicle parameters), Vehicle Color Expander (coloring parameters).

Adds compatibility for game controlled vehicle spawning (Hide Spawn option for game controlled vehicles (eg bus); if vehicle spawning is controlled by game, the vehicle will always be turned on (even if it was disabled before)

New Options are visible in the Mod Option panel of Cities Skylines.
 - Game Balancing : Hide capacity value for vehicles without passenger/cargo capacity; can be turned off, default setting: ON
 - Compatibility : If Color Vehicle Expander is present and active, disable Coloring; cannot be turned off, default setting: ON
 - Compatibility : If TLM is present and active, disable some vehicle parameters; cannot be turned off, default setting: ON
 - Compatibility : If IPT is present and active, disable some vehicle parameters; cannot be turned off, default setting: ON

New Category for Intercity Bus

Support for categorizing properly Bus trailers. SteamID config file 09-Apr-2020 for 1.9.1.

Hiding GUI works again

Added Mod Option Panel links to Wiki and Log directory 

# CS-AdvancedVehicleOptions 1.9.0

This version is 1.9.0, tested in a limited environment only. Last changes include a new logo (yeah) und the change of the namespace for the mod. This should stop annyoing messages when user are still subscribed to the original one. 

- Issue on load: Still error messages when opening an existing savegame (one time only due to exception on loading values)
- Added new vehicle categories for new DLC Sunset Harbor
- Compatibility towards More Vehicles, no more cargo train issues

# CS-AdvancedVehicleOptions

This version is 1.8.7, incorporating airatin's updates and additional changes intended to make this mod compatible with More Vechicles by @dymanoid, most importantly using ```Array16<T>::m_buffer.Length``` instead of ```Array16<T>::m_size``` to iterate over the filled length of ```VehicleManager.instance.m_vehicles``` and ```VehicleManager.instance.m_parkedVehicles``` to circumvent the use of preset vehicle limits.

# Testing

I've seen a normal frequency of functional cargo trains in multiple cities of different sizes while using this version locally with More Vehicles and many other mods (to be updated), but please test further before incorporating into the workshop version.
