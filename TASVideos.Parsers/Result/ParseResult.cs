﻿using System.Collections.Generic;

namespace TASVideos.MovieParsers.Result
{
	/// <summary>
	/// The standard implementation of <seealso cref="IParseResult"/>
	/// </summary>
	internal class ParseResult : IParseResult
	{
		public bool Success { get; internal set; } = true;
		public IEnumerable<string> Errors => ErrorList;
		public IEnumerable<ParseWarnings> Warnings => WarningList;

		public string FileExtension { get; internal set; }
		public RegionType Region { get; internal set; }
		public int Frames { get; internal set; }
		public string SystemCode { get; internal set; }
		public int RerecordCount { get; internal set; }
		public MovieStartType StartType { get; internal set; }
		public double? FrameRateOverride { get; internal set; }

		internal List<ParseWarnings> WarningList { get; set; } = new List<ParseWarnings>();
		internal List<string> ErrorList { get; set; } = new List<string>();
	}

	internal static class ParseResultExtensions
	{
		internal static void WarnNoRerecords(this ParseResult parseResult)
		{
			parseResult.WarningList.Add(ParseWarnings.MissingRerecordCount);
		}

		internal static void WarnNoFrameRate(this ParseResult parseResult)
		{
			parseResult.WarningList.Add(ParseWarnings.FrameRateInferred);
		}
	}
}
