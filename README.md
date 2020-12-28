
# Managing multiple ad services in Unity

Add unlimited ad platforms with minimum coding

Mobile advertisement, as a monetization solution is a hot trend these days. Many game studios have fully switched to ads and are making a considerable amount of money.

There are actually so many mobile ad services that you can use but it is important to use as many of them as possible. For example, according to [Appbrain](http://appbrain.com), Voodoo’s game “Helix Jump” with more than 500 million downloads is using 18 different ad services! Including Admob, UnityAds, Vungle, IronSource, AppLovin, Facebook Ads, Chartboost, and …

![Helix Jump from Voodoo](https://cdn-images-1.medium.com/max/2000/1*vsxszNFp3Q6XbwcAcBwmog.png)*Helix Jump from Voodoo*

So I decided to create a simple solution to use all these different services together.

## MasterAd.cs

Our goal is to reach a solution so that we don’t have to change our main implementation every time we add a new ad service.

First I identified similar ad formats and similar behaviors between different ad services and created an Interface that contains all those things:
```c#
using System;
using System.Collections.Generic;

public interface IAdService
{
    /// <summary>
    /// It stores all ad tokens with their corresponding names
    /// </summary>
    Dictionary<string, string> tokens { get; set; }

    /// <summary>
    /// You should fill this method when requesting to load, and call it when ad loaded
    /// </summary>
    Action onLoaded { get; set; }

    /// <summary>
    /// You should fill this method when requesting to load, and call it when ad failed to load
    /// </summary>
    Action onFailed { get; set; }

    /// <summary>
    /// You should fill this method when requesting to Show, and call it when ad finished showing
    /// </summary>
    Action onFinished { get; set; }

    /// <summary>
    /// You should add tokens inside this method using AddToken 
    /// </summary>
    void Init();

    /// <summary>
    /// You should add new Tokens to tokens dictionary here with additional implementation of desired ad platform
    /// </summary>
    void AddToken(string name, string token);

    /// <summary>
    /// You should put your ad platform specific load implementation here
    /// </summary>
    void Load(string name, Action onLoaded, Action onFailed);

    /// <summary>
    /// You should put your ad platform specific Readiness implementation here
    /// </summary>
    bool Ready(string name);

    /// <summary>
    /// You should put your ad platform specific Show implementation here
    /// </summary>
    void Show(string name, Action onFinished);

    /// <summary>
    /// You should put your ad platform specific Destroy implementation here
    /// </summary>
    void Destroy(string name);
}
```

This interface helps us identify all ad platforms as a single type so that we can easily switch between them. You should remember that each one of these *IAdServices *belongs to one platform and one ad format. For example, *AdmobBannerAd *will be one class implementing *IAdService* and *AdmobRewardedAd *will be another class implementing *IAdService.*

The next step is to create a class that handles all ad service objects. I created *MasterAd *to do so:

```c#

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
```

*MasterAd *does 3 main things:

* Loading, showing, and destroying ads

* Adding and switching between ad services using a service manager

In order to stick to S.O.L.I.D, I separated the part that manages services in an interface called *IServiceManager:*

```c#
public interface IServiceManager
{
    /// <summary>
    /// Adds an Ad Service for further use
    /// </summary>
    /// <param name="adService"></param>
    void AddService(IAdService adService);

    /// <summary>
    /// Whenever an Ad Service fails to load ad, this method switches the service
    /// </summary>
    void SwitchService();

    /// <summary>
    /// It returns the current using service
    /// </summary>
    /// <returns></returns>
    IAdService GetService();
}
```

With this, you can create your own implementation of adding, data structure type to store services, and the way they will be switched.

I created my own implementation called *ServiceSwitcher*:

```c#

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
```

Here I’m using a simple List to store services and a pointer to know which service is currently in use, and switch to the next service when the current one fails. Remember that each time you add a service, you should call the *Init()* function to prepare the ad service.

So now that we have our main classes, it’s time to show you an example. Here I implemented Admob Banner Ad and UnityAds Banner Ad to demonstrate how *IAdService *should be used:

```c#

using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdmobBanner : IAdService
{
    public Dictionary<string, string> tokens { get; set; }
    public Action onLoaded { get; set; }
    public Action onFailed { get; set; }
    public Action onFinished { get; set; }

    private Dictionary<string, BannerView> bannerView;

    public void Init()
    {
        tokens = new Dictionary<string, string>();
        bannerView = new Dictionary<string, BannerView>();
        AddToken("Top-Banner-Ad", "ca-app-pub-3940256099942544/6300978111");
        AddToken("Bottom-Banner-Ad", "ca-app-pub-3940256099942544/6300978111");
    }

    public void AddToken(string name, string token)
    {
        tokens.Add(name, token);
        bannerView.Add(name, null);
    }

    public void Load(string name, Action onLoaded, Action onFailed)
    {
        Debug.Log("Banner is loading...");
        this.onLoaded = onLoaded;
        this.onFailed = onFailed;
        var adUnitId = tokens[name];
        bannerView[name] = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
        var request = new AdRequest.Builder().Build();
        bannerView[name].LoadAd(request);
        bannerView[name].OnAdLoaded += HandleOnAdLoaded;
        bannerView[name].OnAdFailedToLoad += HandleOnAdFailedToLoad;
        bannerView[name].OnAdClosed += HandleOnAdClosed;
    }

    public bool Ready(string name)
    {
        return bannerView != null;
    }

    public void Show(string name, Action onFinished)
    {
        this.onFinished = onFinished;
        bannerView[name].Show();
    }

    public void Destroy(string name)
    {
        bannerView[name].Destroy();
    }

    private void HandleOnAdLoaded(object sender, EventArgs args)
    {
        onLoaded?.Invoke();
    }

    private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        onFailed?.Invoke();
    }

    private void HandleOnAdClosed(object sender, EventArgs args)
    {
        onFinished?.Invoke();
    }
}
```


You can notice that I have added an additional dictionary for banner view objects as we may have multiple banner ads with different ad unit ids.

```c#

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsBanner : IAdService, IUnityAdsListener
{
    public Dictionary<string, string> tokens { get; set; }
    public Action onLoaded { get; set; }
    public Action onFailed { get; set; }
    public Action onFinished { get; set; }

    public void Init()
    {
        tokens = new Dictionary<string, string>();
        AddToken("Top-Banner-Ad", "1234567");
        AddToken("Bottom-Banner-Ad", "1234567");
    }

    public void AddToken(string name, string token)
    {
        tokens.Add(name, token);
    }

    public void Load(string name, Action onLoaded, Action onFailed)
    {
        Debug.Log("Banner is loading...");

        this.onLoaded = onLoaded;
        this.onFailed = onFailed;

        Advertisement.Banner.Load(tokens[name]);
    }

    public bool Ready(string name)
    {
        return Advertisement.IsReady(tokens[name]);
    }

    public void Show(string name, Action onFinished)
    {
        this.onFinished = onFinished;

        Advertisement.Banner.Show(tokens[name]);
    }

    public void Destroy(string name)
    {
        Advertisement.Banner.Hide(true);
    }

    public void OnUnityAdsReady(string placementId)
    {
        onLoaded?.Invoke();
    }

    public void OnUnityAdsDidError(string message)
    {
        onFailed?.Invoke();
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        throw new NotImplementedException();
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        onFinished?.Invoke();
    }
}
```

You can see that I have added the exact same ad token names (with different token ids actually) to this class so that we only need to pass one single ad name to both ad services.

Now that we have at least two ad services we can use them in action. I created a small demo class only to show how *MasterAd *and different *IAdServices *work together:

```c#

using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    [SerializeField] private Text text;
    private MasterAd banner;

    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        Advertisement.Initialize("123456", true);

        banner = new MasterAd(new ServiceSwitcher());
        banner.AddService(new UnityAdsBanner());
        banner.AddService(new AdmobBanner());

    }

    public void LoadBanner()
    {
        banner.LoadAd("Top-Banner-Ad", () => { text.text = "Banner Loaded"; });
    }

    public void ShowBanner()
    {
        banner.ShowAd("Top-Banner-Ad", () => { text.text = "Banner Finished"; });
    }

}
```

After this, whenever I call *LoadBanner *the *MasterAd *class tries to load the ad from the current service and if it fails, it automatically switches to another service and does the same thing until one service answers. And when I call *ShowBanner *it will show the ad after checking if the ad was loaded. This functionality works for most ad services and formats.

Make sure to check it out and feel free to add any comments.

Thank you for your time.
