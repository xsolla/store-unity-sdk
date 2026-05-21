# Анализ: новая Java-библиотека для Unity/Unreal

## Постановка задачи

- Текущий Android SDK (`store-android-sdk`) остается как есть — для нативных Android-приложений
- Создается **новая библиотека** на **чистом Java** (без Kotlin) специально для игровых движков (Unity, Unreal и т.д.)
- Минимум сторонних зависимостей — в идеале только стандартные Android API
- Содержит только тот нативный код, который реально вызывается из Unity

## Что библиотека должна уметь

Из 31 точки вызова (см. `android-native-api-calls.md`) вырисовываются **3 функциональных блока**:

| Блок | Что делает | Текущая реализация |
|---|---|---|
| **Платежи** | Открыть PayStation URL в WebView/CustomTabs/TWA, вернуть результат | XPayments + ActivityPayStation + UI utils |
| **Соцсеть-авторизация** | Запустить OAuth через нативный SDK или браузер, вернуть токен | XLogin + LoginSocial + UI activities |
| **Виджет-авторизация** | Открыть Xsolla Widget в браузере, вернуть токен | XLogin + ActivityAuth |

Плюс мелочи: хранение токенов (SharedPreferences), AnalyticsUtils, SocialNetwork enum.

## Что внутри текущего Android SDK реально делают используемые методы

### Платежи (XPayments)
1. `createIntentBuilder().build()` — **просто собирает URL** вида `https://secure.xsolla.com/paystation4?token=...&engine=android&sdk_v=...` и кладет его в Intent
2. `ActivityPayStation` — **открывает этот URL** в WebView / CustomTabs / TrustedWebActivity, слушает redirect
3. `Result.fromResultIntent()` — **парсит результат** (invoice_id, status) из redirect URI

**Вывод:** Никакой сложной логики. Это URL builder + WebView/browser activity.

### Соцсеть-авторизация (LoginSocial)
1. Проверяет наличие нативного SDK (Facebook/Google/WeChat/QQ) через ClassLoader
2. Если есть — запускает нативный OAuth flow, получает токен провайдера, **обменивает на Xsolla-токен через HTTP API**
3. Если нет — fallback на **браузер/WebView** с OAuth URL
4. Парсит redirect с token/code

**Вывод:** Основная сложность — интеграция с 4 соц. SDK + один HTTP-вызов для обмена токена.

### Виджет-авторизация
1. Формирует URL виджета: `https://login.xsolla.com/api/social/...`
2. Открывает в CustomTabs/WebView
3. Парсит redirect с token

**Вывод:** Аналогично платежам — URL + browser activity.

## Что можно выкинуть при переписывании

| Что выкидываем | LOC | Почему не нужно |
|---|---|---|
| 46 методов XLogin (login, register, friends, devices, phone, avatar...) | ~1,200 | Unity вызывает эти API через HTTP из C# напрямую |
| lib-login-api целиком (LoginApiImpl, сетевой слой) | ~1,844 | Нужен только для выкинутых методов. Для social auth достаточно 1 HTTP-вызова |
| 31 из 35 callback-интерфейсов | ~200 | Связаны с выкинутыми методами |
| Entity/DTO для выкинутых API | ~500 | Не нужны |
| StatusTracker + PaymentsApi (polling статуса) | ~200 | Unity не использует серверный polling |
| **Итого выкидываем** | **~3,900** | |

## Избавление от Kotlin и сторонних зависимостей

### Масштаб проблемы: полное дерево транзитивных зависимостей

6 текущих Xsolla-зависимостей тянут за собой огромное дерево. Прямые зависимости из POM-файлов:

**login:6.0.18** (10 прямых):
- kotlin-stdlib-jdk7, androidx.appcompat, androidx.core-ktx, androidx.browser
- gson
- ktor-client-core, ktor-client-okhttp, ktor-client-content-negotiation, ktor-serialization-kotlinx-json
- kotlinx-coroutines-core

