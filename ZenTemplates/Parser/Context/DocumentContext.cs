using HtmlAgilityPack;

namespace ZenTemplates.Parser.Context
{
	internal class DocumentContext : ILookupContext
	{
		public DocumentContext ParentContext { get; set; }
		public ModelContext ModelContext { get; set; }
		public HtmlNode Element { get; set; }
		public bool NoInject { get; set; }
		public bool NoInfer { get; set; }

		public DocumentContext(ModelContext modelCtx, HtmlNode element)
		{
			ModelContext = modelCtx;
			Element = element;
			NoInject = NoInfer = false;
		}

		public DocumentContext(DocumentContext parent, HtmlNode element)
			: this(parent, parent.ModelContext, element)
		{
		}

		public DocumentContext(DocumentContext parent, ModelContext modelCtx, HtmlNode element)
			: this(modelCtx, element)
		{
			ParentContext = parent;
			NoInject = parent.NoInject;
			NoInfer = parent.NoInfer;
		}

		public object GetProperty(string key)
		{
			if (ParentContext == null)
			{
				return ModelContext.GetProperty(key);
			}
			else
			{
				return ModelContext.GetProperty(key) ?? ParentContext.GetProperty(key);
			}
		}
	}
}
