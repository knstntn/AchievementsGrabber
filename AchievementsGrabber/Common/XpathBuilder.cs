using System;
using System.Linq;
using System.Text;
using AchievementsGrabber.Common.Extensions;

namespace AchievementsGrabber.Common
{
	// TODO: reimplement with expression visitor and node model
	public sealed class XpathBuilder
	{
		// Contains currently generated string
		private readonly StringBuilder _builder;

		public XpathBuilder() : this(new StringBuilder()) {}

		private XpathBuilder(StringBuilder builder)
		{
			_builder = builder;
		}

		// Selects nodes in the document from the current node that match the selection no matter where they are 
		public XpathBuilder Any(string element)
		{
			return Append(builder => builder.AppendFormat("//{0}", element.Safe()));
		}

		// Selects from the root node
		public XpathBuilder Child(string element)
		{
			return Append(builder => builder.AppendFormat("/{0}", element.Safe()));
		}

		// selects child in specified position
		public XpathBuilder ChildInPosition(string element, int position)
		{
			return Append(builder => builder.AppendFormat("/{0}[{1}]", element, position));
		}

		// Selects attributes from the root node
		public XpathBuilder Attribute(string attribute)
		{
			return Append(builder => builder.AppendFormat("/@{0}", attribute.Safe()));
		}

		// Selects all the elements that have an attribute named 'attribute' with value 'value'
		// If value is not specified, only name will be checked
		public XpathBuilder WithAttribute(string attribute, string value)
		{
			return WithAttributes(Tuple.Create(attribute, value));
		}

		// Selects all the elements that have an attributes with names/values specified in collection
		public XpathBuilder WithAttributes(params Tuple<string, string>[] attributes)
		{
			var safe = attributes.Safe().ToArray();
			if (safe.Length == 0) return this;

			var items = from x in safe
			            let name = x.Item1.Safe()
			            let value = x.Item2.Safe()
			            select string.IsNullOrEmpty(value)
				                   ? string.Format("@{0}", name)
				                   : string.Format("@{0}='{1}'", name, value);
			return Append(builder => builder.AppendFormat("[{0}]", string.Join(" or ", items)));
		}

		public XpathBuilder Not(string element)
		{
			return Append(builder => builder.AppendFormat("/*[not(self::{0})]", element));
		}

		// Converting underlying builder into string
		public override string ToString()
		{
			return _builder.ToString();
		}

		// clones existing builder
		public XpathBuilder Clone()
		{
			return new XpathBuilder(new StringBuilder(_builder.ToString()));
		}

		// Added to make code a bit cleaner
		private XpathBuilder Append(Action<StringBuilder> action)
		{
			action(_builder);
			return this;
		}
	}
}