using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class DummyTwo
{
    public static bool Ready;
    public static Action OnAdLoaded;
    public static Action OnAdFailed;
    public static Action OnAdFinished;

    public static void Load()
    {
        var rand = Random.value;
        if (rand < 0.5f)
        {
            Ready = true;
            Debug.Log("Dummy Two Ad Loaded");
            OnAdLoaded?.Invoke();
        }
        else
        {
            Ready = false;
            Debug.Log("Dummy Two Ad Failed");

            OnAdFailed?.Invoke();
        }
    }

    public static void Show()
    {
        Debug.Log("Dummy Two Ad Showed and Finished");

        OnAdFinished?.Invoke();
    }
    
    public static void Destroy()
    {
        Ready = false;
    }
}