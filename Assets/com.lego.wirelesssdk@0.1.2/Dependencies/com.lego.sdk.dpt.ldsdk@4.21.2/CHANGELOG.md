# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

**You should also consider the CHANGELOG.md of the embedded CoreUnityBluetoothBridge.**

## [4.21.2] - 2022-10-20
### Fixed
- Fixed announcement on Teams channel when CI releases a build.

## [4.21.1] - 2022-10-10
### Added
- Added support for Android 12 permissions, for first time scanning in the LDSDK Test App

## [4.21.0] - 2022-09-08
### Added
- Added support for unity 2021. (LPAF-1726)
- Ability to send OAD transfer informatio0n as .csv format data to the log (LPAF-1758)
  
### Changed
- Refactored IOType enum, added commentary for adding hardware_type_ids to reporter block descriptions. Commented out a spamming debug log report. (LPAF-1065)


## [4.20.0] - 2022-07-12
### Removed
- Removed enabling of multithreading for AOD transfers. The functionality was introduced in version 4.19.0 (as LPAF-1550) and it is being reverted. (LPAF-1626)

### Changed
- LDSDK package dependency to CUBB updated to 1.12.1
- LDSDK package dependency to Logger updated to 1.5.1

## [4.19.1] - 2022-07-6
### Changed
- Redid the UI for flashing firmware. When opening the device info-panel you can now explicitly choose between firmware from URLs and from device. The dropdown will also attempt to mark the currently installed firmware version (which is not possible for bootloader devices). (LPAF-1012)
- Setup of the Conection Center now happens using a ConnectionCenterConfiguration. Settings to the ConnectionCenter can be applied here, before or after creating the ConnectionCenter-instance. In either case, the CC-instance is linked to, and kept up-to-date by, the configuration. (LPAF-1427)

### Added
- Support for the Gecko 3x3 LED Matrix. (LPAF-1219)
- Hublang testscripts for the LED Matrix. (LPAF-1219)
- Added Service for Gecko Hub charging light and Gecko Hub gesture sensor. Also updated hublang tests. (LPAF-1379)
- Checked Gecko motor color and add missing sensor IOTypes (LPAF-1218)
- Added Gecko Hub (Hub131) to list of known devices, and created it's portmap. (LPAF-1380) 
- Added functionality for OAD transfers to have "WriteWithResponse" toggled on during FW transfers to mitigate excessive duplicate packets being sent caused by slow device (i.e. Kindle/Nokia devices) (LPAF-1417)

### Fixed
- Cramped text when failing to connect to bootloader. (LPAF-1014)
- Problem where firmware menu can't be opened when connected to a hub that has no new firmware to upload. (LPAF-1423)
- Fixed an issue where when editing a connected device's name on Android device the user was unable to see the written text. (LPAF-1299)

## [4.18.2] - 2022-02-1
### Added
- Added functionality for OAD transfers to have "WriteWithResponse" toggled on during FW transfers to mitigate excessive duplicate packets being sent caused by slow device (i.e. Kindle/Nokia devices) (LPAF-1417)
- after return from CC, stop auto scanning enabled from CC - (LPAF-1368)

## [4.18.1] - 2021-12-3
### Added
- Added the `NSubstitute` library to the LDSDK package. (LPAF-1338)
- Hub compatibility validator. (LPAF-1290)
- New feature: Ignore failed interrogation events. This is implemented in `LEGODeviceManager.Initialize`. `LEGODeviceManager.Initialize` signature has two changes made to it.
    - It now returns the instance of `ILEGODeviceManager`
    - It now takes a parameter `ILEGODeviceConfig` which is a simple data class. When the `ignoreFailedInterrogationEvents` on `ILEGODeviceConfig` is set to true, the LDSDK will not trigger the failed interrogation events, both internal and external. Hence, allowing to use a device that isn't successfully interrogated. (LPAF-1224)

### Fixed
- Hub66 (handset) no longer automatically reconnects after disconnect if autoconnect is enabled. (LPAF-1208v2)
- Fixed issue with the "elapsed time" not showing in the LDSDK Test App when flashing a hub. (LPAF-1277)

### Changed
- dependencies updated - CUBB 1.9.0

