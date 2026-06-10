# Native Android Integration in Assets/Xsolla

## 1. Java-файлы (нативные Android-активити)

| Файл | Назначение |
|---|---|
| `Core/Plugins/Android/PaymentsProxyActivity.java` | Прокси-активити для платежей через `XPayments` (WebView / CustomTab / TrustedWebActivity) |
| `Core/Plugins/Android/SocialAuthProxyActivity.java` | Прокси-активити для соцсетей через `XLogin.startSocialAuth()` / `finishSocialAuth()` |
| `Core/Plugins/Android/XsollaWidgetAuthProxyActivity.java` | Прокси-активити для авторизации через Xsolla Widget |
| `Core/Plugins/Android/WXEntryActivity.java` | Обработка callback от WeChat SDK (`com.tencent.mm.opensdk`) |
| `Core/Plugins/Android/BrowserCallback.java` | Интерфейс `onBrowserClosed(boolean isManually)` для callback из браузера |

## 2. C# классы-обёртки (AndroidJavaClass / AndroidJavaObject)

| Файл | Что делает |
|---|---|
| `Core/Android/AndroidHelper.cs` | Главный хелпер — кеширует `UnityPlayer`, `currentActivity`, `ApplicationContext`. Инициализирует `XLogin.init()` с `LoginConfig` и `SocialConfig` |
| `Core/Android/AndroidPayments.cs` | Вызывает `PaymentsProxyActivity.perform()` через `AndroidJavaClass` с токеном, sandbox-флагом и настройками редиректов |
| `Core/Android/AndroidSocialAuth.cs` | Вызывает `SocialAuthProxyActivity.perform()` — получает `SocialNetwork` enum и запускает соцаутентификацию |
| `Core/Android/AndroidAuthCallback.cs` | `AndroidJavaProxy` -> интерфейс `com.xsolla.android.login.callback.AuthCallback`. Получает токен/refresh через `XLogin.getToken()` |
| `Core/Android/AndroidBrowserCallback.cs` | `AndroidJavaProxy` -> интерфейс `com.xsolla.android.payments.callback.BrowserCallback` |
| `Auth/Internal/WidgetAuthenticators/AndroidWidgetAuthenticator.cs` | Вызывает `XsollaWidgetAuthProxyActivity.perform()` для виджет-авторизации |
| `Core/Browser/Internal/XsollaWebBrowserHandlerAndroid.cs` | Точка входа для открытия PayStation на Android — вызывает `AndroidPayments.Perform()` |
| `Core/Device/DeviceIdUtil.cs` | Получает `android_id` через `Settings$Secure.getString()` |

## 3. Нативные Android-библиотеки, которые вызываются

- **`com.xsolla.android.login.XLogin`** — авторизация, соц. сети, токены
- **`com.xsolla.android.payments.XPayments`** — платежи (PayStation)
- **`com.tencent.mm.opensdk`** — WeChat SDK (WXAPIFactory, IWXAPIEventHandler)
- **`android.provider.Settings$Secure`** — получение device ID

## 4. Editor-скрипты (подготовка билда)

| Файл | Назначение |
|---|---|
| `Core/Editor/AndroidManifestPreprocess.cs` | Генерирует `AndroidManifest.xml` — добавляет proxy-активити и WeChat-активити |
| `Core/Editor/AndroidProguardProcessor.cs` | Копирует ProGuard-правила (`-keep class com.xsolla.**`) |
| `Core/Editor/AndroidScriptsProcessor.cs` | Подменяет package name в Java-файлах на `Application.identifier` перед билдом, возвращает обратно после |

## 5. Шаблоны манифестов и ProGuard

- `Core/Editor/AndroidTemplateFiles/AndroidManifest.xml` — базовый шаблон
- `Core/Editor/AndroidTemplateFiles/AndroidManifest_6000.xml` — шаблон для Unity 6000+
- `Core/Editor/AndroidTemplateFiles/proguard-user.txt` — keep-правила для `com.xsolla.**`

## 6. Файлы с `#if UNITY_ANDROID`

- `Core/Android/AndroidHelper.cs`
- `Core/Android/AndroidPayments.cs`
- `Core/Android/AndroidSocialAuth.cs`
- `Core/Android/AndroidAuthCallback.cs`
- `Core/Android/AndroidBrowserCallback.cs`
- `Core/Browser/Internal/XsollaWebBrowserHandlerAndroid.cs`
- `Core/Device/DeviceIdUtil.cs`
- `Core/Settings/RedirectPolicySettings.cs`
- `Core/Settings/PayStationUISettings.cs`
- `Core/Utils/PurchaseParamsGenerator.cs` (Google Pay, platform = "android")
- `Core/Editor/AndroidManifestPreprocess.cs`
- `Core/Editor/AndroidProguardProcessor.cs`
- `Core/Editor/AndroidScriptsProcessor.cs`
- `Auth/Entities/DeviceInfo.cs`
- `Auth/Internal/WidgetAuthenticators/WidgetAuthenticatorFactory.cs`
- `Core/Browser/XsollaWebBrowser.cs`

## 7. Архитектура

**Паттерн Proxy Activity**: Unity C# код через `AndroidJavaClass`/`AndroidJavaObject` вызывает статические методы Java proxy-активити -> они запускают нативные SDK (`XLogin`, `XPayments`) -> результат возвращается через `onActivityResult` -> callback передаётся в Unity через `AndroidJavaProxy` (реализация Java-интерфейсов на C#).

```
Unity C# (AndroidJavaClass/AndroidJavaObject)
    |
    v
Java Proxy Activity (PaymentsProxyActivity / SocialAuthProxyActivity / XsollaWidgetAuthProxyActivity)
    |
    v
Native Android SDK (XLogin / XPayments / WeChat SDK)
    |
    v
onActivityResult()
    |
    v
AndroidJavaProxy callback (AndroidAuthCallback / AndroidBrowserCallback) -> Unity C#
```
