﻿using System;
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
			FileInfo result = LookInDirectory(fileName, Configuration.SharedRoot);

			if (result == null)
			{
				result = LookInDirectory(fileName, currentTemplateDirectory);
			}

			return result;
		}

		public FileInfo GetSnippetFile(string name, string currentTemplateDirectory = null)
		{
			string fileName = AppendExtension(name, Configuration.SnippetFileExtension);
			FileInfo result = LookInDirectory(fileName, Configuration.SharedRoot);

			if (result == null)
			{
				result = LookInDirectory(fileName, currentTemplateDirectory);
			}

			return result;
		}

		public TemplateFile LoadTemplateFile(string name, string currentTemplateDirectory = null)
		{
			FileInfo file = GetTemplateFile(name, currentTemplateDirectory);
			return BuildTemplateFile(file);
		}

		public TemplateFile LoadParentFile(string name, string currentTemplateDirectory = null)
		{
			FileInfo file = GetParentFile(name, currentTemplateDirectory);
			return BuildTemplateFile(file);
		}

		public TemplateFile LoadSnippetFile(string name, string currentTemplateDirectory = null)
		{
			FileInfo file = GetSnippetFile(name, currentTemplateDirectory);
			return BuildTemplateFile(file);
		}

		private TemplateFile BuildTemplateFile(FileInfo file)
		{
			if (file == null)
			{
				return null;
			}

			string contents;
			using (FileStream stream = file.OpenRead())
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					contents = reader.ReadToEnd();
				}
			}

			return new TemplateFile()
			{
				TemplateName = file.Name.Substring(0, file.Name.LastIndexOf('.')),
				TemplateDirectory = file.DirectoryName,
				TemplateContents = contents
			};
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
