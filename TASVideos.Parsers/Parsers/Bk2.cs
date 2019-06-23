﻿using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using TASVideos.MovieParsers.Extensions;
using TASVideos.MovieParsers.Result;

namespace TASVideos.MovieParsers.Parsers
{
	[FileExtension("bk2")]
	internal class Bk2 : ParserBase, IParser
	{
		private const string HeaderFile = "header";
		private const string InputFile = "input log";

		public override string FileExtension => "bk2";

		public IParseResult Parse(Stream file)
		{
			var result = new ParseResult
			{
				Region = RegionType.Ntsc,
				FileExtension = FileExtension
			};

			var archive = new ZipArchive(file);

			var headerEntry = archive.Entry(HeaderFile);
			if (headerEntry == null)
			{
				return Error($"Missing {HeaderFile}, can not parse");
			}

			int? vBlankCount;
			string core;

			using (var stream = headerEntry.Open())
			{
				using (var reader = new StreamReader(stream))
				{
					var header = reader
						.ReadToEnd()
						.LineSplit();

					string platform = header.GetValueFor(Keys.Platform);
					if (string.IsNullOrWhiteSpace(platform))
					{
						return Error("Could not determine the System Code");
					}

					int? rerecordVal = header.GetValueFor(Keys.RerecordCount).ToInt();
					if (rerecordVal.HasValue)
					{
						result.RerecordCount = rerecordVal.Value;
					}
					else
					{
						result.WarnNoRerecords();
					}

					// Some biz system ids do not match tasvideos, convert if needed
					if (BizToTasvideosSystemIds.ContainsKey(platform))
					{
						platform = BizToTasvideosSystemIds[platform];
					}

					// Check various subsystem flags
					if (header.GetValueFor(Keys.Mode32X).ToBool())
					{
						platform = SystemCodes.X32;
					}
					else if (header.GetValueFor(Keys.ModeCgb).ToBool())
					{
						platform = SystemCodes.Gbc;
					}
					else if (header.GetValueFor(Keys.Board) == SystemCodes.Fds)
					{
						platform = SystemCodes.Fds;
					}
					else if (header.GetValueFor(Keys.ModeSegaCd).ToBool())
					{
						platform = SystemCodes.SegaCd;
					}
					else if (header.GetValueFor(Keys.ModeGg).ToBool())
					{
						platform = SystemCodes.Gg;
					}
					else if (header.GetValueFor(Keys.ModeSg).ToBool())
					{
						platform = SystemCodes.Sg;
					}

					result.SystemCode = platform;

					if (header.GetValueFor(Keys.Pal).ToBool())
					{
						result.Region = RegionType.Pal;
					}

					if (header.GetValueFor(Keys.StartsFromSavestate).ToBool())
					{
						result.StartType = MovieStartType.Savestate;
					}
					else if (header.GetValueFor(Keys.StartsFromSram).ToBool())
					{
						result.StartType = MovieStartType.Sram;
					}

					vBlankCount = header.GetValueFor(Keys.VBlankCount).ToInt();
					core = header.GetValueFor(Keys.Core);
				}
			}

			var inputLog = archive.Entry(InputFile);
			if (inputLog == null)
			{
				return Error($"Missing {InputFile}, can not parse");
			}

			using (var stream = inputLog.Open())
			{
				using (var reader = new StreamReader(stream))
				{
					result.Frames = reader
						.ReadToEnd()
						.LineSplit()
						.PipeCount();
				}
			}

			if (core?.ToLower() == "subneshawk")
			{
				if (!vBlankCount.HasValue)
				{
					return Error($"Missing {Keys.VBlankCount}, could not parse movie time");
				}

				result.Frames = vBlankCount.Value;
			}

			return result;
		}

		private static readonly Dictionary<string, string> BizToTasvideosSystemIds = new Dictionary<string, string>
		{
			["gen"] = SystemCodes.Genesis,
			["sat"] = SystemCodes.Saturn,
			["dgb"] = SystemCodes.GameBoy,
			["a26"] = SystemCodes.Atari2600,
			["a78"] = SystemCodes.Atari7800,
			["uze"] = SystemCodes.UzeBox,
			["vb"] = SystemCodes.VirtualBoy,
			["zxspectrum"] = SystemCodes.ZxSpectrum
		};

		private static class Keys
		{
			public const string RerecordCount = "rerecordcount";
			public const string Platform = "platform";
			public const string Board = "boardname";
			public const string Pal = "pal";
			public const string StartsFromSram = "startsfromsaveram";
			public const string StartsFromSavestate = "startsfromsavestate";
			public const string Mode32X = "is32x";
			public const string ModeCgb = "iscgbmode";
			public const string ModeSegaCd = "issegacdmode";
			public const string ModeGg = "isggmode";
			public const string ModeSg = "issgmode";
			public const string VBlankCount = "vblankcount";
			public const string Core = "core";
		}
	}
}
