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

		private static readonly Regex TokenRegex = new Regex(@"(^|[^\\])(\\\\)*(\$\{[^}]+\})");
		public string Substitute(string input)
		{
			if (String.IsNullOrEmpty(input))
			{
				return "";
			}

			// Produces tokens of 
			string[] tokens = TokenRegex.Split(input);

			if (tokens.Length == 1)
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
				else if (token.StartsWith("\\"))
				{
					sb.Append(token.Substring(0, token.Length / 2));
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

		private static readonly Regex EscapeRegex = new Regex(@"(\\+)\$\{");
		public static string ResolveEscapes(string input)
		{
			if (String.IsNullOrEmpty(input))
			{
				return "";
			}
			else
			{
				return EscapeRegex.Replace(input, EscapeMatchEvaluator);
			}
		}

		private static string EscapeMatchEvaluator(Match match)
		{
			string slashes = match.Groups[1].Value;
			StringBuilder result = new StringBuilder((slashes.Length / 2) + 2);
			result.Append(slashes.Substring(0, slashes.Length / 2));
			result.Append("${");
			return result.ToString();
		}
	}
}
