## Xsolla Store Unity Asset

The Xsolla Store SDK allows you to use a ready-made server solution for  monetization and in-game items management in apps based on the [Unity](https://unity.com/).

After integration of Store SDK, you can use:

* [Xsolla Login](https://developers.xsolla.com/doc/login/) for authenticating users, manage the friend system and user account
* [Xsolla In-Game Store](https://developers.xsolla.com/doc/in-game-store/) for creating an in-game store and player’s inventory
* [Xsolla Player Inventory](https://developers.xsolla.com/doc/pay-station/) for managing player’s inventory in your application
* [Xsolla Pay Station](https://developers.xsolla.com/doc/pay-station/) for setting up payments

With this integration, all operations are processed on the Xsolla side and you do not have to configure your own server side for these tasks.

## WebGL
Usefull links:
* [Unity WebGL docs](https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html)
* [Xsolla PayStation script docs](https://developers.xsolla.com/doc/pay-station/integration-guide/open-payment-ui/#pay_station_guide_pay_station_embed)
* [Xsolla PayStation additional info](https://developers.xsolla.com/doc/pay-station/features/paystation-analytics/#pay_station_features_analytics_ps_events)

### WebGL Getting Started
1. Download branch named as **feat/WebPaystation**
2.  Add scenes to build if it not exist
3.  Switch build settings to WebGL
4.  Build it and run as you want

**NOTE:**
* Login API support CORS. So you need to change CORS settings in [Publisher Account](https://publisher.xsolla.com/signup?store_type=sdk).
* Don't maximize UnityWebPlayer's window — you won't see other IFrame with Pay Station.

## System Requirements

* 64-bit OS
* Windows 7 SP1 and higher
* macOS 10.12 and higher
* A compatible version of Unity:
	* 2018.3.0f2
	* 2019.3.4f1

**NOTE:** We recommend you use the Mono compiler for creating a game build. You can use either Mono or IL2CPP compiler for creating APK.

## Target OS
* iOS
* Android
* macOS
* Windows 64-bit

## Prerequisites

1. [Download Unity](https://store.unity.com/download).
2. Pick a personal or professional Unity license based on your preferences.
3. Create a new Unity project.
4. Register an Xsolla [Publisher Account](https://publisher.xsolla.com/signup?store_type=sdk) and set up a new project. More instructions are on the [Xsolla Developers portal](https://developers.xsolla.com/sdk/game-engines/unity/#unity_sdk_use_xsolla_servers_prerequisites).
5. Go to the [Xsolla Developers portal](https://developers.xsolla.com/sdk/game-engines/unity/#unity_sdk_use_xsolla_servers_store_unity_sdk_integration) to learn how to integrate Xsolla products using  **Xsolla Store SDK for Unity**. 

## Additional resources
* [Website](http://xsolla.com/)
* [Documentation](https://developers.xsolla.com/sdk/game-engines/unity/)
* [Wiki](https://github.com/xsolla/store-unity-sdk/wiki/)