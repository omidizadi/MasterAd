using System;
using System.Collections.Generic;
using UnityEngine;

public class MasterAd
{
    /// <summary>
    /// Service manager to handle service switching in desired way
    /// </summary>
    private IServiceManager serviceManager;


    /// <summary>
    /// Injecting IServiceManager to MasterAd class via its constructor
    /// </summary>
    /// <param name="serviceManager"></param>
    public MasterAd(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }

    /// <summary>
    /// using service manager to add Ad service
    /// </summary>
    /// <param name="adService"></param>
    public void AddService(IAdService adService)
    {
        serviceManager.AddService(adService);
    }

    /// <summary>
    /// Loads an ad with current service and the given name
    /// </summary>
    /// <param name="name"></param>
    /// <param name="onLoaded"></param>
    public void LoadAd(string name, Action onLoaded)
    {
        serviceManager.GetService().Load(name, () => { onLoaded?.Invoke(); }, () =>
        {
            serviceManager.SwitchService();
            LoadAd(name, onLoaded);
        });
    }

    /// <summary>
    /// Shows an ad with current service and the given name
    /// </summary>
    /// <param name="name"></param>
    /// <param name="onFinished"></param>
    public void ShowAd(string name, Action onFinished)
    {
        if (serviceManager.GetService().Ready(name))
            serviceManager.GetService().Show(name, () => { onFinished?.Invoke(); });
    }

    /// <summary>
    /// Destroys the current services add with the given name
    /// </summary>
    /// <param name="name"></param>
    public void DestroyAd(string name)
    {
        serviceManager.GetService().Destroy(name);
    }
}