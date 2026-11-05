using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace com.ktgame.services.ads.editor
{
	public class BuildPreProcessor : IPreprocessBuildWithReport
	{
		public int callbackOrder => 1;

		private const string PackageName = "com.ktgame.services.ads";

		public void OnPreprocessBuild(BuildReport report)
		{
			var source = $"Packages/{PackageName}/Runtime/link.xml";
			var folder = $"Assets/Plugins/Ktgame/Settings/{PackageName}";
			var target = $"{folder}/link.xml";

			if (AssetDatabase.LoadAssetAtPath<TextAsset>(source) == null || AssetDatabase.LoadAssetAtPath<TextAsset>(target) != null)
			{
				return;
			}

			EnsureFolder(folder);
			if (!AssetDatabase.CopyAsset(source, target))
			{
				Debug.LogWarning($"[Ads] Could not copy {source} to {target}.");
			}
		}

		private static void EnsureFolder(string path)
		{
			if (AssetDatabase.IsValidFolder(path))
			{
				return;
			}

			var parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
			if (!string.IsNullOrEmpty(parent))
			{
				EnsureFolder(parent);
			}

			AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
		}
	}
}
