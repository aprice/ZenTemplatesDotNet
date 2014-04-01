using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZenTemplates.Configuration;

namespace ZenTemplates
{
	public class FileRepository
	{
		protected ZenTemplatesConfiguration Configuration { get; set; }

		public FileRepository() : this(new ZenTemplatesConfiguration()) { }

		public FileRepository(ZenTemplatesConfiguration configuration)
		{
			Configuration = configuration;
		}

		/// <summary>
		/// Get a reference to a template file by name.
		/// </summary>
		/// <param name="name">Name of the template file, with or without file extension</param>
		/// <param name="currentTemplateDirectory">Directory to search, relative to template root</param>
		/// <returns>FileInfo referring to the requested file, or null if file was not found</returns>
		public FileInfo GetTemplateFile(string name, string currentTemplateDirectory = null)
		{
			string fileName = AppendExtension(name, Configuration.TemplateFileExtension);
			FileInfo result = LookInDirectory(fileName);

			if (result == null && !String.IsNullOrEmpty(currentTemplateDirectory))
			{
				result = LookInDirectory(fileName, Configuration.TemplateRoot, currentTemplateDirectory);
			}
			
			if (result == null)
			{
				result = LookInDirectory(fileName, Configuration.TemplateRoot);
			}

			return result;
		}

		/// <summary>
		/// Get a reference to a parent template file by name.
		/// </summary>
		/// <param name="name">Name of the template file, with or without file extension</param>
		/// <param name="currentTemplateDirectory">Directory to search, relative to template root</param>
		/// <returns>FileInfo referring to the requested file, or null if file was not found</returns>
		public FileInfo GetParentFile(string name, string currentTemplateDirectory = null)
		{
			string fileName = AppendExtension(name, Configuration.TemplateFileExtension);
			FileInfo result = LookInDirectory(fileName);

			if (result == null && !String.IsNullOrEmpty(currentTemplateDirectory))
			{
				if (Directory.Exists(currentTemplateDirectory))
				{
					result = LookInDirectory(fileName, currentTemplateDirectory);
				}
				else
				{
					result = LookInDirectory(fileName, Configuration.TemplateRoot, currentTemplateDirectory);
				}
			}

			if (result == null)
			{
				result = LookInDirectory(fileName, Configuration.SharedRoot);
			}

			return result;
		}

		/// <summary>
		/// Get a reference to a snippet file by name.
		/// </summary>
		/// <param name="name">Name of the snippet file, with or without file extension</param>
		/// <param name="currentTemplateDirectory">Directory to search, relative to template root</param>
		/// <returns>FileInfo referring to the requested file, or null if file was not found</returns>
		public FileInfo GetSnippetFile(string name, string currentTemplateDirectory = null)
		{
			string fileName = AppendExtension(name, Configuration.SnippetFileExtension);
			FileInfo result = LookInDirectory(fileName);

			if (result == null && !String.IsNullOrEmpty(currentTemplateDirectory))
			{
				result = LookInDirectory(fileName, Configuration.TemplateRoot, currentTemplateDirectory);
			}

			if (result == null)
			{
				result = LookInDirectory(fileName, Configuration.SharedRoot);
			}

			if (result == null)
			{
				result = LookInDirectory(fileName, Configuration.TemplateRoot);
			}

			return result;
		}

		/// <summary>
		/// Get a reference to a model file by name.
		/// </summary>
		/// <param name="name">Name of the model file, with or without file extension</param>
		/// <param name="currentTemplateDirectory">Directory to search, relative to template root</param>
		/// <returns>FileInfo referring to the requested file, or null if file was not found</returns>
		public FileInfo GetModelFile(string name, string currentTemplateDirectory = null)
		{
			string fileName = AppendExtension(name, ".json");
			FileInfo result = LookInDirectory(fileName);

			if (result == null && !String.IsNullOrEmpty(currentTemplateDirectory))
			{
				result = LookInDirectory(fileName, Configuration.TemplateRoot, currentTemplateDirectory);
			}

			if (result == null)
			{
				result = LookInDirectory(fileName, Configuration.SharedRoot);
			}

			if (result == null)
			{
				result = LookInDirectory(fileName, Configuration.TemplateRoot);
			}

			return result;
		}

		/// <summary>
		/// Load a template file by name.
		/// </summary>
		/// <param name="name">Name of the snippet file, with or without file extension</param>
		/// <param name="currentTemplateDirectory">Directory to search, relative to template root</param>
		/// <returns>TemplateFile for the requested file, or null if file was not found</returns>
		public TemplateFile LoadTemplateFile(string name, string currentTemplateDirectory = null)
		{
			FileInfo file = GetTemplateFile(name, currentTemplateDirectory);
			return BuildTemplateFile(file);
		}

		/// <summary>
		/// Load a parent template file by name.
		/// </summary>
		/// <param name="name">Name of the snippet file, with or without file extension</param>
		/// <param name="currentTemplateDirectory">Directory to search, relative to template root</param>
		/// <returns>TemplateFile for the requested file, or null if file was not found</returns>
		public TemplateFile LoadParentFile(string name, string currentTemplateDirectory = null)
		{
			FileInfo file = GetParentFile(name, currentTemplateDirectory);
			return BuildTemplateFile(file);
		}

		/// <summary>
		/// Load a snippet file by name.
		/// </summary>
		/// <param name="name">Name of the snippet file, with or without file extension</param>
		/// <param name="currentTemplateDirectory">Directory to search, relative to template root</param>
		/// <returns>TemplateFile for the requested file, or null if file was not found</returns>
		public TemplateFile LoadSnippetFile(string name, string currentTemplateDirectory = null)
		{
			FileInfo file = GetSnippetFile(name, currentTemplateDirectory);
			return BuildTemplateFile(file);
		}

		private FileInfo LookInDirectory(string fileName, params string[] path)
		{
			string fullPath = BuildPath(path, fileName);
			FileInfo result = new FileInfo(fullPath);
			return result.Exists ? result : null;
		}

		private string BuildPath(string[] parts, params string[] moreParts)
		{
			List<string> pathParts = new List<string>(parts);
			pathParts.AddRange(moreParts);
			for (int i = 0; i < pathParts.Count; i++)
			{
				pathParts[i] = pathParts[i].Trim('/');
			}
			pathParts = pathParts.Where(s => !String.IsNullOrEmpty(s)).ToList();
			return String.Join("/", pathParts);
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
			if (name.Contains("."))
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
