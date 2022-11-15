using UnityEngine;

namespace Xsolla.Core
{
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
		private const string PATH_TO_PREFABS = "";
		private static T _instance;
		private bool _isInitialized = false;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = InstantiateThis();
					_instance?.OneTimeInit();
				}

				return _instance;
			}
		}

		public static bool IsExist => _instance != null;

		private void Awake()
		{
			if (_instance == null)
			{
				DontDestroyOnLoad(gameObject);
				_instance = this as T;
				_instance.OneTimeInit();
			}
			else if (_instance != this)
			{
				Destroy(gameObject);
			}
		}

		private static T InstantiateThis()
		{
			string componentName = typeof(T).Name;
			Debug.Log($"No instance of {componentName}");

			//Try finding pre-existing object on the scene
			T result = FindObjectOfType(typeof(T)) as T;
			if (result != null)
			{
				Debug.Log($"{componentName} found in the scene.");
				return result;
			}

			//Try loading from resources
			var prefab = Resources.Load($"{PATH_TO_PREFABS}{typeof(T).Name}");
			if (prefab != null)
			{
				var instance = Instantiate(prefab) as GameObject;
				if (instance != null)
					instance.name = typeof(T).Name;
				result = instance?.GetComponent<T>();
			}
			if (result != null)
			{
				Debug.Log($"{componentName} loaded from resources.");
				return result;
			}

			//Create new object
			var newInstance = new GameObject($"Temp Instance of {typeof(T)}");
			result = newInstance.AddComponent<T>();
			if (result != null)
			{
				Debug.Log($"{componentName} a temporary one is created.");
				return result;
			}

			//else
			Debug.LogError($"Could not instantiate {componentName}");
			return result;
		}

		public virtual void Init() {}

		protected virtual void OnDestroy()
		{
			Debug.Log($"{typeof(T)} is destroyed.");
			_instance = null;
		}

		private void OneTimeInit()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;
				Init();
			}
		}

		private void OnApplicationQuit()
		{
			Destroy(gameObject);
		}
	}
}
