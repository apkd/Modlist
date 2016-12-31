using System;
using System.Linq;
using System.IO;
using Verse;

namespace ModlistMod
{
	/// <summary> This code is ran once while the mod is being loaded. </summary>
	public class Init : Def
	{
		const string steam_workshop_uri = "http://steamcommunity.com/sharedfiles/filedetails/?id=";

		static void SaveModlist(string filename)
		{
			Log.Message($"Saving the list of installed mods to {filename}");

			var modlist = new System.Text.StringBuilder();

			var mods = ModLister.AllInstalledMods
				.Where(m => m.Active) // only active mods
				.Where(m => !m.IsCoreMod) // ignore the Core mod
				.Where(m => m.Name != "Modlist") // ignore self
				.GroupBy(x => x.Name).Select(m => m.First()) // select distinct by name
				.OrderBy(x => x.Name); // sort

			foreach (var mod in mods)
			{
				string link = mod.OnSteamWorkshop ?
					steam_workshop_uri + mod.GetPublishedFileId().m_PublishedFileId : // use steam uri
					mod.Url; // use mod's own download link as a fallback

				modlist.AppendLine($"{mod.Name} | {link}");
			}

			var path = Path.Combine(Environment.CurrentDirectory, "modlist.txt");
			if (File.Exists(path)) File.Delete(path);
			File.WriteAllText(path, modlist.ToString());
		}

		static Init()
		{
			try
			{
				SaveModlist("modlist.txt");
			}
			catch (Exception)
			{
				Log.Warning("Couldn't save the modlist.");
			}
		}
	}
}
