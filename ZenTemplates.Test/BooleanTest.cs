using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ZenTemplates.Parser;
using ZenTemplates.Parser.Context;

namespace ZenTemplates.Test
{
	[TestClass]
	public class BooleanTest
	{
		[TestMethod]
		public void TruthinessOfTrue()
		{
			Dictionary<string, object> model = new Dictionary<string, object>();
			model["testData"] = true;
			ILookupContext lookup = new ModelContext(model);
			BooleanParser parser = new BooleanParser(lookup);
			Assert.IsTrue(parser.Parse("testData"));
		}

		[TestMethod]
		public void TruthinessOfFalse()
		{
			Dictionary<string, object> model = new Dictionary<string, object>();
			model["testData"] = false;
			ILookupContext lookup = new ModelContext(model);
			BooleanParser parser = new BooleanParser(lookup);
			Assert.IsFalse(parser.Parse("testData"));
		}

		[TestMethod]
		public void TruthinessOfTrueString()
		{
			Dictionary<string, object> model = new Dictionary<string, object>();
			model["testData"] = "true";
			ILookupContext lookup = new ModelContext(model);
			BooleanParser parser = new BooleanParser(lookup);
			Assert.IsTrue(parser.Parse("testData"));
		}

		[TestMethod]
		public void TruthinessOfFalseString()
		{
			Dictionary<string, object> model = new Dictionary<string, object>();
			model["testData"] = "false";
			ILookupContext lookup = new ModelContext(model);
			BooleanParser parser = new BooleanParser(lookup);
			Assert.IsFalse(parser.Parse("testData"));
		}

		[TestMethod]
		public void TruthinessOfEmptyString()
		{
			Dictionary<string, object> model = new Dictionary<string, object>();
			model["testData"] = "";
			ILookupContext lookup = new ModelContext(model);
			BooleanParser parser = new BooleanParser(lookup);
			Assert.IsFalse(parser.Parse("testData"));
		}

		[TestMethod]
		public void TruthinessOfNull()
		{
			Dictionary<string, object> model = new Dictionary<string, object>();
			model["testData"] = null;
			ILookupContext lookup = new ModelContext(model);
			BooleanParser parser = new BooleanParser(lookup);
			Assert.IsFalse(parser.Parse("testData"));
		}

		[TestMethod]
		public void TruthinessOfZero()
		{
			Dictionary<string, object> model = new Dictionary<string, object>();
			model["testInt"] = 0;
			model["testDouble"] = 0.0;
			ILookupContext lookup = new ModelContext(model);
			BooleanParser parser = new BooleanParser(lookup);
			Assert.IsFalse(parser.Parse("testInt"));
			Assert.IsFalse(parser.Parse("testDouble"));
		}

		[TestMethod]
		public void TruthinessOfOne()
		{
			Dictionary<string, object> model = new Dictionary<string, object>();
			model["testInt"] = 1;
			model["testDouble"] = 1.0;
			ILookupContext lookup = new ModelContext(model);
			BooleanParser parser = new BooleanParser(lookup);
			Assert.IsTrue(parser.Parse("testInt"));
			Assert.IsTrue(parser.Parse("testDouble"));
		}
	}
}
