# Сравнение Вариант 1 vs Вариант 2: реальный код

## Итоговые цифры

| Метрика | Вариант 1 | Вариант 2 |
|---------|-----------|-----------|
| LOC Android Java (мини-SDK) | ~2,500 | ~800 |
| LOC Unity C# (новый код) | 0 | ~400 |
| Внешние зависимости Android | 1 (`androidx.browser`) | 1 (`androidx.browser`) |
| Изменений в Unity C# файлах | 0 | 5 файлов |
| Изменений в ProxyActivity Java файлах | 0 | 3 файла |
| Дублирование логики при добавлении Unreal | нет | ~400 LOC C++ |

---

## Таблица: что меняется по каждой группе вызовов

| Группа | Файл вызова (Unity) | Вариант 1 | Вариант 2 |
|--------|---------------------|-----------|-----------|
| **XLogin.init** | `AndroidHelper.cs:88` | без изменений | упрощается: не нужен `LoginConfig` / `OauthBuilder` |
| **XLogin.startSocialAuth** | `SocialAuthProxyActivity.java:39` | без изменений | без изменений |
| **XLogin.finishSocialAuth** | `SocialAuthProxyActivity.java:72` | без изменений | **удаляется** — заменяется перехватом redirect URL |
| **XLogin.startAuthWithXsollaWidget** | `XsollaWidgetAuthProxyActivity.java:39` | без изменений | без изменений |
| **XLogin.finishAuthWithXsollaWidget** | `XsollaWidgetAuthProxyActivity.java:57` | без изменений | **удаляется** — заменяется перехватом redirect URL |
| **XLogin.getToken** | `AndroidAuthCallback.cs:27` | без изменений | **удаляется** — токен приходит из Unity C# |
| **XLogin.getRefreshToken** | `AndroidAuthCallback.cs:28` | без изменений | **удаляется** — refresh token приходит из Unity C# |
| **LoginConfig / OauthBuilder** | `AndroidHelper.cs:74-87` | без изменений | **удаляется** полностью |
| **XPayments builder chain** | `PaymentsProxyActivity.java:71-103` | без изменений | упрощается: без `Result.fromResultIntent()` |
| **XPayments.Result.fromResultIntent** | `PaymentsProxyActivity.java:114` | без изменений | **удаляется** — статус полинг переходит в Unity |
| **AnalyticsUtils** | `AndroidHelper.cs:101-102` | без изменений | без изменений |
| **WeChat** | `WXEntryActivity.java` | без изменений | без изменений |
| **SocialNetwork enum** | `AndroidSocialAuth.cs:16-17` | без изменений | без изменений |
| **Settings$Secure (device ID)** | `DeviceIdUtil.cs:14-15` | без изменений | без изменений |

---

## Вариант 1 — Мини-SDK с теми же контрактами

### Что НЕ меняется в Unity

Все файлы остаются без изменений:
- `Assets/Xsolla/Core/Android/AndroidHelper.cs`
- `Assets/Xsolla/Core/Android/AndroidAuthCallback.cs`
- `Assets/Xsolla/Core/Android/AndroidSocialAuth.cs`
- `Assets/Xsolla/Core/Plugins/Android/SocialAuthProxyActivity.java`
- `Assets/Xsolla/Core/Plugins/Android/PaymentsProxyActivity.java`
- `Assets/Xsolla/Core/Plugins/Android/XsollaWidgetAuthProxyActivity.java`
- `Assets/Xsolla/Core/Plugins/Android/WXEntryActivity.java`

### Что создаётся: мини-SDK Java классы

#### Модуль Login (~1,500 LOC)

| Класс (Java) | Аналог в текущем SDK (Kotlin) | Назначение |
|---|---|---|
| `XLogin.java` | `XLogin.kt` (1,796 LOC → 7 методов) | Фасад: init, startSocialAuth, finishSocialAuth, startAuthWithXsollaWidget, finishAuthWithXsollaWidget, getToken, getRefreshToken |
| `LoginConfig.java` + `OauthBuilder` + `SocialConfig` | `LoginConfig.kt` | Конфиг инициализации |
| `LoginSocial.java` | `LoginSocial.kt` | Social auth flow, HTTP запросы |
| `AnalyticsUtils.java` | `AnalyticsUtils.kt` | Статические поля: gameEngine, gameEngineVersion |
| `AuthCallback.java` (interface) | `AuthCallback.kt` | Callback: onSuccess(), onError() |
| `SocialNetwork.java` (enum) | `SocialNetwork.kt` | Enum провайдеров: GOOGLE, FACEBOOK, WECHAT, QQ, ... |

