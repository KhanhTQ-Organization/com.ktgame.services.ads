using System.Collections.Generic;
using Sirenix.OdinInspector;
using com.ktgame.ads.core;
using com.ktgame.core;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
#endif

namespace com.ktgame.services.ads
{
	public class AdServiceSettings : ServiceSettingsSingleton<AdServiceSettings>
	{
		public override string PackageName => GetType().Namespace;

		[HideInInspector, SerializeField] [TabGroup("IronSource")] private string _androidIronSourceAppId;

		[HideInInspector, SerializeField] [TabGroup("MaxApplovin")] private string _androidMaxApplovinAppKey;
		
		[HideInInspector, SerializeField] [TabGroup("MaxApplovin")] private string _androidMaxApplovinAppOpenUnitId;

		[HideInInspector, SerializeField] [TabGroup("MaxApplovin")] private string _androidMaxApplovinBannerUnitId;

		[HideInInspector, SerializeField] [TabGroup("MaxApplovin")] private string _androidMaxApplovinInterstitialUnitId;

		[HideInInspector, SerializeField] [TabGroup("MaxApplovin")] private string _androidMaxApplovinRewardedVideoUnitId;

		[HideInInspector, SerializeField] [TabGroup("MaxApplovin")] private string _androidMaxApplovinMRecUnitId;

		[HideInInspector, SerializeField] [TabGroup("IronSource")] private string _iOSIronSourceAppId;

		[HideInInspector, SerializeField] [TabGroup("MaxApplovin")] private string _iOSMaxApplovinAppKey;

		[HideInInspector, SerializeField] [TabGroup("MaxApplovin")] private string _iOSMaxApplovinBannerUnitId;

		[HideInInspector, SerializeField] [TabGroup("MaxApplovin")] private string _iOSMaxApplovinInterstitialUnitId;

		[HideInInspector, SerializeField] [TabGroup("MaxApplovin")] private string _iOSMaxApplovinRewardedVideoUnitId;

		[HideInInspector, SerializeField] [TabGroup("MaxApplovin")] private string _iOSMaxApplovinMrecUnitId;

		[HideInInspector, SerializeField] [TabGroup("MaxApplovin")] private string _iOSMaxApplovinAppOpenUnitId;

		[HideInInspector, SerializeField] [TabGroup("Amazon")] private string _androidAmazonAppKey;

		[HideInInspector, SerializeField] [TabGroup("Amazon")] private string _androidAmazonBannerUnitId;

		[HideInInspector, SerializeField] [TabGroup("Amazon")] private string _androidAmazonInterstitialUnitId;

		[HideInInspector, SerializeField] [TabGroup("Amazon")] private string _androidAmazonRewardedVideoUnitId;

		[HideInInspector, SerializeField] [TabGroup("Amazon")] private string _iOSAmazonAppKey;

		[HideInInspector, SerializeField] [TabGroup("Amazon")] private string _iOSAmazonBannerUnitId;

		[HideInInspector, SerializeField] [TabGroup("Amazon")] private string _iOSAmazonInterstitialUnitId;

		[HideInInspector, SerializeField] [TabGroup("Amazon")] private string _iOSAmazonRewardedVideoUnitId;

		[HideInInspector, SerializeField] [TabGroup("Admob")] private string _androidAdmobAppKey;

		[HideInInspector, SerializeField] [TabGroup("Admob")] private string _androidAdmobAppOpenUnitId;
		
		[HideInInspector, SerializeField] [TabGroup("Admob")] private string _androidAdmobAppOpenResumeUnitId;

		[HideInInspector, SerializeField] [TabGroup("Admob")] private string _androidAdmobBannerUnitId;

		[HideInInspector, SerializeField] [TabGroup("Admob")] private string _androidAdmobNativeUnitId;

		[HideInInspector, SerializeField] [TabGroup("Admob")] private string _androidAdmobNativeInterUnitId;
		
		[HideInInspector, SerializeField] [TabGroup("Admob")] private string _androidAdmobNativeCollapsibleUnitId;

		[HideInInspector, SerializeField] [TabGroup("Admob")] private string _androidAdmobRewardedVideoUnitId;

		[HideInInspector, SerializeField] [TabGroup("Admob")] private string _androidAdmobInterstitialUnitId;

		[HideInInspector, SerializeField] [TabGroup("Admob")] private string _androidAdmobInterstitialImageUnitId;
		
		[HideInInspector, SerializeField] [TabGroup("Admob")] private string _iOSAdmobAppKey;

		[HideInInspector, SerializeField] [TabGroup("Admob")] private string _iOSAdmobAppOpenUnitId;

		[HideInInspector, SerializeField] [TabGroup("Admob")] private string _iOSAdmobNativeUnitId;

		[HideInInspector, SerializeField] [TabGroup("Admob")] private string _iOSAdmobRewardedVideoUnitId;

		[HideInInspector, SerializeField] [TabGroup("Admob")] private string _iOSAdmobInterstitialUnitId;

		[HideInInspector, SerializeField] [TabGroup("Admob")] private string _iOSAdmobBannerUnitId;

		[SerializeField] private BannerSize _bannerSize = BannerSize.Standard;

		[SerializeField] private BannerPosition _bannerPosition = BannerPosition.BottomCenter;

		[SerializeField] private Vector2 _mRecDp = new Vector2(300, 250);

		[SerializeField] private MRecPosition _mRecPosition = MRecPosition.BottomCenter;

		[SerializeField] private int _baseRetryDelay = 1;

		[SerializeField] private int _maxRetryAttemptRequest = 3;

		[SerializeField] private List<string> _placements;

		public string AndroidIronSourceAppId => _androidIronSourceAppId;

