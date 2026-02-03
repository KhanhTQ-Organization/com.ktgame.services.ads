using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace com.ktgame.core
{
	[CreateAssetMenu(menuName = "Setting/IAA")]
	public class IAASettingSO : ServiceSettingsSingleton<IAASettingSO>
	{
		public override string PackageName => GetType().Namespace;
		
		[ReadOnly] public IaaUnitID MaxAndroid;
		[ReadOnly] public IaaUnitID MaxIos;
		[ReadOnly] public IaaUnitID IronAndroid;
		[ReadOnly] public IaaUnitID IronIos;
		[ReadOnly] public IaaGmaUnitID GmaAndroid;
		[ReadOnly] public IaaGmaUnitID GmaIos;
		
		[ReadOnly] public bool UseLocalConfig;
		[ReadOnly] public bool IsDebugMode;
		[ReadOnly] public bool IsDebugLog;
		[ReadOnly] public int MaxNativeAdsPreload;
		
		[ReadOnly,EnumPaging] public IAAMediationFlag MainMediation;
		[ReadOnly,EnumPaging] public IAAMediationFlag BackfillMediation;
		
		public bool IsUsingFormatGMA(IAAFormatType formatType)
		{
			return (GmaAndroid.IAAFormats & formatType) == formatType;
		}
		
		public bool IsUsingFormatMax(IAAFormatType formatType)
		{
			return (MaxAndroid.IAAFormats & formatType) == formatType;
		}
		
		public bool IsUsingFormatIronsource(IAAFormatType formatType)
		{
			return (IronAndroid.IAAFormats & formatType) == formatType;
		}
	}

	[Serializable]
	public class IaaUnitID
	{
		public IAAFormatType IAAFormats;
		
		public string AppID;
		
		[ShowIf("@(IAAFormats.HasFlag(IAAFormatType.Interstitial))")]
		public string InterstitialUnitID;
		
		[ShowIf("@(IAAFormats.HasFlag(IAAFormatType.Reward))")]
		public string RewardUnitID;
		
		[ShowIf("@(IAAFormats.HasFlag(IAAFormatType.Banner))")]
		public string BannerUnitID;
		
		[ShowIf("@(IAAFormats.HasFlag(IAAFormatType.MRec))")]
		public string MRecUnitID;
		
		[ShowIf("@(IAAFormats.HasFlag(IAAFormatType.Aoa))")]
		public string AoaUnitID;
	}

	[Serializable]
	public class IaaGmaUnitID: IaaUnitID
	{
		[ShowIf("@(IAAFormats.HasFlag(IAAFormatType.InterstitialImage))")]
		public string InterstitialImageUnitID;
		
		[ShowIf("@(IAAFormats.HasFlag(IAAFormatType.Native))")]
		public string NativeUnitID;
		
		[ShowIf("@(IAAFormats.HasFlag(IAAFormatType.NativeInterstitial))")]
		public string NativeInterstitialUnitID;
		
		[ShowIf("@(IAAFormats.HasFlag(IAAFormatType.BannerCollapsible))")]
		public string BannerCollapsibleUnitID;
		
		[ShowIf("@(IAAFormats.HasFlag(IAAFormatType.AoaResume))")]
		public string AoaResumeUnitID;
		
		[ShowIf("@(IAAFormats.HasFlag(IAAFormatType.NativeCollapsible))")]
		public string NativeCollapsibleUnitID;
	}
}