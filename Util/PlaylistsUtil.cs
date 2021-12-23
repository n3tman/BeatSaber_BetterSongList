using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BetterSongList.Util {
	public static class PlaylistsUtil {
		public static bool hasPlaylistLib = false;
		public static bool requiresListCast = false;

		public static void Init() {
			var x = IPA.Loader.PluginManager.GetPluginFromId("BeatSaberPlaylistsLib");

			hasPlaylistLib = x != null;
			requiresListCast = hasPlaylistLib && x.HVersion <= new Hive.Versioning.Version("1.4.0");
		}

		public static Dictionary<string, IBeatmapLevelPack> builtinPacks = null;

		public static IBeatmapLevelPack GetPack(string packName) {
			if(packName == null)
				return null;

			if(packName == "Custom Levels") {
				return SongCore.Loader.CustomLevelsPack;
			} else if(packName == "WIP Levels") {
				return SongCore.Loader.WIPLevelsPack;
			}

			if(builtinPacks == null) {
				var p = Resources.FindObjectsOfTypeAll<BeatmapLevelsModel>().First(x => x.ostAndExtrasPackCollection != null);

				builtinPacks =
					p.allLoadedBeatmapLevelWithoutCustomLevelPackCollection.beatmapLevelPacks
					// There shouldnt be any duplicate name basegame playlists... But better be safe
					.GroupBy(x => x.shortPackName)
					.Select(x => x.First())
					.ToDictionary(x => x.shortPackName, x => x);
			}

			if(builtinPacks.ContainsKey(packName)) {
				return builtinPacks[packName];
			} else if(hasPlaylistLib) {
				IBeatmapLevelPack wrapper() {
					foreach(var x in BeatSaberPlaylistsLib.PlaylistManager.DefaultManager.GetAllPlaylists()) {
						if(x.packName == packName)
							return x;
					}
					return null;
				}
				return wrapper();
			}
			return null;
		}

		public static bool IsCollection(IAnnotatedBeatmapLevelCollection levelCollection) {
			return levelCollection is BeatSaberPlaylistsLib.Legacy.LegacyPlaylist || levelCollection is BeatSaberPlaylistsLib.Blist.BlistPlaylist;
		}

		public static IPreviewBeatmapLevel[] GetLevelsForLevelCollection(IAnnotatedBeatmapLevelCollection levelCollection) {
			if(levelCollection is BeatSaberPlaylistsLib.Legacy.LegacyPlaylist legacyPlaylist)
				return legacyPlaylist.BeatmapLevels;
			if(levelCollection is BeatSaberPlaylistsLib.Blist.BlistPlaylist blistPlaylist)
				return blistPlaylist.BeatmapLevels;
			return null;
		}

		public static void SaveLastSelectedSong(string pack, string song)
		{
			var lastSelected = Config.Instance.LastSelectedSongs;
			var found = false;

			if (lastSelected.Count > 0)
			{
				foreach (var collection in lastSelected.Where(collection => collection.pack == pack))
				{
					collection.song = song;
					found = true;
					break;
				}
			}

			if (lastSelected.Count == 0 || !found)
			{
				Config.Instance.LastSelectedSongs.Add(new Config.Collection
				{
					pack = pack, song = song
				});
			}
		}
		
		public static string FindLastSelectedSong(string pack)
		{
			var lastSelected = Config.Instance.LastSelectedSongs;

			return pack == null || lastSelected.Count == 0 ? null : lastSelected.Where(collection => collection.pack == pack).Select(collection => collection.song).FirstOrDefault();
		}
	}
}