## [4.18.0]
### Fixed
- Fixed issue with reconnecting a remote control hub (hub66). (LPAF-1208)

### Changed
- The ConnectionCenter has been migrated from the LCC codebase to the LDSDK. It is now possible to add the Connection Center package to a project which depends on the LDSDK package. 
- DeviceAddress has been moved down to the LDSDK. It was previously a defined  in the LCC.



## [4.17.0] - 2021-10-7
### Added
- Added support for unity LTS 2020. Tested on 2020.3.13f1 and 2019.4.2f1
- A timeout has been added to whenflashing and UpdateStartMode.ViaRequestEnterBootMode is selected for a LEAF hub. Without this timeout, flashing would sometimes hang and never start. (LPAF-1199)
- Added LEAF hub69 (Vega) to devices (LPAF-1162).
- Pop up dialogue for connection timeout when downloading firmware/assetpack.

### Removed
- The mode FRIENDSHIP_CODE in the FriendShipService has been removed all together, and is no longer available.(LPAF-1253)

### Fixed
- timing issues withg scan start/stop LPAF-1117
- Loading screen not blocking input
- fix null reference exceptions in VSM (LPAF-714)

### Changed
- Updated CUBB dependency to 1.8.1
- In the LDSDK test app, what previously was just the "Flash" button, now toggles between starting or stopping a flashing of a device. (LPAF-1199)
- DeviceAddress has been moved down to the LDSDK. It was previously a concept in the LCC.
- It is now possible to add the Connection Center package to a project which uses the LDSDK package. 


## [4.15.0] - 2021-06-17
### Added
- Added VCC off command sent before FW update (LPAF-164).

### Changed
- Exposed onCompletion in LEGODeviceManager.FlashFirmware, so clients can get a callback when the entire FW transfer process is complete including any optional auto-connect handled by the LDSDK (LPAF-1016).
- Moved permissions check in LDSDK test app to be actioned when the start scan button is pressed.

## [4.14.0] - 2021-06-02
### Changed
- Now using UPM to include the Logger package - (LPAF-667) - Remember to delete the local package if the Unity package resolver complains
- Exposed event for OnManufacturerDataUpdated on LEGODevice, for clients to get changes on ManufacturerData on advertising.
- Added LEAF M2M friendship service (LPAF-765/881) 
- Reduced No Ack parameters for OAD devices to support faster transfer speeds
- Introduced a MainThreadDispatcher to off load internal events to a more proper time

### Added
- Added internal current and voltage sensors to the port mapping

## [4.13.0] - 2021-04-20
### Changed
- Now using UPM to include the Logger package - (LPAF-667) - Remember to delete the local package if the Unity package resolver complains
- The above change means all developers now need an access token before being able to pull UPM packages from the new scoped registry. Contact Martin Bo Laursen or Timothy James Taylor-Bowden for a new token.
- Exposed event for OnManufacturerDataUpdated on LEGODevice, for clients to get changes on ManufacturerData on advertising. (LPAF-764)
- Added LEAF M2M friendship service (LPAF-765) 

### Added
- Added internal current and voltage sensors to the port mapping


## [4.12.0] - 2021-02-05 
### Added
- Support for new mindstorms sensors (LPAF-311)
  - color (61) (NB: not using combined mode)
  - distance (62)
  - force (63)
- Don't create services for devices that are not supported by the hub type. (LPAF-616)

### Fixed
- iOS stop scan ScanStateChanged wasn't always sent. (LPAF-304)

### Changed
- iOS: LEAF transfer speed improved (LPAF-269 + LPAF268)

## [4.11.0] - 2020-11-26 
### Added
- ability to compare for a LEAF device group (hub67 and hub68)

## [4.10.1] - 2020-11-09 
### Fixed
- Fixed inability to connect to old hub64 FW (LPAF-178)

## [4.10.0] - 2020-10-16 
### Fixed
- Fixed angular technic motors not working on the technic hub (LCC-2712)

