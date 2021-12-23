﻿using System.Collections.Generic;
using IPA.Config.Stores;
using System.Runtime.CompilerServices;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BetterSongList {
	internal class Config {
		public static Config Instance { get; set; }
		public string SettingsSeenInVersion { get; set; } = "";
		
		public bool SortAsc { get; set; } = false;
		public string LastSort { get; set; } = "Default";
		//public virtual bool InvertFilter { get; set; } = false;
		public string LastFilter { get; set; } = "";

		public string LastSong { get; set; } = "";
		public string LastCategory { get; set; } = "All";
		public string LastPack { get; set; } = null;

		[UseConverter(typeof(ListConverter<Collection>))]
		public virtual List<Collection> LastSelectedSongs { get; set; } = new List<Collection>();
		public class Collection
		{
			[NonNullable]
			public virtual string pack { get; set; } = string.Empty;
			[NonNullable]
			public virtual string song { get; set; } = string.Empty;
		}

		public bool AllowWipDelete { get; set; } = false;
		public bool ReselectLastSong { get; set; } = true;
		public bool AutoFilterUnowned { get; set; } = true;
		public bool EnableAlphabetScrollbar { get; set; } = true;
		public bool ClearFiltersOnPlaylistSelect { get; set; } = true;
		public bool ModBasegameSearch { get; set; } = true;
		public bool ShowMapJDInsteadOfOffset { get; set; } = true;
		public bool ExtendSongsScrollbar { get; set; } = true;
		public float AccuracyMultiplier { get; set; } = 1f;

		/// <summary>
		/// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
		/// </summary>
		public virtual void OnReload() {

		}

		/// <summary>
		/// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
		/// </summary>
		public virtual void Changed() {
			// Do stuff when the config is changed.
		}

		/// <summary>
		/// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
		/// </summary>
		public virtual void CopyFrom(Config other) {
			// This instance's members populated from other
		}
	}
}
