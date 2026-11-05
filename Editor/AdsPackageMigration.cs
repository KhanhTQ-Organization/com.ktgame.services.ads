using System;
using System.IO;
using System.Linq;
using com.ktgame.core;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace com.ktgame.services.ads.editor
{
	[InitializeOnLoad]
	internal static class AdsPackageMigration
	{
		private const int CurrentVersion = 1;
		private const string LogTag = "[AdsMigration]";
		private const string MarkerPath = "ProjectSettings/com.ktgame.services.ads.migration.txt";

		private const string MaxDefine = "MAX_APPLOVIN";
		private static readonly string[] LegacyMaxDefines = { "MAX", "APPLOVIN", "APPLOVIN_MAX", "MAX_INSTALLED", "MAX_SDK" };

		private const int OldMaxRetry = 3;
		private const int NewMaxRetry = 6;

		private static readonly NamedBuildTarget[] Targets =
		{
			NamedBuildTarget.Android,
			NamedBuildTarget.iOS,
			NamedBuildTarget.Standalone,
		};

		static AdsPackageMigration()
		{
			EditorApplication.delayCall += TryRun;
		}

		[MenuItem("Ktgame/Services/Ads/Run Package Migration")]
		private static void RunFromMenu()
		{
			Run(force: true);
		}

		private static void TryRun()
		{
			if (EditorApplication.isCompiling || EditorApplication.isUpdating)
			{
				EditorApplication.delayCall += TryRun;
				return;
			}

			if (Application.isBatchMode && Environment.GetEnvironmentVariable("KTGAME_SKIP_ADS_MIGRATION") == "1")
			{
				return;
			}

			Run(force: false);
		}

		private static void Run(bool force)
		{
			if (!force && ReadVersion() >= CurrentVersion)
			{
				return;
			}

			try
			{
				var changes = 0;
				changes += MigrateMaxDefine();
				changes += MigrateGmaBanner();
				changes += MigrateMaxRetry();

				WriteVersion(CurrentVersion);
				Debug.Log(changes > 0
					? $"{LogTag} Upgraded ads package data to v{CurrentVersion} ({changes} change(s))."
					: $"{LogTag} Nothing to migrate (v{CurrentVersion}).");
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		

		private static int MigrateMaxDefine()
		{
			var changes = 0;
			foreach (var target in Targets)
			{
				string raw;
				try
				{
					raw = PlayerSettings.GetScriptingDefineSymbols(target);
				}
				catch (Exception)
				{
					continue;
				}

				var defines = raw.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(d => d.Trim())
					.Where(d => d.Length > 0)
					.ToList();

				var legacy = defines.Where(d => LegacyMaxDefines.Contains(d)).ToList();
				if (legacy.Count == 0)
				{
					continue;
				}

				defines.RemoveAll(d => legacy.Contains(d));
				if (!defines.Contains(MaxDefine))
				{
					defines.Add(MaxDefine);
				}

				PlayerSettings.SetScriptingDefineSymbols(target, string.Join(";", defines));
				Debug.Log($"{LogTag} {target.TargetName}: replaced define [{string.Join(", ", legacy)}] with {MaxDefine}.");
				changes++;
			}

			return changes;
		}

		private static int MigrateGmaBanner()
		{
			var changes = 0;
			foreach (var setting in LoadAll<IAASettingSO>())
			{
				var changed = MigrateGmaUnit(setting.GmaAndroid);
				changed |= MigrateGmaUnit(setting.GmaIos);
				if (!changed)
				{
					continue;
				}

				EditorUtility.SetDirty(setting);
				Debug.Log($"{LogTag} {AssetDatabase.GetAssetPath(setting)}: GMA Banner -> BannerCollapsible.", setting);
				changes++;
			}

			if (changes > 0)
			{
				AssetDatabase.SaveAssets();
			}

			return changes;
		}

		private static bool MigrateGmaUnit(IaaGmaUnitID unit)
		{
			if (unit == null || (unit.IAAFormats & IAAFormatType.Banner) == 0)
			{
				return false;
			}

			unit.IAAFormats = (unit.IAAFormats & ~IAAFormatType.Banner) | IAAFormatType.BannerCollapsible;
			if (string.IsNullOrWhiteSpace(unit.BannerCollapsibleUnitID) && !string.IsNullOrWhiteSpace(unit.BannerUnitID))
			{
				unit.BannerCollapsibleUnitID = unit.BannerUnitID;
			}

			return true;
		}
		
		private static int MigrateMaxRetry()
		{
			var changes = 0;
			foreach (var setting in LoadAll<AdServiceSettings>())
			{
				var so = new SerializedObject(setting);
				var prop = so.FindProperty("_maxRetryAttemptRequest");
				if (prop == null || prop.intValue != OldMaxRetry)
				{
					continue;
				}

				prop.intValue = NewMaxRetry;
				so.ApplyModifiedPropertiesWithoutUndo();
				EditorUtility.SetDirty(setting);
				Debug.Log($"{LogTag} {AssetDatabase.GetAssetPath(setting)}: MaxRetryAttemptRequest {OldMaxRetry} -> {NewMaxRetry}.", setting);
				changes++;
			}

			if (changes > 0)
			{
				AssetDatabase.SaveAssets();
			}

			return changes;
		}
		
		private static T[] LoadAll<T>() where T : ScriptableObject
		{
			return AssetDatabase.FindAssets("t:" + typeof(T).Name)
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<T>)
				.Where(a => a != null)
				.ToArray();
		}

		private static int ReadVersion()
		{
			try
			{
				return File.Exists(MarkerPath) && int.TryParse(File.ReadAllText(MarkerPath).Trim(), out var v) ? v : 0;
			}
			catch (Exception)
			{
				return 0;
			}
		}

		private static void WriteVersion(int version)
		{
			File.WriteAllText(MarkerPath, version.ToString());
		}
	}
}
