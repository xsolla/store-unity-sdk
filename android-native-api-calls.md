# Все вызовы Android SDK из Unity SDK

## Сводка

| Область | Кол-во вызовов |
|---|---|
| XLogin (авторизация) | 7 |
| LoginConfig (конфигурация) | 5 |
| XPayments (платежи) | 10 |
| Analytics | 2 (x2 класса = 4 вызова) |
| WeChat SDK | 4 |
| Android System API | 1 |
| **Итого уникальных точек входа** | **~29** |

---

## 1. XLogin (`com.xsolla.android.login.XLogin`)

### 1.1 `XLogin.init(context, loginConfig)`
- **Откуда:** `Core/Android/AndroidHelper.cs:88`
- **Как:** `_xlogin.CallStatic("init", ApplicationContext, loginConfig)`
- **Назначение:** Инициализация SDK авторизации с конфигом (project ID, OAuth client, соцсети)

### 1.2 `XLogin.startSocialAuth(activity, socialNetwork, callback)`
- **Откуда:** `Core/Plugins/Android/SocialAuthProxyActivity.java:39`
- **Как:** Прямой вызов из Java proxy activity
- **Назначение:** Запуск авторизации через соцсеть (открывает нативный UI)

### 1.3 `XLogin.finishSocialAuth(activity, socialNetwork, requestCode, resultCode, data, callback)`
- **Откуда:** `Core/Plugins/Android/SocialAuthProxyActivity.java:72`
- **Как:** Вызов в `onActivityResult()` Java proxy activity
- **Назначение:** Завершение авторизации через соцсеть, получение результата

### 1.4 `XLogin.startAuthWithXsollaWidget(activity, callback, locale)`
- **Откуда:** `Core/Plugins/Android/XsollaWidgetAuthProxyActivity.java:39`
- **Как:** Прямой вызов из Java proxy activity
- **Назначение:** Запуск авторизации через Xsolla Widget

### 1.5 `XLogin.finishAuthWithXsollaWidget(activity, requestCode, resultCode, data, callback)`
- **Откуда:** `Core/Plugins/Android/XsollaWidgetAuthProxyActivity.java:57`
- **Как:** Вызов в `onActivityResult()` Java proxy activity
- **Назначение:** Завершение авторизации через Xsolla Widget

### 1.6 `XLogin.getToken()`
- **Откуда:** `Core/Android/AndroidAuthCallback.cs:27`
- **Как:** `AndroidHelper.Xlogin.CallStatic<string>("getToken")`
- **Назначение:** Получение access token после успешной авторизации

### 1.7 `XLogin.getRefreshToken()`
- **Откуда:** `Core/Android/AndroidAuthCallback.cs:28`
- **Как:** `AndroidHelper.Xlogin.CallStatic<string>("getRefreshToken")`
- **Назначение:** Получение refresh token после успешной авторизации

---

## 2. LoginConfig (`com.xsolla.android.login.LoginConfig`)

### 2.1 `new XLogin$SocialConfig(fbAppId, fbClientToken, googleServerId, wechatAppId, qqAppId)`
- **Откуда:** `Core/Android/AndroidHelper.cs:74-80`
- **Как:** `new AndroidJavaObject("com.xsolla.android.login.XLogin$SocialConfig", ...)`
- **Назначение:** Конфиг соцсетей (Facebook, Google, WeChat, QQ)

### 2.2 `new LoginConfig$OauthBuilder()`
- **Откуда:** `Core/Android/AndroidHelper.cs:82`
- **Как:** `new AndroidJavaObject("com.xsolla.android.login.LoginConfig$OauthBuilder")`

### 2.3 `OauthBuilder.setSocialConfig(socialConfig)`
- **Откуда:** `Core/Android/AndroidHelper.cs:83`
- **Как:** `loginConfigBuilder.Call<AndroidJavaObject>("setSocialConfig", socialConfig)`

### 2.4 `OauthBuilder.setProjectId(projectId)`
- **Откуда:** `Core/Android/AndroidHelper.cs:84`
- **Как:** `loginConfigBuilder.Call<AndroidJavaObject>("setProjectId", XsollaSettings.LoginId)`

### 2.5 `OauthBuilder.setOauthClientId(clientId)`
- **Откуда:** `Core/Android/AndroidHelper.cs:85`
- **Как:** `loginConfigBuilder.Call<AndroidJavaObject>("setOauthClientId", XsollaSettings.OAuthClientId)`

