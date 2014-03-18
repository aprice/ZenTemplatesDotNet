using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
			Document = new HtmlDocument()
			{
				OptionUseIdAttribute = true,
				OptionFixNestedTags = true,
				OptionWriteEmptyNodes = true
			};
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
		/// Load a template from an HTML file reference.
		/// </summary>
		/// <param name="file">Reference to a file containing template content</param>
		public void LoadTemplateFile(FileInfo file)
		{
			string contents;
			using (FileStream stream = file.OpenRead())
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					contents = reader.ReadToEnd();
				}
			}

			LoadTemplateHtml(contents);
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
					string id = element.Id;
					attribute = element.Attributes["data-z-else"];
					if (attribute != null)
					{
						id = attribute.Value;
					}

					if (!String.IsNullOrEmpty(id))
					{
						IEnumerable<HtmlNode> allElements = Document.DocumentNode.Descendants();
						IEnumerable<HtmlNode> allElses =
							from el in allElements
							where el.NodeType == HtmlNodeType.Element
								 && ((el.Attributes["data-z-else"] != null && el.Attributes["data-z-else"].Value == id) || el.Id == id)
							select el;
						foreach (HtmlNode el in allElses)
						{
							if (el != element)
							{
								el.Remove();
							}
						}

						element.Id = id;
					}
				}
				else
				{
					element.Remove();
					return;
				}
			}
			else
			{
				attribute = element.Attributes["data-z-else"];
				if (attribute != null)
				{
					element.SetAttributeValue("id", attribute.Value);
					attribute.Remove();
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