#### Модуль Payments (~1,000 LOC)

| Класс (Java) | Аналог в текущем SDK (Kotlin) | Назначение |
|---|---|---|
| `XPayments.java` + `IntentBuilder` + `AccessToken` + `Result` | `XPayments.kt` | Фасад: createIntentBuilder, builder chain, result parsing |
| `ActivityPayStation.java` | `ActivityPayStation.kt` (982 LOC) | WebView + CustomTabs + TWA UI |
| `AnalyticsUtils.java` | `AnalyticsUtils.kt` | Статические поля (аналог login) |
| `BrowserCallback.java` (interface) | `BrowserCallback.kt` | Callback: onBrowserClosed(isManually) |

### HTTP замены (Вариант 1)

Все HTTP вызовы остаются в Android, меняется только библиотека:

| Текущая библиотека | Замена | Где используется |
|---|---|---|
| `OkHttp3` | `java.net.HttpURLConnection` | LoginSocial (4 вызова) |
| `Retrofit2` | `java.net.HttpURLConnection` | XPayments status polling |
| `kotlinx-serialization` | `org.json.JSONObject` (встроен) | Везде |
| `kotlin-stdlib` | убирается | — |

#### HTTP вызовы которые остаются в Android (Вариант 1)

```
1. GET  /api/oauth2/social/{provider}/login_url   → получение URL для открытия браузера
2. POST /api/oauth2/social/{provider}/code        → обмен code на токен (через браузер redirect)
3. POST /api/oauth2/token                         → финальный обмен code → access_token + refresh_token
4. GET  /paystation2/api/payments/status          → polling статуса платежа
```

### Зависимости мини-SDK (Вариант 1)

```xml
<!-- Обязательные -->
<androidPackage spec="androidx.browser:browser:1.8.0"/>

<!-- Опциональные (compileOnly — не тянутся если не используются) -->
<!-- com.facebook.android:facebook-login -->
<!-- com.google.android.gms:play-services-auth -->
<!-- com.tencent.mm.opensdk:wechat-sdk-android-without-mta -->
<!-- Tencent QQ SDK -->
```

---

## Вариант 2 — Мини-SDK с новым контрактом (HTTP → Unity)

### Что меняется в Unity

#### `Assets/Xsolla/Core/Android/AndroidHelper.cs`

```csharp
// БЫЛО (строки 74-87): создание LoginConfig, OauthBuilder, SocialConfig
// LoginConfig с 5 параметрами (fbAppId, fbClientToken, googleServerId, wechatAppId, qqAppId)

// СТАНЕТ: упрощённый init, конфиг только для browser настроек
// Вся OAuth конфигурация не нужна на Android стороне
```

#### `Assets/Xsolla/Core/Android/AndroidAuthCallback.cs`

```csharp
// БЫЛО (строки 27-28):
string token = AndroidHelper.Xlogin.CallStatic<string>("getToken");
string refreshToken = AndroidHelper.Xlogin.CallStatic<string>("getRefreshToken");

// СТАНЕТ: токены больше не хранятся в Android SDK
// AndroidAuthCallback получает code из Android, затем Unity сама вызывает POST /oauth2/token
```

#### `Assets/Xsolla/Core/Android/AndroidSocialAuth.cs`

```csharp
// ДОБАВЛЯЕТСЯ после получения auth code от Android:
// WebRequestHelper.Instance.PostRequest(tokenUrl, codePayload,
//   onSuccess: (tokenResponse) => { /* сохранить token */ },
//   onError: onError);
// Используется существующий Assets/Xsolla/Core/WebRequests/WebRequestHelper.cs
```

#### `Assets/Xsolla/Core/Plugins/Android/SocialAuthProxyActivity.java`

