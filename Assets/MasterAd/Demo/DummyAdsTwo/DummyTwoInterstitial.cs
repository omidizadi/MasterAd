using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyTwoInterstitial : IAdService
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

        DummyTwo.OnAdFailed = this.onFailed;
        DummyTwo.OnAdLoaded = this.onLoaded;
        
        DummyTwo.Load();
    }

    public bool Ready(string name)
    {
        return DummyTwo.Ready;
    }

    public void Show(string name, Action onFinished)
    {
        this.onFinished = onFinished; 
        DummyTwo.OnAdFinished = this.onFinished;

        
        DummyTwo.Show();
    }

    public void Destroy(string name)
    {
       DummyTwo.Destroy();
    }
}