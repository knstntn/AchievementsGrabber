using System;
using System.Collections.Generic;

namespace AchievementsGrabber.Model.Default
{
	public sealed class DetailsModel
	{
		public string Name { get; set; }
		public IEnumerable<string> Images { get; set; }
		public IEnumerable<AchievementModel> Achievements { get; set; }
		public OverviewModel Overview { get; set; }
	}

	public sealed class AchievementModel
	{
		public Guid Id { get; set; }
		public string Image { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int? Points { get; set; }
		public string Guide { get; set; }
		public bool IsSecret { get; set; }
	}

	public sealed class OverviewModel
	{
		public string Description { get; set; }
		public string Image { get; set; }
	}
}