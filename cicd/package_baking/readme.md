# 📦 Xsolla Unity SDK Package Baking

This tool builds a modified `.unitypackage` file for a specific SDK version with injected project settings.

---

## 🚀 How to Launch

1. Open a terminal
2. Navigate to the `cicd/package_baking` folder
3. Run the following commands:

```bash
python3 baking.py 2.5.5 <STORE_ID> <LOGIN_ID> <OAUTH_CLIENT_ID>
```

---

## 🧩 Arguments

- `<2.5.5>` — SDK version (currently, only `2.5.5` is supported)
- `<STORE_ID>` — your Xsolla store project ID
- `<LOGIN_ID>` — your login project ID (UUID format)
- `<OAUTH_CLIENT_ID>` — your OAuth client ID (numeric)

---

## 📄 Output

After running the script, you will get the following file:

```
cicd/package_baking/xsolla-unity-sdk-2.5.5.unitypackage
```

This file includes the updated configuration with the values you provided.

---

## 📁 Note

Only version `2.5.5` is currently supported because it's the only one available in the `source_packages` folder.
