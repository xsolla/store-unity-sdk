# Changelog
## [0.6.4.4] - 2021-07-12
### Added
- Horizontal mobile demo scene
### Fixed
- Several payment systems flow for WebGL build
- Pay Station auto-close after purchase for Unity 2020+
- Demo behaviour after social auth cancel
- WeChat errors for android build
- Errors raising once after import for Unity 2020+
- In-built browser render quality
- Included copy of Newtonsoft lib now can be safely removed in case of conflict
- Various other fixes and improvements
### Changed
- API methods update
- Third-party dependency manager is now included into Xsolla package
- Updated third-party Puppeteer Sharp library to match Unity's own Newtonsoft lib

## [0.6.4.3] - 2021-06-10
### Added
- Device ID authorization
- OAuth2.0 state parameter for more API methods
- Added: Paystation UI settings for different build targets
- Added: Additional Paystation UI color themes
- Added: 'Create payment token' method
- Added: Xsolla assembly definitions
### Fixed
- Fixed: In-game browser scroll
- Fixed: In-game browser numpad
- Fixed: In-game browser redirection links
- Fixed: In-game browser post purchase behaviour
- Fixed: In-game browser window size now corresponds with Paystation UI size
- Fixed: Handling of few purchase systems that become incompletable with in-game browser
- Fixed: Various other fixes and improvements
### Changed
- Changed: Paystation opening version 2 -> 3
- Changed: Updated purchase API methods
### Removed
- Removed: Several surplus libraries (reworked corresponding functionality to use custom logic)

## [0.6.4.2] - 2021-04-23
### Added
- Added itmes attributes support
- Added items store/inventory hiding based on HideIn attribute
### Fixed
- Minor fixes

## [0.6.4.1] - 2021-04-16
### Added
- Added QQ Android auth

## [0.6.4.0] - 2021-03-31
### Added
- Battle Pass
- UI Builder
- XboxOne/PS4 support
- Redirect policy settings
- Logging level settings
- API method for resend account confirmation email
### Fixed
 - Payment status polling
 - AppleID authorization
### Changed
- PayStation Widget version

## [0.6.3.4] - 2021-03-18
### Changed
- Minor update

## [0.6.3.3] - 2021-03-05
### Changed
- Added WeChat Android auth

## [0.6.3.2] - 2021-02-05
### Changed
- Minimum supported version of Unity — 2019.4.19f1
- Url of documentation page about user attributes
- Authorization via social networks on android

### Fixed
- Built-in browser for MacOS Big Sur
- Native authorization via Steam
- Automatic change of steam AppId in steam_appid.txt files when changing it in SDK settings
- Various bug fixes and improvements

## [0.6.3.1] - 2020-12-30
### Added
- Ability to pass custom parameters via API
- Access token authentication
- More social networks support
- Hashtag for user identification
- Browser background
- Two more gender options
- Custom parameters for access token auth
- Support for new Unity input system

### Changed
- Refactoring and rework

### Fixed
- Various bug fixes and improvements

## [1.0.0.1] - 2020-10-27 

### Fixed
- Minor bugs.

## [1.0.0.0] - 2020-10-08 

### Added 
- Friends system functionality.