		public string AndroidMaxApplovinAppKey => _androidMaxApplovinAppKey;
		public string AndroidMaxApplovinAppOpenUnitId => _androidMaxApplovinAppOpenUnitId;

		public string AndroidMaxApplovinBannerUnitId => _androidMaxApplovinBannerUnitId;

		public string AndroidMaxApplovinInterstitialUnitId => _androidMaxApplovinInterstitialUnitId;

		public string AndroidMaxApplovinRewardedVideoUnitId => _androidMaxApplovinRewardedVideoUnitId;

		public string AndroidMaxApplovinMRecUnitId => _androidMaxApplovinMRecUnitId;
		public Vector2 MRecDp => _mRecDp;
		public MRecPosition MRecPosition => _mRecPosition;

		public string AndroidAdmobAppKey => _androidAdmobAppKey;
		public string AndroidAdmobAppOpenResumeUnitId => _androidAdmobAppOpenResumeUnitId;
		public string AndroidAdmobAppOpenUnitId => _androidAdmobAppOpenUnitId;
		public string AndroidAdmobBannerUnitId => _androidAdmobBannerUnitId;
		public string AndroidAdmobNativeInterUnitId => _androidAdmobNativeInterUnitId;
		public string AndroidAdmobNativeUnitId => _androidAdmobNativeUnitId;
		public string AndroidAdmobRewardedVideoUnitId => _androidAdmobRewardedVideoUnitId;
		public string AndroidAdmobInterstitialUnitId => _androidAdmobInterstitialUnitId;
		public string AndroidAdmobInterstitialImageUnitId => _androidAdmobInterstitialImageUnitId;
		public string AndroidAdmobNativeCollapsibleUnitId => _androidAdmobNativeCollapsibleUnitId;
		public string IOSAdmobAppKey => _iOSAdmobAppKey;
		public string IOSAdmobAppOpenUnitId => _iOSAdmobAppOpenUnitId;

		public string IOSAdmobNativeUnitId => _iOSAdmobNativeUnitId;

		public string IOSAdmobRewardedVideoUnitId => _iOSAdmobRewardedVideoUnitId;

		public string IOSAdmobInterstitialUnitId => _iOSAdmobInterstitialUnitId;
		public string IOSAdmobBannerUnitId => _iOSAdmobBannerUnitId;

		public string IOSMaxApplovinMRecUnitId => _iOSMaxApplovinMrecUnitId;

		public string IOSIronSourceAppId => _iOSIronSourceAppId;

		public string IOSMaxApplovinAppKey => _iOSMaxApplovinAppKey;

		public string IOSMaxApplovinBannerUnitId => _iOSMaxApplovinBannerUnitId;

		public string IOSMaxApplovinInterstitialUnitId => _iOSMaxApplovinInterstitialUnitId;

		public string IOSMaxApplovinRewardedVideoUnitId => _iOSMaxApplovinRewardedVideoUnitId;
		
		public string IOSMaxApplovinAppOpenUnitId => _iOSMaxApplovinAppOpenUnitId;

		public string AndroidAmazonAppKey => _androidAmazonAppKey;

		public string AndroidAmazonBannerUnitId => _androidAmazonBannerUnitId;

		public string AndroidAmazonInterstitialUnitId => _androidAmazonInterstitialUnitId;

		public string AndroidAmazonRewardedVideoUnitId => _androidAmazonRewardedVideoUnitId;

		public string IOSAmazonAppKey => _iOSAmazonAppKey;

		public string IOSAmazonBannerUnitId => _iOSAmazonBannerUnitId;

		public string IOSAmazonInterstitialUnitId => _iOSAmazonInterstitialUnitId;

		public string IOSAmazonRewardedVideoUnitId => _iOSAmazonRewardedVideoUnitId;

		public BannerSize BannerSize => _bannerSize;

		public BannerPosition BannerPosition => _bannerPosition;

		public int BaseRetryDelay => _baseRetryDelay;

		public int MaxRetryAttemptRequest => _maxRetryAttemptRequest;

#if UNITY_EDITOR
		[Button("Ads Location Generate")]
		private void AdPlacementGenerate()
		{
			if (_placements.Count <= 0)
				return;

			var builder = new StringBuilder();
			builder.Append("using com.ktgame.ads.core;").Append("\n").Append("\n");
			builder.AppendFormat("namespace {0}", PackageName).Append("\n").Append("{").Append("\n");
			builder.Append("\t").Append("public static class AdLocation").Append("\n");
			builder.Append("\t").Append("{").Append("\n");
			foreach (var placement in _placements)
			{
				builder.Append("\t\t").AppendFormat("public static AdPlacement {0}", placement).Append(" = ")
					.AppendFormat("new AdPlacement(\"{0}\")", placement).Append(";").Append("\n");
			}

			builder.Append("\t").Append("}").Append("\n");
			builder.Append("}").Append("\n");
			var fileText = builder.ToString();

			var saveFolderPath = Path.Combine(Application.dataPath, "Scripts/Generated");
			var saveFilePath = Path.Combine(saveFolderPath, "AdLocationGenerate.cs");

			if (!Directory.Exists(saveFolderPath))
			{
				Directory.CreateDirectory(saveFolderPath);
			}

			if (File.Exists(saveFilePath))
			{
				File.Delete(saveFilePath);
			}

			if (File.Exists(saveFilePath + ".meta"))
			{
				File.Delete(saveFilePath + ".meta");
			}

			File.WriteAllText(saveFilePath, fileText, Encoding.UTF8);
			AssetDatabase.ImportAsset(saveFilePath);
			AssetDatabase.Refresh();
		}
#endif
	}
}