using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenTemplates.Parser;

namespace ZenTemplates.Test
{
	[TestClass]
	public class InferenceTest
	{
		[TestMethod]
		public void InferredInjectionByClassTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p class=""testData"">Test placeholder</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p class=""testData"">Test data</p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.Model["testData"] = "Test data";
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void InferredInjectionByIDTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p id=""testData"">Test placeholder</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p id=""testData"">Test data</p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.Model["testData"] = "Test data";
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		//[TestMethod]
		public void InferredDerivationTest()
		{
		}
	}
}
