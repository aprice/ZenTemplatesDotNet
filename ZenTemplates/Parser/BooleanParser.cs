using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ZenTemplates.Parser.Context;

namespace ZenTemplates.Parser
{
	public class BooleanParser
	{
		public const string TrueString = "True";
		public const string FalseString = "False";
		public ILookupContext Lookup { get; set; }

		public BooleanParser(ILookupContext context)
		{
			Lookup = context;
		}

		private static readonly Regex GroupRegex = new Regex(@"(\(|\))");
		public bool Parse(string expression)
		{
			if (String.IsNullOrEmpty(expression))
			{
				return false;
			}

			string[] tokens = GroupRegex.Split(expression);
			if (tokens.Length == 1)
			{
				return IsTruthy(Lookup.GetProperty(expression));
			}
			else
			{
				List<string> tokenList = tokens.ToList();
				int openIdx, closeIdx;
				while ((openIdx = tokenList.LastIndexOf("(")) >= 0)
				{
					if ((closeIdx = tokenList.IndexOf(")", openIdx)) < 0)
					{
						// unmatched paren
						return false;
					}
					else
					{
						bool result = Evaluate(String.Join("", tokenList.GetRange(openIdx + 1, closeIdx - (openIdx + 1))));
						tokenList.RemoveRange(openIdx, (closeIdx - openIdx) + 1);
						tokenList.Insert(openIdx, result ? TrueString : "");
					}
				}

				return Evaluate(String.Join("", tokenList));
			}
		}

		private static readonly Regex ExpressionRegex = new Regex(@"(\|\||&&|!)");
		private bool Evaluate(string expression)
		{
			string[] tokens = ExpressionRegex.Split(expression);
			if (tokens.Length == 1)
			{
				return IsTruthy(Lookup.GetProperty(expression));
			}
			else
			{
				bool value = false, hasValue = false, notSwitch = false;
				string lastOperator = null;
				foreach (string token in tokens)
				{
					string trimmedToken = token.Trim();
					if (trimmedToken == "!")
					{
						notSwitch = !notSwitch;
					}
					else if (trimmedToken == "&&" || trimmedToken == "||")
					{
						lastOperator = trimmedToken;
					}
					else
					{
						bool resolvedValue = !notSwitch && (trimmedToken.Equals(TrueString, StringComparison.OrdinalIgnoreCase) || IsTruthy(Lookup.GetProperty(trimmedToken)));
						if (!hasValue)
						{
							value = resolvedValue;
							hasValue = true;
						}
						else if (lastOperator == "&&")
						{
							value = value && resolvedValue;
						}
						else if (lastOperator == "||")
						{
							value = value || resolvedValue;
						}
						else
						{
							throw new ArgumentException("Invalid boolean expression: '" + expression + "'.");
						}
					}
				}

				return value;
			}
		}

		private bool IsTruthy(object value)
		{
			double doubleVal;
			if (value == null)
			{
				return false;
			}
			if (value is bool)
			{
				return (bool)value;
			}
			else if (value is string)
			{
				return !String.IsNullOrWhiteSpace((string)value)
					&& !((string)value).Equals(FalseString, StringComparison.OrdinalIgnoreCase);
			}
			else if (TryConvertDouble(value, out doubleVal))
			{
				return doubleVal != 0.0d;
			}
			else
			{
				// object null => false handled above
				return true;
			}
		}

		private bool TryConvertDouble(object value, out double doubleValue)
		{
			if (value is sbyte
				|| value is byte
				|| value is short
				|| value is ushort
				|| value is int
				|| value is uint
				|| value is long
				|| value is ulong
				|| value is float
				|| value is double
				|| value is decimal)
			{
				doubleValue = Convert.ToDouble(value);
				return true;
			}
			else
			{
				doubleValue = 0.0d;
				return false;
			}
		}
	}
}
