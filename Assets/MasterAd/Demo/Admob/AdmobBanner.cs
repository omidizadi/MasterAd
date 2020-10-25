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