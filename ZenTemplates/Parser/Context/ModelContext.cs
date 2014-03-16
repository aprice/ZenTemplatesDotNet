using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

		public bool HasProperty(string key)
		{
			return HasProperty(key, true);
		}

		public bool HasProperty(string key, bool checkRoot)
		{
			if (CurrentNode == null || CurrentNode is ValueType)
			{
				return false;
			}
			else if (CurrentNode is IDictionary)
			{
				return ((IDictionary)CurrentNode).Contains(key);
			}
			else if (CurrentNode is IList)
			{
				int idx;
				if (Int32.TryParse(key, out idx))
				{
					return ((IList)CurrentNode).Count > idx;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return HasField(CurrentNode, key);
			}
		}

		public bool LookupBoolean(string key)
		{
			throw new NotImplementedException();
		}

		private static bool HasField(object ob, string name)
		{
			if (ob == null)
			{
				return false;
			}
			else
			{
				Type type = ob.GetType();
				return (type.GetField(name) != null || type.GetProperty(name, BindingFlags.GetProperty) != null);
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
