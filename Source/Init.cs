using System;
using System.Linq;
using System.IO;
using Verse;

namespace ModlistMod
{
	public class Init : Def
	{
		public Init()
		{
			Log.Message($"Saving the list of installed mods to modlist.txt");

			var modlist = new System.Text.StringBuilder();

			var mods = ModLister.AllInstalledMods
				.Where(m => m.Active)
				.Where(m => !m.IsCoreMod)
				.Where(m => m.Name != "Modlist")
				.Distinct();

			foreach (var mod in mods)
				modlist.AppendLine(mod.Name);

			var path = Path.Combine(Environment.CurrentDirectory, "modlist.txt");
			File.WriteAllText(path, modlist.ToString());
		}
	}
}
