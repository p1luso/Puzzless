using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using JetBrains.Annotations;
using System;

public class AdManager : MonoBehaviour
{
    private BannerView _bannerView;
    private InterstitialAd interstitial;
    public string adUnitId;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
        });
        RequestBanner();
        LoadAd();
    }
    

    //InterstitialAds


    internal void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-9279792526470592/7914859163";
#elif UNITY_IPHONE
            string adUnitId = "";
#else
            string adUnitId = "unexpected_platform";
#endif
        /// <summary>
        /// Creates a 320x50 banner view at top of the screen.
        /// </summary>
        /// 
        Debug.Log("Creating interstitial view");

        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
            });

    }

    private InterstitialAd _interstitialAd;

    /// <summary>
    /// Loads the interstitial ad.
    /// </summary>
    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
            });
    }

    /// <summary>
    /// Shows the interstitial ad.
    /// </summary>
    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }
    
    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-9279792526470592/4328766121";
#elif UNITY_IPHONE
            string adUnitId = "";
#else
            string adUnitId = "unexpected_platform";
#endif
        /// <summary>
        /// Creates a 320x40 banner view at bottom of the screen.
        /// </summary>
        /// 
        Debug.Log("Creating banner view");
        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(adUnitId, AdSize.IABBanner, AdPosition.Bottom);

    }

    /// <summary>
    /// Destroys the banner view.
    /// </summary>
    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    public void LoadAd()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            Debug.Log("Creating banner view");

            // Create a 320x50 banner at top of the screen
            _bannerView = new BannerView(adUnitId, AdSize.IABBanner, AdPosition.Bottom);
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }
    
}