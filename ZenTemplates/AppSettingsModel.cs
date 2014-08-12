using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace ZenTemplates
{
	public class AppSettingsModel
	{
		public Node RootNode { get; set; }

		public AppSettingsModel(NameValueCollection settings)
		{
			RootNode = new Node();
			foreach (var key in settings.AllKeys)
			{
				string[] parts = key.Split('.');
				Node currentNode = RootNode;
				foreach (string part in parts)
				{
					if (!currentNode.ContainsKey(part))
					{
						currentNode[part] = new Node();
					}
					currentNode = currentNode[part];
				}
				currentNode.Value = settings[key];
			}
		}

		public AppSettingsModel()
			: this(ConfigurationManager.AppSettings)
		{ }
	}

	public class Node : Dictionary<string,Node>
	{
		public string Value { get; set; }
		public override string ToString()
		{
			return Value;
		}

		public static implicit operator string(Node node)
		{
			return node.ToString();
		}
	}
}
