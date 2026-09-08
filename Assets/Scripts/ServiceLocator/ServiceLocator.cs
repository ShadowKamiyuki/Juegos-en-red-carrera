using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new();

    public static void Register<T>(T service)
    {
        var type = typeof(T);

        if (service == null)
        {
            Debug.LogError($"[ServiceLocator] Intento de registrar servicio null: {type.Name}");
            return;
        }

        if (_services.ContainsKey(type))
        {
            Debug.LogWarning($"[ServiceLocator] Servicio ya registrado: {type.Name}");
            return;
        }

        _services[type] = service;
    }

    public static void Unregister<T>()
    {
        var type = typeof(T);
        if (_services.Remove(type))
            Debug.Log($"[ServiceLocator] Eliminado: {type.Name}");
    }

    public static T Get<T>()
    {
        if (_services.TryGetValue(typeof(T), out var service))
            return (T)service;

        Debug.LogWarning($"[ServiceLocator] Servicio no encontrado: {typeof(T).Name}");
        return default;
    }

    public static bool Exists<T>() => _services.ContainsKey(typeof(T));

    public static void Clear()
    {
        _services.Clear();
        Debug.Log("[ServiceLocator] Servicios limpiados");
    }
}
