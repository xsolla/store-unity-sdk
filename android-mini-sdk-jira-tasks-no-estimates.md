# Android Mini-SDK для Game Engines — Jira задачи

## Epic: Android Mini-SDK для Unity / Unreal

**Описание Epic:**
Создать новую Android библиотеку `com.xsolla.android:engine` на чистом Java (без Kotlin) как замену текущим 6 Maven зависимостям (~100+ JAR). Библиотека должна поддерживать Unity, Unreal и другие движки. Изменений в Unity C# коде не требуется — сохраняются те же API-контракты.

**Цель:** устранить конфликты зависимостей в Unity проектах, убрать Kotlin runtime, уменьшить размер с ~100+ JAR до 2-3 зависимостей.

---

## Процесс тестирования через Local Maven

Вместо публикации в Maven Central на каждой итерации используется локальный Maven-репозиторий (`~/.m2/`). Это позволяет тестировать интеграцию в Unity после каждой значимой задачи.

### Цикл разработки

```
1. Внести изменения в Android-проект
2. ./gradlew publishToMavenLocal
   → публикует AAR + POM в ~/.m2/repository/com/xsolla/android/engine/VERSION/
3. В Unity Editor: Assets → External Dependency Manager → Android Resolver → Force Resolve
   → EDM читает POM, скачивает AAR из ~/.m2/ в Assets/Plugins/Android/
4. Собрать Android APK и проверить на устройстве / эмуляторе
```

### Настройка Unity (один раз, в рамках TASK-2)

В `Assets/Xsolla/Core/Editor/XsollaDependencies.xml` добавить `mavenLocal` и новую зависимость:

```xml
<androidPackages>
  <repositories>
    <repository>mavenLocal</repository>
  </repositories>
  <androidPackage spec="com.xsolla.android:engine:0.1.0-LOCAL"/>
  <!-- старые 6 зависимостей убираются в TASK-17 -->
</androidPackages>
```

### Версия для локальной разработки

В `build.gradle` Android-проекта использовать суффикс `-LOCAL`, чтобы отличать от релизных версий:

```groovy
version = '0.1.0-LOCAL'
```

### Транзитивные зависимости

`publishToMavenLocal` публикует POM-файл с объявленными зависимостями. EDM читает его и подтягивает `androidx.browser` из Google Maven автоматически — ничего дополнительно прописывать не нужно.

---

## Milestone 1: Настройка Android проекта

### TASK-1: Создать Android Library проект (xsolla-engine-sdk)

**Тип:** Task
**Приоритет:** P1 — Blocker

**Описание:**
Создать новый Android library проект `xsolla-engine-sdk` с нуля. Проект должен компилироваться без Kotlin.

**Acceptance Criteria:**
- [ ] Структура проекта создана: `build.gradle`, `settings.gradle`, `src/main/AndroidManifest.xml`
- [ ] `build.gradle` использует только `com.android.library` плагин (без `kotlin-android`)
- [ ] `minSdkVersion = 21`, `compileSdkVersion = 34`
- [ ] `compileOptions` настроены на Java 1.8
- [ ] Зависимости: `androidx.appcompat:1.6.1`, `androidx.browser:1.8.0`, `com.google.androidbrowserhelper:2.5.0`
- [ ] Пустой проект успешно собирается: `./gradlew assembleRelease`
- [ ] В AAR нет Kotlin stdlib
- [ ] `maven-publish` плагин настроен в `build.gradle`, `./gradlew publishToMavenLocal` выполняется успешно

**Пакетная структура:**
```
src/main/java/com/xsolla/android/
  ├── login/
  │   ├── callback/
  │   ├── social/
  │   ├── token/
  │   └── util/
  └── payments/
      ├── callback/
      ├── ui/
      └── util/
```

---

### TASK-2: Stub-класс + первая проверка интеграции в Unity

**Тип:** Task
**Приоритет:** P1 — Blocker
**Depends on:** TASK-1

**Описание:**
Создать минимальный stub-класс с одним методом, который пишет в лог. Опубликовать через `publishToMavenLocal`, подключить в Unity и убедиться, что вызов из C# работает — ещё до реализации какой-либо реальной функциональности.

**Цель:** как можно раньше проверить, что вся цепочка работает: Android-проект → mavenLocal → EDM → Unity → вызов Java-метода → logcat.

