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
			ProcessElements();
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
			HtmlNode htmlElement = Document.DocumentNode.LastChild;
			HtmlAttribute attribute = htmlElement.Attributes["data-z-derivesfrom"];
			string parentName = null;
			if (attribute != null)
			{
				parentName = attribute.Value;
				attribute.Remove();
			}
			else if (htmlElement.Attributes["class"] != null)
			{
				string[] classes = GetClasses(htmlElement);
				parentName = classes[0];
			}

			if (String.IsNullOrEmpty(parentName))
			{
				return;
			}

			TemplateFile parentFile = FileRepository.LoadParentFile(parentName, CurrentTemplateDir);
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
			IEnumerable<HtmlNode> allAppends = GetAllByAttribute("data-z-append");
			foreach (HtmlNode el in allAppends)
			{
				string[] parts = el.Attributes["data-z-append"].Value.Split(new char[]{':'}, 2);
				el.Attributes["data-z-append"].Remove();
				string where = parts.Length > 1 ? parts[0].ToLower() : "after";
				string targetId = parts.Last();
				HtmlNode targetSibling = parentParser.Document.GetElementbyId(targetId);
				if (targetSibling == null)
				{
					continue;
				}
				else if (where == "before")
				{
					targetSibling.ParentNode.InsertBefore(el.CloneNode(true), targetSibling);
				}
				else if (where == "after")
				{
					targetSibling.ParentNode.InsertAfter(el.CloneNode(true), targetSibling);
				}
			}

			// Replace Document
			Document = parentParser.Document;
		}

		private void ProcessElements()
		{
			DocumentContext rootDocContext = new DocumentContext(RootModelContext, Document.DocumentNode);
			HandleElement(rootDocContext);
		}

		private void HandleElement(DocumentContext docContext)
		{
			if (HandleConditionals(docContext))
			{
				HandleInjection(docContext);
			}
		}

		/// <summary>
		/// Handle conditionals for an element
		/// </summary>
		/// <returns>True if element survived, false if it was removed</returns>
		private bool HandleConditionals(DocumentContext docContext)
		{
			HtmlNode element = docContext.Element;
			HtmlAttribute attribute;
			attribute = element.Attributes["data-z-lorem"];
			if (attribute != null)
			{
				element.Remove();
				return false;
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
					return false;
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

			return true;
		}

		private void HandleInjection(DocumentContext docContext)
		{
			HtmlNode element = docContext.Element;
			HtmlAttribute attribute;
			attribute = element.Attributes["data-z-inject"];
			if (attribute != null)
			{
				attribute.Remove();
				Inject(docContext, docContext.GetProperty(attribute.Value));
				return;
			}

			string[] classes = GetClasses(element);
			if (classes.Length > 0)
			{
				object propertyValue = docContext.GetProperty(classes[0]);
				if (propertyValue != null)
				{
					Inject(docContext, propertyValue);
					return;
				}
			}

			attribute = element.Attributes["id"];
			if (attribute != null)
			{
				object propertyValue = docContext.GetProperty(attribute.Value);
				if (propertyValue != null)
				{
					Inject(docContext, docContext.GetProperty(attribute.Value));
					return;
				}
			}

			if (element.HasChildNodes)
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

		private string[] GetClasses(HtmlNode element)
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
	}
}
