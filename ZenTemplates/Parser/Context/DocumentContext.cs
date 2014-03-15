using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZenTemplates.Parser.Context
{
	public class DocumentContext : ILookupContext
	{
		internal ModelContext ModelContext { get; private set; }
		internal HtmlNode Element { get; private set; }

		public DocumentContext(ModelContext modelCtx, HtmlNode element)
		{
			ModelContext = modelCtx;
			Element = element;
		}

		public object GetProperty(string key)
		{
			return ModelContext.GetProperty(key);
		}

		public bool HasProperty(string key)
		{
			return ModelContext.HasProperty(key);
		}

		public bool HasProperty(string key, bool checkRoot)
		{
			return ModelContext.HasProperty(key, checkRoot);
		}

		public bool LookupBoolean(string key)
		{
			return ModelContext.LookupBoolean(key);
		}
	}
}
