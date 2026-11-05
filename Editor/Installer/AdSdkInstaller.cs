using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using com.ktgame.core.editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

namespace com.ktgame.services.ads.editor
{
	public sealed class UpmAdSdk
	{
		public string Id;
		public string DisplayName;
		public string PackageName;
		public string RegistryName;
		public string RegistryUrl;
		public string[] RegistryScopes;
		public string DefineSymbol;
		public string DefaultVersion;
		public string LegacyAssetsFolder;
		public string LegacyHint;
		public string SettingsMenuPath;
	}
	
	[InitializeOnLoad]
	public static class AdSdkInstaller
	{
		private const string LogTag = "[Ad SDK Installer]";
		private const string GoogleEdm = "com.google.external-dependency-manager";
		private const string UnityEdm = "com.unity.external-dependency-manager";
		private static readonly Regex VersionRegex = new Regex(@"^\d+\.\d+\.\d+$");

		public static readonly UpmAdSdk Max = new UpmAdSdk
		{
			Id = "max",
			DisplayName = "AppLovin MAX",
			PackageName = "com.applovin.mediation.ads",
			RegistryName = "AppLovin MAX Unity",
			RegistryUrl = "https://unity.packages.applovin.com/",
			RegistryScopes = new[] { "com.applovin.mediation.ads", "com.applovin.mediation.adapters", "com.applovin.mediation.dsp" },
			DefineSymbol = DefineSymbolName.DS_MAX_INSTALLED,
			DefaultVersion = "8.6.6",
			LegacyAssetsFolder = "MaxSdk",
			LegacyHint = "Use AppLovin > Integration Manager > 'Migrate to UPM' first.",
			SettingsMenuPath = "AppLovin/Integration Manager"
		};

		public static readonly UpmAdSdk Gma = new UpmAdSdk
		{
			Id = "gma",
			DisplayName = "Google Mobile Ads",
			PackageName = "com.google.ads.mobile",
			RegistryName = "package.openupm.com",
			RegistryUrl = "https://package.openupm.com",
			RegistryScopes = new[] { "com.google.ads.mobile" },
			DefineSymbol = "ADMOB",
			DefaultVersion = "11.5.0",
			LegacyAssetsFolder = "GoogleMobileAds",
			LegacyHint = "Delete Assets/GoogleMobileAds (and its ExternalDependencyManager copy) before installing via UPM.",
			SettingsMenuPath = "Assets/Google Mobile Ads/Settings..."
		};

		public static bool IsBusy { get; private set; }

		private static string ManifestPath => Path.GetFullPath(Path.Combine(Application.dataPath, "../Packages/manifest.json"));

		static AdSdkInstaller()
		{
			EditorApplication.delayCall += () =>
			{
				foreach (var sdk in new[] { Max, Gma })
				{
					var key = PendingKey(sdk);
					if (!SessionState.GetBool(key, false))
					{
						continue;
					}

					SessionState.EraseBool(key);
					if (IsInstalledViaUpm(sdk))
					{
						ApplyDefine(sdk, true);
						Debug.Log($"{LogTag} {sdk.PackageName} {GetInstalledUpmVersion(sdk)} installed, define {sdk.DefineSymbol} applied.");
					}
				}
			};
		}

		#region State

		public static bool HasLegacyInstall(UpmAdSdk sdk) =>
			!string.IsNullOrEmpty(sdk.LegacyAssetsFolder) && Directory.Exists(Path.Combine(Application.dataPath, sdk.LegacyAssetsFolder));

		public static bool IsInstalledViaUpm(UpmAdSdk sdk) => GetInstalledUpmVersion(sdk) != null;

		public static bool IsInstalled(UpmAdSdk sdk) => IsInstalledViaUpm(sdk) || HasLegacyInstall(sdk);
		
		public static bool IsLegacyOnly(UpmAdSdk sdk) => HasLegacyInstall(sdk) && !IsInstalledViaUpm(sdk);

		public static bool HasDefine(UpmAdSdk sdk) => DefineSymbolsEditor.HasDefineSymbol(sdk.DefineSymbol);

