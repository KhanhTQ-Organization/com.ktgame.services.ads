using com.ktgame.core;
using UnityEditor;
using com.ktgame.core.editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace com.ktgame.services.ads.editor
{
	[InitializeOnLoad]
	public class IAAEditorModule : IEditorDirtyHandler,	IMenuTreeExtension
	{
		static IAAEditorModule()
		{
			var module = new IAAEditorModule();
			EditorDirtyRegistry.Register(module);
			MenuTreeExtensionRegistry.Register(module);
		}

		public void SetDirty()
		{
			var instance = IAASettingSO.Instance;
			if (instance != null)
			{
				EditorUtility.SetDirty(instance);
			}
			
			var instanceRevenue = RevenueAdSetting.Instance;
			if (instanceRevenue != null)
			{
				EditorUtility.SetDirty(instance);
			}
		}

		public void BuildMenu(OdinMenuTree tree)
		{
			tree.Add("In App Ads", new IAAEditor(KTWindow.Setting, IAASettingSO.Instance), SdfIconType.BadgeAdFill);
			tree.Add("Analytics SDK", new RevenueEditor(KTWindow.Setting, RevenueAdSetting.Instance), KTEditor.GetIconComponent("analytic"));
		}
	}
}