﻿using HarmonyLib;
using HMUI;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BetterSongList.Util;

namespace BetterSongList.HarmonyPatches {
	[HarmonyPatch]
	static class RestoreTableScroll {
		static IEnumerable<MethodBase> TargetMethods() => AccessTools.GetDeclaredMethods(typeof(LevelCollectionTableView)).Where(x => x.Name == "Init");

		static int? scrollToIndex = null;
		static bool doResetScrollOnNext = false;

		public static void ResetScroll() {
			scrollToIndex = 0;
			doResetScrollOnNext = true;

#if TRACE
			Plugin.Log.Warn("RestoreTableScroll.ResetScroll()");
#endif
		}

		[HarmonyPriority(int.MaxValue)]
		static void Prefix(bool ____isInitialized, TableView ____tableView, IPreviewBeatmapLevel[] ____previewBeatmapLevels) {
			if(____isInitialized && ____tableView != null && ____previewBeatmapLevels != null && !doResetScrollOnNext)
				scrollToIndex = ____tableView.GetVisibleCellsIdRange().Item1;

			doResetScrollOnNext = false;

#if TRACE
			Plugin.Log.Warn(string.Format("LevelCollectionTableView.Init():Prefix - scrollToIndex: {0}", scrollToIndex));
#endif
		}

		[HarmonyPatch(typeof(LevelCollectionTableView), nameof(LevelCollectionTableView.SetData))]
		static class DoTheFunnySelect {
			[HarmonyPriority(int.MinValue)]
			static void Postfix(TableView ____tableView, IPreviewBeatmapLevel[] ____previewBeatmapLevels, bool ____showLevelPackHeader) {
#if TRACE
				Plugin.Log.Warn(string.Format("DoTheFunnySelect -> LevelCollectionTableView.SetData():Postfix scrollToIndex: {0}", scrollToIndex));
#endif
				bool specificMap = false;

				var lastSongInPack = PlaylistsUtil.FindLastSelectedSong(Config.Instance.LastPack);

				if(lastSongInPack != null) {
					for(int i = 0; i < (____previewBeatmapLevels?.Length ?? 0); i++) {
						if(____previewBeatmapLevels[i].levelID == lastSongInPack) {
							scrollToIndex = i;
							specificMap = true;

							if(____showLevelPackHeader)
								scrollToIndex++;

							break;
						}
					}
				}

				if(scrollToIndex == null || scrollToIndex < 0)
					return;

#if TRACE
				Plugin.Log.Warn(string.Format("-> Scrolling to {0} (Specific map: {1})", scrollToIndex, specificMap));
#endif

				if(specificMap || scrollToIndex == 0)
				{
					____tableView.ScrollToCellWithIdx(
						(int)scrollToIndex,
						specificMap ? TableView.ScrollPositionType.Center : TableView.ScrollPositionType.Beginning,
						specificMap
					);

					____tableView.SelectCellWithIdx((int)scrollToIndex, false);
				}
			}
		}
	}
}
