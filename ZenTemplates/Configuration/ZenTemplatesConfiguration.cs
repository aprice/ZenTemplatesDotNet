using System.Configuration;
using ZenTemplates.Configuration.Elements;

namespace ZenTemplates.Configuration
{
    public class ZenTemplatesConfiguration
    {
		public string TemplateRoot { get; set; }
		public string SharedRoot { get; set; }
		public string TemplateFileExtension { get; set; }
		public string SnippetFileExtension { get; set; }

		public ZenTemplatesConfiguration()
		{
			ZenTemplatesSection section = (ZenTemplatesSection)ConfigurationManager.GetSection("zenTemplates");
			if (section != null && section.Templates != null)
			{
				TemplateRoot = section.Templates.RootPath;
				SharedRoot = section.Templates.SharedRootPath;
				TemplateFileExtension = section.Templates.Extension;
				SnippetFileExtension = section.Templates.SnippetExtension;
			}
			else
			{
				TemplateRoot = SharedRoot = "..";
				TemplateFileExtension = SnippetFileExtension = ".html";
			}
		}
    }
}