**payments:1.4.20** (9 прямых):
- kotlin-stdlib-jdk7, androidx.appcompat, androidx.core-ktx, androidx.browser
- androidx.startup-runtime
- androidbrowserhelper
- okhttp3, retrofit2 converter-gson, kotlinx-serialization-json

**login-facebook:6.0.18**: facebook-login → facebook-core → facebook-common (каждый со своими зависимостями)
**login-google:6.0.18**: play-services-auth (тянет play-services-base, play-services-basement, и т.д.)
**login-wechat:6.0.18**: wechat-sdk-android-without-mta
**login-qq:6.0.18**: jar-файл

А **ktor** в свою очередь тянет ещё:
- ktor-http-jvm, ktor-events-jvm, ktor-websocket-serialization-jvm
- kotlin-stdlib-common, kotlin-stdlib-jdk8
- kotlinx-coroutines-jdk8, kotlinx-coroutines-slf4j
- slf4j-api

И **okhttp** тянет: okio-jvm, kotlin-stdlib
И **retrofit** тянет: retrofit-core, okhttp, gson

**Итого: 6 Xsolla-зависимостей разворачиваются в ~100+ JAR/AAR файлов** при полном резолве.

### Текущие зависимости → замена

| Зависимость | Зачем используется | Замена в Java-библиотеке | Сложность |
|---|---|---|---|
| **kotlin-stdlib** (+jdk7/jdk8/common) | Весь код на Kotlin | Переписать на Java | Объем работы, но технически тривиально |
| **kotlinx-coroutines** (+jdk8/slf4j) | `runBlocking` для async вызовов | Синхронные вызовы / Thread | Тривиально (и так обернуто в runBlocking) |
| **ktor** (core/okhttp/content-negotiation/serialization) | HTTP-клиент в login SDK | Не нужно (lib-login-api выкидываем). Для 1-2 вызовов — `HttpURLConnection` | — |
| **OkHttp3** + okio | HTTP-клиент в payments | `HttpURLConnection` (Android built-in) | Просто |
| **Retrofit2 + Gson converter** | HTTP + JSON в payments status | Не нужно (status tracking выкидываем) | — |
| **kotlinx-serialization-json** | JSON в lib-login-api и payments | `org.json.JSONObject` (Android built-in) | Тривиально |
| **Gson** | JWT парсинг | `org.json.JSONObject` (Android built-in) | Тривиально |
| **slf4j-api** | Логирование (через ktor) | `android.util.Log` (Android built-in) | — |
| **androidbrowserhelper** | TrustedWebActivity | Убираем TWA | — |
| **androidx.startup** | Инициализация payments | Не нужно | — |
| **androidx.browser** | CustomTabs | **Оставить** — стандартная Android-библиотека | Нужна |
| **androidx.appcompat** | Базовые Activity | **Оставить** или заменить на `android.app.Activity` | Просто |

### Соцсети (compileOnly / опционально)

| SDK | Нужен? | Комментарий |
|---|---|---|
| Facebook SDK | Опционально | Если нет — fallback на browser OAuth. Проверяется через ClassLoader |
| Google Play Services Auth | Опционально | Аналогично |
| WeChat SDK | Опционально | Аналогично |
| QQ SDK | Опционально | Аналогично |

## Итоговая архитектура новой библиотеки

### Зависимости

```
Обязательные:
  - androidx.browser (CustomTabs) — ~100 KB, стандартная

Опциональные (compileOnly):
  - Facebook SDK
  - Google Play Services Auth
  - WeChat SDK
  - QQ SDK

Убраны полностью:
  - Kotlin runtime          ✓
  - kotlinx-coroutines      ✓
  - kotlinx-serialization   ✓
  - OkHttp3                 ✓
  - Retrofit2               ✓
  - Gson                    ✓
  - androidbrowserhelper    ✓ (если убрать TWA)
```

### Структура файлов (~15-20 Java-файлов)

