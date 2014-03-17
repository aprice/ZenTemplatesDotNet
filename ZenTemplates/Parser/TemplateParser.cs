﻿using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ZenTemplates.Parser.Context;

namespace ZenTemplates.Parser
{
	/// <summary>
	/// Default Zen Tempalate Parser using a basic string/object key-value model.
	/// </summary>
	public class TemplateParser : TemplateParser<IDictionary<string, object>>
	{
		public TemplateParser()
			: base()
		{
			Model = new Dictionary<string, object>();
		}
	}

	/// <summary>
	/// Zen Template parser implementation.
	/// </summary>
	/// <typeparam name="TModel">Model type</typeparam>
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

		/// <summary>
		/// Load a template from an HTML string.
		/// </summary>
		/// <param name="inHtml">HTML template content</param>
		public void LoadTemplateHtml(string inHtml)
		{
			Document.LoadHtml(inHtml);
		}

		/// <summary>
		/// Execute rendering process for the current template and model.
		/// </summary>
		public void Render()
		{
			HandleInjection();
		}

		/// <summary>
		/// Return the transformed document as a string.
		/// </summary>
		/// <returns>Output of document transformation</returns>
		public string GetOutput()
		{
			return Document.DocumentNode.OuterHtml;
		}

		private void HandleInjection()
		{
			DocumentContext rootDocContext = new DocumentContext(RootModelContext, Document.DocumentNode);
			HandleElement(rootDocContext);
		}

		private void HandleElement(DocumentContext docContext)
		{
			HtmlNode element = docContext.Element;
			HtmlAttribute attribute;
			attribute = element.Attributes["data-z-lorem"];
			if (attribute != null)
			{
				element.Remove();
				return;
			}

			attribute = element.Attributes["data-z-if"];
			if (attribute != null)
			{
				BooleanParser boolParser = new BooleanParser(docContext);
				if (boolParser.Parse(attribute.Value))
				{
					attribute.Remove();
				}
				else
				{
					element.Remove();
					return;
				}
			}

			bool injecting = false;
			attribute = element.Attributes["data-z-inject"];
			if (attribute != null)
			{
				injecting = true;
				attribute.Remove();
				Inject(docContext, docContext.GetProperty(attribute.Value));
			}

			if (!injecting && element.HasChildNodes)
			{
				HandleChildren(docContext);
			}
		}

		private void HandleChildren(DocumentContext docContext)
		{
			HtmlNode element = docContext.Element;
			IEnumerable<HtmlNode> childNodes = (from node in element.ChildNodes
												where node.NodeType == HtmlNodeType.Element
												select node).ToList();
			foreach (HtmlNode child in childNodes)
			{
				if (child.NodeType == HtmlNodeType.Element)
				{
					DocumentContext childContext = new DocumentContext(docContext.ModelContext, child);
					HandleElement(childContext);
				}
			}
			HandleSubstitution(docContext);
		}

		private void Inject(DocumentContext docContext, object val)
		{
			HtmlNode element = docContext.Element;

			if (val == null)
			{
				element.InnerHtml = "";
			}
			else if (val is ValueType || val is string)
			{
				element.InnerHtml = HtmlEntity.Entitize(val.ToString());
			}
			else if (val is IDictionary)
			{
				ModelContext modelContext = new ModelContext(val);
				DocumentContext childContext = new DocumentContext(modelContext, element);
				HandleChildren(childContext);
			}
			else if (val is IEnumerable)
			{
				HtmlNode insertAfter = element;
				HtmlNode parentNode = element.ParentNode;
				foreach (object item in (IEnumerable)val)
				{
					HtmlNode newElement = element.CloneNode(true);
					parentNode.InsertAfter(newElement, insertAfter);
					insertAfter = newElement;
					ModelContext modelContext = new ModelContext(item);
					DocumentContext childContext = new DocumentContext(modelContext, newElement);
					Inject(childContext, item);
				}
				element.Remove();
			}
			else
			{
				ModelContext modelContext = new ModelContext(val);
				DocumentContext childContext = new DocumentContext(modelContext, element);
				HandleChildren(childContext);
			}
		}

		private void HandleSubstitution(DocumentContext docContext)
		{
			SubstitutionParser subsParser = new SubstitutionParser(docContext);
			string result = subsParser.Substitute(docContext.Element.InnerHtml);
			docContext.Element.InnerHtml = result;
			foreach (HtmlAttribute attribute in docContext.Element.Attributes)
			{
				attribute.Value = subsParser.Substitute(attribute.Value);
			}
		}
	}
}
