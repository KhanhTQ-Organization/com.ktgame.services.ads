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

		[SerializeField] [TabGroup("IronSource")] private string _androidIronSourceAppId;

		[SerializeField] [TabGroup("MaxApplovin")] private string _androidMaxApplovinAppKey;

		[SerializeField] [TabGroup("MaxApplovin")] private string _androidMaxApplovinBannerUnitId;

		[SerializeField] [TabGroup("MaxApplovin")] private string _androidMaxApplovinInterstitialUnitId;

		[SerializeField] [TabGroup("MaxApplovin")] private string _androidMaxApplovinRewardedVideoUnitId;

		[SerializeField] [TabGroup("MaxApplovin")] private string _androidMaxApplovinMRecUnitId;

		[SerializeField] [TabGroup("IronSource")] private string _iOSIronSourceAppId;

		[SerializeField] [TabGroup("MaxApplovin")] private string _iOSMaxApplovinAppKey;

		[SerializeField] [TabGroup("MaxApplovin")] private string _iOSMaxApplovinBannerUnitId;

		[SerializeField] [TabGroup("MaxApplovin")] private string _iOSMaxApplovinInterstitialUnitId;

		[SerializeField] [TabGroup("MaxApplovin")] private string _iOSMaxApplovinRewardedVideoUnitId;

		[SerializeField] [TabGroup("MaxApplovin")] private string _iOSMaxApplovinMrecUnitId;

		[SerializeField] [TabGroup("Amazon")] private string _androidAmazonAppKey;

		[SerializeField] [TabGroup("Amazon")] private string _androidAmazonBannerUnitId;

		[SerializeField] [TabGroup("Amazon")] private string _androidAmazonInterstitialUnitId;

		[SerializeField] [TabGroup("Amazon")] private string _androidAmazonRewardedVideoUnitId;

		[SerializeField] [TabGroup("Amazon")] private string _iOSAmazonAppKey;

		[SerializeField] [TabGroup("Amazon")] private string _iOSAmazonBannerUnitId;

		[SerializeField] [TabGroup("Amazon")] private string _iOSAmazonInterstitialUnitId;

		[SerializeField] [TabGroup("Amazon")] private string _iOSAmazonRewardedVideoUnitId;

		[SerializeField] [TabGroup("Admob")] private string _androidAdmobAppKey;

		[SerializeField] [TabGroup("Admob")] private string _androidAdmobAppOpenUnitId;

		[SerializeField] [TabGroup("Admob")] private string _androidAdmobBannerUnitId;

		[SerializeField] [TabGroup("Admob")] private string _androidAdmobNativeUnitId;
		
		[SerializeField] [TabGroup("Admob")] private string _androidAdmobNativeInterUnitId;

		[SerializeField] [TabGroup("Admob")] private string _androidAdmobRewardedVideoUnitId;

		[SerializeField] [TabGroup("Admob")] private string _androidAdmobInterstitialUnitId;

		[SerializeField] [TabGroup("Admob")] private string _iOSAdmobAppOpenUnitId;

		[SerializeField] [TabGroup("Admob")] private string _iOSAdmobNativeUnitId;

		[SerializeField] [TabGroup("Admob")] private string _iOSAdmobRewardedVideoUnitId;

		[SerializeField] [TabGroup("Admob")] private string _iOSAdmobInterstitialUnitId;

		[SerializeField] [TabGroup("Admob")] private string _iOSAdmobBannerUnitId;
		
		[SerializeField] private BannerSize _bannerSize = BannerSize.Standard;

		[SerializeField] private BannerPosition _bannerPosition = BannerPosition.Bottom;

		[SerializeField] private Vector2 _mRecDp = new Vector2(300, 250);

		[SerializeField] private MRecPosition _mRecPosition = MRecPosition.BottomCenter;

		[SerializeField] private int _baseRetryDelay = 1;

		[SerializeField] private int _maxRetryAttemptRequest = 3;

		[SerializeField] private List<string> _placements;

		public string AndroidIronSourceAppId => _androidIronSourceAppId;

		public string AndroidMaxApplovinAppKey => _androidMaxApplovinAppKey;

		public string AndroidMaxApplovinBannerUnitId => _androidMaxApplovinBannerUnitId;

		public string AndroidMaxApplovinInterstitialUnitId => _androidMaxApplovinInterstitialUnitId;

		public string AndroidMaxApplovinRewardedVideoUnitId => _androidMaxApplovinRewardedVideoUnitId;

		public string AndroidMaxApplovinMRecUnitId => _androidMaxApplovinMRecUnitId;
		public Vector2 MRecDp => _mRecDp;
		public MRecPosition MRecPosition => _mRecPosition;

		public string AndroidAdmobAppKey => _androidAdmobAppKey;
		public string AndroidAdmobAppOpenUnitId => _androidAdmobAppOpenUnitId;
		public string AndroidAdmobBannerUnitId => _androidAdmobBannerUnitId;
		public string AndroidAdmobNativeInterUnitId => _androidAdmobNativeInterUnitId;
		public string AndroidAdmobNativeUnitId => _androidAdmobNativeUnitId;
		public string AndroidAdmobRewardedVideoUnitId => _androidAdmobRewardedVideoUnitId;
		public string AndroidAdmobInterstitialUnitId => _androidAdmobInterstitialUnitId;
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
