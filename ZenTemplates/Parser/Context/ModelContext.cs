using System;
using System.Collections;
using System.Reflection;

namespace ZenTemplates.Parser.Context
{
	public class ModelContext : ILookupContext
	{
		private object CurrentNode;

		public ModelContext(object node)
		{
			CurrentNode = node;
		}

		public object GetProperty(string key)
		{
			return GetProperty(CurrentNode, key);
		}

		private object GetProperty(object node, string key)
		{
			if (String.IsNullOrEmpty(key))
			{
				return node;
			}

			string[] keyParts = key.Split(new char[] { '.' }, 2);
			string currentKey = keyParts[0];
			string remainderKey = keyParts.Length > 1 ? keyParts[1] : null;

			if (node == null || node is ValueType)
			{
				return null;
			}
			else if (node is IDictionary)
			{
				return GetProperty(((IDictionary)node)[currentKey], remainderKey);
			}
			else if (node is IList)
			{
				int idx;
				if (Int32.TryParse(currentKey, out idx))
				{
					return GetProperty(((IList)node)[idx], remainderKey);
				}
				else
				{
					return null;
				}
			}
			else
			{
				return GetProperty(GetFieldValue(node, currentKey), remainderKey);
			}
		}

		private static object GetFieldValue(object ob, string name)
		{
			if (ob != null)
			{
				Type type = ob.GetType();
				MemberInfo[] test = type.GetMembers();
				FieldInfo field = type.GetField(name);
				if (field != null)
				{
					return field.GetValue(ob);
				}
				else
				{
					PropertyInfo prop = type.GetProperty(name);
					if (prop != null)
					{
						return prop.GetValue(ob);
					}
				}
			}

			return null;
		}
	}
}
