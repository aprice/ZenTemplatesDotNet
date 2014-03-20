using HtmlAgilityPack;
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
		public static TemplateParser GetTemplateParser(string name, string directory = null)
		{
			FileRepository repo = new FileRepository();
			TemplateFile file = repo.LoadTemplateFile(name, directory);
			TemplateParser parser = new TemplateParser(repo);
			parser.LoadTemplateFile(file);
			return parser;
		}

		public TemplateParser() : this(new FileRepository()) { }

		public TemplateParser(FileRepository fileRepo)
			: base(fileRepo)
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
		public static TemplateParser<T> GetTemplateParser<T>(string name, string directory = null)
		{
			FileRepository repo = new FileRepository();
			TemplateFile file = repo.LoadTemplateFile(name, directory);
			TemplateParser<T> parser = new TemplateParser<T>(repo);
			parser.LoadTemplateFile(file);
			return parser;
		}

		private HtmlDocument Document;
		private ModelContext RootModelContext;
		private FileRepository FileRepository;
		private string CurrentTemplateDir;
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

		public TemplateParser() : this(new FileRepository()) { }

		public TemplateParser(FileRepository fileRepo)
		{
			Document = new HtmlDocument()
			{
				OptionUseIdAttribute = true,
				OptionFixNestedTags = true,
				OptionWriteEmptyNodes = true
			};

			FileRepository = fileRepo;
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
		public void LoadTemplateFile(TemplateFile file)
		{
			LoadTemplateHtml(file.TemplateContents);
			CurrentTemplateDir = file.TemplateDirectory;
		}

		/// <summary>
		/// Execute rendering process for the current template and model.
		/// </summary>
		public void Render()
		{
			HandleDerivation();
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

		private void HandleDerivation()
		{
			HtmlAttribute attribute = Document.DocumentNode.LastChild.Attributes["data-z-derivesfrom"];
			if (attribute == null)
			{
				return;
			}

			TemplateFile parentFile = FileRepository.LoadParentFile(attribute.Value, CurrentTemplateDir);
			attribute.Remove();
			if (parentFile == null)
			{
				return;
			}

			TemplateParser parentParser = new TemplateParser();
			parentParser.LoadTemplateFile(parentFile);
			parentParser.HandleDerivation();

			// Handle overrides
			IEnumerable<HtmlNode> allIds = GetAllByAttribute("id");

			foreach (HtmlNode el in allIds)
			{
				HtmlNode parentElement = parentParser.Document.GetElementbyId(el.Id);
				if (parentElement != null)
				{
					parentElement.ParentNode.ReplaceChild(el.CloneNode(true), parentElement);
				}
			}

			// Handle appends

			// Replace Document
			Document = parentParser.Document;
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
						IEnumerable<HtmlNode> allElements = GetAllElements();
						IEnumerable<HtmlNode> allElses =
							from el in allElements
							where el.Id == id
								|| (el.Attributes["data-z-else"] != null && el.Attributes["data-z-else"].Value == id)
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

		private IEnumerable<HtmlNode> GetAllElements()
		{
			return Document.DocumentNode.Descendants().Where(n => n.NodeType == HtmlNodeType.Element);
		}

		private IEnumerable<HtmlNode> GetAllByAttribute(string attributeName)
		{
			IEnumerable<HtmlNode> allElements = GetAllElements();
			IEnumerable<HtmlNode> matchingElements =
				from el in allElements
				where el.Attributes[attributeName] != null
				select el;
			return matchingElements;
		}

		private IEnumerable<HtmlNode> GetAllByAttribute(string attributeName, string value)
		{
			IEnumerable<HtmlNode> allElements = GetAllElements();
			IEnumerable<HtmlNode> matchingElements =
				from el in allElements
				where el.Attributes[attributeName] != null
					&& el.Attributes[attributeName].Value == value
				select el;
			return matchingElements;
		}
	}
}
