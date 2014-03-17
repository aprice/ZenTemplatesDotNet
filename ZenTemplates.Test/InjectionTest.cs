using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenTemplates.Parser;

namespace ZenTemplates.Test
{
	[TestClass]
	public class InjectionTest
	{
		[TestMethod]
		public void SimpleInjectionByAttributeTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p data-z-inject=""testData"">Test placeholder</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test data</p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.Model["testData"] = "Test data";
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void ObjectInjectionByAttributeTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p data-z-inject=""testData"">Test <span data-z-inject=""property"">placeholder</span></p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test <span>data</span></p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.Model["testData"] = new { property = "data" };
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void MultiPartInjectionByAttributeTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test <span data-z-inject=""testData.property"">placeholder</span></p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test <span>data</span></p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.Model["testData"] = new { property = "data" };
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void InjectionOfMissingPropertyTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test <span data-z-inject=""testData.property"">placeholder</span></p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test <span></span></p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void SimpleListInjectionTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p data-z-inject=""testData"">Test placeholder</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>one</p><p>two</p><p>three</p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.Model["testData"] = new string[] { "one", "two", "three" };
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}
	}
}
