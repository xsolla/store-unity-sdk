using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public class ServiceLocator : MonoBehaviour
	{
		private readonly Dictionary<Type, object> Services = new();

		public static ServiceLocator Instance { get; private set; }

		private void Awake()
		{
			var instances = FindObjectsByType<ServiceLocator>(FindObjectsSortMode.None);
			switch (instances.Length)
			{
				case 0:
					throw new Exception($"{nameof(ServiceLocator)} instance not found in the scene. Please add one");
				case > 1:
					throw new Exception($"Multiple {nameof(ServiceLocator)} instances detected. There should be only one instance in the scene");
				default:
					Instance = this;
					DontDestroyOnLoad(gameObject);
					break;
			}
		}

		public static TService Resolve<TService>()
		{
			var type = typeof(TService);

			if (Instance.Services.TryGetValue(type, out var service))
				return (TService) service;

			throw new Exception($"Service of type {type.FullName} is not registered");
		}

		public void Register<TService>(TService instance)
		{
			var type = typeof(TService);

			if (!Services.TryAdd(type, instance))
				throw new Exception($"Service of type {type.FullName} is already registered");
		}

		public void Register<TInterface, TImplementation>(TImplementation instance) where TImplementation : TInterface
		{
			var type = typeof(TInterface);

			if (!Services.TryAdd(type, instance))
				throw new Exception($"Service of type {type.FullName} is already registered");
		}
	}
}