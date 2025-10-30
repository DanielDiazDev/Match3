using System;
using System.Collections.Generic;
using UnityEngine;


namespace Systems
{
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator _instance;
        public static ServiceLocator Instance
        {
            get
            {
                if(_instance == null)
                {
                    var go = new GameObject("[ServiceLocator]");
                    _instance = go.AddComponent<ServiceLocator>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        private readonly Dictionary<Type, object> _services = new();

        public void Register<T>(T service)
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"ServiceLocator: El servicio {type.Name} ya está registrado. Se reemplazará.");
            }
            _services[type] = service;
        }
        public T Get<T>()
        {
            var type = typeof(T);
            if(_services.TryGetValue(type, out var service))
            {
                return (T)service;
            }
            Debug.LogError($"ServiceLocator: No se encontró el servicio {type.Name}. Asegúrate de registrarlo antes de usarlo.");
            return default;
        }
        public bool Has<T>() => _services.ContainsKey(typeof(T));
        public void Unregister<T>()
        {
            _services.Remove(typeof(T));
        }
        public void Clear()
        {
            _services.Clear();
        }
    }
}
