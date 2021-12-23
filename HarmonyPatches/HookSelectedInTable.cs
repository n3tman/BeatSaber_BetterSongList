using BetterSongList.Util;
using HarmonyLib;

namespace BetterSongList.HarmonyPatches {
	[HarmonyPatch(typeof(LevelCollectionTableView), nameof(LevelCollectionTableView.HandleDidSelectRowEvent))]
	static class HookSelectedInTable {
		[HarmonyPriority(int.MinValue)]
		static void Postfix(IPreviewBeatmapLevel ____selectedPreviewBeatmapLevel)
		{
			var levelID = ____selectedPreviewBeatmapLevel?.levelID;
			Config.Instance.LastSong = levelID;
			PlaylistsUtil.SaveLastSelectedSong(Config.Instance.LastPack, levelID);

#if TRACE
			Plugin.Log.Warn(string.Format("LevelCollectionTableView.HandleDidSelectRowEvent(): LastSong: {0}", Config.Instance.LastSong));
#endif
		}
	}
}
