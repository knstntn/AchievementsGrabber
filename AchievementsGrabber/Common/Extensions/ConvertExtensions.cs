using System;

namespace AchievementsGrabber.Common.Extensions
{
	public static class ConvertExtensions
	{
		public static T Get<TModel, T>(this TModel model, Func<TModel, T> getter) where TModel : class
		{
			return Get(model, getter, default(T));
		}

		public static T Get<TModel, T>(this TModel model, Func<TModel, T> getter, T @default) where TModel : class
		{
			return model == null ? @default : getter(model);
		}
	}
}