### 2.6 `OauthBuilder.build()`
- **Откуда:** `Core/Android/AndroidHelper.cs:87`
- **Как:** `loginConfigBuilder.Call<AndroidJavaObject>("build")`

---

## 3. XPayments (`com.xsolla.android.payments.XPayments`)

### 3.1 `XPayments.createIntentBuilder(activity)`
- **Откуда:** `Core/Plugins/Android/PaymentsProxyActivity.java:71`
- **Как:** Прямой вызов из Java
- **Назначение:** Создание билдера для запуска PayStation

### 3.2 `IntentBuilder.accessToken(new AccessToken(token))`
- **Откуда:** `Core/Plugins/Android/PaymentsProxyActivity.java:72`

### 3.3 `IntentBuilder.isSandbox(boolean)`
- **Откуда:** `Core/Plugins/Android/PaymentsProxyActivity.java:73`

### 3.4 `IntentBuilder.payStationVersion(version)`
- **Откуда:** `Core/Plugins/Android/PaymentsProxyActivity.java:74`
- **Значения:** `V3`, `V4`

### 3.5 `IntentBuilder.setRedirectUriScheme(scheme)`
- **Откуда:** `Core/Plugins/Android/PaymentsProxyActivity.java:77`

### 3.6 `IntentBuilder.setRedirectUriHost(host)`
- **Откуда:** `Core/Plugins/Android/PaymentsProxyActivity.java:80`

### 3.7 `IntentBuilder.setActivityType(activityType)`
- **Откуда:** `Core/Plugins/Android/PaymentsProxyActivity.java:87-97`
- **Значения:** `ActivityType.WEB_VIEW`, `ActivityType.CUSTOM_TABS`, `ActivityType.TRUSTED_WEB_ACTIVITY`

### 3.8 `IntentBuilder.build()`
- **Откуда:** `Core/Plugins/Android/PaymentsProxyActivity.java:103`
- **Назначение:** Сборка Intent для запуска PayStation activity

### 3.9 `XPayments.Result.fromResultIntent(data)`
- **Откуда:** `Core/Plugins/Android/PaymentsProxyActivity.java:114`
- **Назначение:** Парсинг результата из PayStation

### 3.10 `Result.getStatus()`
- **Откуда:** `Core/Plugins/Android/PaymentsProxyActivity.java:115`
- **Значения:** `XPayments.Status.CANCELLED` и другие

---

## 4. Analytics

### 4.1 `AnalyticsUtils.gameEngine = "unity"` (set static field)
- **Откуда:** `Core/Android/AndroidHelper.cs:101`
- **Как:** `analyticsUtilsClass.SetStatic("gameEngine", "unity")`

### 4.2 `AnalyticsUtils.gameEngineVersion = <version>` (set static field)
- **Откуда:** `Core/Android/AndroidHelper.cs:102`
- **Как:** `analyticsUtilsClass.SetStatic("gameEngineVersion", Application.unityVersion)`

**Применяется к двум классам:**
- `com.xsolla.android.login.util.AnalyticsUtils`
- `com.xsolla.android.payments.util.AnalyticsUtils`

---

## 5. WeChat SDK (`com.tencent.mm.opensdk`)

### 5.1 `WXAPIFactory.createWXAPI(context, appId, false)`
- **Откуда:** `Core/Plugins/Android/WXEntryActivity.java:23`

### 5.2 `iwxapi.handleIntent(intent, handler)`
- **Откуда:** `Core/Plugins/Android/WXEntryActivity.java:24`

### 5.3 `LoginSocial.getWechatAppId()`
- **Откуда:** `Core/Plugins/Android/WXEntryActivity.java:23`
- **Класс:** `com.xsolla.android.login.social.LoginSocial`

### 5.4 `LoginSocial.setWechatResult(resp)`
- **Откуда:** `Core/Plugins/Android/WXEntryActivity.java:34`
- **Класс:** `com.xsolla.android.login.social.LoginSocial`

---

## 6. SocialNetwork enum

### 6.1 `SocialNetwork.<PROVIDER>` (static field access)
- **Откуда:** `Core/Android/AndroidSocialAuth.cs:16-17`
- **Как:** `socialNetworkClass.GetStatic<AndroidJavaObject>(providerName)`
- **Класс:** `com.xsolla.android.login.social.SocialNetwork`
- **Назначение:** Получение enum-значения соцсети по имени (GOOGLE, FACEBOOK и т.д.)

