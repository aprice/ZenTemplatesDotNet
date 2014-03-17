using HtmlAgilityPack;

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
	}
}
