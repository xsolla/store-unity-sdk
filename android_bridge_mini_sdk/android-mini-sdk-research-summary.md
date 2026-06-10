# [Research] Android Mini-SDK для Unity и Unreal: итоги исследования

## Коротко о главном

**Делаем.** Создать новую Android библиотеку на чистом Java (`com.xsolla.android:engine`), которая заменит текущие 6 Kotlin-зависимостей. Те же API-контракты — изменений в Unity C# коде не требуется. Оценка: ~3–4 недели.

---

## Резюме

Полный аудит Unity SDK выявил 31 точку вызова Android-кода, которые сводятся к трём функциональным блокам: открытие PayStation в браузере, OAuth через соцсети и авторизация через Xsolla Widget. При этом из 56 методов XLogin Unity использует только 7 — всё остальное уже вызывается напрямую через HTTP из C#.

Корневая причина конфликтов зависимостей — Kotlin runtime. Шесть Maven-зависимостей разворачиваются в 100+ JAR, каждый из которых тянет свою версию kotlin-stdlib, kotlinx-coroutines и okhttp. При совместном использовании с другими плагинами EDM получает несовместимые версии и либо падает при резолве, либо собирает билд с ошибками в runtime.

Анализ конкурентов (Facebook Unity SDK, Firebase, Google Sign-In) показал единый отраслевой паттерн: Android-часть — это тонкая обёртка на ~150–200 строк Java, которая занимается только системным UI и нативными flow, а вся HTTP-логика и бизнес-логика живёт в C#. Предлагаемая архитектура мини-SDK точно следует этому паттерну.

Рекомендуется создать отдельный Android library проект (`xsolla-engine-sdk`) на чистом Java с одной внешней зависимостью (`androidx.browser`). Библиотека сохраняет те же имена классов и сигнатуры методов, что сейчас вызываются из Unity, — на стороне Unity меняется только один файл (`XsollaDependencies.xml`: 6 зависимостей → 1). Объём: ~2 500 строк Java, 18 задач, 3–4 недели.

Главные риски — сложность LoginSocial (интеграция с 4 соцсетями через reflection) и JS-инъекции в ActivityPayStation — хорошо понятны и покрываются прямым портированием существующего Kotlin-кода. Операционный риск: при добавлении новых фич в основной Android SDK мини-SDK нужно обновлять вручную, однако набор используемых методов стабилен и редко меняется.

**Вывод: делаем.** Проблема чётко локализована, решение технически прямолинейно, изменений в Unity C# коде не требуется, библиотека переиспользуется в Unreal без дополнительных затрат.

---

## 1. Инвентаризация нативного Android-кода

### Что Unity реально вызывает из Android

Полный аудит кодовой базы Unity SDK выявил **31 точку вызова** в 6 группах:

| Группа | Кол-во вызовов | Файлы |
|---|---|---|
| XLogin (авторизация) | 7 | `AndroidHelper.cs`, `AndroidAuthCallback.cs`, proxy activities |
| LoginConfig (инициализация) | 6 | `AndroidHelper.cs` |
| XPayments (платежи) | 10 | `PaymentsProxyActivity.java` |
| AnalyticsUtils | 4 (2 класса × 2 поля) | `AndroidHelper.cs` |
| WeChat SDK | 4 | `WXEntryActivity.java` |
| SocialNetwork enum | 1 | `AndroidSocialAuth.cs` |

### Функциональные блоки

Все 31 вызов сводятся ровно к **3 функциональным блокам**:

**Блок 1 — Платежи**
Открыть URL PayStation в WebView / Chrome Custom Tabs / Trusted Web Activity, перехватить redirect, вернуть статус платежа. Никакой бизнес-логики — чистый браузерный UI.

**Блок 2 — Авторизация через соцсети**
Запустить OAuth flow через нативный SDK (Facebook/Google/WeChat/QQ) или браузер в качестве fallback. Обменять auth code на Xsolla-токен через один HTTP-запрос. Вернуть токен в Unity.

**Блок 3 — Виджет-авторизация**
Открыть URL Xsolla Widget в браузере, перехватить redirect с токеном. Паттерн идентичен платежам.

### Что Unity НЕ использует из текущего Android SDK

У XLogin 56 публичных методов. Unity использует 7. Остальные 49 (login, register, friends, devices, phone, avatar и т.д.) Unity вызывает напрямую через HTTP из C# — Android SDK в этих случаях вообще не задействован. Это означает, что **~3 900 строк Kotlin можно просто удалить** из новой библиотеки.

### Архитектурный паттерн Unity SDK

```
Unity C# (AndroidJavaClass / AndroidJavaProxy)
    ↓
Java Proxy Activities (PaymentsProxyActivity / SocialAuthProxyActivity / XsollaWidgetAuthProxyActivity)
    ↓
Native Android SDK (XLogin / XPayments)
    ↓
onActivityResult() → AndroidJavaProxy callback → Unity C#
```

