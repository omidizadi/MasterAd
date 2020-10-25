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