		public static string GetInstalledUpmVersion(UpmAdSdk sdk)
		{
			if (!File.Exists(ManifestPath))
			{
				return null;
			}

			var match = Regex.Match(File.ReadAllText(ManifestPath), $"\"{Regex.Escape(sdk.PackageName)}\"\\s*:\\s*\"([^\"]+)\"");
			return match.Success ? match.Groups[1].Value : null;
		}

		public static string StatusText(UpmAdSdk sdk) =>
			$"Installed via UPM: {sdk.PackageName} {GetInstalledUpmVersion(sdk)}  |  define {sdk.DefineSymbol}: {(HasDefine(sdk) ? "ON" : "OFF")}";

		public static void OpenSettings(UpmAdSdk sdk)
		{
			if (string.IsNullOrEmpty(sdk.SettingsMenuPath) || !EditorApplication.ExecuteMenuItem(sdk.SettingsMenuPath))
			{
				Debug.LogWarning($"{LogTag} Menu '{sdk.SettingsMenuPath}' not found (wait for compilation to finish).");
			}
		}

		#endregion

		#region Install / Remove

		public static void Install(UpmAdSdk sdk, string version, Action<string> onInstalled = null)
		{
			if (IsBusy)
			{
				return;
			}

			version = string.IsNullOrWhiteSpace(version) ? sdk.DefaultVersion : version.Trim().TrimStart('v', 'V');
			if (!VersionRegex.IsMatch(version))
			{
				EditorUtility.DisplayDialog(sdk.DisplayName, $"'{version}' is not a valid version (expected e.g. {sdk.DefaultVersion}).", "OK");
				return;
			}

			if (IsLegacyOnly(sdk))
			{
				EditorUtility.DisplayDialog(sdk.DisplayName,
					$"{sdk.DisplayName} is already installed as a .unitypackage (Assets/{sdk.LegacyAssetsFolder}).\n\n" +
					$"Installing the UPM package on top would duplicate the SDK. {sdk.LegacyHint}",
					"OK");
				return;
			}

			EditorCoroutine.Start(InstallRoutine(sdk, version, onInstalled));
		}

		private static IEnumerator InstallRoutine(UpmAdSdk sdk, string version, Action<string> onInstalled)
		{
			IsBusy = true;
			try
			{
				string registryDoc = null;
				var fetch = FetchRegistryDoc(sdk, doc => registryDoc = doc);
				while (fetch.MoveNext())
				{
					EditorUtility.DisplayProgressBar(sdk.DisplayName, $"Checking {sdk.PackageName} {version}...", 0.2f);
					yield return fetch.Current;
				}

				EditorUtility.ClearProgressBar();

				if (registryDoc != null)
				{
					var versions = ParseVersions(sdk, registryDoc);
					if (!versions.ContainsKey(version))
					{
						EditorUtility.DisplayDialog(sdk.DisplayName,
							$"Version {version} does not exist on {sdk.RegistryUrl}.\nUse 'Latest' to pick a valid version.", "OK");
						yield break;
					}

					var conflict = GetEdmConflict(versions[version]);
					if (conflict != null && !EditorUtility.DisplayDialog($"{sdk.DisplayName} {version}", conflict + "\n\nInstall anyway?", "Install anyway", "Cancel"))
					{
						yield break;
					}
				}
				
				if (!EnsureScopedRegistry(sdk))
				{
					EditorUtility.DisplayDialog(sdk.DisplayName, "Could not add the scoped registry to Packages/manifest.json. See Console.", "OK");
					yield break;
				}

				EnsureExternalDependencyManager();

				Debug.Log($"{LogTag} Installing {sdk.PackageName}@{version} from {sdk.RegistryUrl} ...");
				SessionState.SetBool(PendingKey(sdk), true);

				AddRequest request = Client.Add($"{sdk.PackageName}@{version}");
				while (!request.IsCompleted)
				{
					EditorUtility.DisplayProgressBar(sdk.DisplayName, $"Installing {sdk.PackageName} {version} (Unity Package Manager)...", 0.6f);
					yield return null;
				}

				EditorUtility.ClearProgressBar();
				SessionState.EraseBool(PendingKey(sdk));

				if (request.Status == StatusCode.Success)
				{
					ApplyDefine(sdk, true);
					onInstalled?.Invoke(request.Result.version);
					Debug.Log($"{LogTag} Installed {sdk.PackageName} {request.Result.version}. Define {sdk.DefineSymbol} applied.");
				}
				else
				{
					var message = request.Error != null ? request.Error.message : "Unknown error";
					Debug.LogError($"{LogTag} Install {sdk.PackageName} failed: {message}");
					EditorUtility.DisplayDialog($"{sdk.DisplayName} Install Error", $"Could not install {sdk.PackageName} {version}.\n\n{message}", "OK");
				}
			}
			finally
			{
				EditorUtility.ClearProgressBar();
				IsBusy = false;
			}
		}

