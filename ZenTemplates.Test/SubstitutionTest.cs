using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenTemplates.Parser;

namespace ZenTemplates.Test
{
	[TestClass]
	public class SubstitutionTest
	{
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
			parser.HandleInjection();
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
			parser.HandleInjection();
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
			parser.HandleInjection();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}
	}
}