**Stub-класс:**
```java
// src/main/java/com/xsolla/android/engine/XsollaEngine.java
package com.xsolla.android.engine;

import android.util.Log;

public class XsollaEngine {
    public static void init() {
        Log.d("XsollaEngine", "XsollaEngine.init() called — integration works!");
    }
}
```

**Вызов из Unity C#:**
```csharp
var cls = new AndroidJavaClass("com.xsolla.android.engine.XsollaEngine");
cls.CallStatic("init");
// В logcat должно появиться: XsollaEngine: XsollaEngine.init() called — integration works!
```

**Acceptance Criteria:**
- [ ] `./gradlew publishToMavenLocal` кладёт AAR + POM в `~/.m2/repository/com/xsolla/android/engine/0.1.0-LOCAL/`
- [ ] `XsollaDependencies.xml` содержит `mavenLocal` и `com.xsolla.android:engine:0.1.0-LOCAL`
- [ ] External Dependency Manager резолвит зависимость из `~/.m2/` без ошибок
- [ ] Unity Android-билд собирается без ошибок
- [ ] На устройстве в logcat видна строка `XsollaEngine.init() called — integration works!`

---

## Milestone 2: Login Module — общие типы

### TASK-3: SocialNetwork enum

**Тип:** Task
**Приоритет:** P1
**Depends on:** TASK-1

**Описание:**
Создать `com.xsolla.android.login.social.SocialNetwork` Java enum. Это точная замена `SocialNetwork.kt` из текущего Android SDK. Unity обращается к этому enum по имени через AndroidJavaClass.GetStatic().

**Файл:** `src/main/java/com/xsolla/android/login/social/SocialNetwork.java`

**Acceptance Criteria:**
- [ ] Enum содержит все 31 значение: GOOGLE, FACEBOOK, TWITTER, LINKEDIN, NAVER, BAIDU, AMAZON, APPLE, BATTLENET, DISCORD, GITHUB, KAKAO, MAILRU, MICROSOFT, MSN, OK, PAYPAL, PSN, QQ, REDDIT, STEAM, TWITCH, VIMEO, VK, WECHAT, WEIBO, YAHOO, YANDEX, YOUTUBE, XBOX, BABKA
- [ ] Каждое значение имеет публичное поле `String providerName`
- [ ] Unity код `SocialNetwork.GOOGLE` доступен через AndroidJavaClass.GetStatic

---

### TASK-4: Login Callback интерфейсы

**Тип:** Task
**Приоритет:** P1
**Depends on:** TASK-1

**Описание:**
Создать Java интерфейсы для callback'ов авторизации. Эти интерфейсы реализуются в Unity C# через `AndroidJavaProxy` — поэтому сигнатуры методов должны совпадать точно.

**Файлы:**
- `com/xsolla/android/login/callback/AuthCallback.java`
- `com/xsolla/android/login/callback/StartSocialCallback.java`
- `com/xsolla/android/login/callback/FinishSocialCallback.java`
- `com/xsolla/android/login/callback/FinishXsollaWidgetAuthCallback.java`

**Acceptance Criteria:**
- [ ] `AuthCallback`: методы `onSuccess()`, `onError(Throwable throwable, String errorMessage)`
- [ ] `StartSocialCallback`: методы `onAuthStarted()`, `onError(Throwable throwable, String errorMessage)`
- [ ] `FinishSocialCallback`: методы `onAuthSuccess()`, `onAuthCancelled()`, `onAuthError(Throwable throwable, String errorMessage)`
- [ ] `FinishXsollaWidgetAuthCallback`: те же 3 метода что в `FinishSocialCallback`
- [ ] `AndroidAuthCallback.cs` в Unity SDK успешно компилируется с новыми интерфейсами

---

### TASK-5: TokenUtils — хранение токенов

**Тип:** Task
**Приоритет:** P1
**Depends on:** TASK-1

**Описание:**
Создать `TokenUtils.java` для хранения OAuth токенов в Android SharedPreferences. Токены читаются из Unity через `XLogin.getToken()` / `XLogin.getRefreshToken()`.

**Файл:** `com/xsolla/android/login/token/TokenUtils.java`

**Acceptance Criteria:**
- [ ] SharedPreferences с ключом `"XSOLLA_LOGIN"`
- [ ] Публичные поля: `String oauthAccessToken`, `String oauthRefreshToken`, `long oauthExpireTimeUnixSec`
- [ ] Изменение поля автоматически сохраняется в SharedPreferences
- [ ] Токены сохраняются между запусками приложения

---

