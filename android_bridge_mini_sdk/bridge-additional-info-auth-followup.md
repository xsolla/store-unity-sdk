# Follow-up: "Additional Info" (ask-fields) auth flow — candidate for bridge

## Status
Deferred. Not part of the UE4 → bridge migration. Tracked here so we can revisit later.

## Where it lives today
`store-ue4-sdk/Source/XsollaLogin/Private/Android/Java/Common/XsollaNativeAdditionalInfoAuthActivity.java`

Registered in `XsollaLogin_UPL.xml` as `com.xsolla.login.XsollaNativeAdditionalInfoAuthActivity`.
Invoked from `XsollaLoginSubsystem.cpp` (`HandleAskFieldsAuthentication` →
`CallStaticVoidMethod("com/xsolla/login/XsollaNativeAuth", "authAdditionalInfo", ...)`).

## What it does
Handles the "additional fields" (ask-fields) auth step — when Xsolla Login requires the user
to fill in missing data (e.g. confirm email, extra profile fields) via a widget/form URL before
a final token is issued. This is a distinct step from social auth and from the main Xsolla
Widget auth.

## Why it's architecturally different from bridge's existing flows
- Bridge's existing flows (`SocialAuthBridgeActivity`, `XsollaWidgetBridgeActivity`) always hand
  the URL off to an external browser/Chrome Custom Tabs and wait for a deep-link redirect back
  into the app (`app://xlogin.<package>`).
- This activity instead loads the URL **directly in an embedded `WebView`** and intercepts
  navigation itself (`WebViewClient.shouldOverrideUrlLoading`), watching for when the WebView
  navigates to `redirect_uri`, then parses `token`/`code`/`error`/`error_description` straight
  out of that URL — no deep link, no external browser involved.
- No dependency on the legacy Android SDK at all (no `com.xsolla.android.login.*` imports) — it's
  fully self-contained.
- `ensureRedirectUri()` rewrites the `redirect_uri` query param on the incoming login URL to the
  one the WebView actually watches for.
- Careful process-death handling: if `savedInstanceState != null` (activity recreated after the
  process was killed), it finishes immediately **without** firing a terminal callback, because the
  `long callback` passed in is a raw JNI pointer (`jlong`) to a native C++ object — after process
  death that pointer is dangling and must not be invoked.
- `onDestroy()` fires `onAuthCancelCallback` as a fallback if the flow never reached a terminal
  state (covers back-press/swipe-away), guarded by an `isCompleted` flag so it can't double-fire.

## Why this could reasonably move into bridge
- It's a generic Xsolla Login feature, not Unreal-specific — any engine integration using ask-fields
  auth would need the same thing.
- Bridge already has precedent for embedding a WebView directly (`ActivityPayStation` supports
  `XPayments.ActivityType.WEB_VIEW`), so this wouldn't introduce a new dependency.
- Would fit bridge's existing pattern: a `*BridgeActivity` with a static `perform(...)` entry point
  + a dedicated callback interface (mirroring `AuthCallback`/`BrowserCallback`).

## Why we didn't do it now
- Only confirmed consumer so far is UE4. Unclear whether Unity's login flow needs the same
  ask-fields step handled natively, or whether it's covered differently there (e.g. as part of the
  Xsolla Widget flow itself). Expanding bridge's public API for a single consumer isn't worth it
  until that's confirmed.

## Next step (when revisited)
1. Check whether Unity's SDK has an equivalent ask-fields auth path and how it currently handles it.
2. If there's a real second consumer (or a clear future need), design
   `AdditionalInfoAuthBridgeActivity` + `AdditionalInfoAuthCallback` in bridge, port the WebView
   redirect-interception + process-death guard logic above, then repoint UE4's
   `XsollaNativeAuth.authAdditionalInfo` (or its bridge-migrated successor) at it, and delete
   `XsollaNativeAdditionalInfoAuthActivity.java` from `store-ue4-sdk`.
