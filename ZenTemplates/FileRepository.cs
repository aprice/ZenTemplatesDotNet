using System;
using System.IO;
using ZenTemplates.Configuration;

namespace ZenTemplates
{
	public class FileRepository
	{
		protected ZenTemplatesConfiguration Configuration { get; set; }

		public FileRepository() : this(ZenTemplatesConfiguration.Current) { }

		public FileRepository(ZenTemplatesConfiguration configuration)
		{
			Configuration = configuration;
		}

		protected FileInfo LookInDirectory(string fileName, string directory)
		{
			string dirName = directory;
			if (!Directory.Exists(directory))
			{
				dirName = Directory.GetCurrentDirectory() + "/" + directory;
			}

			if (!Directory.Exists(directory))
			{
				return null;
			}

			string fullPath = directory + "/" + fileName;
			if (File.Exists(fullPath))
			{
				return new FileInfo(fullPath);
			}
			else
			{
				return null;
			}
		}

		public FileInfo GetTemplateFile(string name, string currentTemplateDirectory = null)
		{
			string fileName = AppendExtension(name, Configuration.TemplateFileExtension);
			FileInfo result = LookInDirectory(fileName, Configuration.TemplateRoot);
			
			if (result == null)
			{
				result = LookInDirectory(fileName, currentTemplateDirectory);
			}

			return result;
		}

		public FileInfo GetParentFile(string name, string currentTemplateDirectory = null)
		{
			string fileName = AppendExtension(name, Configuration.TemplateFileExtension);
			FileInfo result = LookInDirectory(fileName, Configuration.ParentRoot);

			if (result == null)
			{
				result = LookInDirectory(fileName, currentTemplateDirectory);
			}

			return result;
		}

		public FileInfo GetSnippetFile(string name, string currentTemplateDirectory = null)
		{
			string fileName = AppendExtension(name, Configuration.SnippetFileExtension);
			FileInfo result = LookInDirectory(fileName, Configuration.SnippetRoot);

			if (result == null)
			{
				result = LookInDirectory(fileName, currentTemplateDirectory);
			}

			return result;
		}

		private string AppendExtension(string name, string extension)
		{
			if (name.EndsWith(extension))
			{
				return name;
			}
			else
			{
				return name + extension;
			}
		}
	}
}
