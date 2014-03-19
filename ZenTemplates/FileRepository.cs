using System;
using System.Collections.Generic;
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

		protected FileInfo LookInDirectory(string fileName, params string[] path)
		{
			List<string> pathParts = new List<string>(path);
			pathParts.Add(fileName);
			string fullPath = String.Join("/", pathParts);
			FileInfo result = new FileInfo(fullPath);
			return result.Exists ? result : null;
		}

		public FileInfo GetTemplateFile(string name, string currentTemplateDirectory = null)
		{
			string fileName = AppendExtension(name, Configuration.TemplateFileExtension);
			FileInfo result = LookInDirectory(fileName, Configuration.TemplateRoot);
			
			if (result == null && !String.IsNullOrEmpty(currentTemplateDirectory))
			{
				result = LookInDirectory(fileName, Configuration.TemplateRoot, currentTemplateDirectory);
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
