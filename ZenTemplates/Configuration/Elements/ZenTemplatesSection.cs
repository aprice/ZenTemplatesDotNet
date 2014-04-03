using System.Configuration;

namespace ZenTemplates.Configuration.Elements
{
	class ZenTemplatesSection : ConfigurationSection
	{
		[ConfigurationProperty("templates")]
		public TemplatesElement Templates
		{
			get
			{
				return (TemplatesElement)base["templates"];
			}
			set
			{
				base["templates"] = value;
			}
		}
	}

	class TemplatesElement : ConfigurationElement
	{
		[ConfigurationProperty("rootPath", DefaultValue = ".")]
		public string RootPath
		{
			get
			{
				return (string)base["rootPath"];
			}
			set
			{
				base["rootPath"] = value;
			}
		}

		[ConfigurationProperty("sharedRootPath", DefaultValue = ".")]
		public string SharedRootPath
		{
			get
			{
				return (string)base["sharedRootPath"];
			}
			set
			{
				base["sharedRootPath"] = value;
			}
		}

		[ConfigurationProperty("extension", DefaultValue = ".html")]
		public string Extension
		{
			get
			{
				return (string)base["extension"];
			}
			set
			{
				base["extension"] = value;
			}
		}

		[ConfigurationProperty("snippetExtension", DefaultValue = ".html")]
		public string SnippetExtension
		{
			get
			{
				return (string)base["snippetExtension"];
			}
			set
			{
				base["snippetExtension"] = value;
			}
		}
	}
}
