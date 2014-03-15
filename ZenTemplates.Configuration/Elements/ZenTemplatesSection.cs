﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		[ConfigurationProperty("rootPath", DefaultValue = "/")]
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
	}
}