---

## 7. Android System API

### 7.1 `Settings$Secure.getString(contentResolver, "android_id")`
- **Откуда:** `Core/Device/DeviceIdUtil.cs:14-15`
- **Как:** `secureSettings.CallStatic<string>("getString", contentResolver, "android_id")`
- **Класс:** `android.provider.Settings$Secure`
- **Назначение:** Получение уникального ID устройства

---

## 8. Callback-интерфейсы (реализованы на стороне Unity через AndroidJavaProxy)

| Интерфейс | C# класс | Методы |
|---|---|---|
| `com.xsolla.android.login.callback.AuthCallback` | `AndroidAuthCallback.cs` | `onSuccess()`, `onError(throwable, message)` |
| `com.xsolla.android.payments.callback.BrowserCallback` | `AndroidBrowserCallback.cs` | `onBrowserClosed(isManually)` |

---

## 9. Android-библиотеки (Maven-зависимости)

Объявлены в `Assets/Xsolla/Core/Editor/XsollaDependencies.xml`, подтягиваются через Unity External Dependency Manager.

| Библиотека | Версия | Назначение |
|---|---|---|
| `com.xsolla.android:payments` | 1.4.20 | Платежи (PayStation UI, XPayments API) |
| `com.xsolla.android:login` | 6.0.18 | Авторизация (XLogin, токены, виджет) |
| `com.xsolla.android:login-facebook` | 6.0.18 | Авторизация через Facebook |
| `com.xsolla.android:login-google` | 6.0.18 | Авторизация через Google |
| `com.xsolla.android:login-wechat` | 6.0.18 | Авторизация через WeChat |
| `com.xsolla.android:login-qq` | 6.0.18 | Авторизация через QQ |

Репозиторий: `https://repo1.maven.org/maven2/` + стандартные Google/Android Maven.

---

## Сводная таблица всех вызовов Android SDK