```
com.xsolla.android.engine/
├── XsollaPayments.java          — IntentBuilder + URL generation (~200 LOC)
├── XsollaLogin.java             — init + social auth + widget auth + tokens (~300 LOC)
├── XsollaSocialAuth.java        — LoginSocial переписанный (~500 LOC)
├── SocialNetwork.java           — enum (~50 LOC)
├── LoginConfig.java             — конфиг + builder (~80 LOC)
├── TokenStorage.java            — SharedPreferences обертка (~50 LOC)
├── AnalyticsUtils.java          — 2 поля (~15 LOC)
├── callback/
│   ├── AuthCallback.java        — interface (~10 LOC)
│   └── BrowserCallback.java     — interface (~10 LOC)
├── data/
│   ├── AccessToken.java         — data class (~15 LOC)
│   ├── PaymentResult.java       — Parcelable (~30 LOC)
│   └── PaymentStatus.java       — enum (~10 LOC)
├── ui/
│   ├── PayStationActivity.java  — WebView + CustomTabs (~600 LOC)
│   ├── SocialAuthActivity.java  — browser для OAuth (~200 LOC)
│   ├── WidgetAuthActivity.java  — browser для виджета (~150 LOC)
│   └── WebViewAuthActivity.java — WebView fallback для соцсетей (~200 LOC)
└── internal/
    └── HttpHelper.java          — HttpURLConnection обертка (~100 LOC)
```

### Оценка размера

| Компонент | LOC |
|---|---|
| Платежи (URL builder + PayStationActivity) | ~850 |
| Авторизация (social + widget + UI) | ~1,200 |
| Общее (config, tokens, callbacks, data) | ~300 |
| HTTP helper | ~100 |
| **Итого** | **~2,500 LOC** |

## Сравнение

| Параметр | Текущее (3 модуля Kotlin) | Новая Java-библиотека |
|---|---|---|
| Язык | Kotlin | Java |
| Файлов | 115 | ~15-20 |
| LOC | 10,335 | ~2,500 |
| Сторонние зависимости | 8+ (okhttp, retrofit, gson, coroutines, kotlin-serialization...) | 1 (androidx.browser) |
| Размер AAR | ~300+ KB + Kotlin runtime ~1.5 MB | ~50-80 KB |
| Совместимость с Unity | Проблемы с Kotlin runtime и резолвом зависимостей | Без проблем |

## Оценка сложности

| Аспект | Оценка |
|---|---|
| Переписать XPayments + PayStationActivity | Просто — URL builder + WebView, чистая логика |
| Переписать LoginSocial (соцсети) | Средне — 4 SDK интеграции через reflection, fallback логика |
| Переписать Widget auth | Просто — CustomTabs + redirect parsing |
| Заменить OkHttp/Retrofit на HttpURLConnection | Просто — нужен 1-2 HTTP вызова максимум |
| Убрать Kotlin | Механическая работа — синтаксический перевод |
| Тестирование | Средне — нужно проверить все OAuth flow + все соцсети |

**Общая сложность: низкая-средняя.** Основной объем — механический перевод Kotlin→Java и вырезание ненужного. Архитектурных рисков мало, т.к. по сути это URL builders + browser activities.

## Главные риски

1. **LoginSocial** — самая сложная часть. Интеграция с 4 соц. SDK через reflection, fallback на браузер, обмен токенов. Нужно тщательно протестировать каждый провайдер.
2. **ActivityPayStation** — JavaScript-инъекции для отслеживания XHR/fetch в WebView. Нужно аккуратно перенести.
3. **Deep link / redirect handling** — парсинг callback URL должен работать идентично.

---

## Вариант: .java файлы прямо в Unity (без библиотеки)

### Можно ли вообще обойтись без AAR/библиотеки?

У Unity уже есть такой паттерн — `PaymentsProxyActivity.java`, `SocialAuthProxyActivity.java` лежат прямо в `Assets/Xsolla/Core/Plugins/Android/` как обычные .java файлы. Unity компилирует их при сборке Android-билда. Вопрос — можно ли **заинлайнить всю логику** из AAR прямо туда.