## [4.9.0] - 2020-10-06
### Changed
- Updated dependency to CUBB submodule to version 1.6.1 (commit SHA: 1f53c1)
- Updated dependency to LDSDK submodule to version 4.9.0 (commit SHA: d12df0) 
- Updated dependency to Logger submodule to version 1.3.0 (commit SHA: 7c1265) 
 - Send strategy for hub64 changed to hard ack from soft ack to prevent interrogation failures (LCC-2430)
 - Hub alerts checked to see if supported by hub being connected to in interrogation phase (LCC-2509)
 - End interrogation phase gracefully if a disconnect occurs during (LCC-2601)
 - Forward port issues found in release/4.7.2 with LCC-2601 (LCC-2691)

## [4.8.0] - 2020-08-25
### Important
- Updated the Unity version to Unity version 2019.4.2f1 LTS


## [4.7.1] - 2020-08-18
### Fixed
- Fixed an issue where the LEAF hub would disconnect automatically after connecting with application (LCC-2518)

## [4.7.0] - 2020-08-5
### Changed
- Interrogation phase failures are communicated to the client, after which an attempt to disconnect the device is attempted (LCC-2067)
- Interrogation phase unresponded to requests are retried (LCC-2348)
- Moved LDSDK test app part of changelog and readme to seperate files in the test app repo 
- Updated dependency to CUBB submodule to version 1.5.2 (commit SHA: 81fe5a)
- Updated dependency to LDSDK submodule to version 4.7.0 (commit SHA: d11bf5)
- Updated dependency to Logger submodule to version 1.2.1 (commit SHA: f3d45e)
- Updated dependency to BuildTools submodule to version 0.1.0 (commit SHA: e59866)
- Updated dependency to LEAF-Shared submodule to version 0.1.0 (commit SHA: ac674b)
- Updated dependency to LEAF-Asset-Pack submodule to version 0.1.0 (commit SHA: b66498)
- Updated dependency to LEAF-LDSDK-Extension submodule to version 0.1.0 (commit SHA: 52b6ea)
  
### Fixed
 - prevent DeviceInfo information requests before interrogation finished from provoking null reference exceptions (2325)
 - Fix bug where an exception was thrown, if the SDK was given an empty new hub name.
 - Fixed a bug where attempting to re-connect to a device already in LEGO Bootloader mode would fail.

## [4.6.0]- 2020-03-27
### Changed
- The LDSDK has been restructured as a Unity package that can be used via Unity's new package system.

### Fixed
 - Introduced new callback to handle the disconnect from shutdown hubs seperately
 - Improved transfer speeds for LEAF hub using varying MTU's
 - Fixed issues with error output not be displayed for failed LEAF transfers.
 - Fixed issue with Wedo tilt sensor not working.
 
### Added
 - Added callback WIllShutDown to distinguishing between disconnect and shutdown events, can be used to take appropriate actions for when device is shutting down (received ack response from shutdown message sent to hub)

## [4.5.0] - 2020-02-07
### Fixed
- Temporary fix (1875) until 1886 is implemented backported from LCCs LDSDK
- Added a LEAF Asset Pack Extension utility method to check if there is an update available compared to a hub
- Matched changes to LEAF game engine FW. 

### Added
- property set volume changed id from 0x10 to 0x12 + two new rechargeable battery properties added (0x10, 0x11)
- testing of the LEAF game engine VERS mode in LDSDK example app

## [4.4.0] - 2019-12-06
### Added
 - Added ability to initiate asset/FW transfer to LEAF hub with a "get ready" - "ready" exchange before starting the transfer (LCC-1845)
 - instrumentation of OAD aset transfer process for later adaptable comms tuning.
 - now don't ask for device type from the LEAF OAD before we actually want to start a transfer, otherwise the file system is closed and no animations/sounds can be played after connection. 
 - LEAF Asset Pack API in the form of the AssetPack class. 
 - flipper angled motors small, medium and large (65,48,49)
 - LEAF Asset Pack Extension has been added as the API surface meant for client developers  
 - Added support for Technic motors and sensors, technic Flipper angled motors small, medium and large (65,48,49) and technic Gecko motors.

 
### Fixed 
 - stubbed game engine service modes
 - multiple connect/disconnect attempts stop virtual motor functionality - (LCC-1800) backport from LCC
 - made virtual motor pair ports dynamic to match FW dynamic port issue - LCC-1860
 
### Changed
- LEAF game engine service ID moved from 75 to 70

## [4.3.3] - 2019-11-01
### Changed
- Pre firmware update check for hub128 is stricter, and will look for any connected UART peripherals (down from 3 or more)
- Added LEAF service classes (LCC-1699)