Proxy activities уже существуют в `Assets/Xsolla/Core/Plugins/Android/` как обычные `.java` файлы, которые Unity компилирует при сборке Android-билда.

---

## 2. Анализ конфликтов зависимостей

### Текущее состояние

6 Maven-зависимостей из `XsollaDependencies.xml` разворачиваются в **100+ JAR/AAR файлов** после резолва транзитивных зависимостей:

| Прямая зависимость | Версия | Ключевые транзитивные зависимости |
|---|---|---|
| `com.xsolla.android:login` | 6.0.18 | kotlin-stdlib, ktor-client-core, ktor-client-okhttp, kotlinx-coroutines, gson |
| `com.xsolla.android:payments` | 1.4.20 | kotlin-stdlib, okhttp3, retrofit2, kotlinx-serialization, androidbrowserhelper |
| `com.xsolla.android:login-facebook` | 6.0.18 | facebook-login → facebook-core → facebook-common |
| `com.xsolla.android:login-google` | 6.0.18 | play-services-auth + транзитивная цепочка |
| `com.xsolla.android:login-wechat` | 6.0.18 | wechat-sdk-android-without-mta |
| `com.xsolla.android:login-qq` | 6.0.18 | Tencent QQ jar |

### Корневые причины конфликтов

**Главная причина — привязка версии Kotlin runtime.** Каждая зависимость декларирует свою версию `kotlin-stdlib`. Когда Unity-проект включает несколько плагинов (например, Xsolla + Firebase + что-то ещё), EDM видит несовместимые версии Kotlin stdlib и либо падает при резолве, либо собирает билд с несовместимым байткодом.

**Вторичные причины:**
- `ktor` тянет за собой `kotlinx-coroutines-jdk8`, `kotlinx-coroutines-slf4j`, `slf4j-api` — все чувствительны к версиям
- `okhttp3` и `okio-jvm` написаны на Kotlin; конфликт версий с другими плагинами, которые бандлят okhttp
- `retrofit2` добавляет ещё одну зависимость на okhttp
- `androidbrowserhelper` (Trusted Web Activity) — отдельная библиотека, которая большинству Unity-проектов не нужна

**Конкретные конфликтные сценарии:**
- Xsolla login (6.0.18) требует Kotlin 1.8.x; другой плагин требует Kotlin 1.7.x → конфликт резолвера
- Xsolla payments использует okhttp 4.x; другой плагин бандлит okhttp 3.x → `ClassNotFoundException` в runtime
- ktor + kotlinx-coroutines конфликтует с версией coroutines из Firebase

**Последствия:** партнёры регулярно получают ошибки сборки при совместном использовании Xsolla Unity SDK с другими плагинами. Ситуация ухудшается по мере того как всё больше SDK-вендоров выпускают Kotlin-based Android-зависимости.

---

## 3. Практики конкурентов

Как крупные SDK-вендоры решают задачу границы между Unity и Android:

### Facebook Unity SDK
- **Архитектура:** Unity C# → `FacebookAndroidWrapper.java` (~200 строк) → Facebook Android SDK (AAR)
- **Ключевое решение:** HTTP-запросы к Graph API делаются в C# через `UnityWebRequest`. Java возвращает только access token — Unity решает, что с ним делать дальше.

### Firebase Unity SDK
- **Архитектура:** Unity C# → C++ сгенерированные биндинги (P/Invoke) → Java stub (~100 строк на модуль) → Firebase Android SDK
- **Ключевое решение:** Java stub — это чисто мост инициализации, никакой логики в нём нет. Firebase Android SDK занимается HTTP внутри себя, но это его внутреннее дело.

### Google Sign-In Unity Plugin
- **Архитектура:** Unity C# → `GoogleSignInHelper.java` (~150 строк) → Google Sign-In Android SDK
- **Ключевое решение:** Java запускает нативный Sign-In Activity и возвращает `id_token` / `access_token`. Что делать с токеном дальше (обменять, сохранить, отправить на сервер) — только C#.

### Общий вывод

Все три SDK следуют одному правилу:

```
Android Java:  системный UI + нативные API + возврат токенов/результатов как примитивов
                        ↓ String, bool, int
Unity C#:      HTTP-запросы + бизнес-логика + состояние + обработка ответов
```

Предлагаемая архитектура Xsolla мини-SDK **точно совпадает** с отраслевым стандартом.

---

## 4. Предложение по архитектуре мини-SDK

### Рекомендуемый подход: Вариант 1 — те же контракты, переписать на Java

Создать новый Android library проект (`xsolla-engine-sdk`), который сохраняет те же имена классов, сигнатуры методов и интерфейсы, которые сейчас вызываются из Unity. **Нулевые изменения в Unity C# коде.**