### Что мешает

**Проблема 1: Android Resources (R.layout, R.id)**

`ActivityPayStation` использует XML layout (`xsolla_payments_activity_paystation.xml`) с `R.layout` и `R.id`:
```java
setContentView(R.layout.xsolla_payments_activity_paystation);
webView = findViewById(R.id.webview);
loader = findViewById(R.id.loader);
```

Для raw .java файлов в Unity **нет системы Android Resources**. Варианты обхода:
- **Создать layout программно** вместо XML — WebView + FrameLayout + ProgressBar в коде. Это ~30-40 строк Java вместо XML, вполне реально.
- Либо положить layout XML в `Assets/Plugins/Android/res/layout/` — Unity поддерживает это, но это уже почти AAR.

**Проблема 2: AndroidManifest — регистрация Activity**

Каждая Activity должна быть в AndroidManifest. Текущий `AndroidManifestPreprocess.cs` уже делает это для proxy activities. Для новых activity нужно аналогично дописать.

**Проблема 3: Количество файлов**

Если вся логика в .java файлах:
- PayStationActivity — **~700-800 LOC** (WebView + CustomTabs + JS injection + network monitoring + popup handling + download). Это большой файл, но один.
- SocialAuth — **~500 LOC** (reflection для 4 SDK, fallback, HTTP вызов)
- WidgetAuth — **~150 LOC**
- Хелперы — **~300 LOC** (URL builder, browser utils, tokens)
- **Итого: ~1,700-2,000 LOC в ~5-8 .java файлах**

### Решение — отказ от XML layout

`ActivityPayStation` использует простой layout:
```xml
FrameLayout
  ├── WebView (main)
  ├── WebView (child/popup, visibility=gone)
  └── FrameLayout (loader)
        ├── View (background #2c2640)
        ├── ProgressBar
        └── TextView (no connection)
```

Это элементарно создать программно:
```java
// Вместо setContentView(R.layout....)
FrameLayout root = new FrameLayout(this);
WebView webView = new WebView(this);
root.addView(webView, MATCH_PARENT, MATCH_PARENT);
// ... loader, progressbar
setContentView(root);
```

### Зависимости

CustomTabs обязателен → `androidx.browser` остаётся. Это стандартная AndroidX библиотека, конфликтов с Unity/Unreal не создаёт. TrustedWebActivity (androidbrowserhelper) — убираем.

**Итого: 1 зависимость** (`androidx.browser`) вместо текущих 8+.

### Рекомендация: отдельная AAR-библиотека

Библиотека будет использоваться в нескольких движках (Unity, Unreal, другие) → **AAR однозначно лучше**, чем raw .java:

| Фактор | Raw .java в каждом движке | Одна AAR-библиотека |
|---|---|---|
| **Кол-во копий кода** | Дублируется в Unity, Unreal, и т.д. | 1 место |
| **Обновление** | Менять в 2-3 репозиториях | Обновить AAR, подтянуть версию в движках |
| **Тестирование** | Отдельно в каждом движке | Один раз — unit/integration тесты в Android-проекте |
| **Android Resources** | Нет — layout программно | Есть — можно использовать XML layout |
| **ProGuard** | Не нужен | Нужны keep rules (тривиально) |
| **CI/CD** | Не нужен | Gradle + публикация в Maven |

**Подход:**
1. Создать отдельный Android-проект (`xsolla-engine-sdk` или подобное)
2. Чистый Java, без Kotlin
3. Единственная зависимость: `androidx.browser`
4. Опциональные compileOnly: Facebook SDK, Google Play Services Auth, WeChat SDK, QQ SDK
5. Собирается в AAR, публикуется в Maven Central рядом с текущим SDK
6. Каждый движок (Unity, Unreal) подключает AAR как зависимость

**Структура проекта:**

