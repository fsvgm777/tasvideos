﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TASVideos.Models
{
	public class UserMovieListModel
	{
		public long Id { get; set; }

		[Display(Name = "By")]
		public string Author { get; set; }

		[Display(Name = "Uploaded")]
		public DateTime Uploaded { get; set; }

		[Display(Name = "Filename")]
		public string FileName { get; set; }

		[Display(Name = "Title")]
		public string Title { get; set; }
	}

	public class UserFileModel
	{
		public long Id { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public DateTime Uploaded { get; set; }

		public string Author { get; set; }

		public int Views { get; set; }

		public int Downloads { get; set; }

		public bool Hidden { get; set; }

		public string FileName { get; set; }

		public int FileSize { get; set; }

		public int? GameId { get; set; }

		public string GameName { get; set; }

		public string System { get; set; }
	}

	public class UserMovieModel : UserFileModel
	{
		public TimeSpan Length { get; set; }

		public int Frames { get; set; }

		public int Rerecords { get; set; }
	}

	public class UserFileIndexModel
	{
		public IEnumerable<UserWithMovie> UsersWithMovies { get; set; } = new List<UserWithMovie>();
		public IEnumerable<UserMovieListModel> LatestMovies { get; set; } = new List<UserMovieListModel>();
		public IEnumerable<GameWithMovie> GamesWithMovies { get; set; } = new List<GameWithMovie>();

		public class UserWithMovie
		{
			public string UserName { get; set; }
			public DateTime Latest { get; set; }
		}

		public class GameWithMovie
		{
			public int GameId { get; set; }
			public string GameName { get; set; }
			public string SystemCode { get; set; }
			public DateTime Latest { get; set; }
		}
	}

	public class GameFileModel
	{
		public string SystemCode { get; set; }
		public int GameId { get; set; }
		public string GameName { get; set; }

		public IEnumerable<UserFileModel> Files { get; set; } = new List<UserFileModel>();
	}
}
