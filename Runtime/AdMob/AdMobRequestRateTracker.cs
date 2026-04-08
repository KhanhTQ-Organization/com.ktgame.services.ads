using System;
using System.Collections.Generic;
using com.ktgame.ads.core;
using UnityEngine;

namespace com.ktgame.ads.admob
{
	public static class AdMobRequestRateTracker
	{
		private sealed class AdRateStats
		{
			public int RequestCount;
			public int MatchCount;
			public int FillCount;
			public int FailCount;
		}

		private static readonly Dictionary<string, AdRateStats> _statsMap = new Dictionary<string, AdRateStats>();
		private static readonly object _lock = new object();

		public static void TrackRequest(string unitId, AdFormat adFormat)
		{
			UpdateStats(unitId, adFormat, stats => stats.RequestCount++, "request");
		}

		public static void TrackMatched(string unitId, AdFormat adFormat)
		{
			UpdateStats(unitId, adFormat, stats => stats.MatchCount++, "matched");
		}

		public static void TrackFilled(string unitId, AdFormat adFormat)
		{
			UpdateStats(unitId, adFormat, stats => stats.FillCount++, "filled");
		}

		public static void TrackFailed(string unitId, AdFormat adFormat)
		{
			UpdateStats(unitId, adFormat, stats => stats.FailCount++, "failed");
		}

		private static void UpdateStats(string unitId, AdFormat adFormat, Action<AdRateStats> updateAction, string reason)
		{
			lock (_lock)
			{
				var key = BuildKey(unitId, adFormat);
				if (!_statsMap.TryGetValue(key, out var stats))
				{
					stats = new AdRateStats();
					_statsMap[key] = stats;
				}

				updateAction(stats);
				LogStats(unitId, adFormat, stats, reason);
			}
		}

		private static string BuildKey(string unitId, AdFormat adFormat)
		{
			return $"{adFormat}:{unitId}";
		}

		private static void LogStats(string unitId, AdFormat adFormat, AdRateStats stats, string reason)
		{
			var matchRate = stats.RequestCount > 0 ? (double)stats.MatchCount / stats.RequestCount * 100d : 0d;
			var fillRate = stats.RequestCount > 0 ? (double)stats.FillCount / stats.RequestCount * 100d : 0d;
			var showRate = stats.MatchCount > 0 ? (double)stats.FillCount / stats.MatchCount * 100d : 0d;

			Debug.Log($"[AdMobRate] {adFormat} | unit: {unitId} | event: {reason} | request: {stats.RequestCount} | matched: {stats.MatchCount} | filled: {stats.FillCount} | failed: {stats.FailCount} | matchRate: {matchRate:F2}% | fillRate: {fillRate:F2}% | showRate: {showRate:F2}%");
		}
	}
}