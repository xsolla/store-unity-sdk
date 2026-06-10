# store-unity-sdk

[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](./LICENSE.txt)
[![Latest Release](https://img.shields.io/github/v/release/xsolla/store-unity-sdk)](https://github.com/xsolla/store-unity-sdk/releases)
[![Unity 2021.3+](https://img.shields.io/badge/Unity-2021.3%2B-black.svg?logo=unity)](https://unity.com/)

## Overview

The Xsolla SDK for Unity makes it easier to integrate Xsolla products into your Unity project by providing out-of-the-box data structures and methods for working with the Xsolla API. After integration, you can authenticate users, manage in-game purchases and player inventory, set up payment methods, and sell subscriptions.

## Requirements

- **OS (development):** 64-bit Windows 7 SP1+ or macOS 10.12+
- **Unity:** 2021.3.56f2 or later
- **Target platforms:** macOS, Windows 64-bit, Android, iOS, WebGL
- **Xsolla Publisher Account:** Required — [sign up here](https://publisher.xsolla.com/signup?utm_source=sdk&utm_medium=unity-store/)

## Install

### Option 1 — Unity Package (recommended)

1. [Download the latest `.unitypackage`](https://github.com/xsolla/store-unity-sdk/raw/refs/heads/master/xsolla-unity-sdk-latest.unitypackage) or choose a version from [GitHub Releases](https://github.com/xsolla/store-unity-sdk/releases).
2. In the Unity Editor, go to **Assets > Import Package > Custom Package** and select the downloaded file.

### Option 2 — Unity Package Manager (git URL)

1. Open your Unity project.
2. Go to **Window > Package Manager**.
3. Click **+** → **Add package from git URL**.
4. Enter: `https://github.com/xsolla/store-unity-sdk.git?path=Assets/Xsolla`

## Usage

```csharp
// Example: Authenticate a user
XsollaAuth.SignIn(username, password, rememberUser: false,
    onSuccess: token => Debug.Log("Signed in: " + token),
    onError: error => Debug.LogError(error.errorMessage));
```

For a complete walkthrough, see the [step-by-step integration guide](https://developers.xsolla.com/sdk/unity-enterprise/integrate-complete-solution/integrate-on-app-side/#unity_sdk_integrate_on_app_side_quick_start).

## Documentation

Full SDK documentation: [developers.xsolla.com/sdk/unity-enterprise/](https://developers.xsolla.com/sdk/unity-enterprise/)

## Support

- [GitHub Issues](https://github.com/xsolla/store-unity-sdk/issues)
- [Xsolla partner support](https://xsolla.com/partner-support)
- [Integration team](mailto:integration@xsolla.com)

## License

Apache 2.0. See [LICENSE](./LICENSE.txt).
