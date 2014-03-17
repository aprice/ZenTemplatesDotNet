using System;

namespace ZenTemplates.Parser.Context
{
	public interface ILookupContext
	{
		Object GetProperty(String key);
	}
}
