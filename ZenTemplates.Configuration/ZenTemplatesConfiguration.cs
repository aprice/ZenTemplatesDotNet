using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenTemplates.Configuration.Elements;

namespace ZenTemplates.Configuration
{
    public class ZenTemplatesConfiguration
    {
		private static ZenTemplatesConfiguration _instance = new ZenTemplatesConfiguration();
		public static ZenTemplatesConfiguration Instance
		{
			get
			{
				return _instance;
			}
		}

		public string TemplateRoot { get; protected set; }
		public string ParentRoot { get; protected set; }
		public string SnippetRoot { get; protected set; }
		public string TemplateFileExtension { get; protected set; }
		public string SnippetFileExtension { get; protected set; }

		public ZenTemplatesConfiguration()
		{
			ZenTemplatesSection section = (ZenTemplatesSection)ConfigurationManager.GetSection("zenTemplates");
			if (section != null && section.Templates != null)
			{
				TemplateRoot = ParentRoot = SnippetRoot = section.Templates.RootPath;
				TemplateFileExtension = SnippetFileExtension = section.Templates.Extension;
			}
			else
			{
				TemplateRoot = ParentRoot = SnippetRoot = "/";
				TemplateFileExtension = SnippetFileExtension = ".html";
			}
		}
    }
}
