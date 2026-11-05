using System;
using com.ktgame.core;
using com.ktgame.core.editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace com.ktgame.services.ads.editor
{
	public class IAAEditor
	{
		private const string NativeDefine = "ADMOB_NATIVE";

		private KTSettingSO _setting;
		private IAASettingSO _iaaSetting;

		public IAAEditor(KTSettingSO setting, IAASettingSO iaaSetting)
		{
			_setting = setting;
			_iaaSetting = iaaSetting;
			GlobalAdSettings = AdServiceSettings.Instance;
		}

		[Title("In-App Ads Configuration", "Manage your mediation networks, priorities, and unit IDs.", TitleAlignments.Centered)]
		[PropertyOrder(-10)]
		[ShowInInspector, HideLabel, DisplayAsString(false)]
#pragma warning disable CS0414
		private string _header = "";
#pragma warning restore CS0414

		#region Mediation Strategy

		[BoxGroup("Mediation Strategy", CenterLabel = true, Order = 10)]
		[PropertySpace(SpaceBefore = 6, SpaceAfter = 6)]
		[LabelText("Active Mediations"), LabelWidth(150), ShowInInspector, EnumToggleButtons]
		public IAAMediationFlag MediationFlag
		{
			get => _setting.MediationFlag;
			set
			{
				if (((_setting.MediationFlag & IAAMediationFlag.Max) != 0)
					&& (value & IAAMediationFlag.IronSource) != 0)
				{
					_setting.MediationFlag = value;
					_setting.MediationFlag &= ~IAAMediationFlag.Max;
				}
				else if (((_setting.MediationFlag & IAAMediationFlag.IronSource) != 0)
						 && (value & IAAMediationFlag.Max) != 0)
				{
					_setting.MediationFlag = value;
					_setting.MediationFlag &= ~IAAMediationFlag.IronSource;
				}
				else
				{
					_setting.MediationFlag = value;
				}
#if UNITY_EDITOR
				UnityEditor.EditorUtility.SetDirty(_setting);
#endif
			}
		}

		[BoxGroup("Mediation Strategy")]
		[Indent(1)]
		[LabelText("Main Network"), LabelWidth(150), ShowInInspector, EnumPaging]
		public IAAMediationFlag MainMediation
		{
			get => _iaaSetting.MainMediation;
			set => _iaaSetting.MainMediation = value;
		}

		[BoxGroup("Mediation Strategy")]
		[Indent(1)]
		[LabelText("Backfill Network"), LabelWidth(150), ShowInInspector, EnumPaging]
		public IAAMediationFlag BackFillMediation
		{
			get => _iaaSetting.BackfillMediation;
			set => _iaaSetting.BackfillMediation = value;
		}

		[BoxGroup("Mediation Strategy")]
		[OnInspectorGUI]
		private void DrawMediationWarning()
		{
			var warning = MediationWarning;
			if (warning != null)
			{
				SirenixEditorGUI.WarningMessageBox(warning);
			}
		}

		private string MediationWarning
		{
			get
			{
				var active = MediationFlag;
				if (MainMediation != 0 && (active & MainMediation) == 0)
				{
					return $"Main network '{MainMediation}' is not enabled in Active Mediations.";
				}

				if (BackFillMediation != 0 && (active & BackFillMediation) == 0)
				{
					return $"Backfill network '{BackFillMediation}' is not enabled in Active Mediations.";
				}

				if (MainMediation != 0 && MainMediation == BackFillMediation)
				{
					return "Main and Backfill are the same network - backfill has no effect.";
				}

				return null;
			}
		}

		#endregion

		#region Shared SDK helpers

		private static bool IsInstallerBusy => AdSdkInstaller.IsBusy;

		private void SaveIaaSetting()
		{
			if (_iaaSetting != null)
			{
				UnityEditor.EditorUtility.SetDirty(_iaaSetting);
			}
		}

		private static string InstalledSuffix(UpmAdSdk sdk)
		{
			var installed = AdSdkInstaller.GetInstalledUpmVersion(sdk);
			return installed == null ? "not installed" : "installed";
		}

		private static string ActionLabel(UpmAdSdk sdk, string version)
		{
			var installed = AdSdkInstaller.GetInstalledUpmVersion(sdk);
			if (installed == null) return "Install";
			return installed == (version ?? string.Empty).Trim() ? "Up to date" : "Update";
		}

		private static bool CanRunAction(UpmAdSdk sdk, string version)
		{
			return !IsInstallerBusy && !AdSdkInstaller.IsLegacyOnly(sdk) &&
			       AdSdkInstaller.GetInstalledUpmVersion(sdk) != (version ?? string.Empty).Trim();
		}

		/// <summary>Only real problems are reported; null when everything is fine.</summary>
		private static string SdkNotice(UpmAdSdk sdk)
		{
			if (AdSdkInstaller.IsLegacyOnly(sdk))
			{
				return $"{sdk.DisplayName} is installed as a .unitypackage (Assets/{sdk.LegacyAssetsFolder}). {sdk.LegacyHint}";
			}

			if (!AdSdkInstaller.IsInstalled(sdk))
			{
				return $"{sdk.DisplayName} SDK is not installed.";
			}

			if (!AdSdkInstaller.HasDefine(sdk))
			{
				return $"Define {sdk.DefineSymbol} is OFF - ads code for {sdk.DisplayName} is not compiled.";
			}

			return null;
		}

		#endregion

		#region AppLovin MAX

		private const string MaxGroup = "AppLovin MAX Configuration";
		private const string MaxRow = MaxGroup + "/Sdk";
		private static UpmAdSdk Max => AdSdkInstaller.Max;
		private bool IsMaxOn => MediationFlag.HasFlag(IAAMediationFlag.Max);
		private string MaxSuffix => InstalledSuffix(Max);
		private string MaxActionLabel => ActionLabel(Max, MaxSdkVersion);
		private bool CanMaxAction => CanRunAction(Max, MaxSdkVersion);
		private static bool IsMaxUpm => AdSdkInstaller.IsInstalledViaUpm(Max);

		[ShowIf(nameof(IsMaxOn))]
		[BoxGroup(MaxGroup, CenterLabel = true, Order = 20)]
		[PropertyOrder(-1)]
		[OnInspectorGUI]
		private void DrawMaxNotice()
		{
			var notice = SdkNotice(Max);
			if (notice != null)
			{
				SirenixEditorGUI.WarningMessageBox(notice);
			}
		}

		[ShowIf(nameof(IsMaxOn))]
		[HorizontalGroup(MaxRow, Order = 0)]
		[LabelText("SDK Version"), LabelWidth(85)]
		[SuffixLabel("$MaxSuffix", Overlay = true)]
		[DisableIf("@IsInstallerBusy")]
		[ShowInInspector]
		public string MaxSdkVersion
		{
			get => !string.IsNullOrEmpty(_iaaSetting?.MaxSdkVersion) ? _iaaSetting.MaxSdkVersion : Max.DefaultVersion;
			set
			{
				if (_iaaSetting == null) return;
				_iaaSetting.MaxSdkVersion = value;
				SaveIaaSetting();
			}
		}

		[ShowIf(nameof(IsMaxOn))]
		[HorizontalGroup(MaxRow, Width = 90)]
		[EnableIf(nameof(CanMaxAction))]
		[Button("$MaxActionLabel"), GUIColor(0.45f, 0.75f, 1f)]
		private void InstallMaxSdk()
		{
			AdSdkInstaller.Install(Max, MaxSdkVersion, installed => MaxSdkVersion = installed);
		}

		[ShowIf("@IsMaxOn && IsMaxUpm")]
		[HorizontalGroup(MaxRow, Width = 26)]
		[DisableIf("@IsInstallerBusy")]
		[PropertyTooltip("Remove AppLovin MAX SDK")]
		[Button(SdfIconType.Trash, ""), GUIColor(1f, 0.55f, 0.55f)]
		private void RemoveMaxSdk()
		{
			AdSdkInstaller.Remove(Max);
		}

		[ShowIf(nameof(IsMaxOn))]
		[HorizontalGroup(MaxRow, Width = 26)]
		[PropertyTooltip("Open AppLovin dashboard")]
		[Button(SdfIconType.BoxArrowUpRight, "")]
		private void OpenMaxDashboard()
		{
			Application.OpenURL("https://dash.applovin.com/");
		}

		[ShowIf(nameof(IsMaxOn))]
		[BoxGroup(MaxGroup, CenterLabel = true, Order = 20)]
		[TabGroup(MaxGroup + "/Platform", "Android", SdfIconType.Robot, Order = 1)]
		[ShowInInspector, HideReferenceObjectPicker, InlineProperty, HideLabel]
		public IaaUnitID MaxIaaUnitIDAndroid
		{
			get => _iaaSetting.MaxAndroid;
			set => _iaaSetting.MaxAndroid = value;
		}

		[ShowIf(nameof(IsMaxOn))]
		[TabGroup(MaxGroup + "/Platform", "iOS", SdfIconType.Apple, Order = 1)]
		[ShowInInspector, HideReferenceObjectPicker, InlineProperty, HideLabel]
		public IaaUnitID MaxIaaUnitIDIos
		{
			get => _iaaSetting.MaxIos;
			set => _iaaSetting.MaxIos = value;
		}

		#endregion

		#region IronSource

		private bool IsIronOn => MediationFlag.HasFlag(IAAMediationFlag.IronSource);

		[ShowIf(nameof(IsIronOn))]
		[BoxGroup("IronSource Configuration", CenterLabel = true, Order = 30)]
		[TabGroup("IronSource Configuration/Platform", "Android", SdfIconType.Robot)]
		[ShowInInspector, HideReferenceObjectPicker, InlineProperty, HideLabel]
		public IaaUnitID IronIaaUnitIDAndroid
		{
			get => _iaaSetting.IronAndroid;
			set => _iaaSetting.IronAndroid = value;
		}

		[ShowIf(nameof(IsIronOn))]
		[TabGroup("IronSource Configuration/Platform", "iOS", SdfIconType.Apple)]
		[ShowInInspector, HideReferenceObjectPicker, InlineProperty, HideLabel]
		public IaaUnitID IronIaaUnitIDIos
		{
			get => _iaaSetting.IronIos;
			set => _iaaSetting.IronIos = value;
		}

		#endregion

		#region Google AdMob

		private const string GmaGroup = "Google AdMob Configuration";
		private const string GmaRow = GmaGroup + "/Sdk";
		private static UpmAdSdk Gma => AdSdkInstaller.Gma;
		private bool IsGmaOn => MediationFlag.HasFlag(IAAMediationFlag.GMA);
		private string GmaSuffix => InstalledSuffix(Gma);
		private string GmaActionLabel => ActionLabel(Gma, GmaSdkVersion);
		private bool CanGmaAction => CanRunAction(Gma, GmaSdkVersion);
		private static bool IsGmaUpm => AdSdkInstaller.IsInstalledViaUpm(Gma);

		private string GmaNotice
		{
			get
			{
				var notice = SdkNotice(Gma);
				if (notice != null)
				{
					return notice;
				}

				if (Version.TryParse((GmaSdkVersion ?? string.Empty).Trim(), out var version) && version >= new Version(11, 6, 0))
				{
					return "GMA 11.6+ needs com.unity.external-dependency-manager, which conflicts with Firebase's EDM. Use 11.5.0 or lower.";
				}

				const IAAFormatType nativeFormats = IAAFormatType.Native | IAAFormatType.NativeInterstitial | IAAFormatType.NativeCollapsible;
				var usesNative = _iaaSetting != null &&
				                 (((_iaaSetting.GmaAndroid?.IAAFormats ?? 0) | (_iaaSetting.GmaIos?.IAAFormats ?? 0)) & nativeFormats) != 0;
				if (usesNative && !DefineSymbolsEditor.HasDefineSymbol(NativeDefine))
				{
					return $"Native formats are selected but the {NativeDefine} define is OFF.";
				}

				return null;
			}
		}

		[ShowIf(nameof(IsGmaOn))]
		[BoxGroup(GmaGroup, CenterLabel = true, Order = 40)]
		[PropertyOrder(-1)]
		[OnInspectorGUI]
		private void DrawGmaNotice()
		{
			var notice = GmaNotice;
			if (notice != null)
			{
				SirenixEditorGUI.WarningMessageBox(notice);
			}
		}

		[ShowIf(nameof(IsGmaOn))]
		[HorizontalGroup(GmaRow, Order = 0)]
		[LabelText("SDK Version"), LabelWidth(85)]
		[SuffixLabel("$GmaSuffix", Overlay = true)]
		[DisableIf("@IsInstallerBusy")]
		[ShowInInspector]
		public string GmaSdkVersion
		{
			get => !string.IsNullOrEmpty(_iaaSetting?.GmaSdkVersion) ? _iaaSetting.GmaSdkVersion : Gma.DefaultVersion;
			set
			{
				if (_iaaSetting == null) return;
				_iaaSetting.GmaSdkVersion = value;
				SaveIaaSetting();
			}
		}

		[ShowIf(nameof(IsGmaOn))]
		[HorizontalGroup(GmaRow, Width = 90)]
		[EnableIf(nameof(CanGmaAction))]
		[Button("$GmaActionLabel"), GUIColor(0.45f, 0.75f, 1f)]
		private void InstallGmaSdk()
		{
			AdSdkInstaller.Install(Gma, GmaSdkVersion, installed => GmaSdkVersion = installed);
		}

		[ShowIf("@IsGmaOn && IsGmaUpm")]
		[HorizontalGroup(GmaRow, Width = 26)]
		[DisableIf("@IsInstallerBusy")]
		[PropertyTooltip("Remove Google Mobile Ads SDK")]
		[Button(SdfIconType.Trash, ""), GUIColor(1f, 0.55f, 0.55f)]
		private void RemoveGmaSdk()
		{
			AdSdkInstaller.Remove(Gma);
		}

		[ShowIf(nameof(IsGmaOn))]
		[HorizontalGroup(GmaRow, Width = 26)]
		[PropertyTooltip("Open AdMob dashboard")]
		[Button(SdfIconType.BoxArrowUpRight, "")]
		private void OpenAdMobDashboard()
		{
			Application.OpenURL("https://apps.admob.com/");
		}

		[ShowIf(nameof(IsGmaOn))]
		[BoxGroup(GmaGroup, CenterLabel = true, Order = 40)]
		[TabGroup(GmaGroup + "/Platform", "Android", SdfIconType.Robot, Order = 1)]
		[ShowInInspector, HideReferenceObjectPicker, InlineProperty, HideLabel]
		public IaaGmaUnitID GmaIaaUnitIDAndroid
		{
			get => _iaaSetting.GmaAndroid;
			set => _iaaSetting.GmaAndroid = value;
		}

		[ShowIf(nameof(IsGmaOn))]
		[TabGroup(GmaGroup + "/Platform", "iOS", SdfIconType.Apple, Order = 1)]
		[ShowInInspector, HideReferenceObjectPicker, InlineProperty, HideLabel]
		public IaaGmaUnitID GmaIaaUnitIDIos
		{
			get => _iaaSetting.GmaIos;
			set => _iaaSetting.GmaIos = value;
		}

		#endregion

		[PropertySpace(SpaceBefore = 10)]
		[HideLabel]
		[PropertyOrder(100)]
		[InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
		public AdServiceSettings GlobalAdSettings;
	}
}