### TASK-6: AnalyticsUtils (login + payments)

**Тип:** Task
**Приоритет:** P3
**Depends on:** TASK-1

**Описание:**
Два простых класса с публичными статическими полями для аналитики. Unity устанавливает их через `SetStatic()`.

**Файлы:**
- `com/xsolla/android/login/util/AnalyticsUtils.java`
- `com/xsolla/android/payments/util/AnalyticsUtils.java`

**Acceptance Criteria:**
- [ ] Оба класса имеют: `public static String gameEngine`, `public static String gameEngineVersion`
- [ ] `AndroidHelper.cs` строки 101-102 работают без изменений

---

## Milestone 3: Login Module — LoginConfig

### TASK-7: LoginConfig + OauthBuilder + SocialConfig

**Тип:** Task
**Приоритет:** P1
**Depends on:** TASK-3, TASK-4, TASK-5

**Описание:**
Создать `LoginConfig.java` с вложенными классами `OauthBuilder` и `SocialConfig`. Эти классы создаются в `AndroidHelper.cs` строки 74-87 через `AndroidJavaObject`. Сигнатуры конструкторов и методов должны совпадать точно.

**Файл:** `com/xsolla/android/login/LoginConfig.java`

**Сигнатуры (взяты из AndroidHelper.cs):**
```java
// new AndroidJavaObject("com.xsolla.android.login.XLogin$SocialConfig", fbAppId, fbClientToken, googleServerId, wechatAppId, qqAppId)
public static class SocialConfig {
    public SocialConfig(String fbAppId, String fbClientToken, String googleServerId, String wechatAppId, String qqAppId)
}

// new AndroidJavaObject("com.xsolla.android.login.LoginConfig$OauthBuilder")
public static class OauthBuilder {
    public OauthBuilder setSocialConfig(SocialConfig config)
    public OauthBuilder setProjectId(String projectId)
    public OauthBuilder setOauthClientId(int clientId)
    public LoginConfig build()
}
```

**Acceptance Criteria:**
- [ ] `new AndroidJavaObject("com.xsolla.android.login.XLogin$SocialConfig", ...)` работает из Unity
- [ ] `new AndroidJavaObject("com.xsolla.android.login.LoginConfig$OauthBuilder")` работает
- [ ] Все методы builder chain возвращают `this` / `LoginConfig`
- [ ] `AndroidHelper.cs` строки 74-87 компилируются и работают без изменений

---

## Milestone 4: Login Module — LoginSocial + XLogin

### TASK-8: LoginSocial — social auth flow

**Тип:** Task
**Приоритет:** P1
**Depends on:** TASK-3, TASK-4, TASK-5, TASK-7

**Описание:**
Создать `LoginSocial.java` — основная логика social авторизации. Заменяет `LoginSocial.kt` из текущего Android SDK. Содержит HTTP запросы к Xsolla Login API (замена OkHttp3 на HttpURLConnection).

**Файл:** `com/xsolla/android/login/social/LoginSocial.java`

**Методы:**
- `startSocialAuth(Activity, SocialNetwork, StartSocialCallback)` — открыть OAuth URL в браузере (через Intent или CustomTabs)
- `finishSocialAuth(Activity, SocialNetwork, int reqCode, int resCode, Intent data, FinishSocialCallback)` — обработать `onActivityResult`, извлечь code из redirect URI, обменять на токен
- `startXsollaWidgetAuth(Activity, StartXsollaWidgetAuthCallback, String locale)` — открыть виджет URL
- `finishXsollaWidgetAuth(Activity, int, int, Intent, FinishXsollaWidgetAuthCallback)` — обработать result, получить токен
- `getWechatAppId()` — возвращает WeChat App ID (используется в `WXEntryActivity.java`)
- `setWechatResult(Object resp)` — принимает WeChat OAuth ответ (используется в `WXEntryActivity.java`)

**HTTP вызовы (через HttpURLConnection, без OkHttp3):**
1. `GET https://login.xsolla.com/api/oauth2/social/{provider}/login_url` — получить URL для браузера
2. `POST https://login.xsolla.com/api/oauth2/social/{provider}/code` — обменять social token
3. `POST https://login.xsolla.com/api/oauth2/token` (form-urlencoded: code, grant_type, client_id, redirect_uri) — финальный обмен code → access_token + refresh_token

**Async pattern (без Kotlin coroutines):**
```java
Executors.newSingleThreadExecutor().execute(() -> {
    // HTTP call
    new Handler(Looper.getMainLooper()).post(() -> {
        // callback on main thread
    });
});
```

