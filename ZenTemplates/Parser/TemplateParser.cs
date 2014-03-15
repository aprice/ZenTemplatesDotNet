using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenTemplates.Parser.Context;

namespace ZenTemplates.Parser
{
	public class TemplateParser : TemplateParser<IDictionary<string,object>>
	{
		public TemplateParser()
			: base()
		{
			Model = new Dictionary<string, object>();
		}
	}

	public class TemplateParser<TModel>
	{
		private HtmlDocument Document;
		private ModelContext RootModelContext;
		private TModel _model;
		public TModel Model
		{
			get
			{
				return _model;
			}
			set
			{
				_model = value;
				RootModelContext = new ModelContext(_model);
			}
		}

		public TemplateParser()
		{
			Document = new HtmlDocument();
		}

		public void HandleInjection()
		{
			DocumentContext rootDocContext = new DocumentContext(RootModelContext, Document.DocumentNode);
			HandleElement(rootDocContext);
		}

		private void HandleElement(DocumentContext docContext)
		{
			HtmlNode element = docContext.Element;
			bool injecting = false;
			HtmlAttribute attribute = element.Attributes["data-z-inject"];
			if (attribute != null)
			{
				injecting = true;
				Inject(docContext, attribute.Value);
				element.Attributes.Remove(attribute);
			}

			if (!injecting && element.HasChildNodes)
			{
				HandleChildren(docContext);
			}
		}

		private void HandleChildren(DocumentContext docContext)
		{
			HtmlNode element = docContext.Element;
			foreach (HtmlNode child in element.ChildNodes)
			{
				if (child.NodeType == HtmlNodeType.Element)
				{
					DocumentContext childContext = new DocumentContext(docContext.ModelContext, child);
					HandleElement(childContext);
				}
			}
		}

		private void Inject(DocumentContext docContext, string key)
		{
			HtmlNode element = docContext.Element;

			object val = docContext.GetProperty(key);
			if (val == null)
			{
				element.InnerHtml = "";
			}
			else if (val is ValueType || val is string)
			{
				element.InnerHtml = HtmlEntity.Entitize(val.ToString());
			}
			else
			{
				ModelContext modelContext = new ModelContext(val);
				DocumentContext childContext = new DocumentContext(modelContext, element);
				HandleChildren(childContext);
			}
		}

		public string GetOutput()
		{
			return Document.DocumentNode.OuterHtml;
		}

		public void LoadTemplateHtml(string inHtml)
		{
			Document.LoadHtml(inHtml);
		}
	}
}
