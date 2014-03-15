using System;

namespace ZenTemplates.Parser.Context
{
	public interface ILookupContext
	{
		Object GetProperty(String key);
		bool HasProperty(String key);
		bool HasProperty(String key, bool checkRoot);
		bool LookupBoolean(String key);
	}
}
