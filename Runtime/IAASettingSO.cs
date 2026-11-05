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

		[HideInInspector] public string MaxSdkVersion = "8.6.6";
		
		[HideInInspector] public string GmaSdkVersion = "11.5.0";

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
		protected virtual IAAFormatType AllowedFormats =>
			IAAFormatType.Banner | IAAFormatType.Interstitial | IAAFormatType.Reward | IAAFormatType.MRec | IAAFormatType.Aoa;

		[LabelText("Formats")]
		[CustomValueDrawer("DrawFormats")]
		public IAAFormatType IAAFormats;
		
		[HideIf("@$property.Parent != null && $property.Parent.Name.StartsWith(\"Max\")")]
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

#if UNITY_EDITOR
		private IAAFormatType DrawFormats(IAAFormatType value, GUIContent label)
		{
			var allowed = AllowedFormats;
			var formats = new System.Collections.Generic.List<IAAFormatType>();
			var names = new System.Collections.Generic.List<string>();
			foreach (IAAFormatType format in Enum.GetValues(typeof(IAAFormatType)))
			{
				if ((allowed & format) == 0) continue;
				formats.Add(format);
				names.Add(UnityEditor.ObjectNames.NicifyVariableName(format.ToString()));
			}
			
			var mask = 0;
			for (var i = 0; i < formats.Count; i++)
			{
				if ((value & formats[i]) != 0) mask |= 1 << i;
			}

			var newMask = UnityEditor.EditorGUILayout.MaskField(label, mask, names.ToArray());
			
			IAAFormatType result = 0;
			for (var i = 0; i < formats.Count; i++)
			{
				if ((newMask & (1 << i)) != 0) result |= formats[i];
			}

			return result;
		}
#endif
	}

	[Serializable]
	public class IaaGmaUnitID: IaaUnitID
	{
		protected override IAAFormatType AllowedFormats =>
			IAAFormatType.BannerCollapsible | IAAFormatType.Interstitial | IAAFormatType.InterstitialImage |
			IAAFormatType.Reward | IAAFormatType.Aoa | IAAFormatType.AoaResume |
			IAAFormatType.Native | IAAFormatType.NativeInterstitial | IAAFormatType.NativeCollapsible;

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
