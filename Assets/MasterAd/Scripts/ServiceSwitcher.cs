using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceSwitcher : IServiceManager
{
    /// <summary>
    /// list of all ad services
    /// </summary>
    private List<IAdService> adServices;

    /// <summary>
    /// the current using service in above list
    /// </summary>
    private int servicePointer;

    public ServiceSwitcher()
    {
        adServices = new List<IAdService>();
    }

    public void AddService(IAdService adService)
    {
        adService.Init();

        adServices.Add(adService);
    }

  
    public void SwitchService()
    {
        servicePointer = (servicePointer + 1) % adServices.Count;
    }

    public IAdService GetService()
    {
        return adServices[servicePointer];
    }
}