
# Game Commerce asset for Unity

Game Commerce asset is a set of classes and methods that you can integrate into your Unity app to work with Xsolla products.

After integration, you can use:

*   [Login](https://developers.xsolla.com/doc/login/) for authenticating users and managing the friend system and user account
*   [In-Game Store](https://developers.xsolla.com/doc/in-game-store/) for managing in-game purchases and player inventory in your app
*   [Pay Station](https://developers.xsolla.com/doc/pay-station/) for setting up payments

[Try our demo to learn more](https://livedemo.xsolla.com/sdk/unity/webgl/).


![Game Commerce demo](https://i.imgur.com/cx0YR1r.png "Game Commerce demo")

The Game Commerce asset contains the necessary methods to use the features of all Xsolla products listed above. If you plan to use only some of them in your app, feel free to remove unnecessary modules from the asset.

We recommend using the game Commerce asset as is. For some specific situations, you can use other Xsolla assets:

*   Use [Login & Account System asset](https://assetstore.unity.com/packages/slug/180654?_xm=3001.151246642924748818) if you are interested in the ready-made login system only.
*   Use [Cross-Buy asset](https://assetstore.unity.com/packages/slug/184991?_xm=3001.151246642924748818) if you are going to publish your app on a platform that restricts the use of third-party payment tools.

<div style="background-color: WhiteSmoke">
<p><b>Note:</b> Game Commerce asset includes Login & Account System and Cross-Buy assets. You do <b>not</b> need to install them separately.</p>
</div>

For a better understanding of which asset to choose, use the table:


<table>
  <tr>
   <td>
   </td>
   <td style="text-align: center"><b>Game Commerce asset</b>
   </td>
   <td style="text-align: center"><b>Login & Account System asset</b>
   </td>
   <td style="text-align: center"><b>Cross-Buy asset</b>
   </td>
  </tr>
  <tr>
   <td colspan="4" ><b>In-game store</sb>
   </td>
  </tr>
  <tr>
   <td>
    Virtual currency
   </td>
   <td>+
   </td>
   <td>
   </td>
   <td>
   </td>
  </tr>
  <tr>
   <td>
    Virtual items
   </td>
   <td>+
   </td>
   <td>
   </td>
   <td>
   </td>
  </tr>
  <tr>
   <td>
    Player inventory
   </td>
   <td>+
   </td>
   <td>
   </td>
   <td>+
   </td>
  </tr>
  <tr>
   <td>
    Bundles
   </td>
   <td>+
   </td>
   <td>
   </td>
   <td>
   </td>
  </tr>
  <tr>
   <td>
    Promotional campaigns
   </td>
   <td>+
   </td>
   <td>
   </td>
   <td>
   </td>
  </tr>
  <tr>
   <td colspan="4" ><b>Login</b>
   </td>
  </tr>
  <tr>
   <td>
    Authentication
   </td>
   <td>+
   </td>
   <td>+
   </td>
   <td>+
   </td>
  </tr>
  <tr>
   <td>
    User management
   </td>
   <td>+
   </td>
   <td>+
   </td>
   <td>+
   </td>
  </tr>
  <tr>
   <td><strong>Payment UI</strong>
   </td>
   <td>+
   </td>
   <td>
   </td>
   <td>
   </td>
  </tr>
  <tr>
  <td colspan="4" ><b>Additional features</b>
   </td>
  </tr>
  <tr>
   <td>
    UI builder
   </td>
   <td>
    +
   </td>
   <td>
    +
   </td>
   <td>
    +
   </td>
  </tr>
  <tr>
   <td>
    Battle pass
   </td>
   <td>
    +
   </td>
   <td>
   </td>
   <td>
   </td>
  </tr>
</table>


## Requirements

### System requirements

*   64-bit OS
*   Windows 7 SP1 and later
*   macOS 10.12 and later
*   The version of Unity not earlier than 2019.4.19f1

### Target OS

*   Android
*   macOS
*   Windows 64-bit

Additionally, the asset supports [creating WebGL build](https://developers.xsolla.com/sdk/unity/how-tos/application-build/#unity_sdk_how_to_build_webgl) to run your application in a browser.

<div style="background-color: WhiteSmoke">
<p><b>Note:</b></p>
<p>We recommend you use the Mono compiler for desktop platforms as it's compatible with the provided in-game browser. If you use other browser solutions, you can use the IL2CPP compiler instead. To create game builds for Android, you can use either Mono or IL2CPP compilers.</p>
</div>


## Integration

The following integration options are available:

<table>
  <tr>
   <td>
<h3  style="text-align: center">
<img src="https://i.imgur.com/3bdXcuv.png" width="50" >
</h3>
<h3 style="text-align: center">Use Xsolla servers</h3>
   </td>
   <td>
<h3 style="text-align: center">
<img src="https://i.imgur.com/eGmKMPX.png" width="50" >
</h3>
<h3 style="text-align: center">Use your server</h3>
   </td>
  </tr>
  <tr>
   <td>Choose this option if you want a ready-made server solution for monetization and in-game items management. After integration of the asset, you can use <a href="https://developers.xsolla.com/doc/login/">Xsolla Login</a>, <a href="https://developers.xsolla.com/doc/in-game-store/">In-Game Store</a>, <a href="https://developers.xsolla.com/doc/in-game-store/features/player-inventory/">Player Inventory</a>, and <a href="https://developers.xsolla.com/doc/pay-station/">Xsolla Pay Station</a>.
   </td>
   <td>Choose this option if you have already implemented the game logic for authentication, in-game store, and player inventory on your servers and want to use <a href="https://developers.xsolla.com/doc/pay-station/">Xsolla Pay Station</a>.
   </td>
  </tr>
  <tr>
   <td><a href="https://developers.xsolla.com/sdk/unity/commerce/use-xsolla-servers/">Get started →</a> 
   </td>
   <td><a href="https://developers.xsolla.com/sdk/unity/commerce/use-your-server-side/">Get started →</a>
   </td>
  </tr>
</table>


## Usage 

Xsolla provides APIs to work with it’s products. The Game Commerce asset provides classes and methods for API calls, so you won’t need to write boilerplate code. Use the [tutorials](https://developers.xsolla.com/sdk/unity/tutorials/) to learn how you can use the [asset methods](https://developers.xsolla.com/sdk-code-references/unity-store/).

## Known issues

### Conflict of multiple precompiled assemblies with Newtonsoft.json.dll

#### Issue description

The issue appears when importing the asset on Unity version 2020.3.10f1 and later. The following error message is displayed:

>Multiple precompiled assemblies with the same name Newtonsoft.json.dll included on the current platform. Only one assembly with the same name is allowed per platform.

The conflict arises because the `Newtonsoft.json.dll` library is included in both the Unity Editor and the asset. The library is included in the versions 2020.3.10f1 and later of the editor. And the asset includes the library to support the earlier versions of Unity Editor.

**Issue status:** Fixed in 0.6.4.4.


#### Workaround

1. Remove the `Newtonsoft.json.dll` library from the asset:
    1. Create a new Unity project.
    2. Install [Game Commerce asset](https://assetstore.unity.com/packages/slug/145141) from Unity Asset Store.
    3. Go to  `Assets\Xsolla\Core\Browser\XsollaBrowser\Plugins` directory.
    4. Remove `Newtonsoft.Json.dll` and `Newtonsoft.Json.dll.mdb` files.
2. Restart Unity Editor.

### Newtonsoft.json.dll could not be found

#### Issue description

The problem appears if you upgraded a pre-existing project to Unity version 2020.3.10f1 and later. Importing an asset from the [Unity Asset Store](https://assetstore.unity.com/publishers/12995) into such a project is accompanied by many error messages like this:

>The type or namespace name ‘Newtonsoft’ could not be found (are you missing a using directive or an assembly reference?)


The problem occurs because the `Newtonsoft.json.dll` library is not included in the asset for Unity version 2020.3.10f1 and later. As part of the editor, the library is supplied for versions 2020.3.10f1 and later, but when updating the project for these versions, the library requires manual installation.

**Issue status:** Fixed in 0.6.4.4.

#### Workaround

Install the `Newtonsoft.json.dll` library manually using the <a href="https://docs.unity3d.com/Packages/com.unity.package-manager-ui@1.8/manual/index.html">Unity Package Manager</a>.


### Unable to resolve reference UnityEditor.iOS.Extensions.Xcode

#### Issue description

The issue appears when using External Dependency Manager on Unity version 2020.1.0f1 and later.

When building the application, an error message is displayed:


>Assembly 'Packages/com.google.external-dependency-manager/ExternalDependencyManager/Editor/Google.IOSResolver_v1.2.161.dll' will not be loaded due to errors:
Unable to resolve reference 'UnityEditor.iOS.Extensions.Xcode'. Is the assembly missing or incompatible with the current platform?
Reference validation can be disabled in the Plugin Inspector.

**Issue status:** Fixed in 0.6.4.5.

#### Workaround

Install iOS Build Support module from Unity Hub.

### Error occurred running Unity content on page of WebGL build

#### Issue description
 The issue may appear when logging in WebGL build. The following error message is displayed:

![WebGL error message](https://i.imgur.com/me3ADT4.png "WebGL error message")

See details on cause of the issue on [Unity Issue Tracker](https://issuetracker.unity3d.com/issues/il2cpp-notsupportedexceptions-exception-is-thrown-in-build-with-newtonsoft-dot-json-plugin).

**Issue status:** Won’t fix.

#### Workaround

1. Open Unity project.
2. Click **Edit > Project Settings** in the main menu.
3. In the **Player** section, go to the WebGL build settings tab.
4. Go to the **Other Settings** section.
5. Uncheck **Strip engine code** box.
6. Go to the **Publishing Settings** section.
7. Check the **Decompression Fallback** box.
8. Create a new WebGL build.


## Legal info

[Explore legal information](https://developers.xsolla.com/sdk/unity/commerce/get-started/#sdk_legal_compliance) that helps you work with Xsolla.

Xsolla offers the necessary tools to help you build and grow your gaming business, including personalized support at every stage. The terms of payment are determined by the contract that you can sign in Xsolla Publisher Account.

---

### License

See the [LICENSE](https://github.com/xsolla/store-unity-sdk/blob/master/LICENSE.txt) file.


### Community
[Join our Discord server](https://discord.gg/auNFyzZx96) and connect with the Xsolla team and developers who use Xsolla products.


### Additional resources

*   [Xsolla official website](https://xsolla.com/)
*   [Developers documentation](https://developers.xsolla.com/sdk/unity/)
*   [Code reference](https://developers.xsolla.com/sdk-code-references/unity-store/)
*   API reference:
    *   [Pay Station API](https://developers.xsolla.com/pay-station-api/)
    *   [Login API](https://developers.xsolla.com/login-api/) 
    *   [Commerce API](https://developers.xsolla.com/commerce-api/)