**Acceptance Criteria:**
- [ ] Нет зависимости от OkHttp3, Retrofit, kotlinx-serialization
- [ ] Все HTTP запросы через `java.net.HttpURLConnection`
- [ ] JSON парсинг через `org.json.JSONObject`
- [ ] Callbacks вызываются на main thread
- [ ] WeChat интеграция: `getWechatAppId()` и `setWechatResult()` работают с `WXEntryActivity.java`
- [ ] Social auth flow работает end-to-end (Google, браузерный вариант)

---

### TASK-9: XLogin — фасад (7 методов)

**Тип:** Task
**Приоритет:** P1
**Depends on:** TASK-7, TASK-8

**Описание:**
Создать `XLogin.java` — главный публичный фасад. Замена `XLogin.kt` (1796 LOC Kotlin → ~150 LOC Java). Реализует только 7 методов, которые реально вызываются из Unity SDK.

**Файл:** `com/xsolla/android/login/XLogin.java`

**Методы (из `android-native-api-calls.md`):**
```java
public static void init(Context context, LoginConfig loginConfig)
public static void startSocialAuth(Activity activity, SocialNetwork socialNetwork, StartSocialCallback callback)
public static void finishSocialAuth(Activity activity, SocialNetwork socialNetwork, int requestCode, int resultCode, Intent data, FinishSocialCallback callback)
public static void startAuthWithXsollaWidget(Activity activity, FinishXsollaWidgetAuthCallback callback, String locale)
public static void finishAuthWithXsollaWidget(Activity activity, int requestCode, int resultCode, Intent data, FinishXsollaWidgetAuthCallback callback)
public static String getToken()       // читает из TokenUtils.oauthAccessToken
public static String getRefreshToken() // читает из TokenUtils.oauthRefreshToken
```

**Acceptance Criteria:**
- [ ] Singleton паттерн: один экземпляр, хранит `loginConfig`, `tokenUtils`, `loginSocial`
- [ ] `init()` инициализирует все внутренние компоненты
- [ ] `getToken()` / `getRefreshToken()` возвращают значения из TokenUtils
- [ ] `AndroidHelper.cs` и все ProxyActivity файлы работают без изменений
- [ ] `AndroidAuthCallback.cs` строки 27-28 (`XLogin.getToken()`, `XLogin.getRefreshToken()`) работают

---

## Milestone 5: Payments Module

### TASK-10: Payments Callback интерфейс

**Тип:** Task
**Приоритет:** P1
**Depends on:** TASK-1

**Описание:**
Создать `BrowserCallback` интерфейс для payments. Unity реализует его через `AndroidJavaProxy` в `AndroidBrowserCallback.cs`.

**Файл:** `com/xsolla/android/payments/callback/BrowserCallback.java`

**Acceptance Criteria:**
- [ ] Метод: `void onBrowserClosed(boolean isManually)`
- [ ] `AndroidBrowserCallback.cs` успешно реализует этот интерфейс через AndroidJavaProxy

---

### TASK-11: XPayments — фасад и типы

**Тип:** Task
**Приоритет:** P1
**Depends on:** TASK-10

**Описание:**
Создать `XPayments.java` с вложенными классами. Замена `XPayments.kt`. Используется в `PaymentsProxyActivity.java`.

**Файл:** `com/xsolla/android/payments/XPayments.java`

**Вложенные классы (точные сигнатуры из `PaymentsProxyActivity.java`):**

```java
// Enum для статуса платежа
public enum Status { COMPLETED, CANCELLED, UNKNOWN }

// Enum для типа браузера
public enum ActivityType { WEB_VIEW, CUSTOM_TABS, TRUSTED_WEB_ACTIVITY }

// Токен для PayStation
public static class AccessToken implements Parcelable {
    public AccessToken(String token)
}

// Результат платежа
public static class Result implements Parcelable {
    public static Result fromResultIntent(Intent data)  // читает из intent extras
    public Status getStatus()
}

// Builder для запуска PayStation
public static IntentBuilder createIntentBuilder(Activity activity)

public static class IntentBuilder {
    public IntentBuilder accessToken(AccessToken token)
    public IntentBuilder isSandbox(boolean sandbox)
    public IntentBuilder payStationVersion(String version)  // "V3" или "V4"
    public IntentBuilder setRedirectUriScheme(String scheme)
    public IntentBuilder setRedirectUriHost(String host)
    public IntentBuilder setActivityType(ActivityType type)
    public Intent build()  // возвращает Intent для старта ActivityPayStation
}
```

