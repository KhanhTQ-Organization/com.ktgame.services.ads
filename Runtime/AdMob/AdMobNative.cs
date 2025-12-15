using System;
using System.Collections.Generic;

#if APPSFLYER_ANALYTICS
using AppsFlyerSDK;
#endif

using com.ktgame.ads.core;
using com.ktgame.services.ads;
using Cysharp.Threading.Tasks;
using DG.Tweening;

#if ADMOB
using GoogleMobileAds.Api;
#endif

using UnityEngine;
using AdError = com.ktgame.ads.core.AdError;
using AdFormat = com.ktgame.ads.core.AdFormat;

namespace com.ktgame.ads.admob
{
    public class AdMobNative : INativeAdapter
    {
        protected string UnitId { private set; get; }
        
        public event Action<AdPlacement> OnLoadSucceeded;
        public event Action<AdError> OnLoadFailed;
        public event Action<AdPlacement> OnClicked;
        public event Action<AdPlacement> OnShowSucceeded;
        public event Action<ImpressionData> OnPaid;
        public event Action OnClosed;
        public event Action<ImpressionData> OnImpressionSuccess;
        private AdPlacement AdPlacement { get; }
        private bool _isLoading = false;
        private int _retry = 0;
        
#if ADMOB_NATIVE
        public List<NativeAd> NativeAdsTemp { get; }
        public NativeAd CurrentNativeAd => _currentNativeAd;
        public Queue<NativeAd> NativeAdsPreload { get; }

        private AdLoader _adLoader;
        private NativeAd _currentNativeAd;

        public bool IsReady => _currentNativeAd != null;
#else
		public bool IsReady => true;
#endif

        private readonly int _maxPreloadCount = 2;

        public AdMobNative(string unitId)
        {
            UnitId = unitId;
            AdPlacement = new AdPlacement("Native");
        }

        public void Load()
        {
            LoadAsync().Forget();
        }

        private async UniTask LoadAsync()
        {
            if (_isLoading) return;

            _isLoading = true;
            
#if ADMOB_NATIVE
            if (_currentNativeAd != null)
            {
                DestroyNative();
            }

            UniTask.WaitUntil(() => _currentNativeAd == null).ContinueWith(() =>
            {

                Debug.Log($"[Native] Loading native ad {UnitId}.");

                _adLoader = new AdLoader.Builder(UnitId)
                    .ForNativeAd()
                    .Build();

                _adLoader.OnNativeAdLoaded += OnLoadSucceededHandler;
                _adLoader.OnAdFailedToLoad += OnLoadFailedHandler;
                _adLoader.OnNativeAdClicked += ClickedHandler;
                _adLoader.OnNativeAdClosed += ClosedHandler;

                _adLoader.LoadAd(new AdRequest());

            }).Forget();
#endif
        }

        public void Show()
        {
            if (!IsReady)
            {
                Debug.LogWarning("No Native Ads ready to show.");
                return;
            }

            OnShowSucceeded?.Invoke(AdPlacement);

            Load();
        }

        public void Hide()
        {

        }

        public void Destroy()
        {
#if ADMOB_NATIVE
            if (_currentNativeAd != null)
            {
                _currentNativeAd.Destroy();
                _currentNativeAd = null;
            }
#endif
        }

#if ADMOB_NATIVE
        private void OnLoadSucceededHandler(object sender, NativeAdEventArgs nativeAdEventArgs)
        {
            _isLoading = false;
            _retry = 0;
            
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                _currentNativeAd = nativeAdEventArgs.nativeAd;
                _currentNativeAd.OnPaidEvent += OnNativeAdImpression;
                OnLoadSucceeded?.Invoke(AdPlacement);
            });
        }

        private void OnNativeAdImpression(object sender, AdValueEventArgs e)
        {
            double revenue = e.AdValue.Value / 1000000d;
            OnPaid?.Invoke(new ImpressionData(AdPlatform.Admob, "unknow", UnitId, AdFormat.Native, "Native", "USD", revenue));
        }

        private void OnLoadFailedHandler(object sender, AdFailedToLoadEventArgs adFailedEventArgs)
        {
            _isLoading = false;
            _retry++;
            
            var adError = AdMobExtensions.ToAdError(adFailedEventArgs.LoadAdError, AdPlacement);
            OnLoadFailed?.Invoke(adError);
            Debug.LogError($"Native ad failed:{adFailedEventArgs.LoadAdError.GetCode()} - {adFailedEventArgs.LoadAdError.GetMessage()}");
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                float delay = Mathf.Clamp(Mathf.Pow(2, Mathf.Min(5, _retry)), 2, 40);
                DOVirtual.DelayedCall(delay, Load);
            });
        }

#endif
        private void ClosedHandler(object sender, EventArgs eventArgs)
        {
            OnClosed?.Invoke();
        }

        private void ClickedHandler(object sender, EventArgs eventArgs)
        {
            OnClicked?.Invoke(AdPlacement);
        }
        
        private void DestroyNative()
        {
#if ADMOB_NATIVE
            if (_currentNativeAd != null)
            {
                try
                {
                    _currentNativeAd.OnPaidEvent -= OnNativeAdImpression;
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        try
                        {
                            _currentNativeAd.Destroy();
                        }
                        catch
                        {
                            Debug.LogWarning("[Native] DestroyNative - Exception when destroying native ad.");
                        }
                    });
                }
                catch
                {
                    Debug.LogWarning("[Native] DestroyNative - Exception when destroying native ad.");
                }
                _currentNativeAd = null;
            }
#endif
        }
    }
}