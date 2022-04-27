# Changelog
## [1.0.0] - 2022-04-20
### Added
- Static SDK events
- Purchase UI via native SDK for Android
- Purchase UI via native SDK for iOS
- SDK autotest
- Scripted package export
- Demo: UI skin selection
### Fixed
- Request error parsing and handling
- OAuth2.0 token refresh
- Use of redirect URL settings in several API calls
- User's inventory items that were removed from a catalog
- Demo: Catalog and inventory initialization
- Other fixes and improvements
### Changed
- Updated wrapper methods for IGS & BB API calls
- Updated wrapper methods for Login API calls
- Separated 'Xsolla' and 'Xsolla.Demo'
- Separated 'Login' into several modules
- Separated 'Inventory' and 'Store' into several modules
- Updated Android libraries
- Reworked SDK settings
- Made naming of several SDK methods more consistent
- Demo: Moved user attributes to the 'User Account' page ('Character' page removed)
### Removed
- Demo: Battle pass prototype
- Demo: Email confirmation window if confirmation disabled in project settings
- Demo: Custom auth demo

## [0.7.1] - 2021-12-03
### Added
- 'Web Shop' button in demo
- Passwordless login via email
- Passwordless login via SMS
- Auth via FaceID/TouchID for iOS builds
- Game keys and entitlement system functionality
- Checking order status using WebSocket
- 'Resend confirmation email' button
### Fixed
- Improved calculation of in-app browser size for VR projects
- Other fixes and improvements
### Changed
- Updated 'ItemPurchase' method (added 'quantity' parameter)
- Updated packages and dependencies
- Improved social networks auth for iOS builds

## [0.7.0] - 2021-09-17
### Added
- Independent Pay Station UI parameters
- Pay Station for Safari browser (Apple Pay support)
- Virtual currency purchase order status tracking
### Fixed
- Various fixes and improvements
### Changed
- Refactoring and SDK Architecture improvements
- Social friends recommendations list increased
- Update API methods comments
- Move Newtonsoft to packages

## [0.6.4.5] - 2021-07-16
### Added
- Vertical orientation for mobile demo scene
### Fixed
- WebGL build

## [0.6.4.4] - 2021-07-12
### Added
- Horizontal orientation for mobile demo scene
### Fixed
- Several payment systems flow for WebGL build
- Pay Station auto-close after purchase for Unity 2020 and later
- Demo behaviour after social auth cancel
- WeChat errors for android build
- Errors raising once after import for Unity 2020 and later
- In-built browser render quality
- Included copy of Newtonsoft library now can be safely removed in case of conflict
- Various other fixes and improvements
### Changed
- API methods update
- Third-party dependency manager is now included into Xsolla package
- Updated third-party Puppeteer Sharp library to match Unity's own Newtonsoft library

## [0.6.4.3] - 2021-06-10
### Added
- Device ID authorization
- OAuth2.0 state parameter for more API methods
- Pay Station UI settings for different build targets
- Additional Pay Station UI color themes
- 'Create payment token' method
- Xsolla assembly definitions
### Fixed
- In-game browser scroll
- In-game browser numpad
- In-game browser redirection links
- In-game browser post purchase behaviour
- In-game browser window size now corresponds with Paystation UI size
- Handling of few purchase systems that become incompletable with in-game browser
- Various other fixes and improvements
### Changed
- Changed: Pay Station opening version 2 -> 3
- Changed: Updated purchase API methods
### Removed
- Removed: Several surplus libraries (reworked corresponding functionality to use custom logic)

## [0.6.4.2] - 2021-04-23
### Added
- Item attributes support
- Items store/inventory hiding based on HideIn attribute
### Fixed
- Minor fixes

## [0.6.4.1] - 2021-04-16
### Added
- QQ Android auth

## [0.6.4.0] - 2021-03-31
### Added
- Battle Pass
- UI Builder
- Xbox One/PS4 support
- Redirect policy settings
- Logging level settings
- API method for resend account confirmation email
### Fixed
 - Payment status polling
 - Apple ID authorization
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
- URL of user attributes documentation page
- Login via social networks on android

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
- Minor bugs

## [1.0.0.0] - 2020-10-08

### Added
- Friends system functionality.
