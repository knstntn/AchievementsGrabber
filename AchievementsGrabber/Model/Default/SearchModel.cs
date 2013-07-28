using System.ComponentModel.DataAnnotations;
using AchievementsGrabber.Common;

namespace AchievementsGrabber.Model.Default
{
	public sealed class SearchModel
	{
		public bool InSearch { get; set; }

		[Required]
		[RegularExpression(InputPatterns.Text, ErrorMessage = "Search field contains invalid characters")]
		public string Text { get; set; }
	}
}