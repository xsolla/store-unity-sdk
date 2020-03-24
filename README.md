# Xsolla Store Unity Asset

**Xsolla Store Unity Asset** is used to integrate Xsolla products with apps based on [Unity](https://unity.com/). The asset includes the following SDKs:

* Xsolla Login Unity SDK
* Xsolla Store Unity SDK
* Xsolla Pay Station Unity SDK

## WebGL

[Unity WebGL docs](https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html)

[Xsolla PayStation script docs](https://developers.xsolla.com/doc/pay-station/#guides_pay_station_pay_station_embed)

[Xsolla PayStation additional info](https://developers.xsolla.com/api/v2/pay-station/#api_payment_ui_events)

**WebGL Getting Started**
* Download branch named as "feat/WebPaystation"
* Add scenes to build if it not exist
* Switch build settings to WebGL
* Build it and run as you want

**NOTE:**
* LoginAPI support CORS. So you need change CORS settings in `Publisher Account`.
* Don`t maximize UnityWebPlayer's window - you don't see other IFrame with Paystation.

## System Requirements

* 64-bit OS
* Windows 7 SP1 and higher
* Mac OS 10.12 and higher
* DirectX 10
* Visual Studio 2015
* Unity 2017.4.22 and higher

## Target OS
* iOS
* Android
* Linux
* macOS
* Windows 32-bit
* Windows 64-bit

## Prerequisites

1. [Download](https://store.unity.com/download) Unity.
2. Pick a personal or professional Unity license based on your preferences.
3. Create a new Unity project.
4. Register an Xsolla [Publisher Account](https://publisher.xsolla.com/signup?store_type=sdk). More instructions are on [Xsolla Developers portal](https://developers.xsolla.com/sdk/game-engines/unity/).
5. Create and set up a Publisher Account project:
    1. Go to Projects and click **Create new project**.
    2. In setup mode, add **Project name** and click **Create**.
    3. Go to **Project settings > Integration settings** and check that **Tokenless integration** is disabled.

6. Go to the [wiki](https://github.com/xsolla/store-unity-sdk/wiki) to learn how to integrate Xsolla products using Xsolla Store Unity Asset. 
