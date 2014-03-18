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
		private static ZenTemplatesConfiguration _current = new ZenTemplatesConfiguration();
		public static ZenTemplatesConfiguration Current
		{
			get
			{
				return _current;
			}
		}

		public string TemplateRoot { get; set; }
		public string ParentRoot { get; set; }
		public string SnippetRoot { get; set; }
		public string TemplateFileExtension { get; set; }
		public string SnippetFileExtension { get; set; }

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