| # | Группа | Вызов API | Откуда вызывается | Назначение |
|---|---|---|---|---|
| 1 | XLogin | `XLogin.init(context, loginConfig)` | AndroidHelper.cs:88 | Инициализация SDK авторизации |
| 2 | XLogin | `XLogin.startSocialAuth(activity, network, callback)` | SocialAuthProxyActivity.java:39 | Запуск соцсеть-авторизации |
| 3 | XLogin | `XLogin.finishSocialAuth(activity, network, reqCode, resCode, data, callback)` | SocialAuthProxyActivity.java:72 | Завершение соцсеть-авторизации |
| 4 | XLogin | `XLogin.startAuthWithXsollaWidget(activity, callback, locale)` | XsollaWidgetAuthProxyActivity.java:39 | Запуск виджет-авторизации |
| 5 | XLogin | `XLogin.finishAuthWithXsollaWidget(activity, reqCode, resCode, data, callback)` | XsollaWidgetAuthProxyActivity.java:57 | Завершение виджет-авторизации |
| 6 | XLogin | `XLogin.getToken()` | AndroidAuthCallback.cs:27 | Получение access token |
| 7 | XLogin | `XLogin.getRefreshToken()` | AndroidAuthCallback.cs:28 | Получение refresh token |
| 8 | LoginConfig | `new XLogin$SocialConfig(fbAppId, fbClientToken, googleServerId, wechatAppId, qqAppId)` | AndroidHelper.cs:74 | Создание конфига соцсетей |
| 9 | LoginConfig | `new LoginConfig$OauthBuilder()` | AndroidHelper.cs:82 | Создание билдера конфига |
| 10 | LoginConfig | `OauthBuilder.setSocialConfig(config)` | AndroidHelper.cs:83 | Установка конфига соцсетей |
| 11 | LoginConfig | `OauthBuilder.setProjectId(id)` | AndroidHelper.cs:84 | Установка project ID |
| 12 | LoginConfig | `OauthBuilder.setOauthClientId(id)` | AndroidHelper.cs:85 | Установка OAuth client ID |
| 13 | LoginConfig | `OauthBuilder.build()` | AndroidHelper.cs:87 | Сборка конфига |
| 14 | XPayments | `XPayments.createIntentBuilder(activity)` | PaymentsProxyActivity.java:71 | Создание билдера платежа |
| 15 | XPayments | `IntentBuilder.accessToken(new AccessToken(token))` | PaymentsProxyActivity.java:72 | Установка токена |
| 16 | XPayments | `IntentBuilder.isSandbox(flag)` | PaymentsProxyActivity.java:73 | Sandbox режим |
| 17 | XPayments | `IntentBuilder.payStationVersion(version)` | PaymentsProxyActivity.java:74 | Версия PayStation (V3/V4) |
| 18 | XPayments | `IntentBuilder.setRedirectUriScheme(scheme)` | PaymentsProxyActivity.java:77 | Redirect scheme |
| 19 | XPayments | `IntentBuilder.setRedirectUriHost(host)` | PaymentsProxyActivity.java:80 | Redirect host |
| 20 | XPayments | `IntentBuilder.setActivityType(type)` | PaymentsProxyActivity.java:87 | Тип UI (WebView/CustomTab/TWA) |
| 21 | XPayments | `IntentBuilder.build()` | PaymentsProxyActivity.java:103 | Сборка Intent |
| 22 | XPayments | `XPayments.Result.fromResultIntent(data)` | PaymentsProxyActivity.java:114 | Парсинг результата платежа |
| 23 | XPayments | `Result.getStatus()` | PaymentsProxyActivity.java:115 | Получение статуса платежа |
| 24 | Analytics | `AnalyticsUtils.gameEngine = "unity"` | AndroidHelper.cs:101 | Аналитика (login + payments) |
| 25 | Analytics | `AnalyticsUtils.gameEngineVersion = version` | AndroidHelper.cs:102 | Аналитика (login + payments) |
| 26 | WeChat | `WXAPIFactory.createWXAPI(context, appId, false)` | WXEntryActivity.java:23 | Создание WeChat API |
| 27 | WeChat | `iwxapi.handleIntent(intent, handler)` | WXEntryActivity.java:24 | Обработка WeChat callback |
| 28 | WeChat | `LoginSocial.getWechatAppId()` | WXEntryActivity.java:23 | Получение WeChat App ID |
| 29 | WeChat | `LoginSocial.setWechatResult(resp)` | WXEntryActivity.java:34 | Передача результата WeChat auth |
| 30 | SocialNetwork | `SocialNetwork.<PROVIDER>` (static field) | AndroidSocialAuth.cs:17 | Enum соцсети по имени |
| 31 | Android System | `Settings$Secure.getString(resolver, "android_id")` | DeviceIdUtil.cs:15 | Device ID |

---

## Визуальная схема вызовов

```
Unity C# (AndroidHelper.cs)
  |
  |-- XLogin.init(context, loginConfig)               [инициализация]
  |-- AnalyticsUtils.gameEngine = "unity"              [аналитика x2]
  |-- AnalyticsUtils.gameEngineVersion = version       [аналитика x2]
  |
  |-- XLogin.getToken()                                [получение токена]
  |-- XLogin.getRefreshToken()                         [получение refresh токена]
  |
  |-- Settings$Secure.getString("android_id")          [device ID]

Unity C# -> Java Proxy Activities -> Android SDK
  |
  |-- PaymentsProxyActivity.perform(...)
  |     |-- XPayments.createIntentBuilder(activity)
  |     |     .accessToken(token)
  |     |     .isSandbox(flag)
  |     |     .payStationVersion(version)
  |     |     .setRedirectUriScheme(scheme)
  |     |     .setRedirectUriHost(host)
  |     |     .setActivityType(type)
  |     |     .build()
  |     |-- XPayments.Result.fromResultIntent(data)
  |     |-- result.getStatus()
  |
  |-- SocialAuthProxyActivity.perform(...)
  |     |-- XLogin.startSocialAuth(activity, network, callback)
  |     |-- XLogin.finishSocialAuth(activity, network, ...)
  |
  |-- XsollaWidgetAuthProxyActivity.perform(...)
  |     |-- XLogin.startAuthWithXsollaWidget(activity, callback, locale)
  |     |-- XLogin.finishAuthWithXsollaWidget(activity, ...)
  |
  |-- WXEntryActivity (WeChat)
        |-- WXAPIFactory.createWXAPI(context, appId)
        |-- iwxapi.handleIntent(intent, handler)
        |-- LoginSocial.getWechatAppId()
        |-- LoginSocial.setWechatResult(resp)
```