**Acceptance Criteria:**
- [ ] `PaymentsProxyActivity.java` компилируется без изменений
- [ ] `XPayments.createIntentBuilder(activity).accessToken(...).isSandbox(...).payStationVersion(...).setRedirectUriScheme(...).setRedirectUriHost(...).setActivityType(...).build()` работает
- [ ] `XPayments.Result.fromResultIntent(data).getStatus()` возвращает правильный Status
- [ ] Intent extras ключи совпадают между `IntentBuilder.build()` и `ActivityPayStation`

---

## Milestone 6: ActivityPayStation (Browser UI)

### TASK-12: ActivityPayStation — WebView

**Тип:** Task
**Приоритет:** P2
**Depends on:** TASK-11

**Описание:**
Реализовать WebView режим в `ActivityPayStation.java`. Источник: `ActivityPayStation.kt` строки ~400-650.

**Файл:** `com/xsolla/android/payments/ui/ActivityPayStation.java`

**Функциональность:**
- Загрузить PayStation URL в WebView
- Перехват redirect URI через `doUpdateVisitedHistory()` (когда URL начинается с `redirectScheme`)
- JS injection для отслеживания XHR запросов (мониторинг активности страницы)
- Обработка ошибок загрузки (offline state, reload)
- Download listener для файлов
- Loader / progress indicator

**Acceptance Criteria:**
- [ ] WebView открывает PayStation URL
- [ ] При redirect на `{redirectScheme}://...` — перехватывает URL, парсит статус, вызывает `finishWithResult()`
- [ ] JS injection работает: `XPA.onRequestStart()`, `XPA.onRequestDone()`, `XPA.onRequestFail()`
- [ ] Обработка offline: WebView показывает ошибку, восстанавливается при появлении сети
- [ ] `XPayments.Result.fromResultIntent()` получает правильный результат

---

### TASK-13: ActivityPayStation — CustomTabs

**Тип:** Task
**Приоритет:** P2
**Depends on:** TASK-12

**Описание:**
Реализовать CustomTabs режим. PayStation открывается в системном браузере с Chrome CustomTabs. Redirect обрабатывается через `onNewIntent()`.

**Acceptance Criteria:**
- [ ] CustomTabs открывает PayStation URL
- [ ] Deep link redirect возвращается в `onNewIntent()`
- [ ] Статус платежа корректно определяется из redirect URI
- [ ] Activity корректно закрывается и передаёт результат в `PaymentsProxyActivity`

---

### TASK-14: ActivityPayStation — TWA (Trusted Web Activity)

**Тип:** Task
**Приоритет:** P2
**Depends on:** TASK-13

**Описание:**
Реализовать TWA режим через `com.google.androidbrowserhelper:androidbrowserhelper`. Аналогично CustomTabs но с более глубокой интеграцией в Chrome.

**Acceptance Criteria:**
- [ ] TWA запускается через `TrustedWebActivityIntentBuilder`
- [ ] Deep link redirect обрабатывается
- [ ] Результат платежа корректно возвращается

---

### TASK-15: ActivityPayStation — Status Polling

**Тип:** Task
**Приоритет:** P2
**Depends on:** TASK-12

**Описание:**
Реализовать polling статуса платежа через HTTP. Замена Retrofit2 на HttpURLConnection.

**Endpoint:** `GET https://secure.xsolla.com/paystation2/api/payments/status?access_token={token}`

**Логика:**
- Запускать после закрытия браузера
- Повторять каждые N секунд, до 30 попыток
- При статусе `done` — вернуть `Status.COMPLETED`
- При отмене — вернуть `Status.CANCELLED`
- Использовать `ScheduledExecutorService` (без Kotlin coroutines)

**Acceptance Criteria:**
- [ ] Нет зависимости от Retrofit2 / OkHttp3
- [ ] Polling работает на background thread
- [ ] Callback вызывается на main thread
- [ ] Таймаут: если за 30 попыток статус не определён — вернуть `Status.UNKNOWN`

---

## Milestone 7: Сборка и интеграция

### TASK-16: Сборка AAR и публикация

**Тип:** Task
**Приоритет:** P1
**Depends on:** TASK-9, TASK-11, TASK-12, TASK-13, TASK-14, TASK-15