		public static void Remove(UpmAdSdk sdk)
		{
			if (IsBusy || !IsInstalledViaUpm(sdk))
			{
				return;
			}

			if (!EditorUtility.DisplayDialog($"Remove {sdk.DisplayName}",
				    $"This removes {sdk.PackageName} from Packages/manifest.json and the {sdk.DefineSymbol} define.\nContinue?",
				    "Remove", "Cancel"))
			{
				return;
			}

			EditorCoroutine.Start(RemoveRoutine(sdk));
		}

		private static IEnumerator RemoveRoutine(UpmAdSdk sdk)
		{
			IsBusy = true;
			try
			{
				ApplyDefine(sdk, false);

				RemoveRequest request = Client.Remove(sdk.PackageName);
				while (!request.IsCompleted)
				{
					EditorUtility.DisplayProgressBar(sdk.DisplayName, $"Removing {sdk.PackageName}...", 0.5f);
					yield return null;
				}

				EditorUtility.ClearProgressBar();
				if (request.Status == StatusCode.Success)
				{
					Debug.Log($"{LogTag} {sdk.PackageName} removed.");
				}
				else
				{
					Debug.LogError($"{LogTag} Remove {sdk.PackageName} failed: {request.Error?.message}");
				}
			}
			finally
			{
				EditorUtility.ClearProgressBar();
				IsBusy = false;
			}
		}

		#endregion

		#region Latest compatible version
		
		public static void FetchLatestCompatibleVersion(UpmAdSdk sdk, Action<string> onVersion)
		{
			if (IsBusy)
			{
				return;
			}

			EditorCoroutine.Start(FetchLatestRoutine(sdk, onVersion));
		}

		private static IEnumerator FetchLatestRoutine(UpmAdSdk sdk, Action<string> onVersion)
		{
			IsBusy = true;
			string registryDoc = null;
			var fetch = FetchRegistryDoc(sdk, doc => registryDoc = doc);
			while (fetch.MoveNext())
			{
				yield return fetch.Current;
			}

			IsBusy = false;

			if (registryDoc == null)
			{
				yield break;
			}

			var versions = ParseVersions(sdk, registryDoc);
			var ordered = versions.Keys.OrderByDescending(v => new Version(v)).ToList();
			var compatible = ordered.FirstOrDefault(v => GetEdmConflict(versions[v]) == null);

			if (compatible == null)
			{
				Debug.LogError($"{LogTag} No compatible {sdk.PackageName} version found.");
				yield break;
			}

			if (ordered.Count > 0 && ordered[0] != compatible)
			{
				Debug.LogWarning($"{LogTag} {sdk.PackageName} {ordered[0]} is the newest, but it needs a different External Dependency Manager " +
				                 $"than this project ({GoogleEdm}). Selected {compatible} instead.");
			}

			Debug.Log($"{LogTag} Latest compatible {sdk.PackageName}: {compatible}");
			onVersion?.Invoke(compatible);
		}

		private static IEnumerator FetchRegistryDoc(UpmAdSdk sdk, Action<string> onDoc)
		{
			var url = sdk.RegistryUrl.TrimEnd('/') + "/" + sdk.PackageName;
			using (var request = UnityWebRequest.Get(url))
			{
				request.timeout = 20;
				var operation = request.SendWebRequest();
				while (!operation.isDone)
				{
					yield return null;
				}

				if (request.result != UnityWebRequest.Result.Success)
				{
					Debug.LogError($"{LogTag} Could not query {url}: {request.error}");
					onDoc?.Invoke(null);
					yield break;
				}

				onDoc?.Invoke(request.downloadHandler.text);
			}
		}
		
