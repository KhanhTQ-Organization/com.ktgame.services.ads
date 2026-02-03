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
		}

		[PropertySpace(20)]
		[Title("Mediation Setting", Bold = true)]
		[LabelText("Select Mediation"), LabelWidth(150), ShowInInspector, EnumToggleButtons]
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
			}
		}

		[Indent(1)]
		[LabelText("Main: "), LabelWidth(150), ShowInInspector, EnumPaging]
		public IAAMediationFlag MainMediation
		{
			get => _iaaSetting.MainMediation;
			set => _iaaSetting.MainMediation = value;
		}

		[PropertySpace(0, 20)]
		[Indent(1)]
		[LabelText("Backfill: "), LabelWidth(150), ShowInInspector, EnumPaging]
		public IAAMediationFlag BackFillMediation
		{
			get => _iaaSetting.BackfillMediation;
			set => _iaaSetting.BackfillMediation = value;
		}

		[ShowIf("@(MediationFlag.HasFlag(IAAMediationFlag.Max))"), TabGroup("Platform", "Android", SdfIconType.Robot),
		 ShowInInspector, HideReferenceObjectPicker, BoxGroup("Platform/Android/Max Mediation"), InlineProperty,
		 HideLabel, Indent(1)]
		public IaaUnitID MaxIaaUnitIDAndroid
		{
			get => _iaaSetting.MaxAndroid;
			set
			{
				//if (!_iaaSetting.MaxAndroid.AppID.Equals(value.AppID))
				//	MaxMediationEditor.SetSdkKeySafely(value.AppID);

				_iaaSetting.MaxAndroid = value;
			}
		}

		[ShowIf("@(MediationFlag.HasFlag(IAAMediationFlag.Max))"), TabGroup("Platform", "Ios", SdfIconType.Apple),
		 ShowInInspector, HideReferenceObjectPicker, BoxGroup("Platform/Ios/Max Mediation"), InlineProperty, HideLabel,
		 Indent(1)]
		public IaaUnitID MaxIaaUnitIDIos
		{
			get => _iaaSetting.MaxIos;
			set
			{
				//if (!_iaaSetting.MaxIos.AppID.Equals(value.AppID))
				//	MaxMediationEditor.SetSdkKeySafely(value.AppID);
				_iaaSetting.MaxIos = value;
			}
		}

		[ShowIf("@(MediationFlag.HasFlag(IAAMediationFlag.IronSource))"),
		 TabGroup("Platform", "Android", SdfIconType.Robot), ShowInInspector, HideReferenceObjectPicker,
		 BoxGroup("Platform/Android/Ironsource Mediation"), InlineProperty, HideLabel, Indent(1)]
		public IaaUnitID IronIaaUnitIDAndroid
		{
			get => _iaaSetting.IronAndroid;
			set => _iaaSetting.IronAndroid = value;
		}

		[ShowIf("@(MediationFlag.HasFlag(IAAMediationFlag.IronSource))"),
		 TabGroup("Platform", "Ios", SdfIconType.Robot), ShowInInspector, HideReferenceObjectPicker,
		 BoxGroup("Platform/Ios/Ironsource Mediation"), InlineProperty, HideLabel, Indent(1)]
		public IaaUnitID IronIaaUnitIDIos
		{
			get => _iaaSetting.IronIos;
			set => _iaaSetting.IronIos = value;
		}

		[ShowIf("@(MediationFlag.HasFlag(IAAMediationFlag.GMA))"), TabGroup("Platform", "Android", SdfIconType.Robot),
		 ShowInInspector, HideReferenceObjectPicker, BoxGroup("Platform/Android/Google Ad Mediation"), InlineProperty,
		 HideLabel, Indent(1)]
		public IaaGmaUnitID GmaIaaUnitIDAndroid
		{
			get => _iaaSetting.GmaAndroid;
			set
			{
				//if (!_iaaSetting.GmaAndroid.AppID.Equals(value.AppID))
				//{
				//	MaxMediationEditor.SetAdMobAppId(true, value.AppID);
				//	GMAEditor.SetAdMobAppId(true, value.AppID);
				//}

				_iaaSetting.GmaAndroid = value;
			}
		}

		[ShowIf("@(MediationFlag.HasFlag(IAAMediationFlag.GMA))"), TabGroup("Platform", "Ios", SdfIconType.Apple),
		 ShowInInspector, HideReferenceObjectPicker, BoxGroup("Platform/Ios/Google Ad Mediation"), InlineProperty,
		 HideLabel, Indent(1)]
		public IaaGmaUnitID GmaIaaUnitIDIos
		{
			get => _iaaSetting.GmaIos;
			set
			{
				// if (!_iaaSetting.GmaIos.AppID.Equals(value.AppID))
				// {
				// 	MaxMediationEditor.SetAdMobAppId(true, value.AppID);
				// 	GMAEditor.SetAdMobAppId(true, value.AppID);
				// }

				_iaaSetting.GmaIos = value;
			}
		}
	}
}
