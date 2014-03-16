using System;
using System.Text;
using System.Text.RegularExpressions;
using ZenTemplates.Parser.Context;

namespace ZenTemplates.Parser
{
	public class SubstitutionParser
	{
		public ILookupContext Lookup { get; set; }

		public SubstitutionParser(ILookupContext context)
		{
			Lookup = context;
		}

		public string Substitute(string input)
		{
			// Produces tokens of 
			string[] tokens = Regex.Split(input, @"(^|[^\\])(?:\\\\)*(\$\{[^}]+\})");

			if (tokens.Length == 0)
			{
				return "";
			}
			else if (tokens.Length == 1)
			{
				return tokens[0];
			}

			StringBuilder sb = new StringBuilder((int)(input.Length * 1.1));
			foreach (string token in tokens)
			{
				if (String.IsNullOrEmpty(token))
				{
					continue;
				}
				else if (token.StartsWith("${") && token.EndsWith("}"))
				{
					string key = token.Substring(2, token.Length - 3);
					var value = Lookup.GetProperty(key);
					if (value == null)
					{
						continue;
					}
					else
					{
						sb.Append(value.ToString());
					}
				}
				else
				{
					sb.Append(token);
				}
			}

			return sb.ToString();
		}
	}
}