```
xsolla-engine-sdk/
├── build.gradle
├── src/main/
│   ├── AndroidManifest.xml
│   ├── java/com/xsolla/android/engine/
│   │   ├── payments/
│   │   │   ├── XsollaPayments.java       — IntentBuilder + URL generation (~200 LOC)
│   │   │   ├── PayStationActivity.java   — WebView + CustomTabs (~700 LOC)
│   │   │   ├── AccessToken.java          — data class (~15 LOC)
│   │   │   ├── PaymentResult.java        — Parcelable (~30 LOC)
│   │   │   ├── PaymentStatus.java        — enum (~10 LOC)
│   │   │   └── BrowserCallback.java      — interface (~10 LOC)
│   │   ├── login/
│   │   │   ├── XsollaLogin.java          — init + tokens + facade (~300 LOC)
│   │   │   ├── SocialAuth.java           — social OAuth логика (~500 LOC)
│   │   │   ├── WidgetAuth.java           — widget auth flow (~150 LOC)
│   │   │   ├── SocialNetwork.java        — enum (~50 LOC) 
│   │   │   ├── LoginConfig.java          — конфиг + builder (~80 LOC)
│   │   │   ├── AuthCallback.java         — interface (~10 LOC)
│   │   │   └── TokenStorage.java         — SharedPreferences (~50 LOC)
│   │   └── util/
│   │       ├── AnalyticsUtils.java       — static fields (~15 LOC)
│   │       ├── BrowserUtils.java         — CustomTabs/WebView helpers (~200 LOC)
│   │       └── HttpHelper.java           — HttpURLConnection обертка (~100 LOC)
│   └── res/
│       └── layout/
│           └── xsolla_engine_activity_paystation.xml
└── src/test/  — unit тесты
```

**Оценка размера:**

| Компонент | LOC |
|---|---|
| Платежи (PayStationActivity + URL builder + result) | ~950 |
| Авторизация (social + widget + config + tokens) | ~1,150 |
| Утилиты (browser, http, analytics) | ~315 |
| **Итого** | **~2,400 LOC** |

**Maven-координаты (пример):**
```
com.xsolla.android:engine:1.0.0
```

Подключение в Unity (`XsollaDependencies.xml`):
```xml
<androidPackage spec="com.xsolla.android:engine:1.0.0"/>
<!-- Вместо текущих 6 зависимостей -->
```

Подключение в Unreal (`Build.cs` или Gradle):
```
implementation 'com.xsolla.android:engine:1.0.0'
```

### Что меняется на стороне Unity SDK

При переходе на новую библиотеку, на стороне Unity меняется минимально:

1. `XsollaDependencies.xml` — 6 зависимостей → 1
2. `AndroidHelper.cs` — имена классов/пакетов (вместо `com.xsolla.android.login.XLogin` → `com.xsolla.android.engine.login.XsollaLogin`)
3. Proxy activities (.java в Unity) — возможно упростить или убрать, если новая библиотека предоставит единый API
4. Всё остальное (C# код, Editor scripts) — остаётся как есть

### План реализации

| Шаг | Что делать | Оценка |
|---|---|---|
| 1 | Создать Android-проект `xsolla-engine-sdk` с Gradle | 1 день |
| 2 | Переписать PayStationActivity (Kotlin→Java, убрать TWA) | 2-3 дня |
| 3 | Переписать SocialAuth (Kotlin→Java, reflection для соц. SDK) | 2-3 дня |
| 4 | Переписать WidgetAuth + LoginConfig + TokenStorage | 1 день |
| 5 | Заменить OkHttp/Retrofit на HttpURLConnection | 1 день |
| 6 | Утилиты (BrowserUtils, AnalyticsUtils) | 0.5 дня |
| 7 | Тестирование (все OAuth flow + все соцсети + платежи) | 3-5 дней |
| 8 | Публикация в Maven Central | 1 день |
| 9 | Интеграция в Unity SDK (обновить зависимости + пакеты) | 1-2 дня |
| 10 | Интеграция в Unreal SDK | 1-2 дня |
| **Итого** | | **~13-18 рабочих дней** |
