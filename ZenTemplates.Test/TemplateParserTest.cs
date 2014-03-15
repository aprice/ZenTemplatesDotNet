using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenTemplates.Parser;
using System.Collections.Generic;

namespace ZenTemplates.Test
{
	[TestClass]
	public class TemplateParserTest
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
			parser.HandleInjection();
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
			parser.HandleInjection();
			string result = parser.GetOutput();
			Assert.AreEqual(outHtml, result);
		}
	}
}
