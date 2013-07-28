using System;
using System.Collections.Generic;

namespace AchievementsGrabber.Model.Default
{
	public sealed class ResultModel
	{
		public IEnumerable<GameModel> Games { get; set; }
		public string Error { get; set; }
	}

	public sealed class GameModel
	{
		public Guid Id { get; set; }
		public string Image { get; set; }
		public string Text { get; set; }
		public string Url { get; set; }
		public IEnumerable<GameModel> Siblings { get; set; }
	}
}