		private static Dictionary<string, string> ParseVersions(UpmAdSdk sdk, string doc)
		{
			var result = new Dictionary<string, string>();
			var matches = Regex.Matches(doc,
				$"\"name\"\\s*:\\s*\"{Regex.Escape(sdk.PackageName)}\"\\s*,\\s*\"version\"\\s*:\\s*\"(\\d+\\.\\d+\\.\\d+)\"");

			foreach (Match match in matches)
			{
				var version = match.Groups[1].Value;
				if (result.ContainsKey(version))
				{
					continue;
				}

				var depIndex = doc.IndexOf("\"dependencies\"", match.Index, StringComparison.Ordinal);
				var deps = string.Empty;
				if (depIndex > 0)
				{
					var end = doc.IndexOf('}', depIndex);
					deps = end > depIndex ? doc.Substring(depIndex, end - depIndex + 1) : string.Empty;
				}

				result[version] = deps;
			}

			return result;
		}

		/// <summary>Null when compatible, otherwise a human readable reason.</summary>
		private static string GetEdmConflict(string dependenciesBlock)
		{
			if (string.IsNullOrEmpty(dependenciesBlock) || !File.Exists(ManifestPath))
			{
				return null;
			}

			var manifest = File.ReadAllText(ManifestPath);
			if (dependenciesBlock.Contains($"\"{UnityEdm}\"") && manifest.Contains($"\"{GoogleEdm}\""))
			{
				return $"This version depends on {UnityEdm}, but the project uses {GoogleEdm} (Firebase). " +
				       "Having both resolvers installed causes duplicate Android/iOS dependency resolution.";
			}

			if (dependenciesBlock.Contains($"\"{GoogleEdm}\"") && manifest.Contains($"\"{UnityEdm}\""))
			{
				return $"This version depends on {GoogleEdm}, but the project uses {UnityEdm}.";
			}

			return null;
		}

		#endregion

		#region Menu (works even when com.ktgame.services.ads runtime does not compile)

		[MenuItem("Ktgame/Services/Ads/AppLovin MAX/Install Latest (UPM)", priority = 100)]
		private static void MenuInstallMax() => InstallLatestFromMenu(Max);

		[MenuItem("Ktgame/Services/Ads/AppLovin MAX/Install Latest (UPM)", true)]
		private static bool MenuInstallMaxValidate() => !IsBusy && !IsLegacyOnly(Max);

		[MenuItem("Ktgame/Services/Ads/AppLovin MAX/Remove (UPM)", priority = 101)]
		private static void MenuRemoveMax() => Remove(Max);

		[MenuItem("Ktgame/Services/Ads/AppLovin MAX/Remove (UPM)", true)]
		private static bool MenuRemoveMaxValidate() => !IsBusy && IsInstalledViaUpm(Max);

		[MenuItem("Ktgame/Services/Ads/Google Mobile Ads/Install Latest Compatible (UPM)", priority = 110)]
		private static void MenuInstallGma() => InstallLatestFromMenu(Gma);

		[MenuItem("Ktgame/Services/Ads/Google Mobile Ads/Install Latest Compatible (UPM)", true)]
		private static bool MenuInstallGmaValidate() => !IsBusy && !IsLegacyOnly(Gma);

		[MenuItem("Ktgame/Services/Ads/Google Mobile Ads/Remove (UPM)", priority = 111)]
		private static void MenuRemoveGma() => Remove(Gma);

		[MenuItem("Ktgame/Services/Ads/Google Mobile Ads/Remove (UPM)", true)]
		private static bool MenuRemoveGmaValidate() => !IsBusy && IsInstalledViaUpm(Gma);

		private static void InstallLatestFromMenu(UpmAdSdk sdk)
		{
			FetchLatestCompatibleVersion(sdk, latest =>
			{
				if (EditorUtility.DisplayDialog(sdk.DisplayName, $"Install {sdk.PackageName} {latest}?", "Install", "Cancel"))
				{
					EditorApplication.delayCall += () => Install(sdk, latest);
				}
			});
		}

		#endregion

		#region Helpers

