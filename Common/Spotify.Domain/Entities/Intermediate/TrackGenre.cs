﻿namespace Spotify.Domain.Entities.Intermediate
{
	public class TrackGenre
	{
		public int TrackId { get; set; }

		public Track Track { get; set; }

		public int GenreId { get; set; }

		public Genre Genre { get; set; }
	}
}