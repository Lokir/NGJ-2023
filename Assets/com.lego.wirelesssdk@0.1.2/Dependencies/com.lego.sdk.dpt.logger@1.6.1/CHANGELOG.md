# Changelog
All notable changes to this project will be documented in this file.

## [1.6.1] - 2022-09-14
### Added
- Added ability to choose split lineup length as a default parameter (LPAF-1758)
- 
## [1.6.0] - 2022-09-08
### Added
- Added support for unity 2021. (LPAF-1726)

## [1.5.1] - 2022-07-07
### Removed
- Removed support for other threads than the Unity main thread to log messages. (LPAF-1626) (Originally added in version 1.5.0 as LPAF-1486)

## [1.5.0]
### Changed
- Message logging can now be called from other threads, which until now it was only supported on the main Unity thread.

### Added
- FileLogAppender and LogManger accessors and methods for SendLog (LPAF-1441)
- Added support for other threads than the Unity main thread to log messages (LPAF-1486)

## [1.4.2]
### Fixed
- Fixed the scaling of the buttons in the logview, which in turn makes them easier to activate on large DPI devices. (LPAF-1103)
### Changed
- Change displayed text in the Logger to be based on Text Mesh Pro (LPAF-1111).

## [1.4.1]
### Changed
- Modified bundle id to match rest of the LPAF packages.

## [1.4.0]
### Changed
- Bumped the version to 1.4.0 to signal the change to UPM releases.

## [1.3.0]
### Added
- Added new events to listen to for when a packet was sent, received or dropped.

### Fixed
- Fixed a bug where LogMessage prefabs were being instantiated when editor stopped playing, which created ghost gameobjects in the scene heirachy.

## [1.2.1]
### Added
- The Logger now uses a config file created in the client project

### Changed
- LogManager is now disposable

### Fixed
- Fixed a stack trace source identification error in certain cases.
- Fixed broken prefabs
- Fixed ADB log appender

## [1.2.0]
### Changed
- Project is now structured as a package for use with the Unity Package Manager. See [Package layout](https://docs.unity3d.com/2019.1/Documentation/Manual/cus-layout.html).