		private static string PendingKey(UpmAdSdk sdk) => $"ktgame.ads.{sdk.Id}.pendingDefine";

		private static void ApplyDefine(UpmAdSdk sdk, bool add)
		{
			foreach (var target in new[] { NamedBuildTarget.Android, NamedBuildTarget.iOS })
			{
				if (add)
				{
					DefineSymbolsEditor.AddDefineSymbol(sdk.DefineSymbol, target);
				}
				else
				{
					DefineSymbolsEditor.RemoveDefineSymbol(sdk.DefineSymbol, target);
				}
			}
		}

		private static void EnsureExternalDependencyManager()
		{
			if (PackageDependenceEditor.IsPackageInstalled(GoogleEdm) || PackageDependenceEditor.IsPackageInstalled(UnityEdm))
			{
				return;
			}

			var edmVersion = KTWindow.Setting != null ? KTWindow.Setting.EdmVersion : "1.2.185";
			Debug.Log($"{LogTag} External Dependency Manager not found, installing {GoogleEdm} {edmVersion}...");
			PackageDependenceEditor.InstallPackage(GoogleEdm, edmVersion);
		}
		
		private static bool EnsureScopedRegistry(UpmAdSdk sdk)
		{
			if (!File.Exists(ManifestPath))
			{
				Debug.LogError($"{LogTag} manifest.json not found at {ManifestPath}");
				return false;
			}

			var text = File.ReadAllText(ManifestPath);
			var urlNoSlash = sdk.RegistryUrl.TrimEnd('/');
			
			var existing = Regex.Match(text, "\\{[^{}]*\"url\"\\s*:\\s*\"" + Regex.Escape(urlNoSlash) + "/?\"[^{}]*\\}");
			if (existing.Success)
			{
				var block = existing.Value;
				var missing = sdk.RegistryScopes.Where(s => !block.Contains($"\"{s}\"")).ToList();
				if (missing.Count == 0)
				{
					return true;
				}

				var scopesOpen = Regex.Match(block, "\"scopes\"\\s*:\\s*\\[");
				if (!scopesOpen.Success)
				{
					Debug.LogError($"{LogTag} Registry {sdk.RegistryUrl} has no 'scopes' array; please edit manifest.json manually.");
					return false;
				}

				var newBlock = block.Insert(scopesOpen.Index + scopesOpen.Length, string.Join(", ", missing.Select(s => $"\"{s}\"")) + ", ");
				newBlock = Regex.Replace(newBlock, ",\\s*\\]", "]");
				File.WriteAllText(ManifestPath, text.Replace(block, newBlock));
				Debug.Log($"{LogTag} Added scopes [{string.Join(", ", missing)}] to registry {sdk.RegistryUrl}.");
				return true;
			}

			var scopes = string.Join(", ", sdk.RegistryScopes.Select(s => $"\"{s}\""));
			var entry = "{\n      \"name\": \"" + sdk.RegistryName + "\",\n      \"url\": \"" + sdk.RegistryUrl + "\",\n      \"scopes\": [" + scopes + "]\n    }";

			var empty = Regex.Match(text, "\"scopedRegistries\"\\s*:\\s*\\[\\s*\\]");
			var open = Regex.Match(text, "\"scopedRegistries\"\\s*:\\s*\\[");

			if (empty.Success)
			{
				text = text.Substring(0, empty.Index) + "\"scopedRegistries\": [\n    " + entry + "\n  ]" + text.Substring(empty.Index + empty.Length);
			}
			else if (open.Success)
			{
				text = text.Insert(open.Index + open.Length, "\n    " + entry + ",");
			}
			else
			{
				var lastBrace = text.LastIndexOf('}');
				if (lastBrace < 0)
				{
					Debug.LogError($"{LogTag} manifest.json looks invalid.");
					return false;
				}

				text = text.Substring(0, lastBrace).TrimEnd() + ",\n  \"scopedRegistries\": [\n    " + entry + "\n  ]\n}\n";
			}

			File.WriteAllText(ManifestPath, text);
			Debug.Log($"{LogTag} Added scoped registry '{sdk.RegistryName}' ({sdk.RegistryUrl}) to manifest.json.");
			return true;
		}

		#endregion
	}
}