```java
// БЫЛО (строка 72):
XLogin.finishSocialAuth(activity, socialNetwork, requestCode, resultCode, data, finishCallback);
// finishCallback внутри делает HTTP: POST /oauth2/social/{provider}/code + POST /oauth2/token

// СТАНЕТ: перехватить redirect URI, извлечь code и state, вернуть в Unity
// Нет HTTP вызовов в Java — только передача строк обратно в C#
String code = extractCodeFromRedirectUri(data);
authCallback.onSuccess(code); // передаём code, не токен
```

#### `Assets/Xsolla/Core/Plugins/Android/XsollaWidgetAuthProxyActivity.java`

```java
// БЫЛО (строка 57):
XLogin.finishAuthWithXsollaWidget(activity, requestCode, resultCode, data, finishCallback);

// СТАНЕТ: то же что выше — перехватить code из redirect, вернуть в Unity
```

#### `Assets/Xsolla/Core/Plugins/Android/PaymentsProxyActivity.java`

```java
// БЫЛО (строки 114-115):
XPayments.Result result = XPayments.Result.fromResultIntent(data);
result.getStatus(); // XPayments.Status enum

// СТАНЕТ: упрощённый возврат — просто сигнал закрытия браузера
// Polling статуса (GET /paystation2/api/payments/status) переходит в Unity C#
browserCallback.onBrowserClosed(true);
```

### Что создаётся: мини-SDK Java классы (Вариант 2)

Значительно меньше кода — только browser/UI layer:

| Класс (Java) | LOC | Назначение |
|---|---|---|
| `XBrowser.java` | ~200 | Фасад: openCustomTabs, openWebView, openTWA, openSystemBrowser |
| `BrowserActivity.java` | ~400 | Activity: CustomTabs + WebView + TWA + redirect interception |
| `SocialAuthActivity.java` | ~200 | Activity: нативные social SDK (Facebook, Google, WeChat, QQ) + redirect |
| `BrowserCallback.java` (interface) | ~10 | Callback: onRedirectCaptured(String code), onCancelled() |
| `AnalyticsUtils.java` | ~10 | Статические поля |

**Итого: ~820 LOC Java, 0 HTTP, 1 зависимость.**

### HTTP вызовы которые переходят в Unity C# (Вариант 2)

| Вызов | URL | Метод | Что уже есть в Unity |
|---|---|---|---|
| Обмен social code → токен | `POST /api/oauth2/social/{provider}/code` | POST JSON | `WebRequestHelper.cs` |
| Финальный обмен code → access_token | `POST /api/oauth2/token` | POST form-data | `WebRequestHelper.cs` |
| Widget auth code → access_token | `POST /api/oauth2/token` | POST form-data | `WebRequestHelper.cs` |
| Polling статуса платежа | `GET /paystation2/api/payments/status` | GET | `WebRequestHelper.cs` |

---

## Сравнение по файлам

| Файл | Вариант 1 | Вариант 2 |
|------|-----------|-----------|
| `AndroidHelper.cs` | без изменений | меняется: убрать LoginConfig/OauthBuilder |
| `AndroidAuthCallback.cs` | без изменений | меняется: убрать XLogin.getToken() вызовы, добавить HTTP |
| `AndroidSocialAuth.cs` | без изменений | меняется: добавить POST /oauth2/token вызов |
| `SocialAuthProxyActivity.java` | без изменений | меняется: finishSocialAuth → redirect interception |
| `PaymentsProxyActivity.java` | без изменений | меняется: убрать Result.fromResultIntent |
| `XsollaWidgetAuthProxyActivity.java` | без изменений | меняется: finishAuthWithXsollaWidget → redirect interception |
| `WXEntryActivity.java` | без изменений | без изменений |
| `DeviceIdUtil.cs` | без изменений | без изменений |
| Android SDK — классов | ~10 классов, ~2,500 LOC | ~5 классов, ~820 LOC |
| Unity C# — новый код | 0 | ~400 LOC (4 HTTP вызова + ответы) |

---

## Ключевой выбор

**Вариант 1** — меньше рисков, нулевые изменения в Unity. Работа сосредоточена в одном месте (мини-SDK). При добавлении Unreal — Unreal просто использует тот же AAR, не дублирует логику.

**Вариант 2** — меньше кода в Android, но бизнес-логика дублируется при добавлении каждого нового движка. Имеет смысл только если Unity — единственная цель.
