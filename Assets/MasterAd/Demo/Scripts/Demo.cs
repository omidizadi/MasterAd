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
    private MasterAd interstitial;
    private MasterAd rewarded;

    // Start is called before the first frame update
    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        Advertisement.Initialize("123456", true);

        banner = new MasterAd();
        banner.AddService(new UnityAdsBanner());
        banner.AddService(new AdmobBanner());


        interstitial = new MasterAd();
        interstitial.AddService(new DummyOneInterstitial());
        interstitial.AddService(new DummyTwoInterstitial());
    }

    public void LoadBanner()
    {
        banner.LoadAd("Top-Banner-Ad", () => { text.text = "Banner Loaded"; });
    }

    public void ShowBanner()
    {
        banner.ShowAd("Top-Banner-Ad", () => { text.text = "Banner Finished"; });
    }

    public void LoadInterstitial()
    {
        interstitial.LoadAd("Full-Screen", () => { text.text = "Interstitial Loaded"; });
    }

    public void ShowInterstitial()
    {
        interstitial.ShowAd("Full-Screen", () =>
        {
            text.text = "Interstitial Finished";
            interstitial.DestroyAd("Full-Screen");
        });
    }
}