using System.Configuration;
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
		public string SharedRoot { get; set; }
		public string TemplateFileExtension { get; set; }
		public string SnippetFileExtension { get; set; }

		public ZenTemplatesConfiguration()
		{
			ZenTemplatesSection section = (ZenTemplatesSection)ConfigurationManager.GetSection("zenTemplates");
			if (section != null && section.Templates != null)
			{
				TemplateRoot = SharedRoot = section.Templates.RootPath;
				TemplateFileExtension = SnippetFileExtension = section.Templates.Extension;
			}
			else
			{
				TemplateRoot = SharedRoot = "/";
				TemplateFileExtension = SnippetFileExtension = ".html";
			}
		}
    }
}
