# TASK-17: интеграция `com.xsolla.android:bridge` в Unity SDK — план

Цель: заменить `com.xsolla.android:payments` + `login`/`login-facebook`/`login-google`/`login-wechat`/`login-qq`
на единый `com.xsolla.android:bridge` в Android-части Unity SDK.

Вывод анализа: это **не** просто смена неймспейсов в классах. `bridge` уже содержит готовые drop-in
замены для всех Java-прокси Unity SDK с идентичными сигнатурами `perform(...)`, но флоу авторизации
поменялся (deep link вместо `onActivityResult`), поэтому старые прокси-классы Unity нужно удалить,
а не просто перепаковать.

## Что совпадает 1:1 (только смена FQCN, сигнатуры те же)

| Unity SDK сейчас | `bridge`-эквивалент |
|---|---|
| `PaymentsProxyActivity.perform(...)` | `com.xsolla.android.bridge.payments.ui.PayStationBridgeActivity.perform(...)` (сигнатура уже проверена в `Assets/Bridge/PaymentsTest.cs`) |
| `SocialAuthProxyActivity.perform(...)` | `com.xsolla.android.bridge.login.social.SocialAuthBridgeActivity.perform(...)` |
| `XsollaWidgetAuthProxyActivity.perform(...)` | `com.xsolla.android.bridge.login.social.XsollaWidgetBridgeActivity.perform(...)` |
| `XLogin.init` / `SocialConfig` / `LoginConfig.OauthBuilder` | те же классы в `com.xsolla.android.bridge.login.*`, конструкторы/сеттеры идентичны |
| `login.callback.AuthCallback`, `payments.callback.BrowserCallback` | те же интерфейсы, те же методы |
| `login.social.SocialNetwork` | побайтово идентичный enum (сверено со старым `SocialNetwork.kt`) |
| `AnalyticsUtils` (login/payments) | те же классы, те же статик-поля `gameEngine`/`gameEngineVersion` |

## Что реально нужно поменять

### 1. Удалить старые Java-прокси Unity SDK (не просто перепаковать)

`bridge`'s `LoginSocial.startSocialAuth` не делает `startActivityForResult` — открывает Custom Tab
и ждёт **deep link** через `DeeplinkReceiverActivity` / `AuthFlowCoordinator` (манифест
`app://xlogin.{pkg}`). Если оставить старые прокси и просто сменить импорт, их `onActivityResult`
никогда не вызовется — авторизация тихо повиснет.

Удалить:
- `Assets/Xsolla/Core/Plugins/Android/PaymentsProxyActivity.java`
- `Assets/Xsolla/Core/Plugins/Android/SocialAuthProxyActivity.java`
- `Assets/Xsolla/Core/Plugins/Android/XsollaWidgetAuthProxyActivity.java`
- `Assets/Xsolla/Core/Plugins/Android/WXEntryActivity.java`
- `Assets/Xsolla/Core/Plugins/Android/BrowserCallback.java`

### 2. Точки в C#: заменить FQCN-строки (сигнатуры вызовов не менять)

- `Assets/Xsolla/Core/Android/AndroidPayments.cs` —
  `$"{Application.identifier}.androidProxies.PaymentsProxyActivity"` →
  `"com.xsolla.android.bridge.payments.ui.PayStationBridgeActivity"`
- `Assets/Xsolla/Core/Android/AndroidSocialAuth.cs` — аналогично на `SocialAuthBridgeActivity` +
  `"com.xsolla.android.login.social.SocialNetwork"` → `"com.xsolla.android.bridge.login.social.SocialNetwork"`
- `Assets/Xsolla/Auth/Internal/WidgetAuthenticators/AndroidWidgetAuthenticator.cs` — аналогично на
  `XsollaWidgetBridgeActivity`
- `Assets/Xsolla/Core/Android/AndroidHelper.cs` — `com.xsolla.android.login.*` →
  `com.xsolla.android.bridge.login.*` (`XLogin`, `SocialConfig`, `LoginConfig`, `AnalyticsUtils` x2)
- `Assets/Xsolla/Core/Android/AndroidAuthCallback.cs` — база `AndroidJavaProxy` →
  `"com.xsolla.android.bridge.login.callback.AuthCallback"`
- `Assets/Xsolla/Core/Android/AndroidBrowserCallback.cs` — база `AndroidJavaProxy` →
  `"com.xsolla.android.bridge.payments.callback.BrowserCallback"`

### 3. WeChat/QQ — убрать нативную обвязку

`bridge` трактует WeChat и QQ как обычный browser-OAuth, как и все остальные провайдеры;
нативного Tencent SDK внутри нет.

- Удалить `AndroidManifestPreprocess.SetupWeChat()` и вызов `SetupWeChat()` из `OnPreprocessBuild`
- Удалить `SetupActivity("WXEntryActivity", ...)` из `AndroidScriptsProcessor.SetupActivities(...)`

### 4. Убрать ручную инъекцию манифеста для прокси-активити

`bridge` уже декларирует все свои активити (`DeeplinkReceiverActivity`, `SocialAuthBridgeActivity`,
`XsollaWidgetBridgeActivity`, `ActivityPayStation`, `PayStationBridgeActivity`) в собственном
AAR-манифесте — он подмешивается через manifest merger автоматически.

- `AndroidManifestPreprocess.cs` — удалить 3x `SetupProxyActivity(...)` (Payments/SocialAuth/XsollaWidgetAuth)
- `AndroidScriptsProcessor.cs` — удалить соответствующие 3x `SetupActivity(...)` для этих же классов

### 5. `XsollaDependencies.xml`

Файл: `Assets/Xsolla/Core/Editor/XsollaDependencies.xml` (там уже стоит TODO-комментарий
"TASK-2: bridge stub for local integration testing. Remove in TASK-17.").

Заменить 6 записей (`payments`, `login`, `login-facebook`, `login-google`, `login-wechat`, `login-qq`)
на одну `com.xsolla.android:bridge:...`. На момент написания плана `bridge` публикуется только в
`mavenLocal` (`0.1.0-LOCAL`) — реальный репозиторий для публикации ещё не выбран (см. TASK-16 writeup).

## Проверить перед закрытием TASK-17 (не пробелы в коде, но риски поведения)

- **CustomTabs/TWA-редирект payments** жёстко завязан на `app://xpayment.{applicationId}`
  (зашито в манифесте `bridge`). Совпадает с дефолтом `RedirectPolicySettings.CreateDefaultPaymentsPolicy()`
  — стандартный кейс ок. Но если паблишер задаёт свой кастомный Return URL **и** выбирает
  CustomTabs/TWA — deep link не поймается (WebView не пострадает, там перехват через сам WebView).
  Нужно прогнать этот кейс отдельно.
- Пока вживую проверен только `activityType = "WebView"` (см. `Assets/Bridge/PaymentsTest.cs`).
  CustomTabs и TrustedWebActivity пути в `ActivityPayStation` есть в коде, но не подтверждены тестом.
- WeChat/QQ теперь идут через тот же generic OAuth-редирект, что и остальные провайдеры — нужно
  убедиться, что бэкенд `login.xsolla.com/api/oauth2/social/wechat|qq/login_url` поддерживает чисто
  браузерный флоу без native app-switch (продуктовый/бэкенд вопрос, не код).

## Итог

Всё необходимое API уже есть в `bridge` — новых методов дописывать не нужно. Работа TASK-17 —
это направленный рефакторинг integration-слоя (удаление старых прокси + точечная замена FQCN в
C#/editor-скриптах + смена зависимостей), а не разработка недостающей функциональности.
