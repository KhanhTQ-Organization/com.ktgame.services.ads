using com.ktgame.core;
using com.ktgame.core.editor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace com.ktgame.services.ads.editor
{
	public class IAAEditor
	{
		private KTSettingSO _setting;
		private IAASettingSO _iaaSetting;

		public IAAEditor(KTSettingSO setting, IAASettingSO iaaSetting)
		{
			_setting = setting;
			_iaaSetting = iaaSetting;
			GlobalAdSettings = AdServiceSettings.Instance;
		}

		[Title("In-App Ads Configuration", "Manage your mediation networks, priorities, and unit IDs.", TitleAlignments.Centered)]
		[InfoBox("Select and configure the mediation platforms for your game. Ensure you input the correct App ID and Unit IDs for each platform before building.", InfoMessageType.Info)]
		[PropertyOrder(-10)]
		[ShowInInspector, HideLabel, DisplayAsString(false)]
		private string _header = "";
		
		[BoxGroup("Mediation Strategy", CenterLabel = true)]
		[PropertySpace(SpaceBefore = 10, SpaceAfter = 10)]
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
		[LabelText("Main Network:"), LabelWidth(150), ShowInInspector, EnumPaging]
		public IAAMediationFlag MainMediation
		{
			get => _iaaSetting.MainMediation;
			set => _iaaSetting.MainMediation = value;
		}

		[BoxGroup("Mediation Strategy")]
		[PropertySpace(0, 10)]
		[Indent(1)]
		[LabelText("Backfill Network:"), LabelWidth(150), ShowInInspector, EnumPaging]
		public IAAMediationFlag BackFillMediation
		{
			get => _iaaSetting.BackfillMediation;
			set => _iaaSetting.BackfillMediation = value;
		}

		// AppLovin MAX
		[ShowIf("@(MediationFlag.HasFlag(IAAMediationFlag.Max))")]
		[BoxGroup("AppLovin MAX Configuration", CenterLabel = true)]
		[TabGroup("AppLovin MAX Configuration/Platform", "Android", SdfIconType.Robot)]
		[ShowInInspector, HideReferenceObjectPicker, InlineProperty, HideLabel, Indent(1)]
		public IaaUnitID MaxIaaUnitIDAndroid
		{
			get => _iaaSetting.MaxAndroid;
			set => _iaaSetting.MaxAndroid = value;
		}

		[ShowIf("@(MediationFlag.HasFlag(IAAMediationFlag.Max))")]
		[TabGroup("AppLovin MAX Configuration/Platform", "iOS", SdfIconType.Apple)]
		[ShowInInspector, HideReferenceObjectPicker, InlineProperty, HideLabel, Indent(1)]
		public IaaUnitID MaxIaaUnitIDIos
		{
			get => _iaaSetting.MaxIos;
			set => _iaaSetting.MaxIos = value;
		}
		
		[ShowIf("@(MediationFlag.HasFlag(IAAMediationFlag.Max))")]
		[BoxGroup("AppLovin MAX Configuration")]
		[PropertySpace(10)]
		[Button("Open AppLovin Dashboard", ButtonSizes.Medium)]
		private void OpenMaxDashboard()
		{
			Application.OpenURL("https://dash.applovin.com/");
		}

		// IronSource
		[ShowIf("@(MediationFlag.HasFlag(IAAMediationFlag.IronSource))")]
		[BoxGroup("IronSource Configuration", CenterLabel = true)]
		[TabGroup("IronSource Configuration/Platform", "Android", SdfIconType.Robot)]
		[ShowInInspector, HideReferenceObjectPicker, InlineProperty, HideLabel, Indent(1)]
		public IaaUnitID IronIaaUnitIDAndroid
		{
			get => _iaaSetting.IronAndroid;
			set => _iaaSetting.IronAndroid = value;
		}

		[ShowIf("@(MediationFlag.HasFlag(IAAMediationFlag.IronSource))")]
		[TabGroup("IronSource Configuration/Platform", "iOS", SdfIconType.Apple)]
		[ShowInInspector, HideReferenceObjectPicker, InlineProperty, HideLabel, Indent(1)]
		public IaaUnitID IronIaaUnitIDIos
		{
			get => _iaaSetting.IronIos;
			set => _iaaSetting.IronIos = value;
		}

		// Google AdMob
		[ShowIf("@(MediationFlag.HasFlag(IAAMediationFlag.GMA))")]
		[BoxGroup("Google AdMob Configuration", CenterLabel = true)]
		[TabGroup("Google AdMob Configuration/Platform", "Android", SdfIconType.Robot)]
		[ShowInInspector, HideReferenceObjectPicker, InlineProperty, HideLabel, Indent(1)]
		public IaaGmaUnitID GmaIaaUnitIDAndroid
		{
			get => _iaaSetting.GmaAndroid;
			set => _iaaSetting.GmaAndroid = value;
		}

		[ShowIf("@(MediationFlag.HasFlag(IAAMediationFlag.GMA))")]
		[TabGroup("Google AdMob Configuration/Platform", "iOS", SdfIconType.Apple)]
		[ShowInInspector, HideReferenceObjectPicker, InlineProperty, HideLabel, Indent(1)]
		public IaaGmaUnitID GmaIaaUnitIDIos
		{
			get => _iaaSetting.GmaIos;
			set => _iaaSetting.GmaIos = value;
		}
		
		[ShowIf("@(MediationFlag.HasFlag(IAAMediationFlag.GMA))")]
		[BoxGroup("Google AdMob Configuration")]
		[PropertySpace(10)]
		[Button("Open AdMob Dashboard", ButtonSizes.Medium)]
		private void OpenAdMobDashboard()
		{
			Application.OpenURL("https://apps.admob.com/");
		}
		
		[PropertySpace(SpaceBefore = 10)]
		[HideLabel]
		[InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
		public AdServiceSettings GlobalAdSettings;
	}
}