### Почему не Вариант 2 (тонкая обёртка, HTTP переходит в Unity)?

Вариант 2 (Android = только браузер, HTTP переходит в Unity C#) архитектурно чище и даёт ~800 строк вместо ~2 500. Но:
- Требует изменений в 5 Unity C# файлах и 3 Java proxy файлах
- При добавлении Unreal ту же HTTP-логику придётся дублировать на C++
- Риск расхождения поведения между Unity и Unreal реализациями

Вариант 1 создаёт переиспользуемый AAR: Unreal просто подключает ту же библиотеку без какой-либо дополнительной работы.

### Спецификация библиотеки

**Maven-координаты:** `com.xsolla.android:engine:1.0.0`

**Язык:** чистый Java (без Kotlin)

**Зависимости:**
- `androidx.browser:1.8.0` — обязательная (Custom Tabs)
- Facebook SDK, Google Play Services Auth, WeChat SDK, QQ SDK — опциональные (`compileOnly`), загружаются через reflection в runtime

**Размер:** ~50–80 КБ AAR против текущих ~1,5 МБ Kotlin runtime + ~300 КБ AAR

**Структура (~18 Java файлов, ~2 500 строк):**

```
com.xsolla.android.login/
  XLogin.java              — фасад: 7 методов (замена 1 796 строк XLogin.kt)
  LoginConfig.java         — OauthBuilder + SocialConfig (те же сигнатуры)
  social/
    LoginSocial.java       — social OAuth flow, HttpURLConnection вместо OkHttp3
    SocialNetwork.java     — enum (31 провайдер)
  callback/
    AuthCallback.java      — interface (цель AndroidJavaProxy)
    StartSocialCallback.java
    FinishSocialCallback.java
    FinishXsollaWidgetAuthCallback.java
  token/
    TokenUtils.java        — SharedPreferences обёртка
  util/
    AnalyticsUtils.java    — 2 статических поля

com.xsolla.android.payments/
  XPayments.java           — IntentBuilder + AccessToken + Result + Status (те же сигнатуры)
  ui/
    ActivityPayStation.java — WebView + CustomTabs + TWA
  callback/
    BrowserCallback.java   — interface (цель AndroidJavaProxy)
  util/
    AnalyticsUtils.java    — 2 статических поля
```

### Что меняется в Unity SDK

Только два файла:

1. `Assets/Xsolla/Core/Editor/XsollaDependencies.xml` — заменить 6 зависимостей на 1
2. Имена пакетов остаются прежними (та же структура `com.xsolla.android.*`) — изменений в C# нет

```xml
<!-- Удалить: -->
<androidPackage spec="com.xsolla.android:payments:1.4.20"/>
<androidPackage spec="com.xsolla.android:login:6.0.18"/>
<androidPackage spec="com.xsolla.android:login-facebook:6.0.18"/>
<androidPackage spec="com.xsolla.android:login-google:6.0.18"/>
<androidPackage spec="com.xsolla.android:login-wechat:6.0.18"/>
<androidPackage spec="com.xsolla.android:login-qq:6.0.18"/>

<!-- Добавить: -->
<androidPackage spec="com.xsolla.android:engine:1.0.0"/>
```

### Оценка объёма работ

18 задач в 8 milestone'ах. Полная разбивка — в `android-mini-sdk-jira-tasks-no-estimates.md`.

| Milestone | Задачи | Содержание |
|---|---|---|
| M1: Настройка проекта | TASK-1, TASK-2 | Создать Android-проект, maven-publish, первый smoke test в Unity |
| M2: Login — общие типы | TASK-3–6 | SocialNetwork enum, callback-интерфейсы, TokenUtils, AnalyticsUtils |
| M3: LoginConfig | TASK-7 | LoginConfig + OauthBuilder + SocialConfig |
| M4: Login фасад | TASK-8–9 | LoginSocial (HTTP-flow), XLogin (7 методов) |
| M5: Payments типы | TASK-10–11 | BrowserCallback, XPayments фасад + типы |
| M6: PayStation UI | TASK-12–15 | ActivityPayStation: WebView, CustomTabs, TWA, status polling |
| M7: Сборка + интеграция | TASK-16–17 | Публикация AAR, обновление Unity SDK |
| M8: Тестирование | TASK-18 | Полное тестирование на устройстве |

**Общая оценка: 3–4 недели** (укладывается в квартальную оценку, с буфером на QA и edge-кейсы соцсетей).

### Стратегия тестирования в процессе разработки

Локальный Maven (`~/.m2/`) позволяет тестировать интеграцию в Unity с первого дня:

```
./gradlew publishToMavenLocal
→ AAR + POM в ~/.m2/repository/com/xsolla/android/engine/VERSION/
→ EDM резолвит из локального репозитория, публикация в Maven Central не нужна
→ Проверка на устройстве после каждого milestone
```

Первая проверка интеграции — после TASK-2 (stub-класс, ~2-й день разработки).

---

## 5. Риски и альтернативы

### Риски подхода с мини-SDK

| Риск | Серьёзность | Митигация |
|---|---|---|
| **Ручная синхронизация с основным Android SDK.** При добавлении фич в основной SDK — мини-SDK обновляется вручную. | Средняя | Мини-SDK покрывает ~10 стабильных методов, которые редко меняются. На практике — низкий churn. |
| **Сложность LoginSocial.** Social auth через 4 провайдера с reflection — самая сложная часть. | Средняя | Хорошо понятый код — прямой порт существующего Kotlin. Тестировать каждый провайдер end-to-end. |
| **JS-инъекции в ActivityPayStation.** Хуки для мониторинга XHR должны быть перенесены точно. | Средняя | Изолировано в одном файле. Существующий Kotlin — эталон. |
| **Deep link / redirect handling.** Тонкие различия могут сломать парсинг результата платежа. | Низкая–Средняя | Покрывается тест-матрицей TASK-18 (11 сценариев). |
| **TWA (Trusted Web Activity).** Требует `androidbrowserhelper` — библиотеку, от которой хотелось бы избавиться. | Низкая | Оставить как опцию; проекты, не использующие TWA, не пострадают. |

### Рассмотренные альтернативы

**Альтернатива А: Fat AAR (все транзитивные зависимости внутри)**
Собрать okhttp, kotlin-stdlib и т.д. в один AAR через Gradle Shadow plugin с релокацией классов. Устраняет конфликты версий через переименование пакетов. Отклонено: размер AAR вырастает до 3–5 МБ, релокация ломает runtime class lookup, не решает корневую проблему.

**Альтернатива Б: Кастомный dependency resolver / EDM-конфигурация**
Жёстко зафиксировать версии конфликтующих зависимостей в `mainTemplate.gradle`. Отклонено: хрупко, ломается при обновлении любого другого плагина, требует сопровождения под каждый партнёрский проект.

**Альтернатива В: Модуляризация основного Android SDK (Вариант 4)**
Реструктурировать `store-android-sdk` так, чтобы Unity подключал только нужные модули. Отклонено: требует согласования с Android-командой, Kotlin никуда не уходит, объём работы сопоставим, но Kotlin runtime в результате всё равно остаётся.

**Альтернатива Г: Java-файлы прямо в Unity (Вариант 3)**
Отдельная библиотека не нужна, расширить существующие `PaymentsProxyActivity.java` / `SocialAuthProxyActivity.java` напрямую. Рабочий вариант для случая Unity-only. Отклонено: при добавлении Unreal те же Java-файлы придётся писать заново. Нет единой точки изменений — AAR строго лучше.

---

## 6. Рекомендация: делать или нет

**Делать.** Вариант 1 (мини-SDK, те же API-контракты).

**Обоснование:**
- Корневая причина ошибок сборки у партнёров ясна и устранима: транзитивные Kotlin-зависимости
- Решение (Java-переписывание, 1 внешняя зависимость) технически прямолинейно — в основном механический перевод Kotlin → Java
- Нулевые изменения в Unity C# коде — нет риска регрессий в существующем поведении Unity SDK
- Переиспользуется в Unity, Unreal и любых будущих движках
- Объём работ ограничен и хорошо декомпозирован (~2 500 строк, 18 задач с чёткими критериями приёмки)
- Отраслевой прецедент подтверждает правильность подхода (Facebook, Google Sign-In, Firebase — все используют thin Java wrapper)

**Условие:** LoginSocial (TASK-8) и ActivityPayStation (TASK-12) — задачи с наибольшим риском, их нужно ревьюить раньше остальных. Рекомендуется промежуточная проверка после M4 (login-модуль готов), прежде чем переходить к платежам.

---

## Дополнительные документы

| Документ | Содержание |
|---|---|
| `android-native-api-calls.md` | Полный аудит всех 31 точки вызова с указанием файлов |
| `android-native-integration.md` | Карта файлов Unity SDK: Java activities, C# обёртки, Editor-скрипты |
| `android-mini-sdk-analysis.md` | Дерево зависимостей, что убирается, структура новой библиотеки |
| `android-sdk-boundary-analysis.md` | Анализ конкурентов: Facebook, Firebase, Google Sign-In |
| `android-mini-sdk-options.md` | Все 5 вариантов с плюсами и минусами |
| `android-variants-comparison.md` | Вариант 1 vs Вариант 2: пофайловый diff, точные LOC |
| `android-mini-sdk-jira-tasks-no-estimates.md` | 18 Jira-задач с критериями приёмки, workflow локального Maven |