## [4.3.2] - 2019-09-23
### Added
- Updated some features needed for LDSDK example app to accomodate for LEAF offsite workshop.
- Support for connecting to dory hub.
- Support for connecting to duplo train.
- Changed all classes, methods, properties and comments referring to TI to prevent anything in the code to be able to indicate association with Texas Instruments
- up-port of latest CUBB and plugins (1.2.0)

### Fixed
- Fixed firmware upgrade process not being stopped on device disconnect. (LCC-1514)



## [4.3.1] - 2019-07-10
### Fixed
- Fixed freeze during connection issue caused by lack of proper handling of lost packets. (LCC-1250)



## [4.3.0] - 2019-07-01
### Added
- Updated version of CUBB to 1.1.0.
- Updated Unity project version to 2018.4.0f1 LTS, your editor should be updated accordingly.
- Updated scripting backend to .NET 4.x.
- Functionality to check the readiness of a hub for being firmware updated.
- Hub alerts subcribed to (low voltage, high current etc.). Hub alerts are on/off switchable via LEGODevice.EnableUpdateValues.



## [4.2.3] - 2019-05-20
### Changed
- When passing a version text e.g. 1.2.3.4 to BuildUtiltities, this will not be interpreted as Revision=3 and Build=4, which is the opposite of before the change. 
  BuildUtilities will handle this internally, so this is only important if using the Version.Build and Version.Revision from a client app.

### Fixed
- Fixed hub "advertising" after shutdown request.



## [4.2.2] - 2019-05-02
### Added
- Added virtual port mapping for hub65 FW from 1.1.0.0 onwards 

### Changed
- For FW version 1.0.0.224 and onwards, virtual port mapping is now initiated by the LDSDK. For LDSDK clients that means that motors can still be addressed in pairs on newer versions of the FW
- Moved DevicePort and integrated appropriate virtual manager features into Deviceport 

### Fixed
- Fixed a bug causing the wrong port-mappings to be used for Hub64 with FW version 1.0.0.223. For LDSDK clients that means that commands could not be sent for internal motors.
- Fixed a problem in in the virtual pairing: 
  Now the pair is always (A, B) and never (B, A), regardless of the order in which motors are connected. 
- Masked an undocumented bit field out of the event completion processing mechanism. 
  The problem was causing completion callbacks for motor commands not to be invoked in some situations.



## [4.2.0] - 2019-02-18
### Added
- Added new port-mapping for post FW v.2.2.3 FW for Boost Hubs. The solution is backwards compatible with older versions of the LDSDK. 
- Added access to the setup and disconnection of virtual motor pairs (fixed in the latest technic hub)
- Added the automatic setup of virtual motor pairs for FW that no longer create their own set.
- Added BoostVM service and associated HubLang testing functions + test
- Added <= and >= comparators to LEGORevision for use by the LCC DevicePort component 

### Fixed
- LEGODevice.EnableValueUpdates() was broken when the parameter 'enable' was false.



## [4.1.0] - 2019-01-28
- Updated project to Unity 2018.2.11f1
- Imported new release of CUBB v. 1.0.0 build with Unity 2018.2.11f1
- Hotfix - added BoostVM support missing for LCC 2.2.1 



## [4.0.1] - 2019-01-22
### Changed
- Accommodating peculiarities of City hubs:
  - Using WriteWithResponse always for normal operation
  - Using WriteWithoutResponse always for bootloader operation
- Refactored SetDefaultColorCommand in LEGORGBLight, so the client now can chose what color index to use as default.
- Enabled the packet replacement mechanism provided by CUBB, and removed the older C#-level packet replacement.

### Fixed
- Fixed length field of ProgramFlashData message to bootloader hubs.
- Fixed RGBLight not starting in the default set color index, after interrogation is completed.
- LDSDK-201 - update devices list before propagating updates. 



## [4.0.0] - 2018-11-14
### Added
- First offical release of the Unity-based LDSDK - the LDSDK has been completely re-written compared to LDSDK 3.2.
  However, the LDSDK 4.0 (Unity) interface is compatible with the interface provided by 3.2.
- This version supports iOS, MacOS and Android (but not yet UWP)
