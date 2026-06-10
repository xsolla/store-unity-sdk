# Граница Unity/Android в известных SDK

Анализ того, как крупные cross-platform SDK определяют границу между Unity (C#) и Android (Java/Kotlin) частями.

---

## Facebook Unity SDK

**Репозиторий**: https://github.com/facebook/facebook-sdk-for-unity

### Архитектура

```
Unity C# (Facebook.Unity)
    ↓ AndroidJavaObject
Java Bridge (FacebookAndroidWrapper.java) — ~200 LOC
    ↓
Facebook Android SDK (AAR)
```

### Граница

| Слой | Ответственность |
|------|----------------|
| Unity C# | Бизнес-логика, состояние сессии, HTTP запросы к Graph API (через UnityWebRequest), обработка ответов |
| Android Java | Activity lifecycle, нативные диалоги (Login Dialog, Share Dialog), возврат результата в C# |

**Ключевое решение**: HTTP запросы к Graph API делаются **в C#** через `UnityWebRequest`, не в Java.
Java bridge возвращает только access token — дальше Unity решает что с ним делать.

---

## Firebase Unity SDK

**Репозиторий**: https://github.com/firebase/firebase-unity-sdk

### Архитектура

```
Unity C# (Firebase.Auth, Firebase.Analytics, etc.)
    ↓ P/Invoke
C++ wrapper (generated bindings)
    ↓ JNI
Java stub (~100 LOC на модуль)
    ↓
Firebase Android SDK (AAR)
```

### Граница

| Слой | Ответственность |
|------|----------------|
| Unity C# | Callbacks, Task/Promise API, конфигурация, обработка результатов |
| C++ | Тонкий слой сгенерированных биндингов (не пишется вручную) |
| Android Java | Инициализация нативного SDK, передача конфига, возврат токенов/событий |

**Ключевое решение**: Firebase Android SDK внутри себя делает HTTP, но это его внутреннее дело.
Java stub — это только мост инициализации, никакой логики в нём нет.

---

## Google Sign-In Unity Plugin

**Репозиторий**: https://github.com/googlesamples/google-signin-unity

### Архитектура

```
Unity C#
    ↓ AndroidJavaObject
GoogleSignInHelper.java (~150 LOC)
    ↓
Google Sign-In Android SDK
```

### Граница

Самый близкий по структуре к Xsolla Unity SDK.

| Слой | Ответственность |
|------|----------------|
| Unity C# | Конфигурация (client ID и др.), обработка токена, HTTP запросы с токеном |
| Android Java | Открыть Sign-In Activity → получить id_token/access_token → вернуть в C# |

**Ключевое решение**: Java делает одно — запускает нативный flow и возвращает токен.
Что делать с токеном дальше (обменять на свой, сохранить, отправить на сервер) — только в C#.

---

## Общий паттерн (Industry Standard)

Все три SDK следуют одному правилу:

```
Android Java:   системный UI + нативные API + получение токенов/результатов
                        ↓ передача примитивов (String, bool, int)
Unity C#:       HTTP запросы + бизнес-логика + состояние + обработка ответов
```

### Что никогда не делается в Java-части

- HTTP запросы к собственному backend
- Парсинг JSON ответов от своего API
- Хранение состояния сессии
- Retry логика, error handling бизнес-уровня

### Что всегда остаётся в Java-части

- Запуск Activity / Fragment
- Работа с браузером (CustomTabs, WebView)
- Вызов нативных SDK (Google, Facebook, WeChat)
- Обработка Intent / deep link редиректов
- Возврат системного результата в Unity через callback

---

## Вывод для Xsolla мини-SDK

Предлагаемая архитектура (HTTP → Unity C#, Android = браузер/UI layer) **точно совпадает** с industry practice.

**Итоговая граница для Xsolla mini Android SDK:**

| Android (мини-SDK) | Unity C# |
|-------------------|----------|
| Открыть CustomTabs / WebView / System Browser | HTTP запросы к Xsolla API |
| Обработать deep link redirect | Парсинг JSON ответов |
| Запустить нативный social auth flow (Google, Facebook) | Обмен auth code на токен |
| Вернуть auth code / result в Unity | Хранение токенов, refresh логика |

**Размер Java-части**: ~200-400 LOC, 0 внешних зависимостей (только `androidx.browser` для CustomTabs).

Это стандартная "thin wrapper" архитектура — именно так устроены Facebook, Firebase и Google Sign-In Unity плагины.
