using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ZenTemplates.Parser;
using ZenTemplates.Parser.Context;

namespace ZenTemplates.Test
{
	[TestClass]
	public class SubstitutionTest
	{
		[TestMethod]
		public void SubstitutionParserTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test ${testData}</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test data</p></body></html>";

			IDictionary<string,object> model = new Dictionary<string,object>();
			model["testData"] = "data";
			ILookupContext context = new ModelContext(model);
			SubstitutionParser parser = new SubstitutionParser(context);
			string result = parser.Substitute(inHtml);
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void SimpleSubstitutionTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test ${testData}</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test data</p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.Model["testData"] = "data";
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void NestedSubstitutionTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p data-z-inject=""testData"">Test ${property}</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test data</p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.Model["testData"] = new { property = "data" };
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void MultiPartSubstitutionTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test ${testData.property}</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test data</p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.Model["testData"] = new { property = "data" };
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void SubstitutionOfMissingPropertyTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test ${testData.property}</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test </p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void EscapedSubstitutionTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test \${testData}</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test ${testData}</p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void UnEscapedSubstitutionTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test \\${testData}</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test \data</p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.Model["testData"] = "data";
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}

		[TestMethod]
		public void MultiEscapedSubstitutionTest()
		{
			string inHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test \\\${testData}</p></body></html>";
			string outHtml = @"<!DOCTYPE html>
<html><head><title>Test document</title></head>
<body><p>Test \${testData}</p></body></html>";

			TemplateParser parser = new TemplateParser();
			parser.LoadTemplateHtml(inHtml);
			parser.Render();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}
	}
}
