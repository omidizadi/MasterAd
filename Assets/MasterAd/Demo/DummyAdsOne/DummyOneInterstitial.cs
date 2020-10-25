using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyOneInterstitial : IAdService
{
    public Dictionary<string, string> tokens { get; set; }
    public Action onLoaded { get; set; }
    public Action onFailed { get; set; }
    public Action onFinished { get; set; }

    public void Init()
    {
        tokens = new Dictionary<string, string>();
        AddToken("Full-Screen", "1234567");
    }

    public void AddToken(string name, string token)
    {
        tokens.Add(name, token);
    }

    public void Load(string name, Action onLoaded, Action onFailed)
    {
        this.onLoaded = onLoaded;
        this.onFailed = onFailed;

        DummyOne.OnAdFailed = this.onFailed;
        DummyOne.OnAdLoaded = this.onLoaded;

        DummyOne.Load();
    }

    public bool Ready(string name)
    {
        return DummyOne.Ready;
    }

    public void Show(string name, Action onFinished)
    {
        this.onFinished = onFinished;
        DummyOne.OnAdFinished = this.onFinished;
        DummyOne.Show();
    }

    public void Destroy(string name)
    {
        DummyOne.Destroy();
    }
}