**Описание:**
Собрать финальный AAR и подготовить к публикации.

**Acceptance Criteria:**
- [ ] `./gradlew assembleRelease` успешно собирает AAR
- [ ] AAR не содержит Kotlin stdlib
- [ ] Dependency tree AAR: только `androidx.browser` + `androidbrowserhelper` + `androidx.appcompat`
- [ ] AAR опубликован в Maven репозиторий (или доступен локально)

---

### TASK-17: Интеграция в Unity SDK

**Тип:** Task
**Приоритет:** P1
**Depends on:** TASK-16

**Описание:**
Обновить `XsollaDependencies.xml` в Unity SDK — заменить 6 старых зависимостей на одну новую.

**Файл:** `Assets/Xsolla/Core/Editor/XsollaDependencies.xml`

**Изменение:**
```xml
<!-- УДАЛИТЬ: -->
<androidPackage spec="com.xsolla.android:payments:1.4.20"/>
<androidPackage spec="com.xsolla.android:login:6.0.18"/>
<androidPackage spec="com.xsolla.android:login-facebook:6.0.18"/>
<androidPackage spec="com.xsolla.android:login-google:6.0.18"/>
<androidPackage spec="com.xsolla.android:login-wechat:6.0.18"/>
<androidPackage spec="com.xsolla.android:login-qq:6.0.18"/>

<!-- ДОБАВИТЬ: -->
<androidPackage spec="com.xsolla.android:engine:1.0.0"/>
```

**Acceptance Criteria:**
- [ ] Unity проект собирается для Android без ошибок
- [ ] Dependency resolver не подтягивает Kotlin / OkHttp / Retrofit

---

## Milestone 8: Тестирование

### TASK-18: Тестирование на устройстве

**Тип:** Task
**Приоритет:** P1
**Depends on:** TASK-17

**Тест-план:**

| # | Сценарий | Ожидаемый результат |
|---|----------|---------------------|
| 1 | Сборка Unity проекта для Android | Нет ошибок компиляции |
| 2 | Платежи: WebView mode | redirect → `Status.COMPLETED` |
| 3 | Платежи: CustomTabs mode | redirect → `Status.COMPLETED` |
| 4 | Платежи: TWA mode | redirect → `Status.COMPLETED` |
| 5 | Платежи: отмена пользователем | `Status.CANCELLED`, `isManually=true` |
| 6 | Social auth: Google (браузер) | `XLogin.getToken()` возвращает токен |
| 7 | Social auth: Facebook (если SDK) | токен получен |
| 8 | Widget auth: Xsolla Widget | токен получен |
| 9 | WeChat auth | `WXEntryActivity` отрабатывает |
| 10 | Init при старте | нет ошибок, AnalyticsUtils установлен |
| 11 | Dependency check | нет OkHttp / Kotlin в APK |

**Acceptance Criteria:**
- [ ] Все 11 тест-кейсов пройдены
- [ ] Размер APK уменьшился (нет ~100 JAR)
- [ ] Нет runtime ошибок при работе с Android

---

## Сводка по задачам

| Task | Название | Milestone | Приоритет |
|------|----------|-----------|-----------|
| TASK-1 | Android Library проект | M1 | P1 |
| TASK-2 | Stub-класс + первая проверка в Unity | M1 | P1 |
| TASK-3 | SocialNetwork enum | M2 | P1 |
| TASK-4 | Login Callback интерфейсы | M2 | P1 |
| TASK-5 | TokenUtils | M2 | P1 |
| TASK-6 | AnalyticsUtils (x2) | M2 | P3 |
| TASK-7 | LoginConfig + OauthBuilder + SocialConfig | M3 | P1 |
| TASK-8 | LoginSocial | M4 | P1 |
| TASK-9 | XLogin (фасад) | M4 | P1 |
| TASK-10 | BrowserCallback интерфейс | M5 | P1 |
| TASK-11 | XPayments + типы | M5 | P1 |
| TASK-12 | ActivityPayStation — WebView | M6 | P2 |
| TASK-13 | ActivityPayStation — CustomTabs | M6 | P2 |
| TASK-14 | ActivityPayStation — TWA | M6 | P2 |
| TASK-15 | ActivityPayStation — Status Polling | M6 | P2 |
| TASK-16 | Сборка AAR | M7 | P1 |
| TASK-17 | Интеграция в Unity SDK | M7 | P1 |
| TASK-18 | Тестирование на устройстве | M8 | P1 |
