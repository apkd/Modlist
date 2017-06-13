using System;
using Verse;

namespace ModlistMod
{
	/// <summary> This code is ran once while the mod is being loaded. </summary>
	public class Init : Def
	{
		/// <summary> Entry method. </summary>
		static Init()
		{
			try
			{
				Log.Message($"Saving the list of active mods to the RimWorld install dir.");
				Modlist.SaveModlist();
			}
			catch (Exception e)
			{
				Log.Warning("Couldn't save the modlist: " + e.Message);
			}
		}
	}
}
