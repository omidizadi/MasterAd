using System;
using System.Collections.Generic;
using UnityEngine;

public class MasterAd
{
    ///list of all ad services
    private List<IAdService> adServices;

    ///the current using service in above list
    private int servicePointer;

    public MasterAd()
    {
        adServices = new List<IAdService>();
    }

    /// <summary>
    /// Adds an Ad Service for further use
    /// </summary>
    /// <param name="adService"></param>
    public void AddService(IAdService adService)
    {
        adService.Init();

        adServices.Add(adService);
    }

    /// <summary>
    /// Whenever an Ad Service fails to load ad, this method switches the service
    /// </summary>
    private void SwitchService()
    {
        servicePointer = (servicePointer + 1) % adServices.Count;
    }

    /// <summary>
    /// It returns the current using service
    /// </summary>
    /// <returns></returns>
    private IAdService GetService()
    {
        return adServices[servicePointer];
    }

    /// <summary>
    /// Loads an ad with current service and the given name
    /// </summary>
    /// <param name="name"></param>
    /// <param name="onLoaded"></param>
    public void LoadAd(string name, Action onLoaded)
    {
        GetService().Load(name, () => { onLoaded?.Invoke(); }, () =>
        {
            SwitchService();
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
        if (GetService().Ready(name))
            GetService().Show(name, () => { onFinished?.Invoke(); });
    }

    /// <summary>
    /// Destroys the current services add with the given name
    /// </summary>
    /// <param name="name"></param>
    public void DestroyAd(string name)
    {
        GetService().Destroy(name);
    }
}