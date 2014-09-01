using HtmlAgilityPack;
using System;

namespace ZenTemplates.Parser
{
	public static class HtmlUtilities
	{
		public static string[] GetClasses(this HtmlNode element)
		{
			HtmlAttribute attribute = element.Attributes["class"];
			if (attribute != null)
			{
				return attribute.Value.Split(' ');
			}
			else
			{
				return new string[0];
			}
		}

		public static HtmlNode GetIdAttributeTarget(this HtmlNode source, string attributeName)
		{
			HtmlNode target = null;
			string attributeValue = source.GetAttributeValue(attributeName, "");
			if (!String.IsNullOrWhiteSpace(attributeValue))
			{
				target = source.OwnerDocument.GetElementbyId(attributeValue);
			}

			return target;
		}
	}
}
