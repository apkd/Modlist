using System;
using System.Linq;
using System.IO;
using Verse;
using System.Collections.Generic;

namespace ModlistMod
{
	/// <summary> This code is ran once while the mod is being loaded. </summary>
	public class Init : Def
	{
		const string steam_workshop_uri = "http://steamcommunity.com/sharedfiles/filedetails/?id=";
		const string html_head =
@"<!doctype html>
<html lang=""en"">
<head>
<meta charset=""UTF-8"">
<title>RimWorld modlist</title>
<link rel=""stylesheet"" href=""https://cdn.rawgit.com/markdowncss/modest/master/css/modest.css"">
</head>
<body>";
		const string html_tail =
@"</body></html>";


		/// <summary> Entry method. </summary>
		static Init()
		{
			try
			{
				Log.Message($"Saving the list of active mods to the RimWorld install dir.");

				var mods = ModsConfig.ActiveModsInLoadOrder
					.Where(m => m.Active) // only active mods
					.Where(m => !m.IsCoreMod) // ignore the Core mod
					.Where(m => m.Name != "Modlist") // ignore self
					.GroupBy(x => x.Name).Select(m => m.First()); // select distinct by name

				var txt = GenerateTxt(mods);
				var markdown = GenerateMarkdown(mods);
				var html = GenerateHtml(markdown);

				WriteTextFile("modlist.txt", txt);
				//WriteTextFile("modlist.md", markdown);
				WriteTextFile("modlist.html", html);
			}
			catch (Exception e)
			{
				Log.Warning("Couldn't save the modlist: " + e.Message);
			}
		}

		/// <summary> Generates the mod list in a simple text format. </summary>
		static string GenerateTxt(IEnumerable<ModMetaData> mods)
		{
			var text = new System.Text.StringBuilder();

			foreach (var mod in mods)
			{
				text.AppendLine($"{mod.Name} | {GetModUri(mod)}");
			}

			return text.ToString();
		}

		/// <summary> Generates a mod list in a markdown format (needs autonewlines option). </summary>
		static string GenerateMarkdown(IEnumerable<ModMetaData> mods)
		{
			var text = new System.Text.StringBuilder();
			text.AppendLine($"# RimWorld modlist");
			text.AppendLine($"Author: **{SteamUtility.SteamPersonaName}**");
			text.AppendLine($"Mod count: **{mods.Count()}**");
			text.AppendLine();

			foreach (var mod in mods)
			{
				string desc = System.Text.RegularExpressions.Regex.Replace(mod.Description, @"\n\s*", "\n");

				text.AppendLine($"##### [{mod.Name}]({GetModUri(mod)}) by {mod.Author}");

				if (!mod.VersionCompatible)
					text.AppendLine($"*Mod intended for version {mod.TargetVersion}*\n");

				text.AppendLine($"{desc}");
				text.AppendLine();
			}

			return text.ToString();
		}

		/// <summary> Transforms markdown to html. </summary>
		static string GenerateHtml(string markdown)
		{
			var parser = new MarkdownSharp.Markdown(new MarkdownSharp.MarkdownOptions() { AutoNewLines = true });
			string result = parser.Transform(markdown);
			return html_head + result + html_tail;
		}

		static string GetModUri(ModMetaData mod) => mod.OnSteamWorkshop ?
					steam_workshop_uri + mod.GetPublishedFileId().m_PublishedFileId : // use steam uri
					mod.Url; // use mod's own download link as a fallback

		static void WriteTextFile(string filename, string content)
		{
			var path = Path.Combine(Environment.CurrentDirectory, filename);
			if (File.Exists(path)) File.Delete(path);
			File.WriteAllText(path, content);
		}
	}
}
