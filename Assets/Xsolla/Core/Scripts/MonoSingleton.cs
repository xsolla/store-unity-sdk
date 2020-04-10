using UnityEngine;

namespace Xsolla.Core
{
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
		const string PATH_TO_PREFABS = "Prefabs/";
		static T _instance;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = InstantiateThis();
					_instance?.Init();
				}

				return _instance;
			}
		}

		private static T InstantiateThis()
		{
			T result = InstantiateFromScene();

			if (result == null)
				result = InstantiateFromResources();

			if (result == null)
				result = InstantiateAsNewObject();

			return result;
		}

		private static T InstantiateFromScene()
		{
			return FindObjectOfType(typeof(T)) as T;
		}

		private static T InstantiateFromResources()
		{
			return Resources.Load<T>(PATH_TO_PREFABS + typeof(T).Name);
		}

		private static T InstantiateAsNewObject()
		{
			Debug.Log("No instance of " + typeof(T) + ", a temporary one is created.");
			GameObject instance = new GameObject("Temp Instance of " + typeof(T));
			return instance.AddComponent<T>();
		}

		public static bool IsExist {
			get {
				return _instance != null;
			}
		}

		// If no other MonoBehaviour request the instance in an awake function
		// executing before this one, no need to search the object.
		void Awake()
		{
			if (_instance == null)
			{
				DontDestroyOnLoad(gameObject);
				_instance = this as T;
				_instance.Init();
			}
			else if (_instance != this)
			{
				Destroy(gameObject);
			}
		}

		// This function is called when the instance is used the first time
		// Put all the initializations you need here, as you would do in Awake
		public virtual void Init()
		{
		}
	}
}