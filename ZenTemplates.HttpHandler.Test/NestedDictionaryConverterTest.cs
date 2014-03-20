using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ZenTemplates.HttpHandler.Test
{
	[TestClass]
	public class NestedDictionaryConverterTest
	{
		[TestMethod]
		public void DictionaryConversionTest()
		{
			var json = @"{
	'prop1':'value1',
	'prop2':['item1','item2','item3'],
	'child':{
		'childProp1': 'childValue1',
		'childProp2': 'childValue2'
	}
}";
			var obj = JsonConvert.DeserializeObject<IDictionary<string, object>>(
				json, new JsonConverter[] { new NestedDictionaryConverter() });

			Assert.IsInstanceOfType(obj["child"], typeof(IDictionary<string,object>));
		}
	}
}
