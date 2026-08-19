using System.Collections.Generic;
using UnityEngine;

public class CustomUpdateManager : MonoBehaviour, IUpdateService
{
    private List<IUpdatable> updatables = new List<IUpdatable>();

    private void OnDestroy()
    {
        if (ServiceLocator.Exists<IUpdateService>())
            ServiceLocator.Unregister<IUpdateService>();
    }

    void Update()
    {
        foreach (var u in updatables)
        {
            u.Tick(Time.deltaTime);
        }
    }

    public void Register(IUpdatable updatable)
    {
        if (!updatables.Contains(updatable))
            updatables.Add(updatable);
    }

    public void Unregister(IUpdatable updatable)
    {
        updatables.Remove(updatable);
    }
}